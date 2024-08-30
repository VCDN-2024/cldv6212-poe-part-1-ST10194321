
using ABC_Retailers.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace ABC_Retailers.Controllers
{
    // Controller responsible for handling user registration actions
    public class RegisterController : Controller
    {
        private readonly CustomerStorageService _customerStorageService;

        // Constructor that initializes the customer storage service
        public RegisterController(CustomerStorageService customerStorageService)
        {
            _customerStorageService = customerStorageService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // HTTP POST action to handle the registration form submission
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var profile = new Customers
                {
                    PartitionKey = "Customer", 
                    RowKey = model.Email, 
                    Email = model.Email,
                    PasswordHash = passwordHash
                };

                await _customerStorageService.AddOrUpdateProfileAsync(profile);
                return RedirectToAction("Login", "Login");
            }
            return View(model);
        }
    }
}
//code from: https://learn.microsoft.com/en-us/azure/storage/files/storage-files-introduction
// https://www.youtube.com/watch?v=BCzeb0IAy2k
// https://learn.microsoft.com/en-us/aspnet/mvc/overview/security/create-an-aspnet-mvc-5-web-app-with-email-confirmation-and-password-reset
// https://learn.microsoft.com/en-us/azure/storage/files/storage-files-active-directory-overview