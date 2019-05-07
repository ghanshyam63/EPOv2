using System;

namespace EPOv2.Business
{
    using EPOv2.Business.Interfaces;
    public partial class Data:IData
    {
        public string GenerateMDXQueryAttainmentToPlan(DateTime date)
        {
            var dt = string.Format("{0:s}", date);
            var query = @"select {[Measures].[ATV], [Measures].[ATT], [Measures].[ATS], [Measures].[Fact ATP Count]} on columns,
                            {[Dim ATP Sites].[Site].&[BDL], [Dim ATP Sites].[Site].&[BNE], [Dim ATP Sites].[Site].&[PTH], [Dim ATP Sites].[Site].&[SYD]} on rows
                            FROM [ATP]
                            WHERE([Base Financial Calendar 3].[Financial Date].&["+dt+"])";
            return query;
        }

        public string GenerateMDXQueryRightFirstTime(DateTime date)
        {
            var dt = String.Format("{0:s}", date);
            var query = @"select {[Measures].[Packed Qty],[Measures].[Total Qty]} on columns,
                        {[Dim Giveaway Sites].[Site].&[BDL], [Dim Giveaway Sites].[Site].&[BNE], [Dim Giveaway Sites].[Site].&[PTH], [Dim Giveaway Sites].[Site].&[SYD]} on rows
                        FROM [Giveaway] 
                        WHERE ([Base Financial Calendar 2].[Financial Date].&["+dt+"])";
            return query;
        }

        public string GenerateMDXQueryYield(DateTime date, string[] codeList)
        {
            var dt = String.Format("{0:s}", date);
            var query = @"select CROSSJOIN ({";
            for (var i = 0; i < codeList.Length; i++)
            {
                if (i != 0) query +=", ";
                query += @"[Component].[Part].&[" + codeList[i] + "]";
            }
            query += @"}, {[Measures].[Actual Yield]} ) ON COLUMNS,
                        non empty [Site].[Site].[Site] ON ROWS
                        FROM [Processing]
                        WHERE [Financial Calendar].[Date].&[" + dt + "]";
            return query;
        }

        public string GenerateMDXQueryDateRangeYield(DateTime startDate, string[] codeList)
        {
            startDate = startDate.AddDays(-1);
            
            var query = @"select CROSSJOIN ({[Site].[Site].&[BDL],[Site].[Site].&[BNE],[Site].[Site].&[PTH],[Site].[Site].&[SYD]},{ ";
            
                for (var i = 0; i < codeList.Length; i++)
                {
                    if (i != 0) query += ", ";
                    query += @"[Component].[Part].&[" + codeList[i] + "]";
                }
            query += @"}) ON COLUMNS, NON EMPTY {";

            for (var j = 5; j >=0; j--)
            {
                var dt = string.Format("{0:s}", startDate.AddDays(j * (-1)));
                query += @"[Financial Calendar].[Date].&[" + dt + "]";
                if (j != 0) query += ", ";
            }

            query += @"} ON ROWS 
                        FROM [Processing] WHERE ([Measures].[Actual Yield])";

            return query;
        }
    }
}
