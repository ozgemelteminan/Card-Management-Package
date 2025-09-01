namespace Merchant.API.Models
{


    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }=string.Empty;

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }
    }
    
        
            
            //Ürün ekleme
        public class CreateProductRequest
        {
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
        }


    //ürün güncelleme
    public class UpdateProductRequest
        {
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
        }
    }

