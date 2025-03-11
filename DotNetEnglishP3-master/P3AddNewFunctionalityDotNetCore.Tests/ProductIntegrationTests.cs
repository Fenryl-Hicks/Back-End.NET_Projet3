using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests.Integration
{
    public class ProductIntegrationTests : IDisposable
    {
        private readonly P3Referential _context;
        private readonly ProductRepository _productRepository;
        private readonly ProductService _productService;
        private readonly ProductController _productController;

        public ProductIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<P3Referential>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestDatabase;Trusted_Connection=True;")
                .Options;

            _context = new P3Referential(options, null);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _productRepository = new ProductRepository(_context);
            var cart = new Cart();
            var orderRepo = new Mock<IOrderRepository>();
            var localizer = new Mock<IStringLocalizer<ProductService>>();
            _productService = new ProductService(cart, _productRepository, orderRepo.Object, localizer.Object);

            var languageService = new Mock<ILanguageService>();
            _productController = new ProductController(_productService, languageService.Object);
        }

        [Fact]
        public void FullProductWorkflow_ShouldWorkCorrectly()
        {
            var newProduct = new ProductViewModel
            {
                Name = "Produit Test",
                Description = "Description Test",
                Details = "Détails Test",
                Price = 50.0,
                Stock = 10
            };

            var createResult = _productController.Create(newProduct) as RedirectToActionResult;
            Assert.NotNull(createResult);
            Assert.Equal("Admin", createResult.ActionName);

            var products = _productService.GetAllProductsViewModel();
            Assert.Single(products);
            Assert.Equal("Produit Test", products[0].Name);
            Assert.Equal(50.0, products[0].Price);
            Assert.Equal(10, products[0].Stock);

            var productId = products[0].Id;
            var deleteResult = _productController.DeleteProduct(productId) as RedirectToActionResult;
            Assert.NotNull(deleteResult);
            Assert.Equal("Admin", deleteResult.ActionName);

            products = _productService.GetAllProductsViewModel();
            Assert.Empty(products);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}