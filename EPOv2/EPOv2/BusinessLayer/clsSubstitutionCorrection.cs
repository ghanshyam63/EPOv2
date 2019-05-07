using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Entity;
using System.Net.Mail;
using DomainModel.DataContext;
using EPOv2.Business;
using EPOv2.Business.Interfaces;
using DomainModel.Entities;
using Microsoft.VisualBasic;
using Quartz;
using System.Threading.Tasks;

namespace EPOv2.BusinessLayer
{
    public class ClsSubstitutionCorrection:IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => this.EpoSubStitutionCorrection());
        }



        public void EpoSubStitutionCorrection()
        {

            PurchaseOrderContext p = new PurchaseOrderContext();
           
            var subsList = p.SubstituteApprovers.Where(x => !x.IsDeleted && x.End <= DateTime.Now).ToList();
            
            var currDate = DateTime.Now.Date;
            var subsList1 = p.SubstituteApprovers.Where(x => !x.IsDeleted && currDate >= x.Start && currDate <= x.End).ToList();
            foreach (var subs in subsList)
            {
                subs.IsDeleted = true;

            }
            foreach (var substitute in subsList1)
            {
                ApplySubstitution(substitute,p);
                ApplyInvoiceSubstitution(substitute,p);
                ApplyCapexSubstitution(substitute,p);
            }
            p.SaveChanges();
            var mail = new MailMessage();
            mail.From = new MailAddress("epo@oneharvest.com.au");
            mail.To.Add(new MailAddress("sam.shah@oneharvest.com.au"));
            var client = new SmtpClient();
            mail.Subject = "EPO SUBSTITUTION CORRECTION SCHEDULED JOB from viis1";
            mail.IsBodyHtml = true;
            mail.Body = "Test";
            client.Send(mail);
            mail.Dispose();


        }
        public void ApplySubstitution(SubstituteApprover substitute,PurchaseOrderContext p)
        {
        
            var currDate = DateTime.Now;
            if (currDate >= substitute.Start && currDate <= substitute.End)
            {
                var approverList =
                    p.Approvers.Where(x => !x.IsDeleted && x.User.Id == substitute.ApproverUser.Id).ToList();
                foreach (var approver in approverList)
                {
                    var olduserapprover = approver.User;
                  //  var temp = p.Users.Where(x => x.Id == approver.User.Id).FirstOrDefault();
                    approver.User = p.Users.Where(x => x.Id == substitute.SubstitutionUser.Id).FirstOrDefault(); ;//approver.OldApprover;
                    approver.OldApprover = olduserapprover.Id;
                   
                }
                p.SaveChanges();
            }
        }
        public void ApplyInvoiceSubstitution(SubstituteApprover substitute, PurchaseOrderContext p)
        {
          
            var currDate = DateTime.Now;
            if (currDate >= substitute.Start && currDate <= substitute.End)
            {
                var voucherDocList =
                    p.VoucherDocuments.Where(
                        x =>
                            !x.IsAuthorised && !x.IsDeleted
                            && (x.Voucher.Status.Name == StatusEnum.Pending.ToString()
                                || x.Voucher.Status.Name == StatusEnum.Declined.ToString())
                            && x.Authoriser.Id == substitute.ApproverUser.Id).Include(x => x.Voucher).ToList();
                

                foreach (var voucherDocument in voucherDocList)
                {
                    var NewApprover =substitute.SubstitutionUser.Id;
                    var olduserapprover = voucherDocument.Authoriser.Id;
                    voucherDocument.Authoriser = p.Users.Where(x => x.Id == NewApprover).FirstOrDefault(); 
                    voucherDocument.oldAuthoriser = olduserapprover;
                   
                   
                }
                p.SaveChanges();
            }
        }
        public void ApplyCapexSubstitution(SubstituteApprover substitute, PurchaseOrderContext p)
        {
          
            var approverList =
                p.CapexApprovers.Where(x => !x.IsDeleted && x.User.Id == substitute.ApproverUser.Id).ToList();
            foreach (var capexApprover in approverList)
            {
                var newApprover = substitute.SubstitutionUser.Id;
                var olduserapprover = substitute.ApproverUser.Id;
                  
                capexApprover.User = p.Users.Where(x => x.Id == newApprover).FirstOrDefault(); 
                capexApprover.oldapprover = olduserapprover;

            }
            p.SaveChanges();
        }
       

    }

}