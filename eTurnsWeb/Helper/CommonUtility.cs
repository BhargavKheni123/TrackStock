using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace eTurnsWeb.Helper
{

    public static class CommonUtility
    {
        //public static bool SendMail(string FromEmailID, string ToEmailID, string CCEmailID, string BCCEmailID, string NotificationMailID, string Subject, string Body, Boolean IsBodyHtml, ArrayList _Attachments = null)
        //{
        //    BCCEmailID = "niraj_dave@semaphore-software.com";
        //    String Host = ConfigurationManager.AppSettings["SMTPHost"].ToString();
        //    String Port = ConfigurationManager.AppSettings["SMTPPort"].ToString();
        //    String UserName = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
        //    String Password = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

        //    MailMessage mm = null;
        //    SmtpClient smtp = null;
        //    Attachment attach = null;

        //    try
        //    {
        //        Body = Body.Replace("@@ETURNSLOGO@@", CommonUtility.GeteTurnsImage("http://" + System.Web.HttpContext.Current.Request.Url.Authority, "/Content/images/logo.jpg"));
        //        Body = Body.ToString().Replace("@@Year@@", DateTime.Now.Year.ToString());
        //        mm = new MailMessage(FromEmailID, ToEmailID, Subject, Body);
        //        mm.IsBodyHtml = IsBodyHtml;
        //        if (!string.IsNullOrEmpty(CCEmailID)) mm.CC.Add(CCEmailID);
        //        if (!string.IsNullOrEmpty(BCCEmailID)) mm.Bcc.Add(BCCEmailID);

        //        if (_Attachments != null && _Attachments.Count > 0)
        //        {
        //            for (int i = 0; i < _Attachments.Count; i++)
        //            {
        //                mm.Attachments.Add((Attachment)_Attachments[i]);
        //            }
        //        }
        //        if (NotificationMailID != "")
        //            mm.Headers.Add("Disposition-Notification-To", NotificationMailID);//ConfigurationManager.AppSettings["NotificationEmail"].ToString());

        //        smtp = new SmtpClient(Host, Convert.ToInt32(Port));
        //        smtp.EnableSsl = true;
        //        smtp.UseDefaultCredentials = false;
        //        System.Net.NetworkCredential cr = new NetworkCredential(UserName, Password);
        //        smtp.Credentials = cr;
        //        //smtp.ServicePoint.MaxIdleTime = 1;
        //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

        //        //smtp.Timeout = 3000;
        //        smtp.Send(mm);

        //        return true;

        //    }
        //    //catch (Exception ex)
        //    //// catch
        //    //{
        //    //    //HttpContext.Current.Response.Write("Error StackTrace:" + ex.StackTrace.ToString() + "<br/><br/><br/>");
        //    //    //HttpContext.Current.Response.Write("Error :" + ex.InnerException.ToString() + "<br/>");
        //    //    if (attach != null)
        //    //        attach.Dispose();
        //    //    return false;
        //    //}
        //    finally
        //    {
        //        if (attach != null)
        //            attach.Dispose();

        //        if (mm != null)
        //            mm.Dispose();

        //        if (smtp != null)
        //            smtp.Dispose();

        //        mm = null;
        //        smtp = null;
        //        attach = null;

        //    }
        //}

        #region Import Related Enum
        public enum ImportBinColumn
        {
            ID,
            BinNumber,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
        }
        public enum ImportCostUOMColumn
        {
            ID,
            CostUOM,
            CostUOMValue,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            //CategoryColor
        }

        public enum ImportInventoryClassificationColumn
        {
            ID,
            InventoryClassification,
            BaseOfInventory,
            RangeStart,
            RangeEnd,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            //CategoryColor
        }

        public enum ImportCategoryColumn
        {
            ID,
            Category,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            //CategoryColor
        }

        public enum ImportCustomerColumn
        {
            ID,
            Customer,
            Account,
            Contact,
            Address,
            City,
            State,
            ZipCode,
            Country,
            Phone,
            Email,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            Remarks
            //,UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10
        }
        public enum ImportFreightTypeColumn
        {
            ID,
            FreightType,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
            //,UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10
        }
        public enum ImportGLAccountColumn
        {
            ID,
            GLAccount,
            Description,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
            //,UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10

        }
        public enum ImportGXPRConsignedColumn
        {
            ID,
            GXPRConsigmentJob,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
            //UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10

        }
        public enum ImportJobTypeColumn
        {
            ID,
            JobType,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
            //UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10

        }
        public enum ImportShipViaColumn
        {
            ID,
            ShipVia,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
            //UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10

        }
        public enum ImportTechnicianColumn
        {
            ID,
            Technician,
            TechnicianCode,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
            //UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10

        }
        public enum ImportManufacturerColumn
        {
            ID,
            Manufacturer,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
            //UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10

        }
        public enum ImportMeasurementTermColumn
        {
            ID,
            MeasurementTerm,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
            //UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10

        }
        public enum ImportUnitsColumn
        {
            ID,
            Unit,
            Description,
            Odometer,
            OpHours,
            SerialNo,
            Year,
            Make,
            Model,
            Plate,
            EngineModel,
            EngineSerialNo,
            MarkupParts,
            MarkupLabour,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
            //UDF6,
            //UDF7,
            //UDF8,
            //UDF9,
            //UDF10

        }
        public enum ImportSupplierColumn
        {
            ID,
            SupplierName,
            SupplierColor,
            Description,
            BranchNumber,
            MaximumOrderSize,
            //ReceiverID,
            Address,
            City,
            State,
            ZipCode,
            Country,
            Contact,
            Phone,
            Fax,
            IsSendtoVendor,
            IsVendorReturnAsn,
            IsSupplierReceivesKitComponents,
            Email,
            //IsEmailPOInBody,
            //IsEmailPOInPDF,
            //IsEmailPOInCSV,
            //IsEmailPOInX12,
            OrderNumberTypeBlank,
            OrderNumberTypeFixed,
            OrderNumberTypeBlanketOrderNumber,
            OrderNumberTypeIncrementingOrderNumber,
            OrderNumberTypeIncrementingbyDay,
            OrderNumberTypeDateIncrementing,
            OrderNumberTypeDate,

            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            UDF6,
            UDF7,
            UDF8,
            UDF9,
            UDF10,
            AccountNumber,
            AccountName,
            AccountAddress,
            AccountCity,
            AccountState,
            AccountZip,
            AccountIsDefault,
            BlanketPONumber,
            StartDate,
            EndDate,
            MaxLimit,
            DoNotExceed,
            PullPurchaseNumberFixed,
            PullPurchaseNumberBlanketOrder,
            PullPurchaseNumberDateIncrementing,
            PullPurchaseNumberDate,
            LastPullPurchaseNumberUsed,
            IsBlanketDeleted,

            SupplierImage,
            ImageExternalURL,
            MaxLimitQty,
            DoNotExceedQty,
            AccountCountry,
            AccountShipToID,
            POAutoReleaseNumber
        }
        public enum ImportItemColumn
        {
            ID,
            ItemNumber,
            Manufacturer,
            ManufacturerNumber,
            SupplierName,
            SupplierPartNo,
            BlanketOrderNumber,
            UPC,
            UNSPSC,
            Description,
            LongDescription,
            CategoryName,
            GLAccount,
            UOM,
            Unit,
            DefaultReorderQuantity,
            DefaultPullQuantity,
            Cost,
            Markup,
            SellPrice,
            ExtendedCost,
            LeadTimeInDays,
            Link1,
            Link2,
            Trend,
            Taxable,
            Consignment,
            StagedQuantity,
            InTransitquantity,
            OnOrderQuantity,
            OnTransferQuantity,
            SuggestedOrderQuantity,
            RequisitionedQuantity,
            AverageUsage,
            Turns,
            OnHandQuantity,
            CriticalQuantity,
            MinimumQuantity,
            MaximumQuantity,
            WeightPerPiece,
            ItemUniqueNumber,
            IsTransfer,
            IsPurchase,
            InventryLocation,
            InventoryClassification,
            SerialNumberTracking,
            LotNumberTracking,
            DateCodeTracking,
            ItemType,
            ImagePath,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            IsLotSerialExpiryCost,
            IsItemLevelMinMaxQtyRequired,
            CostUOM,
            TrendingSetting,
            PullQtyScanOverride,
            EnforceDefaultPullQuantity,
            EnforceDefaultReorderQuantity,
            IsAutoInventoryClassification,
            IsBuildBreak,
            IsPackslipMandatoryAtReceive,
            ItemImageExternalURL,
            ItemDocExternalURL,
            IsDeleted,
            ItemLink2ExternalURL,
            ItemLink2ImageType,
            IsActive,
            UDF6,
            UDF7,
            UDF8,
            UDF9,
            UDF10,
            PerItemCost,
            OutTransferQuantity,
            IsAllowOrderCostuom,
            eLabelKey,
            EnrichedProductData,
            EnhancedDescription,
            POItemLineNumber
        }
        public enum ImportLocationColumn
        {
            ID,
            Location,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
        }
        public enum ImportToolCategoryColumn
        {
            ID,
            ToolCategory,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5
        }
        public enum ImportToolMasterColumn
        {
            ID,
            GUID,
            ToolName,
            Serial,
            Description,
            ToolCategory,
            ToolCategoryID,
            Cost,
            IscheckedOut,
            IsGroupOfItems,
            Quantity,
            CheckedOutQTY,
            CheckedOutMQTY,
            Location,
            LocationID,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            Technician,
            CheckOutQuantity,
            CheckInQuantity,
            CheckOutUDF1,
            CheckOutUDF2,
            CheckOutUDF3,
            CheckOutUDF4,
            CheckOutUDF5,
            ImagePath,
            ToolImageExternalURL,
            SerialNumberTracking,
            NoOfPastMntsToConsider,
            MaintenanceDueNoticeDays
        }

        public enum ImportToolCheckInCheckOutColumn
        {
            ToolName,
            Serial,
            Location,
            TechnicianCode,
            Quantity,
            Operation,
            CheckOutUDF1,
            CheckOutUDF2,
            CheckOutUDF3,
            CheckOutUDF4,
            CheckOutUDF5
        }

        public enum ImportToolCertificationImagesColumn
        {
            ToolName,
            Serial,
            ImageName
        }

        public enum AssetToolSchedulerMappingColumn
        {

            ScheduleFor,
            SchedulerName,
            ToolName,
            Serial,
            AssetName,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5

        }

        public enum ImportAssetMasterColumn
        {
            ID,
            AssetName,
            Description,
            Make,
            Model,
            Serial,
            ToolCategory,
            ToolCategoryID,
            PurchaseDate,
            PurchasePrice,
            DepreciatedValue,
            SuggestedMaintenanceDate,
            Created,
            CreatedBy,
            Updated,
            LastUpdatedBy,
            Room,
            IsArchived,
            IsDeleted,
            GUID,
            CompanyID,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            UDF6,
            UDF7,
            UDF8,
            UDF9,
            UDF10,
            AssetCategory,
            ImagePath,
            AssetImageExternalURL
        }
        public enum ImportQuickListItemsColumn
        {
            ID,
            QuickListGUID,
            ItemGUID,
            Quantity,
            ConsignedQuantity,
            Created,
            LastUpdated,
            CreatedBy,
            LastUpdatedBy,
            Room,
            IsDeleted,
            IsArchived,
            CompanyID,
            GUID,
            Comment,
            ItemNumber,
            Name,
            Type,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            BinNumber,
            BinID
        }
        public enum ImportOrderItemsColumn
        {
            ID,
            Supplier,
            OrderNumber,
            RequiredDate,
            OrderStatus,
            StagingName,
            OrderComment,
            CustomerName,
            PackSlipNumber,
            ShippingTrackNumber,
            OrderUDF1,
            OrderUDF2,
            OrderUDF3,
            OrderUDF4,
            OrderUDF5,
            ShipVia,
            OrderType,
            ShippingVendor,
            AccountNumber,
            SupplierAccount,
            ItemNumber,
            Bin,
            RequestedQty,
            ReceivedQty,
            ASNNumber,
            ApprovedQty,
            InTransitQty,
            IsCloseItem,
            LineNumber,
            ControlNumber,
            ItemComment,
            LineItemUDF1,
            LineItemUDF2,
            LineItemUDF3,
            LineItemUDF4,
            LineItemUDF5,
            OrderCost,
            SalesOrder
        }
        public enum ImportQuoteItemsColumn
        {
            ID,
            SupplierName,
            QuoteNumber,
            RequiredDate,
            QuoteStatus,
            StagingName,
            QuoteComment,
            CustomerName,
            PackSlipNumber,
            ShippingTrackNumber,
            QuoteUDF1,
            QuoteUDF2,
            QuoteUDF3,
            QuoteUDF4,
            QuoteUDF5,
            ShipVia,
            ShippingVendor,
            AccountNumber,
            SupplierAccount,
            ItemNumber,
            Bin,
            RequestedQty,
            ASNNumber,
            ApprovedQty,
            InTransitQty,
            IsCloseItem,
            LineNumber,
            ControlNumber,
            ItemComment,
            LineItemUDF1,
            LineItemUDF2,
            LineItemUDF3,
            LineItemUDF4,
            LineItemUDF5,
            QuoteCost
        }
        public enum ImportKitsItemsColumn
        {
            ID,
            KitGUID,
            ItemGUID,
            QuantityPerKit,
            QuantityReadyForAssembly,
            AvailableItemsInWIP,
            Created,
            LastUpdated,
            CreatedBy,
            LastUpdatedBy,
            Room,
            IsDeleted,
            IsArchived,
            CompanyID,
            GUID,
            KitPartNumber,
            ItemNumber,
            IsBuildBreak,
            OnHandQuantity,

            KitDemand,
            AvailableKitQuantity,
            Description,
            CriticalQuantity,
            MinimumQuantity,
            //MaximumQuantiy,
            MaximumQuantity,
            ReOrderType,
            KitCategory,
            SupplierName,
            SupplierPartNo,
            DefaultLocation,
            CostUOMName,
            UOM,
            DefaultReorderQuantity,
            DefaultPullQuantity,
            ItemTypeName,
            IsItemLevelMinMaxQtyRequired,
            SerialNumberTracking,
            LotNumberTracking,
            DateCodeTracking,
            IsActive
        }
        public enum ImportInventoryLocationColumn
        {
            ID,
            BinID,
            CustomerOwnedQuantity,
            ConsignedQuantity,
            MeasurementID,
            LotNumber,
            SerialNumber,
            Expiration,
            Received,
            ExpirationDate,
            ReceivedDate,
            Cost,
            eVMISensorPort,
            eVMISensorID,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            GUID,
            ItemGUID,
            Created,
            Updated,
            CreatedBy,
            LastUpdatedBy,
            IsDeleted,
            IsArchived,
            CompanyID,
            Room,
            PULLGUID,
            KitDetailGUID,
            TransferDetailGUID,
            OrderDetailGUID,
            ItemNumber,
            locationname,
            Quantity,
            SupplierPartNumber,
            SensorId,
            SensorPort,
            CriticalQuantity,
            MinimumQuantity,
            MaximumQuantity,
            IsDefault,
            ReceiveDate,
            ProjectSpend,
            IsStagingLocation,
            IsEnforceDefaultPullQuantity,
            DefaultPullQuantity,
            IsEnforceDefaultReorderQuantity,
            DefaultReorderQuantity,
            ItemDescription,
            BinUDF1,
            BinUDF2,
            BinUDF3,
            BinUDF4,
            BinUDF5
        }
        public enum WorkOrderColumn
        {
            ID,
            GUID,
            WOName,
            Created,
            Updated,
            CreatedBy,
            LastUpdatedBy,
            Room,
            IsDeleted,
            CompanyID,
            IsArchived,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            CustomerGUID,
            CustomerID,
            Technician,
            TechnicianId,
            WOStatus,
            WOType,
            WhatWhereAction,
            Description,
            Customer,
            ReceivedOn,
            ReceivedOnWeb,
            AddedFrom,
            EditedFrom,
            ReleaseNumber,
            SupplierName,
            Asset,
            Odometer,
            SupplierAccount
        }
        public enum ImportItemManufacturer
        {
            ID,
            ItemNumber,
            ManufacturerName,
            ManufacturerNumber,
            IsDefault,

        }
        public enum ImportItemSupplier
        {
            ID,
            ItemNumber,
            SupplierName,
            SupplierNumber,
            IsDefault,
            BlanketPOID,
            BlanketPOName,

        }
        public enum ImportBarcode
        {
            ID,
            ItemNumber,
            BinNumber,
            ModuleName,
            BarcodeString,


        }

        public enum ImportUDF
        {
            ModuleName,
            UDFColumnName,
            UDFName,
            ControlType,
            DefaultValue,
            OptionName,
            IsRequired,
            IsDeleted,
            IncludeInNarrowSearch
        }

        public enum POAutoSequence
        {
            OrderNumberTypeBlank = 0,
            OrderNumberTypeFixed = 1,
            OrderNumberTypeBlanketOrderNumber = 2,
            OrderNumberTypeIncrementingOrderNumber = 3,
            OrderNumberTypeIncrementingbyDay = 4,
            OrderNumberTypeDateIncrementing = 5,
            OrderNumberTypeDate = 6,
        }

        public enum PullPurchaseNumberType
        {
            PullPurchaseNumberFixed = 1,
            PullPurchaseNumberBlanketOrder = 2,
            PullPurchaseNumberDateIncrementing = 5,
            PullPurchaseNumberDate = 6,
        }
        public enum ImportProjectMaster
        {
            ProjectSpendName,
            Description,
            DollarLimitAmount,
            TrackAllUsageAgainstThis,
            IsClosed,
            IsDeleted,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            ItemNumber,
            ItemDollarLimitAmount,
            ItemQuantityLimitAmount,
            IsLineItemDeleted
        }

        public enum PullImportColumn
        {
            ItemNumber,
            PullQuantity,
            Location,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            ProjectSpendName,
            PullOrderNumber,
            WorkOrder,
            Asset,
            ActionType,
            ItemSellPrice
        }

        public enum PullImportWithLotSerialColumn
        {
            ItemNumber,
            PullQuantity,
            Location,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            ProjectSpendName,
            PullOrderNumber,
            WorkOrder,
            Asset,
            LotNumber,
            SerialNumber,
            ExpirationDate,
            ActionType,
            ItemSellPrice
        }

        public enum PullImportWithSameQtyColumn
        {
            ItemNumber,
            PullQuantity,
            BinNumber,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            ProjectSpendName,
            PullOrderNumber,
            WorkOrder,
            Asset,
            ActionType,
            Created,
            ItemCost,
            CostUOMValue
        }
        public enum PastMaintenanceDueImportColumn
        {
            ScheduleFor,
            MaintenanceDate,
            AssetName,
            ToolName,
            Serial,
            SchedulerName,
            ItemNumber,
            ItemCost,
            Quantity
        }
        public enum AssetToolSchedulerColumn
        {

            ScheduleFor,
            SchedulerName,
            Description,
            SchedulerType,
            TimeBasedUnit,
            TimeBasedFrequency,
            CheckOuts,
            OperationalHours,
            Mileage,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            ItemNumber,
            Quantity,
            IsDeleted
        }

        public enum ItemLocationChangeImportColumn
        {
            ItemNumber,
            NewLocationName,
            OldLocationName

        }


        public enum ImportToolAssetQuantityDetailsColumn
        {
            ID,
            BinID,
            SerialNumber,
            Received,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            GUID,
            ToolGUID,
            AssetGUID,
            Created,
            Updated,
            CreatedBy,
            LastUpdatedBy,
            CompanyID,
            RoomID,
            ToolName,
            AssetName,
            LocationName,
            Quantity,

        }

        public enum ImportMoveMaterialColumn
        {
            ItemNumber,
            SourceBin,
            DestinationBin,
            MoveType,
            Quantity,
            DestinationStagingHeader
        }

        public enum ImportEnterpriseQuickListColumn
        {
            Name,
            QLDetailNumber,
            Quantity
        }

        public enum ImportRequisitionColumn
        {
            RequisitionNumber,
            Workorder,
            RequiredDate,
            RequisitionStatus,
            CustomerName,
            ReleaseNumber,
            ProjectSpend,
            Description,
            StagingName,
            Supplier,
            SupplierAccount,
            BillingAccount,
            Technician,
            RequisitionUDF1,
            RequisitionUDF2,
            RequisitionUDF3,
            RequisitionUDF4,
            RequisitionUDF5,
            ItemNumber,
            Tool,
            ToolSerial,
            Bin,
            QuantityRequisitioned,
            QuantityApproved,
            QuantityPulled,
            LineItemRequiredDate,
            LineItemProjectSpend,
            LineItemSupplierAccount,
            PullOrderNumber,
            LineItemTechnician,
            PullUDF1,
            PullUDF2,
            PullUDF3,
            PullUDF4,
            PullUDF5,
            ToolCheckoutUDF1,
            ToolCheckoutUDF2,
            ToolCheckoutUDF3,
            ToolCheckoutUDF4,
            ToolCheckoutUDF5
        }

        public enum ImportSupplierCatalogColumn
        {
            ItemNumber,
            Description,
            SellPrice,
            //PackingQuantity, 
            ManufacturerPartNumber,
            ImagePath,
            UPC,
            SupplierPartNumber,
            SupplierName,
            ManufacturerName,
            //ConcatedColumnText, 
            UOM,
            CostUOM,
            Cost,
            UNSPSC,
            LongDescription,
            Category
        }

        public enum ImportReturnOrderItemsColumn
        {
            ID,
            Supplier,
            ReturnOrderNumber,
            ReturnDate,
            ReturnOrderStatus,
            StagingName,
            ReturnOrderComment,
            PackSlipNumber,
            ShippingTrackNumber,
            ReturnOrderUDF1,
            ReturnOrderUDF2,
            ReturnOrderUDF3,
            ReturnOrderUDF4,
            ReturnOrderUDF5,
            ShipVia,
            OrderType,
            ShippingVendor,
            AccountNumber,
            SupplierAccount,
            ItemNumber,
            Bin,
            RequestedReturnedQty,
            ReceivedReturnedQty,
            ASNNumber,
            ApprovedQty,
            InTransitQty,
            IsCloseItem,
            LineNumber,
            ControlNumber,
            ItemComment,
            LineItemUDF1,
            LineItemUDF2,
            LineItemUDF3,
            LineItemUDF4,
            LineItemUDF5,
            ReturnOrderCost
        }
        public enum CommonBOMTOItemColumn
        {
            ItemNumber,
            RoomName,
            BlanketOrderNumber,
            DefaultReorderQuantity,
            DefaultPullQuantity,
            LeadTimeInDays,
            Link1,
            Link2,
            Taxable,
            Consignment,
            OnHandQuantity,
            CriticalQuantity,
            MinimumQuantity,
            MaximumQuantity,
            WeightPerPiece,
            ItemUniqueNumber,
            IsTransfer,
            IsPurchase,
            InventryLocation,
            InventoryClassification,
            SerialNumberTracking,
            LotNumberTracking,
            DateCodeTracking,
            ItemType,
            ImagePath,
            UDF1,
            UDF2,
            UDF3,
            UDF4,
            UDF5,
            IsLotSerialExpiryCost,
            IsItemLevelMinMaxQtyRequired,
            TrendingSetting,
            EnforceDefaultPullQuantity,
            EnforceDefaultReorderQuantity,
            IsAutoInventoryClassification,
            IsBuildBreak,
            IsPackslipMandatoryAtReceive,
            ItemImageExternalURL,
            ItemDocExternalURL,
            IsDeleted,
            ItemLink2ExternalURL,
            IsActive,
            UDF6,
            UDF7,
            UDF8,
            UDF9,
            UDF10,
            IsAllowOrderCostuom,
            eLabelKey,
            POItemLineNumber

        }
        #endregion

        //public static MvcHtmlString RenderDropDownList(string ID, string ListType)
        //{
        //    if (ListType == "BinListFull")
        //    {
        //        BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
        //        StringBuilder sbTemp = new StringBuilder();
        //        var objUDF = objBinMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
        //        //var objUDF = objBinMasterApi.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
        //        sbTemp.Append("<select id=\"" + ID + "\" class=\"selectBox\">");
        //        sbTemp.Append("<option></option>");
        //        foreach (var item in objUDF)
        //        {
        //            sbTemp.Append("<option value=\"" + item.ID + "\">" + item.BinNumber + "</option>");
        //        }
        //        sbTemp.Append("</select>");

        //        return MvcHtmlString.Create(sbTemp.ToString());
        //    }
        //    return null;
        //}

        //public static string RenderDropDownList(string ID, bool IsStaging)
        //{
        //    BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
        //    StringBuilder sbTemp = new StringBuilder();
        //    var objUDF = objBinMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == IsStaging).OrderBy(x => x.BinNumber);
        //    //var objUDF = objBinMasterApi.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == IsStaging).OrderBy(x => x.BinNumber);
        //    sbTemp.Append("<select id=\"" + ID + "\" class=\"selectBox\">");
        //    if (objUDF != null && objUDF.Count() > 0)
        //    {
        //        foreach (var item in objUDF)
        //        {
        //            sbTemp.Append("<option value=\"" + item.ID + "\">" + HttpUtility.HtmlEncode(item.BinNumber) + "</option>");
        //        }
        //        sbTemp.Append("</select>");
        //    }
        //    else
        //    {
        //        sbTemp.Append("<option></option>");
        //    }

        //    //return MvcHtmlString.Create(sbTemp.ToString().Replace("'", "`"));
        //    return sbTemp.ToString();
        //}

        public static long[] ToIntArray(this string[] strArray)
        {
            return Array.ConvertAll<string, long>(strArray, delegate (string intParameter)
            {
                long retval = 0;
                long.TryParse((intParameter ?? string.Empty).ToString(), out retval);
                return retval;
            });
        }

        /// <summary>
        /// GetStringFromXSLT
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="xmlString"></param>
        /// <param name="xsltPath"></param>
        /// <returns></returns>
        public static MvcHtmlString GetHTMLStringFromXSLT(this HtmlHelper htmlHelper, string xmlString, string xsltPath)
        {
            XslCompiledTransform xsl = new XslCompiledTransform();
            XsltArgumentList xslArgs = new XsltArgumentList();
            string urlPart = HttpContext.Current.Request.Url.ToString();
            string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];
            xslArgs.AddParam("eturnslogourl", "", replacePart + "/Content/OpenAccess/logoInReport.png");
            xslArgs.AddParam("copyrightyear", "", DateTimeUtility.DateTimeNow.Year.ToString() + " ");
            xsl.Load(xsltPath);
            StringReader stream = new StringReader(xmlString);
            XPathDocument xpath = new XPathDocument(stream);
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            xsl.Transform(xpath, xslArgs, writer);
            return MvcHtmlString.Create(sw.ToString());
        }

        /// <summary>
        /// CreateXML
        /// </summary>
        /// <param name="YourClassObject"></param>
        /// <returns></returns>
        public static string CreateXML(Object YourClassObject)
        {

            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(YourClassObject.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, YourClassObject);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }

        public static IEnumerable<EmailUserConfigurationDTO> GetExternalUserEmailAddress(string eMailTemplateName, long RoomID, long CompanyID)
        {
            EmailUserConfigurationDAL objExternalUser = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<EmailUserConfigurationDTO> lstExternalUser = objExternalUser.GetAllExternalUserRecords(eMailTemplateName, true, RoomID, CompanyID);
            return lstExternalUser;
        }

        public static IEnumerable<EmailUserConfigurationDTO> GetExternalUserEmailAddress(string eMailTemplateName, long RoomID, long CompanyID, string DataBaseName)
        {
            EmailUserConfigurationDAL objExternalUser = new EmailUserConfigurationDAL(DataBaseName);
            IEnumerable<EmailUserConfigurationDTO> lstExternalUser = objExternalUser.GetAllExternalUserRecords(eMailTemplateName, true, RoomID, CompanyID);
            return lstExternalUser;
        }

        public static string GetEmailToAddress(List<UserMasterDTO> arrUsers, string Status)
        {
            string strToAddress = string.Empty;
            try
            {

                strToAddress = ConfigurationManager.AppSettings["OverrideToEmail"];
                if (!string.IsNullOrEmpty(strToAddress))
                {
                    Array ArrToAddress = strToAddress.Split(',');
                    //if (strToAddress.Contains(","))
                    //{
                    //  ArrToAddress = 
                    //}
                    //else
                    //{
                    //    ArrToAddress.SetValue(strToAddress, 0);
                    //}
                    string strEmails = string.Empty;
                    foreach (var itemEmail in ArrToAddress)
                    {

                        if (arrUsers != null && arrUsers.Count > 0)
                        {
                            foreach (var item in arrUsers)
                            {
                                if (!string.IsNullOrEmpty(item.Email))
                                {
                                    if (!string.IsNullOrEmpty(strEmails))
                                        strEmails += ",";

                                    strEmails += @"""" + item.Email + @"""" + @"<" + itemEmail + @">";
                                }
                            }
                        }
                        //Add External User for send mail
                        IEnumerable<EmailUserConfigurationDTO> extUsers = GetExternalUserEmailAddress(Status, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (extUsers != null && extUsers.Count() > 0)
                        {
                            foreach (var item in extUsers)
                            {
                                if (!string.IsNullOrEmpty(strEmails))
                                    strEmails += ",";

                                strEmails += @"""" + item.Email + @"""" + @"<" + itemEmail + @">";
                            }
                        }


                    }
                    if (!string.IsNullOrEmpty(strEmails))
                        strToAddress = strEmails;


                }
                else
                {
                    strToAddress = string.Empty;
                    if (arrUsers != null && arrUsers.Count > 0)
                    {
                        foreach (var item in arrUsers)
                        {
                            if (!string.IsNullOrEmpty(strToAddress))
                                strToAddress += ",";

                            strToAddress += item.Email;
                        }
                    }
                    //Add External User for send mail
                    IEnumerable<EmailUserConfigurationDTO> extUsers = GetExternalUserEmailAddress(Status, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (extUsers != null && extUsers.Count() > 0)
                    {
                        foreach (var item in extUsers)
                        {
                            if (!string.IsNullOrEmpty(strToAddress))
                                strToAddress += ",";

                            strToAddress += item.Email;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
            return strToAddress;
        }

        public static string GetEmailToAddress(List<UserMasterDTO> arrUsers, string Status, Int64 RoomID, Int64 CompanyID, string dataBaseName)
        {
            string strToAddress = string.Empty;
            try
            {

                strToAddress = ConfigurationManager.AppSettings["OverrideToEmail"];
                if (!string.IsNullOrEmpty(strToAddress))
                {
                    Array ArrToAddress = strToAddress.Split(',');

                    string strEmails = string.Empty;
                    foreach (var itemEmail in ArrToAddress)
                    {

                        if (arrUsers != null && arrUsers.Count > 0)
                        {
                            foreach (var item in arrUsers)
                            {
                                if (!string.IsNullOrEmpty(item.Email))
                                {
                                    if (!string.IsNullOrEmpty(strEmails))
                                        strEmails += ",";

                                    strEmails += @"""" + item.Email + @"""" + @"<" + itemEmail + @">";
                                }
                            }
                        }
                        //Add External User for send mail
                        IEnumerable<EmailUserConfigurationDTO> extUsers = GetExternalUserEmailAddress(Status, RoomID, CompanyID, dataBaseName);
                        if (extUsers != null && extUsers.Count() > 0)
                        {
                            foreach (var item in extUsers)
                            {
                                if (!string.IsNullOrEmpty(strEmails))
                                    strEmails += ",";

                                strEmails += @"""" + item.Email + @"""" + @"<" + itemEmail + @">";
                            }
                        }


                    }
                    if (!string.IsNullOrEmpty(strEmails))
                        strToAddress = strEmails;


                }
                else
                {
                    strToAddress = string.Empty;
                    if (arrUsers != null && arrUsers.Count > 0)
                    {
                        foreach (var item in arrUsers)
                        {
                            if (!string.IsNullOrEmpty(strToAddress))
                                strToAddress += ",";

                            strToAddress += item.Email;
                        }
                    }
                    //Add External User for send mail
                    IEnumerable<EmailUserConfigurationDTO> extUsers = GetExternalUserEmailAddress(Status, RoomID, CompanyID, dataBaseName);
                    if (extUsers != null && extUsers.Count() > 0)
                    {
                        foreach (var item in extUsers)
                        {
                            if (!string.IsNullOrEmpty(strToAddress))
                                strToAddress += ",";

                            strToAddress += item.Email;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
            return strToAddress;
        }
        public static string GeteTurnsImage(string path, string imagePath)
        {
            string str = string.Empty;

            str = @"<a href='" + path + @"' title=""E Turns Powered""> <img alt=""E Turns Powered"" src='" + path + imagePath + @"' style=""border: 0px currentColor; border-image: none;"" /></a>";
            return str;
        }

        public static string EncryptText(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.PKCS7;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string DecryptText(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public static string ComputeHash(string plainText, string hashAlgorithm, byte[] saltBytes)
        {
            // If salt is not specified, generate it on the fly.
            if (saltBytes == null)
            {
                // Define min and max salt sizes.
                int minSaltSize = 4;
                int maxSaltSize = 8;

                // Generate a random number for the size of the salt.
                Random random = new Random();
                int saltSize = random.Next(minSaltSize, maxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(saltBytes);
            }

            // Convert plain text into a byte array.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes =
                    new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (int i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;

            // Make sure hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hash = new SHA1Managed();
                    break;

                case "SHA256":
                    hash = new SHA256Managed();
                    break;

                case "SHA384":
                    hash = new SHA384Managed();
                    break;

                case "SHA512":
                    hash = new SHA512Managed();
                    break;

                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length +
                                                saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            string hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }

        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        public static string getSHA15Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            SHA1CryptoServiceProvider Sha1Hasher = new SHA1CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = Sha1Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        // Verify a hash against a string. 
        public static bool verifyMd5Hash(string input, string hash)
        {
            // Hash the input. 
            string hashOfInput = getMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool VerifyHash(string plainText, string hashAlgorithm, string hashValue)
        {
            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // We must know size of hash (without salt).
            int hashSizeInBits, hashSizeInBytes;

            // Make sure that hashing algorithm name is specified.
            if (hashAlgorithm == null)
                hashAlgorithm = "";

            // Size of hash is based on the specified algorithm.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hashSizeInBits = 160;
                    break;

                case "SHA256":
                    hashSizeInBits = 256;
                    break;

                case "SHA384":
                    hashSizeInBits = 384;
                    break;

                case "SHA512":
                    hashSizeInBits = 512;
                    break;

                default: // Must be MD5
                    hashSizeInBits = 128;
                    break;
            }

            // Convert size of hash from bits to bytes.
            hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;

            // Allocate array to hold original salt bytes retrieved from hash.
            byte[] saltBytes = new byte[hashWithSaltBytes.Length -
                                        hashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

            // Compute a new hash string.
            string expectedHashString =
                        ComputeHash(plainText, hashAlgorithm, saltBytes);

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashValue == expectedHashString);
        }

        public static string ToClientTime(this DateTime dt)
        {
            // read the value from session
            var timeOffSet = HttpContext.Current.Session["timezoneoffset"];

            if (timeOffSet != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                dt = dt.AddMinutes(-1 * offset);

                return dt.ToString();
            }

            // if there is no offset in session return the datetime in server timezone
            return dt.ToLocalTime().ToString();
        }
        public static DateTime? ToClientDateTime(this DateTime? dt)
        {
            // read the value from session
            if (dt.HasValue && dt != DateTime.MinValue)
            {
                TimeZone localZone = TimeZone.CurrentTimeZone;

                double ServerOffset = localZone.GetUtcOffset(dt.Value).TotalMinutes;
                var timeOffSet = HttpContext.Current.Session["timezoneoffset"];
                //dt = TimeZoneInfo.ConvertTimeFromUtc(dt.Value, TimeZone.CurrentTimeZone);
                if (timeOffSet != null)
                {
                    var offset = double.Parse(timeOffSet.ToString());
                    //offset = offset;
                    dt = dt.Value.AddMinutes(-1 * offset);
                    return dt;
                }
                return dt.Value.ToLocalTime();
            }
            else
            {
                return null;
            }
            // if there is no offset in session return the datetime in server timezone

        }

        public static void SetUserLocale()
        {
            HttpRequest Request = HttpContext.Current.Request;
            if (Request.UserLanguages == null)
                return;

            string Lang = Request.UserLanguages[0];

        }

        public static DateTime TransactionTime
        {
            get
            {

                return DateTime.UtcNow;
            }
        }

        public static DateTime? ConvertLocalDateTimeToUTCDateTime(DateTime? targetDate)
        {
            if (targetDate.HasValue && targetDate.Value != DateTime.MinValue)
            {
                //TimeZoneInfo.ConvertTimeToUtc(targetDate.Value,)
                TimeZoneInfo easternZone = TimeZoneInfo.Utc;
                return TimeZoneInfo.ConvertTime(targetDate.Value, easternZone, TimeZoneInfo.Utc);
            }
            return null;
        }

        public static string ConvertDateByTimeZone(DateTime? DateToConvert, TimeZoneInfo DestinationTimeZone, string DateTimeFormate, CultureInfo RoomCulture, bool ConvertAndFormat)
        {

            DateTime? retdate = null;
            if (DateToConvert.HasValue && DateToConvert.Value != DateTime.MinValue)
            {
                DateToConvert = DateTime.SpecifyKind(DateToConvert.Value, DateTimeKind.Unspecified);
                if (ConvertAndFormat)
                {
                    retdate = TimeZoneInfo.ConvertTimeFromUtc(DateToConvert.Value, DestinationTimeZone);
                }
                else
                {
                    retdate = DateToConvert;
                }
                return retdate.Value.ToString(DateTimeFormate);
            }
            return string.Empty;
        }


        public static DateTime? ConvertDateByTimeZonedt(DateTime? DateToConvert, TimeZoneInfo DestinationTimeZone, string DateTimeFormate, CultureInfo RoomCulture)
        {

            DateTime? retdate = null;
            if (DateToConvert.HasValue && DateToConvert.Value != DateTime.MinValue)
            {
                DateToConvert = DateTime.SpecifyKind(DateToConvert.Value, DateTimeKind.Unspecified);
                retdate = TimeZoneInfo.ConvertTimeFromUtc(DateToConvert.Value, DestinationTimeZone);
                return retdate;
            }
            return null;
        }

        public static string GetFormatedCostQtyValues(double? objValue, int objType)
        {
            if (!objValue.HasValue)
                objValue = 0;

            string qtyFormat = "N";
            if (objType == 1)
            {
                //if (SessionHelper.CompanyConfig != null)
                //    qtyFormat = "N" + SessionHelper.CompanyConfig.CostDecimalPoints.GetValueOrDefault(0);

                if (!string.IsNullOrEmpty(SessionHelper.CurrencyDecimalDigits))
                    qtyFormat = "N" + SessionHelper.CurrencyDecimalDigits;
                return objValue.Value.ToString(qtyFormat);
                //return Math.Round(objValue.Value, SessionHelper.CompanyConfig.CostDecimalPoints.GetValueOrDefault(0)).ToString();
            }
            else if (objType == 2)
            {
                //if (SessionHelper.CompanyConfig != null)
                //    qtyFormat = "N" + SessionHelper.CompanyConfig.QuantityDecimalPoints.GetValueOrDefault(0);
                if (!string.IsNullOrEmpty(SessionHelper.NumberDecimalDigits))
                    qtyFormat = "N" + SessionHelper.NumberDecimalDigits;
                return objValue.Value.ToString(qtyFormat);
                //return Math.Round(objValue.Value, SessionHelper.CompanyConfig.QuantityDecimalPoints.GetValueOrDefault(0)).ToString();
            }
            else if (objType == 4)
            {
                //if (SessionHelper.CompanyConfig != null)
                //    qtyFormat = "N" + SessionHelper.CompanyConfig.TurnsAvgDecimalPoints.GetValueOrDefault(0);

                if (!string.IsNullOrEmpty(SessionHelper.TurnUsageFormat))
                    qtyFormat = "N" + SessionHelper.NumberAvgDecimalPoints;
                return objValue.Value.ToString(qtyFormat);
                //return Math.Round(objValue.Value, SessionHelper.CompanyConfig.TurnsAvgDecimalPoints.GetValueOrDefault(0)).ToString();
            }
            else
            {
                return objValue.ToString();
            }
        }

        public static string GetFormatedCostQtyValues(decimal? objValue, int objType)
        {
            if (!objValue.HasValue)
                objValue = 0;

            string qtyFormat = "N";
            if (objType == 1)
            {
                //if (SessionHelper.CompanyConfig != null)
                //    qtyFormat = "N" + SessionHelper.CompanyConfig.CostDecimalPoints.GetValueOrDefault(0);

                if (!string.IsNullOrEmpty(SessionHelper.CurrencyDecimalDigits))
                    qtyFormat = "N" + SessionHelper.CurrencyDecimalDigits;
                return objValue.Value.ToString(qtyFormat);
                //return Math.Round(objValue.Value, SessionHelper.CompanyConfig.CostDecimalPoints.GetValueOrDefault(0)).ToString();
            }
            else if (objType == 2)
            {
                //if (SessionHelper.CompanyConfig != null)
                //    qtyFormat = "N" + SessionHelper.CompanyConfig.QuantityDecimalPoints.GetValueOrDefault(0);

                if (!string.IsNullOrEmpty(SessionHelper.NumberDecimalDigits))
                    qtyFormat = "N" + SessionHelper.NumberDecimalDigits;
                return objValue.Value.ToString(qtyFormat);
                //return Math.Round(objValue.Value, SessionHelper.CompanyConfig.QuantityDecimalPoints.GetValueOrDefault(0)).ToString();
            }
            else if (objType == 4)
            {
                //if (SessionHelper.CompanyConfig != null)
                //    qtyFormat = "N" + SessionHelper.CompanyConfig.TurnsAvgDecimalPoints.GetValueOrDefault(0);

                if (!string.IsNullOrEmpty(SessionHelper.TurnUsageFormat))
                    qtyFormat = "N" + SessionHelper.TurnUsageFormat;
                return objValue.Value.ToString(qtyFormat);
                //return Math.Round(objValue.Value, SessionHelper.CompanyConfig.TurnsAvgDecimalPoints.GetValueOrDefault(0)).ToString();
            }
            else
            {
                return objValue.ToString();
            }
        }

        public static string htmlEscape(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                //return str.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&#39;").Replace("<", "&lt;").Replace(">", "&gt;");
                return str.Replace("&", "").Replace("\"", "").Replace("'", "").Replace("<", "").Replace(">", "");
            }
            else
                return str;
        }


        public static void LogError(Exception ex, Int64 RoomId, Int64 CompanyID, Int64 EnterpriseID)
        {
            eTurnsMaster.DAL.ReportSchedulerError objReportSchedulerError = new eTurnsMaster.DAL.ReportSchedulerError();
            eTurnsMaster.DAL.CommonMasterDAL commonDAL = new eTurnsMaster.DAL.CommonMasterDAL();
            objReportSchedulerError.RoomID = RoomId;
            objReportSchedulerError.CompanyID = CompanyID;
            objReportSchedulerError.EnterpriseID = EnterpriseID;

            objReportSchedulerError.Exception = Convert.ToString(ex) ?? "Null exception";
            if (HttpContext.Current.Request != null && HttpContext.Current.Request.Url != null)
            {
                objReportSchedulerError.Exception = objReportSchedulerError.Exception + " " + HttpContext.Current.Request.Url.ToString();
            }
            objReportSchedulerError.ID = 0;
            objReportSchedulerError.NotificationID = 0;
            objReportSchedulerError.ScheduleFor = 0;
            commonDAL.SaveNotificationError(objReportSchedulerError);
            if (ex.InnerException != null)
            {
                LogError(ex.InnerException, RoomId, CompanyID, EnterpriseID);
            }

        }

        public static string SetjQueryTableSearchValue(string SearchString, string Key, string Value)
        {
            if (SearchString != null && !string.IsNullOrEmpty(SearchString) && !string.IsNullOrWhiteSpace(SearchString))
            {
                string[] ArrStr1 = SearchString.Split(new string[] { "[###]" }, StringSplitOptions.None);
                if (ArrStr1.Length > 2)
                {
                    string[] ArrStr2 = ArrStr1[0].Split(',');
                    string[] ArrStr3 = ArrStr1[1].Split('@');
                    int Index = Array.IndexOf(ArrStr2, Key);
                    if (Index >= 0 && ArrStr3.Length >= Index)
                    {
                        ArrStr3[Index] = Value;
                        SearchString = String.Join(",", ArrStr2) + "[###]" + String.Join("@", ArrStr3) + "[###]" + ArrStr1[2];
                    }
                }
                return SearchString;
            }
            else
            {
                return "";
            }

        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string GetExportFileName(string ModuleName, string fileType = "csv")
        {

            string ExportFileName = ModuleName;
            string dateFormat = "yyyy/MM/dd";

            //CSVFileName = SessionHelper.CompanyID + "_" + SessionHelper.RoomID + "_" + DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy HH:mm:ss").Replace("/", "").Replace(":", "") + ModuleName + ".csv";
            ExportFileName = ModuleName + "_" + DateTimeUtility.DateTimeNow.ToString(dateFormat).Replace("/", "") + "." + fileType;
            return ExportFileName;
        }


        public static void SaveBOMImageToRoomItem(string strID, string saveImageFor = null)
        {
            if (!string.IsNullOrEmpty(strID))
            {
                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                string BOMInvPhotoPathRoot = SiteSettingHelper.BOMInventoryPhoto;  //Settinfile.Element("BOMInventoryPhoto").Value;
                string BOMInvLink2PathRoot = SiteSettingHelper.BOMInventoryLink2; // Settinfile.Element("BOMInventoryLink2").Value;

                string InvPhotoPathRoot = SiteSettingHelper.InventoryPhoto; // Settinfile.Element("InventoryPhoto").Value;
                string InvLink2PathRoot = SiteSettingHelper.InventoryLink2; // Settinfile.Element("InventoryLink2").Value;

                BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                List<BOMItemDTO> bomItemList = new List<BOMItemDTO>();

                bomItemList = obj.CompanyBOMItems(SessionHelper.CompanyID, false, false, strID);
                List<ItemMasterDTO> roomItemList = new List<ItemMasterDTO>();
                List<ItemMasterDTO> simpleItemList = null;
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                roomItemList = objItemMasterDAL.GetAllItemByBOMRefID(SessionHelper.CompanyID);
                foreach (BOMItemDTO bitem in bomItemList)
                {
                    if (!string.IsNullOrEmpty(bitem.ImagePath) || !string.IsNullOrEmpty(bitem.Link2))
                    {

                        Int64 EnterpriseId = SessionHelper.EnterPriceID;
                        Int64 CompanyID = SessionHelper.CompanyID;
                        string imagePath = System.Web.HttpContext.Current.Server.MapPath(BOMInvPhotoPathRoot + EnterpriseId + "/" + CompanyID + "/" + bitem.ID + "/" + bitem.ImagePath);
                        string link2Path = System.Web.HttpContext.Current.Server.MapPath(BOMInvLink2PathRoot + EnterpriseId + "/" + CompanyID + "/" + bitem.ID + "/" + bitem.Link2);

                        simpleItemList = new List<ItemMasterDTO>();
                        simpleItemList = roomItemList.Where(x => x.RefBomId == bitem.ID).ToList();
                        foreach (ItemMasterDTO sitem in simpleItemList)
                        {
                            if (!string.IsNullOrEmpty(bitem.ImagePath) && !string.IsNullOrEmpty(saveImageFor) && (saveImageFor == "image" || saveImageFor == "both"))
                            {
                                string newImagePath = string.Empty;
                                newImagePath = System.Web.HttpContext.Current.Server.MapPath(InvPhotoPathRoot + EnterpriseId + "/" + CompanyID + "/" + sitem.Room.GetValueOrDefault(0) + "/" + sitem.ID + "/");
                                if (System.IO.File.Exists(imagePath))
                                {
                                    if (!Directory.Exists(newImagePath))
                                    {
                                        Directory.CreateDirectory(newImagePath);
                                    }

                                    System.IO.File.Copy(imagePath, newImagePath + bitem.ImagePath, true);
                                }
                            }

                            if (!string.IsNullOrEmpty(bitem.Link2) && !string.IsNullOrEmpty(saveImageFor) && (saveImageFor == "link2" || saveImageFor == "both"))
                            {
                                string newLink2Path = string.Empty;
                                newLink2Path = System.Web.HttpContext.Current.Server.MapPath(InvLink2PathRoot + EnterpriseId + "/" + CompanyID + "/" + sitem.Room.GetValueOrDefault(0) + "/" + sitem.ID + "/");
                                if (System.IO.File.Exists(link2Path))
                                {
                                    if (!Directory.Exists(newLink2Path))
                                    {
                                        Directory.CreateDirectory(newLink2Path);
                                    }

                                    System.IO.File.Copy(link2Path, newLink2Path + bitem.Link2, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static List<AlleTurnsActionMethodsDTO> GetAlleTurnsMethods()
        {
            eTurnsMaster.DAL.CommonMasterDAL objDAL = new eTurnsMaster.DAL.CommonMasterDAL();
            List<AlleTurnsActionMethodsDTO> methodList = new List<AlleTurnsActionMethodsDTO>();
            methodList = objDAL.GetAlleTurnsActionMethodsData().ToList();
            return methodList;
        }

        public static void SaveeTurnsAuthorizationError(long EnterPriceID, long CompanyID, long RoomID, long UserID, string UserName, string ActionName, string ControllerName, string ErrorPurposeFor, string ErrorException)
        {
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string _strSaveError = "No";

            if (ErrorPurposeFor == "Authorization")
            {
                //if (Settinfile.Element("SaveNewAuthorizationError") != null)
                //{
                //    _strSaveError = Convert.ToString(Settinfile.Element("SaveNewAuthorizationError").Value);
                //}

                if (SiteSettingHelper.SaveNewAuthorizationError != string.Empty)
                {
                    _strSaveError = Convert.ToString(SiteSettingHelper.SaveNewAuthorizationError);
                }
            }
            else if (ErrorPurposeFor == "AntiForgery")
            {
                //if (Settinfile.Element("SaveAntiForgeryError") != null)
                //{
                //    _strSaveError = Convert.ToString(Settinfile.Element("SaveAntiForgeryError").Value);
                //}

                if (SiteSettingHelper.SaveAntiForgeryError != string.Empty)
                {
                    _strSaveError = Convert.ToString(SiteSettingHelper.SaveAntiForgeryError);
                }

            }


            if ((_strSaveError ?? string.Empty).ToLower() == "yes")
            {
                eTurnsMaster.DAL.CommonMasterDAL objDAL = new eTurnsMaster.DAL.CommonMasterDAL();
                objDAL.SaveeTurnsAuthorizationError(EnterPriceID, CompanyID, RoomID, UserID, UserName, ActionName, ControllerName, ErrorPurposeFor, ErrorException);
            }
        }

        public static string GetReleaseNumber()
        {
            //var ObjCache = CacheHelper<string>.GetCacheItem("ReleaseNumber");
            string releaseNumber = DateTime.UtcNow.ToString("yyyy-MM-dd");

            //if (ObjCache == null)
            //{
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

            //if (Settinfile.Element("ReleaseNumber") != null)
            //{
            //    releaseNumber = Convert.ToString(Settinfile.Element("ReleaseNumber").Value);
            //}
            if (SiteSettingHelper.ReleaseNumber != string.Empty)
            {
                releaseNumber = Convert.ToString(SiteSettingHelper.ReleaseNumber);
            }

            //string cache = CacheHelper<string>.AddCacheItem("ReleaseNumber", releaseNumber);
            //}
            //else
            //{
            //    releaseNumber = ObjCache;
            //}
            return releaseNumber;
        }

        public static bool GetIsLoadEnterpriseGridOrdering()
        {
            //var ObjCache = CacheHelper<bool?>.GetCacheItem("LoadEnterpriseGridOrdering");
            bool isLoadEnterpriseGridOrdering = false;

            //if (ObjCache == null)
            //{
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

            //if (Settinfile.Element("LoadEnterpriseGridOrdering") != null)
            //{
            //    isLoadEnterpriseGridOrdering = Convert.ToBoolean(Settinfile.Element("LoadEnterpriseGridOrdering").Value);
            //}

            if (SiteSettingHelper.LoadEnterpriseGridOrdering != string.Empty)
            {
                isLoadEnterpriseGridOrdering = Convert.ToBoolean(SiteSettingHelper.LoadEnterpriseGridOrdering);
            }

            //bool? cache = CacheHelper<bool?>.AddCacheItem("LoadEnterpriseGridOrdering", isLoadEnterpriseGridOrdering);
            //}
            //else
            //{
            //    isLoadEnterpriseGridOrdering = ObjCache.Value;
            //}
            return isLoadEnterpriseGridOrdering;
        }

        public static string GetNotAllowCharList()
        {
            EnterPriseConfigDTO obj = new eTurns.DAL.EnterPriseConfigDAL(SessionHelper.EnterPriseDBName).GetRecord(eTurnsWeb.Helper.SessionHelper.EnterPriceID);
            string NotAllowedCharacterList = string.Empty;
            string CharCodeList = string.Empty;
            if (obj != null)
            {
                NotAllowedCharacterList = obj.NotAllowedCharacter;
                if (NotAllowedCharacterList != null && (!string.IsNullOrEmpty(NotAllowedCharacterList)))
                {
                    foreach (string s in NotAllowedCharacterList.Split(','))
                    {
                        foreach (char c in s.ToCharArray())
                        {
                            int unicode = c;
                            if (!string.IsNullOrEmpty(CharCodeList))
                            {
                                CharCodeList = CharCodeList + "," + unicode;
                            }
                            else
                            {
                                CharCodeList = Convert.ToString(unicode);
                            }
                        }
                    }
                }
            }
            return CharCodeList;
        }


        public static string ValidateBOMConsignToRoomItem(string strID)
        {
            string strMsg = string.Empty;
            if (!string.IsNullOrEmpty(strID))
            {
                BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                List<BOMItemDTO> bomItemList = new List<BOMItemDTO>();

                bomItemList = obj.CompanyBOMItems(SessionHelper.CompanyID, false, false, strID);
                List<ItemMasterDTO> roomItemList = new List<ItemMasterDTO>();
                List<ItemMasterDTO> simpleItemList = null;
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                roomItemList = objItemMasterDAL.GetAllItemByBOMRefID(SessionHelper.CompanyID, SessionHelper.RoomID);
                foreach (BOMItemDTO bitem in bomItemList)
                {
                    Int64 EnterpriseId = SessionHelper.EnterPriceID;
                    Int64 CompanyID = SessionHelper.CompanyID;

                    simpleItemList = new List<ItemMasterDTO>();
                    simpleItemList = roomItemList.Where(x => x.RefBomId == bitem.ID).ToList();
                    foreach (ItemMasterDTO sitem in simpleItemList)
                    {
                        if (bitem.Consignment && sitem.Consignment == false)
                        {
                            strMsg += bitem.ItemNumber + " " + ResBomItemMaster.ConsignValidate + " " + Environment.NewLine;
                        }
                    }
                }
            }
            return strMsg;
        }

        //public static bool ValidateExternalImage(string url)
        //{
        //    bool isvalid = true;
        //    //System.Net.HttpWebRequest wreq;
        //    //System.Net.HttpWebResponse wresp;
        //    //System.IO.Stream mystream;
        //    //System.Drawing.Bitmap bmp;

        //    //bmp = null;
        //    //mystream = null;
        //    //wresp = null;
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //    request.AllowWriteStreamBuffering = true;
        //    request.Method = "HEAD";
        //    try
        //    {
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            isvalid = true;
        //        }
        //        else
        //        {
        //            isvalid = false;
        //        }

        //        //wreq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
        //        //wreq.AllowWriteStreamBuffering = true;
        //        //wreq.Method = "HEAD";

        //        //wresp = (HttpWebResponse)wreq.GetResponse();

        //        //if ((mystream = wresp.GetResponseStream()) != null)
        //        //    bmp = new System.Drawing.Bitmap(mystream);
        //        //isvalid = true;
        //    }
        //    catch
        //    {
        //        isvalid = false;
        //    }
        //    finally
        //    {
        //        //if (mystream != null)
        //        //    mystream.Close();

        //        //if (wresp != null)
        //        //    wresp.Close();
        //    }
        //    return isvalid;
        //}

        public static bool ValidateExternalImage(string url)
        {
            bool isvalid = true;
            System.Net.HttpWebRequest wreq;
            System.Net.HttpWebResponse wresp;
            System.IO.Stream mystream;
            System.Drawing.Bitmap bmp;

            bmp = null;
            mystream = null;
            wresp = null;
            try
            {
                wreq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                wreq.AllowWriteStreamBuffering = true;
                //request.Method = "HEAD";

                wresp = (HttpWebResponse)wreq.GetResponse();

                if ((mystream = wresp.GetResponseStream()) != null)
                    bmp = new System.Drawing.Bitmap(mystream);
                isvalid = true;
            }
            catch
            {
                isvalid = false;
            }
            finally
            {
                if (mystream != null)
                    mystream.Close();

                if (wresp != null)
                    wresp.Close();
            }
            return isvalid;
        }

        public static string OnTransferInApproveEventName = "OTIA";
        public static string OnTransferOutApproveEventName = "OTOA";

        public static bool IsOldeVMIRoom()
        {
            bool _IsOldeVMIRoom = false;
            string CurrentRoomFullId = eTurnsWeb.Helper.SessionHelper.EnterPriceID + "_" + eTurnsWeb.Helper.SessionHelper.CompanyID + "_" + eTurnsWeb.Helper.SessionHelper.RoomID;
            if ((SiteSettingHelper.eVMIRooms ?? string.Empty).ToLower().Contains(CurrentRoomFullId.ToLower()))
            {
                _IsOldeVMIRoom = true;
            }

            return _IsOldeVMIRoom;
        }
        public static bool IsOldeVMIRoomRB()
        {
            string streVMIRooms = SiteSettingHelper.eVMIRooms;
            bool _IsOldeVMIRoom = false;
            if (!string.IsNullOrWhiteSpace(streVMIRooms))
            {
                string[] arrEntCmpRoom = streVMIRooms.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrEntCmpRoom != null && arrEntCmpRoom.Length > 0)
                {
                    foreach (string strEntCmpRoom in arrEntCmpRoom)
                    {
                        string[] EntCmpRoom = strEntCmpRoom.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        if (EntCmpRoom != null && EntCmpRoom.Length > 0 && SessionHelper.RoomID == Convert.ToInt64(EntCmpRoom[2]))
                        {
                            _IsOldeVMIRoom = true;
                            break;
                        }
                    }
                }
            }

            return _IsOldeVMIRoom;
        }

        public static bool IsRoomActive()
        {
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            RoomDTO objRoomDTo = objRoomDAL.IsRoomActive(SessionHelper.RoomID);
            if (objRoomDTo != null && objRoomDTo.IsRoomActive)
                return true;

            return false;
        }

        public static bool CheckUDFIsRequiredAndInsert(IEnumerable<UDFDTO> DataFromDB, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, out string Reason, string prefix = "", string UDF6 = "", string UDF7 = "", string UDF8 = "", string UDF9 = "", string UDF10 = "",long RoomID = 0)
        {
            bool isRequired = false;
            Reason = string.Empty;
            var udfRequireMsg = ResMessage.MsgRequired;
            var allowedMaxCharForUDF = ResUDFSetup.AllowedMaxCharacterForUDF;
            if (DataFromDB != null && DataFromDB.Count() > 0)
            {

                foreach (var i in DataFromDB)
                {
                    if (i.UDFColumnName == "UDF1")
                    {
                        if (string.IsNullOrEmpty(UDF1) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF1) && (UDF1 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF1) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF1,RoomID);
                        }
                    }
                    if (i.UDFColumnName == "UDF2")
                    {
                        if (string.IsNullOrEmpty(UDF2) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF2) && (UDF2 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF2) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF2,RoomID);
                        }
                    }
                    if (i.UDFColumnName == "UDF3")
                    {
                        if (string.IsNullOrEmpty(UDF3) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF3) && (UDF3 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF3) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF3,RoomID);
                        }
                    }
                    if (i.UDFColumnName == "UDF4")
                    {
                        if (string.IsNullOrEmpty(UDF4) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF4) && (UDF4 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF4) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF4,RoomID);
                        }
                    }
                    if (i.UDFColumnName == "UDF5")
                    {
                        if (string.IsNullOrEmpty(UDF5) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF5) && (UDF5 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF5) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF5,RoomID);
                        }
                    }
                    if (i.UDFColumnName == "UDF6")
                    {
                        if (string.IsNullOrEmpty(UDF6) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF6) && (UDF6 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF6) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF6,RoomID);
                        }
                    }
                    if (i.UDFColumnName == "UDF7")
                    {
                        if (string.IsNullOrEmpty(UDF7) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF7) && (UDF7 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF7) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF7,RoomID);
                        }
                    }
                    if (i.UDFColumnName == "UDF8")
                    {
                        if (string.IsNullOrEmpty(UDF8) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF8) && (UDF8 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF8) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF8,RoomID);
                        }
                    }
                    if (i.UDFColumnName == "UDF9")
                    {
                        if (string.IsNullOrEmpty(UDF9) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF9) && (UDF9 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF9) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF9,RoomID);
                        }
                    }
                    if (i.UDFColumnName == "UDF10")
                    {
                        if (string.IsNullOrEmpty(UDF10) && i.UDFIsRequired.HasValue && i.UDFIsRequired.Value == true)
                        {
                            Reason = Reason + " " + string.Format(udfRequireMsg, prefix + "" + i.UDFColumnName);
                        }
                        else if (!string.IsNullOrEmpty(UDF10) && (UDF10 ?? string.Empty).Length > (i.UDFMaxLength ?? 0))
                        {
                            Reason = Reason + " " + string.Format(allowedMaxCharForUDF, prefix + "" + i.UDFColumnName, i.UDFMaxLength);
                        }
                        else if (!string.IsNullOrEmpty(UDF10) && !string.IsNullOrEmpty(i.UDFControlType) && i.UDFControlType != "Textbox")
                        {
                            InsertUDFOptionMultiimport(i, UDF10,RoomID);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Reason))
                {
                    isRequired = true;
                }
            }

            return isRequired;
        }

        public static void InsertUDFOption(UDFDTO objDTO, string UDFValue)
        {
            eTurns.DAL.UDFOptionDAL objUDFOptionDAL = new eTurns.DAL.UDFOptionDAL(SessionHelper.EnterPriseDBName);
            bool IsExist = objUDFOptionDAL.IsUdfOptionExistsInUDF(objDTO.ID, UDFValue);

            if (IsExist == false)
            {
                UDFOptionsDTO objUDFOptionsDTO = new UDFOptionsDTO();
                objUDFOptionsDTO.ID = 0;
                objUDFOptionsDTO.UDFID = objDTO.ID;
                objUDFOptionsDTO.UDFOption = UDFValue;
                objUDFOptionsDTO.Created = DateTimeUtility.DateTimeNow;
                objUDFOptionsDTO.CreatedBy = SessionHelper.UserID;
                objUDFOptionsDTO.Updated = DateTimeUtility.DateTimeNow;
                objUDFOptionsDTO.LastUpdatedBy = SessionHelper.UserID;
                objUDFOptionsDTO.CompanyID = SessionHelper.CompanyID;
                objUDFOptionsDTO.Room = SessionHelper.RoomID;
                objUDFOptionsDTO.GUID = Guid.NewGuid();
                objUDFOptionDAL.Insert(objUDFOptionsDTO);
            }
        }

        public static void InsertUDFOptionMultiimport(UDFDTO objDTO, string UDFValue,long RoomID)
        {
            eTurns.DAL.UDFOptionDAL objUDFOptionDAL = new eTurns.DAL.UDFOptionDAL(SessionHelper.EnterPriseDBName);
            bool IsExist = objUDFOptionDAL.IsUdfOptionExistsInUDF(objDTO.ID, UDFValue);

            if (IsExist == false)
            {
                UDFOptionsDTO objUDFOptionsDTO = new UDFOptionsDTO();
                objUDFOptionsDTO.ID = 0;
                objUDFOptionsDTO.UDFID = objDTO.ID;
                objUDFOptionsDTO.UDFOption = UDFValue;
                objUDFOptionsDTO.Created = DateTimeUtility.DateTimeNow;
                objUDFOptionsDTO.CreatedBy = SessionHelper.UserID;
                objUDFOptionsDTO.Updated = DateTimeUtility.DateTimeNow;
                objUDFOptionsDTO.LastUpdatedBy = SessionHelper.UserID;
                objUDFOptionsDTO.CompanyID = SessionHelper.CompanyID;
                objUDFOptionsDTO.Room = RoomID;
                objUDFOptionsDTO.GUID = Guid.NewGuid();
                objUDFOptionDAL.Insert(objUDFOptionsDTO);
            }
        }
        public static string GetQBEmailToAddress()
        {
            string strToAddress = string.Empty;
            try
            {
                strToAddress = ConfigurationManager.AppSettings["QBLogEmailTo"];
            }
            catch (Exception)
            {
                return "";
            }
            return strToAddress;
        }

        public static string GetQBEmailCCAddress()
        {
            string strToAddress = string.Empty;
            try
            {
                strToAddress = ConfigurationManager.AppSettings["QBLogEmailCC"];
            }
            catch (Exception)
            {
                return "";
            }
            return strToAddress;
        }

        public static string RDLCBaseFilePath
        {
            get
            {
                string strRDLCBaseFilePath = @"D:\WebSites\eTurns4040\RDLC_Reports";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["RDLCBaseFilePath"])))
                {
                    strRDLCBaseFilePath = Convert.ToString(ConfigurationManager.AppSettings["RDLCBaseFilePath"]);
                }
                return strRDLCBaseFilePath;
            }
        }
        public static string ResourceBaseFilePath
        {
            get
            {
                string strResourceBaseFilePath = @"D:\WebSites\eTurns4040\Resources";
                if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["ResourceBaseFilePath"])))
                {
                    strResourceBaseFilePath = Convert.ToString(ConfigurationManager.AppSettings["ResourceBaseFilePath"]);
                }
                return strResourceBaseFilePath;
            }
        }
    }
    public class ResImportModule
    {
        private static string ResourceFileName = "ResImportModule";

        /// <summary>
        ///   Looks up a localized string similar to ({0}) Error! Record Not Saved..
        /// </summary>
        public static string Categories
        {
            get
            {
                return ResourceHelper.GetResourceValue("Categories", ResourceFileName);
            }
        }
        public static string CostUOM
        {
            get
            {
                return ResourceHelper.GetResourceValue("CostUOM", ResourceFileName);
            }
        }
        public static string Customers
        {
            get
            {
                return ResourceHelper.GetResourceValue("Customers", ResourceFileName);
            }
        }
        public static string GLAccounts
        {
            get
            {
                return ResourceHelper.GetResourceValue("GLAccounts", ResourceFileName);
            }
        }
        public static string InventoryClassification
        {
            get
            {
                return ResourceHelper.GetResourceValue("InventoryClassification", ResourceFileName);
            }
        }
        public static string AdjustmentCount
        {
            get
            {
                return ResourceHelper.GetResourceValue("AdjustmentCount", ResourceFileName);
            }
        }
        public static string ItemLocations
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemLocations", ResourceFileName);
            }
        }
        public static string Manufacturers
        {
            get
            {
                return ResourceHelper.GetResourceValue("Manufacturers", ResourceFileName);
            }
        }
        public static string MeasurementTerms
        {
            get
            {
                return ResourceHelper.GetResourceValue("MeasurementTerms", ResourceFileName);
            }
        }
        public static string ShipVias
        {
            get
            {
                return ResourceHelper.GetResourceValue("ShipVias", ResourceFileName);
            }
        }
        public static string Suppliers
        {
            get
            {
                return ResourceHelper.GetResourceValue("Suppliers", ResourceFileName);
            }
        }
        public static string Technicians
        {
            get
            {
                return ResourceHelper.GetResourceValue("Technicians", ResourceFileName);
            }
        }
        public static string ToolCategories
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolCategories", ResourceFileName);
            }
        }
        public static string ToolLocations
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolLocations", ResourceFileName);
            }
        }
        public static string Units
        {
            get
            {
                return ResourceHelper.GetResourceValue("Units", ResourceFileName);
            }
        }
        public static string Items
        {
            get
            {
                return ResourceHelper.GetResourceValue("Items", ResourceFileName);
            }
        }
        public static string EditItems
        {
            get
            {
                return ResourceHelper.GetResourceValue("EditItems", ResourceFileName);
            }
        }
        public static string Assets
        {
            get
            {
                return ResourceHelper.GetResourceValue("Assets", ResourceFileName);
            }
        }

        //AssetToolSchedulerMapping
        public static string AssetToolSchedulerMapping
        {
            get
            {
                return ResourceHelper.GetResourceValue("AssetToolSchedulerMapping", ResourceFileName);
            }
        }
        public static string Tools
        {
            get
            {
                return ResourceHelper.GetResourceValue("Tools", ResourceFileName);
            }
        }
        public static string QuickList
        {
            get
            {
                return ResourceHelper.GetResourceValue("QuickList", ResourceFileName);
            }
        }
        public static string BOMItems
        {
            get
            {
                return ResourceHelper.GetResourceValue("BOMItems", ResourceFileName);
            }
        }
        public static string CommonBOMItems
        {
            get
            {
                return ResourceHelper.GetResourceValue("CommonBOMItems", ResourceFileName);
            }
        }
        public static string Kits
        {
            get
            {
                return ResourceHelper.GetResourceValue("Kits", ResourceFileName);
            }
        }
        public static string ItemManufacturer
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemManufacturer", ResourceFileName);
            }
        }
        public static string ItemSupplier
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemSupplier", ResourceFileName);
            }
        }
        public static string BarcodeAssociations
        {
            get
            {
                return ResourceHelper.GetResourceValue("BarcodeAssociations", ResourceFileName);
            }
        }
        public static string UDF
        {
            get
            {
                return ResourceHelper.GetResourceValue("UDF", ResourceFileName);
            }
        }
        public static string ProjectSpends
        {
            get
            {
                return ResourceHelper.GetResourceValue("ProjectSpends", ResourceFileName);
            }
        }
        public static string ItemQuantityImportwithCost
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemQuantityImportwithCost", ResourceFileName);
            }
        }
        public static string ManualCount
        {
            get
            {
                return ResourceHelper.GetResourceValue("ManualCount", ResourceFileName);
            }
        }
        public static string WorkOrder
        {
            get
            {
                return ResourceHelper.GetResourceValue("WorkOrder", ResourceFileName);
            }
        }
        public static string PullImport
        {
            get
            {
                return ResourceHelper.GetResourceValue("PullImport", ResourceFileName);
            }
        }
        public static string PullImportWithLotSerial
        {
            get
            {
                return ResourceHelper.GetResourceValue("PullImportWithLotSerial", ResourceFileName);
            }
        }
        public static string ItemLocationChange
        {
            get
            {
                return ResourceHelper.GetResourceValue("ItemLocationChange", ResourceFileName);
            }
        }
        public static string PullWithSameQty
        {
            get
            {
                return ResourceHelper.GetResourceValue("PullWithSameQty", ResourceFileName);
            }
        }

        public static string AssetToolScheduler
        {
            get
            {
                return ResourceHelper.GetResourceValue("AssetToolScheduler", ResourceFileName);
            }
        }
        public static string PastMaintenanceDue
        {
            get
            {
                return ResourceHelper.GetResourceValue("PastMaintenanceDue", ResourceFileName);
            }
        }

        public static string ToolCheckInCheckOut
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolCheckInCheckOut", ResourceFileName);
            }
        }

        public static string ToolAdjustmentCount
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolAdjustmentCount", ResourceFileName);
            }
        }

        /// <summary>
        /// Tool Certification Images
        /// </summary>
        public static string ToolCertificationImages
        {
            get
            {
                return ResourceHelper.GetResourceValue("ToolCertificationImages", ResourceFileName);
            }
        }

        public static string Order
        {
            get
            {
                return ResourceHelper.GetResourceValue("Order", ResourceFileName);
            }
        }

        public static string MoveMaterial
        {
            get
            {
                return ResourceHelper.GetResourceValue("MoveMaterial", ResourceFileName);
            }
        }
        public static string EnterpriseQuickList
        {
            get
            {
                return ResourceHelper.GetResourceValue("EnterpriseQuickList", ResourceFileName);
            }
        }
        public static string Requisition
        {
            get
            {
                return ResourceHelper.GetResourceValue("Requisition", ResourceFileName);
            }
        }
        public static string Quote
        {
            get
            {
                return ResourceHelper.GetResourceValue("Quote", ResourceFileName);
            }
        }

        public static string ImportReceiptAddNewReceipt
        {
            get
            {
                return ResourceHelper.GetResourceValue("ImportReceiptAddNewReceipt", ResourceFileName);
            }
        }

        public static string SupplierCatalog
        {
            get
            {
                return ResourceHelper.GetResourceValue("SupplierCatalog", ResourceFileName);
            }
        }
        public static string Returns
        {
            get
            {
                return ResourceHelper.GetResourceValue("Returns", ResourceFileName);
            }
        }
        public static string CommonBOMToItem
        {
            get
            {
                return ResourceHelper.GetResourceValue("CommonBOMToItem", ResourceFileName);
            }
        }
    }

    public enum ExportType
    {
        csv,
        xls
    }
}