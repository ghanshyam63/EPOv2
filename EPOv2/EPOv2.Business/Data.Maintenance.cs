namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Linq;

    using DomainModel.BaseClasses;
    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;
    using EPOv2.ViewModels.Interfaces;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.VisualBasic;

    public partial class Data
    {
        #region Property




        #endregion

        #region Entity

        public string GetSupplierFullName(int supplierId)
        {
            return _oldDb.tblSuppliers.Where(x => x.SupplierID == supplierId)
                            .Select(x => new { Name = x.SupplierName + " - " + x.SupplierCode })
                            .Select(x => x.Name)
                            .FirstOrDefault();
        }

        public string GetSupplierFullName(string supplierCode)
        {
            return _oldDb.tblSuppliers.Where(x => x.SupplierCode == supplierCode && x.Active)
                            .Select(x => new { Name = x.SupplierName + " - " + x.SupplierCode })
                            .Select(x => x.Name)
                            .FirstOrDefault();
        }

        public List<EntityViewModel> GetEntityViewModelList()
        {
            var modelList = new List<EntityViewModel>();
            var entities = _entityRepository.Get().ToList();
            foreach (var entity in entities)
            {
                var model = GetEntityViewModel(entity);
                modelList.Add(model);
            }
            return modelList;
        }

        private static EntityViewModel GetEntityViewModel(Entity entity)
        {
            var model = new EntityViewModel()
                            {
                                Id = entity.Id,
                                ABN = entity.ABN,
                                ACN = entity.ACN,
                                Code = entity.Code,
                                CodeNumber = entity.CodeNumber,
                                Email = entity.Email,
                                Fax = entity.Fax,
                                Name = entity.Name,
                                Phone = entity.Phone,
                                Prefix = entity.Prefix,
                                isDelete = entity.IsDeleted,
                                LastModifiedBy = entity.LastModifiedBy.Replace("ONEHARVEST\\", ""),
                                LastModifiedDate = entity.LastModifiedDate.ToShortDateString()
                            };
            return model;
        }

        public EntityViewModel GetEntityViewModelById(int id)
        {
            var entity = _entityRepository.Find(id);
            return GetEntityViewModel(entity);
        }

        public void SaveEntity(EntityViewModel passModel)
        {
            try
            {
                var model = _entityRepository.Find(passModel.Id);
                //db.Entry(model).State = EntityState.Modified;
                model.LastModifiedDate = DateTime.Now;
                model.LastModifiedBy = _curUser;
                model.Name = passModel.Name;
                model.Phone = passModel.Phone;
                model.Prefix = passModel.Prefix;
                model.IsDeleted = passModel.isDelete;
                model.ABN = passModel.ABN;
                model.ACN = passModel.ACN;
                model.Fax = passModel.Fax;
                model.Email = passModel.Email;
                model.CodeNumber = passModel.CodeNumber;
                model.Code = passModel.Code;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SaveEntity(Entity model)
        {
            try
            {
                var entity = new Entity()
                                 {
                                     ABN = model.ABN,
                                     ACN = model.ACN,
                                     Code = model.Code,
                                     Email = model.Email,
                                     Fax = model.Fax,
                                     Name = model.Name,
                                     Phone = model.Phone,
                                     Prefix = model.Prefix,
                                     CodeNumber = model.CodeNumber,
                                     LastModifiedBy = _curUser,
                                     LastModifiedDate = DateTime.Now,
                                     CreatedBy = _curUser,
                                     DateCreated = DateTime.Now
                                 };
                _entityRepository.Add(entity);
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void DeleteOrActivateEntity(int id, bool toDelete = true)
        {
            try
            {
                var model = _entityRepository.Find(id);
                //this.db.Entry(model).State = EntityState.Modified;
                model.LastModifiedBy = _curUser;
                model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public EntityDDLViewModel GetEntityDDLVM()
        {
            var model = new EntityDDLViewModel() { SelectedEntity = 0, Entities = GetEntityViewModelList() };
            return model;
        }

        #endregion


        #region Cost Centre

        public List<CostCentreToEntityViewModel> GetCostCentreToEntityViewModelList(int entityId)
        {
            var list =
                _costCentreToEntityRepository.Get(x => x.Entity.Id == entityId).OrderBy(x => x.CostCentre.Code).ToList();
            return
                list.Select(
                    item =>
                    new CostCentreToEntityViewModel()
                        {
                            isDeleted = item.IsDeleted,
                            CostCentre = item.CostCentre.Code + " - " + item.CostCentre.Name,
                            Id = item.Id,
                            LastModifiedDate = item.LastModifiedDate.ToString()
                        }).ToList();
        }

        public void DeleteOrActivateCostCentreToEntityMap(int id, bool toDelete)
        {
            try
            {
                var model = _costCentreToEntityRepository.Find(id);
                //db.Entry(model).State = EntityState.Modified;
                model.IsDeleted = toDelete;
                model.LastModifiedBy = _curUser;
                model.LastModifiedDate = DateTime.Now;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public CostCentreToEntityAdding GetCostCentreVMFilteredList(int entityId)
        {
            var ccMappedList =
                _costCentreToEntityRepository.Get(x => x.Entity.Id == entityId).Select(x => x.CostCentre).ToList();

            var ccList = _costCentreRepository.Get().ToList();

            var filteredList =
                ccList.Except(ccMappedList)
                    .Select(
                        item =>
                        new CostCentreViewModel()
                            {
                                FullName = item.Code + " - " + item.Name,
                                Id = item.Id,
                                Code = item.Code
                            })
                    .ToList();
            var model = new CostCentreToEntityAdding() { SelectedEntity = entityId, CostCentres = filteredList };
            return model;
        }

        public void SaveCostCentreToEntityMapping(CostCentreToEntityAdding model)
        {
            var entity = _entityRepository.Find(Convert.ToInt32(model.SelectedEntity));
            foreach (var ccId in model.SelectedCostCentres)
            {
                var cc = _costCentreRepository.Find(Convert.ToInt32(ccId));

                var map = new CostCentreToEntity()
                              {
                                  Entity = entity,
                                  CostCentre = cc,
                              };
                _costCentreToEntityRepository.Add(map);
            }
            _dataContext.SaveChanges();
        }



        public CostCentre GetCostCentre(int id)
        {
            return _costCentreRepository.Find(id);
        }

        public Entity GetEntity(int id)
        {
            return _entityRepository.Find(id);
        }

        public Group GetGroup(int id)
        {
            return _groupRepository.Find(id);
        }

        public DeliveryAddress GetDeliveryAddress(int id)
        {
            return _deliveryAddressRepository.Find(id);
        }

        

        public void SaveCostCentre(CostCentre passModel)
        {
            try
            {
                var model = _costCentreRepository.Find(passModel.Id);
                if (model != null)
                {
                    model.Name = passModel.Name;
                    model.Code = passModel.Code;
                    model.IsDeleted = passModel.IsDeleted;
                }
                else
                {
                    model = new CostCentre()
                                {
                                    Code = passModel.Code,
                                    Name = passModel.Name,
                                    IsDeleted = passModel.IsDeleted,
                                };
                    _costCentreRepository.Add(model);
                }
                _dataContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine(
                        "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name,
                        eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public void DeleteOrActivateCostCentre(int id, bool toDelete = true)
        {
            try
            {
                var model = _costCentreRepository.Find(id);
                //this.db.Entry(model).State = EntityState.Modified;
                model.LastModifiedBy = _curUser;
                model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion


        public Account GetAccount(int id)
        {
            return _accountRepository.Find(id);
        }

        public void SaveAccount(Account passModel)
        {
            try
            {
                var model = _accountRepository.Find(passModel.Id);

                if (model != null)
                {
                    model.Name = passModel.Name;
                    model.Code = passModel.Code;
                    model.Type = passModel.Type;
                    model.IsDeleted = passModel.IsDeleted;
                    _accountRepository.Update(model);
                }
                else
                {
                    model = new Account()
                                {
                                    Code = passModel.Code,
                                    Name = passModel.Name,
                                    Type = passModel.Type,
                                    IsDeleted = passModel.IsDeleted
                                };
                    _accountRepository.Add(model);
                }
                _dataContext.SaveChanges();

            }
            catch (DbEntityValidationException e)
            {
               _logger.Error(e, "SaveAccount");
            }
        }

        public void DeleteOrActivateAccount(int id, bool toDelete=true)
        {
            try
            {
                var model = _accountRepository.Find(id);
                //this.db.Entry(model).State = EntityState.Modified;
                model.LastModifiedBy = _curUser;
                model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public List<AccountToCostCentreViewModel> GetAccountToCCViewModelList()
        {
            var list = _accountToCostCentreRepository.Get().OrderBy(x => x.CostCentre.Code).ThenBy(x => x.Account.Code).ToList();
            return list.Select(item => new AccountToCostCentreViewModel() { isDeleted = item.IsDeleted, Account = item.Account.Code + " - " + item.Account.Name, CostCentre = item.CostCentre.Code + " - " + item.CostCentre.Name, Id = item.Id, AccountType = item.Account.Type==1 ? "SubAccount": String.Empty})
                .ToList();
        }

        public List<AccountToCostCentreViewModel> GetAccountToCCViewModelList(int costCentreId)
        {
            var list = _accountToCostCentreRepository.Get(x=>x.CostCentre.Id==costCentreId).OrderBy(x => x.Account.Code).ToList();
            return list.Select(item => new AccountToCostCentreViewModel() { isDeleted = item.IsDeleted, Account = item.Account.Code + " - " + item.Account.Name, Id = item.Id, LastModifiedDate = item.LastModifiedDate.ToString(), AccountType = item.Account.Type == 1 ? "SubAccount" : String.Empty })
                .ToList();
        }

        public List<AccountViewModel> GetAccountViewModelList(int type=0)
        {
            var list = _accountRepository.Get(x => x.Type == type).OrderBy(x => x.Code).ToList();
            return list.Select(account => new AccountViewModel() { Code = account.Code, Id = account.Id, FullName = account.Code + " - " + account.Name }).ToList();
        }

        public AccountToCostCentreAdding GetAccountVMFilteredList(int costCentreId, int type = 0)
        {
            var accMappedList = _accountToCostCentreRepository.Get(x => x.CostCentre.Id == costCentreId).Select(x=>x.Account).ToList();

            var accList = _accountRepository.Get(x => x.Type == type && !x.IsDeleted).ToList();

            var filteredList= accList.Except(accMappedList).OrderBy(x=>x.Code).Select(item => new AccountViewModel() { FullName = item.Code + " - " + item.Name, Id = item.Id, Code = item.Code })
                .ToList();
            var model = new AccountToCostCentreAdding() { SelectedCostCentre = costCentreId, Accounts = filteredList };
            return model;
        }



        public CostCentreDDLViewModel GetCostCentresDDLVM()
        {
            var model = new CostCentreDDLViewModel()
                            {
                                SelectedCostCentre = 0,
                                CostCentres = GetCostCentresVM(true)
                            };
            return model;
        }

        public void DeleteOrActivateAccountToCCMap(int id,bool toDelete)
        {
            try
            {
                var model = _accountToCostCentreRepository.Find(id);
                //this.db.Entry(model).State=EntityState.Modified;
                model.IsDeleted = toDelete;
                model.LastModifiedBy = _curUser;
                model.LastModifiedDate = DateTime.Now;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public AccountToCostCentreAdding GetAccountToCCAddingViewModel()
        {
            var ccDdl = GetCostCentresDDLVM();
            var list = _accountToCostCentreRepository.Get().ToList();
            return null;
        }

        public void SaveAccountToCCMapping(AccountToCostCentreAdding model)
        {
            var cc = _costCentreRepository.Find(Convert.ToInt32(model.SelectedCostCentre));
            foreach (var accountId in model.SelectedAccounts)
            {
                var account = _accountRepository.Find(Convert.ToInt32(accountId));
                var map = new AccountToCostCentre()
                              {
                                  Account = account,
                                  CostCentre = cc,
                                  CreatedBy = _curUser,
                                  LastModifiedBy = _curUser,
                                  DateCreated = DateTime.Now,
                                  LastModifiedDate = DateTime.Now
                              };
                _accountToCostCentreRepository.Add(map);
            }
            _dataContext.SaveChanges();
        }


        #region Substitution

        public SubstituteApproverViewModel GetSubstituteApproverViewModel()
        {
            _logger.Error("testing Mail server");
            var model = new SubstituteApproverViewModel();
            model.Approver = _userRepository.Get(x=>x.UserName== _curUser).Select(x=> new {fullname =x.UserInfo.FirstName + " " + x.UserInfo.LastName }).Select(x=>x.fullname).FirstOrDefault();
            model.SelectedApprover = _ad.CurrentUser.Id;
            model.ApproverList = GetUserViewModels();
            model.SubtitutionList = model.ApproverList;
            return model;
        }

        public void SaveSubstituteApprover(SubstituteApproverViewModel model)
        {
            try
            {
                var dateStart = Convert.ToDateTime(model.StartDate);
                var dateEnd = model.isPermanent? DateTime.MaxValue : Convert.ToDateTime(model.EndDate).AddHours(23).AddMinutes(59).AddSeconds(59);
                var subsList =
                    _substituteApproverRepository.Get(
                        x =>
                        x.ApproverUser.Id == model.SelectedApprover && !x.IsDeleted
                        && ((dateStart >= x.Start && dateStart <= x.End) || (dateEnd <= x.End && dateEnd >= x.Start)
                            || (dateStart <= x.Start && dateEnd >= x.End))).ToList();
                    // && x.SubstitutionUser.Id == model.SelectedSubstitution
                foreach (var subs in subsList)
                {
                    //this.db.Entry(subs).State = EntityState.Modified;
                    subs.IsDeleted = true;
                    subs.LastModifiedBy = _curUser;
                    subs.LastModifiedDate = DateTime.Now;
                    _dataContext.SaveChanges();
                }

                var substitute = new SubstituteApprover()
                                     {
                                         ApproverUser = _userRepository.Find(model.SelectedApprover),
                                         SubstitutionUser = _userRepository.Find(model.SelectedSubstitution),
                                         Start = dateStart,
                                         End = dateEnd,
                                         CreatedBy = _curUser,
                                         LastModifiedBy = _curUser,
                                         DateCreated = DateTime.Now,
                                         LastModifiedDate = DateTime.Now
                                     };
                _substituteApproverRepository.Add(substitute);
                _dataContext.SaveChanges();
                ApplySubstitution(substitute);
                ApplyInvoiceSubstitution(substitute);
                ApplyCapexSubstitution(substitute);
                ApplyOldEPOSubsitution(substitute);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ApplyOldEPOSubsitution(SubstituteApprover substitute)
        {
            var currDate = DateAndTime.Now;
            if (currDate >= substitute.Start && currDate <= substitute.End)
            {
                var item = new tblRoutingSubstitute()
                               {
                                   AuthoriserName =
                                       @"oneharvest\" + substitute.ApproverUser.UserName,
                                   SubstituteName =
                                       @"oneharvest\" + substitute.SubstitutionUser.UserName,
                                   UserID = _curUser,
                                   active = true,
                                   StartDate = substitute.Start,
                                   FinishDate = substitute.End,
                                   Created = DateTime.Now,
                                   Updated = DateTime.Now
                               };
                _oldDb.tblRoutingSubstitutes.Add(item);
                _oldDb.SaveChanges();
            }
        }

        public void ApplySubstitution(SubstituteApprover substitute)
        {
         
            var currDate = DateAndTime.Now;
            if (currDate >= substitute.Start && currDate <= substitute.End)
            {
                var approverList =
                    _approverRepository.Get(x => !x.IsDeleted && x.User.Id == substitute.ApproverUser.Id).ToList();
                foreach (var approver in approverList)
                {
                    approver.OldApprover = approver.User.Id;
                   approver.User = substitute.SubstitutionUser;
                  
                    _approverRepository.Update(approver);
                }
                _dataContext.SaveChanges();
            }
        }

        public List<SubstituteApproverTableViewModel> GetSubstituteApproverTableVMList(string approverId)
        {
            var list = new List<SubstituteApproverTableViewModel>();
            var modelList = _substituteApproverRepository.Get(x => x.ApproverUser.Id == approverId && !x.IsDeleted).ToList();
            foreach (var subs in modelList)
            {
                var item = new SubstituteApproverTableViewModel()
                               {
                                   Id = subs.Id,
                                   Start = subs.Start.ToShortDateString(),
                                   End = subs.End.ToShortDateString(),
                                   UpdatedBy =
                                       GetUserFullName(
                                           _ad.GetUserIdByLogin(subs.LastModifiedBy)),
                                   UpdatedDate = subs.LastModifiedDate.ToString(),
                                   Substitution =
                                       GetUserFullName(subs.SubstitutionUser.Id)
                               };
                list.Add(item);
            }
            return list;
        }

        public void DeleteSubstitution(int sustitutionId)
        {
            var model = _substituteApproverRepository.Find(sustitutionId);
            model.IsDeleted = true;
            _substituteApproverRepository.Update(model);
            _dataContext.SaveChanges();
            CancelSubstitution(model);
        }

        public void CancelSubstitution(SubstituteApprover substitute)
        {
            try
            {
                var currDate = DateAndTime.Now;
                if (currDate >= substitute.Start && currDate <= substitute.End)
                {
                    var approverList =_approverRepository.Get(x => !x.IsDeleted && x.User.Id == substitute.SubstitutionUser.Id).ToList();
                    foreach (var approver in approverList)
                    {
                        var olduserapprover = _userRepository.Get(x => x.Id == approver.OldApprover).FirstOrDefault();
                        approver.User = olduserapprover;//approver.OldApprover;
                        approver.OldApprover = olduserapprover.Id;
                        //approver.User = substitute.ApproverUser;
                        approver.LastModifiedBy = _curUser;
                        approver.LastModifiedDate = DateTime.Now;
                    }
                    CancelSubstitutionForVoucher(substitute);
                    CancelSubstitutionForCapex(substitute);
                    _dataContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e,"CancelSubstitution(subsId:{substituteId}, approver:{ApproverUser}", substitute.Id, substitute.ApproverUser.GetFullName());
            }
        }

        public void CheckForSubstitution(Order order)
        {
            var currDate = DateAndTime.Now;
            var approverList = GetActiveApproverList(order.Id);
            foreach (var approver in approverList)
            {
                var subsList =
                    _substituteApproverRepository.Get(x => x.ApproverUser.Id == approver.User.Id && !x.IsDeleted).ToList();
                var subs = subsList.FirstOrDefault(x => currDate >= x.Start && currDate <= x.End);
                if (subs != null)
                {
                    subs = CheckForCascadeSubstitute(subs);
                    if (subs != null)
                    {
                        MakeApproverSubstitution(approver, subs);
                    }
                }
                DeleteExpiredSubstitutions(subsList);
            }
        }

        public void CheckForSubstitutionForVoucher(Voucher voucher)
        {
            var currDate = DateAndTime.Now;
            var approverList = GetActiveVoucherApproverList(voucher.Id);
            foreach (var approver in approverList)
            {
                var subsList =
                    _substituteApproverRepository.Get(x => x.ApproverUser.Id == approver.User.Id && !x.IsDeleted).ToList();
                var subs = subsList.FirstOrDefault(x => currDate >= x.Start && currDate <= x.End);
                if (subs != null)
                {
                    subs = CheckForCascadeSubstitute(subs);
                    if (subs != null)
                    {
                        MakeApproverSubstitution(approver, subs);
                    }
                }
                DeleteExpiredSubstitutions(subsList);
            }
        }

        public User CheckAuthoriserForSubstitution(string userId)
        {
            var currDate = DateAndTime.Now;
            var approver = _userRepository.Find(userId);
            var user = approver;
                var subsList =
                    _substituteApproverRepository.Get(x => x.ApproverUser.Id == approver.Id && !x.IsDeleted).ToList();
                var subs = subsList.FirstOrDefault(x => currDate >= x.Start && currDate <= x.End);
                if (subs != null)
                {
                    subs = CheckForCascadeSubstitute(subs);
                    if (subs != null)
                    {   
                        user = subs.SubstitutionUser;
                    }
                }
                DeleteExpiredSubstitutions(subsList);
            return user;
        }

        public SubstituteApprover CheckForCascadeSubstitute(SubstituteApprover subsIn)
        {
            var currDate = DateAndTime.Now;
            var subsOut =
                _substituteApproverRepository.Get(x =>
                    !x.IsDeleted && x.ApproverUser.Id == subsIn.SubstitutionUser.Id && currDate >= x.Start
                    && currDate <= x.End).FirstOrDefault();
            if (subsOut != null)
            {
                subsIn = CheckForCascadeSubstitute(subsOut);
                return subsIn;
            }
            return subsIn;
        }

        private void MakeApproverSubstitution(Approver approver, SubstituteApprover subs)
        {
            approver.OldApprover = approver.User.Id;
            approver.User = subs.SubstitutionUser;
            _approverRepository.Update(approver);
            _dataContext.SaveChanges();
        }

        private void DeleteExpiredSubstitutions(IEnumerable<SubstituteApprover> subsList)
        {
            var list = subsList.Where(x => x.End <= DateAndTime.Now).ToList();
            foreach (var subs in list)
            {
                subs.IsDeleted = true;
                _substituteApproverRepository.Update(subs);
            }
            _dataContext.SaveChanges();
        }
        //public void DeleteExpiredSubstitutionsEveryday()
        //{
        //    var subsList =
        //        _substituteApproverRepository.Get(x=>!x.IsDeleted).ToList();
        //    var list = subsList.Where(x => x.End <= DateAndTime.Now).ToList();
        //    foreach (var subs in list)
        //    {
        //        subs.IsDeleted = true;
        //        _substituteApproverRepository.Update(subs);
        //    }
        //    _dataContext.SaveChanges();
        //}

        public void ReApplySubstitutions()
        {
            var currDate = DateAndTime.Now;
            var subsList =
                _substituteApproverRepository.Get(x => !x.IsDeleted && currDate >= x.Start && currDate <= x.End)
                    .ToList();
            foreach (var substitute in subsList)
            {
                ApplySubstitution(substitute);
                ApplyInvoiceSubstitution(substitute);
                ApplyCapexSubstitution(substitute);
            }
        }

       

        public List<Approver> GetActiveApproverList(int id)
        {
            return _routeRepository.Get(x => x.Order.Id == id && !x.Approver.IsDeleted).Select(x => x.Approver).ToList();
        }

        public List<Approver> GetActiveVoucherApproverList(int voucherId)
        {
            return _voucherRouteRepository.Get(x => x.Voucher.Id == voucherId && !x.Approver.IsDeleted).Select(x => x.Approver).ToList();
        }

        #endregion

        public List<UserViewModel> GetUserViewModelList()
        {
            var list = new List<UserViewModel>();
            var users = _userRepository.Get().ToList();
            foreach (var user in users)
            {
                try
                {
                    var item = new UserViewModel()
                                   {
                                       Id = user.Id,
                                       Name = user.UserName,
                                       Email = user.Email,
                                       EmployeeId = user.EmployeeId.ToString(),
                                       FirstName = user.UserInfo.FirstName,
                                       FullName = user.UserInfo.FirstName + " " + user.UserInfo.LastName,
                                       LastName = user.UserInfo.LastName,
                                       Login = user.UserName,
                                       Mobile = user.UserInfo.PhoneMobile,
                                       Work = user.UserInfo.PhoneWork,
                                       isDeleted = user.UserInfo.IsDeleted
                                   };
                    list.Add(item);
                }
                catch (Exception e)
                {
                    _logger.Error(e,"Data.GetUserViewModelList(userId:{userId}, userName:{userName}", user.Id,user.UserName);
                }
            }
            return list.OrderBy(x=>x.FullName).ToList();
        }
        public List<IUserViewModel> GetUserViewModelDdList()
        {
            var list = new List<IUserViewModel>();
            var users = _userRepository.Get(x => !x.UserInfo.IsDeleted).Include(x=>x.UserInfo).ToList();
            foreach (var user in users)
            {
                var item = new UserViewModeDdl()
                {
                    Id = user.Id,
                    FullName = user.UserInfo.FirstName + " " + user.UserInfo.LastName,
                };
                list.Add(item);
            }
            return list.OrderBy(x => x.FullName).ToList();
        }

        public UserEditViewModel GetUserEditViewModel(string id)
        {
            var user = _userRepository.Get(x=>x.Id==id).Include(x=>x.UserDashboardSettings).Include(x=>x.UserInfo).Single();
            var model = new UserEditViewModel()
                            {
                                Id = user.Id,
                                Name = user.UserName,
                                Email = user.Email,
                                EmployeeId = user.EmployeeId.ToString(),
                                FirstName = user.UserInfo.FirstName,
                                FullName = user.UserInfo.FirstName + " " + user.UserInfo.LastName,
                                LastName = user.UserInfo.LastName,
                                Login = user.UserName,
                                Mobile = user.UserInfo.PhoneMobile,
                                Work = user.UserInfo.PhoneWork,
                                isDeleted = user.UserInfo.IsDeleted,
                                DUserGroup = user.UserDashboardSettings?.DUserGroup?.Id,
                                SelectedUserGroup = user.UserDashboardSettings?.DUserGroup?.Id.ToString(),
                                SelectedRoles = user.Roles.Select(x=>x.RoleId).ToList()
            };
            model.UserRoles = _roleRepository.Get().Select(x => new UserRoleViewModel() { Id = x.Id, Name = x.Name }).ToList();
            model.UserGroups =
                _dUserGroupRepository.Get(x => !x.IsDeleted)
                    .Select(x => new DUserGroupViewModel() { Id = x.Id, Name = x.Name })
                    .ToList();
            return model;
        }

        #region Receipt Group

        public void DeleteOrActivateReceiptGroup(int id, bool toDelete = true)
        {
            try
            {
                var model = _groupRepository.Find(id);
                model.LastModifiedBy = _curUser;
                model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DeleteOrActivateReceiptGroupMember(int id, bool toDelete = true)
        {
            try
            {
                var model = _groupMemberRepository.Find(id);
                //model.LastModifiedBy = this._curUser;
                //model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public OrderViewModel GetOrderViewForStatusChange(int orderNumber)
        {
            var orderList = _orderRepository.Get(x=>x.OrderNumber==orderNumber).ToList();
            if (orderList.Count> 0)
            {
                var order = orderList.First();
                var model = new OrderViewModel()
                                {
                                    OrderNumber = orderNumber,
                                    OrderId = order.Id,
                                    EPOView =
                                        new SearchEPOResult()
                                            {
                                                SearchEpoResultItems =
                                                    new List<SearchEPOResultItem>()
                                            },
                                    StatusList = new List<Status>(),
                                    

            };
                model.EPOView.UserRoles = _ad.CurrentUserRoles;
                model.EPOView.SearchEpoResultItems = Main.ConvertToSearchEPOResult(orderList);
                model.StatusList.AddRange(_statusRepository.Get(x=>x.Name==StatusEnum.Receipt_Partial.ToString().Replace("_"," ") || x.Name == StatusEnum.Receipt_in_Full.ToString().Replace("_", " ") || x.Name == StatusEnum.Closed.ToString()|| x.Name == StatusEnum.Approved.ToString()).ToList());

                return model;
            }
            return new OrderViewModel() {StatusList = new List<Status>()};
        }

        public void ChangeOrderStatus(OrderViewModel model)
        {
            var order = _orderRepository.Find(model.OrderId);
            if (model.SelectedStatus != 0)
            {
                var isStatusChanged = order.Status?.Id != model.SelectedStatus;
                if (order != null)
                {
                    if (isStatusChanged) GenerateOrderLog(order, OrderLogSubject.Manual);
                    order.Status = _statusRepository.Find(model.SelectedStatus);
                    _orderRepository.Update(order);
                    _dataContext.SaveChanges();
                }
            }
        }

        public List<AccountCategory> GetAccountCategoryList()
        {
            var list = _accountCategoryRepository.Get().OrderBy(x=>x.Name).ToList();
            return list;
        }

        public void SaveAccountCategory(AccountCategory model)
        {
            var category = _accountCategoryRepository.Find(model.Id);
            if (category != null)
            {
                category.Name = model.Name;
                category.LastModifiedBy = _curUser;
                category.LastModifiedDate = DateTime.Now;
                category.IsDeleted = model.IsDeleted;
            }
            else
            {
                category = new AccountCategory()
                {
                    CreatedBy = _curUser,
                    LastModifiedBy = _curUser,
                    DateCreated = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    Name = model.Name
                };
                _accountCategoryRepository.Add(category);
            }
            _dataContext.SaveChanges();
        }

        public void DeleteOrActivateAccountCategory(int id, bool toDelete)
        {
            try
            {
                var model = _accountCategoryRepository.Find(id);
                model.LastModifiedBy = _curUser;
                model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public AccountCategory GetAccountCategory(int id)
        {
            return _accountCategoryRepository.Find(id);
        }

        public List<AccountToCategoryMatrixViewModel> GetAccountToCategoryMappingList()
        {
            var list = new List<AccountToCategoryMatrixViewModel>();
            var mapList = _accountToCategoryRepository.Get().ToList();
            foreach (var map in mapList)
            {
                var model = new AccountToCategoryMatrixViewModel()
                                {
                                    Id = map.Id,
                                    AccountName = map.Account.Name,
                                    AccountCode = map.Account.Code,
                                    AccountFullName = map.Account.GetFullName(),
                                    CategoryName = map.Category.Name,
                                    AccountId = map.Account.Id,
                                    CategoryId = map.Category.Id
                                };
                list.Add(model);
            }
            return list.OrderBy(x=>x.CategoryName).ThenBy(x=>x.AccountCode).ToList();
        }

        public AccountToCategoryAdding GetAccountToCategoryAddingViewModel(int categoryId)
        {
            var accMappedList = _accountToCategoryRepository.Get(x => x.Category.Id == categoryId).Select(x => x.Account).ToList();

            var accList = _accountRepository.Get().ToList().OrderBy(x=>x.Code);

            var filteredList =
                accList.Except(accMappedList)
                    .Select(
                        item =>
                        new AccountViewModel()
                        {
                            FullName = item.Code + " - " + item.Name,
                            Id = item.Id,
                            Code = item.Code
                        })
                    .ToList();
            var model = new AccountToCategoryAdding() { SelectedCategory = categoryId,Accounts = filteredList };
            return model;
        }

        public AccountCategoryDLLViewModel GetAccountCategoryDDL()
        {
            var list = _accountCategoryRepository.Get().OrderBy(x=>x.Name).ToList();
            var model = new AccountCategoryDLLViewModel() { AccountCategories = list};
            return model;
        }

        public void SaveAccountToCategoryMap(AccountToCategoryAdding model)
        {
            var category = _accountCategoryRepository.Find(model.SelectedCategory);
            foreach (var accId in model.SelectedAccounts)
            {
                var account= _accountRepository.Find(Convert.ToInt32(accId));

                var map = new AccountToCategory()
                {
                    Account = account,
                    Category = category,
                    CreatedBy = _curUser,
                    LastModifiedBy = _curUser,
                    DateCreated = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                };
                _accountToCategoryRepository.Add(map);
            }
            _dataContext.SaveChanges();
        }

        public void DeleteOrActivateAccountToCategory(int id, bool toDelete)
        {
            try
            {
                var model = _accountToCategoryRepository.Find(id);
                model.LastModifiedBy = _curUser;
                model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public ReceiptGroupViewModel GetReceiptGroupViewModel(int groupId)
        {
            var group = _groupRepository.Find(groupId);
            var model = new ReceiptGroupViewModel() { Id = group.Id, Name = group.Name, isDeleted = group.IsDeleted, Members = new List<ReceiptGroupMemberViewModel>()};
            model.Members=GetGroupMembers(groupId);
            model.UserList = GetUserViewModelList();
            return model;
        }

        public List<ReceiptGroupMemberViewModel> GetGroupMembers(int groupId)
        {
            var model = new List<ReceiptGroupMemberViewModel>();
            var members = _groupMemberRepository.Get(x => x.Group.Id == groupId).ToList();
            foreach (var item in members)
            {
                var member = new ReceiptGroupMemberViewModel()
                                 {
                                     MemberId = item.Id,
                                     isDeleted = item.IsDeleted,
                                     UserId = item.User.Id,
                                     UserName =
                                         item.User.UserInfo.FirstName + " "
                                         + item.User.UserInfo.LastName
                                 };
                model.Add(member);
            }
            return model;
        }

        public OrderViewModel GetOrderViewForReassignByTempOrder(string tempNumber)
        {
            var model = new OrderViewModel() {EPOView = new SearchEPOResult() {SearchEpoResultItems = new List<SearchEPOResultItem>()} };
            var orderList =
                _orderRepository.Get(
                    x =>
                    x.TempOrderNumber.Substring(0, 6).Contains(tempNumber) && !x.IsDeleted
                    && x.Status.Name == StatusEnum.Pending.ToString()).ToList();
            if (orderList.Count > 0)
            {
                var order = orderList.First();
                //var main= new Main();
                model.OrderId = order.Id;
                model.ApproverList = GetUserViewModelList();
                model.TempOrder = tempNumber;
                model.EPOView.UserRoles = _ad.CurrentUserRoles;
                model.EPOView.SearchEpoResultItems =Main.ConvertToSearchEPOResult(orderList);
                var approver =
                    _routeRepository.Get(x => x.Order.Id == order.Id && !x.IsDeleted)
                        .OrderBy(x => x.Number)
                        .Select(x => x.Approver)
                        .FirstOrDefault();
                if (approver != null)
                {
                    model.SelectedApprover = approver.User.Id;
                    model.ApproverId = approver.Id;
                }
                return model;
            }
            return new OrderViewModel(){ApproverList = new List<UserViewModel>()};
        }

        public void ReassignOrder(OrderViewModel model)
        {
            var approver = _approverRepository.Find(model.ApproverId);
            approver.User = _userRepository.Find(model.SelectedApprover);
            approver.LastModifiedBy = _curUser;
            approver.LastModifiedDate = DateTime.Now;
            _dataContext.SaveChanges();
        }

        public void SaveReceiptGroup(ReceiptGroupViewModel model)
        {
            var group = _groupRepository.Find(model.Id);
            if (group != null)
            {
                group.Name = model.Name;
                group.LastModifiedBy = _curUser;
                group.LastModifiedDate = DateTime.Now;
                group.IsDeleted = model.isDeleted;
            }
            else
            {
                group = new Group()
                {
                    CreatedBy = _curUser,
                    LastModifiedBy = _curUser,
                    DateCreated = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    Name = model.Name
                };
                _groupRepository.Add(group);
            }

            if (model.SelectedUsers != null)
            {

                foreach (var item in model.SelectedUsers)
                {
                    var member = new GroupMember()
                                     {
                                         DateCreated = DateTime.Now,
                                         LastModifiedDate = DateTime.Now,
                                         CreatedBy = _curUser,
                                         LastModifiedBy = _curUser,
                                         Group = group,
                                         User = _userRepository.Find(item)
                                     };
                    _groupMemberRepository.Add(member);
                }
            }
            _dataContext.SaveChanges();
        }

        public void SaveUser(UserEditViewModel model)
        {
            var selectedUserGroupId = model.SelectedUserGroup != null ? Convert.ToInt32(model.SelectedUserGroup) : 0;
            var userManager = new UserManager<User>(new UserStore<User>());
            var user = _userRepository.Find(model.Id);
            user.EmployeeId = Convert.ToInt32(model.EmployeeId);
            user.Email = model.Email;
            user.UserInfo.IsDeleted = model.isDeleted;
            user.UserInfo.FirstName = model.FirstName;
            user.UserInfo.LastName = model.LastName;

            if (user.UserDashboardSettings != null)
            {
                user.UserDashboardSettings.DUserGroup = _dUserGroupRepository.Find(selectedUserGroupId);
            }
            else
            {
                user.UserDashboardSettings = new UserDashboardSettings()
                                                 {
                                                     DUserGroup =
                                                         _dUserGroupRepository.Find(selectedUserGroupId),
                                                     MyTiles = new PersistableIntCollection()
                                                 };
            }

            var roleList = _roleRepository.Get().Select(x => new UserRoleViewModel() { Id = x.Id, Name = x.Name }).ToList();
            var rolesToAdd = model.SelectedRoles!=null ? roleList.Where(x => model.SelectedRoles.Any(y => y.Equals(x.Id))).Select(x => x.Id).ToList() : new List<string>();
            var rolesToDelete = user.Roles.Select(x => x.RoleId).Except(rolesToAdd).ToList();
            var newRolesToAdd = rolesToAdd.Except(user.Roles.Select(x => x.RoleId).ToList()).ToList();

            foreach (var role in newRolesToAdd)
            {
                var roleName = _roleRepository.Get(x => x.Id == role).Select(y => y.Name).First();
                user.Roles.Add(new IdentityUserRole {RoleId = role,UserId = user.Id});
                //userManager.AddToRole(user.Id, roleName);
            }
            foreach (var role in rolesToDelete)
            {
                var roleName = _roleRepository.Get(x => x.Id == role).Select(y => y.Name).First();
                userManager.RemoveFromRoles(user.Id, roleName);
            }
            _dataContext.SaveChanges();
        }

        public List<OrderItemKitViewModel> GetItemKitList()
        {
            var list = _orderItemKitRepository.Get()
                .Select(x=>new OrderItemKitViewModel() {Id = x.Id,CostCentre = x.CostCentre.Code+" - "+x.CostCentre.Name, Description = x.Description,Name = x.Part, CostCentreId = x.CostCentre.Id,Account = x.Account.Code+" - "+x.Account.Name, AccountId = x.Account.Id,Price = x.Price})
                .ToList();
            return list;
        }

        public void DeleteOrActivateItemKit(int id, bool toDelete = true)
        {
            try
            {
                var model = _orderItemKitRepository.Find(id);
                model.LastModifiedBy = _curUser;
                model.LastModifiedDate = DateTime.Now;
                model.IsDeleted = toDelete;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public OrderItemKitCRUDViewModel GetOrderItemKitCRUD(int id)
        {
            var model = _orderItemKitRepository.Find(id);
            if (model != null)
            {
                var itemKit = new OrderItemKitCRUDViewModel()
                                  {
                                      Id = model.Id,
                                      Name = model.Part,
                                      CostCentre =
                                          model.CostCentre.Code + " - "
                                          + model.CostCentre.Name,
                                      Account =
                                          model.Account.Code + " - " + model.Account.Name,
                                      Description = model.Description,
                                      AccountId = model.Account.Id,
                                      Accounts = GetAccountViewModelList(),
                                      CostCentres = GetCostCentresVM(),
                                      Price = model.Price,
                                      CostCentreId = model.CostCentre.Id
                                  };
                return itemKit;
            }
            return  new OrderItemKitCRUDViewModel()
            {
                Accounts = GetAccountViewModelList(),
                CostCentres = GetCostCentresVM()
            };
        }

        public OrderItemKitViewModel GetOrderItemKit(int id)
        {
            var model = _orderItemKitRepository.Find(id);
            if (model != null)
            {
                var itemKit = new OrderItemKitViewModel()
                {
                    Id = model.Id,
                    Name = model.Part,
                    Description = model.Description,
                    Price = model.Price,
                };
                return itemKit;
            }
            return null;
        }

        public void SaveOrderItemKit(OrderItemKitCRUDViewModel model)
        {
            var itemKit = _orderItemKitRepository.Find(model.Id);
            if (itemKit != null)
            {
                itemKit.Account = _accountRepository.Find(model.SelectedAccount);
                itemKit.CostCentre = _costCentreRepository.Find(model.SelectedCostCentre);
                itemKit.Description = model.Description;
                itemKit.Part = model.Name;
                itemKit.Price = model.Price;
                itemKit.LastModifiedBy = _curUser;
                itemKit.LastModifiedDate = DateTime.Now;
            }
            else
            {
                itemKit = new OrderItemKit
                              {
                                  Account = _accountRepository.Find(model.SelectedAccount),
                                  CostCentre = _costCentreRepository.Find(model.SelectedCostCentre),
                                  Description = model.Description,
                                  Part = model.Name,
                                  Price = model.Price,
                                  LastModifiedBy = _curUser,
                                  LastModifiedDate = DateTime.Now,
                                  CreatedBy = _curUser,
                                  DateCreated = DateTime.Now
                              };
                _orderItemKitRepository.Add(itemKit);
            }
            _dataContext.SaveChanges();
        }

        public UserOrderSettingsViewModel GetUserOrderSettingsViewModel(string id)
        {
            var user = _userRepository.Get(x => x.Id == id).Include(x => x.UserOrderSettings).Include(x=>x.UserInfo).FirstOrDefault();
            if (user != null)
            {
                var model = new UserOrderSettingsViewModel()
                                {
                                    Id = id,
                                    FullName =
                                        user.UserInfo.FirstName + " "
                                        + user.UserInfo.LastName,
                                    AutoApproveItemKit =user.UserOrderSettings?.AutoApproveItemKit ?? false,
                                    OrderSettingsId = user.UserOrderSettings?.Id ?? 0
                                };
                return model;
            }
            return new UserOrderSettingsViewModel();
        }

        public void SaveUserOrderSettings(UserOrderSettingsViewModel model)
        {
            var userSettings = _userOrderSettingsRepository.Find(model.OrderSettingsId);
            if (userSettings != null)
            {
                userSettings.AutoApproveItemKit = model.AutoApproveItemKit;
                userSettings.LastModifiedBy = _curUser;
                userSettings.LastModifiedDate = DateTime.Now;
                userSettings.IsDeleted = false;
            }
            else
            {
                userSettings = new UserOrderSettings()
                                   {
                                       AutoApproveItemKit = model.AutoApproveItemKit,
                                       CreatedBy = _curUser,
                                       LastModifiedBy = _curUser,
                                       DateCreated = DateTime.Now,
                                       LastModifiedDate = DateTime.Now,
                                   };
                var user = _userRepository.Find(model.Id);
                user.UserOrderSettings = userSettings;
            }
            _dataContext.SaveChanges();
        }

        public OrderViewModel GetOrderViewForChangeAuthor(int orderNumber)
        {
            var orderList = _orderRepository.Get(x => x.OrderNumber == orderNumber).ToList();//Using list because ConvertToSearchEPOResult accepting List off orders
            if (orderList.Count > 0)
            {
                var order = orderList.First();
                var model = new OrderViewModel()
                {
                    OrderNumber = orderNumber,
                    OrderId = order.Id,
                    EPOView =new SearchEPOResult(){SearchEpoResultItems =new List<SearchEPOResultItem>()},
                    AuthorList =  new List<UserViewModel>()
                };
                model.EPOView.UserRoles = _ad.CurrentUserRoles;
                model.EPOView.SearchEpoResultItems = Main.ConvertToSearchEPOResult(orderList);
                model.AuthorList = GetUserViewModelList();

                return model;
            }
            return new OrderViewModel() { StatusList = new List<Status>() };
        }

        public void ChangeOrderAuthor(OrderViewModel model)
        {
            var order = _orderRepository.Find(model.OrderId);
            if (order != null)
            {
                if(!string.IsNullOrEmpty(model.SelectedAuthor))
                {
                    var isAuthorChanged = order.Author?.Id != model.SelectedAuthor;
                    if (isAuthorChanged) GenerateOrderLog(order, OrderLogSubject.Manual);
                    order.Author = _userRepository.Find(model.SelectedAuthor);
                    _orderRepository.Update(order);
                    _dataContext.SaveChanges();
                }
            }
        }
    }
}