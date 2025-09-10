using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;

namespace ABOrderConsole.Helper
{
    public class ABEnterpriseSetup
    {
        public bool ABECRSetupProcess()
        {
            try
            {
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(GeneralHelper.eTurnsLoggingDBName);
                List<ZohoReponseDTO> lstPendingOps = objABSetUpDAL.FetchZohoReponseForProcessing();
                if (lstPendingOps != null && lstPendingOps.Count > 0)
                {
                    lstPendingOps = lstPendingOps.Where(x => x.isValid == true).OrderBy(t => t.ID).ToList();
                    foreach (var objECRDetails in lstPendingOps)
                    {
                        ABECRCreatingProcess(objECRDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRSetupProcess", ex, GeneralHelper.DoSendLogsInMail);
                return false;
            }

            return true;
        }
        public bool ABECRCreatingProcess(ZohoReponseDTO objECRDetails)
        {
            ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(GeneralHelper.eTurnsLoggingDBName);
            try
            {
                if (objECRDetails != null && objECRDetails.subscriptionInfo != null)
                {
                    objECRDetails.subscriptionInfo.event_type = (objECRDetails.subscriptionInfo.event_type ?? String.Empty).ToLower();
                    switch (objECRDetails.subscriptionInfo.event_type)
                    {
                        case "subscription_created":
                            if (objECRDetails != null && objECRDetails.subscriptionInfo != null
                                && objECRDetails.subscription.customer != null
                                && !string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.company_name)
                                //&& (!string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.first_name)
                                //    || !string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.last_name))
                                && !string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.email)
                                )
                            {
                                #region Create Enterprise

                                EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                                EnterpriseDTO enterpriseDTO = new EnterpriseDTO();
                                Int64 CreatedByUserID = 0;
                                try
                                {
                                    enterpriseDTO.CreatedBy = GeneralHelper.EnterpiseCreatedBy;
                                    enterpriseDTO.IsActive = true;
                                    // 1. Check enterprise name if duplicate append _1
                                    string strEnterPriseName = CommonFunctions.CheckDuplicateEnterprise(objECRDetails.subscription.customer.company_name, 0);                                    
                                    enterpriseDTO.Name = strEnterPriseName;
                                    if (!string.IsNullOrWhiteSpace(strEnterPriseName))
                                    {
                                        // 2. Check user name if duplicate append _1
                                        string strUserName = strEnterPriseName + GeneralHelper.EnterpiseUserNamePostFix;
                                        enterpriseDTO.UserName = strUserName;
                                        enterpriseDTO.GUID = Guid.NewGuid();
                                        if (objECRDetails.subscription.contactpersons.Count > 0
                                            && !string.IsNullOrWhiteSpace(objECRDetails.subscription.contactpersons[0].phone))
                                        {
                                            enterpriseDTO.ContactPhone = objECRDetails.subscription.contactpersons[0].phone;
                                        }
                                        if (objECRDetails.subscription.contactpersons.Count > 0
                                            && !string.IsNullOrWhiteSpace(objECRDetails.subscription.contactpersons[0].email))
                                        {
                                            enterpriseDTO.ContactEmail = objECRDetails.subscription.contactpersons[0].email;
                                        }
                                        enterpriseDTO.EnterpriseUserEmail = objECRDetails.subscription.customer.email;
                                        if (objECRDetails.subscription.customer.billing_address != null
                                            && !string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.billing_address.country)
                                            )
                                        {
                                            enterpriseDTO.Country = objECRDetails.subscription.customer.billing_address.country.Replace(".", "");
                                        }
                                        else if (objECRDetails.subscription.customer.shipping_address != null
                                            && !string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.shipping_address.country)
                                            )
                                        {
                                            enterpriseDTO.Country = objECRDetails.subscription.customer.shipping_address.country.Replace(".", "");
                                        }
                                        else
                                        {
                                            enterpriseDTO.Country = "USA";
                                        }
                                        long EnterPriseID = 0;
                                        string PasswordWithoutHash = enterpriseDTO.EnterpriseUserPassword;
                                        enterpriseDTO.EnterpriseUserPassword =CommonFunctions.getSHA15Hash(CommonFunctions.RandomString(8));
                                        enterpriseDTO.EnterPriseDomainURL = GeneralHelper.DomainName;
                                        enterpriseDTO = objEnterpriseMasterDAL.Insert(enterpriseDTO);
                                        if(enterpriseDTO != null && enterpriseDTO.ID > 0)
                                        {
                                            UserMasterDTO objEnterpriseUser = new eTurnsMaster.DAL.UserMasterDAL().GetUserByNameAndEnterPriseIDPlain("Enterprise System User", enterpriseDTO.ID);
                                            if(objEnterpriseUser != null && objEnterpriseUser.ID > 0)
                                            {
                                                CreatedByUserID = objEnterpriseUser.ID;
                                            }
                                        }
                                        EnterPriseID = enterpriseDTO.ID;

                                        #region Insert country if not exist
                                        if (enterpriseDTO != null
                                            && enterpriseDTO.ID > 0
                                            && !string.IsNullOrWhiteSpace(enterpriseDTO.Country))
                                        {
                                            try
                                            {
                                                CountryMasterDAL objCountryMasterDAL = new CountryMasterDAL(enterpriseDTO.EnterpriseDBName);
                                                CountryMasterDTO objCountryMasterDTO = new CountryMasterDTO();
                                                objCountryMasterDTO.CountryName = enterpriseDTO.Country;
                                                objCountryMasterDTO.CreatedBy = CreatedByUserID;
                                                bool isCountryUSA = false;
                                                if (enterpriseDTO.Country.ToLower() == "usa")
                                                {
                                                    objCountryMasterDTO.CountryCode = "USA";
                                                    objCountryMasterDTO.PhoneNunberFormat = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
                                                    isCountryUSA = true;
                                                }
                                                objCountryMasterDAL.ABInsertCountryForNewEnt(objCountryMasterDTO, isCountryUSA);
                                            }
                                            catch (Exception ex)
                                            {
                                                CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating ent-> country EnterpriseName: " + enterpriseDTO.Name + " Country Name: " + enterpriseDTO.Country, ex, GeneralHelper.DoSendLogsInMail);
                                                objABSetUpDAL.SetABECRCreateCompletedWithError(objECRDetails.ID, (Convert.ToString(ex) ?? "Exception not found"), objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                            }
                                        }
                                        #endregion                                        

                                        if (EnterPriseID > 0)
                                        {
                                            try
                                            {
                                                if (enterpriseDTO.ID > 0)
                                                {
                                                    objECRDetails.EnterpriseID = enterpriseDTO.ID;
                                                    EnterpriseDAL objEnterpriseDAL = new EnterpriseDAL(enterpriseDTO.EnterpriseDBName);
                                                    EnterpriseDTO objEnterPriseDTO = new EnterpriseDTO();
                                                    objEnterPriseDTO = objEnterpriseDAL.Insert(enterpriseDTO);
                                                    if(objEnterPriseDTO != null && objEnterPriseDTO.ID > 0)
                                                    {
                                                        ResourceDAL objEnterprise = new ResourceDAL(enterpriseDTO.EnterpriseDBName);
                                                        List<ResourceLanguageDTO> lstEnterpriseResourceDTO = objEnterprise.GetResourceLanguageData().ToList();
                                                        var AllEnterpriseID = from f in lstEnterpriseResourceDTO select f.Culture.ToString();                                                        
                                                        AllEnterpriseID = from item in AllEnterpriseID
                                                                       where item != "en-US"
                                                                       select item;
                                                        foreach (var rmitem in AllEnterpriseID) // Remove Operation
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(Convert.ToString(rmitem)))
                                                            {
                                                                ResourceLanguageDTO RLDTO = lstEnterpriseResourceDTO.Where(x => x.Culture == rmitem).FirstOrDefault();
                                                                objEnterprise.DeleteResourceLanguageData(RLDTO, 0);
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating ent EnterpriseName: " + enterpriseDTO.Name, ex, GeneralHelper.DoSendLogsInMail);
                                                objABSetUpDAL.SetABECRCreateCompletedWithError(objECRDetails.ID, (Convert.ToString(ex) ?? "Exception not found"), objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                            }
                                        }
                                        else
                                        {
                                            CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating ent EnterpriseName: " + enterpriseDTO.Name, null, GeneralHelper.DoSendLogsInMail);
                                            objABSetUpDAL.UpdateABECRCreateWithException(objECRDetails.ID, "Exception not found", objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                            return false;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating ent EnterpriseName: " + enterpriseDTO.Name, ex, GeneralHelper.DoSendLogsInMail);
                                    objABSetUpDAL.UpdateABECRCreateWithException(objECRDetails.ID, (Convert.ToString(ex) ?? "Exception not found"), objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                    return false;
                                }
                                #endregion

                                if (enterpriseDTO.ID > 0)
                                {
                                    #region Create Company
                                    try
                                    {
                                        CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(enterpriseDTO.EnterpriseDBName);
                                        CompanyMasterDTO objCompanyMasterDTO = new CompanyMasterDTO();
                                        objCompanyMasterDTO.Name = objECRDetails.subscription.customer.company_name;
                                        if (objECRDetails.subscription.customer.billing_address != null
                                                && !string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.billing_address.country)
                                                )
                                        {
                                            objCompanyMasterDTO.Country = objECRDetails.subscription.customer.billing_address.country.Replace(".", "");
                                        }
                                        else if (objECRDetails.subscription.customer.shipping_address != null
                                            && !string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.shipping_address.country)
                                            )
                                        {
                                            objCompanyMasterDTO.Country = objECRDetails.subscription.customer.shipping_address.country.Replace(".", "");
                                        }
                                        else
                                        {
                                            objCompanyMasterDTO.Country = "USA";
                                        }
                                        if (objECRDetails.subscription.contactpersons.Count > 0
                                                && !string.IsNullOrWhiteSpace(objECRDetails.subscription.contactpersons[0].phone))
                                        {
                                            objCompanyMasterDTO.ContactPhone = objECRDetails.subscription.contactpersons[0].phone;
                                        }
                                        if (objECRDetails.subscription.contactpersons.Count > 0
                                            && !string.IsNullOrWhiteSpace(objECRDetails.subscription.contactpersons[0].email))
                                        {
                                            objCompanyMasterDTO.ContactEmail = objECRDetails.subscription.contactpersons[0].email;
                                        }
                                        objCompanyMasterDTO.CreatedBy = CreatedByUserID;
                                        objCompanyMasterDTO.LastUpdatedBy = CreatedByUserID;
                                        objCompanyMasterDTO.GUID = Guid.NewGuid();
                                        objCompanyMasterDTO.IsActive = true;
                                        long CompanyID = objCompanyMasterDAL.Insert(objCompanyMasterDTO);
                                        if (CompanyID > 0)
                                        {
                                            #region WI-8314 default udf at company level by default.
                                            eTurns.DAL.UDFDAL uDFDAL = new eTurns.DAL.UDFDAL(enterpriseDTO.EnterpriseDBName);
                                            uDFDAL.InsertDefaultUDFsByRoom(CreatedByUserID, CompanyID);
                                            #endregion
                                            objECRDetails.CompanyID = CompanyID;
                                            try
                                            {                                                
                                                string id = Convert.ToString(CompanyID);
                                                string _baseFilePath = GeneralHelper.BaseFilePath;
                                                string _cmpIdForSignature = _baseFilePath + "Uploads\\WorkOrderSignature\\" + id;
                                                if (!System.IO.Directory.Exists(_cmpIdForSignature))
                                                {
                                                    System.IO.Directory.CreateDirectory(_cmpIdForSignature);
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating cmp for WorkOrderSignature EnterpriseName: " + enterpriseDTO.Name + " CompanyName : " + objECRDetails.subscription.customer.company_name, null, GeneralHelper.DoSendLogsInMail);
                                            }
                                        }
                                        else
                                        {
                                            CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating cmp EnterpriseName: " + enterpriseDTO.Name + " CompanyName : " + objECRDetails.subscription.customer.company_name, null, GeneralHelper.DoSendLogsInMail);
                                            objABSetUpDAL.UpdateABECRCreateWithException(objECRDetails.ID, "Exception not found", objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                            return false;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating cmp EnterpriseName: " + enterpriseDTO.Name + " CompanyName : " + objECRDetails.subscription.customer.company_name, ex, GeneralHelper.DoSendLogsInMail);
                                        objABSetUpDAL.UpdateABECRCreateWithException(objECRDetails.ID, (Convert.ToString(ex) ?? "Exception not found"), objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);

                                        return false;
                                    }
                                    #endregion

                                    #region Create  Room

                                    if (objECRDetails.CompanyID > 0)
                                    {
                                        try
                                        {
                                            RoomDAL objRoomDAL = new RoomDAL(enterpriseDTO.EnterpriseDBName);
                                            RoomDTO objRoomDTO = new RoomDTO();
                                            objRoomDTO.CompanyID = objECRDetails.CompanyID;
                                            objRoomDTO.EnterpriseId = objECRDetails.EnterpriseID.GetValueOrDefault(0);
                                            objRoomDTO.RoomName = objECRDetails.subscription.customer.company_name;
                                            objRoomDTO.IsActive = true;
                                            objRoomDTO.AllowABIntegration = true;
                                            objRoomDTO.IsRoomActive = true;
                                            objRoomDTO.SuggestedOrder = true;
                                            objRoomDTO.IsAllowRequisitionDuplicate = false;
                                            objRoomDTO.IsAllowOrderDuplicate = false;
                                            objRoomDTO.IsAllowWorkOrdersDuplicate = false;
                                            objRoomDTO.AllowInsertingItemOnScan = false;
                                            objRoomDTO.IsTax2onTax1 = false;
                                            objRoomDTO.WarnUserOnAssigningNonDefaultBin = false;
                                            objRoomDTO.CreatedBy = CreatedByUserID;
                                            objRoomDTO.ContactName = ((objECRDetails.subscription.customer.first_name ?? string.Empty) + " " + (objECRDetails.subscription.customer.last_name ?? string.Empty));
                                            if (objECRDetails.subscription.contactpersons.Count > 0
                                            && !string.IsNullOrWhiteSpace(objECRDetails.subscription.contactpersons[0].phone))
                                            {
                                                objRoomDTO.PhoneNo = objECRDetails.subscription.contactpersons[0].phone;
                                            }
                                            if (objECRDetails.subscription.contactpersons.Count > 0
                                                && !string.IsNullOrWhiteSpace(objECRDetails.subscription.contactpersons[0].email))
                                            {
                                                objRoomDTO.Email = objECRDetails.subscription.contactpersons[0].email;
                                            }
                                            enterpriseDTO.EnterpriseUserEmail = objECRDetails.subscription.customer.email;
                                            if (objECRDetails.subscription.customer.billing_address != null
                                                && !string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.billing_address.country)
                                                )
                                            {
                                                objRoomDTO.Country = objECRDetails.subscription.customer.billing_address.country.Replace(".", "");
                                            }
                                            else if (objECRDetails.subscription.customer.shipping_address != null
                                                && !string.IsNullOrWhiteSpace(objECRDetails.subscription.customer.shipping_address.country)
                                                )
                                            {
                                                objRoomDTO.Country = objECRDetails.subscription.customer.shipping_address.country.Replace(".", "");
                                            }
                                            else
                                            {
                                                objRoomDTO.Country = "USA";
                                            }

                                            if(!string.IsNullOrWhiteSpace(objECRDetails.subscription.product_name))
                                            {
                                                BillingRoomTypeMasterDAL objBillingRoomTypeMasterDAL = new BillingRoomTypeMasterDAL();
                                                BillingRoomTypeMasterDTO objBillingRoomType = new BillingRoomTypeMasterDTO();
                                                objBillingRoomType = objBillingRoomTypeMasterDAL.GetBillingRoomTypeMasterByName(objECRDetails.subscription.product_name, enterpriseDTO.ID);
                                                if (objBillingRoomType != null)
                                                {
                                                    objRoomDTO.BillingRoomType = objBillingRoomType.ID;
                                                }
                                            }
                                            objRoomDTO.CountAutoSequence = 5; // Date + incrementing #
                                            objRoomDTO.POAutoSequence = 5; // Date + incrementing #
                                            objRoomDTO.PullPurchaseNumberType = 5; // Date + incrementing #
                                            objRoomDTO.ReqAutoSequence = 5; // Date + incrementing #
                                            objRoomDTO.StagingAutoSequence = 5; // Date + incrementing #
                                            objRoomDTO.TransferAutoSequence = 5; // Date + incrementing #
                                            objRoomDTO.WorkOrderAutoSequence = 5; // Date + incrementing #
                                            objRoomDTO.ToolCountAutoSequence = 5; // Date + incrementing #
                                            objRoomDTO.TAOAutoSequence = 5; // Date + incrementing #
                                            objRoomDTO.MethodOfValuingInventory = "4";
                                            objRoomDTO.DefaultCountType = "A";
                                            objRoomDTO.InventoryConsuptionMethod = "Fifo";
                                            objRoomDTO.AttachingWOWithRequisition = (int)AttachingWOWithRequisition.New;
                                            objRoomDTO.PreventMaxOrderQty = 1;
                                            objRoomDTO.IsWOSignatureRequired = false;
                                            objRoomDTO.IsIgnoreCreditRule = false;
                                            objRoomDTO.SlowMovingValue = 2;
                                            objRoomDTO.FastMovingValue = 10;
                                            objRoomDTO.BaseOfInventory = 2;
                                            objRoomDTO.ReplenishmentType = "3";
                                            objRoomDTO.ForceSupplierFilter = false;
                                            objRoomDTO.SuggestedReturn = false;
                                            objRoomDTO.AllowPullBeyondAvailableQty = true;
                                            objRoomDTO.PullRejectionType = 1;   
                                            List<long> lstModules = new List<long> { (long)ModuleList.Requisitions, (long)ModuleList.WorkOrders, (long)ModuleList.Orders };
                                            objRoomDTO.lstRoomModleSettings = objRoomDAL.GetDummyRoomModuleSettingsDTO(lstModules, 0, 0);
                                            objRoomDTO.GUID = Guid.NewGuid();
                                            objRoomDTO.DefaultBinName = enterpriseDTO.Name + GeneralHelper.DefaultBinNamePostFix;
                                            objRoomDTO.DefaultSupplierName = GeneralHelper.DefaultSupplierName;
                                            long RoomID = objRoomDAL.Insert(objRoomDTO);
                                            if(RoomID > 0)
                                            {
                                                objECRDetails.RoomID = RoomID;
                                                try
                                                {
                                                    DashboardParameterDTO objConfig = new DashboardParameterDTO();
                                                    EnterpriseMasterDAL objDAL = new EnterpriseMasterDAL();
                                                    objConfig = objDAL.GetDashboardParamByRoomAndCompanyPlain(0, 0);                                                    
                                                    if (objConfig != null)
                                                    {
                                                        objConfig.GraphFromYear = DateTime.UtcNow.Year;
                                                        objConfig.GraphFromMonth = DateTime.UtcNow.Month;

                                                        objConfig.GraphToYear = DateTime.UtcNow.Year;
                                                        objConfig.GraphToMonth = DateTime.UtcNow.Month + 6;
                                                        if (objConfig.GraphToMonth > 12)
                                                        {
                                                            objConfig.GraphToMonth = objConfig.GraphToMonth - 12;
                                                            objConfig.GraphToYear = objConfig.GraphToYear + 1;
                                                        }
                                                        DashboardParameterDTO objDashboard = AssignValueDashboard(objConfig);
                                                        objDashboard.ID = 0;
                                                        objDashboard.EnterpriseId = objECRDetails.EnterpriseID.GetValueOrDefault(0);
                                                        objDashboard.CompanyId = objECRDetails.CompanyID.GetValueOrDefault(0);
                                                        objDashboard.RoomId = RoomID;
                                                        objDAL.InsertDashboardParameter(objDashboard);
                                                        DashboardDAL objDashboardDAL = new DashboardDAL(enterpriseDTO.EnterpriseDBName);
                                                        objDashboard.ID = 0;
                                                        objDashboardDAL.SaveDashboardParameters(objDashboard, CreatedByUserID);
                                                    }
                                                    else
                                                    {
                                                        DashboardParameterDTO objDashboard = new DashboardParameterDTO();
                                                        objDashboard.ID = 0;
                                                        objDashboard.EnterpriseId = objECRDetails.EnterpriseID.GetValueOrDefault(0);
                                                        objDashboard.CompanyId = objECRDetails.CompanyID.GetValueOrDefault(0);
                                                        objDashboard.RoomId = RoomID;
                                                        objDashboard.CreatedOn = DateTime.UtcNow;
                                                        objDashboard.UpdatedOn = DateTime.UtcNow;
                                                        objDashboard.GraphFromYear = DateTime.UtcNow.Year;
                                                        objDashboard.GraphFromMonth = DateTime.UtcNow.Month;

                                                        objDashboard.GraphToYear = DateTime.UtcNow.Year;
                                                        objDashboard.GraphToMonth = DateTime.UtcNow.Month + 6;
                                                        if (objDashboard.GraphToMonth > 12)
                                                        {
                                                            objDashboard.GraphToMonth = objConfig.GraphToMonth - 12;
                                                            objDashboard.GraphToYear = objDashboard.GraphToYear + 1;
                                                        }
                                                        objDAL.InsertDashboardParameter(objDashboard);
                                                        DashboardDAL objDashboardDAL = new DashboardDAL(enterpriseDTO.EnterpriseDBName);
                                                        objDashboard.ID = 0;
                                                        objDashboardDAL.SaveDashboardParameters(objDashboard, CreatedByUserID);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating Rmp-> dashboard Param EnterpriseName: " + enterpriseDTO.Name + " RoomName : " + objECRDetails.subscription.customer.company_name, ex, GeneralHelper.DoSendLogsInMail);                                                    
                                                }
                                            }
                                            else
                                            {
                                                CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating Rmp EnterpriseName: " + enterpriseDTO.Name + " RoomName : " + objECRDetails.subscription.customer.company_name, null, GeneralHelper.DoSendLogsInMail);
                                                objABSetUpDAL.UpdateABECRCreateWithException(objECRDetails.ID, "Exception not found", objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                                return false;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess while creating Rmp EnterpriseName: " + enterpriseDTO.Name + " RoomName : " + objECRDetails.subscription.customer.company_name, ex, GeneralHelper.DoSendLogsInMail);
                                            objABSetUpDAL.UpdateABECRCreateWithException(objECRDetails.ID, (Convert.ToString(ex) ?? "Exception not found"), objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);

                                            return false;
                                        }
                                    }

                                    #endregion

                                    if (objECRDetails.EnterpriseID.GetValueOrDefault(0) > 0
                                        && objECRDetails.CompanyID.GetValueOrDefault(0) > 0
                                        && objECRDetails.RoomID.GetValueOrDefault(0) > 0)
                                    {
                                        try
                                        {
                                            #region Create Default ItemMaster UDF
                                            try
                                            {
                                                eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(enterpriseDTO.EnterpriseDBName);
                                                UDFDTO objUDFDTO = new UDFDTO();
                                                objUDFDTO.CompanyID = objECRDetails.CompanyID.GetValueOrDefault(0);
                                                objUDFDTO.Room = objECRDetails.RoomID.GetValueOrDefault(0);
                                                objUDFDTO.UDFTableName = "ItemMaster";
                                                objUDFDTO.UDFColumnName = "UDF1";
                                                objUDFDTO.UDFControlType = "Textbox";
                                                objUDFDTO.UDFOptionsCSV = null;
                                                objUDFDTO.UDFDefaultValue = null;
                                                objUDFDTO.UDFIsRequired = false;
                                                objUDFDTO.UDFIsSearchable = true;
                                                objUDFDTO.CreatedBy = CreatedByUserID;
                                                objUDFDTO.LastUpdatedBy = CreatedByUserID;
                                                objUDFDTO.IsDeleted = false;
                                                objUDFDTO.IsEncryption = false;
                                                objUDFDTO.UDFMaxLength = 200;
                                                objUDFDAL.Insert(objUDFDTO);
                                            }
                                            catch (Exception ex)
                                            {
                                                CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess  not success for ItemMaster UDF creation", ex, GeneralHelper.DoSendLogsInMail);
                                                objABSetUpDAL.SetABECRCreateCompletedWithError(objECRDetails.ID, (Convert.ToString(ex) ?? "Exception not found"), objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                            }
                                            #endregion

											//#region Create entry for Default barcode
           //                                 try
           //                                 {
           //                                     LabelModuleTemplateDetailDAL objLabelModuleTemplateDetail = new LabelModuleTemplateDetailDAL(enterpriseDTO.EnterpriseDBName);
           //                                     objLabelModuleTemplateDetail.ABSetAsDefaultTemplateForModule(objECRDetails.CompanyID.GetValueOrDefault(0), objECRDetails.RoomID.GetValueOrDefault(0), CreatedByUserID, GeneralHelper.DefaultBarcodeTemplateName);
           //                                 }
           //                                 catch (Exception ex)
           //                                 {
           //                                     CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess  not success for default template creation", ex, GeneralHelper.DoSendLogsInMail);
           //                                     objABSetUpDAL.SetABECRCreateCompletedWithError(objECRDetails.ID, "Error in process for ABECRCreatingProcess  not success for default template creation", objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
           //                                 }
           //                                 #endregion                                            

											objABSetUpDAL.SetABECRCreateCompletedWithSuccess(objECRDetails.ID, objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);

                                            //CreateRoleAndUserFromZohoResponse(objECRDetails, enterpriseDTO);
                                            //if (objECRDetails.RoleId.GetValueOrDefault(0) > 0 && objECRDetails.UserId.GetValueOrDefault(0) > 0)
                                            //{
                                            //    objABSetUpDAL.SetABECRCreateCompletedWithSuccess(objECRDetails.ID, objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                            //}
                                            //else
                                            //{
                                            //    CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess not success for Role and User Creation", null, GeneralHelper.DoSendLogsInMail);
                                            //    objABSetUpDAL.SetABECRCreateCompletedWithError(objECRDetails.ID, "Error in process for ABECRCreatingProcess not success for Role and User Creation", objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                            //}
                                        }
                                        catch (Exception ex)
                                        {
                                            CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess  not success for Role and User Creation", ex, GeneralHelper.DoSendLogsInMail);
                                            objABSetUpDAL.SetABECRCreateCompletedWithError(objECRDetails.ID, (Convert.ToString(ex) ?? "Exception not found"), objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);                                            
                                        }
                                    }
                                    else
                                    {
                                        CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess not success", null, GeneralHelper.DoSendLogsInMail);
                                        objABSetUpDAL.UpdateABECRCreateWithException(objECRDetails.ID, "Error in process for ABECRCreatingProcess not success", objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("Error in process for ABECRCreatingProcess", ex, GeneralHelper.DoSendLogsInMail);
                objABSetUpDAL.UpdateABECRCreateWithException(objECRDetails.ID, (Convert.ToString(ex) ?? "Exception not found"), objECRDetails.EnterpriseID, objECRDetails.CompanyID, objECRDetails.RoomID);
                return false;
            }
        }

        #region Private Method        
        private DashboardParameterDTO AssignValueDashboard(DashboardParameterDTO objDTO)
        {
            DashboardParameterDTO obj = new DashboardParameterDTO();
            obj.ID = 0;
            obj.RoomId = objDTO.RoomId;
            obj.CompanyId = objDTO.CompanyId;
            obj.CreatedOn = objDTO.CreatedOn;
            obj.CreatedBy = objDTO.CreatedBy;
            obj.UpdatedOn = objDTO.UpdatedOn;
            obj.UpdatedBy = objDTO.UpdatedBy;
            obj.TurnsMeasureMethod = objDTO.TurnsMeasureMethod;
            obj.TurnsMonthsOfUsageToSample = objDTO.TurnsMonthsOfUsageToSample;
            obj.TurnsDaysOfUsageToSample = objDTO.TurnsDaysOfUsageToSample;
            obj.AUDayOfUsageToSample = objDTO.AUDayOfUsageToSample;
            obj.AUMeasureMethod = objDTO.AUMeasureMethod;
            obj.AUDaysOfDailyUsage = objDTO.AUDaysOfDailyUsage;
            obj.MinMaxMeasureMethod = objDTO.MinMaxMeasureMethod;
            obj.MinMaxDayOfUsageToSample = objDTO.MinMaxDayOfUsageToSample;
            obj.MinMaxDayOfAverage = objDTO.MinMaxDayOfAverage;
            obj.MinMaxMinNumberOfTimesMax = objDTO.MinMaxMinNumberOfTimesMax;
            obj.MinMaxOptValue1 = objDTO.MinMaxOptValue1;
            obj.MinMaxOptValue2 = objDTO.MinMaxOptValue2;
            obj.GraphFromMonth = objDTO.GraphFromMonth;
            obj.GraphToMonth = objDTO.GraphToMonth;
            obj.GraphFromYear = objDTO.GraphFromYear;
            obj.GraphToYear = objDTO.GraphToYear;
            obj.IsTrendingEnabled = objDTO.IsTrendingEnabled;
            obj.PieChartmetricOn = objDTO.PieChartmetricOn;
            obj.TurnsCalculatedStockRoomTurn = objDTO.TurnsCalculatedStockRoomTurn;
            obj.AUCalculatedDailyUsageOverSample = objDTO.AUCalculatedDailyUsageOverSample;
            obj.MinMaxCalculatedDailyUsageOverSample = objDTO.MinMaxCalculatedDailyUsageOverSample;
            obj.MinMaxCalcAvgPullByDay = objDTO.MinMaxCalcAvgPullByDay;
            obj.MinMaxCalcualtedMax = objDTO.MinMaxCalcualtedMax;
            obj.AutoClassification = objDTO.AutoClassification;
            obj.MonthlyAverageUsage = objDTO.MonthlyAverageUsage;
            obj.AnnualCarryingCostPercent = objDTO.AnnualCarryingCostPercent;
            obj.LargestAnnualCashSavings = objDTO.LargestAnnualCashSavings;
            return obj;
        }

        #endregion
    }
}
