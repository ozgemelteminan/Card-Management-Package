[HttpGet("generate/{transactionId}")]
public IActionResult GenerateQr(int transactionId)
{
    // Merchant kimliğini token'dan alıyoruz
    var merchantId = int.Parse(User.Claims.First(c => c.Type == "merchantId").Value);

    // Transaction gerçekten bu merchant'a mı ait kontrol ediyoruz
    var transaction = _db.Transactions
        .FirstOrDefault(t => t.TransactionId == transactionId && t.MerchantId == merchantId);

    if (transaction == null)
        return NotFound("Transaction bulunamadı veya size ait değil");

    // QR kod içine gömülecek payload
    var payload = new { transactionId = transaction.TransactionId };
    var qrText = System.Text.Json.JsonSerializer.Serialize(payload);

    // QRCoder ile QR verisi oluşturma
    using var qrGenerator = new QRCodeGenerator();
    using var qrData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);

    // Cross-platform QR kod PNG byte dizisi üretme
    var qrCode = new PngByteQRCode(qrData);
    var qrBytes = qrCode.GetGraphic(20);

    // Base64 olarak encode etme
    var base64Qr = Convert.ToBase64String(qrBytes);
    var dataUrl = $"data:image/png;base64,{base64Qr}";

    // JSON response içinde hem raw hem de HTML preview dönüyoruz
    return Ok(new
    {
        transaction.TransactionId,
        QrCodeBase64 = dataUrl,
        QrHtmlPreview = $"<html><body><img src='{dataUrl}' /></body></html>"
    });
}
