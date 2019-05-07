namespace EPOv2.Business.Interfaces
{
    using System.Collections.Generic;

    using Intranet.ViewModels;

    public interface IMock
    {
        List<YieldGraphTile> GetYieldDashboardData();

        object GetNotificationDashboardData();

        EpoDashboardViewModel GetEPODashboardViewModel();

        TnADashboardTiles GetTNADashboardViewModel();

        CasefillDashboardViewModel GetCasefillDashboardViewModel();

        AttainmentToPlanDashboardViewModel GetAttainmentToPlanDashboardViewModel();

        RightFirstTimeDashboardViewModel GetRightFirstTimeDashboardViewModel();

        TileMockData FetchDataForShowroomTile(DTileShowroomViewModel tileVm);

        SmsTileViewModel GetSafetyActionsForEmployee();
    }
}