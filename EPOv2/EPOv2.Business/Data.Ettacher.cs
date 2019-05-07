namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;

    using Microsoft.VisualBasic;

    public partial class Data
	{
		//E-ttacher methods

        #region Voucher Document Type

        public List<VoucherDocumentType> GetVoucherDocumentTypeList()
        {
            return _voucherDocumentTypeRepository.Get().ToList();
        }

        public void SaveVoucherDocumentType(VoucherDocumentType model)
        {
            var m = this._voucherDocumentTypeRepository.Find(model.Id);
            if (m != null)
            {
                m.Description = model.Description;
                m.Name = model.Name;
                m.LastModifiedBy = this._curUser;
                m.LastModifiedDate = DateTime.Now;
            }
            else
            {
                model.CreatedBy = this._curUser;
                model.LastModifiedBy = this._curUser;
                model.LastModifiedDate = DateTime.Now;
                model.DateCreated = DateTime.Now;
                this._voucherDocumentTypeRepository.Add(model);
            }
            this._dataContext.SaveChanges();
        }

        public VoucherDocumentType GetVoucherDocumentType(int id)
        {
            return this._voucherDocumentTypeRepository.Find(id);
        }

        public void DeleteOrActivateVoucherDocumentType(int id, bool toDelete = true)
        {
            try
            {
                var model = this._voucherDocumentTypeRepository.Find(id);
                model.LastModifiedBy = this._curUser;
                model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                this._dataContext.SaveChanges();
            }
            catch (Exception EX_NAME)
            {
                Console.WriteLine(EX_NAME);
            }
        }

        #endregion

        #region Voucher Status

        public List<VoucherStatus> GetVoucherStatusList()
        {
            return this._voucherStatusRepository.Get().ToList();
        }

        public VoucherStatus GetVoucherStatus(int id)
        {
            return this._voucherStatusRepository.Find(id);
        }

       public void SaveVoucherStatus(VoucherStatus model)
        {
            var m = this._voucherStatusRepository.Find(model.Id);
            if (m != null)
            {
                m.Description = model.Description;
                m.Name = model.Name;
                m.LastModifiedBy = this._curUser;
                m.LastModifiedDate = DateTime.Now;
            }
            else
            {
                model.CreatedBy = this._curUser;
                model.LastModifiedBy = this._curUser;
                model.LastModifiedDate = DateTime.Now;
                model.DateCreated = DateTime.Now;
                this._voucherStatusRepository.Add(model);
            }
            this._dataContext.SaveChanges();
        }

        public void DeleteOrActivateVoucherStatus(int id, bool toDelete = true)
        {
            try
            {
                var model = this._voucherStatusRepository.Find(id);
                model.LastModifiedBy = this._curUser;
                model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                this._dataContext.SaveChanges();
            }
            catch (Exception EX_NAME)
            {
                Console.WriteLine(EX_NAME);
            }
        }

        #endregion

        public VoucherDocument GetVoucherDocument(int voucherDocumentId)
        {
            return this._voucherDocumentRepository.Find(voucherDocumentId);
        }

        
        public void ApplyInvoiceSubstitution(SubstituteApprover substitute)
        {
            var currDate = DateAndTime.Now;
            if (currDate >= substitute.Start && currDate <= substitute.End)
            {
                var voucherDocList =
                    _voucherDocumentRepository.Get(
                        x =>
                        !x.IsAuthorised && !x.IsDeleted
                        && (x.Voucher.Status.Name == StatusEnum.Pending.ToString()
                            || x.Voucher.Status.Name == StatusEnum.Declined.ToString())
                        && x.Authoriser.Id == substitute.ApproverUser.Id).Include(x => x.Voucher).ToList();
                    //exact list needs to change

                foreach (var voucherDocument in voucherDocList)
                {
                    voucherDocument.oldAuthoriser = voucherDocument.Authoriser.Id;
                    voucherDocument.Authoriser = substitute.SubstitutionUser;
                    _voucherDocumentRepository.Update(voucherDocument);
                }
                _dataContext.SaveChanges();
            }
        }

        public void CancelSubstitutionForVoucher(SubstituteApprover substitute)
        {
            var voucherDocList =
                    _voucherDocumentRepository.Get(
                        x =>
                        !x.IsAuthorised && !x.IsDeleted
                        && (x.Voucher.Status.Name == StatusEnum.Pending.ToString() || x.Voucher.Status.Name == StatusEnum.Declined.ToString())
                        && x.oldAuthoriser == substitute.ApproverUser.Id).Include(x => x.Voucher).ToList();
            //exact list needs to change

            foreach (var voucherDocument in voucherDocList)
            {
                var temp = voucherDocument.Authoriser.Id;
                var olduserapprover = _userRepository.Get(x => x.Id == voucherDocument.oldAuthoriser).FirstOrDefault();
                voucherDocument.Authoriser = olduserapprover;
                voucherDocument.oldAuthoriser = olduserapprover.Id;

                // var olduserapprover = _userRepository.Get(x => x.Id == voucherDocument.oldAuthoriser).FirstOrDefault();
                // voucherDocument.Authoriser = olduserapprover;//approver.OldApprover;
                // voucherDocument.oldAuthoriser = olduserapprover.Id;
                //// voucherDocument.oldAuthoriser = voucherDocument.Authoriser.Id;
                //// voucherDocument.Authoriser = substitute.ApproverUser;
                // _voucherDocumentRepository.Update(voucherDocument);
            }
        }
	}
}