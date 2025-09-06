namespace MerchantApp.Service.Utils
{
    public static class TransactionConstants
    {
        // Represents a transaction that is created but not yet completed.
        public const string StatusPending = "Pending";

        // Represents a transaction that has been successfully completed.
        public const string StatusSuccess = "Success";

        // Represents a transaction that has failed.
        public const string StatusFailed = "Failed";

        // Represents a transaction that did not complete within the allowed time.
        public const string StatusTimeout = "Timeout";
    }
}
