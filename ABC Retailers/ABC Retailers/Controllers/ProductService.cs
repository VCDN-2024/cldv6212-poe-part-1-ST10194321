using ABC_Retailers.Models;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ABC_Retailers.Services
{
    // Service class responsible for managing products, including storing, retrieving,
    // and managing product images and cart operations with Azure Blob Storage and Azure Queues
    public class ProductService
    {
        private readonly string _filePath = "products.json";
        private readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=st10194321cloud;AccountKey=s38gZsc2VOtEo2wMNm5X3hl0Mb9f6R/fAmCI76nwgBgL7u4wZKMEP3STwU4uZ4mvGQvkClpqvG7R+AStyGuJZg==;EndpointSuffix=core.windows.net";
        private readonly string _containerName = "product-images";
        private readonly string _queueName = "cart";

        // Method to retrieve all products from the local JSON file
        public async Task<List<Product>> GetAllProductsAsync()
        {
            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<Product>>(json);
            }

            return new List<Product>();
        }

        // Method to add a new product to the list and store it in the local JSON file
        public async Task AddProductAsync(ProductViewModel model)
        {
            var products = await GetAllProductsAsync();

            
            string imageUrl = await UploadProductImageAsync(model.ImageFile);

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                ImageUrl = imageUrl
            };

            products.Add(product);

            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        // Method to upload a product image to Azure Blob Storage
        private async Task<string> UploadProductImageAsync(IFormFile imageFile)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }

        // Method to delete a product by its ID
        public async Task DeleteProductAsync(Guid productId)
        {
            var products = await GetAllProductsAsync();

            var productToDelete = products.FirstOrDefault(p => p.ProductId == productId);
            if (productToDelete != null)
            {
               
                await DeleteProductImageAsync(productToDelete.ImageUrl);

               
                products.Remove(productToDelete);

                
                var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, json);
            }
        }

        //Method to delete a product image from Azure Blob Storage
        private async Task DeleteProductImageAsync(string imageUrl)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            
            var blobName = Path.GetFileName(new Uri(imageUrl).LocalPath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

           
            await blobClient.DeleteIfExistsAsync();
        }

        //Method to add a product to the cart by adding it to an Azure Queue
        public async Task AddProductToCartAsync(Product product)
        {
            
            QueueClient queueClient = new QueueClient(_connectionString, _queueName);
            await queueClient.CreateIfNotExistsAsync();

           
            var message = JsonSerializer.Serialize(product);

           
            await queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message)));
        }

        //Method to retrieve all products currently in the cart (from the Azure Queue)
        public async Task<List<Product>> GetCartProductsAsync()
        {
           
            QueueClient queueClient = new QueueClient(_connectionString, _queueName);
            await queueClient.CreateIfNotExistsAsync();

            var cartProducts = new List<Product>();

            PeekedMessage[] messages = await queueClient.PeekMessagesAsync(maxMessages: 32);

            foreach (PeekedMessage message in messages)
            {
               
                var product = JsonSerializer.Deserialize<Product>(Convert.FromBase64String(message.MessageText));
                cartProducts.Add(product);
            }

            return cartProducts;
        }

        // Method to clear the queue after processing (if needed)
        public async Task ClearCartAsync()
        {
            QueueClient queueClient = new QueueClient(_connectionString, _queueName);
            await queueClient.CreateIfNotExistsAsync();

            
            await queueClient.ClearMessagesAsync();
        }
    }
}
//code from: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction
//https://medium.com/@isaacdcloudprof/azure-storage-accounts-a-step-by-step-guide-to-creating-a-blob-storage-in-azure-portal-58c6288a2f
//https://learn.microsoft.com/en-us/azure/storage/queues/storage-queues-introduction
//https://learn.microsoft.com/en-us/azure/storage/queues/storage-tutorial-queues

