using System;

namespace EPOv2.UnitTest
{
    using System.Web.Mvc;

    using DomainModel.DataContext;

    using EPOv2.Business;
    using EPOv2.Business.Interfaces;
    using EPOv2.Controllers;
    using EPOv2.Repositories.Interfaces;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    

   // using Assert = NUnit.Framework.Assert;

    [TestClass]
    public class UnitTest1
    {
        private IData data;

        readonly Mock<IAd> _adMock = new Mock<IAd>();

        readonly Mock<IDataContext> _dataContext = new Mock<IDataContext>();
       
        [TestMethod]
        public void GetCorrectPeriod()
        {
            //Arrange
            data = new Data(_dataContext.Object, null, null,null,null,null,null,null,null,null,
                 null, null, null, null, null, null, null, null, null, null, 
                 null, null,null, null, null, null, null, null, null, null, null, 
                 null, null,null, null, null, null, null, null, null, null, null, 
                 null, null,null, null, _adMock.Object
                 );

            //HomeController controller = new HomeController(uiMock.Object, adMock.Object);

            //Act
            //var actual = controller.Index();

            //Assert
            Assert.AreEqual(7, data.GetCorrectPeriod(1));
            Assert.AreEqual(1, data.GetCorrectPeriod(7));
            Assert.AreEqual(12, data.GetCorrectPeriod(6));
        }

        [TestMethod]
        public void ConvertFinancialPeriodToCalendar()
        {
            //Arrange
            data = new Data(_dataContext.Object, null, null, null, null, null, null, null, null, null,
                 null, null, null, null, null, null, null, null, null, null,
                 null, null, null, null, null, null, null, null, null, null, null,
                 null, null, null, null, null, null, null, null, null, null, null,
                 null, null, null, null, _adMock.Object

             );
            //HomeController controller = new HomeController(uiMock.Object, adMock.Object);

            //Act
            //var actual = controller.Index();

            //Assert
            Assert.AreEqual(7, Data.ConvertFinancePeriodToCalendar(1));
            Assert.AreEqual(6, Data.ConvertFinancePeriodToCalendar(12));
            Assert.AreEqual(12, Data.ConvertFinancePeriodToCalendar(6));
        }
    }
}
