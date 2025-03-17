using Xunit;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductViewModelTests
    {
        private List<ValidationResult> ValidateModel(ProductViewModel model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void ProductViewModel_ShouldHaveError_WhenNameIsMissing()
        {
            var model = new ProductViewModel { Name = "", Price = 10, Stock = 5 };
            var errors = ValidateModel(model);
            Assert.Contains(errors, v => v.ErrorMessage.Contains("MissingName"));
        }
      
        [Fact]
        public void ProductViewModel_ShouldHaveError_WhenPriceIsNotGreaterThanZero()
        {
            var model = new ProductViewModel { Name = "Valid Name", Price = 0, Stock = 5 };
            var errors = ValidateModel(model);
            Assert.Contains(errors, v => v.ErrorMessage.Contains("PriceNotGreaterThanZero"));
        }     

        [Fact]
        public void ProductViewModel_ShouldHaveError_WhenStockIsNotGreaterThanZero()
        {
            var model = new ProductViewModel { Name = "Valid Name", Price = 10, Stock = 0 };
            var errors = ValidateModel(model);
            Assert.Contains(errors, v => v.ErrorMessage.Contains("StockNotGreaterThanZero"));
        }
    }
}