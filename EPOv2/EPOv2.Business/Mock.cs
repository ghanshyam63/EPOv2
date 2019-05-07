using System.Collections.Generic;

namespace EPOv2.Business
{
    using DomainModel.Entities;
    using DomainModel.Enums;

    using EPOv2.Business.Interfaces;

    using Intranet.ViewModels;

    public partial class Mock: IMock
    {
        public List<YieldGraphTile> GetYieldDashboardData()
        {
            var model = new List<YieldGraphTile>();
            model.Add(GetYieldCOS());
            model.Add(GetYieldSpin());

            return model;
        }

        private static YieldGraphTile GetYieldSpin()
        {
            return new YieldGraphTile()
                       {
                           Id = 1,
                           Header = "Yesterday’s Yield - Spin",
                           chartType = "Bar",
                           IsMock = true,


                           YieldDashboardItems = new List<YieldGraphItem>() {new YieldGraphItem() {productCode = "Spinb01", Points = new List<TileItem>()
                                                                                                                                         {
                                                                                                                                             new TileItem() {ProgressValue = 0, SiteName = "BDL"},
                                                                                                                                             new TileItem() {ProgressValue = 0, SiteName = "PTH"},
                                                                                                                                             new TileItem() {SiteName = "SYD",ProgressValue = 0},
                                                                                                                                             new TileItem() {SiteName = "BNE",ProgressValue = 0}
                                                                                                                                         },
                                                                                                   Data = new byte[]{14,78,34,80},
                                                                                                   fillColor = "rgba(28,132,198,0.2)",
                                                                                                   pointColor = "rgba(28,132,198,1)",
                                                                                                   strokeColor= "rgba(28,132,198,1)",
                                                                                                   pointStrokeColor= "#fff",
                                                                                                   pointHighlightFill= "#fff",
                                                                                                   pointHighlightStroke= "rgba(28,132,198,1)",
                                                                                                  },
                                                                             new YieldGraphItem() {productCode = "Spinb02", Points = new List<TileItem>()
                                                                                                                                         {
                                                                                                                                             new TileItem() {ProgressValue = 0, SiteName = "BDL"},
                                                                                                                                             new TileItem() {ProgressValue = 0, SiteName = "PTH"},
                                                                                                                                             new TileItem() {SiteName = "SYD",ProgressValue = 0},
                                                                                                                                             new TileItem() {SiteName = "BNE",ProgressValue = 0}
                                                                                                                                         },
                                                                                                   Data = new byte[]{7,78,45,23},
                                                                                                   fillColor = "rgba(26,179,148,0.2)",
                                                                                                   pointColor = "rgba(26,179,148,1)",
                                                                                                   strokeColor= "rgba(26,179,148,1)",
                                                                                                   pointStrokeColor= "#fff",
                                                                                                   pointHighlightFill= "#fff",
                                                                                                   pointHighlightStroke= "rgba(151,187,205,1)",
                                                                                                  },


                                                                            },
                           Labels = new[] { "BNE", "BDL", "SYD", "PTH" },
                       };
        }

        private static YieldGraphTile GetYieldCOS()
        {
            return new YieldGraphTile()
                       {
                           Id = 0,
                           Header = "Yesterday’s Yield - COS",
                           chartType = "Bar",
                           IsMock = true,
                
                
                           YieldDashboardItems = new List<YieldGraphItem>() {new YieldGraphItem() {productCode = "Cos01", Points = new List<TileItem>()
                                                                                                                                       {
                                                                                                                                           new TileItem() {ProgressValue = 0, SiteName = "BDL"},
                                                                                                                                           new TileItem() {ProgressValue = 0, SiteName = "PTH"},
                                                                                                                                           new TileItem() {SiteName = "SYD",ProgressValue = 0},
                                                                                                                                           new TileItem() {SiteName = "BNE",ProgressValue = 0}
                                                                                                                                       },
                                                                                                   Data = new byte[]{1,23,16,89},
                                                                                                   fillColor = "rgba(28,132,198,0.2)",
                                                                                                   pointColor = "rgba(28,132,198,1)",
                                                                                                   strokeColor= "rgba(28,132,198,1)",
                                                                                                   pointStrokeColor= "#fff",
                                                                                                   pointHighlightFill= "#fff",
                                                                                                   pointHighlightStroke= "rgba(28,132,198,1)",
                                                                                                  },
                                                                             new YieldGraphItem() {productCode = "Cos02", Points = new List<TileItem>()
                                                                                                                                       {
                                                                                                                                           new TileItem() {ProgressValue = 0, SiteName = "BDL"},
                                                                                                                                           new TileItem() {ProgressValue = 0, SiteName = "PTH"},
                                                                                                                                           new TileItem() {SiteName = "SYD",ProgressValue = 0},
                                                                                                                                           new TileItem() {SiteName = "BNE",ProgressValue = 0}
                                                                                                                                       },
                                                                                                   Data = new byte[]{3,45,34,15},
                                                                                                   fillColor = "rgba(26,179,148,0.2)",
                                                                                                   pointColor = "rgba(26,179,148,1)",
                                                                                                   strokeColor= "rgba(26,179,148,1)",
                                                                                                   pointStrokeColor= "#fff",
                                                                                                   pointHighlightFill= "#fff",
                                                                                                   pointHighlightStroke= "rgba(151,187,205,1)",
                                                                                                  },


                                                                            },
                           Labels =new[] {"BNE","BDL","SYD","PTH"},
                       };
        }

        private static YieldGraphTile GetYieldLineGraphCOS()
        {
            return new YieldGraphTile()
            {
                Id = 0,
                Header = "Yield - COS",
                chartType = "line",
                IsMock = true,


                YieldDashboardItems = new List<YieldGraphItem>() {new YieldGraphItem() {productCode = "Cos01", Points = new List<TileItem>()
                                                                                                                                       {
                                                                                                                                           new TileItem() {ProgressValue = 0, SiteName = "BDL"},
                                                                                                                                           new TileItem() {ProgressValue = 0, SiteName = "PTH"},
                                                                                                                                           new TileItem() {SiteName = "SYD",ProgressValue = 0},
                                                                                                                                           new TileItem() {SiteName = "BNE",ProgressValue = 0}
                                                                                                                                       },
                                                                                                   Data = new byte[]{1,23,16,89},
                                                                                                   fillColor = "rgba(28,132,198,0.2)",
                                                                                                   pointColor = "rgba(28,132,198,1)",
                                                                                                   strokeColor= "rgba(28,132,198,1)",
                                                                                                   pointStrokeColor= "#fff",
                                                                                                   pointHighlightFill= "#fff",
                                                                                                   pointHighlightStroke= "rgba(28,132,198,1)",
                                                                                                  },
                                                                             new YieldGraphItem() {productCode = "Cos02", Points = new List<TileItem>()
                                                                                                                                       {
                                                                                                                                           new TileItem() {ProgressValue = 0, SiteName = "BDL"},
                                                                                                                                           new TileItem() {ProgressValue = 0, SiteName = "PTH"},
                                                                                                                                           new TileItem() {SiteName = "SYD",ProgressValue = 0},
                                                                                                                                           new TileItem() {SiteName = "BNE",ProgressValue = 0}
                                                                                                                                       },
                                                                                                   Data = new byte[]{3,45,34,15},
                                                                                                   fillColor = "rgba(26,179,148,0.2)",
                                                                                                   pointColor = "rgba(26,179,148,1)",
                                                                                                   strokeColor= "rgba(26,179,148,1)",
                                                                                                   pointStrokeColor= "#fff",
                                                                                                   pointHighlightFill= "#fff",
                                                                                                   pointHighlightStroke= "rgba(151,187,205,1)",
                                                                                                  },


                                                                            },
                Labels = new[] { "BNE", "BDL", "SYD", "PTH" },
            };
        }

        public object GetNotificationDashboardData()
        {
            var model = new
            {
                Tiles = new[] {
                    new {Name = "Ashton Cox", Position = "Junior Technical Author", Office = "London", Age = 33, isOverdue =true, TypeKey=1},
                    new {Name = "Bradley Greer", Position = "Pre-Sales Support", Office = "Tokyo", Age = 27, isOverdue =true, TypeKey=4},
                    new {Name = "Airi Satou", Position = "Integration Specialist", Office = "New York", Age = 43,isOverdue =false, TypeKey=2},
                    new {Name = "Caesar Vance", Position = "Software Engineer", Office = "San Francisco", Age = 36, isOverdue =false, TypeKey=3},
                },
                NotificationQty = 2
            };
            return model;
        }

        public EpoDashboardViewModel GetEPODashboardViewModel()
        {
            var model = new EpoDashboardViewModel() { EpoTiles = new List<EpoTile>(), NotificationQty = 0 };
            model.EpoTiles.Add(new EpoTile() { Header = "Orders to Approve", TileColor = "blue-bg", TileIcon = "fa fa-check-circle-o", SortNumber = 1, TypeKey = 1, FunctionParam = FunctionParam.OAP.ToString(), BottomDescription = "New",Value = 0, IsMock = true});
            model.EpoTiles.Add(new EpoTile() { Header = "Orders To Be Matched", TileColor = "yellow-bg", TileIcon = "fa fa-shopping-bag", TypeKey = 2, SortNumber = 2, FunctionParam = FunctionParam.OMA.ToString(), BottomDescription = "New", Value = 0, IsMock = true});
            model.EpoTiles.Add(new EpoTile() { Header = "My Orders", TileColor = "white-bg", TileIcon = "fa fa-shopping-basket", SortNumber = 3, TypeKey = 3, FunctionParam = FunctionParam.OMY.ToString(), BottomDescription = "New",Value = 0, IsMock = true});
            model.EpoTiles.Add(new EpoTile() { Header = "Invoices to Approve", TileColor = "red-bg", TileIcon = "fa fa-credit-card-alt", SortNumber = 4, TypeKey = 4, FunctionParam = FunctionParam.IAP.ToString(), BottomDescription = "New",Value = 0, IsMock = true});
            model.EpoTiles.Add(new EpoTile() { Header = "Capex to Approve", TileColor = "navy-bg", TileIcon = "fa fa-briefcase", SortNumber = 5, TypeKey = 5, FunctionParam = FunctionParam.CAP.ToString(), BottomDescription = "New",Value = 0, IsMock = true});
            return model;
        }

        public EpoTile GetEPOTile(string header)
        {
            return new EpoTile()
                       {
                           Header = header,
                           TileColor = "blue-bg",
                           TileIcon = "fa fa-check-circle-o",
                           SortNumber = 1,
                           TypeKey = 1,
                           FunctionParam = FunctionParam.OAP.ToString(),
                           BottomDescription = "New",
                           Value = 0,
                           IsMock = true
                       };
        }

        public TnADashboardTiles GetTNADashboardViewModel()
        {
            var model = new TnADashboardTiles() { TnATiles = new List<TnATile>() };
            model.TnATiles.Add(GetTnATile("Emp Exceptions"));
            return model;
        }

        private TnATile GetTnATile(string header)
        {
            return new TnATile() {TypeKey = 1,TileColor = "black-bg",Header = header,SortNumber = 1,Value = 0,TileIcon = "fa fa-street-view", IsMock = true};
        }

        public CasefillDashboardViewModel GetCasefillDashboardViewModel()
        {
            var model = new CasefillDashboardViewModel()
                            {
                                IsMock = true,
                                CasefillBudget = new CasefillBudgetTile(),
                                CasefillSales = new List<TileItem>(),
                                TileColor = "link-tile"
            };
            model.CasefillSales.Add(new TileItem() { SiteName = "BNE", Color = "danger", ProgressValue = 0});
            model.CasefillSales.Add(new TileItem() { SiteName = "SYD", Color = "danger", ProgressValue = 0 });
            model.CasefillSales.Add(new TileItem() { SiteName = "BDL", Color = "danger", ProgressValue = 0 });
            model.CasefillSales.Add(new TileItem() { SiteName = "PTH", Color = "danger", ProgressValue = 0 });

            return model;
        }

        public AttainmentToPlanDashboardViewModel GetAttainmentToPlanDashboardViewModel()
        {
            var model = new AttainmentToPlanDashboardViewModel() { AtpTiles = new List<TileItem>(), IsMock = true, TileColor = "black-bg" };
            model.AtpTiles.Add(new TileItem() { SiteName = "BNE", Color = "success", ProgressValue = 0});
            model.AtpTiles.Add(new TileItem() { SiteName = "SYD", Color = "success", ProgressValue = 0 });
            model.AtpTiles.Add(new TileItem() { SiteName = "BDL", Color = "success", ProgressValue = 0 });
            model.AtpTiles.Add(new TileItem() { SiteName = "PTH", Color = "success", ProgressValue = 0 });
            return model;
        }

        public RightFirstTimeDashboardViewModel GetRightFirstTimeDashboardViewModel()
        {
            var model = new RightFirstTimeDashboardViewModel() { IsMock = true, RftPerSites = new List<TileItem>(), TileColor = "blue-bg" };
            model.RftPerSites.Add(new TileItem() {SiteName = "BNE", Color = "warning", ProgressValue = 0});
            model.RftPerSites.Add(new TileItem() {SiteName = "SYD", Color = "warning", ProgressValue = 0});
            model.RftPerSites.Add(new TileItem() {SiteName = "BDL", Color = "warning", ProgressValue = 0});
            model.RftPerSites.Add(new TileItem() {SiteName = "PTH", Color = "warning", ProgressValue = 0});
            return model;
        }

        public TileMockData FetchDataForShowroomTile(DTileShowroomViewModel tileVm)
        {
            var model = new TileMockData() { };
            switch (tileVm.Type)
            {
                case TileType.EPO: model.EpoTile = GetEPOTile(tileVm.Name);
                    break;
                case TileType.TNA: model.TnATile = GetTnATile(tileVm.Name);
                    break;
                case TileType.Casefill:
                    model.CasefillBudgetTile = GetCasefillDashboardViewModel();
                    break;
                case TileType.ATP:
                    model.ATPTile = GetAttainmentToPlanDashboardViewModel();
                    break;
                case TileType.RFT:
                    model.RFTTile = GetRightFirstTimeDashboardViewModel();
                    break;
                case TileType.Yield:
                    switch (tileVm.SubType)
                    {
                            case TileSubType.YYCOS: model.YieldGraphTile = GetYieldCOS();
                                model.YieldGraphTile.Id = tileVm.Id;
                                break;
                            case TileSubType.YYSPIN:
                                model.YieldGraphTile = GetYieldSpin();
                                model.YieldGraphTile.Id = tileVm.Id;
                                break;
                        case TileSubType.YLCOS:
                            model.YieldGraphTile = GetYieldLineGraphCOS();
                            model.YieldGraphTile.Id = tileVm.Id;
                            break;
                    }
                    break;
                case TileType.SMS:
                    switch (tileVm.SubType)
                    {
                            case TileSubType.SAY: model.SmsTile = GetSafetyActionsForEmployee();
                                break;
                            case TileSubType.STSAR:
                                model.SmsTile = GetSafetyActionsRegister();
                                break;
                            case TileSubType.STIN:
                                model.SmsTile = GetSafetyIncidentsByMonth();
                                break;
                            case TileSubType.STSWAT:
                                model.SmsTile = GetSafetyWalkAndTalkByMonth();
                                break;
                    }
                    
                    break;
                default: break;
            }
            return model;
        }

        private SmsTileViewModel GetSafetyWalkAndTalkByMonth()
        {
            var model = new SmsTileViewModel()
            {
                Header = "Safety Walk & Talk",
                TileColor = "grayblue-bg",
                TypeKey = 4,
                SortNumber = 4,
                IsMock = true,
                SmsTileItems = new List<TileItem>()
            };

            model.SmsTileItems.Add(new TileItem() { SiteName = "BDL", ValueArray = new List<int>() { 1, 6, 13 }, ValueNameArray = new List<string>() { "Day", "Week", "Month" } });
            model.SmsTileItems.Add(new TileItem() { SiteName = "BNE", ValueArray = new List<int>() { 0, 2, 10 }, ValueNameArray = new List<string>() { " Day", " Week", " Month" } });
            model.SmsTileItems.Add(new TileItem() { SiteName = "PTH", ValueArray = new List<int>() { 1, 9, 29 }, ValueNameArray = new List<string>() { "Day", " Week", " Month" } });
            model.SmsTileItems.Add(new TileItem() { SiteName = "SYD", ValueArray = new List<int>() { 1, 3, 56 }, ValueNameArray = new List<string>() { "Day", "Week", "Month" } });
            return model;
        }

        private SmsTileViewModel GetSafetyIncidentsByMonth()
        {
            var model = new SmsTileViewModel()
                            {
                                Header = "Safety Incidents",
                                TileColor = "grayblue-bg",
                                TypeKey = 3,
                                SortNumber = 3,
                                IsMock = true,
                                SmsTileItems = new List<TileItem>()
                            };

            model.SmsTileItems.Add(new TileItem() {SiteName = "BDL", ValueArray = new List<int>() {1,6,13}, ValueNameArray = new List<string>() {"Day", "Week", "Month"} });
            model.SmsTileItems.Add(new TileItem() {SiteName = "BNE", ValueArray = new List<int>() {0,2,10}, ValueNameArray = new List<string>() {"per Day", "per Week", "per Month"} });
            model.SmsTileItems.Add(new TileItem() {SiteName = "PTH", ValueArray = new List<int>() {1,9,29}, ValueNameArray = new List<string>() {"Day", "per Week", "per Month"} });
            model.SmsTileItems.Add(new TileItem() {SiteName = "SYD", ValueArray = new List<int>() {1,3,56}, ValueNameArray = new List<string>() {"Day", "Week", "Month"} });
            return model;
        }

        private SmsTileViewModel GetSafetyActionsRegister()
        {
            var model = new SmsTileViewModel()
                            {
                                IsMock = true,
                                SmsTileItems = new List<TileItem>(),
                                TileColor = "grayblue-bg",
                                Header = "Safety Actions Register",
                                TypeKey = 2,
                                SortNumber = 2
                            };
            model.SmsTileItems.Add(new TileItem() {SiteName = "BDL", ProgressValue = 45, Color ="info"});
            model.SmsTileItems.Add(new TileItem() { SiteName = "BNE", ProgressValue = 69, Color = "info" });
            model.SmsTileItems.Add(new TileItem() {SiteName = "PTH", ProgressValue = 10, Color ="info"});
            model.SmsTileItems.Add(new TileItem() {SiteName = "SYD", ProgressValue = 102, Color ="info"});
            return model;
        }

        public SmsTileViewModel GetSafetyActionsForEmployee()
        {
            var model = new SmsTileViewModel()
                            {
                                TileColor = "grayblue-bg",
                                TypeKey = 1,
                                Header = "Safety Actions",
                                Value = 3,
                                TileIcon = "fa fa-universal-access",
                                IsMock = true,
                                SortNumber = 1
                            };
            return model;
        }
    }
}
