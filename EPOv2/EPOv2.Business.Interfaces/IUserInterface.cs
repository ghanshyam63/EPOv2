namespace EPOv2.Business.Interfaces
{
    using System.Collections.Generic;

    using EPOv2.ViewModels;

    public interface IUserInterface
    {
        int curEmpId { get; set; }
        int orderId { get; set; }
        List<int> SubUsersList { get; set; }
        List<OrderDashboardViewModel> GetMyOrders();

        List<OrderDashboardViewModel> GetOrdersForApprove();

        List<Transaction> GetOrderTransactions();

        List<DashboardIncoiceViewModel> GetInvoicesForApprove();

        SearchViewModel GetSearchViewModel();

        List<OrderDashboardViewModel> GetOrdersForMatching();

        List<CapexDashboardViewModel> GetMyCapexes();

        IEnumerable<CapexDashboardViewModel> GetCapexesForApprove();

        int GetOldData();
    }
}