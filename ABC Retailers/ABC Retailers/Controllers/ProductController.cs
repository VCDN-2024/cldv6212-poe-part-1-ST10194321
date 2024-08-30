using ABC_Retailers.Models;
using ABC_Retailers.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ABC_Retailers.Controllers
{
    // Controller responsible for handling product-related actions
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        // Constructor that initializes the ProductService
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // HTTP POST action to handle the product creation form submission
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _productService.AddProductAsync(model);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // HTTP GET action to display a list of products
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // HTTP POST action to handle product deletion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // HTTP POST action to add a product to the cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid id)
        {
            var products = await _productService.GetAllProductsAsync();
            var product = products.FirstOrDefault(p => p.ProductId == id);

            if (product != null)
            {
                await _productService.AddProductToCartAsync(product);
                return RedirectToAction(nameof(Cart));
            }

            return NotFound();
        }

        // HTTP GET action to display the cart with added products
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var cartProducts = await _productService.GetCartProductsAsync();
            return View(cartProducts);
        }

        // HTTP POST action to clear all items from the cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            await _productService.ClearCartAsync();
            return RedirectToAction(nameof(Cart));
        }
    }
}
//code from: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction
//https://medium.com/@isaacdcloudprof/azure-storage-accounts-a-step-by-step-guide-to-creating-a-blob-storage-in-azure-portal-58c6288a2f
