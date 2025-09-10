using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class ToolAssetQuantityDetailDAL : eTurnsBaseDAL
    {
        string ResourceBaseFilePath = System.Configuration.ConfigurationManager.AppSettings["ResourceBaseFilePath"];

        #region [Class Constructor]

        //public ToolAssetQuantityDetailDAL(base.DataBaseName)
        //{


        //}

        public ToolAssetQuantityDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolAssetQuantityDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}
        #endregion
        public IEnumerable<ToolAssetQuantityDetailDTO> GetToolAssetQuantity(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted)
        {
            IEnumerable<ToolAssetQuantityDetailDTO> objToolAssetQuantityDetailDTO;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchieved", IsArchived) };
                objToolAssetQuantityDetailDTO = context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec GetToolAssetQuantity @RoomId,@CompanyId,@IsDeleted,@IsArchieved", params1).ToList();
            }
            return objToolAssetQuantityDetailDTO;
        }
        public IEnumerable<ToolAssetQuantityDetailDTO> GetToolAssetQuantityByAssetToolGuid(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGuid)
        {
            IEnumerable<ToolAssetQuantityDetailDTO> objToolAssetQuantityDetailDTO;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyId", CompanyID), new SqlParameter("@IsDeleted", IsDeleted), new SqlParameter("@IsArchieved", IsArchived), new SqlParameter("@ToolGuid", ToolGuid) };
                objToolAssetQuantityDetailDTO = context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec GetToolAssetQuantityByAssetToolGuid @RoomId,@CompanyId,@IsDeleted,@IsArchieved,@ToolGuid", params1).ToList();
            }
            return objToolAssetQuantityDetailDTO;
        }

        public List<ToolAssetQuantityDetailDTO> GetToolLocationWithQtyByToolGuid(Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, Guid ToolGuid)
        {
            List<ToolAssetQuantityDetailDTO> lstLocQty = new List<ToolAssetQuantityDetailDTO>();
            lstLocQty = GetToolAssetQuantityByAssetToolGuid(RoomID, CompanyID, IsArchived, IsDeleted, ToolGuid).ToList();
            if (lstLocQty != null && lstLocQty.Count > 0)
            {
                var lstToolLocWithQty = (from A in lstLocQty
                                         where A.Quantity > 0
                                         group A by new { A.Location, A.ToolGUID, A.ToolBinID, A.SerialNumber } into G
                                         select new ToolAssetQuantityDetailDTO
                                         {
                                             Location = G.Key.Location,
                                             ToolGUID = G.Key.ToolGUID.Value,
                                             ToolBinID = G.Key.ToolBinID.Value,
                                             SerialNumber = (G.Key.SerialNumber ?? string.Empty),
                                             Quantity = G.Sum(x => x.Quantity)
                                         }
                                           ).ToList();

                return lstToolLocWithQty;
            }

            return null;
        }

        public ToolAssetQuantityDetailDTO Insert(ToolAssetQuantityDetailDTO objDTO, bool InCrementQTY = false, string PullAction = "Credit", Guid? CheckoutGUID = null, Guid? CheckinGUID = null, string ReferalAction = null, string SerialNumber = null)
        {
            try
            {
                objDTO.IsDeleted = objDTO.IsDeleted ? (bool)objDTO.IsDeleted : false;
                objDTO.IsArchived = objDTO.IsArchived ? (bool)objDTO.IsArchived : false;

                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objDTO.Created = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;

                if ((string.IsNullOrWhiteSpace(objDTO.WhatWhereAction)))
                {
                    try
                    {
                        objDTO.WhatWhereAction = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().ReflectedType.Name + ">>" + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
                    }
                    catch (Exception ex)
                    {
                        objDTO.WhatWhereAction = ex.Message.ToString();
                    }
                }


                var Count = 0;
                string EditedOnAction = string.Empty;
                if ((string.IsNullOrWhiteSpace(objDTO.EditedOnAction)))
                {
                    try
                    {
                        for (int i = 0; i <= new System.Diagnostics.StackTrace().FrameCount; i++)
                        {
                            if (new System.Diagnostics.StackTrace().GetFrame(i).GetMethod().ReflectedType == null)
                            {
                                Count = i - 1;
                                break;
                            }
                        }
                        for (; Count > 0; Count--)
                        {
                            if (!string.IsNullOrWhiteSpace(EditedOnAction))
                            {
                                EditedOnAction += "#############" + new System.Diagnostics.StackTrace().GetFrame(Count).GetMethod().ReflectedType.Name + ">>" + new System.Diagnostics.StackTrace().GetFrame(Count).GetMethod().Name;
                            }
                            else
                            {
                                EditedOnAction = new System.Diagnostics.StackTrace().GetFrame(Count).GetMethod().ReflectedType.Name + ">>" + new System.Diagnostics.StackTrace().GetFrame(Count).GetMethod().Name;
                            }
                        }
                        objDTO.EditedOnAction = EditedOnAction + "$$$$$$$$$$$$$$" + new System.Diagnostics.StackTrace().ToString();
                    }
                    catch (Exception ex)
                    {
                        objDTO.EditedOnAction = ex.Message.ToString();
                    }
                }
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO;
                    Int64 value = 0;

                    var params1 = new SqlParameter[]
                            { new SqlParameter("@ToolGUID", objDTO.ToolGUID )
                            , new SqlParameter("@CheckedInQTY", objDTO.Quantity )
                            , new SqlParameter("@CheckedInMQTY", value)
                            , new SqlParameter("@UserID", objDTO.UpdatedBy)
                            , new SqlParameter("@RoomID", objDTO.RoomID)
                            , new SqlParameter("@CompanyID", objDTO.CompanyID)
                            , new SqlParameter("@AddedFrom",  objDTO.EditedFrom)
                            , new SqlParameter("@EditedOnAction", objDTO.EditedOnAction)
                            , new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction)
                            , new SqlParameter("@PullCredit", PullAction)
                            , new SqlParameter("@ActionType", PullAction)
                            ,new SqlParameter("@InCrementQTY",InCrementQTY)
                            , new SqlParameter("@CheckoutGUID", CheckoutGUID??Guid.Empty )
                            ,new SqlParameter("@CheckInGUID",CheckinGUID??Guid.Empty  )
                            ,new SqlParameter("@ReferalAction",ReferalAction??string.Empty  )
                            ,new SqlParameter("@SerialNumber",SerialNumber??string.Empty  )
                            ,new SqlParameter("@Description",objDTO.Description??string.Empty  )
                            };
                    objToolAssetQuantityDetailDTO = context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec ToolQuantityCheckIN @ToolGUID,@CheckedInQTY,@CheckedInMQTY,@UserID,@RoomID,@CompanyID,@AddedFrom,@EditedOnAction,@WhatWhereAction,@PullCredit,@ActionType,@InCrementQTY,@CheckoutGUID,@CheckInGUID,@ReferalAction,@SerialNumber,@Description", params1).FirstOrDefault();

                    //double InitialQuantityWeb = objDTO.Quantity;
                    //double InitialQuantityPDA = objDTO.InitialQuantityPDA;
                    ////List<ToolAssetQuantityDetailDTO> ToolAssetQuantityDetailDTO = GetToolAssetQuantityByAssetToolGuid(objDTO.RoomID, objDTO.CompanyID, false, false, objDTO.ToolGUID ?? Guid.Empty).ToList();
                    ////if (ToolAssetQuantityDetailDTO != null && ToolAssetQuantityDetailDTO.Count() > 0)
                    ////{
                    ////    InitialQuantityWeb = ToolAssetQuantityDetailDTO.FirstOrDefault().InitialQuantityWeb;
                    ////    InitialQuantityPDA = ToolAssetQuantityDetailDTO.FirstOrDefault().InitialQuantityPDA;
                    ////}
                    //ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(base.DataBaseName);
                    //ToolLocationDetailsDTO objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolDefaultLocation(objDTO.RoomID, objDTO.CompanyID, false, false, objDTO.ToolGUID ?? Guid.Empty, objDTO.UpdatedBy, objDTO.EditedFrom, objDTO.WhatWhereAction);

                    //ToolAssetQuantityDetail obj = new ToolAssetQuantityDetail();
                    //obj.ID = 0;

                    //obj.GUID = Guid.NewGuid();
                    //obj.ToolGUID = objDTO.ToolGUID;

                    //obj.AssetGUID = objDTO.AssetGUID;


                    //obj.ToolBinID = objToolLocationDetailsDTO.ID;
                    //obj.Quantity = objDTO.Quantity;
                    //obj.RoomID = objDTO.RoomID;
                    //obj.CompanyID = objDTO.CompanyID;
                    //obj.Created = objDTO.Created;
                    //obj.Updated = objDTO.Updated;
                    //obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                    //obj.ReceivedOn = objDTO.ReceivedOn;
                    //obj.AddedFrom = objDTO.AddedFrom;
                    //obj.EditedFrom = objDTO.EditedFrom;
                    //obj.WhatWhereAction = objDTO.WhatWhereAction;
                    //obj.ReceivedDate = objDTO.ReceivedDate;
                    //obj.InitialQuantityWeb = InitialQuantityWeb;
                    //obj.InitialQuantityPDA = InitialQuantityPDA;
                    //obj.ExpirationDate = objDTO.ExpirationDate;
                    //obj.EditedOnAction = objDTO.EditedOnAction;
                    //obj.CreatedBy = objDTO.CreatedBy;
                    //obj.UpdatedBy = objDTO.UpdatedBy;
                    //obj.IsDeleted = objDTO.IsDeleted;
                    //obj.IsArchived = objDTO.IsArchived;

                    //context.ToolAssetQuantityDetails.Add(obj);
                    //context.SaveChanges();

                    //if (objDTO.ToolGUID != null && objDTO.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                    //{
                    //    new ToolMasterDAL(base.DataBaseName).UpdateToolAvailableQty(objDTO.ToolGUID ?? Guid.Empty, objDTO.RoomID, objDTO.CompanyID, objDTO.WhatWhereAction);
                    //}

                    return objToolAssetQuantityDetailDTO;
                }
            }
            catch
            {
                return null;
            }
        }

        public ToolAssetQuantityDetailDTO Insert_Light(ToolAssetQuantityDetailDTO objDTO)
        {
            try
            {
                objDTO.IsDeleted = objDTO.IsDeleted ? (bool)objDTO.IsDeleted : false;
                objDTO.IsArchived = objDTO.IsArchived ? (bool)objDTO.IsArchived : false;

                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objDTO.Created = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;


                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    ToolAssetQuantityDetail obj = new ToolAssetQuantityDetail();
                    obj.ID = 0;

                    obj.GUID = Guid.NewGuid();
                    obj.ToolGUID = objDTO.ToolGUID;

                    obj.AssetGUID = objDTO.AssetGUID;


                    obj.ToolBinID = objDTO.ToolBinID;
                    obj.Quantity = objDTO.Quantity;
                    obj.RoomID = objDTO.RoomID;
                    obj.CompanyID = objDTO.CompanyID;
                    obj.Created = objDTO.Created;
                    obj.Updated = objDTO.Updated;
                    obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                    obj.ReceivedOn = objDTO.ReceivedOn;
                    obj.AddedFrom = objDTO.AddedFrom;
                    obj.EditedFrom = objDTO.EditedFrom;
                    obj.WhatWhereAction = objDTO.WhatWhereAction;
                    obj.ReceivedDate = objDTO.ReceivedDate;
                    obj.InitialQuantityWeb = objDTO.Quantity;
                    obj.InitialQuantityPDA = 0;
                    obj.ExpirationDate = objDTO.ExpirationDate;
                    obj.EditedOnAction = objDTO.EditedOnAction;
                    obj.CreatedBy = objDTO.CreatedBy;
                    obj.UpdatedBy = objDTO.UpdatedBy;
                    obj.IsDeleted = objDTO.IsDeleted;
                    obj.IsArchived = objDTO.IsArchived;
                    obj.SerialNumber = objDTO.SerialNumber;


                    context.ToolAssetQuantityDetails.Add(obj);
                    context.SaveChanges();
                    objDTO.ID = obj.ID;
                    objDTO.GUID = obj.GUID;

                    return objDTO;
                }
            }
            catch
            {
                return null;
            }
        }

        public ToolAssetQuantityDetailDTO Edit_Light(ToolAssetQuantityDetailDTO objDTO)
        {
            try
            {


                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    ToolAssetQuantityDetail obj = context.ToolAssetQuantityDetails.Where(x => x.ID == objDTO.ID).FirstOrDefault();
                    if (obj != null && obj.ID > 0)
                    {

                        obj.ToolGUID = objDTO.ToolGUID;

                        obj.AssetGUID = objDTO.AssetGUID;


                        obj.ToolBinID = objDTO.ToolBinID;
                        obj.Quantity = objDTO.Quantity;
                        obj.RoomID = objDTO.RoomID;
                        obj.CompanyID = objDTO.CompanyID;
                        obj.Updated = objDTO.Updated;
                        obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                        obj.ReceivedOn = objDTO.ReceivedOn;
                        obj.EditedFrom = objDTO.EditedFrom;
                        obj.WhatWhereAction = objDTO.WhatWhereAction;
                        obj.ReceivedDate = objDTO.ReceivedDate;
                        obj.InitialQuantityWeb = objDTO.Quantity;
                        obj.InitialQuantityPDA = 0;
                        obj.ExpirationDate = objDTO.ExpirationDate;
                        obj.EditedOnAction = objDTO.EditedOnAction;
                        obj.UpdatedBy = objDTO.UpdatedBy;
                        obj.IsDeleted = objDTO.IsDeleted;
                        obj.IsArchived = objDTO.IsArchived;
                        obj.SerialNumber = objDTO.SerialNumber;



                        context.SaveChanges();
                    }

                    return objDTO;
                }
            }
            catch
            {
                return null;
            }
        }

        public bool UpdateOrInsert(ToolAssetQuantityDetailDTO objDTO, Double? QuantityDeduct, bool InCrementQTY = false, string PullCredit = "Credit", Guid? CheckoutGUID = null, Guid? CheckinGUID = null, string ReferalAction = null, string SerialNumber = null)
        {
            try
            {
                objDTO.IsDeleted = objDTO.IsDeleted ? (bool)objDTO.IsDeleted : false;
                objDTO.IsArchived = objDTO.IsArchived ? (bool)objDTO.IsArchived : false;

                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objDTO.Created = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                if ((string.IsNullOrWhiteSpace(objDTO.WhatWhereAction)))
                {
                    try
                    {
                        objDTO.WhatWhereAction = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().ReflectedType.Name + ">>" + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
                    }
                    catch (Exception ex)
                    {
                        objDTO.WhatWhereAction = ex.Message.ToString();
                    }
                }


                var Count = 0;
                string EditedOnAction = string.Empty;
                if ((string.IsNullOrWhiteSpace(objDTO.EditedOnAction)))
                {
                    try
                    {
                        for (int i = 0; i <= new System.Diagnostics.StackTrace().FrameCount; i++)
                        {
                            if (new System.Diagnostics.StackTrace().GetFrame(i).GetMethod().ReflectedType == null)
                            {
                                Count = i - 1;
                                break;
                            }
                        }
                        for (; Count > 0; Count--)
                        {
                            if (!string.IsNullOrWhiteSpace(EditedOnAction))
                            {
                                EditedOnAction += "#############" + new System.Diagnostics.StackTrace().GetFrame(Count).GetMethod().ReflectedType.Name + ">>" + new System.Diagnostics.StackTrace().GetFrame(Count).GetMethod().Name;
                            }
                            else
                            {
                                EditedOnAction = new System.Diagnostics.StackTrace().GetFrame(Count).GetMethod().ReflectedType.Name + ">>" + new System.Diagnostics.StackTrace().GetFrame(Count).GetMethod().Name;
                            }
                        }
                        objDTO.EditedOnAction = EditedOnAction + "$$$$$$$$$$$$$$" + new System.Diagnostics.StackTrace().ToString();
                    }
                    catch (Exception ex)
                    {
                        objDTO.EditedOnAction = ex.Message.ToString();
                    }
                }
                List<string> PullDetailsQuantities = new List<string>();

                ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(base.DataBaseName);
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    ToolMasterDTO objTOOL = new ToolMasterDAL(base.DataBaseName).GetToolByGUIDPlain(objDTO.ToolGUID.GetValueOrDefault(Guid.NewGuid()));
                    if (objTOOL != null)
                    {
                        List<ToolAssetQuantityDetail> lstToolAssetQuantityDetail = context.ToolAssetQuantityDetails.Where(x => x.ToolGUID == objDTO.ToolGUID).ToList();

                        objDTO.Quantity = (objDTO.Quantity - (objTOOL.CheckedOutQTY + objTOOL.CheckedOutMQTY)) ?? 0;
                        if (QuantityDeduct == null || QuantityDeduct < 1)
                        {
                            double InitialQuantityWeb = objDTO.InitialQuantityWeb;
                            double InitialQuantityPDA = objDTO.InitialQuantityPDA;
                            ToolLocationDetailsDTO objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolDefaultLocation(objDTO.RoomID, objDTO.CompanyID, false, false, objDTO.ToolGUID ?? Guid.Empty, objDTO.UpdatedBy, objDTO.EditedFrom, objDTO.WhatWhereAction);


                            Double? TotalAvaialbleQuantity = 0;

                            if (lstToolAssetQuantityDetail != null && lstToolAssetQuantityDetail.Count > 0)
                            {
                                TotalAvaialbleQuantity = lstToolAssetQuantityDetail.Sum(T => T.Quantity);

                            }
                            if (TotalAvaialbleQuantity < (objDTO.Quantity))
                            {
                                objDTO.Quantity = objDTO.Quantity - (TotalAvaialbleQuantity ?? 0);
                                ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO;
                                Int64 value = 0;

                                var params1 = new SqlParameter[]
                                    { new SqlParameter("@ToolGUID", objDTO.ToolGUID )
                                    , new SqlParameter("@CheckedInQTY", objDTO.Quantity )
                                    , new SqlParameter("@CheckedInMQTY", value)
                                    , new SqlParameter("@UserID", objDTO.UpdatedBy)
                                    , new SqlParameter("@RoomID", objDTO.RoomID)
                                    , new SqlParameter("@CompanyID", objDTO.CompanyID)
                                    , new SqlParameter("@AddedFrom",  objDTO.EditedFrom)
                                    , new SqlParameter("@EditedOnAction", objDTO.EditedOnAction)
                                    , new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction)
                                    , new SqlParameter("@PullCredit", PullCredit)
                                    , new SqlParameter("@ActionType", PullCredit)
                                    ,new SqlParameter("@InCrementQTY", InCrementQTY)
                                    ,new SqlParameter("@CheckoutGUID", CheckoutGUID??Guid.Empty )
                                    ,new SqlParameter("@CheckInGUID", CheckinGUID??Guid.Empty )
                                    ,new SqlParameter("@ReferalAction", ReferalAction??string.Empty )
                                    ,new SqlParameter("@SerialNumber", SerialNumber??string.Empty )
                                    ,new SqlParameter("@Description",objDTO.Description??string.Empty  )
                                    };
                                objToolAssetQuantityDetailDTO = context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec ToolQuantityCheckIN @ToolGUID,@CheckedInQTY,@CheckedInMQTY,@UserID,@RoomID,@CompanyID,@AddedFrom,@EditedOnAction,@WhatWhereAction,@PullCredit,@ActionType,@InCrementQTY,@CheckoutGUID,@CheckInGUID,@ReferalAction,@SerialNumber,@Description", params1).FirstOrDefault();

                            }
                            else
                            {
                                if (TotalAvaialbleQuantity > (objDTO.Quantity))
                                {
                                    objDTO.Quantity = (TotalAvaialbleQuantity.GetValueOrDefault(0) - (objDTO.Quantity));
                                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO;
                                    PullCredit = "Pull";
                                    var params1 = new SqlParameter[]
                                        {
                                            new SqlParameter("@ToolGUID", objDTO.ToolGUID )
                                            , new SqlParameter("@Quantity", objDTO.Quantity )
                                            , new SqlParameter("@UserId", objDTO.UpdatedBy)
                                            , new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction)
                                            , new SqlParameter("@EditedOnAction", objDTO.EditedOnAction)
                                            , new SqlParameter("@ActionType", PullCredit)
                                            , new SqlParameter("@PullCredit", PullCredit)
                                            , new SqlParameter("@AddedFrom",  objDTO.EditedFrom)
                                            , new SqlParameter("@CheckoutGUID", CheckoutGUID??Guid.Empty)
                                            , new SqlParameter("@CheckInGUID",  CheckinGUID??Guid.Empty)
                                            , new SqlParameter("@ReferalAction",  ReferalAction??string.Empty)
                                            , new SqlParameter("@SerialNumber",  SerialNumber??string.Empty)
                                            ,new SqlParameter("@Description",objDTO.Description??string.Empty  )
                                            ,new SqlParameter("@ToolBinID",objDTO.ToolBinID ?? 0)

                                        };
                                    objToolAssetQuantityDetailDTO = context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec ToolAssetQuantityTableCalculation @ToolGuid,@Quantity,@UserId,@WhatWhereAction,@EditedOnAction,@ActionType,@PullCredit,@AddedFrom,@CheckoutGUID,@CheckInGUID,@ReferalAction,@SerialNumber,@Description,@ToolBinID", params1).FirstOrDefault();
                                }

                            }
                        }
                        else
                        {
                            if ((QuantityDeduct ?? 0) > 0)
                            {
                                ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO;
                                PullCredit = "Pull";
                                var params1 = new SqlParameter[]
                                    { new SqlParameter("@ToolGUID", objDTO.ToolGUID )
                                    , new SqlParameter("@Quantity",QuantityDeduct??0 )
                                    , new SqlParameter("@UserId", objDTO.UpdatedBy)
                                    , new SqlParameter("@WhatWhereAction", objDTO.WhatWhereAction)
                                    , new SqlParameter("@EditedOnAction", objDTO.EditedOnAction)
                                    , new SqlParameter("@ActionType", PullCredit)
                                    , new SqlParameter("@PullCredit", PullCredit)
                                    , new SqlParameter("@AddedFrom",  objDTO.EditedFrom)
                                    , new SqlParameter("@CheckoutGUID", CheckoutGUID??Guid.Empty)
                                    , new SqlParameter("@CheckInGUID", CheckinGUID??Guid.Empty)
                                    , new SqlParameter("@ReferalAction", ReferalAction??string.Empty)
                                    , new SqlParameter("@SerialNumber", SerialNumber??string.Empty)
                                    ,new SqlParameter("@Description",objDTO.Description??string.Empty  )
                                    ,new SqlParameter("@ToolBinID",objDTO.ToolBinID ?? 0  )
                                    };
                                objToolAssetQuantityDetailDTO = context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec ToolAssetQuantityTableCalculation @ToolGuid,@Quantity,@UserId,@WhatWhereAction,@EditedOnAction,@ActionType,@PullCredit,@AddedFrom,@CheckoutGUID,@CheckInGUID,@ReferalAction,@SerialNumber,@Description,@ToolBinID", params1).FirstOrDefault();



                            }
                        }
                    }

                    return true;
                }

            }
            catch
            {
                return false;
            }
        }
        public void UpdateSerial(Guid? ToolGUID, string OldSerialNumber, string NewSerialNumber)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[]
                                    { new SqlParameter("@ToolGUID", ToolGUID??Guid.Empty )
                                    , new SqlParameter("@OldSerial",OldSerialNumber )
                                    , new SqlParameter("@NewSerial", NewSerialNumber)
                                    };
                context.Database.SqlQuery<string>("exec UpdateSerialNumber @ToolGUID,@OldSerial,@NewSerial", params1);
            }

        }
        public ToolAssetQuantityDetailDTO GetRecordByLocationTool(Guid ToolGUID, Guid LocationGUID, Int64 RoomID, Int64 CompanyID)
        {
            //return GetCachedData(RoomID, CompanyID).Where(t => t.ItemID == ItemID && t.BinID == BinID).SingleOrDefault();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@LocationGUID", LocationGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };

                ToolAssetQuantityDetailDTO obj = (from u in context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec [GetRecordByLocationTool] @ToolGUID,@LocationGUID,@RoomID,@CompanyID", paramA)
                                                  select new ToolAssetQuantityDetailDTO
                                                  {
                                                      ID = u.ID,
                                                      ToolGUID = u.ToolGUID,
                                                      Quantity = u.Quantity,
                                                      ToolBinID = u.ToolBinID,
                                                      LotNumber = u.LotNumber,
                                                      GUID = u.GUID,

                                                      Created = u.Created,
                                                      Updated = u.Updated,
                                                      CreatedBy = u.CreatedBy,
                                                      UpdatedBy = u.UpdatedBy,
                                                      RoomID = u.RoomID,
                                                      CompanyID = u.CompanyID,
                                                      CreatedByName = u.CreatedByName,
                                                      UpdatedByName = u.UpdatedByName,
                                                      RoomName = u.RoomName,
                                                      AddedFrom = u.AddedFrom,
                                                      EditedFrom = u.EditedFrom,
                                                      ReceivedOnWeb = u.ReceivedOnWeb,
                                                      ReceivedOn = u.ReceivedOn,
                                                  }).FirstOrDefault();
                return obj;
            }
        }
        public List<ToolAssetQuantityDetailDTO> GetRecordByLocationToolAll(Guid ToolGUID, Int64 RoomID, Int64 CompanyID)
        {
            //return GetCachedData(RoomID, CompanyID).Where(t => t.ItemID == ItemID && t.BinID == BinID).SingleOrDefault();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };

                List<ToolAssetQuantityDetailDTO> obj = (from u in context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec [GetRecordByLocationToolAll] @ToolGUID,@RoomID,@CompanyID", paramA)
                                                        select new ToolAssetQuantityDetailDTO
                                                        {
                                                            ID = u.ID,
                                                            ToolGUID = u.ToolGUID,
                                                            Quantity = u.Quantity,
                                                            ToolBinID = u.ToolBinID,
                                                            LotNumber = u.LotNumber,
                                                            GUID = u.GUID,

                                                            Created = u.Created,
                                                            Updated = u.Updated,
                                                            CreatedBy = u.CreatedBy,
                                                            UpdatedBy = u.UpdatedBy,
                                                            RoomID = u.RoomID,
                                                            CompanyID = u.CompanyID,
                                                            CreatedByName = u.CreatedByName,
                                                            UpdatedByName = u.UpdatedByName,
                                                            RoomName = u.RoomName,
                                                            AddedFrom = u.AddedFrom,
                                                            EditedFrom = u.EditedFrom,
                                                            ReceivedOnWeb = u.ReceivedOnWeb,
                                                            ReceivedOn = u.ReceivedOn,
                                                            Location = u.Location,
                                                            LocationGUID = u.LocationGUID,

                                                        }).ToList();
                return obj;
            }
        }
        public ToolAssetQuantityDetailDTO GetRecordByLocationToolByToolBinId(Guid ToolGUID, Int64 ToolBinID, Int64 RoomID, Int64 CompanyID)
        {
            //return GetCachedData(RoomID, CompanyID).Where(t => t.ItemID == ItemID && t.BinID == BinID).SingleOrDefault();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@ToolBinID", ToolBinID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };

                ToolAssetQuantityDetailDTO obj = (from u in context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec [GetRecordByLocationToolByToolBinId] @ToolGUID,@ToolBinID,@RoomID,@CompanyID", paramA)
                                                  select new ToolAssetQuantityDetailDTO
                                                  {
                                                      ID = u.ID,
                                                      ToolGUID = u.ToolGUID,
                                                      Quantity = u.Quantity,
                                                      ToolBinID = u.ToolBinID,
                                                      LotNumber = u.LotNumber,
                                                      GUID = u.GUID,
                                                      Location = u.Location,
                                                      Created = u.Created,
                                                      Updated = u.Updated,
                                                      CreatedBy = u.CreatedBy,
                                                      UpdatedBy = u.UpdatedBy,
                                                      RoomID = u.RoomID,
                                                      CompanyID = u.CompanyID,
                                                      CreatedByName = u.CreatedByName,
                                                      UpdatedByName = u.UpdatedByName,
                                                      RoomName = u.RoomName,
                                                      AddedFrom = u.AddedFrom,
                                                      EditedFrom = u.EditedFrom,
                                                      ReceivedOnWeb = u.ReceivedOnWeb,
                                                      ReceivedOn = u.ReceivedOn,
                                                  }).FirstOrDefault();
                return obj;
            }
        }

        public ToolAssetQuantityDetailDTO GetLocationQuantityToolByToolBinId(Guid ToolGUID, Int64 ToolBinID, Int64 RoomID, Int64 CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@ToolBinID", ToolBinID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };

                ToolAssetQuantityDetailDTO obj = (from u in context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec [GetLocationQuantityToolByToolBinId] @ToolGUID,@ToolBinID,@RoomID,@CompanyID", paramA)
                                                  select new ToolAssetQuantityDetailDTO
                                                  {
                                                      ToolGUID = u.ToolGUID,
                                                      AssetGUID = u.AssetGUID,
                                                      ToolBinID = u.ToolBinID,
                                                      Quantity = u.Quantity,
                                                      Location = u.Location,
                                                      RoomID = u.RoomID
                                                  }).FirstOrDefault();
                return obj;
            }
        }
        public List<ToolAssetQuantityDetailDTO> ValidateTAQRecords(List<ToolAssetQuantityMain> lstToolAssetQuantityDetails, long RoomID, long CompanyID, long UserID, long EnterpriseId, string CultureCode, bool isUDFRequired = true, bool isCountPullValidate = false)
        {
            List<ToolAssetQuantityDetailDTO> lstReturnList = new List<ToolAssetQuantityDetailDTO>();

            ToolMasterDTO objItem = new ToolMasterDTO();
            LocationMasterDTO objLocationMasterDTO = new LocationMasterDTO();

            ToolLocationDetailsDTO objTLDDTO = new ToolLocationDetailsDTO();

            ToolLocationDetailsDAL objTLDDAL = new ToolLocationDetailsDAL(base.DataBaseName);

            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(base.DataBaseName);

            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(base.DataBaseName);
            eTurns.DAL.UDFDAL objUDFDAL = new eTurns.DAL.UDFDAL(base.DataBaseName);
            //IEnumerable<UDFDTO> UDFDataFromDB = null;
            //if (isUDFRequired == true)
            //    UDFDataFromDB = objUDFDAL.GetAllRecordsNew(CompanyID, "ToolAssetCount", RoomID);
            var toolMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResToolMaster", CultureCode, EnterpriseId, CompanyID);
            var pullMasterResourceFilePath = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResPullMaster", CultureCode, EnterpriseId, CompanyID);
            

            if (lstToolAssetQuantityDetails != null && lstToolAssetQuantityDetails.Count > 0)
            {
                foreach (var Locitem in lstToolAssetQuantityDetails)
                {
                    ToolAssetQuantityDetailDTO Locitemmain = new ToolAssetQuantityDetailDTO();
                    Locitemmain.Location = Locitem.BinNumber;
                    Locitemmain.CompanyID = CompanyID;
                    Locitemmain.Cost = Locitem.Cost;
                    Locitemmain.Created = DateTime.UtcNow;
                    Locitemmain.CreatedBy = UserID;
                    Locitemmain.Quantity = Locitem.Quantity.GetValueOrDefault(0);
                    Locitemmain.GUID = Locitem.GUID;
                    Locitemmain.ID = Locitem.ID;
                    Locitemmain.ToolGUID = Locitem.ToolGUID;
                    Locitemmain.ToolName = Locitem.ToolName;
                    Locitemmain.UpdatedBy = Locitem.UpdatedBy;
                    Locitemmain.RoomID = RoomID;
                    Locitemmain.SerialNumber = (!string.IsNullOrWhiteSpace(Locitem.SerialNumber)) ? Locitem.SerialNumber.Trim() : string.Empty;
                    Locitemmain.Updated = DateTime.UtcNow;

                    string Errormessage = string.Empty;

                    if (string.IsNullOrWhiteSpace(Locitem.ToolName))
                    {
                        string msgEnterToolName = ResourceRead.GetResourceValueByKeyAndFullFilePath("EnterToolName", toolMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                        Errormessage += ";" + msgEnterToolName;
                    }
                    //if (string.IsNullOrWhiteSpace(Locitem.BinNumber))
                    //{
                    //    Errormessage += ";please enter bin number";
                    //}
                    if ((Locitem.ToolName != objItem.ToolName) && string.IsNullOrWhiteSpace(Errormessage))
                    {
                        objItem = objToolMasterDAL.GetToolByName(Locitem.ToolName, RoomID, CompanyID);
                        if (objItem == null)
                        {
                            objItem = new ToolMasterDTO();
                        }

                        Locitemmain.ToolGUID = objItem.GUID;
                    }
                    else
                    {
                        Locitemmain.ToolName = objItem.ToolName;
                        Locitemmain.ToolGUID = objItem.GUID;
                        if (objItem == null)
                        {
                            objItem = new ToolMasterDTO();
                        }
                    }
                    if (objItem.ID < 1)
                    {
                        string msgToolNameDoesNotExistInRoom = ResourceRead.GetResourceValueByKeyAndFullFilePath("ToolNameDoesNotExistInRoom", toolMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                        Errormessage += ";" + msgToolNameDoesNotExistInRoom;
                    }

                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.ID > 0)
                    {
                        objTLDDTO = objTLDDAL.GetToolLocation(objItem.GUID, Locitem.BinNumber, RoomID, CompanyID, UserID, "Import>>ToolAdjustMentCount");
                        if (objTLDDTO != null)
                        {
                            Locitemmain.Location = Locitem.BinNumber;
                            Locitemmain.ToolBinID = objTLDDTO.ID;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(Errormessage) && (Locitemmain.Quantity < 0))
                    {
                        string msgCountQtyShouldBeGreaterThanEqualsZero = ResourceRead.GetResourceValueByKeyAndFullFilePath("CountQtyShouldBeGreaterThanEqualsZero", toolMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                        Errormessage += ";" + msgCountQtyShouldBeGreaterThanEqualsZero; 
                    }

                    if (string.IsNullOrWhiteSpace(Errormessage) && objItem.SerialNumberTracking)
                    {
                        if (string.IsNullOrWhiteSpace(Locitem.SerialNumber))
                        {
                            string msgSerialNoCantBeBlank = ResourceRead.GetResourceValueByKeyAndFullFilePath("SerialNoCantBeBlank", toolMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                            Errormessage += ";" + msgSerialNoCantBeBlank; 
                        }
                        else
                        {
                            if (Locitem.Quantity == 0)
                            {
                                string msgCountQtyShouldBeOneForSelectedSerialNo = ResourceRead.GetResourceValueByKeyAndFullFilePath("CountQtyShouldBeOneForSelectedSerialNo", toolMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                                Errormessage += ";" + string.Format(msgCountQtyShouldBeOneForSelectedSerialNo, Locitemmain.SerialNumber);
                            }
                            if (Locitem.Quantity > 1)
                            {
                                string msgCountQtyShouldBeOneForSelectedSerialNo = ResourceRead.GetResourceValueByKeyAndFullFilePath("CountQtyShouldBeOneForSelectedSerialNo", toolMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                                Errormessage += ";" + string.Format(msgCountQtyShouldBeOneForSelectedSerialNo, Locitemmain.SerialNumber);
                            }

                            if (Locitem.Quantity == 1)
                            {
                                bool isSerailExistinChekout = GetSerialExistinCheckOut(objItem.GUID, Locitem.SerialNumber, objItem.CompanyID.GetValueOrDefault(0), objItem.Room.GetValueOrDefault(0));
                                if (isSerailExistinChekout)
                                {
                                    string msgQuantityIsAlreadyAvailableForSelectedSerial = ResourceRead.GetResourceValueByKeyAndFullFilePath("QuantityIsAlreadyAvailableForSelectedSerial", toolMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResToolMaster", CultureCode);
                                    Errormessage = string.Format(msgQuantityIsAlreadyAvailableForSelectedSerial,Locitemmain.SerialNumber); 
                                }
                            }

                            IEnumerable<ToolAssetQuantityDetailDTO> lstToolAssetQuantityDetail = null;
                            lstToolAssetQuantityDetail = GetCountDifferenceforValidatPull(objItem.GUID, objItem.Room.GetValueOrDefault(0), objItem.CompanyID.GetValueOrDefault(0), Locitemmain.ToolBinID.GetValueOrDefault(0));
                            lstToolAssetQuantityDetail = lstToolAssetQuantityDetail.Where(x => x.SerialNumber.ToLower().Equals(Locitem.SerialNumber.ToLower())).ToList();


                            double? Difference = 0;
                            double? countedQty = 0;


                            if (objItem.ID > 0)
                            {
                                if (lstToolAssetQuantityDetail != null && lstToolAssetQuantityDetail.Count() > 0)
                                {
                                    countedQty = lstToolAssetQuantityDetail.Sum(x => x.Quantity);
                                }

                                Difference = (Locitemmain.Quantity - countedQty);

                                if (Difference > 0)
                                {
                                    bool IsSerailAvailableForCredit = ValidateToolSRNumberForCredit(objItem.GUID, Locitemmain.SerialNumber, CompanyID, RoomID);
                                    if (IsSerailAvailableForCredit == false)
                                    {

                                        string msgCreditTransactionForSerialNumber = ResourceRead.GetResourceValueByKeyAndFullFilePath("MsgCreditTransactionForSerialNumber", pullMasterResourceFilePath, EnterpriseId, CompanyID, RoomID, "ResPullMaster", CultureCode);
                                        if (string.IsNullOrWhiteSpace(Errormessage))
                                            Errormessage = msgCreditTransactionForSerialNumber  + " " + Locitemmain.SerialNumber;
                                        else
                                            Errormessage +=( ";" + msgCreditTransactionForSerialNumber + " " + Locitemmain.SerialNumber);
                                    }
                                }
                            }

                        }

                    }





                    //Locitemmain.UDF1 = Locitem.UDF1;
                    //Locitemmain.UDF2 = Locitem.UDF2;
                    //Locitemmain.UDF3 = Locitem.UDF3;
                    //Locitemmain.UDF4 = Locitem.UDF4;
                    //Locitemmain.UDF5 = Locitem.UDF5;

                    //string errorMsg = string.Empty;
                    //if (UDFDataFromDB != null && isUDFRequired == true)
                    //    CommonUtilityHelper.CheckUDFIsRequired(UDFDataFromDB, Locitem.UDF1, Locitem.UDF2, Locitem.UDF3, Locitem.UDF4, Locitem.UDF5, out errorMsg);
                    //if (!string.IsNullOrWhiteSpace(errorMsg))
                    //{
                    //    //Locitem.Status = "Fail";
                    //    if (!string.IsNullOrEmpty(Errormessage))
                    //        Errormessage += "; " + errorMsg;
                    //    else
                    //        Errormessage = "; " + errorMsg;


                    //}

                    if (!string.IsNullOrWhiteSpace(Errormessage))
                    {
                        Locitemmain.ErrorMessege = Errormessage;
                    }
                    Locitemmain.SerialNumberTracking = objItem.SerialNumberTracking;
                    Locitemmain.Type = Convert.ToInt32(objItem.Type);
                    lstReturnList.Add(Locitemmain);
                }
            }
            return lstReturnList;
        }

        public bool ValidateToolSRNumberForCredit(Guid ToolGUID, string SerialNumber, long CompanyID, long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@SerialNumber", SerialNumber), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                Int32 ExistCount = 0;
                ExistCount = context.Database.SqlQuery<Int32>("exec [ValidateToolSRNumberForCredit] @ToolGUID,@SerialNumber,@CompanyID,@RoomID", params1).FirstOrDefault();
                if (ExistCount > 0)
                    return false;
                else
                    return true;
            }
        }

        public IEnumerable<ToolAssetQuantityDetailDTO> GetCountDifferenceforValidatPull(Guid ToolGUID, Int64 RoomID, Int64 CompanyID, Int64 ToolBinID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@ToolBinID", ToolBinID) };
                IEnumerable<ToolAssetQuantityDetailDTO> obj = context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec [GetTAQDtllWithCountQuantity] @CompanyID,@RoomID,@ToolGUID,@ToolBinID", paramA).ToList();
                return obj;
            }
        }

        public bool InsertToolAssetQuantityDetailsFromRecieve(List<ToolAssetQuantityDetailDTO> objData)
        {
            ReceivedToolAssetOrderTransferDetailDAL objROTDDAL = new ReceivedToolAssetOrderTransferDetailDAL(base.DataBaseName);
            IEnumerable<ReceivedToolAssetOrderTransferDetailDTO> lst;
            ToolAssetOrderDetailsDTO OrdDetailDTO = null;
            ToolAssetOrderDetailsDAL ordDetailDAL = null;
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                #region "Location Detail Save"
                foreach (ToolAssetQuantityDetailDTO item in objData)
                {

                    item.InitialQuantityWeb = item.Quantity;

                    if (string.IsNullOrEmpty(item.AddedFrom))
                        item.AddedFrom = "Web";

                    if (string.IsNullOrEmpty(item.EditedFrom))
                        item.EditedFrom = "Web";

                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    if (string.IsNullOrEmpty(item.Action))
                        item.Action = "Receive";

                    Insert(item);


                    ordDetailDAL = new ToolAssetOrderDetailsDAL(base.DataBaseName);
                    OrdDetailDTO = ordDetailDAL.GetRecord(item.ToolAssetOrderDetailGUID.GetValueOrDefault(Guid.Empty), item.RoomID, item.CompanyID);

                    lst = objROTDDAL.GetAllRecords(item.RoomID, item.CompanyID, item.ToolGUID.GetValueOrDefault(Guid.Empty), item.ToolAssetOrderDetailGUID, "ID ASC");
                    double rcvQty = lst.Sum(x => x.Quantity.GetValueOrDefault(0));


                    OrdDetailDTO.ReceivedQuantity = rcvQty;
                    OrdDetailDTO.IsOnlyFromUI = true;
                    OrdDetailDTO.EditedFrom = "Web";
                    OrdDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    ordDetailDAL.Edit(OrdDetailDTO);


                }

                #endregion


            }
            return true;
        }

        public void ToolAssetApplyCountLineitem(DataTable DT, long RoomID, long CompanyID, long UserID)
        {
            try
            {
                string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));
                SqlConnection ChildDbConnection = new SqlConnection(Connectionstring);
                DataSet Ds = SqlHelper.ExecuteDataset(ChildDbConnection, "CountATool", RoomID, CompanyID, UserID, DT);
            }
            catch
            {

            }
        }
        public List<ToolAssetQuantityDetailDTO> GetToolsLocationsSerLotQty(Guid ToolGUID, long BinID, string LotNumber, string SerialNumber, long RoomId, long CompanyId)
        {
            List<ToolAssetQuantityDetailDTO> oItemLocations = new List<ToolAssetQuantityDetailDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                oItemLocations = (from il in context.ToolAssetQuantityDetails
                                  where il.ToolGUID == ToolGUID && il.ToolBinID == BinID && il.RoomID == RoomId && il.CompanyID == CompanyId
                                  // && (il.LotNumber == LotNumber || LotNumber == string.Empty)
                                  && (il.SerialNumber == SerialNumber || SerialNumber == string.Empty)
                                  && (il.IsDeleted) == false && (il.IsArchived) == false
                                  && ((il.Quantity ?? 0)) > 0
                                  select new ToolAssetQuantityDetailDTO
                                  {
                                      ID = il.ID,
                                      GUID = il.GUID,
                                      Quantity = il.Quantity ?? 0,
                                      //LotNumber = il.LotNumber,
                                      SerialNumber = il.SerialNumber,
                                  }).ToList();
            }
            return oItemLocations;
        }
        public List<ToolAssetQuantityDetailDTO> GetRecordsByBinNumberAndLotSerial(Guid ToolGUID, string Location, string LotSerialNumber, Int64 RoomID, Int64 CompanyID)
        {
            //Need to changes...
            //return GetCachedData(0,RoomID, CompanyID).Where(t => t.ID == id).SingleOrDefault();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramA = new SqlParameter[] { new SqlParameter("@ToolGuid", ToolGUID), new SqlParameter("@Location", Location ?? string.Empty), new SqlParameter("@LotSerialNumber", LotSerialNumber), new SqlParameter("@RoomId", RoomID), new SqlParameter("@CompanyID", CompanyID) };

                List<ToolAssetQuantityDetailDTO> obj = (from u in context.Database.SqlQuery<ToolAssetQuantityDetailDTO>("exec [GetToolAssetQuantityDetailUsingSRLocation] @ToolGuid,@Location,@LotSerialNumber,@RoomId,@CompanyID", paramA)
                                                        select new ToolAssetQuantityDetailDTO
                                                        {
                                                            ID = u.ID,

                                                            ToolBinID = u.ToolBinID,
                                                            Quantity = u.Quantity,
                                                            //LotNumber = u.LotNumber,
                                                            SerialNumber = u.SerialNumber,
                                                            ExpirationDate = u.ExpirationDate,
                                                            ReceivedDate = u.ReceivedDate,
                                                            //Expiration = u.Expiration == null && u.DateCodeTracking ? DateTime.Now.ToString("MM /dd/yy") : u.Expiration,
                                                            //Received = u.Received == null ? DateTime.Now.ToString("MM/dd/yy") : u.Received,
                                                            Cost = u.Cost,

                                                            GUID = u.GUID,
                                                            ToolGUID = u.ToolGUID,
                                                            Created = u.Created,
                                                            Updated = u.Updated,
                                                            CreatedBy = u.CreatedBy,
                                                            UpdatedBy = u.UpdatedBy,
                                                            IsDeleted = (u.IsDeleted),
                                                            IsArchived = (u.IsArchived),
                                                            CompanyID = u.CompanyID,
                                                            RoomID = u.RoomID,
                                                            CreatedByName = u.CreatedByName,
                                                            UpdatedByName = u.UpdatedByName,
                                                            RoomName = u.RoomName,
                                                            Location = u.Location,
                                                            ToolName = u.ToolName,
                                                            SerialNumberTracking = u.SerialNumberTracking,
                                                            LotNumberTracking = u.LotNumberTracking,
                                                            DateCodeTracking = u.DateCodeTracking,
                                                            ToolAssetOrderDetailGUID = u.ToolAssetOrderDetailGUID,
                                                            AddedFrom = (u.AddedFrom == null ? "Web" : u.AddedFrom),
                                                            EditedFrom = (u.EditedFrom == null ? "Web" : u.EditedFrom),
                                                            ReceivedOn = u.ReceivedOn,
                                                            ReceivedOnWeb = u.ReceivedOnWeb,
                                                            Description = u.Description
                                                        }).ToList();

                return obj;
            }
        }
        public bool CheckQtyExistsOnLocation(Guid? LocationGUID, Guid? ToolGUID)
        {
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    var paramA = new SqlParameter[] { new SqlParameter("@LocationGUID", LocationGUID), new SqlParameter("@ToolGUID", ToolGUID) };

                    bool obj = context.Database.SqlQuery<bool>("exec [CheckQtyExistsOnLocation] @LocationGUID,@ToolGUID", paramA).FirstOrDefault();


                    return obj;

                }
            }
            catch
            {
                return true;
            }
        }
        public bool GetSerialExistsForToolForCheckOut(string SerialNumber, Guid? ToolGUID, Int64 RoomId, Int64 CompanyId)
        {
            try
            {
                bool SerialExistsForCheckOut = false;
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetail = (from il in context.ToolAssetQuantityDetails
                                                                             where il.ToolGUID == ToolGUID && il.RoomID == RoomId && il.CompanyID == CompanyId

                                                                             && (il.SerialNumber == SerialNumber)
                                                                             && (il.IsDeleted) == false && (il.IsArchived) == false
                                                                             && ((il.Quantity ?? 0)) > 0
                                                                             select new ToolAssetQuantityDetailDTO
                                                                             {
                                                                                 ID = il.ID,
                                                                                 GUID = il.GUID,
                                                                                 Quantity = il.Quantity ?? 0,
                                                                                 //LotNumber = il.LotNumber,
                                                                                 SerialNumber = il.SerialNumber,
                                                                             }).FirstOrDefault();
                    if (objToolAssetQuantityDetail != null)
                    {
                        SerialExistsForCheckOut = true;
                    }
                }
                return SerialExistsForCheckOut;
            }
            catch
            {
                return false;
            }
        }
        public bool GetSerialExistsForToolForCheckIn(string SerialNumber, Guid? ToolGUID, Int64 RoomId, Int64 CompanyId)
        {
            try
            {
                bool SerialExistsForCheckOut = false;
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {

                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetail = (from il in context.ToolAssetQuantityDetails
                                                                             where il.ToolGUID == ToolGUID && il.RoomID == RoomId && il.CompanyID == CompanyId

                                                                             && (il.SerialNumber == SerialNumber)
                                                                             && (il.IsDeleted) == false && (il.IsArchived) == false

                                                                             select new ToolAssetQuantityDetailDTO
                                                                             {
                                                                                 ID = il.ID,
                                                                                 GUID = il.GUID,
                                                                                 Quantity = il.Quantity ?? 0,
                                                                                 //LotNumber = il.LotNumber,
                                                                                 SerialNumber = il.SerialNumber,
                                                                             }).FirstOrDefault();
                    if (objToolAssetQuantityDetail != null)
                    {
                        List<ToolAssetQuantityDetailDTO> lstToolAssetQuantityDetail = (from il in context.ToolAssetQuantityDetails
                                                                                       where il.ToolGUID == ToolGUID && il.RoomID == RoomId && il.CompanyID == CompanyId

                                                                                       && (il.SerialNumber == SerialNumber)
                                                                                       && (il.IsDeleted) == false && (il.IsArchived) == false
                                                                                       && ((il.Quantity ?? 0)) > 0
                                                                                       select new ToolAssetQuantityDetailDTO
                                                                                       {
                                                                                           ID = il.ID,
                                                                                           GUID = il.GUID,
                                                                                           Quantity = il.Quantity ?? 0,
                                                                                           //LotNumber = il.LotNumber,
                                                                                           SerialNumber = il.SerialNumber,
                                                                                       }).ToList();
                        if (lstToolAssetQuantityDetail.Where(c => c.Quantity >= 1).Count() > 0)
                        {
                            SerialExistsForCheckOut = false;
                        }
                        else
                        {
                            SerialExistsForCheckOut = true;
                        }
                    }
                    else
                    {
                        SerialExistsForCheckOut = false;
                    }
                }
                return SerialExistsForCheckOut;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method is used to update serial description in ToolAssetQuantityDetails
        /// </summary>
        /// <param name="ToolGuid"></param>
        /// <param name="SerialNumber"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public bool UpdateSerialDescription(Guid ToolGuid, string SerialNumber, string Description)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var serials = context.ToolAssetQuantityDetails.Where(e => e.ToolGUID == ToolGuid && (e.SerialNumber ?? string.Empty) == (SerialNumber ?? string.Empty)).ToList();
                if (serials != null && serials.Any())
                {
                    foreach (var serial in serials)
                    {
                        serial.Description = Description;
                    }
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool DeleteToolAssetQuantityDetailRecords(string IDs, Int64 userid, Int64 RoomID, Int64 CompanyID, string WhatWhereAction)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<Guid> lstToolGUIDs = (from u in context.Database.SqlQuery<Guid>(@"CSP_DeleteToolAssetQuantityDetailById '" + IDs + "', " + CompanyID.ToString() + ", " + RoomID.ToString() + ", " + userid + ", '" + WhatWhereAction + "'")
                                           select u).ToList();

                if (lstToolGUIDs != null && lstToolGUIDs.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public bool GetSerialExistinCheckOut(Guid ToolGUID, string SerialNumber, long CompanyID, long RoomID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@SerialNumber", SerialNumber), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                double ToolSerailQuantity = 0;
                ToolSerailQuantity = context.Database.SqlQuery<double>("exec [GetSerialExistinCheckOut] @ToolGUID,@SerialNumber,@CompanyID,@RoomID", params1).FirstOrDefault();
                if (ToolSerailQuantity > 0)
                    return true;
                else
                    return false;
            }
        }

        public List<ToolQuantityLotSerialDTO> GetToolLocationsWithLotSerialsForMove(Guid ToolGUID, long RoomID, long CompanyID)
        {
            List<ToolQuantityLotSerialDTO> lstItemLocations = new List<ToolQuantityLotSerialDTO>();

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID) };
                lstItemLocations = (from u in context.Database.SqlQuery<ToolQuantityLotSerialDTO>("exec [GetToolLocationsWithLotSerialsForMove] @ToolGUID,@RoomID,@CompanyID", params1)
                                    select new ToolQuantityLotSerialDTO
                                    {

                                        ToolGUID = u.ToolGUID,
                                        Location = u.Location,
                                        BinID = u.BinID,
                                        ID = u.BinID ?? 0,
                                        Quantity = (u.Quantity ?? 0),
                                        SerialNumber = u.SerialNumber,
                                        SerialNumberTracking = u.SerialNumberTracking,
                                        LotSerialQuantity = (u.Quantity ?? 0),
                                        QuantityToMove = (u.Quantity ?? 0),
                                        LotOrSerailNumber = u.SerialNumber ?? string.Empty,
                                    }).ToList();
            }
            return lstItemLocations;
        }
        /// <summary>
        /// This method is used to get the tool's serial(which are not checkedout) for written off.
        /// </summary>
        /// <param name="ToolGUID"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <returns></returns>
        public List<CommonDTO> GetToolSerialForWrittenOff(Guid ToolGUID, long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var toolSerials = from element in context.ToolAssetQuantityDetails
                                  where element.ToolGUID == ToolGUID && element.RoomID == RoomID && element.CompanyID == CompanyID
                                  group element by element.SerialNumber
                  into groups
                                  select groups.OrderByDescending(p => p.ID).FirstOrDefault();

                if (toolSerials != null && toolSerials.Any())
                {
                    toolSerials = toolSerials.Where(e => e.Quantity.Value > 0);
                }

                return toolSerials.Select(u => new CommonDTO()
                {
                    Text = u.SerialNumber,
                    Value = u.SerialNumber
                }).AsParallel().ToList();
            }
        }


        public List<ToolAssetQuantityDetailDTO> GetToolAssetQuantityDetailsForConsume(bool IsFifo, Int64 ToolBinID, Int64 RoomID, Int64 CompanyId, Guid ToolGUID, Guid? OrderDetailGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@IsFifo", IsFifo), new SqlParameter("@ToolBinID", ToolBinID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId), new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@OrderDetailGUID", OrderDetailGUID ?? (object)DBNull.Value) };
            string Qry = "EXEC dbo.[GetQuantityByLIFOFIFO] @IsFifo,@ToolGUID,@ToolBinID,@RoomID,@CompanyID,@OrderDetailGUID";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolAssetQuantityDetailDTO>(Qry, params1).ToList();
            }
        }


        public List<ToolAssetQuantityDetailDTO> GetToolAssetQuantityDetailsForConsumeLotSr(Int64 RoomID, Int64 CompanyId, Guid ToolGUID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ToolGUID", ToolGUID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyId) };
            string Qry = "EXEC dbo.[GetQuantityByLIFOFIFOSr] @ToolGUID,@RoomID,@CompanyId";
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<ToolAssetQuantityDetailDTO>(Qry, params1).ToList();
            }
        }


        public IEnumerable<ToolAssetQuantityDetailDTO> GetQuantityByLIFOFIFO(bool IsFifo, Int64 ToolBinID, Int64 RoomID, Int64 CompanyId, Guid ToolGUID, Guid? OrderDetailGUID)
        {

            List<ToolAssetQuantityDetailDTO> lstFromDB = GetToolAssetQuantityDetailsForConsume(IsFifo, ToolBinID, RoomID, CompanyId, ToolGUID, OrderDetailGUID);

            string sOrderBy = "";
            if (IsFifo)
            {
                sOrderBy = "ID ASC";
            }
            else
            {
                sOrderBy = "ID DESC";
            }

            IEnumerable<ToolAssetQuantityDetailDTO> result = null;
            if (OrderDetailGUID == null)
            {
                result = lstFromDB.Where(t => (t.ToolGUID == ToolGUID && t.ToolBinID == ToolBinID && (t.QtyConsumable.GetValueOrDefault(0) > 0) && t.IsArchived == false && t.IsDeleted == false)).OrderBy(sOrderBy);
                return result;
            }
            else
            {
                result = lstFromDB.Where(t => (t.ToolGUID == ToolGUID && t.ToolBinID == ToolBinID && (t.QtyConsumable.GetValueOrDefault(0) > 0) && t.ToolAssetOrderDetailGUID == OrderDetailGUID && t.IsArchived == false && t.IsDeleted == false)).OrderBy(sOrderBy);
                return result;
            }
        }


        public IEnumerable<ToolAssetQuantityDetailDTO> GetQuantityByLIFOFIFOForLotSr(bool IsFifo, Int64 ToolBinID, Int64 RoomID, Int64 CompanyId, Guid ToolGUID, Guid? OrderDetailGUID, string LotNumber, string SerialNumber)
        {

            List<ToolAssetQuantityDetailDTO> lstFromDB = GetToolAssetQuantityDetailsForConsumeLotSr(RoomID, CompanyId, ToolGUID);


            string sOrderBy = "";
            if (IsFifo)
            {
                sOrderBy = "ID ASC";
            }
            else
            {
                sOrderBy = "ID DESC";
            }

            IEnumerable<ToolAssetQuantityDetailDTO> result = null;
            if (OrderDetailGUID == null)
            {
                //first customer 
                //result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && t.ConsignedQuantity.GetValueOrDefault(0) == 0) && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                result = lstFromDB.Where(t => (t.ToolGUID == ToolGUID && t.ToolBinID == ToolBinID
                    && (t.SerialNumber == SerialNumber || string.IsNullOrWhiteSpace(SerialNumber)) && (t.LotNumber == LotNumber || string.IsNullOrWhiteSpace(LotNumber))
                    && (t.Quantity > 0) && t.IsArchived == false && t.IsDeleted == false)).OrderBy(sOrderBy);

                List<ToolAssetQuantityDetailDTO> FinalResult = new List<ToolAssetQuantityDetailDTO>();
                foreach (var item in result)
                {
                    if (FinalResult.Where(x => x.GUID == item.GUID).Count() <= 0)
                    {
                        FinalResult.Add(item);
                    }
                }
                return FinalResult;
                //return result.Concat(resultcons);
            }
            else
            {
                //first customer
                //result = GetCachedData(ItemGUID, RoomID, CompanyId).Where(t => (t.ItemGUID == ItemGUID && t.BinID == BinID && (t.CustomerOwnedQuantity.GetValueOrDefault(0) > 0 && t.ConsignedQuantity.GetValueOrDefault(0) == 0) && t.OrderDetailGUID == OrderDetailGUID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false)).OrderBy(sOrderBy);
                result = lstFromDB.Where(t => (t.ToolGUID == ToolGUID && t.ToolBinID == ToolBinID
                    && (t.SerialNumber == SerialNumber || string.IsNullOrWhiteSpace(SerialNumber)) && (t.LotNumber == LotNumber || string.IsNullOrWhiteSpace(LotNumber))
                    && (t.Quantity > 0) && t.ToolAssetOrderDetailGUID == OrderDetailGUID && t.IsArchived == false && t.IsDeleted == false)).OrderBy(sOrderBy);

                List<ToolAssetQuantityDetailDTO> FinalResult = new List<ToolAssetQuantityDetailDTO>();

                foreach (var item in result)
                {
                    if (FinalResult.Where(x => x.GUID == item.GUID).Count() <= 0)
                    {
                        FinalResult.Add(item);
                    }
                }
                return FinalResult;
            }


        }

    }
}
