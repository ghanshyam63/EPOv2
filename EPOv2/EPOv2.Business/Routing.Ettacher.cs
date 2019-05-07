using System;
using System.Collections.Generic;
using System.Linq;

namespace EPOv2.Business
{
    using System.Data.Entity;
    using System.IO;

    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;

    public partial class Routing
    {/// <summary>
     /// 
     /// </summary>
     /// <param name="DocumentTypeId"></param>
     /// <returns>True if Invoice/EPO | False if GRNI/Scan PO </returns>
        public bool CheckAttachedDocumentStatus(int DocumentTypeId)
        {
            switch (DocumentTypeId)
            {
                case 1:
                    return true;
                case 2:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Check voucher for both type of Docs Invoice and EPO. If so than begin routing logic.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ReturnResutViewModel TryToStartVoucherRouting(VoucherAttachingFormViewModel model)
        {
            var voucherTotal = 0d;
            var voucher = _voucherRepository.Get(x => x.VoucherNumber == model.VoucherNumber).FirstOrDefault();
            if (voucher == null) return _main.SaveVoucherAttachForm(model);
            var existingDocs = _voucherDocumentRepository.Get(x => !x.IsDeleted && x.Voucher.Id == voucher.Id).Include(x => x.DocumentType).ToList();
            if ((model.SelectedDocumentTypeId == 1
                 && existingDocs.Select(x => x.DocumentType.Name)
                        .ToList()
                        .Contains(_main.GetVoucherDocumentTypeName(DocumentTypeEnum.Purchase_Order)))
                || (model.SelectedDocumentTypeId == 2
                    && existingDocs.Select(x => x.DocumentType.Name)
                           .ToList()
                           .Contains(_main.GetVoucherDocumentTypeName(DocumentTypeEnum.Invoice))))
            {
                var orderId = GetOrderIdAsRefFromAttachedDocs(model, existingDocs);
                if (CheckVoucherVariance(voucher.Id, Convert.ToInt32(orderId),ref voucherTotal)) return StartVoucherRouting(voucher, model, existingDocs, Convert.ToInt32(orderId),voucherTotal);
            }
            return _main.SaveVoucherAttachForm(model);
        }

        private string GetOrderIdAsRefFromAttachedDocs(VoucherAttachingFormViewModel model, List<VoucherDocument> existingDocs)
        {
            return model.SelectedDocumentTypeId == 2 ? model.SelectedDocument : existingDocs.Where(x => x.DocumentType.Id == 2).Select(x => x.Reference).First();
        }

        private bool CheckVoucherVariance(int voucherId, int orderId, ref double voucherTotal)
        {
            //voucherTotal = _voucherRepository.Get(x => !x.IsDeleted && x.Id == voucherId).Select(x => x.Amount).Sum();
            voucherTotal = _main.FindRelatedVoucher(orderId.ToString(), voucherId).Select(x => x.Amount).Sum();
            var orderTotal = _orderRepository.Get(x => x.Id == orderId).Select(x => x.Total).FirstOrDefault();
            return voucherTotal > (orderTotal + 1); //$1 margin
        }

        private ReturnResutViewModel StartVoucherRouting(Voucher voucher, VoucherAttachingFormViewModel model, IEnumerable<VoucherDocument> existingDocuments, int orderId, double voucherTotal)
        {
            var fileName = Main.GenerateFileName(model);
            var voucherDocument = _main.CreateUpdateVoucherDocument(model, fileName, voucher);
            if (voucherDocument.DocumentType.Name == DocumentTypeEnum.Invoice.ToString())
            {
                voucherDocument.Reference = fileName;
                if (!File.Exists(Path.Combine(Main.UnlinkedPath, model.SelectedDocument)))
                {
                    return new ReturnResutViewModel()
                               {
                                   Status = "Error",
                                   Message = "File not exist, please check folder."
                               };
                }
                _output.SaveInvoiceFile(model, voucherDocument, fileName);
            }
            if (voucherDocument.Id == 0) _voucherDocumentRepository.Add(voucherDocument);

            var vDocList = new List<VoucherDocument>();
            vDocList.AddRange(existingDocuments);
            vDocList.Add(voucherDocument);

            var order = _orderRepository.Get(x=>x.Id==orderId).Include(x=>x.CostCentre).First();
            _orderCostCentre = this.IsCapex ? this._data.GetCostCentreForCapex(order.Capex_Id) : order.CostCentre;
            var ownerEmpId = order.CostCentre.Owner.UserInfo.EmployeeId;
            var ownerData = this.GetRockieLevelData(ownerEmpId.ToString());
            if (ownerData == null) { return new ReturnResutViewModel() { Status = "Error", Message = "Please contact to Mitchell Kilkeary, Cost center("+order.CostCentre.Code+") owner doesn't exist" }; }
            var ownerlevel = Convert.ToInt32(ownerData.Level);
            var owner = this.GetOwnerViewModel(order, ownerlevel);
            //Getting approvers
            GreaterWayForInvoice(owner, voucher, order,voucherTotal);
            //Check and use subs
            _data.CheckForSubstitutionForVoucher(voucher);
            //Change status on voucher and docs
            _main.SetAuthoriserForVoucherAndChangeStatus(voucher, vDocList);


            return new ReturnResutViewModel() { Status = "Success", Message = "Attached"};
        }

        

        public void GreaterWayForInvoice(OwnerViewModel owner, Voucher voucher, Order order, double voucherTotal)
        {
            //Getting list of 2 approvers. CC owner plus one above him
            try
            {
                var approver = new Approver()
                                   {
                                       User = owner.User,
                                       Level = owner.Level,
                                       Limit = owner.Limit,
                                   };
                _approverRepository.Add(approver);
                CreateVoucherRoute(voucher, approver, 1);
                if(voucherTotal>owner.Limit) this.GetApproverAboveOwnerForVoucher(owner, order, voucher);
                this.Db.SaveChanges();
            }
            catch (Exception e)
            {
                _main.LogError("Routing.GreaterWayForInvoice(voucher:"+voucher.VoucherNumber+")", e);
            }

        }

        private void CreateVoucherRoute(Voucher voucher, Approver approver, int number)
        {
            var exRoute = this._voucherRouteRepository.Get(x => x.Voucher.Id == voucher.Id && x.Number == number).FirstOrDefault();
            if (exRoute != null)
            {
                exRoute.Number = number;
                exRoute.Approver = approver;
                //exRoute.LastModifiedBy = this._curUser;
                //exRoute.LastModifiedDate = DateTime.Now;
                exRoute.IsDeleted = false;
                _voucherRouteRepository.Update(exRoute);

            }
            else {
                var route = new VoucherRoute()  
                {
                    Voucher = voucher,
                    Number = number,
                    Approver = approver,
                    //LastModifiedDate = DateTime.Now,
                    //DateCreated = DateTime.Now,
                    //CreatedBy = this._curUser,
                    //LastModifiedBy = this._curUser
                };
                this._voucherRouteRepository.Add(route);
            }
        }

        private void GetApproverAboveOwnerForVoucher(OwnerViewModel owner, Order order,Voucher voucher)
        {
            CheckApproverForVoucher(owner.EmpId, order, voucher);
        }

        private void CheckApproverForVoucher(int empId, Order order, Voucher voucher)
        {
            var isForeignCurrency = order.OrderItems[0].Currency.Id != 1;
            var totalExGst = order.TotalExGST;
            if (isForeignCurrency)
            {
                totalExGst = order.TotalExGST * order.OrderItems[0].CurrencyRate;
            }
            var manager =
                this._rockyEmployeesRepository.Get(x => x.EmpNo == empId.ToString() && x.Active == 1).Select(x => new { x.ManagerEmpNo, x.ManagerLevel }).FirstOrDefault();
            var managerLimit =
                this._levelRepository.Get(x => x.Code.ToString() == manager.ManagerLevel).Select(x => x.Value).FirstOrDefault();
            if (managerLimit > totalExGst)
            {
                var approver = new Approver()
                {
                    Level = Convert.ToInt32(manager.ManagerLevel),
                    Limit = managerLimit,
                    User =this._userRepository.Get(x => x.EmployeeId.ToString() == manager.ManagerEmpNo).FirstOrDefault(),
                };
                this._approverRepository.Add(approver);
                this.CreateVoucherRoute(voucher, approver, 2);
            }
            else
            {
                this.CheckApprover(Convert.ToInt32(manager.ManagerEmpNo), order);
            }
        }
    }
}
