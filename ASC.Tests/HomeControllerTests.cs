using System;
using ASC.Web.Configuration;
using ASC.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ASC.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> loggerMock;
        private readonly Mock<IOptions<ApplicationSettings>> optionsMock;

        public HomeControllerTests()
        {
            // Create an instance of Mock IOptions
            optionsMock = new Mock<IOptions<ApplicationSettings>>();
            // Set IOptions<> Values property to return ApplicationSettings object
            optionsMock.Setup(ap => ap.Value).Returns(new ApplicationSettings { ApplicationTitle = "ASC" });
            // Create an instance of Mock ILogger
            loggerMock = new Mock<ILogger<HomeController>>();
        }

        [Fact]
        public void HomeController_Index_View_Test()
        {
            // Home controller instantiated with Mock IOptions<> object
            var controller = new HomeController(loggerMock.Object, optionsMock.Object);

            Assert.IsType<ViewResult>(controller.Index());
        }

        [Fact]
        public void HomeController_Index_NoModel_Test()
        {
            var controller = new HomeController(loggerMock.Object, optionsMock.Object);
            // Assert Model for Null
            Assert.Null((controller.Index() as ViewResult).ViewData.Model);
        }

        [Fact]
        public void HomeController_Index_Validation_Test()
        {
            var controller = new HomeController(loggerMock.Object, optionsMock.Object);
            // Assert ModelState Error Count to 0
            Assert.Equal(0, (controller.Index() as ViewResult).ViewData.ModelState.ErrorCount);
        }
    }
}
