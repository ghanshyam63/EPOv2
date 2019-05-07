namespace EPOv2.Business
{
    using System;

    using DomainModel.Entities;

    using EPOv2.ViewModels;

    public partial class Main
    {
       

        public void SaveOrderItemLog(int itemId)
        {
            var unChangedModel = this._orderItemRepository.Find(itemId);
            SaveOrderItemLog(unChangedModel);
        }

        public void SaveOrderItemLog(OrderItemViewModel model)
        {
            var orderItem = this.CreateOrderItemFromViewModel(model);
            SaveOrderItemLog(orderItem);
        }

        public void SaveOrderItemLog(OrderItem unChangedModel)
        {
            var log = new OrderItemLog
            {
                LatestOrderItem = unChangedModel,
                Account = unChangedModel.Account,
                Capex_Id = unChangedModel.Capex_Id,
                Description = unChangedModel.Description,
                DueDate = unChangedModel.DueDate,
                IsGSTInclusive = unChangedModel.IsGSTInclusive,
                IsTaxable = unChangedModel.IsTaxable,   
                IsGSTFree=unChangedModel.IsGSTFree,
                ItemDateCreated = unChangedModel.DateCreated,
                Currency = unChangedModel.Currency,
                CurrencyRate = unChangedModel.CurrencyRate,
                ItemCreatedBy = unChangedModel.CreatedBy,
                ItemLastModifiedBy = unChangedModel.LastModifiedBy,
                ItemLastModifiedDate = unChangedModel.LastModifiedDate,
                LineNumber = unChangedModel.LineNumber,
                Qty = unChangedModel.Qty,
                RevisionQty = unChangedModel.RevisionQty,
                Status = unChangedModel.Status,
                SubAccount = unChangedModel.SubAccount,
                Total = unChangedModel.Total,
                TotalExTax = unChangedModel.TotalExTax,
                TotalTax = unChangedModel.TotalTax,
                UnitPrice = unChangedModel.UnitPrice,
                CreatedBy = this._curUser,
                DateCreated = DateTime.Now,
                LastModifiedBy = this._curUser,
                LastModifiedDate = DateTime.Now
            };
            this._orderItemLogRepository.Add(log);
            this.db.SaveChanges();
        }

        public void LogError(string msg, Exception exception)
        {
           _logger.Error(exception, msg);
        }
    }
}