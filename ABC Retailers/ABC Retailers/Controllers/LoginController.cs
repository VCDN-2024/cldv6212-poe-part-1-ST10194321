
using ABC_Retailers.Models;
using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ABC_Retailers.Controllers
{
    // Controller responsible for handling login actions
    public class LoginController : Controller
    {
        private readonly CustomerStorageService _customerStorageService;
        private readonly string _fileShareConnectionString = "DefaultEndpointsProtocol=https;AccountName=st10194321cloud;AccountKey=s38gZsc2VOtEo2wMNm5X3hl0Mb9f6R/fAmCI76nwgBgL7u4wZKMEP3STwU4uZ4mvGQvkClpqvG7R+AStyGuJZg==;EndpointSuffix=core.windows.net";
        private readonly string _fileShareName = "logreport";

        // Constructor that initializes the customer storage service
        public LoginController(CustomerStorageService customerStorageService)
        {
            _customerStorageService = customerStorageService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // HTTP POST action to process the login form submission
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var profile = await _customerStorageService.GetProfileAsync("Customer", model.Email);

                if (profile != null && BCrypt.Net.BCrypt.Verify(model.Password, profile.PasswordHash))
                {
                    
                    await LogUserLoginAsync(model.Email);

                   
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(model);
        }

        // Private method to log user login events to Azure File Share
        private async Task LogUserLoginAsync(string email)
        {
            var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"User: {email} logged in at {timeStamp}\n";

            var shareClient = new ShareClient(_fileShareConnectionString, "reportlog");
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient("login_log.txt");

            
            string existingContent = string.Empty;
            if (await fileClient.ExistsAsync())
            {
                var download = await fileClient.DownloadAsync();
                using (var reader = new StreamReader(download.Value.Content))
                {
                    existingContent = await reader.ReadToEndAsync();
                }
            }

            existingContent += logEntry;

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(existingContent)))
            {
                await fileClient.CreateAsync(memoryStream.Length);
                await fileClient.UploadRangeAsync(
                    new Azure.HttpRange(0, memoryStream.Length),
                    memoryStream);
            }
        }




    }
}
//code from: https://learn.microsoft.com/en-us/azure/storage/files/storage-files-introduction
// https://www.youtube.com/watch?v=BCzeb0IAy2k
// https://learn.microsoft.com/en-us/aspnet/mvc/overview/security/create-an-aspnet-mvc-5-web-app-with-email-confirmation-and-password-reset
// https://learn.microsoft.com/en-us/azure/storage/files/storage-files-active-directory-overview