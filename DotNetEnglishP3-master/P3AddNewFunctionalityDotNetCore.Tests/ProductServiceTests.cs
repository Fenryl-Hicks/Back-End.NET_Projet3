using Xunit;
using Moq;
using System.Collections.Generic;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using P3AddNewFunctionalityDotNetCore.Models;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ICart> _mockCart;
        private readonly Mock<IStringLocalizer<ProductService>> _mockLocalizer;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockCart = new Mock<ICart>();
            _mockLocalizer = new Mock<IStringLocalizer<ProductService>>();

            _productService = new ProductService(
                _mockCart.Object,
                _mockProductRepository.Object,
                _mockOrderRepository.Object,
                _mockLocalizer.Object
            );
        }        

        [Fact]
        public void GetAllProductsViewModel_ShouldReturn_ProductViewModelList()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Test Product", Price = 10.0, Quantity = 5, Description = "Test", Details = "Details" },
                new Product { Id = 2, Name = "Another Product", Price = 20.0, Quantity = 3, Description = "Another", Details = "Details" }
            };
            _mockProductRepository.Setup(repo => repo.GetAllProducts()).Returns(products);

            // Act
            var result = _productService.GetAllProductsViewModel();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Product", result[0].Name);
        }

        [Fact]
        public void GetProductById_ShouldReturn_CorrectProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Test Product", Price = 10.0, Quantity = 5, Description = "Test", Details = "Details" };
            _mockProductRepository.Setup(repo => repo.GetAllProducts()).Returns(new List<Product> { product });

            // Act
            var result = _productService.GetProductById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public void SaveProduct_ShouldThrowException_WhenProductIsInvalid()
        {
            // Arrange
            var invalidProduct = new ProductViewModel
            {
                Name = "",
                Price = -10,
                Stock = -5
            };

            // Act & Assert
            Assert.Throws<ValidationException>(() => _productService.SaveProduct(invalidProduct));
        }

        [Fact]
        public void SaveProduct_ShouldCall_RepositorySaveProduct()
        {
            // Arrange
            var productViewModel = new ProductViewModel
            {
                Name = "New Product",
                Price = 15.00,
                Stock = 10,
                Description = "Desc",
                Details = "Details"
            };

            // Act
            _productService.SaveProduct(productViewModel);

            // Assert
            _mockProductRepository.Verify(repo => repo.SaveProduct(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public void DeleteProduct_ShouldRemoveFromCartAndRepository()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product", Price = 10.0, Quantity = 5 };
            _mockProductRepository.Setup(repo => repo.GetAllProducts()).Returns(new List<Product> { product });
            _mockCart.Setup(cart => cart.AddItem(product, 1));

            // Act            
            _productService.DeleteProduct(product.Id);

            // Assert
            _mockCart.Verify(cart => cart.RemoveLine(It.IsAny<Product>()), Times.Once);
            _mockProductRepository.Verify(repo => repo.DeleteProduct(1), Times.Once);
        }
    }
}