using Azure;
using Azure.Data.Tables;

namespace ABC_Retailers.Models
{
    // Represents a customer entity in Azure Table Storage
    public class Customers : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } 
    }
}
