using EPOv2.Business;
using EPOv2.Business.Interfaces;
using Quartz;
using System;
using System.Net;
using System.Threading.Tasks;
using Serilog;
using EPOv2.Controllers;

namespace EPOv2.BusinessLayer
{
    public class EPOReportRunnerJob:IJob
    {
        ILogger _logger = Serilog.Log.Logger;
        public async Task Execute(IJobExecutionContext context)
        {  
            JobDataMap jobDataMap = context.JobDetail.JobDataMap;

            string Parameter  = jobDataMap.GetString("Parameter");
            await Task.Run(() => this.RunTask(Parameter));
        }

        private void RunTask(string Parameter)
        {
            switch (Parameter)
            {
                case "OutstandingInvoice":
                    RunOutstandingInvoicesReport();
                    break;
                case "DeclinedVouchers":
                    RunEttacherEmailReports();
                    break;
                case "UsersUpdate":
                    RunUsersUpdate();
                    break;
                case "CCOwnerChecker":
                    RunCostCentreOwnerChecker();
                    break;
            }
        }
        private void RunOutstandingInvoicesReport()
        {
            try
            {




                var req = WebRequest.Create(
                    string.Format("http://viis1.oneharvest.com.au/EPOv2/Report/RunOutstandingInvoiceEmailReport"));
                req.Credentials = new NetworkCredential("oneharvest\\it-service-account", "$ch3dul3d_Ta$k$");
                req.Timeout = 1000000;
                HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
                _logger.Information("EPOv2 Outstanding Invoice Report Updated Successfully.");
            }
            catch (Exception Ex)
            {
                _logger.Error("Outstanding Invoice Report Error" + Ex.ToString());
            }
        }

        private void RunEttacherEmailReports()
        {
            try
            {
                var req = WebRequest.Create(
                    string.Format("http://viis1.oneharvest.com.au/EPOv2/Report/RunEttacherEmailReports"));
                req.Timeout = 1000000;
                req.Credentials = new NetworkCredential("oneharvest\\it-service-account", "$ch3dul3d_Ta$k$");
                HttpWebResponse webResponse = (HttpWebResponse) req.GetResponse();
                _logger.Information("EPOv2 Run Email Ettacher Successfully.");
            }

            catch (Exception Ex)
            {
                _logger.Error("Run Email Ettacher Report Error" + Ex.ToString());
            }
        }

        private void RunUsersUpdate()
        {
          
            try
            {
                var req = WebRequest.Create(string.Format("http://viis1.oneharvest.com.au/EPOv2/Maintenance/DownloadUsers"));
                req.Credentials = new NetworkCredential("oneharvest\\it-service-account", "$ch3dul3d_Ta$k$");
                req.Timeout = 1000000;
                HttpWebResponse webResponse = (HttpWebResponse)req.GetResponse();
                _logger.Information("EPOv2 User Updated Successfully.");
            }
            catch(Exception Ex)
            {
                _logger.Error("User Update Error" + Ex.ToString());
            }
          

           
        }

        private void RunCostCentreOwnerChecker()
        {
            try
            {
                var req = WebRequest.Create(
                    string.Format("http://viis1.oneharvest.com.au/EPOv2/Report/RunCostCentreOwnerCheckReport"));
                req.Credentials = new NetworkCredential("oneharvest\\it-service-account", "$ch3dul3d_Ta$k$");
                req.Timeout = 1000000;
                HttpWebResponse webResponse = (HttpWebResponse) req.GetResponse();
                _logger.Information("EPOv2 Run Cost Centre Owner Checker Updated Successfully.");
            }
            catch (Exception Ex)
            {
                _logger.Error("Run Cost Centre Owner Checker Error" + Ex.ToString());
            }
        }
    }
}