using NUnit.Framework;
using EPOv2.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EPOv2.Business.Tests
{
    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;

    using global::Repositories;

    using Moq;

    [TestFixture()]
    public class RoutingTests
    {
        private IRouting _routing;

        readonly Mock<IAd> _adMock = new Mock<IAd>();

        readonly Mock<IDataContext> _dataContext = new Mock<IDataContext>();

        readonly Mock<IOrderRepository> _orderMock = new Mock<IOrderRepository>();
        readonly Mock<IApproverRepository> _approverMock = new Mock<IApproverRepository>();
        readonly Mock<IRouteRepository> _routeMock = new Mock<IRouteRepository>();
        readonly Mock<ICapexRouteRepository> _capexRouteMock = new Mock<ICapexRouteRepository>();
        readonly Mock<ICapexRepository> _capexMock = new Mock<ICapexRepository>();
        readonly Mock<ICapexApproverRepository> _capexApproverMock = new Mock<ICapexApproverRepository>();
        readonly Mock<IOrderItemRepository> _orderItemMock = new Mock<IOrderItemRepository>();

        public List<User> UserList;

        public List<CapexRoute> CapexRoutesList;
        [SetUp]
        public void SetUp()
        {
            _routing = new Routing(_dataContext.Object, null, null, null, null, null, _orderMock.Object, null, null, null,
                 null, null, null,_approverMock.Object, _routeMock.Object, null,_capexRouteMock.Object, _capexMock.Object, _capexApproverMock.Object, null,
                 null, null, null, null, _orderItemMock.Object);

            UserList =new List<User>()
                           {
                               new User()
                                   {
                                       EmployeeId = 217,
                                       UserInfo = new UserInfo() {Id = 1,EmployeeId = 217,FirstName = "Graeme", LastName = "Everingham"},
                                       Id = "e80873b5-087d-4f3c-bc14-c11e3ea215e6",
                                   },
                                new User()
                                   {
                                       EmployeeId = 193,
                                       UserInfo = new UserInfo() {Id = 1,EmployeeId = 193,FirstName = "Andrew", LastName = "Beard"},
                                       Id = "c5d14d8a-9cf8-4b22-8a7b-28c02a3c4085",
                                   },
                                new User()
                                   {
                                       EmployeeId = 307,
                                       UserInfo = new UserInfo() {Id = 1,EmployeeId = 307,FirstName = "Sam", LastName = "Robson"},
                                       Id = "f1ecb6a4-c342-46a2-a64a-9f2eb6dfd460",
                                   },
                                new User()
                                   {
                                       EmployeeId = 2123,
                                       UserInfo = new UserInfo() {Id = 1,EmployeeId = 2123,FirstName = "Michelle", LastName = "Fraser"},
                                       Id = "08913075-e703-4201-bff7-ccb0fc738f76",
                                   },
                           };

            var capexApproverLevel1 = new CapexApprover()
            {
                Id = 1,
                Division = null,
                Level = 1,
                Limit = 20000,
                Role = "",
                User = UserList.FirstOrDefault(x => x.EmployeeId == 2123),
            };
            var capexApproverLevel2 = new CapexApprover()
            {
                Id = 6,
                Division = null,
                Level = 2,
                Limit = 20000,
                Role = "",
                User = UserList.FirstOrDefault(x => x.EmployeeId == 217),
            };
            var capexApproverLevel3 = new CapexApprover()
            {
                Id = 2,
                Division = null,
                Level = 3,
                Limit = 20000,
                Role = "",
                User = UserList.FirstOrDefault(x => x.EmployeeId == 193),
            };
            var capexApproverLevel4 = new CapexApprover()
            {
                Id = 3,
                Division = null,
                Level = 4,
                Limit = 100000,
                Role = "",
                User = UserList.FirstOrDefault(x => x.EmployeeId == 217),
            };
            var capexApproverLevel5 = new CapexApprover()
            {
                Id =4,
                Division = null,
                Level = 5,
                Limit = 100000,
                Role = "",
                User = UserList.FirstOrDefault(x => x.EmployeeId == 307),
            };
            var capexApproverLevel6 = new CapexApprover()
            {
                Id = 5,
                Division = null,
                Level = 6,
                Limit = 9999999,
                Role = "",
                User = UserList.FirstOrDefault(x => x.EmployeeId == 217),
            };
            CapexRoutesList = new List<CapexRoute>()
                                      {
                                          new CapexRoute()
                                              {
                                                  Id = 1,
                                                  Number = 1,
                                                  IsDeleted = false,
                                                  Approver = capexApproverLevel1,
                                                  Capex = new Capex() {Id = 1,Total = 900000}
                                              },
                                          new CapexRoute()
                                              {
                                                  Id = 2,
                                                  Number = 2,
                                                  IsDeleted = false,
                                                  Approver = capexApproverLevel2,
                                                  Capex = new Capex() {Id = 1,Total = 900000}
                                              },
                                          new CapexRoute()
                                              {
                                                  Id = 3,
                                                  Number = 3,
                                                  IsDeleted = false,
                                                  Approver = capexApproverLevel3,
                                                  Capex = new Capex() {Id = 1,Total = 900000}
                                              },
                                      };

        }

        [Test()]
        public void ApproveCapexTest()
        {
            _capexRouteMock.Setup(x => x.Get().ToList()).Returns(CapexRoutesList);
            Assert.Fail();
        }
    }
}