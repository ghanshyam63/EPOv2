using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPOv2.Controllers;
namespace EPOv2.Tests
{
    using System.Web.Mvc;

    using EPOv2.Business.Interfaces;

    using Moq;

    using NUnit.Framework;

    

    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            //Arrange
            Mock<IUserInterface> uiMock = new Mock<IUserInterface>();
            Mock<IAd> adMock = new Mock<IAd>();
            
            HomeController controller = new HomeController(uiMock.Object,adMock.Object);

            //Act
            var actual = controller.Index();

            //Assert
            Assert.IsInstanceOf<ActionResult>(actual);
        }
    }
}
