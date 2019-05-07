using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace EPOv2.Business
{
    using System.Data;
    using System.Data.Entity;
    using System.Linq.Dynamic;

    using DomainModel.Entities;
    using DomainModel.Enums;

    using Intranet.ViewModels;

    using User = Microsoft.VisualBasic.ApplicationServices.User;

    public partial class Main
    {
        public enum CasedillNumberColor
        {
            success,

            warning,

            danger
        }

        public class ChartSettings
        {
            public string[] fillColor { get; set; }

            public string[] strokeColor { get; set; }

            public string[] pointColor { get; set; }

            public string[] pointStrokeColor { get; set; }

            public string[] pointHighlightFill { get; set; }

            public string[] pointHighlightStroke { get; set; }

            public string[] chartType { get; set; }

            public string[] backgroundColor { get; set; }
            public string[] borderColor { get; set; }


            public string[] pointBorderColor { get; set; }
            public string pointBackgroundColor { get; set; }

            public string pointHoverBackgroundColor { get; set; }
            public string pointHoverBorderColor { get; set; }







        }

        public ChartSettings chartSettings = new ChartSettings()
                                                 {
                                                     fillColor =
                                                         new[]
                                                             {
                                                                 "rgba(28,132,198,0.2)",
                                                                 "rgba(26,179,148,0.2)",
                                                                 "rgba(26,179,148,0.2)",
                                                                 "rgba(26,179,148,0.2)",
                                                                 "rgba(26,179,148,0.2)",
                                                                 "rgba(26,179,148,0.2)",
                                                                 "rgba(26,179,148,0.2)",
                                                                 "rgba(26,179,148,0.2)",
                                                                 "rgba(26,179,148,0.2)",
                                                                 "rgba(26,179,148,0.2)"
                                                             },
                                                     pointHighlightStroke =
                                                         new[]
                                                             {
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(151,187,205,1)"
                                                             },
                                                     pointColor =
                                                         new[]
                                                             {
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(151,187,205,1)"
                                                             },
                                                     pointHighlightFill = new[] { "#fff", "#fff" },
                                                     pointStrokeColor = new[] { "#fff", "#fff" },
                                                     strokeColor =
                                                         new[]
                                                             {
                                                                 "rgba(28,132,198,1)",
                                                                 "rgba(151,187,205,1)"
                                                             },
                                                     chartType = new[] { "Bar", "Bar","Line" },

                                                     backgroundColor = new[]
                                                                       {
                                                                           "rgba(98, 145, 237,.1)",
                                                                           "rgba(237, 191, 98,.1)",
                                                                           "rgba(237, 98, 145,.1)",
                                                                           "rgba(39, 164, 60,.1)",
                                                                           "rgba(60, 39, 164,.1)",
                                                                           "rgba(164, 39, 81,.1)",
                                                                           "rgba(104, 39, 164,.1)",
                                                                           "rgba(85, 86, 40,.1)",
                                                                           "rgba(0, 245, 249,.1)",
                                                                           "rgba(245, 249, 0,.1)",
                                                                           "rgba(249, 4, 0,.1)",
                                                                           "rgba(105, 105, 114,.1)"
                                                                       },
                                                     borderColor = new[]
                                                                       {
                                                                           "rgba(98, 145, 237,.8)",
                                                                           "rgba(237, 191, 98,.8)",
                                                                           "rgba(237, 98, 145,.8)",
                                                                           "rgba(39, 164, 60,.8)",
                                                                           "rgba(60, 39, 164,.8)",
                                                                           "rgba(164, 39, 81,.8)",
                                                                           "rgba(104, 39, 164,.8)",
                                                                           "rgba(85, 86, 40,.8)",
                                                                           "rgba(0, 245, 249,.8)",
                                                                           "rgba(245, 249, 0,.8)",
                                                                           "rgba(249, 4, 0,.8)",
                                                                           "rgba(105, 105, 114,.8)"
                                                                       },
                                                     pointHoverBorderColor = "rgba(220,220,220,1)",
                                                     pointHoverBackgroundColor = "rgba(75,192,192,1)",
                                                     pointBackgroundColor = "rgba(75,192,192,1)",
                                                     pointBorderColor = new[]
                                                                       {
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)",
                                                                           "rgba(28,132,198,1)"
                                                                       },
                                                     
        };

        public List<CalendarEventViewModel> GetDashboardCalendar()
        {
            var model =
                _dCalendarEventRepository.Get(
                    x =>
                    !x.IsDeleted
                    && ((x.IsOneDayEvent && x.Start >= DateTime.Today) || (!x.IsOneDayEvent && x.End >= DateTime.Today)))
                    .OrderBy(x => x.Start)
                    .ToList();

            var list = new List<CalendarEventViewModel>();
            foreach (var dCalendarEvent in model)
            {
                var item = new CalendarEventViewModel()
                               {
                                   Id = dCalendarEvent.Id,
                                   Title = dCalendarEvent.Title,
                                   Description = dCalendarEvent.Description,
                                   Icon = dCalendarEvent.EventType.Icon,
                                   IconColor = dCalendarEvent.EventType.IconColor,
                                   
                                   Start = dCalendarEvent.Start.Date.ToShortDateString(),
                                   IsOneDayEvent = dCalendarEvent.IsOneDayEvent,
                                   End =
                                       dCalendarEvent.IsOneDayEvent
                                           ? string.Empty
                                           : dCalendarEvent.End.Date.ToShortDateString()
                               };
                item.LabelColor = "label" + item.IconColor.Substring(item.IconColor.IndexOf("-"));
                if (dCalendarEvent.DateCreated.Date.Equals(DateTime.Today)) item.RightLabel = "NEW";
                else
                {
                    if (dCalendarEvent.Start.Date == DateTime.Today) item.RightLabel = "Today";
                    else
                    {
                        var days = (dCalendarEvent.Start.Date - DateTime.Today).Days;

                        item.RightLabel = days > 1 ? days + " days to go" : days + " day to go";
                    }
                }
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// Casefill By Day report for Dashboard. Sorry don't like what I've done, but I had to.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public CasefillDashboardSecondLevelViewModel GetCasefillDashboardSecondLevelViewModel(
            DateTime date,
            string connectionString)
        {
            const string partnerSub = "Partner Sub";
            const string partner = "Partner";
            const string subAll = "All";
            const string total = "All";
            //const string partnerWW = "WW";
            var subPartners = new List<string>();

            var model = new CasefillDashboardSecondLevelViewModel();
            model.Title = "Casefill by day";
            model.Value = new Dictionary<string, string>();
            model.ValueColor = new Dictionary<string, string>();
            model.Keys = new List<string>();
            var query = this.Data.GenerateMDXQueryCasefillByDay(date);
            var dataTable = this.Data.MDxQueryRun(connectionString, query);
            dataTable = GetReadyForAnalyzeCasefillByDay(dataTable);
            model.Columns = (from dc in dataTable.Columns.Cast<DataColumn>() select dc.ColumnName).ToList();
            model.Rows = (from dr in dataTable.Rows.Cast<DataRow>() select dr[0].ToString()).ToList();
            subPartners = dataTable.AsEnumerable().Select(x => x.Field<string>(partnerSub)).Distinct().ToList();
            subPartners.Remove(subAll);
            model.Columns = model.Columns.Where(x => !x.Contains("flag")).ToList();
            var columns = new List<string>(model.Columns);
            columns.RemoveRange(0, 2);
            var rows = model.Rows.Distinct().ToList();
            model.Rows.RemoveRange(11, 2);
            model.DicColumns = new List<string>(columns);
            model.DicRows = new List<string>(model.Rows);
            model.Columns.Remove(partnerSub);

            foreach (var column in columns)
            {
                var i = 0;
                foreach (var row in rows)
                {
                    var dicKey = row + ":" + column;
                    model.Keys.Add(dicKey);
                    var subPartner =
                        dataTable.AsEnumerable()
                            .Where(x => x.Field<string>(partner) == row)
                            .Select(x => x.Field<string>(partnerSub))
                            .FirstOrDefault();

                    var value =
                        dataTable.AsEnumerable()
                            .Where(x => x.Field<string>(partner) == row && x.Field<string>(partnerSub) == subPartner)
                            .Select(x => x.Field<double?>(column))
                            .FirstOrDefault();

                    model.Value.Add(dicKey, value?.ToString() ?? "-");
                    model.ValueColor.Add(dicKey, value != null ? CheckValueForColor(value) : "");

                    if (
                        dataTable.AsEnumerable()
                            .Where(x => x.Field<string>(partner) == row)
                            .Select(x => x.Field<string>(partner))
                            .Count() > 1 && row != total)
                    {
                        foreach (var sub in subPartners)
                        {
                            i++;
                            dicKey = row + " " + sub + ":" + column;

                            model.Keys.Add(dicKey);
                            value =
                                dataTable.AsEnumerable()
                                    .Where(x => x.Field<string>(partner) == row && x.Field<string>(partnerSub) == sub)
                                    .Select(x => x.Field<double?>(column))
                                    .FirstOrDefault();
                            model.Value.Add(dicKey, value?.ToString() ?? "-");
                            model.ValueColor.Add(dicKey, value != null ? CheckValueForColor(value) : "");
                            model.DicRows[i] = row + " " + sub;
                        }

                    }
                    i++;
                }
            }

            return model;
        }

        public string CheckValueForColor(double? value)
        {
            if (value == 1) return CasedillNumberColor.success.ToString();
            if (value >= 0.98 && value < 1) return CasedillNumberColor.warning.ToString();
            return CasedillNumberColor.danger.ToString();
        }

        public DataTable GetReadyForAnalyzeCasefillByDay(DataTable dt)
        {
            dt.Columns[0].ColumnName = "Partner";
            dt.Columns[1].ColumnName = "Partner Sub";
            dt.Columns[2].ColumnName = "BDL";
            dt.Columns[3].ColumnName = "BDL-flag";
            dt.Columns[4].ColumnName = "BNE";
            dt.Columns[5].ColumnName = "BNE-flag";
            dt.Columns[6].ColumnName = "PTH";
            dt.Columns[7].ColumnName = "PTH-flag";
            dt.Columns[8].ColumnName = "SYD";
            dt.Columns[9].ColumnName = "SYD-flag";
            dt.Columns[10].ColumnName = "Total";
            dt.Columns[11].ColumnName = "Total-flag";
            return dt;
        }

        public CasefillDashboardSecondLevelViewModel GetCasefillDashboardTrendViewModel(
            DateTime dateFrom,
            DateTime dateTo,
            string connectionString)
        {
            const string Site = "Site";

            var model = new CasefillDashboardSecondLevelViewModel();
            model.Title = "Casefill by site";
            model.Value = new Dictionary<string, string>();
            model.ValueColor = new Dictionary<string, string>();
            var query = this.Data.GenerateMDXQueryCasefillTrend(dateFrom, dateTo);
            var dataTable = this.Data.MDxQueryRun(connectionString, query);
            dataTable = GetReadyForAnalyzeCasefillTrend(dataTable, dateFrom, dateTo);
            model.Columns = (from dc in dataTable.Columns.Cast<DataColumn>() select dc.ColumnName).ToList();
            model.Rows = (from dr in dataTable.Rows.Cast<DataRow>() select dr[0].ToString()).ToList();
            model.DicColumns = new List<string>(model.Columns);
            model.DicRows = new List<string>(model.Rows);
            model.DicColumns.RemoveAt(0);
            foreach (var column in model.DicColumns)
            {
                foreach (var row in model.Rows)
                {
                    var dicKey = row + ":" + column;
                    var value =
                        dataTable.AsEnumerable()
                            .Where(x => x.Field<string>(Site) == row)
                            .Select(x => x.Field<double?>(column))
                            .FirstOrDefault();

                    model.Value.Add(dicKey, value?.ToString() ?? "-");
                    model.ValueColor.Add(dicKey, value != null ? CheckValueForColor(value) : "");
                }
            }

            return model;
        }

        public CasefillDashboardSecondLevelViewModel GetCasefillDashboardDetailViewModel(
            DateTime date,
            string connectionString)
        {
            const string Site = "Site";
            const string CCode = "Customer Code";
            const string Order = "Order #";
            const string ICode = "Item Code";

            var model = new CasefillDashboardSecondLevelViewModel();
            model.Title = "Casefill by site";
            model.Value = new Dictionary<string, string>();
            model.ValueColor = new Dictionary<string, string>();
            model.Keys = new List<string>();
            var query = this.Data.GenerateMDXQueryCasefillDetail(date);
            var dataTable = this.Data.MDxQueryRun(connectionString, query);
            dataTable = GetReadyForAnalyzeCasefillDetail(dataTable);
            model.Columns = (from dc in dataTable.Columns.Cast<DataColumn>() select dc.ColumnName).ToList();
            model.Rows = (from dr in dataTable.Rows.Cast<DataRow>() select dr[0].ToString()).ToList();
            model.DicColumns = new List<string>(model.Columns);
            model.DicColumns.RemoveAt(0);
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var column in model.DicColumns)
                {
                    var dicKey = row[0] + "!" + row[1] + "!" + row[3] + "!" + row[4] + ":" + column;
                    model.Keys.Add(dicKey);
                    var value =
                        dataTable.AsEnumerable()
                            .Where(
                                x =>
                                x.Field<string>(Site) == row[0].ToString()
                                && x.Field<string>(CCode) == row[1].ToString()
                                && x.Field<string>(Order) == row[3].ToString()
                                && x.Field<string>(ICode) == row[4].ToString())
                            .Select(x => x.Field<object>(column))
                            .FirstOrDefault();

                    model.Value.Add(dicKey, value.ToString());
                    model.ValueColor.Add(dicKey, "");
                }
            }
            return model;
        }

        private DataTable GetReadyForAnalyzeCasefillDetail(DataTable dt)
        {
            dt.Columns[0].ColumnName = "Site";
            dt.Columns[1].ColumnName = "Customer code";
            dt.Columns[2].ColumnName = "Customer name";
            dt.Columns[3].ColumnName = "Order #";
            dt.Columns[4].ColumnName = "Item code";
            dt.Columns[5].ColumnName = "Item desc";
            dt.Columns[6].ColumnName = "Order qty";
            dt.Columns[7].ColumnName = "Invoice qty";
            dt.Columns[8].ColumnName = "Short qty";
            dt.Columns[9].ColumnName = "Total";
            return dt;
        }

        public DataTable GetReadyForAnalyzeCasefillTrend(DataTable dt, DateTime dateFrom, DateTime dateTo)
        {
            var days = Math.Abs((dateTo - dateFrom).TotalDays);
            dt.Columns[0].ColumnName = "Site";
            for (int i = 1; i <= days; i++)
            {
                dt.Columns[i].ColumnName = dateTo.AddDays(i * -1).Date.ToShortDateString();
            }
            return dt;
        }

        public AttainmentToPlanDashboardViewModel GetAttainmentToPlanDashboardViewModel(DateTime date)
        {
            var model = new AttainmentToPlanDashboardViewModel() { AtpTiles = new List<TileItem>() };
            var connectionString = ConfigurationManager.ConnectionStrings["OneCubeOlap2016"].ConnectionString;

            try
            {
                var query = Data.GenerateMDXQueryAttainmentToPlan(date);
                var dataTable = Data.MDxQueryRun(connectionString, query);
                dataTable = GetReadyForAnalyzeAttainmentToPlan(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    var atv = row[1] != null ? Convert.ToDecimal(row[1]) : 0;
                    var att = row[2] != null ? Convert.ToDecimal(row[2]) : 0;
                    var ats = row[3] != null ? Convert.ToDecimal(row[3]) : 0;
                    var fact = row[4] != null ? Convert.ToDecimal(row[4]) : 0;
                    var tile = new TileItem();
                    tile.SiteName = row[0].ToString();
                    var value = atv * att * ats;
                    value = value / fact / fact / fact;
                    tile.ProgressValue = value * 100;
                    tile.Color = "success";
                    //}
                    model.AtpTiles.Add(tile);
               
                }
                model.AtpTiles = model.AtpTiles.OrderBy(x => x.SiteName).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetAttainmentToPlanDashboardViewModel(date:{date})",date);
            }
            return model;
        }



        private DataTable GetReadyForAnalyzeAttainmentToPlan(DataTable dt)
        {
            dt.Columns[0].ColumnName = "Site";
            dt.Columns[1].ColumnName = "ATV";
            dt.Columns[2].ColumnName = "ATT";
            dt.Columns[3].ColumnName = "ATS";
            dt.Columns[4].ColumnName = "Fact ATP Count";
            return dt;
        }

        public RightFirstTimeDashboardViewModel GetRightFirstTimeDashboardViewModel(DateTime date)
        {
            var model = new RightFirstTimeDashboardViewModel() { RftPerSites = new List<TileItem>() };
            var connectionString = ConfigurationManager.ConnectionStrings["OneCubeOlap2016"].ConnectionString;
            var query = Data.GenerateMDXQueryRightFirstTime(date);
            try
            {
                var dataTable = Data.MDxQueryRun(connectionString, query);
                dataTable = GetReadyForAnalyzeRightFirstTime(dataTable);
                foreach (DataRow row in dataTable.Rows)
                {
                    var tile = new TileItem();
                    tile.SiteName = row[0].ToString();
                    var packQty = string.IsNullOrEmpty(row[1].ToString()) ? 0 : Convert.ToDecimal(row[1]);
                    var totalQty = string.IsNullOrEmpty(row[2].ToString()) ? 0 : Convert.ToDecimal(row[2]);
                    var value = totalQty!=0 ? packQty / totalQty : 0;
                    tile.ProgressValue = value * 100;
                    tile.Color = "warning";
                    model.RftPerSites.Add(tile);
                }
                model.RftPerSites = model.RftPerSites.OrderBy(x => x.SiteName).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetRightFirstTimeDashboardViewModel(date:{date})",date);
            }
            return model;
        }

        private DataTable GetReadyForAnalyzeRightFirstTime(DataTable dt)
        {
            dt.Columns[0].ColumnName = "Site";
            dt.Columns[1].ColumnName = "Packed Qty";
            dt.Columns[2].ColumnName = "Total Qty";
            return dt;
        }

        /// <summary>
        /// Launching if user hasn't setup his tiles.
        /// </summary>
        /// <returns></returns>
        public List<YieldGraphTile> GetYieldDashboardData()
        {
            var model = new List<YieldGraphTile>();
            var codeList = new string[] { "COSLT01", "COSLT02" };
            var product = "COS";
            var id = 0;
            model.Add(GetYieldGraphData(codeList, product, id, chartSettings, 0));

            codeList = new [] { "SPINB01" };
            product = "SPIN";
            id = 1;
            model.Add(GetYieldGraphData(codeList, product, id, chartSettings, 1));
            return model;
        }

        

        private YieldGraphTile GetYieldGraphData(string[] codeList,string product,int id,ChartSettings settings,int j)
        {
            const string Site = "Site";
            var model = new YieldGraphTile()
                            {
                                Header = "Yesterday’s Yield - " + product,
                                Id = id,
                                chartType = settings.chartType[j],
                                YieldDashboardItems = new List<YieldGraphItem>()
                            };
            var connectionString = ConfigurationManager.ConnectionStrings["OneCubeOlap"].ConnectionString;
            var date = DateTime.Today.AddDays(-1).Date;
            var query = Data.GenerateMDXQueryYield(date, codeList);
            try
            {
                var dataTable = Data.MDxQueryRun(connectionString, query);
                dataTable = GetReadyForAnalyzeYieldGraphData(dataTable, codeList);
                var rows = (from dr in dataTable.Rows.Cast<DataRow>() select dr[0].ToString()).ToList();
                model.Labels = rows.ToArray();
                for (var i=1; i< dataTable.Columns.Count; i++)
                {
                        var item = new YieldGraphItem
                                       {
                                           fillColor = settings.fillColor[i - 1],
                                           pointColor = settings.pointColor[i - 1],
                                           pointHighlightFill = settings.pointHighlightFill[i - 1],
                                           pointHighlightStroke = settings.pointHighlightStroke[i - 1],
                                           pointStrokeColor = settings.pointStrokeColor[i - 1],
                                           strokeColor = settings.strokeColor[i - 1],
                                           Points = new List<TileItem>(),
                                           productCode = codeList[i-1]
                                       };
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            var point = new TileItem() { SiteName = dataRow[0].ToString() };
                            var value = dataTable.AsEnumerable()
                                .Where(x =>x.Field<string>(Site) == dataRow[0].ToString())
                                .Select(x => x.Field<object>(codeList[i-1]))
                                .FirstOrDefault();
                            decimal decValue;
                            point.ProgressValue = decimal.TryParse(value.ToString(), out decValue) ? decValue*100 : 0;
                            item.Points.Add(point);
                        }
                        item.Data = item.Points.Select(x => Convert.ToByte(x.ProgressValue)).ToArray();
                        model.YieldDashboardItems.Add(item);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetYieldGraphData(product:{product})",product);
            }
            return model;
        }

        private YieldGraphTile GetYieldLineGraphData(string[] codeList, string product, int id, ChartSettings settings, int j)
        {
            var model = new YieldGraphTile()
            {
                Header = "Yield - " + product,
                Id = id,
                chartType = settings.chartType[j],
                YieldDashboardItems = new List<YieldGraphItem>()
            };
            var connectionString = ConfigurationManager.ConnectionStrings["OneCubeOlap"].ConnectionString;
            var date = DateTime.Today.Date;
            var query = Data.GenerateMDXQueryDateRangeYield(date, codeList);
            try
            {
                var dataTable = Data.MDxQueryRun(connectionString, query);
                var rows = (from dr in dataTable.Rows.Cast<DataRow>() select dr[0].ToString()).ToList();
                model.Labels = rows.ToArray();
                for (var i = 1; i < dataTable.Columns.Count; i++)
                {
                    var item = new YieldGraphItem
                    {
                        fillColor = settings.borderColor[i - 1],
                        pointColor = settings.backgroundColor[i - 1],
                        pointHighlightFill = settings.pointHoverBorderColor,
                        pointHighlightStroke = settings.pointHoverBackgroundColor,
                        Points = new List<TileItem>(),
                        productCode = ConvertColumnName(dataTable.Columns[i].ColumnName)
                    };

                    for (var f =0; f <= dataTable.Rows.Count - 1; f++)
                    {
                        var point = new TileItem() { PointName = dataTable.Rows[f][0].ToString() };
                        decimal decValue;
                        point.ProgressValue = decimal.TryParse(dataTable.Rows[f][i].ToString(), out decValue) ? Math.Round(decValue * 100, 2) : 0;
                        if (point.ProgressValue > byte.MaxValue) point.ProgressValue = byte.MaxValue;
                        item.Points.Add(point);
                    }

                    item.Data = item.Points.Select(x => Convert.ToByte(x.ProgressValue)).ToArray();
                    model.YieldDashboardItems.Add(item);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetYieldGraphData(product:{product})", product);
            }
            return model;
        }

        private DataTable GetReadyForAnalyzeYieldGraphData(DataTable dt, string[] codes)
        {
            dt.Columns[0].ColumnName = "Site";
            for (var i = 0; i < codes.Length; i++)
            {
                dt.Columns[i+1].ColumnName = codes[i];
            }
            return dt;
        }

        private string ConvertColumnName(string columnName)
        {
            var sbOpen = '[';
            var sbClose = ']';
            var charArray = columnName.ToCharArray();
            var isData = false;
            var columnPureName = string.Empty;
                for(var j = 0; j < charArray.Length; j++)
                {
                    if(isData) {
                        if (charArray[j]==sbOpen)
                        {
                            while (charArray[j+1]!=sbClose)
                            {
                                j++;
                                columnPureName += charArray[j];
                            }
                            columnPureName += ".";
                            isData = false;
                        }
                    }
                    if(charArray[j] == '&') isData = true;
                }
                return columnPureName.TrimEnd('.');
        }

        public List<NewsViewModel> GetDashboardNews_Published()
        {
            var model = new List<NewsViewModel>();
            var list = _dNewsRepository.Get(x => !x.IsDeleted && x.IsPublished).ToList();
            foreach (var item in list)
            {
                var news = new NewsViewModel()
                               {
                                   Title = item.Title,
                                   Text = item.Text,
                                   ImageSrc = item.ImagePath,
                                   DateTime = item.DateCreated,
                                   IconColor = item.IconColor,
                                   Icon = item.Icon,
                                   IsPublished = item.IsPublished,
                               };
                model.Add(news);
            }
            return model;
        }

        public UserTilesForDashboard GetUserDashboard()
        {
            var userSettings =
                _userRepository.Get(x => x.EmployeeId == CurEmpId)
                    .Include(x => x.UserDashboardSettings)
                    .Include(x => x.UserDashboardSettings.DUserGroup)
                    .Select(x => x.UserDashboardSettings)
                    .FirstOrDefault();
            var model = new UserTilesForDashboard()
            {
                NotificationQty = 0,
                TilesDictionary = new Dictionary<TileStyle, List<UserTile>>()
            };

            if (userSettings == null) return model; //if null return nothing

            var myTileIds = userSettings.MyTiles.ToList();
            myTileIds.AddRange(userSettings.DUserGroup.RequiredTiles.ToList());

            if(!userSettings.MyTiles.Any() && userSettings.DUserGroup.DefaultTiles!=null) myTileIds.AddRange(userSettings.DUserGroup.DefaultTiles.ToList()); //if User doesn't has the "My Tiles", then fetch group Default tiles

            var tileList = _dTileRepository.Get(x => !x.IsDeleted && myTileIds.Contains(x.Id)).ToList();
           
            foreach (var style in Enum.GetValues(typeof(TileStyle)).Cast<TileStyle>().ToList())
            {
                var userTileList = new List<UserTile>();
                foreach (var tile in tileList.Where(x=>x.TileStyle== style).ToList())
                {
                    var userTile = new UserTile()
                                       {
                                           Id = tile.Id,
                                           Name = tile.Name,
                                           Style = style,
                                           Type = tile.TileType,
                                           SubType = tile.TileSubType,
                                           UserTileData = new UserTileData()
                                       };
                    switch (userTile.Type)
                    {
                        case TileType.EPO:
                            userTile.UserTileData.EpoTile = Data.InitEpoTileData(tile);
                            break;
                        case TileType.TNA:
                            userTile.UserTileData.TnATile = Data.InitTnaTileData(tile);
                            break;
                        case TileType.Casefill:
                            userTile.UserTileData.CasefillBudgetTile = Data.InitCasefillTileData(tile);
                            break;
                        case TileType.ATP:
                            userTile.UserTileData.ATPTile = InitATPTileData(tile);
                            break;
                        case TileType.RFT:
                            userTile.UserTileData.RFTTile = InitRFTTileData(tile);
                            break;
                        case TileType.Yield:
                            userTile.UserTileData.YieldGraphTile = InitYiledTileData(tile);
                            break;
                        case TileType.SMS:
                            userTile.UserTileData.SmsTile = Data.InitSmsTileData(tile);
                            break;
                        default: break;
                    }
                    userTileList.Add(userTile);
                }
                if(userTileList.Any()) model.TilesDictionary.Add(style,userTileList);
            }

            return model;
        }

        private YieldGraphTile InitYiledTileData(DTile tile)
        {
            return GetYieldTilebySubType(tile.TileSubType);
        }

        private YieldGraphTile GetYieldTilebySubType(TileSubType tileSubType)
        {
            try
            {
                string[] codeList;
                string product;
                var id = 0;
                switch (tileSubType)
                {
                    case TileSubType.YYCOS:
                        codeList = new string[] { "COSLT01", "COSLT02" };
                        product = "COS";
                        id = 0;
                        return GetYieldGraphData(codeList, product, id, chartSettings, 0);
                    case TileSubType.YYSPIN:
                        codeList = new[] { "SPINB01" };
                        product = "SPIN";
                        id = 1;
                        return GetYieldGraphData(codeList, product, id, chartSettings, 0);
                    case TileSubType.YLCOS:
                        codeList = new[] { "COSLT01", "COSLT02" };
                        product = "COS";
                        id = 2;
                        return GetYieldLineGraphData(codeList, product, id, chartSettings, 2);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Main.Dashboard.GetYieldTilebySubType(type:{tileSubType})",tileSubType);
            }
            return new YieldGraphTile();
        }

        private RightFirstTimeDashboardViewModel InitRFTTileData(DTile tile)
        {
            return GetRFTTilebySubType(tile.TileSubType);
        }

        private RightFirstTimeDashboardViewModel GetRFTTilebySubType(TileSubType tileSubType)
        {
            try
            {
                switch (tileSubType)
                {
                    case TileSubType.YRFT:
                        return GetRightFirstTimeDashboardViewModel(DateTime.Today.AddDays(-1).Date);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Data.Dashboard.GetRFTTilebySubType({tileSubType})", tileSubType);
            }
            return new RightFirstTimeDashboardViewModel();
        }

        public AttainmentToPlanDashboardViewModel InitATPTileData(DTile tile)
        {
            var atpTile = GetATPTilebySubType(tile.TileSubType);
            return atpTile;
        }

        private AttainmentToPlanDashboardViewModel GetATPTilebySubType(TileSubType tileSubType)
        {
            try
            {
                switch (tileSubType)
                {
                    case TileSubType.YATP:
                        return GetAttainmentToPlanDashboardViewModel(DateTime.Today.AddDays(-1).Date);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Data.Dashboard.GetATPTilebySubType", e);
            }
            return new AttainmentToPlanDashboardViewModel();
        }

        
    }
}
