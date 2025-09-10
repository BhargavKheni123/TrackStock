using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public class ToolDetailDAL : eTurnsBaseDAL
    {

        #region [Class Constructor]

        public ToolDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public bool IsKitItemDeletable(string KitItemGUID, Int64 RoomId, Int64 CompanyID)
        {
            ToolDetailDTO KitDetailDTO = GetRecordNew(KitItemGUID, RoomId, CompanyID, false, false, false);
            if (KitDetailDTO == null)
                return false;

            ToolMasterDTO KitMasterDTO = new ToolMasterDAL(base.DataBaseName).GetToolByGUIDPlain(KitDetailDTO.ToolGUID.GetValueOrDefault(Guid.Empty));
            if ((KitDetailDTO != null && KitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0) > 0) || (KitMasterDTO != null && KitMasterDTO.Quantity > 0))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get Paged Records from the KitDetail Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<ToolDetailDTO> GetAllRecordsByKitGUID(Guid KitGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return DB_GetKitRecords(KitGUID, null, null, RoomID, CompanyId);
            //return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsNeededItemDetail).OrderBy("ID DESC").Where(x => x.KitGUID == KitGUID);
        }
        public IEnumerable<ToolDetailDTO> GetAllRecordsByKitGUIDNew(Guid KitGUID, Int64 RoomID, Int64 CompanyId, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return DB_GetKitRecordsNew(KitGUID, null, null, RoomID, CompanyId);
            //return GetCachedData(RoomID, CompanyId, IsArchived, IsDeleted, IsNeededItemDetail).OrderBy("ID DESC").Where(x => x.KitGUID == KitGUID);
        }

        /// <summary>
        /// Get Particullar Record from the KitDetail by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ToolDetailDTO GetRecord(string GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return DB_GetKitRecords(null, null, Guid.Parse(GUID), RoomID, CompanyID).FirstOrDefault();
            //return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsNeededItemDetail).SingleOrDefault(t => t.GUID == Guid.Parse(GUID));
        }
        public Int64 Insert(ToolDetailDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objDTO.Created = DateTimeUtility.DateTimeNow;
                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                //objDTO.IsDeleted = false;
                //objDTO.IsArchived = false;

                ToolDetail obj = new ToolDetail();
                obj.ID = 0;
                obj.GUID = objDTO.GUID;
                obj.ToolItemGUID = objDTO.ToolItemGUID;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.QuantityPerKit = objDTO.QuantityPerKit;
                obj.QuantityReadyForAssembly = objDTO.QuantityReadyForAssembly;

                obj.Created = objDTO.Created;
                obj.Updated = objDTO.Updated;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.IsDeleted = objDTO.IsDeleted ?? false;
                obj.IsArchived = objDTO.IsArchived ?? false;
                obj.CompanyID = objDTO.CompanyID;
                if (objDTO.GUID == null || objDTO.GUID == Guid.Empty)
                {
                    objDTO.GUID = Guid.NewGuid();
                }
                obj.GUID = objDTO.GUID;
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                if (objDTO.AddedFrom == null || (string.IsNullOrWhiteSpace(objDTO.AddedFrom)))
                {
                    objDTO.AddedFrom = "Web";
                }
                if (objDTO.EditedFrom == null || (string.IsNullOrWhiteSpace(objDTO.EditedFrom)))
                {
                    objDTO.EditedFrom = "Web";
                }
                obj.AddedFrom = objDTO.AddedFrom;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.WhatWhereAction = objDTO.WhatWhereAction;
                //obj.QtyToMeetDemand = GetQtyOnDemand(Convert.ToString(objDTO.KitGUID), objDTO.QuantityPerKit.GetValueOrDefault(0), objDTO.AvailableItemsInWIP.GetValueOrDefault(0), objDTO.Room, objDTO.CompanyID);
                context.ToolDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID ?? Guid.Empty;



                if (objDTO.ID > 0)
                {
                    //Get Cached-Media

                    // GetSumOfQtyOnDemandForItem(Convert.ToString(objDTO.ItemGUID), objDTO.Room, objDTO.CompanyID);
                }

                return obj.ID;
            }

        }

        /// <summary>
        /// Edit Particullar record provided in the DTO
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public bool Edit(ToolDetailDTO objDTO)
        {
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {

                ToolDetail obj = context.ToolDetails.SingleOrDefault(x => x.GUID == objDTO.GUID);
                // obj.ID = objDTO.ID;
                obj.QuantityPerKit = objDTO.QuantityPerKit;
                obj.Updated = objDTO.Updated;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Room = objDTO.Room;
                obj.CompanyID = objDTO.CompanyID;
                obj.AvailableItemInWIP = Convert.ToInt64(objDTO.AvailableItemsInWIP);
                obj.ReceivedOn = objDTO.ReceivedOn;
                obj.EditedFrom = objDTO.EditedFrom;
                obj.ToolGUID = objDTO.ToolGUID;
                obj.WhatWhereAction = objDTO.WhatWhereAction;
                obj.IsDeleted = objDTO.IsDeleted ?? false;
                obj.IsArchived = objDTO.IsArchived ?? false;
                obj.CompanyID = objDTO.CompanyID;


                obj.QuantityReadyForAssembly = Convert.ToInt64((int)(objDTO.AvailableItemsInWIP.GetValueOrDefault(0) / objDTO.QuantityPerKit.GetValueOrDefault(0)));


                context.SaveChanges();


                //GetSumOfQtyOnDemandForItem(Convert.ToString(objDTO.ItemGUID), objDTO.Room, objDTO.CompanyID);
                //Get Cached-Media

                return true;
            }
        }

        /// <summary>
        /// Delete Multiple Records
        /// </summary>
        /// <param name="IDs"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool DeleteRecordsExcept(string IDs, Guid KitGUID, Int64 userid, Int64 CompanyID)
        {
            using (eTurnsEntities context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (IDs != "")
                {
                    string strQuery = "";
                    strQuery += "UPDATE ToolDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE ToolGUID = '" + KitGUID.ToString() + "' AND ID Not in( ";
                    foreach (var item in IDs.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            strQuery += item.ToString() + ",";
                        }
                    }
                    strQuery = strQuery.Substring(0, strQuery.Length - 1);
                    strQuery += ");";
                    context.Database.ExecuteSqlCommand(strQuery);




                }
                else
                {
                    string strQuery = "";
                    strQuery += "UPDATE ToolDetail SET Updated = getutcdate() , LastUpdatedBy = " + userid.ToString() + ", IsDeleted=1, ReceivedOn=GETUTCDATE(),EditedFrom='Web' WHERE ToolGUID = '" + KitGUID.ToString() + "'";
                    context.Database.ExecuteSqlCommand(strQuery);
                }

                return true;
            }
        }

        public double GetQtyOnDemand(string ItemGUID, double QtyPerKit, double AvailableItemsInWIP, Int64 RoomId, Int64 CompanyID)
        {
            double QtyToMeetDemand = 0;
            ItemMasterDAL obj = new ItemMasterDAL(base.DataBaseName);
            Guid itemGUID1 = Guid.Empty;
            ItemMasterDTO objDTO = null;
            if (Guid.TryParse(ItemGUID, out itemGUID1))
            {
                objDTO = obj.GetItemWithoutJoins(null, itemGUID1);
            }
            if (objDTO != null)
            {
                QtyPerKit = QtyPerKit * objDTO.SuggestedOrderQuantity ?? 0;

                if ((QtyPerKit - AvailableItemsInWIP) > 0)
                    QtyToMeetDemand = QtyPerKit - AvailableItemsInWIP;
                else
                    QtyToMeetDemand = 0;
            }
            return QtyToMeetDemand;
        }

        /// <summary>
        /// Get Paged Records from the OrderMaster Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        private IEnumerable<ToolDetailDTO> DB_GetKitRecords(Guid? KitGuid, Int64? ID, Guid? GUID, Int64? RoomID, Int64? CompanyID)
        {

            //string strQuer = @"";


            //if (KitGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += "@KitGUID= '" + KitGuid.GetValueOrDefault(Guid.Empty) + "'";
            //}

            //if (ID.GetValueOrDefault(0) > 0)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += " @ID=" + ID.GetValueOrDefault(0);
            //}
            //if (GUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += "@GUID= '" + GUID.GetValueOrDefault(Guid.Empty) + "'";
            //}


            //if (RoomID.GetValueOrDefault(0) > 0)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += " @RoomID=" + RoomID.GetValueOrDefault(0);
            //}

            //if (CompanyID.GetValueOrDefault(0) > 0)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += " @CompanyID=" + CompanyID.GetValueOrDefault(0);
            //}

            //strQuer = @"EXEC [Kit_GetToolKitDetailData] " + strQuer;

            //IEnumerable<ToolDetailDTO> obj = ExecuteQuery(strQuer);

            //return obj;

            IEnumerable<ToolDetailDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@KitGUID",(KitGuid.GetValueOrDefault(Guid.Empty) == Guid.Empty ? (object)DBNull.Value : KitGuid)),
                                                   new SqlParameter("@ID", ID ?? (object)DBNull.Value),
                                                   new SqlParameter("@GUID", (GUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? (object)DBNull.Value : GUID)),
                                                   new SqlParameter("@RoomID", RoomID?? (object)DBNull.Value),
                                                   new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value)};

                obj = (from u in context.Database.SqlQuery<ToolDetailDTO>("exec Kit_GetToolKitDetailData @KitGUID,@ID,@GUID,@RoomID,@CompanyID", params1)
                       select new ToolDetailDTO
                       {
                           ID = u.ID,
                           ToolItemGUID = u.ToolItemGUID,
                           ToolGUID = u.ToolGUID,
                           QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                           QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                           AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                           Created = u.Created,
                           Updated = u.Updated,
                           CreatedBy = u.CreatedBy,
                           LastUpdatedBy = u.LastUpdatedBy,
                           Room = u.Room,
                           IsDeleted = u.IsDeleted,
                           IsArchived = u.IsArchived,
                           CompanyID = u.CompanyID,
                           GUID = u.GUID,
                           CreatedByName = u.CreatedByName,
                           UpdatedByName = u.UpdatedByName,
                           RoomName = u.RoomName,
                           ReceivedOn = u.ReceivedOn,
                           ReceivedOnWeb = u.ReceivedOnWeb,
                           AddedFrom = u.AddedFrom,
                           EditedFrom = u.EditedFrom,
                           ToolName = u.ToolName,
                           Serial = u.Serial,
                           Quantity = u.Quantity,
                           CheckedOutQTY = u.CheckedOutQTY,
                           CheckedOutMQTY = u.CheckedOutMQTY,
                           Cost = u.Cost,
                           SerialNumberTracking = u.SerialNumberTracking
                       }).AsParallel().ToList();

                return obj;
            }

        }

        /// <summary>
        /// Executer Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IEnumerable<ToolDetailDTO> ExecuteQuery(string query)
        {
            IEnumerable<ToolDetailDTO> obj = null;
            try
            {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    obj = (from u in context.Database.SqlQuery<ToolDetailDTO>(query)
                           select new ToolDetailDTO
                           {
                               ID = u.ID,
                               ToolItemGUID = u.ToolItemGUID,
                               ToolGUID = u.ToolGUID,
                               QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                               QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                               AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                               Created = u.Created,
                               Updated = u.Updated,
                               CreatedBy = u.CreatedBy,
                               LastUpdatedBy = u.LastUpdatedBy,
                               Room = u.Room,
                               IsDeleted = u.IsDeleted,
                               IsArchived = u.IsArchived,
                               CompanyID = u.CompanyID,
                               GUID = u.GUID,
                               CreatedByName = u.CreatedByName,
                               UpdatedByName = u.UpdatedByName,
                               RoomName = u.RoomName,
                               ReceivedOn = u.ReceivedOn,
                               ReceivedOnWeb = u.ReceivedOnWeb,
                               AddedFrom = u.AddedFrom,
                               EditedFrom = u.EditedFrom,
                               ToolName = u.ToolName,
                               Serial = u.Serial,
                               Quantity = u.Quantity,
                               CheckedOutQTY = u.CheckedOutQTY,
                               CheckedOutMQTY = u.CheckedOutMQTY,
                               Cost = u.Cost,
                               SerialNumberTracking = u.SerialNumberTracking
                           }).AsParallel().ToList();

                    return obj;
                }
            }
            finally
            {
                obj = null;
            }
        }
        private IEnumerable<ToolDetailDTO> DB_GetKitRecordsNew(Guid? KitGuid, Int64? ID, Guid? GUID, Int64? RoomID, Int64? CompanyID)
        {

            //string strQuer = @"";


            //if (KitGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += "@KitGUID= '" + KitGuid.GetValueOrDefault(Guid.Empty) + "'";
            //}

            //if (ID.GetValueOrDefault(0) > 0)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += " @ID=" + ID.GetValueOrDefault(0);
            //}
            //if (GUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += "@GUID= '" + GUID.GetValueOrDefault(Guid.Empty) + "'";
            //}


            //if (RoomID.GetValueOrDefault(0) > 0)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += " @RoomID=" + RoomID.GetValueOrDefault(0);
            //}

            //if (CompanyID.GetValueOrDefault(0) > 0)
            //{
            //    if (strQuer.Length > 0)
            //        strQuer += ", ";
            //    strQuer += " @CompanyID=" + CompanyID.GetValueOrDefault(0);
            //}

            //strQuer = @"EXEC [Kit_GetToolKitDetailDataNew] " + strQuer;

            //IEnumerable<ToolDetailDTO> obj = ExecuteQuery(strQuer);

            //return obj;

            IEnumerable<ToolDetailDTO> obj = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@KitGUID",(KitGuid.GetValueOrDefault(Guid.Empty) == Guid.Empty ? (object)DBNull.Value : KitGuid)),
                                                   new SqlParameter("@ID", ID ?? (object)DBNull.Value),
                                                   new SqlParameter("@GUID", (GUID.GetValueOrDefault(Guid.Empty) == Guid.Empty ? (object)DBNull.Value : GUID)),
                                                   new SqlParameter("@RoomID", RoomID?? (object)DBNull.Value),
                                                   new SqlParameter("@CompanyID", CompanyID ?? (object)DBNull.Value)};

                obj = (from u in context.Database.SqlQuery<ToolDetailDTO>("exec Kit_GetToolKitDetailDataNew @KitGUID,@ID,@GUID,@RoomID,@CompanyID", params1)
                       select new ToolDetailDTO
                       {
                           ID = u.ID,
                           ToolItemGUID = u.ToolItemGUID,
                           ToolGUID = u.ToolGUID,
                           QuantityPerKit = u.QuantityPerKit.GetValueOrDefault(0),
                           QuantityReadyForAssembly = u.QuantityReadyForAssembly.GetValueOrDefault(0),
                           AvailableItemsInWIP = u.AvailableItemsInWIP.GetValueOrDefault(0),
                           Created = u.Created,
                           Updated = u.Updated,
                           CreatedBy = u.CreatedBy,
                           LastUpdatedBy = u.LastUpdatedBy,
                           Room = u.Room,
                           IsDeleted = u.IsDeleted,
                           IsArchived = u.IsArchived,
                           CompanyID = u.CompanyID,
                           GUID = u.GUID,
                           CreatedByName = u.CreatedByName,
                           UpdatedByName = u.UpdatedByName,
                           RoomName = u.RoomName,
                           ReceivedOn = u.ReceivedOn,
                           ReceivedOnWeb = u.ReceivedOnWeb,
                           AddedFrom = u.AddedFrom,
                           EditedFrom = u.EditedFrom,
                           ToolName = u.ToolName,
                           Serial = u.Serial,
                           Quantity = u.Quantity,
                           CheckedOutQTY = u.CheckedOutQTY,
                           CheckedOutMQTY = u.CheckedOutMQTY,
                           Cost = u.Cost,
                           SerialNumberTracking = u.SerialNumberTracking
                       }).AsParallel().ToList();
                return obj;
            }
        }

        public ToolDetailDTO GetRecordNew(string GUID, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, bool IsNeededItemDetail)
        {
            return DB_GetKitRecordsNew(null, null, Guid.Parse(GUID), RoomID, CompanyID).FirstOrDefault();
            //return GetCachedData(RoomID, CompanyID, IsArchived, IsDeleted, IsNeededItemDetail).SingleOrDefault(t => t.GUID == Guid.Parse(GUID));
        }

    }
}


