using System.Collections.Generic;
using System.Linq;

using DomainModel.Entities;

using EPOv2.ViewModels;

namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using CsQuery.Utility;

    using DomainModel.Entities;
    using DomainModel.Enums;

    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;

    public partial class Data : IData
    {
        public const int PeriodsQty = 12;

        public int BudgetYear = 2015;
        public OwnerViewModel GetOwnerViewModel(CostCentre cc, vRockyEmployees employeesData)
        {
            //var routing = new Routing();
            var ccOwner = new OwnerViewModel()
                              {
                                  Id = cc.Owner.Id,
                                  EmpId = cc.Owner.UserInfo.EmployeeId,
                                  Name = cc.Owner.UserInfo.FirstName + " " + cc.Owner.UserInfo.LastName,
                                  CostCentre = cc,
                                  UserName = cc.Owner.UserName,
                                  User = cc.Owner
                              };
            var levelData = employeesData;//_routing.GetRockieLevelData(ccOwner.EmpId.ToString());
            ccOwner.Level = Convert.ToInt32(levelData.Level);
            ccOwner.Limit = _levelRepository.Get(x => x.Code == ccOwner.Level).Select(x => x.Value).FirstOrDefault();
            return ccOwner;
        }



        public RoutingStructureElement GetRSElement(string empId, int pseudoLevel, vRockyEmployees employeesData)
        {
            var element = new RoutingStructureElement()
                              {
                                  EmpId =
                                      Convert.ToInt32(
                                          empId.Replace(pseudoLevel.ToString() + ":", "")),
                                  PseudoLevel = pseudoLevel
                              };
            //var employeeId = element.EmpId.ToString();
            element.FullName =
                _userRepository.Get(x => x.EmployeeId == element.EmpId)
                    .Select(x => new { fullName = x.UserInfo.FirstName + " " + x.UserInfo.LastName })
                    .Select(x => x.fullName)
                    .FirstOrDefault();
            var empData = employeesData;//_routing.GetRockieLevelData(element.EmpId.ToString());
            element.Level = Convert.ToInt32(empData.Level);
            if (!string.IsNullOrEmpty(empData.ManagerEmpNo))
            {
                element.ManagerEmpId = Convert.ToInt32(empData.ManagerEmpNo);
            }
            element.Limit =
                _levelRepository.Get(x => x.Code == element.Level && !x.IsDeleted).Select(x => x.Value).FirstOrDefault();
            if (String.IsNullOrEmpty(element.FullName))
            {
                element.FullName = "!" + empData.FirstName + " " + empData.Surname;
            }
            return element;
        }

        public AdminStatsViewModel GetAdminStats()
        {
            var model = new AdminStatsViewModel();
            var dateLaunch = Convert.ToDateTime("29-06-2015");
            var daysFromLaunch = (DateTime.Now - dateLaunch).Days+1;
            var raisedOrder = _orderRepository.Get(x =>x.DateCreated>=dateLaunch && !x.IsDeleted && x.Author != null).ToList();
            model.RaisedOrders = raisedOrder.Count();
            model.ApptovedOrders = raisedOrder.Count(x => x.Status.Name == "Approved");
            model.ClosedOrders = raisedOrder.Count(x => x.Status.Name == "Closed");
            model.MatchedOrders = raisedOrder.Count(x => x.Status.Name == "Closed" || x.Status.Name == "Receipt in Full" || x.Status.Name == "Receipt Partial");
            model.OrdersPerDay =model.RaisedOrders / Convert.ToDouble(daysFromLaunch);
            model.OrdersPerWeek = model.RaisedOrders /( Convert.ToDouble(daysFromLaunch / 7));
            model.TotalOrdersAmount = raisedOrder.Select(x => x.Total).Sum();
            model.AveOrdersAmount = model.TotalOrdersAmount / model.RaisedOrders;
            var spender = raisedOrder.GroupBy(x =>x.Author).Select(x =>new { FullName= x.Key.UserInfo.FirstName+" "+ x.Key.UserInfo.LastName, total = x.Sum(y=>y.Total), qty = x.Count()}).OrderByDescending(x=>x.total).First();
            model.TopSpender = spender.FullName;
            model.TopSpenderAmount = spender.total;
            model.TopSpenderQty = spender.qty;

            var raiser = raisedOrder.GroupBy(x => x.Author).Select(x => new { FullName = x.Key.UserInfo.FirstName + " " + x.Key.UserInfo.LastName, total = x.Sum(y => y.Total), qty = x.Count() }).OrderByDescending(x => x.qty).First();
            model.TopRaiser = raiser.FullName;
            model.TopRaiserAmount = raiser.total;
            model.TopRaiserQty = raiser.qty;

            var vouchers = _voucherRepository.Get(x => !x.IsDeleted && x.DateCreated >= dateLaunch).ToList();
            model.ScannedVouchers = vouchers.Count();
            model.AuthorisedVouchers = vouchers.Count(x => x.Status.Name == "Authorised");
            return model;
        }

        public static string RenderPartialViewToString<T>(Controller controller, string viewName, T model)
        {
            controller.ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, null);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.ToString();
            }
        }

        public ReportEPOAccuralsFilterViewModel GetReportEPOAccuralsFilterViewModel()
        {

            var financialCalendarList =
                _financialCalendarRepository.Get(x => x.FinancialStartingDate >= _launchAppDate).ToList();
            var model = new ReportEPOAccuralsFilterViewModel()
                            {
                                FinancialYearList =
                                    financialCalendarList.GroupBy(x => x.FinancialYear).Select(x => new { Id = x.Key}).Select(x=>x.Id).ToList(),
                                FinancialPeriodList =
                                    financialCalendarList.GroupBy(x => x.FinancialPeriod).Select(x =>new { Id =  x.Key}).Select(x=>x.Id).ToList()
                            };
            return model;
        }

        public List<EPOAccuralReportViewModel> RunEPOAccuralsReport(ReportEPOAccuralsFilterViewModel filter)
        {
            var list = new List<EPOAccuralReportViewModel>();
            var financialYearList = new List<vBasedataFinancialCalendar>();
            var fyStart =DateTime.Now;
            var fyEnd = DateTime.Now;
            if (!string.IsNullOrEmpty(filter.ReceiptDate))
            {//TODO: trick it's not really receipt date it's order date. Need to check with Luz.
                fyStart = Convert.ToDateTime(filter.ReceiptDate);
                fyEnd = fyStart;
            }
            if (!string.IsNullOrEmpty(filter.SelectedFY))
            {
                financialYearList = !string.IsNullOrEmpty(filter.SelectedFinancialPeriod)
                                        ? _financialCalendarRepository.Get(
                                            x =>
                                            x.FinancialYear == filter.SelectedFY
                                            && x.FinancialPeriod == filter.SelectedFinancialPeriod).ToList()
                                        : _financialCalendarRepository.Get(
                                            x => x.FinancialYear == filter.SelectedFY).ToList();

                fyStart = financialYearList.Min(x => x.FinancialStartingDate).Date;
                fyEnd = financialYearList.Max(x => x.FinancialEndingDate).Date;
            }
            var listOrder =
                    _orderRepository.Get(
                        x =>
                        x.OrderDate >= fyStart && x.OrderDate <= fyEnd && !x.IsDeleted
                        && (x.Status.Name == StatusEnum.Receipt_Partial.ToString().Replace("_", " ")
                            || x.Status.Name == StatusEnum.Receipt_in_Full.ToString().Replace("_", " ")
                            || x.Status.Name == StatusEnum.Invoiced.ToString())).OrderBy(x=>x.OrderNumber).ToList();

            var attachmentsIDList =
                _voucherDocumentRepository.Get(
                    x =>
                    !x.IsDeleted
                    && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " "))
                    .Select(x => x.Reference)
                    .ToList();
            listOrder = listOrder.Where(x => !attachmentsIDList.Contains(x.Id.ToString())).ToList();
                foreach (var order in listOrder)
                {
                    var supplierName =
                        _oldDb.tblSuppliers.Where(x => x.SupplierID == order.SupplierId)
                            .Select(x => new { Name = x.SupplierName + " - " + x.SupplierCode })
                            .Select(x => x.Name)
                            .FirstOrDefault();
                    foreach (var item in order.OrderItems)
                    {
                        var reportItem = new EPOAccuralReportViewModel()
                                             {
                                                 Status = order.Status.Name,
                                                 OrderNumber = order.OrderNumber,
                                                 OrderId = order.Id,
                                                 EntityName = order.Entity.Name,
                                                 SupplierName = supplierName,
                                                 TotalExGST = item.Currency.Id==1 ? item.TotalExTax : item.TotalExTax*item.CurrencyRate,
                                                 AccountCode = item.Account.Code,
                                                 CostCentreCode = order.CostCentre.Code,
                                                 OrderQty = item.Qty,
                                                 CurrencySym=item.Currency.Name,
                                                 CurrencyRate=item.CurrencyRate,
                                                 ReceiptQty =
                                                     _matchOrderRepository.Get(
                                                         x =>
                                                         x.OrderItem.Id == item.Id
                                                         && !x.IsDeleted)
                                                         .Sum(x => (double?)x.Qty) ?? 0,
                                             };
                        reportItem.AwaitingQty = reportItem.OrderQty - reportItem.ReceiptQty;
                        reportItem.AwaitingTotalExGST = item.Currency.Id==1 ? item.UnitPrice * reportItem.AwaitingQty : item.UnitPrice *item.CurrencyRate * reportItem.AwaitingQty;
                    list.Add(reportItem);

                    }
                }
            
            
            //var orderList 

            return list;
        }

        public List<CostCentreViewModel> CheckCostCentreOwners()
        {
            var list = _costCentreRepository.Get(x =>!x.IsDeleted).Include(x=>x.Owner.UserInfo).ToList();
            var returnList = new List<CostCentreViewModel>();
            FillCostCentreOwnerViewModelList(list, returnList);
            return returnList.OrderBy(x=>x.Code).ToList();
        }

        public BudgetReportEntryDataViewModel GetBudgetReportEntryDataViewModel(int entityId = 0, int costCentreId = 0, int categoryId = 0)
        {
            var model = new BudgetReportEntryDataViewModel()
                            {
                                AccountCategories = entityId != 0 ? GetAccountCategoryList() : new List<AccountCategory>(),
                                CostCentres = entityId != 0 ? GetCostCentresVM(entityId) : new List<CostCentreViewModel>(),
                                Entities = GetEntities(),
                                SelectedCategory = categoryId,
                                SelectedCostCenter = costCentreId,
                                SelectedEntity = entityId
                            };
            return model;
        }

        public BudgetReportResult GetBudgetReport(BudgetReportEntryDataViewModel filter)
        {
            var today = DateTime.Today;
            var forecastYear = today.Year;
            var epoYear = today.Year;
            if (today.Month > 6) {
                epoYear += 1;
                forecastYear += 1;
            }
            
            var model = new BudgetReportResult()
                            {
                                EPOApprovedNotReceipted = new List<BudgetReportItem>(),
                                EstimateEPOSpend = new List<BudgetReportItem>(),
                                Reforecast = new List<BudgetReportItem>(),
                                Variance = new List<BudgetReportItem>(),
                                Filter = filter
                            };
            try
            {
                model.EstimateEPOSpend = GetEstimateEPOSpendOrEPOApproved(filter, new List<string>() { GetStatusName(StatusEnum.Receipt_in_Full), GetStatusName(StatusEnum.Receipt_Partial), GetStatusName(StatusEnum.Closed) }, epoYear);
                model.EPOApprovedNotReceipted = GetEstimateEPOSpendOrEPOApproved(filter, new List<string>() { GetStatusName(StatusEnum.Approved) }, epoYear, false);
            }
            catch (Exception exception)
            {
                _logger.Error(exception,"GetBudgetReport");
                model.isError = true;
            }
            
            return model;
        }

        public BudgetReportResult GetReForecastAndVarianceForBudgetReport(BudgetReportResult inputData)
        {
            var today = DateTime.Today;
            var forecastYear = today.Year;
            var epoYear = today.Year;
            if (today.Month >= 7)
            {
                epoYear += 1;
                forecastYear += 1;
            }

            var model = new BudgetReportResult()
            {
                EPOApprovedNotReceipted = null,
                EstimateEPOSpend = null,
                Reforecast = new List<BudgetReportItem>(),
                Variance = new List<BudgetReportItem>()
            };
            //model.EstimateEPOSpend = GetEstimateEPOSpendOrEPOApproved(inputData.Filter, new List<string>() { GetStatusName(StatusEnum.Receipt_in_Full), GetStatusName(StatusEnum.Receipt_Partial) }, epoYear);
            StartWatch();
            model.Reforecast = GetReforecastForBudgetReport(inputData.Filter, forecastYear);
            _logger.Information($"GetBudgetReport.GetReforecastForBudgetReport: time ~ {Stopwatch()}");
            StartWatch();
            model.Variance = GetVarianceForBudgetReport(inputData.EstimateEPOSpend, model.Reforecast);
            _logger.Information($"GetBudgetReport.GetVarianceForBudgetReport: time ~ {Stopwatch()}");


            return  model;
        }
        

        public UnrecieptedPOReportViewModel GetUnrecieptedPO(DateTime today)
        {
            var model = new UnrecieptedPOReportViewModel() { Items = new List<UnrecieptedPOItem>() };
            var orderList = _orderRepository.Get(x => !x.IsDeleted && (x.Status.Id == 3 || x.Status.Id == 8)).Include(x=>x.ReceiptGroup).OrderBy(x=>x.OrderNumber).ToList();
            foreach (var order in orderList)
            {
                var item = new UnrecieptedPOItem()
                               {
                                   Id = order.Id,
                                   LastModified = order.LastModifiedDate,
                                   OrderNumber = order.OrderNumber,
                                   CostCentre = order.CostCentre.GetFullName(),
                                   Author = order.Author.GetFullName(),
                                   ReceiptGroup = order.ReceiptGroup.Name,
                                   Status = order.Status.Name,
                                   Total = order.Total,
                                   SupplierName =
                                       _oldDb.tblSuppliers.Where(
                                           x => x.SupplierID == order.SupplierId)
                                       .Select(
                                           x => new { Name = x.SupplierName + " - " + x.SupplierCode })
                                       .Select(x => x.Name)
                                       .FirstOrDefault(),
                                   Recievers = new List<string>()
                               };
                var rList =_groupMemberRepository.Get(x => x.Group.Id == order.ReceiptGroup.Id).Select(x => x.User).ToList();
                foreach (var user in rList)
                {
                    item.Recievers.Add(user.GetFullName());
                }
                model.Items.Add(item);
            }
            return model;
        }

       

        public int GetCorrectPeriod(int calendarPeriod)
        {
            if (calendarPeriod > 6) return calendarPeriod - 6;
            return calendarPeriod + 6;
        }

        public static int ConvertFinancePeriodToCalendar(int financePeriod)
        {
            if (financePeriod > 6) return financePeriod - 6;
            return financePeriod + 6;
        }

        private List<BudgetReportItem> GetVarianceForBudgetReport(IEnumerable<BudgetReportItem> estimateEpoSpend, List<BudgetReportItem> reforecast)
        {
            var list = new List<BudgetReportItem>();
            try
            {
                foreach (var epoItem in estimateEpoSpend)
                {
                    var totalByPeriod = new double[13];
                    var totalByAccount = new double();
                    for (var period = 1; period <= PeriodsQty; period++)
                    {
                        var per = GetCorrectPeriod(period);
                        var forecastTotal =
                            reforecast.Where(
                                x =>
                                x.GLCode == epoItem.GLCode && x.Category == epoItem.Category
                                && x.AccountName == epoItem.AccountName).Select(x => x.Period[per]).FirstOrDefault();
                        totalByPeriod[per] = forecastTotal - epoItem.Period[per];
                        totalByAccount += totalByPeriod[per];
                    }
                    var item = new BudgetReportItem()
                    {
                        Category = epoItem.Category,
                        AccountName = epoItem.AccountName,
                        GLCode = epoItem.GLCode,
                        Period = totalByPeriod,
                        Total = totalByAccount
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception,"GetVarianceForBudgetReport");
            }
            return list;
        }

        private List<BudgetWithVoucherReportItem> GetVarianceForBudgetWithVouchersReport(IEnumerable<BudgetWithVoucherReportItem> estimateEpoSpend, IEnumerable<BudgetWithVoucherReportItem> vouchers, IEnumerable<BudgetWithVoucherReportItem> reforecast)
        {
            var list = new List<BudgetWithVoucherReportItem>();
            try
            {
                foreach (var epoItem in estimateEpoSpend)
                {
                    var totalByPeriod = new double[13];
                    var totalByCategory = new double();
                    for (var period = 1; period <= PeriodsQty; period++)
                    {
                        var per = GetCorrectPeriod(period);
                        var forecastTotal =
                            reforecast.Where(x =>x.CategoryId == epoItem.CategoryId).Select(x => x.Period[per]).FirstOrDefault();
                        var vouchersTotal = vouchers.Where(x => x.CategoryId == epoItem.CategoryId).Select(x => x.Period[per]).FirstOrDefault();
                        totalByPeriod[per] = forecastTotal - (epoItem.Period[per] + vouchersTotal);
                        totalByCategory += totalByPeriod[per];
                    }
                    var item = new BudgetWithVoucherReportItem()
                    {
                        Category = epoItem.Category,
                        CategoryId = epoItem.CategoryId,
                        Period = totalByPeriod,
                        Total = totalByCategory
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "GetVarianceForBudgetWithVouchersReport");
            }
            return list;
        }

        private List<BudgetReportItem> GetReforecastForBudgetReport(BudgetReportEntryDataViewModel filter, int year)
        {
            var list = new List<BudgetReportItem>();
            try
            {
                var entity = _entityRepository.Find(filter.SelectedEntity);
                var cc = _costCentreRepository.Find(filter.SelectedCostCenter);
                var category = _accountCategoryRepository.Find(filter.SelectedCategory);
                var accountList = category != null ? _accountToCategoryRepository.Get(x => !x.IsDeleted && x.Category.Id == filter.SelectedCategory).Select(x => x.Account).ToList() : _accountToCategoryRepository.Get(x => !x.IsDeleted).Select(x => x.Account).ToList();
                var accountIdList = accountList.Select(x => x.Code).ToList();
                var listForecast =
                    _glBalanceWithBudgetRepository.Get(
                        x =>
                        x.GLYear == year && x.EntityCode == entity.CodeNumber.ToString() && x.CostCentreCode == cc.Code.ToString()
                        && accountIdList.Contains(x.AccountCode)).ToList();

                foreach (var account in accountList)
                {
                    category =
                        _accountToCategoryRepository.Get(x => x.Account.Id == account.Id)
                            .Select(x => x.Category)
                            .FirstOrDefault();
                    var totalByPeriod = new double[13];
                    var totalByAccount = new double();
                    for (var period = 1; period <= PeriodsQty; period++)
                    {
                        var per = period;//GetCorrectPeriod(period);
                        var forecastsForPeriod =
                            listForecast.FirstOrDefault(x => x.GLPeriod == per && x.AccountCode == account.Code);
                        if (forecastsForPeriod != null)
                        {
                            totalByPeriod[per] = Convert.ToDouble(forecastsForPeriod.BudgetPeriod);
                            totalByAccount += Convert.ToDouble(forecastsForPeriod.BudgetPeriod);
                        }


                    }
                    var item = new BudgetReportItem()
                    {
                        Category = category != null ? category.Name : string.Empty,
                        AccountName = account.Name,
                        GLCode = account.Code,
                        Period = totalByPeriod,
                        Total = totalByAccount
                    };
                    if (item.Total > 0) list.Add(item);
                }
                if (filter.SelectedCategory == 0) list = MakeCategoryTotals(list);
                list = MakeOverallTotals(list);
            }
            catch (Exception exception)
            {
                _logger.Error(exception,$"GetReforecastForBudgetReport({JSON.ToJSON(filter)})");
            }
            return list;
        }

        

        private List<BudgetReportItem> GetEstimateEPOSpendOrEPOApproved(BudgetReportEntryDataViewModel filter, ICollection<string> statusList,int epoYear, bool isEstimate=true )
        {
            var list = new List<BudgetReportItem>();
            try
            {
                var category = filter.SelectedCategory!=0 ?  _accountCategoryRepository.Get(x=>x.Id ==filter.SelectedCategory).FirstOrDefault() : null;
                var accountList = category != null ? _accountToCategoryRepository.Get(x => !x.IsDeleted && x.Category.Id == filter.SelectedCategory && !x.Account.IsDeleted).Select(x => x.Account).ToList()
                                                 : _accountToCategoryRepository.Get(x => !x.IsDeleted && !x.Account.IsDeleted).Select(x => x.Account).ToList();
                var accountIdList = accountList.Select(x => x.Id).ToList();
                var listEPO =
                    _orderRepository.Get(
                        x =>
                        !x.IsDeleted
                        && statusList.Contains(x.Status.Name)
                        && x.Entity.Id == filter.SelectedEntity && x.CostCentre.Id == filter.SelectedCostCenter && x.OrderItems.Any(y => accountIdList.Contains(y.Account.Id)))
                        .Include(x=>x.OrderItems.Select(c=>c.Account))
                        .ToList();
                
                    var categoryList = category==null ?
                    _accountToCategoryRepository.Get(x => accountIdList.Contains(x.Account.Id))
                        .Include(x => x.Category)
                        .Include(x=>x.Account)
                        .ToList() : null;
               
                foreach (var account in accountList)
                {
                    category = categoryList!=null ? categoryList.Where(x => x.Account.Id == account.Id).Select(x => x.Category).FirstOrDefault() : category;

                    var totalByPeriod = new double[13];
                    var totalByAccount = new double();
                    for (var calPeriod = 1; calPeriod <= PeriodsQty; calPeriod++)
                    {
                        var year = epoYear;
                        var finPeriod = GetCorrectPeriod(calPeriod);
                        if (finPeriod <= 7) year = epoYear - 1;
                        var period = calPeriod;
                        var ordersForPeriod = isEstimate ?
                            listEPO.Where(
                                x => x.LastModifiedDate.Month == period && x.LastModifiedDate.Year == year) : listEPO;
                        var oItems = new List<OrderItem>();

                        foreach (var order in ordersForPeriod)
                        {
                            oItems.AddRange(isEstimate ? order.OrderItems.Where(x => !x.IsDeleted && x.Account.Id == account.Id)
                                                        : order.OrderItems.Where(x => !x.IsDeleted && x.Account.Id == account.Id && x.DueDate.Year == year && x.DueDate.Month == calPeriod));
                        }

                        var periodTotal = oItems.Sum(x => x.TotalExTax);
                        totalByPeriod[finPeriod] = periodTotal;
                        totalByAccount += periodTotal;
                    }
                    var item = new BudgetReportItem()
                    {
                        Category = category?.Name,
                        AccountName = account.Name,
                        GLCode = account.Code,
                        Period = totalByPeriod,
                        Total = totalByAccount
                    };

                    if (item.Total > 0) list.Add(item);
                }
                if (filter.SelectedCategory == 0) list = MakeCategoryTotals(list);
                list = MakeOverallTotals(list);
            }
            catch (Exception exception)
            {
                _logger.Error(exception,"GetEstimateEPOSpendOrEPOApproved");
            }

           return list;
        }

       
        public BudgetWithVoucherReportResult GetBudgetWithVouchersReport(BudgetReportEntryDataViewModel filter)
        {
            var today = DateTime.Today;
            var forecastYear = today.Year;
            var epoYear = today.Year;
            if (today.Month > 6)
            {
                epoYear += 1;
                forecastYear += 1;
            }

            var model = new BudgetWithVoucherReportResult()
            {
                EstimateEPOSpend = new List<BudgetWithVoucherReportItem>(),
                Reforecast = new List<BudgetWithVoucherReportItem>(),
                Variance = new List<BudgetWithVoucherReportItem>(),
                Vouchers = new List<BudgetWithVoucherReportItem>()
            };
            try
            {
                List<AccountToCategory> accountToCategoryList;
                List<Order> listEPO;
                var entity = _entityRepository.Find(filter.SelectedEntity);
                var cc = _costCentreRepository.Find(filter.SelectedCostCenter);
                PrepareDataForBudgetReport(filter, new List<string>() { GetStatusName(StatusEnum.Receipt_in_Full), GetStatusName(StatusEnum.Receipt_Partial)}, out accountToCategoryList, out listEPO);
                model.EstimateEPOSpend = GetEstimateEPOSpend(accountToCategoryList, listEPO, epoYear);
                model.Reforecast = GetReforecastForBudgetWithVouchersReport(accountToCategoryList, forecastYear, entity,cc);
                model.Vouchers = GetVouchersForBudgetWithVouchersReport(accountToCategoryList, epoYear,entity.Id,cc.Id);
                model.Variance = GetVarianceForBudgetWithVouchersReport(model.EstimateEPOSpend,model.Vouchers, model.Reforecast);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "GetBudgetWithVouchersReport");
                model.isError = true;
            }

            return model;
        }

        private List<BudgetWithVoucherReportItem> GetVouchersForBudgetWithVouchersReport(
            List<AccountToCategory> accountToCategoryList,
            int epoYear,
            int entityId,
            int ccId)
        {
            var statusList = new List<string>() { GetStatusName(StatusEnum.Draft), GetStatusName(StatusEnum.Pending) };
            var accountList = accountToCategoryList.Select(x => x.Account);
            var accountIdList = accountList.Select(x => x.Id).Distinct().ToList();
            var list = new List<BudgetWithVoucherReportItem>();
            var ordersList =
                _orderRepository.Get(
                        x =>
                            !x.IsDeleted && !statusList.Contains(x.Status.Name) && x.Entity.Id == entityId
                            && x.CostCentre.Id == ccId && x.OrderItems.Any(y => accountIdList.Contains(y.Account.Id)))
                    .Include(x => x.OrderItems.Select(c => c.Account))
                    .ToList();
            foreach (var accCategory in accountToCategoryList.Select(x => x.Category).Distinct())
            {
                var totalByPeriod = new double[13];
                var totalByCategory = new double();
                var categoryAccointIdList =
                        accountToCategoryList.Where(x => x.Category.Id == accCategory.Id)
                            .Select(x => x.Account)
                            .ToList()
                            .Select(x => x.Id);
                for (var calPeriod = 1; calPeriod <= PeriodsQty; calPeriod++)
                {
                    var year = epoYear;
                    var finPeriod = GetCorrectPeriod(calPeriod);
                    if (finPeriod <= 7) year = epoYear - 1;
                    var period = calPeriod;

                    var ordersForPeriod = ordersList
                        .Where(x => x.LastModifiedDate.Month == period && x.LastModifiedDate.Year == year 
                        && x.OrderItems.Any(y=>!y.IsDeleted && categoryAccointIdList.Contains(y.Account.Id)))
                        .Select(x=>x.Id.ToString()).ToList();
                   
                    var vouchersList = _voucherDocumentRepository.Get(x =>
                                !x.IsDeleted
                                && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")
                                && ordersForPeriod.Contains(x.Reference)).Select(x=>x.Voucher).Distinct().ToList();

                    var periodTotal = vouchersList.Sum(x => x.Amount);
                    totalByPeriod[finPeriod] = periodTotal;
                    totalByCategory += periodTotal;


                }
                var item = new BudgetWithVoucherReportItem()
                {
                    Category = accCategory != null ? accCategory.Name : string.Empty,
                    CategoryId = accCategory?.Id ?? 0,
                    Period = totalByPeriod,
                    Total = totalByCategory
                };
                if (item.Total > 0) list.Add(item);

            }
            list = MakeOverallTotals(list);

            return list;
        }

        private List<BudgetWithVoucherReportItem> GetEstimateEPOSpend(List<AccountToCategory> accountToCategoryList, List<Order> ordersList, int epoYear)
        {
            var list = new List<BudgetWithVoucherReportItem>();
            try
            {
                

                foreach (var accCategory in accountToCategoryList.Select(x => x.Category).Distinct())
                {
                    var totalByPeriod = new double[13];
                    var totalByCategory = new double();
                    var categoryAccointIdList =
                            accountToCategoryList.Where(x => x.Category.Id == accCategory.Id)
                                .Select(x => x.Account)
                                .ToList()
                                .Select(x => x.Id);
                    for (var calPeriod = 1; calPeriod <= PeriodsQty; calPeriod++)
                    {
                        var year = epoYear;
                        var finPeriod = GetCorrectPeriod(calPeriod);
                        if (finPeriod <= 7) year = epoYear - 1;
                        var period = calPeriod;
                        var ordersForPeriod = ordersList.Where(x => x.LastModifiedDate.Month == period && x.LastModifiedDate.Year == year);
                        var oItems = new List<OrderItem>();

                        

                        foreach (var order in ordersForPeriod)
                        {
                            oItems.AddRange(order.OrderItems.Where(x => !x.IsDeleted && categoryAccointIdList.Contains(x.Account.Id)));
                        }

                        var periodTotal = oItems.Sum(x => x.TotalExTax);
                        totalByPeriod[finPeriod] = periodTotal;
                        totalByCategory += periodTotal;
                    }
                    var item = new BudgetWithVoucherReportItem()
                    {
                        Category = accCategory?.Name,
                        CategoryId = accCategory?.Id??0,
                        Period = totalByPeriod,
                        Total = totalByCategory
                    };

                    if (item.Total > 0) list.Add(item);
                }
                //if (filter.SelectedCategory == 0) list = MakeCategoryTotals(list);
                list = MakeOverallTotals(list);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "GetEstimateEPOSpendOrEPOApproved");
            }

            return list;
        }

        private void PrepareDataForBudgetReport(BudgetReportEntryDataViewModel filter, ICollection<string> statusList, out List<AccountToCategory> accountToCategoryList, out List<Order> listEPO)
        {
            accountToCategoryList = filter.SelectedCategory != 0 ? _accountToCategoryRepository.Get(x => !x.IsDeleted && x.Category.Id == filter.SelectedCategory && !x.Account.IsDeleted).ToList()
                                                                     : _accountToCategoryRepository.Get(x => !x.IsDeleted && !x.Category.IsDeleted && !x.Account.IsDeleted).ToList();
            var accountList = accountToCategoryList.Select(x => x.Account);
            var accountIdList = accountList.Select(x => x.Id).Distinct().ToList();
            listEPO = _orderRepository.Get(
                    x =>
                    !x.IsDeleted
                    && statusList.Contains(x.Status.Name)
                    && x.Entity.Id == filter.SelectedEntity && x.CostCentre.Id == filter.SelectedCostCenter && x.OrderItems.Any(y => accountIdList.Contains(y.Account.Id)))
                    .Include(x => x.OrderItems.Select(c => c.Account))
                    .ToList();
        }

        private List<BudgetWithVoucherReportItem> GetReforecastForBudgetWithVouchersReport(IEnumerable<AccountToCategory> accountToCategoryList, int year, Entity entity, CostCentre cc)
        {
            var list = new List<BudgetWithVoucherReportItem>();
            try
            {
                var accountList = accountToCategoryList.Select(x => x.Account).Distinct();
                var accountCodeList = accountList.Select(x => x.Code);
                var listForecast =
                    _glBalanceWithBudgetRepository.Get(
                        x =>
                        x.GLYear == year && x.EntityCode == entity.CodeNumber.ToString() && x.CostCentreCode == cc.Code.ToString()
                        && accountCodeList.Contains(x.AccountCode)).ToList();

                foreach (var accCategory in accountToCategoryList.Select(x => x.Category).Distinct())
                {
                    var totalByPeriod = new double[13];
                    var totalByAccount = new double();
                    var categoryAccountCodeList =
                            accountToCategoryList.Where(x => x.Category.Id == accCategory.Id)
                                .Select(x => x.Account)
                                .ToList()
                                .Select(x => x.Code);
                    for (var period = 1; period <= PeriodsQty; period++)
                    {
                        var per = period;
                        var forecastsForPeriod =
                            listForecast.Where(x => x.GLPeriod == per && categoryAccountCodeList.Contains(x.AccountCode)).ToList();

                            totalByPeriod[per] = Convert.ToDouble(forecastsForPeriod.Sum(x=>x.BudgetPeriod));
                            totalByAccount += totalByPeriod[per];


                    }
                    var item = new BudgetWithVoucherReportItem()
                    {
                        Category = accCategory != null ? accCategory.Name : string.Empty,
                        CategoryId = accCategory?.Id??0,
                        Period = totalByPeriod,
                        Total = totalByAccount
                    };
                    if (item.Total > 0) list.Add(item);
                }
                list = MakeOverallTotals(list);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, $"GetReforecastForBudgetWithVouchersReport()");
            }
            return list;
        }

        public Dictionary<string, string> GetPeriodEPODetailsForBudgetReport(string glCode, int period, int entity, int cc, bool isEstimate=true)
        {
            var today = DateTime.Today;
            var epoYear = today.Year;

            var statusList = isEstimate
                                 ? new List<string>()
                                       {
                                           GetStatusName(StatusEnum.Receipt_in_Full),
                                           GetStatusName(StatusEnum.Receipt_Partial),
                                           GetStatusName(StatusEnum.Closed)
                                       }
                                 : new List<string>() { GetStatusName(StatusEnum.Approved) };
            
            var calPeriod = ConvertFinancePeriodToCalendar(period);
            if ((today.Month >=1 && today.Month<=6 ) &&(calPeriod>=1 && calPeriod<=6) )
            {
                epoYear--;
            }
            else if((today.Month >= 1 && today.Month <= 6) && (calPeriod >= 7 && calPeriod <= 12))
            {
                epoYear ++;
            }
            var listEPO = isEstimate ?
                   _orderRepository.Get(
                       x =>
                       !x.IsDeleted
                       && x.Entity.Id == entity && x.CostCentre.Id == cc && statusList.Contains(x.Status.Name) && x.OrderItems.Any(y => y.Account.Code==glCode) && x.LastModifiedDate.Month == calPeriod && x.LastModifiedDate.Year == epoYear)
                       .Include(x => x.OrderItems)
                       .ToList()
                       : _orderRepository.Get(
                       x =>
                       !x.IsDeleted
                       && x.Entity.Id == entity && x.CostCentre.Id == cc && statusList.Contains(x.Status.Name) && x.OrderItems.Any(y => y.Account.Code == glCode && y.DueDate.Year==epoYear && y.DueDate.Month==calPeriod))
                       .Include(x => x.OrderItems)
                       .ToList();
            
            var result = new Dictionary<string,string>();
            foreach (var order in listEPO)
            {
                var oItems = new List<OrderItem>();
                oItems.AddRange(order.OrderItems.Where(x => !x.IsDeleted && x.Account.Code == glCode).ToList());
                result.Add($"PO# {order.OrderNumber}",oItems.Sum(x=>x.TotalExTax).ToString("C"));
            }
            return result;
        }

        public Dictionary<string, string> GetPeriodEPODetailsForBudgetReport(int categoryId, int period, int entity, int cc)
        {
            var today = DateTime.Today;
            var epoYear = today.Year;
            var accountIdList =
                            _accountToCategoryRepository.Get(x => x.Category.Id == categoryId)
                                .Select(x => x.Account)
                                .ToList()
                                .Select(x => x.Id);
            var statusList = new List<string>()
                                 {
                                     GetStatusName(StatusEnum.Receipt_in_Full),
                                     GetStatusName(StatusEnum.Receipt_Partial),
                                     GetStatusName(StatusEnum.Closed)
                                 };

            var calPeriod = ConvertFinancePeriodToCalendar(period);
            if ((today.Month >= 1 && today.Month <= 6) && (calPeriod >= 1 && calPeriod <= 6))
            {
                epoYear--;
            }
            else if ((today.Month >= 1 && today.Month <= 6) && (calPeriod >= 7 && calPeriod <= 12))
            {
                epoYear++;
            }

            var listEPO =
                   _orderRepository.Get(
                       x =>
                       !x.IsDeleted
                       && x.Entity.Id == entity && x.CostCentre.Id == cc && statusList.Contains(x.Status.Name) && x.OrderItems.Any(y => accountIdList.Contains(y.Account.Id)) && x.LastModifiedDate.Month == calPeriod && x.LastModifiedDate.Year == epoYear)
                       .Include(x => x.OrderItems)
                       .ToList();

            var result = new Dictionary<string, string>();
            foreach (var order in listEPO)
            {
                order.OrderItems = order.OrderItems.Where(x => !x.IsDeleted).ToList();
                //var oItems = new List<OrderItem>();
                //oItems.AddRange(order.OrderItems.Where(x => accountIdList.Contains(x.Account.Id)).ToList());
                result.Add($"PO# {order.OrderNumber}", order.OrderItems.Where(x => accountIdList.Contains(x.Account.Id)).Sum(x => x.TotalExTax).ToString("C"));
            }
            return result;
        }

        public Dictionary<string, string> GetPeriodVoucherDetailsForBudgetReportByCategory(int categoryId, int period, int entity, int cc)
        {
            var today = DateTime.Today;
            var epoYear = today.Year;
            var accountIdList =
                            _accountToCategoryRepository.Get(x => x.Category.Id == categoryId)
                                .Select(x => x.Account)
                                .ToList()
                                .Select(x => x.Id);
            var statusList = new List<string>()
                                 {
                                     GetStatusName(StatusEnum.Draft),
                                     GetStatusName(StatusEnum.Pending)
                                 };

            var calPeriod = ConvertFinancePeriodToCalendar(period);
            if ((today.Month >= 1 && today.Month <= 6) && (calPeriod >= 1 && calPeriod <= 6))
            {
                epoYear--;
            }
            else if ((today.Month >= 1 && today.Month <= 6) && (calPeriod >= 7 && calPeriod <= 12))
            {
                epoYear++;
            }
            var orderIdList =
                   _orderRepository.Get(
                       x =>
                       !x.IsDeleted
                       && x.Entity.Id == entity && x.CostCentre.Id == cc && !statusList.Contains(x.Status.Name) 
                       && x.OrderItems.Any(y => accountIdList.Contains(y.Account.Id)) && x.LastModifiedDate.Month == calPeriod 
                       && x.LastModifiedDate.Year == epoYear)
                       .Include(x => x.OrderItems)
                       .Select(x=>x.Id.ToString())
                       .ToList();
            

            var vouchersList = _voucherDocumentRepository.Get(x =>
                        !x.IsDeleted
                        && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")
                        && orderIdList.Contains(x.Reference)).Select(x => x.Voucher).Distinct().ToList();

            var result = new Dictionary<string, string>();
            foreach (var voucher in vouchersList)
            {
                result.Add($"Voucher# {voucher.VoucherNumber}", voucher.Amount.ToString("C"));
            }
            return result;
        }


        private List<BudgetReportItem> MakeCategoryTotals(List<BudgetReportItem> list)
        {
            var index = 0;
            list = list.OrderBy(x => x.Category).ToList();
            var categoryList = list.GroupBy(x => x.Category).OrderBy(x=>x.Key).Select(x=>x.Key).ToList();

            if (categoryList.Count == 1) return list;

            foreach (var cat in categoryList)
            {
                var totalByPeriod = new double[13];
                var totalByCat = new double();
                var categoryAccList = list.Where(x => x.Category == cat).Select(x => x.GLCode).ToList();
                    
                for (var period = 1; period <= PeriodsQty; period++)
                {
                    var per = GetCorrectPeriod(period);
                    totalByPeriod[per] =
                        list.Where(x => categoryAccList.Contains(x.GLCode)).Sum(x => x.Period[per]);
                    totalByCat += totalByPeriod[per];
                }
                index += categoryAccList.Count;
                list.Insert(index,new BudgetReportItem() {Category = cat+" Totals", Period = totalByPeriod,AccountName = string.Empty,GLCode = string.Empty,Total = totalByCat});
                index += 1;//shift
            }
            return list;
        }
        

        private List<BudgetReportItem> MakeOverallTotals(List<BudgetReportItem> list)
        {
            var totalByPeriod = new double[13];
            var total = 0d;
            for (var period = 1; period <= PeriodsQty; period++)
            {
                var per = GetCorrectPeriod(period);
                totalByPeriod[per] = list.Sum(x => x.Period[per]);
                total += totalByPeriod[per];
            }
            list.Add(new BudgetReportItem()
                         {
                             Category = "Total", AccountName =string.Empty, GLCode = string.Empty, Period = totalByPeriod, Total = total
                         });
            return list;
        }

        private List<BudgetWithVoucherReportItem> MakeOverallTotals(List<BudgetWithVoucherReportItem> list)
        {
            var totalByPeriod = new double[13];
            var total = 0d;
            for (var period = 1; period <= PeriodsQty; period++)
            {
                var per = GetCorrectPeriod(period);
                totalByPeriod[per] = list.Sum(x => x.Period[per]);
                total += totalByPeriod[per];
            }
            list.Add(new BudgetWithVoucherReportItem()
            {
                Category = "Total",
                Period = totalByPeriod,
                Total = total
            });
            return list;
        }

        private void FillCostCentreOwnerViewModelList(IEnumerable<CostCentre> list, ICollection<CostCentreViewModel> returnList)
        {
            foreach (var cc in list)
            {
                if (cc.Owner == null)
                {
                    var ccvm = new CostCentreViewModel() { Id = cc.Id, Code = cc.Code, Name = cc.Name, Owner = "NO OWNER" };
                    returnList.Add(ccvm);
                }
                else
                {
                    if (cc.Owner.UserInfo.IsDeleted)
                    {
                        var ccvm = new CostCentreViewModel()
                                       {
                                           Id = cc.Id,
                                           Code = cc.Code,
                                           Name = cc.Name,
                                           Owner = "NO OWNER (was: " + cc.Owner.GetFullName() + ")"
                                       };
                        returnList.Add(ccvm);
                    }
                    else
                    {
                        var rfModel =
                            _rockyEmployeesRepository.Get(x => x.EmpNo == cc.Owner.EmployeeId.ToString() && x.Active == 1)
                                .FirstOrDefault();
                        if (rfModel == null)
                        {
                            var ccvm = new CostCentreViewModel()
                                           {
                                               Id = cc.Id,
                                               Code = cc.Code,
                                               Name = cc.Name,
                                               Owner = "NO OWNER (was: " + cc.Owner.GetFullName() + ")"
                                           };
                            returnList.Add(ccvm);
                        }
                    }
                }
            }
        }

        private Stopwatch Watch;

        public void StartWatch()
        {
            Watch = new Stopwatch();
            Watch.Start();
        }

        public string Stopwatch()
        {
            Watch.Stop();
            var ts = Watch.Elapsed;
            var elapsedTime = $"{ts.Hours:00}h:{ts.Minutes:00}m:{ts.Seconds:00}s.{ts.Milliseconds / 10:00}ms";
            return elapsedTime;
        }
    }
}