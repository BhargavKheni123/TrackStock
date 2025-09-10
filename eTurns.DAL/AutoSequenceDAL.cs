using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eTurns.DAL
{
    public class AutoSequenceDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public AutoSequenceDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];

        //public AutoSequenceDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public AutoOrderNumberGenerate GetNextOrderNumber(Int64 RoomID, Int64 CompanyID, Int64 SupplierID, Int64 EnterpriseID,AutoOrderNumberGenerate LastUsedTemp = null, bool IsPreviousSupplier = false)
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            Int64 iNextNo = 0;
            RoomDTO obj = null;
            RoomDAL objRoomDAL = null;
            CommonDAL objDAL = null;
            SupplierMasterDTO objSupplierDTO = null;
            SupplierMasterDAL objSupplierDAL = null;
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;
            try
            {
                #region DateFormate From CompnayConfig

                string strDateFormat = "MM/dd/yyyy";
                string returnString = string.Empty;
                //objCompConfDAL = new CompanyConfigDAL(base.DataBaseName);
                //objCompConfDTO = objCompConfDAL.GetRecord(CompanyID);

                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

                if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
                {
                    strDateFormat = "yyyy/MM/dd";
                }

                objRegionSettingDAL = null;
                #endregion

                #region [Regional Settings]
                //objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
                if (objeTurnsRegionInfo != null)
                {
                    strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

                }
                #endregion

                objAutoNumber = new AutoOrderNumberGenerate();
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else
                {
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterpriseID, CompanyID);
                string MsgPOSequanceNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgPOSequanceNotDefineValidation", ResourceFileCommon,EnterpriseID,CompanyID,RoomID, "ResOrder",currentCulture);
                #region From Supplier
                if (SupplierID > 0)
                {
                    objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(SupplierID);
                   
                    string MsgFixedOrderNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFixedOrderNumberValidation", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                    string MsgBlanketNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgBlanketNotDefineValidation", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                    
                    if (objSupplierDTO != null && objSupplierDTO.POAutoSequence.HasValue)
                    {
                        objAutoNumber.OrderGeneratedFrom = "Supplier";
                        objAutoNumber.OrderNumberFormateType = objSupplierDTO.POAutoSequence.GetValueOrDefault(0);
                        if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberSupplier > 0 && IsPreviousSupplier)
                            objSupplierDTO.NextOrderNo = (LastUsedTemp.LastUsedTempIncrementNumberSupplier).ToString();


                        switch (objSupplierDTO.POAutoSequence.GetValueOrDefault(0))
                        {

                            case 0: //Blank
                                objAutoNumber.IsBlank = true;
                                break;
                            case 1: //Fixed

                                if (!string.IsNullOrEmpty(objSupplierDTO.NextOrderNo))
                                {
                                    objAutoNumber.OrderNumber = objSupplierDTO.NextOrderNo;
                                    objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                }
                                else
                                    objAutoNumber.ErrorDescription = MsgFixedOrderNumberValidation;
                                break;
                            case 2: //Blanket Order #
                                objAutoNumber.IsBlanketPO = true;
                                objAutoNumber.BlanketPOs = new SupplierBlanketPODetailsDAL(base.DataBaseName).GetBlanketPOBySupplierIDPlain(SupplierID, RoomID, CompanyID).ToList();
                                if (objAutoNumber.BlanketPOs == null || objAutoNumber.BlanketPOs.Count <= 0)
                                {
                                    objAutoNumber.ErrorDescription = MsgBlanketNotDefineValidation;
                                }
                                else if (objAutoNumber.BlanketPOs.Count > 0)
                                {
                                    objAutoNumber.OrderNumber = objAutoNumber.BlanketPOs[0].BlanketPO;
                                }
                                if (objAutoNumber.BlanketPOs == null || objAutoNumber.BlanketPOs.Count <= 0)
                                    objAutoNumber.BlanketPOs.Insert(0, null);

                                break;
                            case 3: //Increamenting by Order#
                                //iNextNo = objSupplierDTO.NextOrderNo.GetValueOrDefault(0) + 1;
                                Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = Convert.ToString(iNextNo);
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                break;
                            case 4: //Increamenting by Day
                                //objAutoNumber.OrderNumber = System.DateTime.Now.DayOfYear.ToString();
                                //iNextNo = objSupplierDTO.NextOrderNo.GetValueOrDefault(0);
                                Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                                OrderMasterDAL objOrdDAL = new OrderMasterDAL(base.DataBaseName);
                                OrderMasterDTO objOrdDTO = objOrdDAL.GetLatestOrderByRoomIdPlain(RoomID, CompanyID);

                                if (objOrdDTO == null || objOrdDTO.Created.GetValueOrDefault(DateTime.MinValue).DayOfYear < DateTime.Now.DayOfYear)
                                {
                                    iNextNo = iNextNo + 1;
                                }

                                objAutoNumber.OrderNumber = iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                break;
                            case 5: //Date + Incrementing#
                                Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0');
                                break;
                            case 6: //Date
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat);
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                                break;
                            case 7: //Fixed + Incrementing #
                                Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = objSupplierDTO.POAutoNrFixedValue + "-" + iNextNo.ToString();
                                objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = objSupplierDTO.POAutoNrFixedValue + iNextNo.ToString().PadLeft(3, '0');
                                break;
                            case 8: //Date  + Incrementing #+Fixed

                                //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                }
                                else
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                                }


                                //string OrderNumberDateFormat = Settinfile.Element("OrderNumberDateFormat") != null ? Settinfile.Element("OrderNumberDateFormat").Value : "yyMMdd";
                                string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";

                                Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(OrderNumberDateFormat) + "-" + objSupplierDTO.POAutoNrFixedValue + "-" + iNextNo.ToString();
                                objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + objSupplierDTO.POAutoNrFixedValue + "-" + "000" + iNextNo.ToString().PadLeft(3, '0');
                                break;

                        }
                        if (objAutoNumber != null)
                        {
                            if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                            {
                                objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                            }
                        }
                        return objAutoNumber;
                    }
                }
                #endregion

                #region From Room
                objRoomDAL = new RoomDAL(base.DataBaseName);
                objDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,POAutoSequence,NextOrderNo,POAutoNrFixedValue";
                obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.POAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.POAutoSequence.GetValueOrDefault(0);
                    if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberRoom > 0)
                        obj.NextOrderNo = Convert.ToString(LastUsedTemp.LastUsedTempIncrementNumberRoom);

                    switch (obj.POAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            objAutoNumber.IsBlank = true;
                            break;
                        case 3: //Increamenting by Order#
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse(obj.NextOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;

                            objAutoNumber.OrderNumber = Convert.ToString(iNextNo).Length > 22 ? Convert.ToString(iNextNo).Substring(0, 22) : Convert.ToString(iNextNo);
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 4: //Increamenting by Day
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0);
                            Int64.TryParse(obj.NextOrderNo, out iNextNo);
                            OrderMasterDAL objOrdDAL = new OrderMasterDAL(base.DataBaseName);
                            OrderMasterDTO objOrdDTO = objOrdDAL.GetLatestOrderByRoomIdPlain(RoomID, CompanyID);
                            if (objOrdDTO == null || objOrdDTO.Created.GetValueOrDefault(DateTime.MinValue).DayOfYear < DateTime.Now.DayOfYear)
                            {
                                iNextNo = iNextNo + 1;
                            }
                            objAutoNumber.OrderNumber = iNextNo.ToString().Length > 22 ? iNextNo.ToString().Substring(0, 22) : iNextNo.ToString();
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 5: //Date + Incrementing#
                            // iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse(obj.NextOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Length > 22 ? Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Substring(0, 22) : Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString());
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 6: //Date
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat).Length > 22 ? datetimetoConsider.ToString(strDateFormat).Substring(0, 22) : datetimetoConsider.ToString(strDateFormat);
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                            break;
                        case 7: //Fixed + Incrementing #
                            Int64.TryParse(obj.NextOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = obj.POAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = obj.POAutoNrFixedValue + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 8: //Date  + Incrementing #+Fixed
                            //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            }
                            else
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                            }

                            string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                            Int64.TryParse(obj.NextOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(OrderNumberDateFormat) + "-" + obj.POAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + obj.POAutoNrFixedValue + "-" + "000" + iNextNo.ToString().PadLeft(3, '0');
                            break;
                    }

                    if (objAutoNumber != null)
                    {
                        if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                        {
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                        }
                    }

                    return objAutoNumber;
                }
                #endregion

                objAutoNumber.ErrorDescription = MsgPOSequanceNotDefineValidation;
                if (objAutoNumber != null)
                {
                    if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                    {
                        objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                    }
                }
                return objAutoNumber;
            }
            finally
            {
                objSupplierDTO = null;
                objSupplierDAL = null;
                objAutoNumber = null;
            }

        }
        public AutoOrderNumberGenerate GetNextToolAssetOrderNumber(Int64 RoomID, Int64 CompanyID, Int64 SupplierID,Int64 EnterpriseId, AutoOrderNumberGenerate LastUsedTemp = null, bool IsPreviousSupplier = false)
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            Int64 iNextNo = 0;
            RoomDTO obj = null;
            RoomDAL objRoomDAL = null;
            CommonDAL objDAL = null;
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;
            try
            {
                #region DateFormate From CompnayConfig

                string strDateFormat = "MM/dd/yyyy";
                string returnString = string.Empty;
                //objCompConfDAL = new CompanyConfigDAL(base.DataBaseName);
                //objCompConfDTO = objCompConfDAL.GetRecord(CompanyID);

                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

                if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
                {
                    strDateFormat = "yyyy/MM/dd";
                }

                objRegionSettingDAL = null;
                #endregion

                #region [Regional Settings]
                //objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
                if (objeTurnsRegionInfo != null)
                {
                    strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

                }
                #endregion

                objAutoNumber = new AutoOrderNumberGenerate();

                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else
                {
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterpriseId, CompanyID);
                string MsgPOSequanceNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgPOSequanceNotDefineValidation", ResourceFileCommon,EnterpriseId,CompanyID,RoomID, "ResOrder",currentCulture);
                #region From Room
                objDAL = new CommonDAL(base.DataBaseName);
                objRoomDAL = new RoomDAL(base.DataBaseName);
                string columnList = "ID,RoomName,TAOAutoSequence,NextOrderNo,TAOAutoNrFixedValue";
                obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.TAOAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.TAOAutoSequence.GetValueOrDefault(0);
                    if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberRoom > 0)
                        obj.NextOrderNo = Convert.ToString(LastUsedTemp.LastUsedTempIncrementNumberRoom);

                    switch (obj.TAOAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            objAutoNumber.IsBlank = true;
                            break;
                        case 3: //Increamenting by Order#
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse(obj.NextToolAssetOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;

                            objAutoNumber.OrderNumber = Convert.ToString(iNextNo).Length > 22 ? Convert.ToString(iNextNo).Substring(0, 22) : Convert.ToString(iNextNo);
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 4: //Increamenting by Day
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0);
                            Int64.TryParse(obj.NextToolAssetOrderNo, out iNextNo);
                            ToolAssetOrderMasterDAL objOrdDAL = new ToolAssetOrderMasterDAL(base.DataBaseName);
                            ToolAssetOrderMasterDTO objOrdDTO = objOrdDAL.GetAllRecords(RoomID, CompanyID, ToolAssetOrderType.Order).OrderBy("ID Desc").FirstOrDefault();
                            if (objOrdDTO == null || objOrdDTO.Created.GetValueOrDefault(DateTime.MinValue).DayOfYear < DateTime.Now.DayOfYear)
                            {
                                iNextNo = iNextNo + 1;
                            }
                            objAutoNumber.OrderNumber = iNextNo.ToString().Length > 22 ? iNextNo.ToString().Substring(0, 22) : iNextNo.ToString();
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 5: //Date + Incrementing#
                            // iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse(obj.NextToolAssetOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Length > 22 ? Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Substring(0, 22) : Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString());
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 6: //Date
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat).Length > 22 ? datetimetoConsider.ToString(strDateFormat).Substring(0, 22) : datetimetoConsider.ToString(strDateFormat);
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                            break;
                        case 7: //Fixed + Incrementing #
                            Int64.TryParse(obj.NextToolAssetOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = obj.TAOAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = obj.TAOAutoNrFixedValue + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 8: //Date  + Incrementing #+Fixed
                            //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                            Int64.TryParse(obj.NextToolAssetOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(OrderNumberDateFormat) + "-" + obj.TAOAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + obj.TAOAutoNrFixedValue + "-" + "000" + iNextNo.ToString().PadLeft(3, '0');
                            break;
                    }

                    if (objAutoNumber != null)
                    {
                        if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                        {
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                        }
                    }

                    return objAutoNumber;
                }
                #endregion

                objAutoNumber.ErrorDescription = MsgPOSequanceNotDefineValidation;
                if (objAutoNumber != null)
                {
                    if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                    {
                        objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                    }
                }
                return objAutoNumber;
            }
            finally
            {
                objAutoNumber = null;
            }

        }

        public AutoOrderNumberGenerate GetNextWorkOrderNumber(Int64 RoomID, Int64 CompanyID,Int64 EnterpriseID, AutoOrderNumberGenerate LastUsedTemp = null, bool IsPreviousSupplier = false)
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            Int64 iNextNo = 0;
            RoomDTO obj = null;
            RoomDAL objRoomDAL = null;
            CommonDAL objDAL = null;
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;
            try
            {
                #region DateFormate From CompnayConfig

                string strDateFormat = "MM/dd/yyyy";
                string returnString = string.Empty;
                //objCompConfDAL = new CompanyConfigDAL(base.DataBaseName);
                //objCompConfDTO = objCompConfDAL.GetRecord(CompanyID);

                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

                if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
                {
                    strDateFormat = "yyyy/MM/dd";
                }

                objRegionSettingDAL = null;
                #endregion

                #region [Regional Settings]
                //objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
                if (objeTurnsRegionInfo != null)
                {
                    strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

                }
                #endregion

                objAutoNumber = new AutoOrderNumberGenerate();
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else
                {
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterpriseID, CompanyID);
                string MsgWOSequanceNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgWOSequanceNotDefineValidation", ResourceFileCommon,EnterpriseID,CompanyID,RoomID, "ResOrder",currentCulture);

                #region From Room
                objDAL = new CommonDAL(base.DataBaseName);
                objRoomDAL = new RoomDAL(base.DataBaseName);
                string columnList = "ID,RoomName,WorkOrderAutoSequence,NextWorkOrderNo,WorkOrderAutoNrFixedValue";
                obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.WorkOrderAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.WorkOrderAutoSequence.GetValueOrDefault(0);
                    if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberRoom > 0)
                        obj.NextWorkOrderNo = LastUsedTemp.LastUsedTempIncrementNumberRoom;

                    switch (obj.WorkOrderAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            objAutoNumber.IsBlank = true;
                            break;
                        case 3: //Increamenting by Order#
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;

                            objAutoNumber.OrderNumber = Convert.ToString(iNextNo).Length > 22 ? Convert.ToString(iNextNo).Substring(0, 22) : Convert.ToString(iNextNo);
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 4: //Increamenting by Day
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0);
                            Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out iNextNo);
                            WorkOrderDAL objWOrdDAL = new WorkOrderDAL(base.DataBaseName);
                            WorkOrderDTO objWOrdDTO = objWOrdDAL.GetLastWorkOrderByRoomPlain(RoomID, CompanyID);
                            if (objWOrdDTO == null || objWOrdDTO.Created.GetValueOrDefault(DateTime.MinValue).DayOfYear < DateTime.Now.DayOfYear)
                            {
                                iNextNo = iNextNo + 1;
                            }
                            objAutoNumber.OrderNumber = iNextNo.ToString().Length > 22 ? iNextNo.ToString().Substring(0, 22) : iNextNo.ToString();
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 5: //Date + Incrementing#
                            // iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Length > 22 ? Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Substring(0, 22) : Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString());
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 6: //Date
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat).Length > 22 ? datetimetoConsider.ToString(strDateFormat).Substring(0, 22) : datetimetoConsider.ToString(strDateFormat);
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                            break;
                        case 7: //Fixed + Incrementing #
                            Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = obj.WorkOrderAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = obj.WorkOrderAutoNrFixedValue + "-" + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 8: //Date  + Incrementing #+Fixed
                            //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                            //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            }
                            else
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                            }

                            string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                            Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(OrderNumberDateFormat) + "-" + obj.WorkOrderAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            //objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + obj.WorkOrderAutoNrFixedValue + "-" + "000" + iNextNo.ToString().PadLeft(3, '0');
                            break;
                    }

                    if (objAutoNumber != null)
                    {
                        if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                        {
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                        }
                    }

                    return objAutoNumber;
                }
                #endregion

                objAutoNumber.ErrorDescription = MsgWOSequanceNotDefineValidation;
                if (objAutoNumber != null)
                {
                    if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                    {
                        objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                    }
                }
                return objAutoNumber;
            }
            finally
            {
                objAutoNumber = null;
            }

        }

        public AutoOrderNumberGenerate GetNextTransferNumber(Int64 RoomID, Int64 CompanyID,Int64 EnterpriseID, AutoOrderNumberGenerate LastUsedTemp = null, bool IsPreviousSupplier = false)
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            Int64 iNextNo = 0;
            RoomDTO obj = null;
            RoomDAL objRoomDAL = null;
            CommonDAL objDAL = null;
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;
            try
            {
                #region DateFormate From CompnayConfig

                string strDateFormat = "MM/dd/yyyy";
                string returnString = string.Empty;
                //objCompConfDAL = new CompanyConfigDAL(base.DataBaseName);
                //objCompConfDTO = objCompConfDAL.GetRecord(CompanyID);

                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

                if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
                {
                    strDateFormat = "yyyy/MM/dd";
                }

                objRegionSettingDAL = null;
                #endregion

                #region [Regional Settings]
                //objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
                if (objeTurnsRegionInfo != null)
                {
                    strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

                }
                #endregion

                objAutoNumber = new AutoOrderNumberGenerate();
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else
                {
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResTransfer", currentCulture, EnterpriseID, CompanyID);
                string MsgTransferSequienceNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgTransferSequienceNotDefineValidation", ResourceFileCommon,EnterpriseID,CompanyID,RoomID, "ResTransfer",currentCulture);

                #region From Room
                objDAL = new CommonDAL(base.DataBaseName);
                objRoomDAL = new RoomDAL(base.DataBaseName);
                string columnList = "ID,RoomName,TransferAutoSequence,NextTransferNo,TransferAutoNrFixedValue";
                obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.TransferAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.TransferAutoSequence.GetValueOrDefault(0);
                    if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberRoom > 0)
                        obj.NextTransferNo = LastUsedTemp.LastUsedTempIncrementNumberRoom;

                    switch (obj.TransferAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            objAutoNumber.IsBlank = true;
                            break;
                        case 3: //Increamenting by Order#
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;

                            objAutoNumber.OrderNumber = Convert.ToString(iNextNo).Length > 22 ? Convert.ToString(iNextNo).Substring(0, 22) : Convert.ToString(iNextNo);
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 4: //Increamenting by Day
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0);
                            Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out iNextNo);
                            TransferMasterDAL objTransDAL = new TransferMasterDAL(base.DataBaseName);
                            TransferMasterDTO objTransDTO = objTransDAL.GetLatestTransferByRoomPlain(RoomID, CompanyID);
                            if (objTransDTO == null || objTransDTO.Created.DayOfYear < DateTime.Now.DayOfYear)
                            {
                                iNextNo = iNextNo + 1;
                            }
                            objAutoNumber.OrderNumber = iNextNo.ToString().Length > 22 ? iNextNo.ToString().Substring(0, 22) : iNextNo.ToString();
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 5: //Date + Incrementing#
                            // iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Length > 22 ? Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Substring(0, 22) : Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString());
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 6: //Date
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat).Length > 22 ? datetimetoConsider.ToString(strDateFormat).Substring(0, 22) : datetimetoConsider.ToString(strDateFormat);
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                            break;
                        case 7: //Fixed + Incrementing #
                            Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = obj.TransferAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = obj.TransferAutoNrFixedValue + "-" + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 8: //Date  + Incrementing #+Fixed
                            //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                            //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            }
                            else
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                            }

                            string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                            Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(OrderNumberDateFormat) + "-" + obj.TransferAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            //objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + obj.TransferAutoNrFixedValue + "-" + "000" + iNextNo.ToString().PadLeft(3, '0');
                            break;
                    }

                    if (objAutoNumber != null)
                    {
                        if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                        {
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                        }
                    }

                    return objAutoNumber;
                }
                #endregion

                objAutoNumber.ErrorDescription = MsgTransferSequienceNotDefineValidation;
                if (objAutoNumber != null)
                {
                    if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                    {
                        objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                    }
                }
                return objAutoNumber;
            }
            finally
            {
                objAutoNumber = null;
            }

        }

        public AutoOrderNumberGenerate GetNextStagingNumber(Int64 RoomID, Int64 CompanyID,Int64 EnterpriseId, AutoOrderNumberGenerate LastUsedTemp = null, bool IsPreviousSupplier = false)
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            Int64 iNextNo = 0;
            RoomDTO obj = null;
            RoomDAL objRoomDAL = null;
            CommonDAL objDAL = null;
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;
            try
            {
                #region DateFormate From CompnayConfig

                string strDateFormat = "MM/dd/yyyy";
                string returnString = string.Empty;
                //objCompConfDAL = new CompanyConfigDAL(base.DataBaseName);
                //objCompConfDTO = objCompConfDAL.GetRecord(CompanyID);

                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

                if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
                {
                    strDateFormat = "yyyy/MM/dd";
                }

                objRegionSettingDAL = null;
                #endregion

                #region [Regional Settings]
                //objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
                if (objeTurnsRegionInfo != null)
                {
                    strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

                }
                #endregion

                objAutoNumber = new AutoOrderNumberGenerate();
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else
                {
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResMaterialStaging", currentCulture, EnterpriseId, CompanyID);
                string MsgStagingSequienceNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgStagingSequienceNotDefineValidation", ResourceFileCommon,EnterpriseId,CompanyID,RoomID, "ResMaterialStaging",currentCulture);



                #region From Room
                objDAL = new CommonDAL(base.DataBaseName);
                objRoomDAL = new RoomDAL(base.DataBaseName);
                string columnList = "ID,RoomName,StagingAutoSequence,NextStagingNo,StagingAutoNrFixedValue";
                obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                if (obj.StagingAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.StagingAutoSequence.GetValueOrDefault(0);
                    if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberRoom > 0)
                        obj.NextStagingNo = LastUsedTemp.LastUsedTempIncrementNumberRoom;

                    switch (obj.StagingAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            objAutoNumber.IsBlank = true;
                            break;
                        case 3: //Increamenting by Order#
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;

                            objAutoNumber.OrderNumber = Convert.ToString(iNextNo).Length > 22 ? Convert.ToString(iNextNo).Substring(0, 22) : Convert.ToString(iNextNo);
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 4: //Increamenting by Day
                            //iNextNo = obj.NextOrderNo.GetValueOrDefault(0);
                            Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out iNextNo);
                            MaterialStagingDAL objMSDAL = new MaterialStagingDAL(base.DataBaseName);
                            //MaterialStagingDTO objMSDTO = objMSDAL.GetAllRecords(RoomID, CompanyID, false, false).OrderBy("ID Desc").FirstOrDefault();
                            MaterialStagingDTO objMSDTO = objMSDAL.GetMaterialStaging(RoomID, CompanyID, false, false, string.Empty, null).OrderBy("ID Desc").FirstOrDefault();
                            if (objMSDTO == null || objMSDTO.Created.GetValueOrDefault(DateTime.MinValue).DayOfYear < DateTime.Now.DayOfYear)
                            {
                                iNextNo = iNextNo + 1;
                            }
                            objAutoNumber.OrderNumber = iNextNo.ToString().Length > 22 ? iNextNo.ToString().Substring(0, 22) : iNextNo.ToString();
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            break;
                        case 5: //Date + Incrementing#
                            // iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                            Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Length > 22 ? Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Substring(0, 22) : Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString());
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 6: //Date
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat).Length > 22 ? datetimetoConsider.ToString(strDateFormat).Substring(0, 22) : datetimetoConsider.ToString(strDateFormat);
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                            break;
                        case 7: //Fixed + Incrementing #
                            Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = obj.StagingAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            //if (LastUsedTemp != null)
                            //    objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = obj.StagingAutoNrFixedValue + "-" + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 8: //Date  + Incrementing #+Fixed
                            //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                            //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            }
                            else
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                            }

                            string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                            Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(OrderNumberDateFormat) + "-" + obj.StagingAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            //objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + obj.StagingAutoNrFixedValue + "-" + "000" + iNextNo.ToString().PadLeft(3, '0');
                            break;
                    }

                    if (objAutoNumber != null)
                    {
                        if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                        {
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                        }
                    }

                    return objAutoNumber;
                }
                #endregion

                objAutoNumber.ErrorDescription = MsgStagingSequienceNotDefineValidation;
                if (objAutoNumber != null)
                {
                    if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                    {
                        objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                    }
                }
                return objAutoNumber;
            }
            finally
            {
                objAutoNumber = null;
            }

        }

        public AutoOrderNumberGenerate GetNextRequisitionNumber(Int64 RoomID, Int64 CompanyID, Int64 SupplierID, Int64 EnterpriseID, AutoOrderNumberGenerate LastUsedTemp = null, bool IsPreviousSupplier = false)
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            Int64 iNextNo = 0;
            RoomDTO obj = null;
            RoomDAL objRoomDAL = null;
            CommonDAL objDAL = null;
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;
            try
            {
                #region DateFormate From CompnayConfig

                string strDateFormat = "MM/dd/yyyy";
                string returnString = string.Empty;

                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

                if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
                {
                    strDateFormat = "yyyy/MM/dd";
                }

                objRegionSettingDAL = null;
                #endregion

                #region [Regional Settings]
                //objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
                if (objeTurnsRegionInfo != null)
                {
                    strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

                }
                #endregion

                objAutoNumber = new AutoOrderNumberGenerate();
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else
                {
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterpriseID, CompanyID);
                string MsgPOSequanceNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgPOSequanceNotDefineValidation", ResourceFileCommon,EnterpriseID,CompanyID,RoomID, "ResOrder",currentCulture);
                #region From Supplier
                /* if (SupplierID > 0)
                {
                    objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSupplierDTO = objSupplierDAL.GetSupplierByID(SupplierID, RoomID, CompanyID, false);
                    if (objSupplierDTO != null && objSupplierDTO.POAutoSequence.HasValue)
                    {
                        objAutoNumber.OrderGeneratedFrom = "Supplier";
                        objAutoNumber.OrderNumberFormateType = objSupplierDTO.POAutoSequence.GetValueOrDefault(0);
                        if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberSupplier > 0 && IsPreviousSupplier)
                            objSupplierDTO.NextOrderNo = (LastUsedTemp.LastUsedTempIncrementNumberSupplier).ToString();


                        switch (objSupplierDTO.POAutoSequence.GetValueOrDefault(0))
                        {

                            case 0: //Blank
                                objAutoNumber.IsBlank = true;
                                break;
                            case 1: //Fixed

                                if (!string.IsNullOrEmpty(objSupplierDTO.NextOrderNo))
                                {
                                    objAutoNumber.OrderNumber = objSupplierDTO.NextOrderNo;
                                    objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                }
                                else
                                    objAutoNumber.ErrorDescription = "Fixed order number not define.";
                                break;
                            case 2: //Blanket Order #
                                objAutoNumber.IsBlanketPO = true;
                                objAutoNumber.BlanketPOs = new SupplierBlanketPODetailsDAL(base.DataBaseName).GetAllBlanketPOBySupplierID(SupplierID, RoomID, CompanyID).ToList();
                                if (objAutoNumber.BlanketPOs == null || objAutoNumber.BlanketPOs.Count <= 0)
                                {
                                    objAutoNumber.ErrorDescription = "Blanket PO not define.";
                                }
                                if (objAutoNumber.BlanketPOs == null || objAutoNumber.BlanketPOs.Count <= 0)
                                    objAutoNumber.BlanketPOs.Insert(0, null);

                                break;
                            case 3: //Increamenting by Order#
                                //iNextNo = objSupplierDTO.NextOrderNo.GetValueOrDefault(0) + 1;
                                Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = Convert.ToString(iNextNo);
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                break;
                            case 4: //Increamenting by Day
                                //objAutoNumber.OrderNumber = System.DateTime.Now.DayOfYear.ToString();
                                //iNextNo = objSupplierDTO.NextOrderNo.GetValueOrDefault(0);
                                Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                                OrderMasterDAL objOrdDAL = new OrderMasterDAL(base.DataBaseName);
                                OrderMasterDTO objOrdDTO = objOrdDAL.GetAllRecords(RoomID, CompanyID, 0).OrderBy("ID Desc").FirstOrDefault();

                                if (objOrdDTO == null || objOrdDTO.Created.GetValueOrDefault(DateTime.MinValue).DayOfYear < DateTime.Now.DayOfYear)
                                {
                                    iNextNo = iNextNo + 1;
                                }

                                objAutoNumber.OrderNumber = iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                break;
                            case 5: //Date + Incrementing#
                                Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0');
                                break;
                            case 6: //Date
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat);
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                                break;

                        }
                        if (objAutoNumber != null)
                        {
                            if (!string.IsNullOrEmpty(objAutoNumber.OrderNumber) && objAutoNumber.OrderNumber.Length > 22)
                            {
                                objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Substring(0, 22);
                            }
                        }
                        return objAutoNumber;
                    }
                } */
                #endregion

                #region From Room
                objRoomDAL = new RoomDAL(base.DataBaseName);
                objDAL = new CommonDAL(base.DataBaseName);
                obj = new RoomDTO();
                string columnList = "ID,RoomName,ReqAutoSequence,NextRequisitionNo";

                obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                if (obj.ReqAutoSequence.HasValue)
                {
                    objAutoNumber.RequisitionGeneratedFrom = "Room";
                    objAutoNumber.RequisitionFormateType = obj.ReqAutoSequence.GetValueOrDefault(0);
                    if (LastUsedTemp != null && LastUsedTemp.LastReqUsedTempIncrementNumberRoom > 0)
                        obj.NextRequisitionNo = Convert.ToInt64(LastUsedTemp.LastReqUsedTempIncrementNumberRoom);

                    switch (obj.ReqAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            objAutoNumber.IsBlank = true;
                            break;
                        case 3: //Increamenting by Order#
                            iNextNo = obj.NextRequisitionNo.GetValueOrDefault(0);
                            iNextNo = iNextNo + 1;

                            objAutoNumber.RequisitionNumber = Convert.ToString(iNextNo).Length > 22 ? Convert.ToString(iNextNo).Substring(0, 22) : Convert.ToString(iNextNo);
                            objAutoNumber.LastReqUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastReqUsedTempIncrementNumberSupplier = LastUsedTemp.LastReqUsedTempIncrementNumberSupplier;
                            objAutoNumber.RequisitionNumberForSorting = objAutoNumber.RequisitionNumber.ToString().PadLeft(11, '0');
                            break;
                        case 4: //Increamenting by Day
                            iNextNo = obj.NextRequisitionNo.GetValueOrDefault(0);
                            //Int64.TryParse(obj.NextOrderNo, out iNextNo);
                            RequisitionMasterDAL objReqDAL = new RequisitionMasterDAL(base.DataBaseName);
                            //RequisitionMasterDTO objReqDTO = objReqDAL.GetAllRecords(RoomID, CompanyID, false, false).OrderBy("ID Desc").FirstOrDefault();
                            RequisitionMasterDTO objReqDTO = objReqDAL.GetLastRequisitionByRoomIDPlain(RoomID, CompanyID);
                            if (objReqDTO == null || objReqDTO.Created.GetValueOrDefault(DateTime.MinValue).DayOfYear < DateTime.Now.DayOfYear)
                            {
                                iNextNo = iNextNo + 1;
                            }
                            objAutoNumber.RequisitionNumber = iNextNo.ToString().Length > 22 ? iNextNo.ToString().Substring(0, 22) : iNextNo.ToString();
                            objAutoNumber.LastReqUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastReqUsedTempIncrementNumberSupplier = LastUsedTemp.LastReqUsedTempIncrementNumberSupplier;
                            objAutoNumber.RequisitionNumberForSorting = objAutoNumber.RequisitionNumber.ToString().PadLeft(11, '0');
                            break;
                        case 5: //Date + Incrementing#
                            iNextNo = obj.NextRequisitionNo.GetValueOrDefault(0);
                            //Int64.TryParse(obj.NextOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.RequisitionNumber = Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Length > 22 ? Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Substring(0, 22) : Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString());
                            objAutoNumber.LastReqUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastReqUsedTempIncrementNumberSupplier = LastUsedTemp.LastReqUsedTempIncrementNumberSupplier;
                            objAutoNumber.RequisitionNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0');
                            break;
                        case 6: //Date
                            objAutoNumber.RequisitionNumber = datetimetoConsider.ToString(strDateFormat).Length > 22 ? datetimetoConsider.ToString(strDateFormat).Substring(0, 22) : datetimetoConsider.ToString(strDateFormat);
                            objAutoNumber.RequisitionNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                            break;

                    }
                    if (objAutoNumber != null)
                    {
                        if (!string.IsNullOrEmpty(objAutoNumber.RequisitionNumber) && objAutoNumber.RequisitionNumber.Length > 22)
                        {
                            objAutoNumber.RequisitionNumber = objAutoNumber.RequisitionNumber.Substring(0, 22);
                        }
                    }
                    return objAutoNumber;
                }
                #endregion

                objAutoNumber.ErrorDescription = MsgPOSequanceNotDefineValidation;
                if (objAutoNumber != null)
                {
                    if (!string.IsNullOrEmpty(objAutoNumber.RequisitionNumber) && objAutoNumber.RequisitionNumber.Length > 22)
                    {
                        objAutoNumber.RequisitionNumber = objAutoNumber.RequisitionNumber.Substring(0, 22);
                    }
                }
                return objAutoNumber;
            }
            finally
            {
                objAutoNumber = null;
            }

        }


        public AutoOrderNumberGenerate GetNextPullOrderNumber(Int64 RoomID, Int64 CompanyID, Int64 SupplierID, Guid ItemGUID,Int64 EnterpriseID, AutoOrderNumberGenerate LastUsedTemp = null, bool IsPreviousSupplier = false)
        {
            AutoOrderNumberGenerate objAutoNumber = null;
            Int64 iNextNo = 0;
            RoomDTO obj = null;
            RoomDAL objRoomDAL = null;
            CommonDAL objDAL = null;
            SupplierMasterDTO objSupplierDTO = null;
            SupplierMasterDAL objSupplierDAL = null;
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            try
            {
                #region DateFormate From CompnayConfig

                string strDateFormat = "MM/dd/yyyy";
                string returnString = string.Empty;
                //objCompConfDAL = new CompanyConfigDAL(base.DataBaseName);
                //objCompConfDTO = objCompConfDAL.GetRecord(CompanyID);

                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

                if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
                {
                    strDateFormat = "yyyy/MM/dd";
                }

                objRegionSettingDAL = null;
                #endregion

                #region [Regional Settings]
                //objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
                if (objeTurnsRegionInfo != null)
                {
                    strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

                }
                #endregion

                objAutoNumber = new AutoOrderNumberGenerate();
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else
                {
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterpriseID, CompanyID);
                string MsgFixedOrderNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFixedOrderNumberValidation", ResourceFileCommon,EnterpriseID,CompanyID,RoomID, "ResOrder",currentCulture);
                string MsgBlanketNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgBlanketNotDefineValidation", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                string MsgPOSequanceNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgPOSequanceNotDefineValidation", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                #region [From Item]
                var blanketPO = objItemMasterDAL.GetItemSupplierDetails(ItemGUID);

                if (!string.IsNullOrEmpty(blanketPO) && !string.IsNullOrWhiteSpace(blanketPO))
                {
                    objAutoNumber.OrderNumber = blanketPO;
                    return objAutoNumber;
                }


                #endregion

                #region From Supplier

                if (SupplierID > 0)
                {
                    objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(SupplierID);
                    if (objSupplierDTO != null && objSupplierDTO.PullPurchaseNumberType.HasValue)
                    {
                        objAutoNumber.OrderGeneratedFrom = "Supplier";
                        objAutoNumber.OrderNumberFormateType = objSupplierDTO.PullPurchaseNumberType.GetValueOrDefault(0);
                        if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberSupplier > 0 && IsPreviousSupplier)
                            objSupplierDTO.LastPullPurchaseNumberUsed = (LastUsedTemp.LastUsedTempIncrementNumberSupplier).ToString();


                        switch (objSupplierDTO.PullPurchaseNumberType.GetValueOrDefault(0))
                        {

                            case 0: //Blank
                                objAutoNumber.IsBlank = true;
                                break;
                            case 1: //Fixed

                                if (!string.IsNullOrEmpty(objSupplierDTO.LastPullPurchaseNumberUsed))
                                {
                                    objAutoNumber.OrderNumber = objSupplierDTO.LastPullPurchaseNumberUsed;
                                    objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                }
                                else
                                    objAutoNumber.ErrorDescription = MsgFixedOrderNumberValidation;
                                break;
                            case 2: //Blanket Order #
                                objAutoNumber.IsBlanketPO = true;
                                objAutoNumber.BlanketPOs = new SupplierBlanketPODetailsDAL(base.DataBaseName).GetBlanketPOBySupplierIDPlain(SupplierID, RoomID, CompanyID).ToList();
                                if (objAutoNumber.BlanketPOs == null || objAutoNumber.BlanketPOs.Count <= 0)
                                {
                                    objAutoNumber.ErrorDescription = MsgBlanketNotDefineValidation;
                                    objAutoNumber.BlanketPOs.Insert(0, null);
                                }
                                if (!string.IsNullOrWhiteSpace(objSupplierDTO.LastPullPurchaseNumberUsed))
                                    objAutoNumber.OrderNumber = objSupplierDTO.LastPullPurchaseNumberUsed;
                                else
                                    objAutoNumber.OrderNumber = string.Empty;

                                objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');

                                if (objAutoNumber.BlanketPOs != null && string.IsNullOrWhiteSpace(objSupplierDTO.LastPullPurchaseNumberUsed) && objAutoNumber.BlanketPOs.Count > 0)
                                {
                                    if (objAutoNumber.BlanketPOs.First() != null)
                                    {
                                        objAutoNumber.OrderNumber = objAutoNumber.BlanketPOs.First().BlanketPO;
                                    }
                                }
                                break;
                            case 3: //Increamenting by Order#
                                //iNextNo = objSupplierDTO.LastPullPurchaseNumberUsed.GetValueOrDefault(0) + 1;
                                Int64.TryParse(objSupplierDTO.LastPullPurchaseNumberUsed, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = Convert.ToString(iNextNo);
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                objSupplierDAL.UpdatedLastUsedNumber(iNextNo, objSupplierDTO.ID, 0, "supplier");
                                break;
                            case 4: //Increamenting by Day
                                //objAutoNumber.OrderNumber = System.DateTime.Now.DayOfYear.ToString();
                                //iNextNo = objSupplierDTO.LastPullPurchaseNumberUsed.GetValueOrDefault(0);
                                Int64.TryParse(objSupplierDTO.LastPullPurchaseNumberUsed, out iNextNo);
                                OrderMasterDAL objOrdDAL = new OrderMasterDAL(base.DataBaseName);
                                OrderMasterDTO objOrdDTO = objOrdDAL.GetLatestOrderByRoomIdPlain(RoomID, CompanyID);

                                if (objOrdDTO == null || objOrdDTO.Created.GetValueOrDefault(DateTime.MinValue).DayOfYear < DateTime.Now.DayOfYear)
                                {
                                    iNextNo = iNextNo + 1;
                                }

                                objAutoNumber.OrderNumber = iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                objSupplierDAL.UpdatedLastUsedNumber(iNextNo, objSupplierDTO.ID, 0, "supplier");
                                break;
                            case 5: //Date + Incrementing#
                                Int64.TryParse(objSupplierDTO.LastPullPurchaseNumberUsed, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0');
                                objSupplierDAL.UpdatedLastUsedNumber(iNextNo, objSupplierDTO.ID, 0, "supplier");
                                break;
                            case 6: //Date
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat);
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                                break;
                            case 7: //Fixed + Incrementing#
                                Int64.TryParse(objSupplierDTO.LastPullPurchaseNumberUsed, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = objSupplierDTO.PullPurchaseNrFixedValue + "-" + iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = objSupplierDTO.PullPurchaseNrFixedValue + iNextNo.ToString().PadLeft(3, '0');
                                objSupplierDAL.UpdatedLastUsedNumber(iNextNo, objSupplierDTO.ID, 0, "supplier");
                                break;
                            case 8: //Date  + Incrementing #+Fixed
                                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                                //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                }
                                else
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                                }

                                string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                                Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(OrderNumberDateFormat) + objSupplierDTO.POAutoNrFixedValue + iNextNo.ToString();
                                objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + objSupplierDTO.POAutoNrFixedValue + "000" + iNextNo.ToString().PadLeft(3, '0');
                                objSupplierDAL.UpdatedLastUsedNumber(iNextNo, objSupplierDTO.ID, 0, "supplier");
                                break;

                        }
                        return objAutoNumber;
                    }
                }
                #endregion

                #region From Room

                objDAL = new CommonDAL(base.DataBaseName);
                objRoomDAL = new RoomDAL(base.DataBaseName);
                objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                string columnList = "ID,RoomName,PullPurchaseNumberType,LastPullPurchaseNumberUsed,POAutoNrFixedValue,PullPurchaseNrFixedValue";
                obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.PullPurchaseNumberType.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.PullPurchaseNumberType.GetValueOrDefault(0);
                    if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberRoom > 0)
                        obj.LastPullPurchaseNumberUsed = Convert.ToString(LastUsedTemp.LastUsedTempIncrementNumberRoom);

                    switch (obj.PullPurchaseNumberType.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            objAutoNumber.IsBlank = true;
                            break;
                        case 1: //Fixed
                            if (!string.IsNullOrEmpty(objSupplierDTO.LastPullPurchaseNumberUsed))
                            {
                                objAutoNumber.OrderNumber = obj.LastPullPurchaseNumberUsed;
                                objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            }
                            else
                                objAutoNumber.ErrorDescription = MsgFixedOrderNumberValidation;
                            break;
                        case 3: //Increamenting by Order#
                            //iNextNo = obj.LastPullPurchaseNumberUsed.GetValueOrDefault(0) + 1;
                            Int64.TryParse(obj.LastPullPurchaseNumberUsed, out iNextNo);
                            iNextNo = iNextNo + 1;

                            objAutoNumber.OrderNumber = Convert.ToString(iNextNo);
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            //objSupplierDAL.UpdatedLastUsedNumber(iNextNo, 0, obj.ID, "room");
                            break;
                        case 4: //Increamenting by Day
                            //iNextNo = obj.LastPullPurchaseNumberUsed.GetValueOrDefault(0);
                            Int64.TryParse(obj.LastPullPurchaseNumberUsed, out iNextNo);
                            OrderMasterDAL objOrdDAL = new OrderMasterDAL(base.DataBaseName);
                            OrderMasterDTO objOrdDTO = objOrdDAL.GetLatestOrderByRoomIdPlain(RoomID, CompanyID);
                            if (objOrdDTO == null || objOrdDTO.Created.GetValueOrDefault(DateTime.MinValue).DayOfYear < DateTime.Now.DayOfYear)
                            {
                                iNextNo = iNextNo + 1;
                            }
                            objAutoNumber.OrderNumber = iNextNo.ToString();
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                            //objSupplierDAL.UpdatedLastUsedNumber(iNextNo, 0, obj.ID, "room");
                            break;
                        case 5: //Date + Incrementing#
                            // iNextNo = obj.LastPullPurchaseNumberUsed.GetValueOrDefault(0) + 1;
                            Int64.TryParse(obj.LastPullPurchaseNumberUsed, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString();
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0'); ;
                            //objSupplierDAL.UpdatedLastUsedNumber(iNextNo, 0, obj.ID, "room");
                            break;
                        case 6: //Date
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat);
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                            break;
                        case 7: //Fixed + Incrementing#
                            // iNextNo = obj.LastPullPurchaseNumberUsed.GetValueOrDefault(0) + 1;
                            Int64.TryParse(obj.LastPullPurchaseNumberUsed, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = obj.PullPurchaseNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = LastUsedTemp.LastUsedTempIncrementNumberSupplier;
                            objAutoNumber.OrderNumberForSorting = obj.PullPurchaseNrFixedValue + iNextNo.ToString().PadLeft(3, '0'); ;
                            //objSupplierDAL.UpdatedLastUsedNumber(iNextNo, 0, obj.ID, "room");
                            break;
                        case 8: //Date  + Incrementing #+Fixed
                            //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                            //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                            }
                            else
                            {
                                //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                            }

                            string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                            Int64.TryParse(objSupplierDTO.NextOrderNo, out iNextNo);
                            iNextNo = iNextNo + 1;
                            objAutoNumber.OrderNumber = datetimetoConsider.ToString(OrderNumberDateFormat) + objSupplierDTO.POAutoNrFixedValue + iNextNo.ToString();
                            objAutoNumber.OrderNumber = objAutoNumber.OrderNumber.Length > 22 ? objAutoNumber.OrderNumber.Substring(0, 22) : objAutoNumber.OrderNumber;
                            objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                            objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + objSupplierDTO.POAutoNrFixedValue + "000" + iNextNo.ToString().PadLeft(3, '0');
                            //objSupplierDAL.UpdatedLastUsedNumber(iNextNo, 0, obj.ID, "room");
                            break;

                    }

                    return objAutoNumber;
                }
                #endregion

                objAutoNumber.ErrorDescription = MsgPOSequanceNotDefineValidation;
                return objAutoNumber;
            }
            finally
            {
                objSupplierDTO = null;
                objSupplierDAL = null;
                objAutoNumber = null;
            }

        }

        public void UpdateNextCountNumber(Int64 RoomID, Int64 CompanyID, string CountNumber)
        {
            RoomDTO obj = null;
            RoomDAL objDAL = null;
            objDAL = new RoomDAL(base.DataBaseName);
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            string columnList = "ID,RoomName,CountAutoSequence,NextCountNo";
            obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            //obj = objDAL.GetRoomByIDPlain(RoomID);
            Int64 intNextAutoNumber = 0;
            if (obj.CountAutoSequence.HasValue)
            {

                switch (obj.CountAutoSequence.GetValueOrDefault(0))
                {

                    case 3: //Increamenting by Count#
                        Int64.TryParse(CountNumber, out intNextAutoNumber);
                        if (intNextAutoNumber > 0)
                        {
                            Int64 CurrentAutoNumber = 0;
                            Int64.TryParse(Convert.ToString(obj.NextCountNo), out CurrentAutoNumber);
                            if (intNextAutoNumber == (CurrentAutoNumber + 1))
                            {
                                obj.NextCountNo = Convert.ToString(intNextAutoNumber);
                                // objDAL.Edit(obj);
                                objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextCountNo, "NextCountNo");
                            }
                        }
                        return;
                    case 4: //Increamenting by day#
                        Int64.TryParse(CountNumber, out intNextAutoNumber);
                        if (intNextAutoNumber > 0)
                        {
                            Int64 CurrentAutoNumber = 0;
                            Int64.TryParse(obj.NextCountNo, out CurrentAutoNumber);
                            if (intNextAutoNumber == (CurrentAutoNumber + 1))
                            {
                                obj.NextCountNo = Convert.ToString(intNextAutoNumber);
                                //objDAL.Edit(obj);
                                objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextCountNo, "NextCountNo");
                            }
                        }
                        return;
                    case 5: //Date + Incrementing#
                        if (CountNumber.LastIndexOf("-") > 0 && CountNumber.LastIndexOf("-") < CountNumber.Trim().Length)
                        {
                            string incrNo = CountNumber.Remove(0, CountNumber.LastIndexOf("-") + 1);
                            Int64.TryParse(incrNo, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse(obj.NextCountNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextCountNo = Convert.ToString(intNextAutoNumber);
                                    //objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextCountNo, "NextCountNo");
                                }
                            }
                        }
                        return;
                }


            }

        }
       

        public void UpdateNextOrderNumber(Int64 RoomID, Int64 CompanyID, Int64 SupplierID, string OrderNumber, long SessionUserId, string ReleaseNumber = "")
        {
            AutoOrderNumberGenerate objAutoNumber = null;

            RoomDTO obj = null;
            RoomDAL objDAL = null;
            CommonDAL objCommonDAL = null;
            SupplierMasterDTO objSupplierDTO = null;
            SupplierMasterDAL objSupplierDAL = null;
            Int64 intNextAutoNumber = 0;
            try
            {
                objAutoNumber = new AutoOrderNumberGenerate();

                #region From Supplier

                if (SupplierID > 0)
                {
                    objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(SupplierID);
                    if (objSupplierDTO != null && objSupplierDTO.POAutoSequence.HasValue)
                    {
                        objAutoNumber.OrderGeneratedFrom = "Supplier";
                        objAutoNumber.OrderNumberFormateType = objSupplierDTO.POAutoSequence.GetValueOrDefault(0);

                        switch (objSupplierDTO.POAutoSequence.GetValueOrDefault(0))
                        {
                            case 1://Fix PO
                                if (!string.IsNullOrWhiteSpace(ReleaseNumber) && (!string.IsNullOrWhiteSpace(OrderNumber)))
                                {
                                    Int64.TryParse(ReleaseNumber, out intNextAutoNumber);
                                    if (intNextAutoNumber > 0)
                                    {
                                        Int64 CurrentAutoNumber = 0;
                                        Int64.TryParse(objSupplierDTO.POAutoNrReleaseNumber, out CurrentAutoNumber);
                                        if (intNextAutoNumber == (CurrentAutoNumber + 1) && objSupplierDTO.NextOrderNo.Trim() == OrderNumber.Trim())
                                        {
                                            objSupplierDTO.POAutoNrReleaseNumber = intNextAutoNumber.ToString();
                                            objSupplierDTO.IsOnlyFromItemUI = true;
                                            objSupplierDTO.EditedFrom = "Web";
                                            objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                        }
                                    }
                                }
                                return;
                            case 2: //Blanket PO
                                return;
                            case 6: //Date Only
                                return;
                            case 3: //Increamenting by Order#
                                Int64.TryParse(OrderNumber, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(objSupplierDTO.NextOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        objSupplierDTO.NextOrderNo = intNextAutoNumber.ToString();
                                        objSupplierDTO.IsOnlyFromItemUI = true;
                                        objSupplierDTO.EditedFrom = "Web";
                                        objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                    }
                                }
                                return;
                            case 4: //Increamenting by Day
                                Int64.TryParse(OrderNumber, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(objSupplierDTO.NextOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        objSupplierDTO.NextOrderNo = intNextAutoNumber.ToString();
                                        objSupplierDTO.IsOnlyFromItemUI = true;
                                        objSupplierDTO.EditedFrom = "Web";
                                        objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                    }
                                }
                                return;
                            case 5: //Date + Incrementing#
                                if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                                {
                                    string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                    Int64.TryParse(incrNo, out intNextAutoNumber);
                                    if (intNextAutoNumber > 0)
                                    {
                                        Int64 CurrentAutoNumber = 0;
                                        Int64.TryParse(objSupplierDTO.NextOrderNo, out CurrentAutoNumber);
                                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                        {
                                            objSupplierDTO.NextOrderNo = intNextAutoNumber.ToString();
                                            objSupplierDTO.IsOnlyFromItemUI = true;
                                            objSupplierDTO.EditedFrom = "Web";
                                            objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                        }
                                    }
                                }
                                return;
                            case 7: //Fixed + Incrementing #
                                if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                                {
                                    string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                    Int64.TryParse(incrNo, out intNextAutoNumber);
                                    if (intNextAutoNumber > 0)
                                    {
                                        Int64 CurrentAutoNumber = 0;
                                        Int64.TryParse(objSupplierDTO.NextOrderNo, out CurrentAutoNumber);
                                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                        {
                                            objSupplierDTO.NextOrderNo = intNextAutoNumber.ToString();
                                            objSupplierDTO.IsOnlyFromItemUI = true;
                                            objSupplierDTO.EditedFrom = "Web";
                                            objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                        }
                                    }
                                }
                                return;
                            case 8: //Date  + Incrementing #+ Fixed
                                    //  if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                                if (objSupplierDTO != null && objSupplierDTO.POAutoNrFixedValue != null)
                                {
                                    //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                                    //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                    if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                                    {
                                        //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                    }
                                    else
                                    {
                                        //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                                    }

                                    string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                                    string incrNo = OrderNumber.Replace(objSupplierDTO.POAutoNrFixedValue, string.Empty);
                                    if (incrNo.Length > OrderNumberDateFormat.Length)
                                        incrNo = incrNo.Remove(0, OrderNumberDateFormat.Length);

                                    incrNo = incrNo.Replace("--", "");
                                    Int64.TryParse(incrNo, out intNextAutoNumber);
                                    if (intNextAutoNumber > 0)
                                    {
                                        Int64 CurrentAutoNumber = 0;
                                        Int64.TryParse(objSupplierDTO.NextOrderNo, out CurrentAutoNumber);
                                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                        {
                                            objSupplierDTO.NextOrderNo = intNextAutoNumber.ToString();
                                            objSupplierDTO.IsOnlyFromItemUI = true;
                                            objSupplierDTO.EditedFrom = "Web";
                                            objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                        }
                                    }
                                }
                                return;
                        }

                    }
                }
                #endregion

                #region From Room
                objDAL = new RoomDAL(base.DataBaseName);
                objCommonDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,POAutoSequence,NextOrderNo,POAutoNrFixedValue";
                obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.POAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.POAutoSequence.GetValueOrDefault(0);

                    switch (obj.POAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            return;
                        case 6: //Date Only
                            return;
                        case 3: //Increamenting by Order#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse(obj.NextOrderNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextOrderNo = Convert.ToString(intNextAutoNumber);
                                    //objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextOrderNo, "NextOrderNo");
                                }
                            }
                            return;
                        case 4: //Increamenting by day#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse(obj.NextOrderNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextOrderNo = Convert.ToString(intNextAutoNumber);
                                    // objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextOrderNo, "NextOrderNo");
                                }
                            }
                            return;
                        case 5: //Date + Incrementing#
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(obj.NextOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextOrderNo = Convert.ToString(intNextAutoNumber);
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextOrderNo, "NextOrderNo");
                                    }
                                }
                            }
                            return;
                        case 7: //Fixed + Incrementing #
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(obj.NextOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextOrderNo = Convert.ToString(intNextAutoNumber);
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextOrderNo, "NextOrderNo");
                                    }
                                }
                            }
                            return;
                        case 8: //Date  + Incrementing #+ Fixed
                            //if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            if (obj != null && obj.POAutoNrFixedValue != null)
                            {
                                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                                //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                }
                                else
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                                }

                                string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                                //string incrNo = OrderNumber.Replace(obj.POAutoNrFixedValue, string.Empty).Remove(0, OrderNumberDateFormat.Length);
                                string incrNo = OrderNumber.Replace(obj.POAutoNrFixedValue, string.Empty);
                                if (incrNo.Length > OrderNumberDateFormat.Length)
                                    incrNo = incrNo.Remove(0, OrderNumberDateFormat.Length);

                                incrNo = incrNo.Replace("--", "");
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(obj.NextOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextOrderNo = Convert.ToString(intNextAutoNumber);
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextOrderNo, "NextOrderNo");
                                    }
                                }
                            }
                            return;
                    }


                }
                #endregion

            }
            finally
            {
                objAutoNumber = null;
                obj = null;
                objDAL = null;
                objSupplierDTO = null;
                objSupplierDAL = null;
            }

        }

        public AutoQuoteNumberGenerate GetNextQuoteNumber(long RoomID, long CompanyID,long EnterpriseID, Int64 SupplierID, AutoQuoteNumberGenerate LastUsedTemp = null, bool IsPreviousSupplier = false)
        {
            AutoQuoteNumberGenerate objAutoNumber = null;
            long iNextNo = 0;
            RoomDTO obj = null;
            RoomDAL objRoomDAL = null;
            CommonDAL objDAL = null;
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;
            SupplierMasterDTO objSupplierDTO = null;
            SupplierMasterDAL objSupplierDAL = null;

            try
            {
                #region DateFormate From CompnayConfig

                string strDateFormat = "MM/dd/yyyy";
                string returnString = string.Empty;

                objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

                if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
                {
                    strDateFormat = "yyyy/MM/dd";
                }

                objRegionSettingDAL = null;
                #endregion

                #region [Regional Settings]
                if (objeTurnsRegionInfo != null)
                {
                    strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                    objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                    datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

                }
                #endregion

                objAutoNumber = new AutoQuoteNumberGenerate();
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else if (objeTurnsRegionInfo != null)
                {
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResQuoteMaster", currentCulture, EnterpriseID, CompanyID);
                string MsgPOSequanceNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgQuoteSequanceNotDefineValidation", ResourceFileCommon,EnterpriseID,CompanyID,RoomID, "ResQuoteMaster", currentCulture);

                #region From Supplier
                if (SupplierID > 0)
                {
                    objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(SupplierID);

                    string MsgFixedOrderNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFixedQuoteNumberValidation", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);
                    string MsgBlanketNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgBlanketNotDefineValidation", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResQuoteMaster", currentCulture);

                    if (objSupplierDTO != null && objSupplierDTO.QuoteAutoSequence.HasValue)
                    {
                        objAutoNumber.QuoteGeneratedFrom = "Supplier";
                        objAutoNumber.QuoteNumberFormateType = objSupplierDTO.POAutoSequence.GetValueOrDefault(0);
                        if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberSupplier > 0 && IsPreviousSupplier)
                            objSupplierDTO.NextQuoteNo = (LastUsedTemp.LastUsedTempIncrementNumberSupplier).ToString();


                        switch (objSupplierDTO.QuoteAutoSequence.GetValueOrDefault(0))
                        {

                            case 0: //Blank
                                objAutoNumber.IsBlank = true;
                                break;
                            case 1: //Fixed

                                if (!string.IsNullOrEmpty(objSupplierDTO.NextQuoteNo))
                                {
                                    objAutoNumber.QuoteNumber = objSupplierDTO.NextQuoteNo;
                                    objAutoNumber.QuoteNumberForSorting = objAutoNumber.QuoteNumber.ToString().PadLeft(11, '0');
                                }
                                else
                                    objAutoNumber.ErrorDescription = MsgFixedOrderNumberValidation;
                                break;
                            case 2: //Blanket Quote #
                                objAutoNumber.IsBlanketPO = true;
                                objAutoNumber.BlanketPOs = new SupplierBlanketPODetailsDAL(base.DataBaseName).GetBlanketPOBySupplierIDPlain(SupplierID, RoomID, CompanyID).ToList();
                                if (objAutoNumber.BlanketPOs == null || objAutoNumber.BlanketPOs.Count <= 0)
                                {
                                    objAutoNumber.ErrorDescription = MsgBlanketNotDefineValidation;
                                }
                                else if (objAutoNumber.BlanketPOs.Count > 0)
                                {
                                    objAutoNumber.QuoteNumber = objAutoNumber.BlanketPOs[0].BlanketPO;
                                }
                                if (objAutoNumber.BlanketPOs == null || objAutoNumber.BlanketPOs.Count <= 0)
                                    objAutoNumber.BlanketPOs.Insert(0, null);

                                break;
                            case 3: //Increamenting by Order#
                                //iNextNo = objSupplierDTO.NextOrderNo.GetValueOrDefault(0) + 1;
                                Int64.TryParse(objSupplierDTO.NextQuoteNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.QuoteNumber = Convert.ToString(iNextNo);
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.QuoteNumberForSorting = objAutoNumber.QuoteNumber.ToString().PadLeft(11, '0');
                                break;
                            case 4: //Increamenting by Day
                                //objAutoNumber.OrderNumber = System.DateTime.Now.DayOfYear.ToString();
                                //iNextNo = objSupplierDTO.NextOrderNo.GetValueOrDefault(0);
                                Int64.TryParse(objSupplierDTO.NextQuoteNo, out iNextNo);
                                QuoteMasterDAL objQuoteDAL = new QuoteMasterDAL(base.DataBaseName);
                                QuoteMasterDTO objOrdDTO = objQuoteDAL.GetLatestQuoteByRoomIdPlain(RoomID, CompanyID);

                                if (objOrdDTO == null || objOrdDTO.Created.DayOfYear < DateTime.Now.DayOfYear)
                                {
                                    iNextNo = iNextNo + 1;
                                }

                                objAutoNumber.QuoteNumber = iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.QuoteNumberForSorting = objAutoNumber.QuoteNumber.ToString().PadLeft(11, '0');
                                break;
                            case 5: //Date + Incrementing#
                                Int64.TryParse(objSupplierDTO.NextQuoteNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.QuoteNumber = datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.QuoteNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0');
                                break;
                            case 6: //Date
                                objAutoNumber.QuoteNumber = datetimetoConsider.ToString(strDateFormat);
                                objAutoNumber.QuoteNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                                break;
                            case 7: //Fixed + Incrementing #
                                Int64.TryParse(objSupplierDTO.NextQuoteNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.QuoteNumber = objSupplierDTO.QuoteAutoNrFixedValue + "-" + iNextNo.ToString();
                                objAutoNumber.QuoteNumber = objAutoNumber.QuoteNumber.Length > 22 ? objAutoNumber.QuoteNumber.Substring(0, 22) : objAutoNumber.QuoteNumber;
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.QuoteNumberForSorting = objSupplierDTO.QuoteAutoNrFixedValue + iNextNo.ToString().PadLeft(3, '0');
                                break;
                            case 8: //Date  + Incrementing #+Fixed

                                //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                }
                                else
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                                }


                                //string OrderNumberDateFormat = Settinfile.Element("OrderNumberDateFormat") != null ? Settinfile.Element("OrderNumberDateFormat").Value : "yyMMdd";
                                string OrderNumberDateFormat = (DTO.SiteSettingHelper.QuoteNumberDateFormat != string.Empty && DTO.SiteSettingHelper.QuoteNumberDateFormat != null) ? DTO.SiteSettingHelper.QuoteNumberDateFormat : "yyMMdd";

                                Int64.TryParse(objSupplierDTO.NextQuoteNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.QuoteNumber = datetimetoConsider.ToString(OrderNumberDateFormat) + "-" + objSupplierDTO.QuoteAutoNrFixedValue + "-" + iNextNo.ToString();
                                objAutoNumber.QuoteNumber = objAutoNumber.QuoteNumber.Length > 22 ? objAutoNumber.QuoteNumber.Substring(0, 22) : objAutoNumber.QuoteNumber;
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                if (LastUsedTemp != null)
                                    objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;
                                objAutoNumber.QuoteNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + objSupplierDTO.QuoteAutoNrFixedValue + "-" + "000" + iNextNo.ToString().PadLeft(3, '0');
                                break;

                        }
                        if (objAutoNumber != null)
                        {
                            if (!string.IsNullOrEmpty(objAutoNumber.QuoteNumber) && objAutoNumber.QuoteNumber.Length > 22)
                            {
                                objAutoNumber.QuoteNumber = objAutoNumber.QuoteNumber.Substring(0, 22);
                            }
                        }
                        return objAutoNumber;
                    }
                    else
                    {
                        return objAutoNumber;
                    }
                }
                else
                {
                    return objAutoNumber;
                }

                #endregion


                #region From Room
                objRoomDAL = new RoomDAL(base.DataBaseName);
                objDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,QuoteAutoSequence,NextQuoteNo,QuoteAutoNrFixedValue"; //"ID,RoomName,POAutoSequence,NextOrderNo,POAutoNrFixedValue";
                obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                if (obj.QuoteAutoSequence.HasValue)
                {
                    objAutoNumber.QuoteGeneratedFrom = "Room";
                    objAutoNumber.QuoteNumberFormateType = obj.QuoteAutoSequence.GetValueOrDefault(0);
                    
                    if (LastUsedTemp != null && LastUsedTemp.LastUsedTempIncrementNumberRoom > 0)
                        obj.NextQuoteNo = Convert.ToString(LastUsedTemp.LastUsedTempIncrementNumberRoom);

                    switch (obj.QuoteAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            objAutoNumber.IsBlank = true;
                            break;
                        case 3: //Increamenting by Order#
                            long.TryParse(obj.NextQuoteNo, out iNextNo);
                            iNextNo += 1;
                            objAutoNumber.QuoteNumber = Convert.ToString(iNextNo).Length > 22 ? Convert.ToString(iNextNo).Substring(0, 22) : Convert.ToString(iNextNo);
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            objAutoNumber.QuoteNumberForSorting = objAutoNumber.QuoteNumber.ToString().PadLeft(11, '0');
                            break;
                        case 4: //Increamenting by Day
                            long.TryParse(obj.NextQuoteNo, out iNextNo);
                            QuoteMasterDAL quoteDAL = new QuoteMasterDAL(base.DataBaseName);
                            QuoteMasterDTO objOrdDTO = quoteDAL.GetLatestQuoteByRoomIdPlain(RoomID, CompanyID);
                            if (objOrdDTO == null || objOrdDTO.Created.DayOfYear < DateTime.Now.DayOfYear)
                            {
                                iNextNo += 1;
                            }
                            objAutoNumber.QuoteNumber = iNextNo.ToString().Length > 22 ? iNextNo.ToString().Substring(0, 22) : iNextNo.ToString();
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;                            
                            objAutoNumber.QuoteNumberForSorting = objAutoNumber.QuoteNumber.ToString().PadLeft(11, '0');
                            break;
                        case 5: //Date + Incrementing#
                            long.TryParse(obj.NextQuoteNo, out iNextNo);
                            iNextNo += 1;
                            objAutoNumber.QuoteNumber = Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Length > 22 ? Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString()).Substring(0, 22) : Convert.ToString(datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString());
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            objAutoNumber.QuoteNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 6: //Date
                            objAutoNumber.QuoteNumber = datetimetoConsider.ToString(strDateFormat).Length > 22 ? datetimetoConsider.ToString(strDateFormat).Substring(0, 22) : datetimetoConsider.ToString(strDateFormat);
                            objAutoNumber.QuoteNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                            break;
                        case 7: //Fixed + Incrementing #
                            long.TryParse(obj.NextQuoteNo, out iNextNo);
                            iNextNo += 1;
                            objAutoNumber.QuoteNumber = obj.QuoteAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.QuoteNumber = objAutoNumber.QuoteNumber.Length > 22 ? objAutoNumber.QuoteNumber.Substring(0, 22) : objAutoNumber.QuoteNumber;
                            objAutoNumber.LastUsedTempIncrementNumberRoom = iNextNo;
                            objAutoNumber.QuoteNumberForSorting = obj.QuoteAutoNrFixedValue + iNextNo.ToString().PadLeft(3, '0'); ;
                            break;
                        case 8: //Date  + Incrementing #+Fixed
                            string QuoteNumberDateFormat = (!string.IsNullOrEmpty(SiteSettingHelper.OrderNumberDateFormat) && !string.IsNullOrWhiteSpace(SiteSettingHelper.OrderNumberDateFormat)) 
                                                            ? SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                            long.TryParse(obj.NextQuoteNo, out iNextNo);
                            iNextNo += 1;
                            objAutoNumber.QuoteNumber = datetimetoConsider.ToString(QuoteNumberDateFormat) + "-" + obj.QuoteAutoNrFixedValue + "-" + iNextNo.ToString();
                            objAutoNumber.QuoteNumber = objAutoNumber.QuoteNumber.Length > 22 ? objAutoNumber.QuoteNumber.Substring(0, 22) : objAutoNumber.QuoteNumber;

                            if (LastUsedTemp != null)
                                objAutoNumber.LastUsedTempIncrementNumberRoom = LastUsedTemp.LastUsedTempIncrementNumberRoom;

                            objAutoNumber.QuoteNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "-" + obj.QuoteAutoNrFixedValue + "-" + "000" + iNextNo.ToString().PadLeft(3, '0');
                            break;
                    }

                    if (objAutoNumber != null)
                    {
                        if (!string.IsNullOrEmpty(objAutoNumber.QuoteNumber) && objAutoNumber.QuoteNumber.Length > 22)
                        {
                            objAutoNumber.QuoteNumber = objAutoNumber.QuoteNumber.Substring(0, 22);
                        }
                    }

                    return objAutoNumber;
                }
                #endregion

                objAutoNumber.ErrorDescription = MsgPOSequanceNotDefineValidation;
                
                if (objAutoNumber != null)
                {
                    if (!string.IsNullOrEmpty(objAutoNumber.QuoteNumber) && objAutoNumber.QuoteNumber.Length > 22)
                    {
                        objAutoNumber.QuoteNumber = objAutoNumber.QuoteNumber.Substring(0, 22);
                    }
                }
                return objAutoNumber;
            }
            finally
            {
                objAutoNumber = null;
            }

        }

        public void UpdateNextQuoteNumber(Int64 RoomID, Int64 CompanyID, string QuoteNumber, Int64 SupplierID, long SessionUserId, string ReleaseNumber = "")
        {
            AutoQuoteNumberGenerate objAutoNumber = null;

            RoomDTO obj = null;
            RoomDAL objDAL = null;
            CommonDAL objCommonDAL = null;
            SupplierMasterDAL objSupplierDAL = null;
            SupplierMasterDTO objSupplierDTO = null;

            Int64 intNextAutoNumber = 0;
            try
            {
                objAutoNumber = new AutoQuoteNumberGenerate();

                #region From Supplier

                if (SupplierID > 0)
                {
                    objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(SupplierID);
                    if (objSupplierDTO != null && objSupplierDTO.QuoteAutoSequence.HasValue)
                    {
                        objAutoNumber.QuoteGeneratedFrom = "Supplier";
                        objAutoNumber.QuoteNumberFormateType = objSupplierDTO.QuoteAutoSequence.GetValueOrDefault(0);

                        switch (objSupplierDTO.QuoteAutoSequence.GetValueOrDefault(0))
                        {
                            case 1://Fix PO
                                if (!string.IsNullOrWhiteSpace(ReleaseNumber) && (!string.IsNullOrWhiteSpace(QuoteNumber)))
                                {
                                    Int64.TryParse(ReleaseNumber, out intNextAutoNumber);
                                    if (intNextAutoNumber > 0)
                                    {
                                        Int64 CurrentAutoNumber = 0;
                                        Int64.TryParse(objSupplierDTO.QuoteAutoNrReleaseNumber, out CurrentAutoNumber);
                                        if (intNextAutoNumber == (CurrentAutoNumber + 1) && objSupplierDTO.NextQuoteNo.Trim() == QuoteNumber.Trim())
                                        {
                                            objSupplierDTO.QuoteAutoNrReleaseNumber = intNextAutoNumber.ToString();
                                            objSupplierDTO.IsOnlyFromItemUI = true;
                                            objSupplierDTO.EditedFrom = "Web";
                                            objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                        }
                                    }
                                }
                                return;
                            case 2: //Blanket PO
                                return;
                            case 6: //Date Only
                                return;
                            case 3: //Increamenting by Order#
                                Int64.TryParse(QuoteNumber, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(objSupplierDTO.NextQuoteNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        objSupplierDTO.NextQuoteNo = intNextAutoNumber.ToString();
                                        objSupplierDTO.IsOnlyFromItemUI = true;
                                        objSupplierDTO.EditedFrom = "Web";
                                        objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                    }
                                }
                                return;
                            case 4: //Increamenting by Day
                                Int64.TryParse(QuoteNumber, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(objSupplierDTO.NextQuoteNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        objSupplierDTO.NextQuoteNo = intNextAutoNumber.ToString();
                                        objSupplierDTO.IsOnlyFromItemUI = true;
                                        objSupplierDTO.EditedFrom = "Web";
                                        objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                    }
                                }
                                return;
                            case 5: //Date + Incrementing#
                                if (QuoteNumber.LastIndexOf("-") > 0 && QuoteNumber.LastIndexOf("-") < QuoteNumber.Trim().Length)
                                {
                                    string incrNo = QuoteNumber.Remove(0, QuoteNumber.LastIndexOf("-") + 1);
                                    Int64.TryParse(incrNo, out intNextAutoNumber);
                                    if (intNextAutoNumber > 0)
                                    {
                                        Int64 CurrentAutoNumber = 0;
                                        Int64.TryParse(objSupplierDTO.NextQuoteNo, out CurrentAutoNumber);
                                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                        {
                                            objSupplierDTO.NextQuoteNo = intNextAutoNumber.ToString();
                                            objSupplierDTO.IsOnlyFromItemUI = true;
                                            objSupplierDTO.EditedFrom = "Web";
                                            objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                        }
                                    }
                                }
                                return;
                            case 7: //Fixed + Incrementing #
                                if (QuoteNumber.LastIndexOf("-") > 0 && QuoteNumber.LastIndexOf("-") < QuoteNumber.Trim().Length)
                                {
                                    string incrNo = QuoteNumber.Remove(0, QuoteNumber.LastIndexOf("-") + 1);
                                    Int64.TryParse(incrNo, out intNextAutoNumber);
                                    if (intNextAutoNumber > 0)
                                    {
                                        Int64 CurrentAutoNumber = 0;
                                        Int64.TryParse(objSupplierDTO.NextQuoteNo, out CurrentAutoNumber);
                                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                        {
                                            objSupplierDTO.NextQuoteNo = intNextAutoNumber.ToString();
                                            objSupplierDTO.IsOnlyFromItemUI = true;
                                            objSupplierDTO.EditedFrom = "Web";
                                            objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                        }
                                    }
                                }
                                return;
                            case 8: //Date  + Incrementing #+ Fixed
                                    //  if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                                if (objSupplierDTO != null && objSupplierDTO.QuoteAutoNrFixedValue != null)
                                {
                                    //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                                    //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                    if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                                    {
                                        //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                    }
                                    else
                                    {
                                        //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                                    }

                                    string OrderNumberDateFormat = (DTO.SiteSettingHelper.QuoteNumberDateFormat != string.Empty && DTO.SiteSettingHelper.QuoteNumberDateFormat != null) ? DTO.SiteSettingHelper.QuoteNumberDateFormat : "yyMMdd";
                                    string incrNo = QuoteNumber.Replace(objSupplierDTO.QuoteAutoNrFixedValue, string.Empty);
                                    if (incrNo.Length > OrderNumberDateFormat.Length)
                                        incrNo = incrNo.Remove(0, OrderNumberDateFormat.Length);

                                    incrNo = incrNo.Replace("--", "");
                                    Int64.TryParse(incrNo, out intNextAutoNumber);
                                    if (intNextAutoNumber > 0)
                                    {
                                        Int64 CurrentAutoNumber = 0;
                                        Int64.TryParse(objSupplierDTO.NextQuoteNo, out CurrentAutoNumber);
                                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                        {
                                            objSupplierDTO.NextQuoteNo = intNextAutoNumber.ToString();
                                            objSupplierDTO.IsOnlyFromItemUI = true;
                                            objSupplierDTO.EditedFrom = "Web";
                                            objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objSupplierDAL.Edit(objSupplierDTO, SessionUserId);
                                        }
                                    }
                                }
                                return;
                        }

                    }
                }
                #endregion

                #region From Room
                objDAL = new RoomDAL(base.DataBaseName);
                objCommonDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,QuoteAutoSequence,NextQuoteNo,QuoteAutoNrFixedValue";
                obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

                if (obj.QuoteAutoSequence.HasValue)
                {
                    objAutoNumber.QuoteGeneratedFrom = "Room";
                    objAutoNumber.QuoteNumberFormateType = obj.QuoteAutoSequence.GetValueOrDefault(0);

                    switch (obj.QuoteAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            return;
                        case 6: //Date Only
                            return;
                        case 3: //Increamenting by Order#
                            Int64.TryParse(QuoteNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse(obj.NextQuoteNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextQuoteNo = Convert.ToString(intNextAutoNumber);
                                    objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextQuoteNo, "NextQuoteNo");
                                }
                            }
                            return;
                        case 4: //Increamenting by day#
                            Int64.TryParse(QuoteNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse(obj.NextQuoteNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextQuoteNo = Convert.ToString(intNextAutoNumber);
                                    objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextQuoteNo, "NextQuoteNo");
                                }
                            }
                            return;
                        case 5: //Date + Incrementing#
                            if (QuoteNumber.LastIndexOf("-") > 0 && QuoteNumber.LastIndexOf("-") < QuoteNumber.Trim().Length)
                            {
                                string incrNo = QuoteNumber.Remove(0, QuoteNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(obj.NextQuoteNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextQuoteNo = Convert.ToString(intNextAutoNumber);
                                        objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextQuoteNo, "NextQuoteNo");
                                    }
                                }
                            }
                            return;
                        case 7: //Fixed + Incrementing #
                            if (QuoteNumber.LastIndexOf("-") > 0 && QuoteNumber.LastIndexOf("-") < QuoteNumber.Trim().Length)
                            {
                                string incrNo = QuoteNumber.Remove(0, QuoteNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(obj.NextQuoteNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextQuoteNo = Convert.ToString(intNextAutoNumber);
                                        objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextQuoteNo, "NextQuoteNo");
                                    }
                                }
                            }
                            return;
                        case 8: //Date  + Incrementing #+ Fixed
                            if (obj != null && obj.QuoteAutoNrFixedValue != null)
                            {
                               string QuoteNumberDateFormat = (DTO.SiteSettingHelper.QuoteNumberDateFormat != string.Empty && DTO.SiteSettingHelper.QuoteNumberDateFormat != null)? DTO.SiteSettingHelper.QuoteNumberDateFormat : "yyMMdd";
                               string incrNo = QuoteNumber.Replace(obj.QuoteAutoNrFixedValue, string.Empty);
                               if (incrNo.Length > QuoteNumberDateFormat.Length)
                                    incrNo = incrNo.Remove(0, QuoteNumberDateFormat.Length);

                                incrNo = incrNo.Replace("--", "");
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(obj.NextQuoteNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextQuoteNo = Convert.ToString(intNextAutoNumber);
                                        objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextQuoteNo, "NextQuoteNo");
                                    }
                                }
                            }
                            return;
                    }
                }
                #endregion

            }
            finally
            {
                objAutoNumber = null;
                obj = null;
                objDAL = null;
               
            }

        }
        public void UpdateNextToolAssetOrderNumber(Int64 RoomID, Int64 CompanyID, string OrderNumber)
        {
            AutoOrderNumberGenerate objAutoNumber = null;

            RoomDTO obj = null;
            RoomDAL objDAL = null;
            CommonDAL objCommonDAL = null;
            Int64 intNextAutoNumber = 0;
            try
            {
                objAutoNumber = new AutoOrderNumberGenerate();



                #region From Room
                objDAL = new RoomDAL(base.DataBaseName);
                objCommonDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,TAOAutoSequence,NextToolAssetOrderNo,POAutoNrFixedValue";
                obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.TAOAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.TAOAutoSequence.GetValueOrDefault(0);

                    switch (obj.TAOAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            return;
                        case 6: //Date Only
                            return;
                        case 3: //Increamenting by Order#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse(obj.NextToolAssetOrderNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextToolAssetOrderNo = Convert.ToString(intNextAutoNumber);
                                    //objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextToolAssetOrderNo, "NextToolAssetOrderNo");
                                }
                            }
                            return;
                        case 4: //Increamenting by day#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse(obj.NextToolAssetOrderNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextToolAssetOrderNo = Convert.ToString(intNextAutoNumber);
                                    //objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextToolAssetOrderNo, "NextToolAssetOrderNo");
                                }
                            }
                            return;
                        case 5: //Date + Incrementing#
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(obj.NextToolAssetOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextToolAssetOrderNo = Convert.ToString(intNextAutoNumber);
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextToolAssetOrderNo, "NextToolAssetOrderNo");
                                    }
                                }
                            }
                            return;
                        case 7: //Fixed + Incrementing #
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(obj.NextToolAssetOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextToolAssetOrderNo = Convert.ToString(intNextAutoNumber);
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextToolAssetOrderNo, "NextToolAssetOrderNo");
                                    }
                                }
                            }
                            return;
                        case 8: //Date  + Incrementing #+ Fixed
                            //if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            if (obj != null && obj.POAutoNrFixedValue != null)
                            {
                                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                                //string incrNo = OrderNumber.Replace(obj.POAutoNrFixedValue, string.Empty).Remove(0, OrderNumberDateFormat.Length);
                                string incrNo = OrderNumber.Replace(obj.POAutoNrFixedValue, string.Empty);
                                if (incrNo.Length > OrderNumberDateFormat.Length)
                                    incrNo = incrNo.Remove(0, OrderNumberDateFormat.Length);

                                incrNo = incrNo.Replace("--", "");
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(obj.NextOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextToolAssetOrderNo = Convert.ToString(intNextAutoNumber);
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextToolAssetOrderNo, "NextToolAssetOrderNo");
                                    }
                                }
                            }
                            return;
                    }


                }
                #endregion

            }
            finally
            {
                objAutoNumber = null;
                obj = null;
                objDAL = null;
            }

        }

        public void UpdateNextWorkOrderNumber(Int64 RoomID, Int64 CompanyID, Int64 SupplierID, string OrderNumber)
        {
            AutoOrderNumberGenerate objAutoNumber = null;

            RoomDTO obj = null;
            RoomDAL objDAL = null;
            CommonDAL objCommonDAL = null;
            Int64 intNextAutoNumber = 0;
            try
            {
                objAutoNumber = new AutoOrderNumberGenerate();

                #region From Room
                objDAL = new RoomDAL(base.DataBaseName);
                objCommonDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,WorkOrderAutoSequence,NextWorkOrderNo,WorkOrderAutoNrFixedValue";
                obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.WorkOrderAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.WorkOrderAutoSequence.GetValueOrDefault(0);

                    switch (obj.WorkOrderAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            return;
                        case 6: //Date Only
                            return;
                        case 3: //Increamenting by Order#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextWorkOrderNo = intNextAutoNumber;
                                    //objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextWorkOrderNo, "NextWorkOrderNo");
                                }
                            }
                            return;
                        case 4: //Increamenting by day#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextWorkOrderNo = intNextAutoNumber;
                                    //objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextWorkOrderNo, "NextWorkOrderNo");
                                }
                            }
                            return;
                        case 5: //Date + Incrementing#
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextWorkOrderNo = intNextAutoNumber;
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextWorkOrderNo, "NextWorkOrderNo");
                                    }
                                }
                            }
                            return;
                        case 7: //Fixed + Incrementing #
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextWorkOrderNo = intNextAutoNumber;
                                        // objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextWorkOrderNo, "NextWorkOrderNo");
                                    }
                                }
                            }
                            return;
                        case 8: //Date  + Incrementing #+ Fixed
                            //if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            if (obj != null && obj.WorkOrderAutoNrFixedValue != null)
                            {
                                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                                //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                }
                                else
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                                }

                                string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                                string incrNo = OrderNumber.Replace(obj.WorkOrderAutoNrFixedValue, string.Empty).Remove(0, OrderNumberDateFormat.Length);
                                incrNo = incrNo.Replace("--", "");
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse((obj.NextWorkOrderNo ?? 0).ToString(), out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextWorkOrderNo = intNextAutoNumber;
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextWorkOrderNo, "NextWorkOrderNo");
                                    }
                                }
                            }
                            return;
                    }


                }
                #endregion

            }
            finally
            {
                objAutoNumber = null;
                obj = null;
                objDAL = null;
            }

        }

        public void UpdateNextTransferNumber(Int64 RoomID, Int64 CompanyID, string OrderNumber)
        {
            AutoOrderNumberGenerate objAutoNumber = null;

            RoomDTO obj = null;
            RoomDAL objDAL = null;
            CommonDAL objCommonDAL = null;
            Int64 intNextAutoNumber = 0;
            try
            {
                objAutoNumber = new AutoOrderNumberGenerate();

                #region From Room
                objDAL = new RoomDAL(base.DataBaseName);
                objCommonDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,TransferAutoSequence,NextTransferNo,TransferAutoNrFixedValue";
                obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.TransferAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.TransferAutoSequence.GetValueOrDefault(0);

                    switch (obj.TransferAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            return;
                        case 6: //Date Only
                            return;
                        case 3: //Increamenting by Order#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextTransferNo = intNextAutoNumber;
                                    //objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextTransferNo, "NextTransferNo");
                                }
                            }
                            return;
                        case 4: //Increamenting by day#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextTransferNo = intNextAutoNumber;
                                    //objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextTransferNo, "NextTransferNo");
                                }
                            }
                            return;
                        case 5: //Date + Incrementing#
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextTransferNo = intNextAutoNumber;
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextTransferNo, "NextTransferNo");
                                    }
                                }
                            }
                            return;
                        case 7: //Fixed + Incrementing #
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextTransferNo = intNextAutoNumber;
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextTransferNo, "NextTransferNo");
                                    }
                                }
                            }
                            return;
                        case 8: //Date  + Incrementing #+ Fixed
                            //if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            if (obj != null && obj.TransferAutoNrFixedValue != null)
                            {
                                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                                //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                }
                                else
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                                }

                                string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                                string incrNo = OrderNumber.Replace(obj.TransferAutoNrFixedValue, string.Empty).Remove(0, OrderNumberDateFormat.Length);
                                incrNo = incrNo.Replace("--", "");
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse((obj.NextTransferNo ?? 0).ToString(), out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextTransferNo = intNextAutoNumber;
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextTransferNo, "NextTransferNo");
                                    }
                                }
                            }
                            return;
                    }


                }
                #endregion

            }
            finally
            {
                objAutoNumber = null;
                obj = null;
                objDAL = null;
            }

        }

        public void UpdateNextStagingNumber(Int64 RoomID, Int64 CompanyID, string OrderNumber)
        {
            AutoOrderNumberGenerate objAutoNumber = null;

            RoomDTO obj = null;
            RoomDAL objDAL = null;
            CommonDAL objCommonDAL = null;
            Int64 intNextAutoNumber = 0;
            try
            {
                objAutoNumber = new AutoOrderNumberGenerate();

                #region From Room
                objDAL = new RoomDAL(base.DataBaseName);
                objCommonDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,StagingAutoSequence,NextStagingNo,StagingAutoNrFixedValue";
                obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.StagingAutoSequence.HasValue)
                {
                    objAutoNumber.OrderGeneratedFrom = "Room";
                    objAutoNumber.OrderNumberFormateType = obj.StagingAutoSequence.GetValueOrDefault(0);

                    switch (obj.StagingAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            return;
                        case 6: //Date Only
                            return;
                        case 3: //Increamenting by Order#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextStagingNo = intNextAutoNumber;
                                    // objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextStagingNo, "NextStagingNo");
                                }
                            }
                            return;
                        case 4: //Increamenting by day#
                            Int64.TryParse(OrderNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextStagingNo = intNextAutoNumber;
                                    // objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextStagingNo, "NextStagingNo");
                                }
                            }
                            return;
                        case 5: //Date + Incrementing#
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextStagingNo = intNextAutoNumber;
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextStagingNo, "NextStagingNo");
                                    }
                                }
                            }
                            return;
                        case 7: //Fixed + Incrementing #
                            if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            {
                                string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextStagingNo = intNextAutoNumber;
                                        //objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextStagingNo, "NextStagingNo");
                                    }
                                }
                            }
                            return;
                        case 8: //Date  + Incrementing #+ Fixed
                            //if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                            if (obj != null && obj.StagingAutoNrFixedValue != null)
                            {
                                //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));

                                //System.Xml.Linq.XElement Settinfile = null;//System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                                }
                                else
                                {
                                    //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                                }

                                string OrderNumberDateFormat = DTO.SiteSettingHelper.OrderNumberDateFormat != string.Empty ? DTO.SiteSettingHelper.OrderNumberDateFormat : "yyMMdd";
                                string incrNo = OrderNumber.Replace(obj.StagingAutoNrFixedValue, string.Empty).Remove(0, OrderNumberDateFormat.Length);
                                incrNo = incrNo.Replace("--", "");
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse((obj.NextStagingNo ?? 0).ToString(), out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextStagingNo = intNextAutoNumber;
                                        // objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextStagingNo, "NextStagingNo");
                                    }
                                }
                            }
                            return;
                    }


                }
                #endregion

            }
            finally
            {
                objAutoNumber = null;
                obj = null;
                objDAL = null;
            }

        }

        public void UpdateNextRequisitionNumber(Int64 RoomID, Int64 CompanyID, Int64 SupplierID, string RequisitionNumber)
        {
            AutoOrderNumberGenerate objAutoNumber = null;

            RoomDTO obj = null;
            RoomDAL objDAL = null;
            CommonDAL objCommonDAL = null;
            Int64 intNextAutoNumber = 0;
            try
            {
                objAutoNumber = new AutoOrderNumberGenerate();

                #region From Supplier

                /*if (SupplierID > 0)
                {
                    objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                    objSupplierDTO = objSupplierDAL.GetSupplierByID(SupplierID, RoomID, CompanyID, false);
                    if (objSupplierDTO != null && objSupplierDTO.POAutoSequence.HasValue)
                    {
                        objAutoNumber.OrderGeneratedFrom = "Supplier";
                        objAutoNumber.OrderNumberFormateType = objSupplierDTO.POAutoSequence.GetValueOrDefault(0);

                        switch (objSupplierDTO.POAutoSequence.GetValueOrDefault(0))
                        {
                            case 1://Fix PO
                                return;
                            case 2: //Blanket PO
                                return;
                            case 6: //Date Only
                                return;
                            case 3: //Increamenting by Order#
                                Int64.TryParse(OrderNumber, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(objSupplierDTO.NextOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        objSupplierDTO.NextOrderNo = intNextAutoNumber.ToString();
                                        objSupplierDTO.IsOnlyFromItemUI = true;
                                        objSupplierDTO.EditedFrom = "Web";
                                        objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objSupplierDAL.Edit(objSupplierDTO);
                                    }
                                }
                                return;
                            case 4: //Increamenting by Day
                                Int64.TryParse(OrderNumber, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    Int64.TryParse(objSupplierDTO.NextOrderNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        objSupplierDTO.NextOrderNo = intNextAutoNumber.ToString();
                                        objSupplierDTO.IsOnlyFromItemUI = true;
                                        objSupplierDTO.EditedFrom = "Web";
                                        objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objSupplierDAL.Edit(objSupplierDTO);
                                    }
                                }
                                return;
                            case 5: //Date + Incrementing#
                                if (OrderNumber.LastIndexOf("-") > 0 && OrderNumber.LastIndexOf("-") < OrderNumber.Trim().Length)
                                {
                                    string incrNo = OrderNumber.Remove(0, OrderNumber.LastIndexOf("-") + 1);
                                    Int64.TryParse(incrNo, out intNextAutoNumber);
                                    if (intNextAutoNumber > 0)
                                    {
                                        Int64 CurrentAutoNumber = 0;
                                        Int64.TryParse(objSupplierDTO.NextOrderNo, out CurrentAutoNumber);
                                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                        {
                                            objSupplierDTO.NextOrderNo = intNextAutoNumber.ToString();
                                            objSupplierDTO.IsOnlyFromItemUI = true;
                                            objSupplierDTO.EditedFrom = "Web";
                                            objSupplierDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            objSupplierDAL.Edit(objSupplierDTO);
                                        }
                                    }
                                }
                                return;
                        }

                    }
                }*/
                #endregion

                #region From Room
                objDAL = new RoomDAL(base.DataBaseName);
                objCommonDAL = new CommonDAL(base.DataBaseName);
                string columnList = "ID,RoomName,ReqAutoSequence,NextRequisitionNo";
                obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj.ReqAutoSequence.HasValue)
                {
                    objAutoNumber.RequisitionGeneratedFrom = "Room";
                    objAutoNumber.RequisitionFormateType = obj.ReqAutoSequence.GetValueOrDefault(0);

                    switch (obj.ReqAutoSequence.GetValueOrDefault(0))
                    {
                        case 0: //Blank
                            return;
                        case 6: //Date Only
                            return;
                        case 3: //Increamenting by Order#
                            Int64.TryParse(RequisitionNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                if (obj.NextRequisitionNo != null)
                                    CurrentAutoNumber = Convert.ToInt64(obj.NextRequisitionNo);
                                //Int64.TryParse(obj.NextRequisitionNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextRequisitionNo = intNextAutoNumber;
                                    //objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextRequisitionNo, "NextRequisitionNo");
                                }
                            }
                            return;
                        case 4: //Increamenting by day#
                            Int64.TryParse(RequisitionNumber, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                CurrentAutoNumber = Convert.ToInt64(obj.NextRequisitionNo);
                                //Int64.TryParse(obj.NextOrderNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextRequisitionNo = intNextAutoNumber;
                                    // objDAL.Edit(obj);
                                    objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextRequisitionNo, "NextRequisitionNo");
                                }
                            }
                            return;
                        case 5: //Date + Incrementing#
                            if (RequisitionNumber.LastIndexOf("-") > 0 && RequisitionNumber.LastIndexOf("-") < RequisitionNumber.Trim().Length)
                            {
                                string incrNo = RequisitionNumber.Remove(0, RequisitionNumber.LastIndexOf("-") + 1);
                                Int64.TryParse(incrNo, out intNextAutoNumber);
                                if (intNextAutoNumber > 0)
                                {
                                    Int64 CurrentAutoNumber = 0;
                                    CurrentAutoNumber = Convert.ToInt64(obj.NextRequisitionNo);
                                    //Int64.TryParse(obj.NextRequisitionNo, out CurrentAutoNumber);
                                    if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                    {
                                        obj.NextRequisitionNo = intNextAutoNumber;
                                        // objDAL.Edit(obj);
                                        objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextRequisitionNo, "NextRequisitionNo");
                                    }
                                }
                            }
                            return;
                    }
                }
                #endregion

            }
            finally
            {
                objAutoNumber = null;
                obj = null;
                objDAL = null;
            }

        }

        /// <summary>
        /// Get Next Auto Number By Module
        /// </summary>
        /// <param name="type"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public string GetNextAutoNumberByModule(string type, Int64 RoomID, Int64 CompanyID)
        {
            RoomDTO obj = null;
            RoomDAL objDAL = new RoomDAL(base.DataBaseName);
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            string iReturnNext = string.Empty;

            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;
            #region DateFormate From RegionalSetting

            string strDateFormat = "MM/dd/yyyy";
            string returnString = string.Empty;
            objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

            if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
            {
                strDateFormat = "yyyy/MM/dd";
            }

            objRegionSettingDAL = null;
            #endregion

            #region [Regional Settings]
            if (objeTurnsRegionInfo != null)
            {
                strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

            }
            #endregion

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string columnList = "ID,RoomName,NextRequisitionNo,NextWorkOrderNo,NextTransferNo,CountAutoSequence,NextCountNo,NextProjectSpendNo,NextToolNo,NextAssetNo,NextBinNo,NextStagingNo";
                obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //obj = objDAL.GetRoomByIDPlain(RoomID);
                if (obj != null)
                {
                    if (type.ToLower() == "nextrequisitionno")
                    {
                        iReturnNext = Convert.ToString(obj.NextRequisitionNo.GetValueOrDefault(0) + 1);
                    }
                    else if (type.ToLower() == "nextworkorderno")
                    {
                        iReturnNext = Convert.ToString(obj.NextWorkOrderNo.GetValueOrDefault(0) + 1);
                    }
                    else if (type.ToLower() == "nexttransferno")
                    {
                        iReturnNext = Convert.ToString(obj.NextTransferNo.GetValueOrDefault(0) + 1);
                    }
                    else if (type.ToLower() == "nextcountnumber")
                    {
                        Int64 iNextNo = 0;
                        switch (obj.CountAutoSequence.GetValueOrDefault(0))
                        {
                            case 0:
                                iReturnNext = string.Empty;
                                break;
                            case 3: //Increamenting by Count#
                                Int64.TryParse(Convert.ToString(obj.NextCountNo), out iNextNo);
                                iNextNo = iNextNo + 1;
                                iReturnNext = Convert.ToString(iNextNo);
                                //obj.NextCountNo = Convert.ToString(iNextNo);
                                break;
                            case 5: //Date + Incrementing#
                                // iNextNo = obj.NextOrderNo.GetValueOrDefault(0) + 1;
                                Int64.TryParse(obj.NextCountNo, out iNextNo);
                                iNextNo = iNextNo + 1;
                                iReturnNext = datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString();
                                break;
                            case 6: //Date
                                iReturnNext = datetimetoConsider.ToString(strDateFormat);
                                break;
                        }
                        // iReturnNext = Convert.ToString(obj.NextCountNo.GetValueOrDefault(0) + 1);
                    }
                    else if (type.ToLower() == "nextprojectspendno")
                    {
                        iReturnNext = Convert.ToString(obj.NextProjectSpendNo.GetValueOrDefault(0) + 1);
                    }
                    else if (type.ToLower() == "nexttoolno")
                    {
                        iReturnNext = Convert.ToString(obj.NextToolNo.GetValueOrDefault(0) + 1);
                    }
                    else if (type.ToLower() == "nextassetno")
                    {
                        iReturnNext = Convert.ToString(obj.NextAssetNo.GetValueOrDefault(0) + 1);
                    }
                    else if (type.ToLower() == "nextbinno")
                    {
                        iReturnNext = Convert.ToString(obj.NextBinNo.GetValueOrDefault(0) + 1);
                    }
                    else if (type.ToLower() == "nextstagingno")
                    {
                        iReturnNext = Convert.ToString(obj.NextStagingNo.GetValueOrDefault(0) + 1);
                    }

                }
            }

            return iReturnNext;
        }

        /// <summary>
        /// Update Room Detail For Next AutoNumber By Module
        /// </summary>
        /// <param name="type"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="NextAutoNumber"></param>
        public void UpdateRoomDetailForNextAutoNumberByModule(string type, Int64 RoomID, Int64 CompanyID, string NextAutoNumber)
        {
            RoomDTO obj = null;
            RoomDAL objDAL = new RoomDAL(base.DataBaseName);
            CommonDAL objCommonDAL = new CommonDAL(base.DataBaseName);
            try
            {
                if (type.ToLower() == "nextrequisitionno")
                {
                    Int64 intNextAutoNumber = 0;
                    Int64.TryParse(NextAutoNumber, out intNextAutoNumber);
                    if (intNextAutoNumber > 0)
                    {
                        string columnList = "ID,RoomName,NextRequisitionNo";
                        obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                        //obj = objDAL.GetRoomByIDPlain(RoomID);
                        Int64 CurrentAutoNumber = obj.NextRequisitionNo.GetValueOrDefault(0);
                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                        {
                            obj.NextRequisitionNo = intNextAutoNumber;
                            //objDAL.Edit(obj);
                            objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextRequisitionNo, "NextRequisitionNo");
                        }
                    }
                }
                else if (type.ToLower() == "nextworkorderno")
                {
                    Int64 intNextAutoNumber = 0;
                    Int64.TryParse(NextAutoNumber, out intNextAutoNumber);
                    if (intNextAutoNumber > 0)
                    {
                        string columnList = "ID,RoomName,NextWorkOrderNo";
                        obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                        //obj = objDAL.GetRoomByIDPlain(RoomID);
                        Int64 CurrentAutoNumber = obj.NextWorkOrderNo.GetValueOrDefault(0);
                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                        {
                            obj.NextWorkOrderNo = intNextAutoNumber;
                            // objDAL.Edit(obj);
                            objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextWorkOrderNo, "NextWorkOrderNo");
                        }
                    }
                }
                else if (type.ToLower() == "nexttransferno")
                {
                    Int64 intNextAutoNumber = 0;
                    Int64.TryParse(NextAutoNumber, out intNextAutoNumber);
                    if (intNextAutoNumber > 0)
                    {
                        string columnList = "ID,RoomName,NextTransferNo";
                        obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                        //obj = objDAL.GetRoomByIDPlain(RoomID);
                        Int64 CurrentAutoNumber = obj.NextTransferNo.GetValueOrDefault(0);
                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                        {
                            obj.NextTransferNo = intNextAutoNumber;
                            //objDAL.Edit(obj);
                            objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextTransferNo, "NextTransferNo");
                        }
                    }
                }
                else if (type.ToLower() == "nextcountnumber")
                {
                    this.UpdateNextCountNumber(RoomID, CompanyID, NextAutoNumber);

                }
                else if (type.ToLower() == "nextprojectspendno")
                {
                    Int64 intNextAutoNumber = 0;
                    Int64.TryParse(NextAutoNumber, out intNextAutoNumber);
                    if (intNextAutoNumber > 0)
                    {
                        string columnList = "ID,RoomName,NextProjectSpendNo";
                        obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                        //obj = objDAL.GetRoomByIDPlain(RoomID);
                        Int64 CurrentAutoNumber = obj.NextProjectSpendNo.GetValueOrDefault(0);
                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                        {
                            obj.NextProjectSpendNo = intNextAutoNumber;
                            // objDAL.Edit(obj);
                            objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextProjectSpendNo, "NextProjectSpendNo");
                        }
                    }
                }
                //else if (type.ToLower() == "nexttoolno")
                //{
                //    Int64 intNextAutoNumber = 0;
                //    Int64.TryParse(NextAutoNumber, out intNextAutoNumber);
                //    if (intNextAutoNumber > 0)
                //    {
                //        string columnList = "ID,RoomName,NextToolNo";
                //        obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //        //obj = objDAL.GetRoomByIDPlain(RoomID);
                //        Int64 CurrentAutoNumber = obj.NextToolNo.GetValueOrDefault(0);
                //        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                //        {
                //            obj.NextToolNo = intNextAutoNumber;
                //            // objDAL.Edit(obj);
                //            objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextToolNo, "NextToolNo");
                //        }
                //    }
                //}
                //else if (type.ToLower() == "nextassetno")
                //{
                //    Int64 intNextAutoNumber = 0;
                //    Int64.TryParse(NextAutoNumber, out intNextAutoNumber);
                //    if (intNextAutoNumber > 0)
                //    {
                //        string columnList = "ID,RoomName,NextAssetNo";
                //        obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //        //obj = objDAL.GetRoomByIDPlain(RoomID);
                //        Int64 CurrentAutoNumber = obj.NextAssetNo.GetValueOrDefault(0);
                //        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                //        {
                //            obj.NextAssetNo = intNextAutoNumber;
                //            // objDAL.Edit(obj);
                //            objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextAssetNo, "NextAssetNo");
                //        }
                //    }
                //}
                //else if (type.ToLower() == "nextbinno")
                //{
                //    Int64 intNextAutoNumber = 0;
                //    Int64.TryParse(NextAutoNumber, out intNextAutoNumber);
                //    if (intNextAutoNumber > 0)
                //    {
                //        string columnList = "ID,RoomName,NextBinNo";
                //        obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                //        //obj = objDAL.GetRoomByIDPlain(RoomID);
                //        Int64 CurrentAutoNumber = obj.NextBinNo.GetValueOrDefault(0);
                //        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                //        {
                //            obj.NextBinNo = intNextAutoNumber;
                //            objDAL.Edit(obj);
                //            objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextBinNo, "NextBinNo");
                //        }
                //    }
                //}
                else if (type.ToLower() == "nextstagingno")
                {
                    Int64 intNextAutoNumber = 0;
                    Int64.TryParse(NextAutoNumber, out intNextAutoNumber);
                    if (intNextAutoNumber > 0)
                    {
                        string columnList = "ID,RoomName,NextStagingNo";
                        obj = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                        //obj = objDAL.GetRoomByIDPlain(RoomID);
                        Int64 CurrentAutoNumber = obj.NextStagingNo.GetValueOrDefault(0);
                        if (intNextAutoNumber == (CurrentAutoNumber + 1))
                        {
                            obj.NextStagingNo = intNextAutoNumber;
                            // objDAL.Edit(obj);
                            objDAL.UpdateNextCounterNumber(RoomID, CompanyID, obj.NextStagingNo, "NextStagingNo");
                        }
                    }
                }
            }
            finally
            {
                obj = null;
                objDAL = null;
            }

        }

        public string GetPONumberBySupplier(SupplierMasterDTO objSupplierDTO, long RoomID, long CompanyID, DateTime datetimetoConsider, string strDateFormat,long userID,long EnterpriseID)
        {
            string poNumber = string.Empty;

            AutoOrderNumberGenerate objAutoNumber = null;
            long iNextNo = 0;
            try
            {
                objAutoNumber = new AutoOrderNumberGenerate();
                string currentCulture = "en-US";
                if (HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(base.DataBaseName).GetRegionSettingsById(RoomID, CompanyID, userID);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResOrder", currentCulture, EnterpriseID, CompanyID);
                string MsgFixedOrderNumberValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgFixedOrderNumberValidation", ResourceFileCommon,EnterpriseID,CompanyID,RoomID, "ResOrder",currentCulture);
                string MsgBlanketNotDefineValidation = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgBlanketNotDefineValidation", ResourceFileCommon, EnterpriseID, CompanyID, RoomID, "ResOrder", currentCulture);
                #region From Supplier

                if (objSupplierDTO.ID > 0)
                {
                    if (objSupplierDTO != null && objSupplierDTO.PullPurchaseNumberType.HasValue)
                    {
                        switch (objSupplierDTO.PullPurchaseNumberType.GetValueOrDefault(0))
                        {

                            case 1: //Fixed

                                if (!string.IsNullOrEmpty(objSupplierDTO.LastPullPurchaseNumberUsed))
                                {
                                    objAutoNumber.OrderNumber = objSupplierDTO.LastPullPurchaseNumberUsed;
                                    objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');
                                }
                                else
                                    objAutoNumber.ErrorDescription = MsgFixedOrderNumberValidation;
                                break;
                            case 2: //Blanket Order #
                                objAutoNumber.IsBlanketPO = true;
                                objAutoNumber.BlanketPOs = new SupplierBlanketPODetailsDAL(base.DataBaseName).GetBlanketPOBySupplierIDPlain(objSupplierDTO.ID, RoomID, CompanyID).ToList();
                                if (objAutoNumber.BlanketPOs == null || objAutoNumber.BlanketPOs.Count <= 0)
                                {
                                    objAutoNumber.ErrorDescription = MsgBlanketNotDefineValidation;
                                    objAutoNumber.BlanketPOs.Insert(0, null);
                                }
                                if (!string.IsNullOrWhiteSpace(objSupplierDTO.LastPullPurchaseNumberUsed))
                                    objAutoNumber.OrderNumber = objSupplierDTO.LastPullPurchaseNumberUsed;
                                else
                                    objAutoNumber.OrderNumber = string.Empty;

                                objAutoNumber.OrderNumberForSorting = objAutoNumber.OrderNumber.ToString().PadLeft(11, '0');

                                if (objAutoNumber.BlanketPOs != null && string.IsNullOrWhiteSpace(objSupplierDTO.LastPullPurchaseNumberUsed) && objAutoNumber.BlanketPOs.Count > 0)
                                {
                                    if (objAutoNumber.BlanketPOs.First() != null)
                                    {
                                        objAutoNumber.OrderNumber = objAutoNumber.BlanketPOs.First().BlanketPO;
                                    }
                                }
                                break;
                            case 5: //Date + Incrementing#
                                Int64.TryParse(objSupplierDTO.LastPullPurchaseNumberUsed, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0');
                                break;
                            case 6: //Date
                                objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat);
                                objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + "000";
                                break;
                            case 7: //Fixed + Incrementing#
                                Int64.TryParse(objSupplierDTO.LastPullPurchaseNumberUsed, out iNextNo);
                                iNextNo = iNextNo + 1;
                                objAutoNumber.OrderNumber = objSupplierDTO.PullPurchaseNrFixedValue + "-" + iNextNo.ToString();
                                objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                objAutoNumber.OrderNumberForSorting = objSupplierDTO.PullPurchaseNrFixedValue + iNextNo.ToString().PadLeft(3, '0');
                                break;
                        }

                    }
                }
                #endregion

            }
            finally
            {
            }

            if (objAutoNumber != null && !string.IsNullOrEmpty(objAutoNumber.OrderNumber))
            {
                poNumber = objAutoNumber.OrderNumber;
            }

            return poNumber ?? string.Empty;

        }

        public List<SupplierPODTO> GetSupplierPOPair(long RoomID, long CompanyID,long userID,long EnterpriceID)
        {

            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
            DateTime datetimetoConsider = DateTime.Now.Date;

            #region DateFormate From CompnayConfig

            string strDateFormat = "MM/dd/yyyy";
            string returnString = string.Empty;

            objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);

            if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
            {
                strDateFormat = "yyyy/MM/dd";
            }

            objRegionSettingDAL = null;
            #endregion

            #region [Regional Settings]

            if (objeTurnsRegionInfo != null)
            {
                strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);
            }

            #endregion

            SupplierMasterDAL objSuppDAL = new SupplierMasterDAL(base.DataBaseName);
            List<SupplierPODTO> supplierPOPair = new List<SupplierPODTO>();
            List<SupplierMasterDTO> supplierList = new List<SupplierMasterDTO>();
            supplierList = objSuppDAL.GetSupplierByRoomPlain(RoomID, CompanyID, false);
            string _poNumber = string.Empty;
            foreach (SupplierMasterDTO objsupplier in supplierList)
            {

                _poNumber = string.Empty;

                _poNumber = GetPONumberBySupplier(objsupplier, RoomID, CompanyID, datetimetoConsider, strDateFormat,userID,EnterpriceID);
                supplierPOPair.Add(new SupplierPODTO { PONumber = _poNumber, SupplierID = objsupplier.ID });//, SupplierName = objsupplier.SupplierName
            }
            return supplierPOPair;
        }


        public string GetAndUpdatePONumber(string PullOrderNumber, long RoomID, long CompanyID, long SupplierID, Guid ItemGUID)
        {
            string strPONumber = PullOrderNumber;
            int currentPOIncNumber = 0;
            bool isValidPOForUpdate = false;
            string[] splitwithdash = PullOrderNumber.Split(new char[1] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitwithdash != null && splitwithdash.Length > 1)
            {
                int.TryParse(splitwithdash[splitwithdash.Length - 1], out currentPOIncNumber);
                if (currentPOIncNumber > 0)
                {
                    isValidPOForUpdate = true;
                }
            }

            if (isValidPOForUpdate)
            {
                AutoOrderNumberGenerate objAutoNumber = null;
                SupplierMasterDTO objSupplierDTO = null;
                SupplierMasterDAL objSupplierDAL = null;
                eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                DateTime datetimetoConsider = DateTime.Now.Date;
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
                try
                {
                    #region DateFormate From CompnayConfig
                    /*
                    string strDateFormat = "MM/dd/yyyy";
                    string returnString = string.Empty;
                    objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(RoomID, CompanyID, 0);
                    if (objeTurnsRegionInfo != null && !string.IsNullOrEmpty(objeTurnsRegionInfo.ShortDatePattern) && objeTurnsRegionInfo.ShortDatePattern.Equals("yy/m/d"))
                    {
                        strDateFormat = "yyyy/MM/dd";
                    }
                    objRegionSettingDAL = null;
                     */
                    #endregion

                    #region [Regional Settings]
                    /*
                    if (objeTurnsRegionInfo != null)
                    {
                        strDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                        objRegionSettingDAL = new RegionSettingDAL(base.DataBaseName);
                        datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(RoomID, CompanyID, 0);

                    }
                    */
                    #endregion

                    objAutoNumber = new AutoOrderNumberGenerate();

                    #region [From Item]
                    /*
                    ItemSupplierDetailsDTO objItemSupplierDetailsDTO = objItemMasterDAL.GetItemSupplierDetails(ItemGUID);
                    if (objItemSupplierDetailsDTO != null)
                    {
                        if (!string.IsNullOrWhiteSpace(objItemSupplierDetailsDTO.BlanketPO))
                        {
                            objAutoNumber.OrderNumber = objItemSupplierDetailsDTO.BlanketPO;
                            return objAutoNumber.OrderNumber;
                        }
                    }
                    */
                    #endregion

                    #region From Supplier

                    if (SupplierID > 0)
                    {
                        objSupplierDAL = new SupplierMasterDAL(base.DataBaseName);
                        objSupplierDTO = objSupplierDAL.GetSupplierByIDPlain(SupplierID);
                        if (objSupplierDTO != null && objSupplierDTO.PullPurchaseNumberType.HasValue)
                        {
                            switch (objSupplierDTO.PullPurchaseNumberType.GetValueOrDefault(0))
                            {

                                case 5: //Date + Incrementing#
                                    /*
                                    Int64.TryParse(objSupplierDTO.LastPullPurchaseNumberUsed, out iNextNo);
                                    iNextNo = iNextNo + 1;
                                    objAutoNumber.OrderNumber = datetimetoConsider.ToString(strDateFormat) + "-" + iNextNo.ToString();
                                    objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                    objAutoNumber.OrderNumberForSorting = datetimetoConsider.ToString("yyyyMMdd") + iNextNo.ToString().PadLeft(3, '0');
                                    objSupplierDAL.UpdatedLastUsedNumber(iNextNo, objSupplierDTO.ID, 0, "supplier");
                                    */
                                    objSupplierDAL.UpdatedLastUsedNumber(currentPOIncNumber, objSupplierDTO.ID, 0, "supplier");
                                    break;
                                case 7: //Fixed + Incrementing#
                                    /*
                                    Int64.TryParse(objSupplierDTO.LastPullPurchaseNumberUsed, out iNextNo);
                                    iNextNo = iNextNo + 1;
                                    objAutoNumber.OrderNumber = objSupplierDTO.PullPurchaseNrFixedValue + "-" + iNextNo.ToString();
                                    objAutoNumber.LastUsedTempIncrementNumberSupplier = iNextNo;
                                    objAutoNumber.OrderNumberForSorting = objSupplierDTO.PullPurchaseNrFixedValue + iNextNo.ToString().PadLeft(3, '0');
                                    objSupplierDAL.UpdatedLastUsedNumber(iNextNo, objSupplierDTO.ID, 0, "supplier");
                                    */

                                    objSupplierDAL.UpdatedLastUsedNumber(currentPOIncNumber, objSupplierDTO.ID, 0, "supplier");

                                    break;
                            }

                        }
                    }
                    #endregion


                }
                finally
                {

                }

                /*
                if (objAutoNumber != null && !string.IsNullOrEmpty(objAutoNumber.OrderNumber))
                {
                    strPONumber = objAutoNumber.OrderNumber;
                }
                */
            }

            return strPONumber;
        }

        public void UpdateNextCountNumberForImport(Int64 RoomID, Int64 CompanyID, string CountNumber)
        {
            RoomDTO obj = null;
            CommonDAL objDAL = null;
            RoomDAL objRoomDAL = null;
            objDAL = new CommonDAL(base.DataBaseName);
            objRoomDAL = new RoomDAL(base.DataBaseName);
            string columnList = "ID,RoomName,CountAutoSequence,NextCountNo,";
            obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            //obj = objDAL.GetRoomByIDPlain(RoomID);
            Int64 intNextAutoNumber = 0;
            if (obj != null && (obj.CountAutoSequence == null || obj.CountAutoSequence <= 0))
            {
                obj.CountAutoSequence = 3;
            }
            if (obj.CountAutoSequence.HasValue)
            {

                switch (obj.CountAutoSequence.GetValueOrDefault(0))
                {

                    case 3: //Increamenting by Count#
                        Int64.TryParse(CountNumber, out intNextAutoNumber);
                        if (intNextAutoNumber > 0)
                        {
                            Int64 CurrentAutoNumber = 0;
                            Int64.TryParse(Convert.ToString(obj.NextCountNo), out CurrentAutoNumber);
                            if (intNextAutoNumber == (CurrentAutoNumber + 1))
                            {
                                obj.NextCountNo = Convert.ToString(intNextAutoNumber);
                                // objDAL.Edit(obj);
                                objRoomDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextCountNo, "NextCountNo");

                            }
                        }
                        return;
                    case 4: //Increamenting by day#
                        Int64.TryParse(CountNumber, out intNextAutoNumber);
                        if (intNextAutoNumber > 0)
                        {
                            Int64 CurrentAutoNumber = 0;
                            Int64.TryParse(obj.NextCountNo, out CurrentAutoNumber);
                            if (intNextAutoNumber == (CurrentAutoNumber + 1))
                            {
                                obj.NextCountNo = Convert.ToString(intNextAutoNumber);
                                // objDAL.Edit(obj);
                                objRoomDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextCountNo, "NextCountNo");
                            }
                        }
                        return;
                    case 5: //Date + Incrementing#
                        if (CountNumber.LastIndexOf("-") > 0 && CountNumber.LastIndexOf("-") < CountNumber.Trim().Length)
                        {
                            string incrNo = CountNumber.Remove(0, CountNumber.LastIndexOf("-") + 1);
                            Int64.TryParse(incrNo, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse(obj.NextCountNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    obj.NextCountNo = Convert.ToString(intNextAutoNumber);
                                    //objDAL.Edit(obj);
                                    objRoomDAL.UpdateNextCounterNumberString(RoomID, CompanyID, obj.NextCountNo, "NextCountNo");
                                }
                            }
                        }
                        return;
                }
            }

        }

        public void UpdateNextToolCountNumberForImport(Int64 RoomID, Int64 CompanyID, string CountNumber)
        {
            RoomDTO obj = null;
            CommonDAL objDAL = null;
            RoomDAL objRoomDAL = null;
            objDAL = new CommonDAL(base.DataBaseName);
            objRoomDAL = new RoomDAL(base.DataBaseName);
            string columnList = "ID,RoomName,ToolCountAutoSequence,NextToolCountNo,";
            obj = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
            Int64 intNextAutoNumber = 0;
            if (obj != null && (obj.ToolCountAutoSequence == null || obj.ToolCountAutoSequence <= 0))
            {
                obj.ToolCountAutoSequence = 3;
            }
            if (obj.ToolCountAutoSequence.HasValue)
            {

                switch (obj.ToolCountAutoSequence.GetValueOrDefault(0))
                {

                    case 3: //Increamenting by Count#
                        Int64.TryParse(CountNumber, out intNextAutoNumber);
                        if (intNextAutoNumber > 0)
                        {
                            Int64 CurrentAutoNumber = 0;
                            Int64.TryParse(Convert.ToString(obj.NextToolCountNo), out CurrentAutoNumber);
                            if (intNextAutoNumber == (CurrentAutoNumber + 1))
                            {
                                objRoomDAL.UpdateNextToolCountNo(RoomID, CompanyID, Convert.ToString(intNextAutoNumber));
                            }
                        }
                        return;
                    case 4: //Increamenting by day#
                        Int64.TryParse(CountNumber, out intNextAutoNumber);
                        if (intNextAutoNumber > 0)
                        {
                            Int64 CurrentAutoNumber = 0;
                            Int64.TryParse(obj.NextToolCountNo, out CurrentAutoNumber);
                            if (intNextAutoNumber == (CurrentAutoNumber + 1))
                            {
                                objRoomDAL.UpdateNextToolCountNo(RoomID, CompanyID, Convert.ToString(intNextAutoNumber));
                            }
                        }
                        return;
                    case 5: //Date + Incrementing#
                        if (CountNumber.LastIndexOf("-") > 0 && CountNumber.LastIndexOf("-") < CountNumber.Trim().Length)
                        {
                            string incrNo = CountNumber.Remove(0, CountNumber.LastIndexOf("-") + 1);
                            Int64.TryParse(incrNo, out intNextAutoNumber);
                            if (intNextAutoNumber > 0)
                            {
                                Int64 CurrentAutoNumber = 0;
                                Int64.TryParse(obj.NextToolCountNo, out CurrentAutoNumber);
                                if (intNextAutoNumber == (CurrentAutoNumber + 1))
                                {
                                    objRoomDAL.UpdateNextToolCountNo(RoomID, CompanyID, Convert.ToString(intNextAutoNumber));
                                }
                            }
                        }
                        return;
                }


            }

        }

    }



}
