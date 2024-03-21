using Xunit;
using WebApplication2.Pages.Main;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Tests.Pages.Main
{

   
    public class MenuPageTests
    {
        [Fact]
        public async Task OnGetAsync_ReturnsPageResultWithAssets()
        {
            // Arrange
            var assets = new List<Assets> { new Assets { id = 1, object_name = "Test Asset" } };
            var mockContext = new Mock<IApplicationContext>();
            mockContext.Setup(c => c.GetAssetsAsync()).ReturnsAsync(assets);
            var pageModel = new MenuModel(mockContext.Object);

            // Act
            var result = await pageModel.OnGetAsync();

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.Equal(assets, pageModel.Assets);
        }

        [Fact]
        public async Task OnPostCreateExcel_ReturnsFileResult()
        {
            // Arrange
            var assets = new List<Assets> { new Assets { id = 1, object_name = "Test Asset" } };
            var mockContext = new Mock<IApplicationContext>();
            mockContext.Setup(c => c.GetAssetsAsync()).ReturnsAsync(assets);
            var pageModel = new MenuModel(mockContext.Object);

            // Act
            var result = pageModel.OnPostCreateExcelAsync();

            // Assert
            var fileResult = Assert.IsType<FileResult>(result);
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
            Assert.Equal("Assets.xlsx", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task OnPostSearchAsync_WithValidInventoryNumber_ReturnsPageResultWithFilteredAssets()
        {
            // Arrange
            var searchString = "123456";
            var inventoryNumber = 123456;
            var assets = new List<Assets> { new Assets { id = 1, object_name = "Test Asset", inventory_number = inventoryNumber } };
            var mockContext = new Mock<IApplicationContext>();
            mockContext.Setup(c => c.GetAssetsAsync()).ReturnsAsync(assets);
            var pageModel = new MenuModel(mockContext.Object);

            // Act
            var result = await pageModel.OnPostSearchAsync(searchString);

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.Single(pageModel.Assets);
            Assert.Equal(inventoryNumber, pageModel.Assets.First().inventory_number);
        }

        [Fact]
        public async Task OnPostSearchAsync_WithInvalidInventoryNumber_ReturnsPageResultWithAllAssets()
        {
            // Arrange
            var searchString = "invalid";
            var assets = new List<Assets> { new Assets { id = 1, object_name = "Test Asset" } };
            var mockContext = new Mock<IApplicationContext>();
            mockContext.Setup(c => c.GetAssetsAsync()).ReturnsAsync(assets);
            var pageModel = new MenuModel(mockContext.Object);

            // Act
            var result = await pageModel.OnPostSearchAsync(searchString);

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.Equal(assets, pageModel.Assets);
        }
       
    }
}