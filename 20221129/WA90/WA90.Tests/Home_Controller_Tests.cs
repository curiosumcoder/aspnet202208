using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WA90.Controllers;

namespace WA90.Tests
{
    public class Home_Controller_Tests
    {
        [Fact]
        public void Index_Retorna_ListaDeNúmeros()
        {
            // Arrange
            ILogger<HomeController> logger = Mock.Of<ILogger<HomeController>>();

            // Objeto simulado
            var mockNum = new Mock<Data.IGetList<int>>();
            mockNum.Setup(repo => repo.GetList())
                .Returns(new List<int>() { 2, 46, 3, 6, 9, 2, 34, 67, 89, 202, 304 });

            var controller = new HomeController(logger, mockNum.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<int>>(viewResult.Model);
            Assert.Equal(11, model.Count());
        }
    }
}
