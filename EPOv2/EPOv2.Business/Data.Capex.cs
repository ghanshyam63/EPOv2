namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;
    using EPOv2.ViewModels.Interfaces;

    public partial class Data
    {
        public CapexCRUDViewModel GetCapexCRUDViewModel(int selectedItem, CapexAction action )
        {
            var capex = this._capexRepository.FirstOrDefault(x=>!x.IsDeleted && x.Id==selectedItem);
            var model = new CapexCRUDViewModel();
            if (capex != null)
            {
                model.Id = capex.Id;
                model.CapexCompanyBox = this.GetCapexCompanyBox(capex);
                model.Author = capex.Author.GetFullName();
                model.DateCreated = capex.DateCreated.ToShortDateString();
                model.Status = capex.Status.Name;
                model.RevisionQty = capex.RevisionQty;
                model.CapexNumber = capex.CapexNumber;
                model.Reference = capex.Reference;
                model.Title = capex.Title;
                model.Description = capex.Description;
                model.TotalExGST = capex.TotalExGST;

                switch (action)
                {
                    case CapexAction.View:
                        model.IsLocked = true;
                        model.CapexCompanyBox.IsLocked = true;
                        break;
                    case CapexAction.New:
                        break;
                    case CapexAction.Edit:
                        model.IsEdit = true;
                        model.CapexCompanyBox.IsEdit = true;
                        if (capex.Status.Name == StatusEnum.Pending.ToString())
                        {
                            model.RevisionQty++;
                        }
                        break;
                    case CapexAction.Delete:
                        break;
                    case CapexAction.Authorise:
                        model.IsLocked = true;
                        model.CapexCompanyBox.IsLocked = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(action), action, null);
                }
            }
            else
            {
                model.IsDeleted = true;
            }
            return model;
        }

        private CapexCompanyBox GetCapexCompanyBox(Capex capex)
        {
            var model = new CapexCompanyBox() { Entities = this.GetEntityViewModelList(), SelectedEntity = capex.Entity.Id, CostCentres = this.GetCostCentresVM(), SelectedCostCentre = capex.CostCentre.Id, Users = this.GetUserViewModelDdList(), SelectedOwner = capex.Owner.Id, SelectedCapexType = capex.CapexType };
            return model;
        }

        public CapexCRUDViewModel GetCapexCRUDViewModel()
        {
            var model = new CapexCRUDViewModel();

            //model.CapexDetailBox = GetCapexDetailBox();
            model.CapexCompanyBox = this.GetCapexCompanyBox();
            model.Author = this._ad.GetADNamebyLogin(this._curUser);
            model.DateCreated = DateTime.Now.ToShortDateString();
            model.Status = StatusEnum.Pending.ToString();
            model.RevisionQty = 0;
            model.CapexNumber = "-";

            return model;
        }

        private CapexCompanyBox GetCapexCompanyBox()
        {
            var model = new CapexCompanyBox() { Entities = this.GetEntityViewModelList(), CostCentres = new List<CostCentreViewModel>(), //this.GetCostCentresVM(),
                                                Users = new List<IUserViewModel>() //this.GetUserViewModelList(),
                                              };
            return model;
        }

        public CapexCompanyBox GetCapexCompanyBox(int entityId)
        {
            var model = new CapexCompanyBox() { Entities = this.GetEntityViewModelList(), SelectedEntity = entityId, CostCentres = this.GetCostCentresVM(entityId), Users = this.GetUserViewModelDdList(), };
            var cc90 = model.CostCentres.FirstOrDefault(x => x.Code == 90);
            model.CostCentres.Remove(cc90);
            return model;
        }

        public string GetCapexReference(string capexId)
        {
            var capexReference = this._capexRepository.Find(Convert.ToInt32(capexId)).Reference;
            return Path.Combine(capexFilesPath, capexReference);
        }

        public void SaveCapexApprover(CapexApproverCRUDViewModel model)
        {
            var approver = this._capexApproverRepository.Find(model.Id);
            if (approver != null)
            {
                approver.Division = this._divisionRepository.Find(model.SelectedDivision);
                approver.User = this._userRepository.Find(model.SelectedUser);
                approver.Level = model.Level;
                approver.Limit = model.Limit;
                approver.Role = model.Role;
                approver.LastModifiedBy = this._curUser;
                approver.LastModifiedDate = DateTime.Now;
            }
            else
            {
                approver = new CapexApprover()
                               {
                                   CreatedBy = this._curUser,
                                   LastModifiedBy = this._curUser,
                                   LastModifiedDate = DateTime.Now,
                                   DateCreated = DateTime.Now,
                                   Level = model.Level,
                                   Limit = model.Limit,
                                   Role = model.Role,
                                   Division = this._divisionRepository.Find(model.SelectedDivision),
                                   User = this._userRepository.Find(model.SelectedUser)
                               };
                this._capexApproverRepository.Add(approver);
            }
            this._dataContext.SaveChanges();
        }

        public CapexApproverCRUDViewModel GetCapexApproverCRUDViewModel()
        {
            var model = new CapexApproverCRUDViewModel()
                            {
                                Divisions = this.GetDivisionViewModelList(),
                                Users = this.GetUserViewModelDdList()
                            };
            return model;
        }

        public List<DivsionViewModel> GetDivisionViewModelList()
        {
            var list = new List<DivsionViewModel>();
            foreach (var division in this.GetDivisions())
            {
                var item = new DivsionViewModel() { Id = division.Id, FullName = division.Name };
                list.Add(item);
            }
            return list;
        }

        public CapexApproverCRUDViewModel GetCapexApproverCRUDViewModel(int id)
        {
            var approver = this._capexApproverRepository.Find(id);
            var model = new CapexApproverCRUDViewModel()
                            {
                                Id = approver.Id,
                                Level = approver.Level,
                                Limit = approver.Limit,
                                Role = approver.Role,
                                SelectedDivision = approver.Division?.Id ?? 0,
                                SelectedUser = approver.User!=null ? approver.User.Id: string.Empty,
                                Divisions = this.GetDivisionViewModelList(),
                                Users = this.GetUserViewModelDdList()
                            };
            return model;
        }

        public List<CapexApprover> GetCapexApproversList()
        {
            return this._capexApproverRepository.Get(x=>!x.IsDeleted).OrderBy(x=>x.Level).ToList();
        }

        public List<Division> GetDivisions()
        {
            return this._divisionRepository.Get().OrderBy(x=>x.Name).ToList();
        }

        public DivisionCRUDViewModel GetDivisionCRUDViewModel()
        {
            var model = new DivisionCRUDViewModel() { Users = this.GetUserViewModelDdList() };
            return model;
        }

        public void SaveDivision(DivisionCRUDViewModel model)
        {
            var division = this._divisionRepository.Find(model.Id);
            if (division != null)
            {
                //update
                division.Owner = _userRepository.Find(model.SelectedUser);
                division.CostCentreRangeFrom = model.CostCentreCodeRangeFrom;
                division.CostCentreRangeTo = model.CostCentreCodeRangeTo;
                division.Name = model.Name;
                division.LastModifiedDate = DateTime.Now;
                division.LastModifiedBy = this._curUser;
            }
            else
            {
                division = new Division()
                               {
                                   Owner = this._userRepository.Find(model.SelectedUser),
                                   CostCentreRangeFrom = model.CostCentreCodeRangeFrom,
                                   CostCentreRangeTo = model.CostCentreCodeRangeTo,
                                   Name = model.Name,
                                   CreatedBy = this._curUser,
                                   DateCreated = DateTime.Now,
                                   LastModifiedDate = DateTime.Now,
                                   LastModifiedBy = this._curUser
                               };
                this._divisionRepository.Add(division);
            }
            this._dataContext.SaveChanges();
        }

        public DivisionCRUDViewModel GetDivisionCRUDViewModel(int id)
        {
            var division = _divisionRepository.Find(id);
            var model = new DivisionCRUDViewModel()
                            {
                                Users = this.GetUserViewModelDdList(),
                                Name = division.Name,
                                CostCentreCodeRangeFrom = division.CostCentreRangeFrom,
                                Id = division.Id,
                                CostCentreCodeRangeTo = division.CostCentreRangeTo,
                                SelectedUser = division.Owner?.Id ?? string.Empty
                            };
            return model;
        }

        public CapexViewModel GetCapexViewModelForStatusChange(string capexNumber)
        {
            var capexList = this._capexRepository.Get(x => x.CapexNumber == capexNumber).ToList();
            if (capexList.Count > 0)
            {
                var capex = capexList.First();
                var model = new CapexViewModel()
                {
                    Number = capexNumber,
                    Id = capex.Id,
                    CapexView = new List<SearchCapexResult>(),
                    StatusList = new List<Status>(),
                };
                model.CapexView = Main.ConvertToSearchCapexResult(capexList);
                model.StatusList.AddRange(_statusRepository.Get(x => x.Name == StatusEnum.Receipt_Partial.ToString().Replace("_", " ") || x.Name == StatusEnum.Receipt_in_Full.ToString().Replace("_", " ") || x.Name == StatusEnum.Closed.ToString()).ToList());

                return model;
            }
            return new CapexViewModel() { StatusList = new List<Status>() };
        }

        public void ChangeCapexStatus(CapexViewModel model)
        {
            var capex = this._capexRepository.Find(model.Id);
            if (capex != null)
            {
                capex.Status = this._statusRepository.Find(model.SelectedStatus);
                this._dataContext.SaveChanges();
            }
        }

        public void ApplyCapexSubstitution(SubstituteApprover substitute)
        {
            var approverList =
                _capexApproverRepository.Get(x => !x.IsDeleted && x.User.Id == substitute.ApproverUser.Id).ToList();
            foreach (var capexApprover in approverList)
            {
                capexApprover.oldapprover = capexApprover.User.Id;
                capexApprover.User = substitute.SubstitutionUser;
                _capexApproverRepository.Update(capexApprover);
            }
            _dataContext.SaveChanges();
        }

        public void CancelSubstitutionForCapex(SubstituteApprover substitute)
        {
            try
            {
                var approverList =
                    _capexApproverRepository.Get(x => !x.IsDeleted && x.oldapprover == substitute.ApproverUser.Id).ToList();
                foreach (var capexApprover in approverList)
                {
                    var temp = capexApprover.User.Id;
                    var olduserapprover = _userRepository.Get(x => x.Id == capexApprover.oldapprover).FirstOrDefault();
                    capexApprover.User = olduserapprover;
                    capexApprover.oldapprover = olduserapprover.Id;
                }
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error(e,"CancelSubstitutionForCapex(subsId:{substituteId}, approver:{substitute.ApproverUser}",substitute.Id,substitute.ApproverUser.GetFullName());
            }
        }
    }
}
