using Azure;
using Azure.Data.Tables;
using System.Threading.Tasks;

namespace ABC_Retailers.Models
{
    // Service class to interact with Azure Table Storage for Customer profiles
    public class CustomerStorageService
    {
        private readonly TableClient _tableClient;

        // Constructor that initializes the TableClient with a connection string and table name
        public CustomerStorageService(string connectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        // Method to add or update a customer profile in the table storage
        public async Task AddOrUpdateProfileAsync(Customers profile)
        {
            
            await _tableClient.UpsertEntityAsync(profile);
        }

        // Method to retrieve a customer profile from the table storage by partition key and row key
        public async Task<Customers> GetProfileAsync(string partitionKey, string rowKey)
        {
            try
            {
                return await _tableClient.GetEntityAsync<Customers>(partitionKey, rowKey);
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }
    }
}

//code attribution: https://learn.microsoft.com/en-us/azure/storage/tables/table-storage-overview
// https://stackoverflow.com/questions/45325823/exploring-the-data-from-azure-table-storage-within-azure-cloud-itself
