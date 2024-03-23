using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication2.Data.Models;
using WebApplication2.Data;
using WebApplication2.Pages.Main;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Tests
{
    public class MenuAssetModelTests
    {
        [Fact]
        public async Task OnPostDelete_WithExistingId_RemovesAssetAndRedirectsToMenuPage()
        {
            // Arrange
            var asset = new Assets { id = 1, object_name = "Test Asset" };
            var mockContext = new Mock<IApplicationContext>();
            mockContext.Setup(c => c.Assets.FindAsync(1)).ReturnsAsync(asset);
            var pageModel = new MenuAssetModel(mockContext.Object);

            // Act
            var result = await pageModel.OnPostDelete(1);

            // Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Main/menu", redirectToPageResult.PageName);
            mockContext.Verify(c => c.Assets.Remove(asset), Times.Once);
            mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
        }
        [Fact]
        public async Task OnPostAsync_WithValidData_UpdatesAssetAndRedirectsToMenuPage()
        {
            // Arrange
            var asset = new Assets { id = 1, object_name = "Test Asset" };
            var mockContext = new Mock<IApplicationContext>();
            mockContext.Setup(c => c.Assets.FindAsync(1)).ReturnsAsync(asset);
            var pageModel = new MenuAssetModel(mockContext.Object);
            var updatedAsset = new Assets { id = 1, object_name = "Updated Asset" };

            // Act
            var result = await pageModel.OnPostAsync(1, "Updated Asset", "123456", "5", 2022, 1000.00m, 500.00m, 10, "Норм");

            // Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Main/menu", redirectToPageResult.PageName);
            mockContext.Verify(c => c.SaveChangesAsync(), Times.Once);
        }
    }
}
