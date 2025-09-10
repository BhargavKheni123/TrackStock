using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eTurns.DAL
{
    public class ToolAssetMoveMaterialDAL : eTurnsBaseDAL
    {
        public ToolAssetMoveMaterialDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public bool MoveTool(ToolAssetMoveMaterialDTO objDTO)
        {
            LocationMasterDAL objItemLocationDetailsDAL = new LocationMasterDAL(base.DataBaseName);

            #region Insert Location or Material Staging


            if (objDTO.MoveType == (int)MoveType.InvToInv || objDTO.MoveType == (int)MoveType.StagToInv)
            {
                ToolLocationDetailsDAL objToolLocationDetailDAL = new ToolLocationDetailsDAL(base.DataBaseName);
                List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetailDAL.GetToolLocationsWithBlankByToolGUID(objDTO.RoomID, objDTO.CompanyID, false, false, objDTO.ToolGUID.GetValueOrDefault(Guid.Empty)).ToList();
                ToolLocationDetailsDTO objBinMasterDTO = new ToolLocationDetailsDTO();
                if (lstToolLocationDetailsDTO != null && lstToolLocationDetailsDTO.Count > 0)
                {
                    objBinMasterDTO = lstToolLocationDetailsDTO.Where(x => (x.ToolLocationName ?? string.Empty).ToLower() == (objDTO.DestinationLocation ?? string.Empty).ToLower()).FirstOrDefault();
                    if (objBinMasterDTO == null || objBinMasterDTO.ID <= 0)
                    {
                        objBinMasterDTO = objToolLocationDetailDAL.GetToolLocation(objDTO.ToolGUID.GetValueOrDefault(Guid.Empty), objDTO.DestinationLocation, objDTO.RoomID, objDTO.CompanyID, objDTO.UpdatedBy, "MoveMaterial>>Destination");
                    }
                }
                objDTO.DestToolBinID = objBinMasterDTO.ID;
            }


            #endregion

            if (objDTO.MoveType == (int)MoveType.InvToInv)
            {
                MoveInvetoryToInventoryLocation(objDTO);
            }


            return true;
        }

        /// <summary>
        /// InsertMoveMaterial
        /// </summary>
        /// <param name="objMove"></param>
        /// <returns></returns>
        public Int64 InsertMoveMaterial(ToolAssetMoveMaterialDTO objMove)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetMoveMaterialMaster obj = new ToolAssetMoveMaterialMaster()
                {

                    DestToolBinID = objMove.DestToolBinID,
                    ToolGUID = objMove.ToolGUID,
                    MoveQuanity = objMove.MoveQuanity,
                    MoveType = objMove.MoveType,

                    SourceToolBinID = objMove.SourceToolBinID,


                    RoomID = objMove.RoomID,
                    CompanyID = objMove.CompanyID,
                    UpdatedBy = objMove.UpdatedBy,
                    CreatedBy = objMove.CreatedBy,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    GUID = Guid.NewGuid(),
                    IsArchived = false,
                    IsDeleted = false,
                    ID = 0,
                    AddedFrom = (objMove.AddedFrom == null ? "Web" : objMove.AddedFrom),
                    EditedFrom = (objMove.EditedFrom == null ? "Web" : objMove.EditedFrom),
                    ReceivedOn = objMove.ReceivedOn,
                    ReceivedOnWeb = objMove.ReceivedOnWeb

                };
                context.ToolAssetMoveMaterialMasters.Add(obj);
                context.SaveChanges();
                objMove.ID = obj.ID;
                objMove.GUID = obj.GUID;

            }

            return objMove.ID;
        }

        /// <summary>
        /// InsertMoveMaterialDetail
        /// </summary>
        /// <param name="objMoveDtl"></param>
        /// <returns></returns>
        public Int64 InsertMoveMaterialDetail(ToolAssetMoveMaterialDetailDTO objMoveDtl)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolAssetMoveMaterialDetail obj = new ToolAssetMoveMaterialDetail()
                {


                    Quantity = objMoveDtl.Quantity,
                    ToolAssetQtyDetailGUID_Destination = objMoveDtl.ToolAssetQtyDetailGUID_Destination,
                    ToolAssetQtyDetailGUID_Source = objMoveDtl.ToolAssetQtyDetailGUID_Source,
                    MoveMaterialGuid = objMoveDtl.MoveMaterialGuid,
                    SerialNumber = objMoveDtl.SerialNumber,
                    LotNumber = objMoveDtl.LotNumber,
                    RoomID = objMoveDtl.RoomID,
                    CompanyID = objMoveDtl.CompanyID,
                    UpdatedBy = objMoveDtl.UpdatedBy,
                    CreatedBy = objMoveDtl.CreatedBy,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    GUID = Guid.NewGuid(),
                    IsArchived = false,
                    IsDeleted = false,
                    ID = 0,
                    AddedFrom = (objMoveDtl.AddedFrom == null ? "Web" : objMoveDtl.AddedFrom),
                    EditedFrom = (objMoveDtl.EditedFrom == null ? "Web" : objMoveDtl.EditedFrom),
                    ReceivedOn = objMoveDtl.ReceivedOn,
                    ReceivedOnWeb = objMoveDtl.ReceivedOnWeb
                };
                context.ToolAssetMoveMaterialDetails.Add(obj);
                context.SaveChanges();
                objMoveDtl.ID = obj.ID;
            }

            return objMoveDtl.ID;
        }

        #region [Private Method]

        private void MoveInvetoryToInventoryLocation(ToolAssetMoveMaterialDTO objDTO)
        {
            List<ToolAssetQuantityDetailDTO> lstSourceInvLocs = GetSourceLocationsFromGeneralInventory(objDTO);
            List<ToolAssetQuantityDetailDTO> lstDestInvLocs = GetDestinationLocationsForGeneralInventory(objDTO, lstSourceInvLocs);
            UpdateSourceAndDestinationLocationQty(objDTO, lstSourceInvLocs, lstDestInvLocs);
            List<ToolAssetMoveMaterialDetailDTO> lstMoveMaterialDtlDTO = GetMoveMaterialDeteailsDTO(objDTO, lstDestInvLocs);

            #region Save and Insert Source And Destination Location
            ToolAssetQuantityDetailDAL objItmLocDtlsDAL = new ToolAssetQuantityDetailDAL(base.DataBaseName);

            int CountSource = lstSourceInvLocs.Count;
            int CountDesting = lstDestInvLocs.Count;
            int CountMoveMtrDtl = lstMoveMaterialDtlDTO.Count;

            if (CountSource == CountDesting && CountDesting == CountMoveMtrDtl)
            {
                for (int i = 0; i < CountSource; i++)
                {
                    lstSourceInvLocs[i].Updated = DateTimeUtility.DateTimeNow;
                    lstSourceInvLocs[i].UpdatedBy = objDTO.UpdatedBy;
                    lstSourceInvLocs[i].MoveQuantity = lstSourceInvLocs[i].MoveQuantity;
                    lstSourceInvLocs[i].EditedFrom = "Web";
                    lstSourceInvLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    lstSourceInvLocs[i].EditedOnAction = "Move Material>>Update";
                    objItmLocDtlsDAL.Edit_Light(lstSourceInvLocs[i]);

                    lstDestInvLocs[i].AddedFrom = "Web";
                    lstDestInvLocs[i].EditedFrom = "Web";
                    lstDestInvLocs[i].ReceivedOn = DateTimeUtility.DateTimeNow;
                    lstDestInvLocs[i].ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    lstDestInvLocs[i].EditedOnAction = "Move Material";
                    objItmLocDtlsDAL.Insert_Light(lstDestInvLocs[i]);

                    lstMoveMaterialDtlDTO[i].ToolAssetQtyDetailGUID_Source = lstSourceInvLocs[i].GUID;
                    lstMoveMaterialDtlDTO[i].ToolAssetQtyDetailGUID_Destination = lstDestInvLocs[i].GUID;
                }

                objDTO.AddedFrom = "Web";
                objDTO.EditedFrom = "Web";
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                InsertMoveMaterial(objDTO);
                foreach (var item in lstMoveMaterialDtlDTO)
                {
                    item.MoveMaterialGuid = objDTO.GUID;
                    item.AddedFrom = "Web";
                    item.EditedFrom = "Web";
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    InsertMoveMaterialDetail(item);
                }
            }
            #endregion

        }

        private List<ToolAssetQuantityDetailDTO> GetSourceLocationsFromGeneralInventory(ToolAssetMoveMaterialDTO objDTO)
        {
            List<ToolAssetQuantityDetailDTO> lstSourceInvLocs = new List<ToolAssetQuantityDetailDTO>();

            #region Take Records up to sum of Move qty

            List<ToolAssetQuantityDetailDTO> objItemLocDtlsList = null;
            ToolAssetQuantityDetailDAL objItmLocDtlsDAL = new ToolAssetQuantityDetailDAL(base.DataBaseName);
            objItemLocDtlsList = objItmLocDtlsDAL.GetQuantityByLIFOFIFOForLotSr(false, objDTO.SourceToolBinID, objDTO.RoomID, objDTO.CompanyID, objDTO.ToolGUID.GetValueOrDefault(Guid.Empty), null, objDTO.LotNumber, objDTO.SerialNumber).ToList();

            foreach (var item in objItemLocDtlsList)
            {
                if (item.Quantity > 0)
                {
                    ToolAssetQuantityDetailDTO objItemLocDetailDTO = new ToolAssetQuantityDetailDTO();
                    double SumSourceQty = lstSourceInvLocs.Sum(x => x.Quantity);
                    if (SumSourceQty < objDTO.MoveQuanity)
                    {
                        item.ToolAssetOrderDetailGUID = null;
                        lstSourceInvLocs.Add(item);
                    }
                }
            }
            #endregion



            return lstSourceInvLocs;
        }

        private void UpdateSourceAndDestinationLocationQty(ToolAssetMoveMaterialDTO objDTO, List<ToolAssetQuantityDetailDTO> SourceLocation, List<ToolAssetQuantityDetailDTO> DestinatonLocation)
        {
            double sumSourceInvLocs = SourceLocation.Sum(x => x.Quantity);
            double sumQty = 0;
            for (int i = 0; i < SourceLocation.Count; i++)
            {
                sumQty += SourceLocation[i].Quantity;

                if (sumQty > objDTO.MoveQuanity)
                {
                    double diff = sumQty - objDTO.MoveQuanity;


                    if (SourceLocation[i].Quantity > 0)
                    {
                        if (SourceLocation[i].Quantity <= diff)
                        {
                            DestinatonLocation[i].Quantity = SourceLocation[i].Quantity;
                            SourceLocation[i].Quantity = 0;
                        }
                        else if (SourceLocation[i].Quantity > diff)
                        {
                            DestinatonLocation[i].Quantity = SourceLocation[i].Quantity - diff;
                            SourceLocation[i].Quantity = diff;
                        }
                    }
                }
                else
                {
                    DestinatonLocation[i].Quantity = SourceLocation[i].Quantity;
                    SourceLocation[i].Quantity = 0;

                }
                SourceLocation[i].MoveQuantity = DestinatonLocation[i].Quantity;
            }

        }

        private List<ToolAssetQuantityDetailDTO> GetDestinationLocationsForGeneralInventory(ToolAssetMoveMaterialDTO objDTO, List<ToolAssetQuantityDetailDTO> SourceInvLocList)
        {
            List<ToolAssetQuantityDetailDTO> lstDestInvLocs = new List<ToolAssetQuantityDetailDTO>();
            foreach (var item in SourceInvLocList)
            {
                ToolAssetQuantityDetailDTO obj = new ToolAssetQuantityDetailDTO();

                obj.ToolBinID = objDTO.DestToolBinID;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.UpdatedBy = objDTO.UpdatedBy;
                obj.RoomID = objDTO.RoomID;
                obj.CompanyID = objDTO.CompanyID;

                obj.ToolGUID = item.ToolGUID;
                obj.Quantity = 0;
                obj.SerialNumber = item.SerialNumber;
                obj.LotNumber = item.LotNumber;
                obj.Cost = item.Cost;
                obj.ReceivedDate = item.ReceivedDate;
                obj.SerialNumberTracking = item.SerialNumberTracking;
                obj.LotNumberTracking = item.LotNumberTracking;

                obj.ToolName = item.ToolName;
                obj.Location = string.Empty;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;

                obj.IsArchived = false; obj.IsDeleted = false;

                obj.ID = 0;
                obj.GUID = Guid.Empty;


                obj.UDF1 = item.UDF1;
                obj.UDF2 = item.UDF2;
                obj.UDF3 = item.UDF3;
                obj.UDF4 = item.UDF4;
                obj.Action = string.Empty;
                obj.UpdatedByName = string.Empty;
                obj.CreatedByName = string.Empty;
                obj.RoomName = "";
                //obj.mode = "Move Material";

                lstDestInvLocs.Add(obj);

            }

            return lstDestInvLocs;
        }

        private List<ToolAssetMoveMaterialDetailDTO> GetMoveMaterialDeteailsDTO(ToolAssetMoveMaterialDTO objDTO, List<ToolAssetQuantityDetailDTO> lstInvLocs)
        {
            List<ToolAssetMoveMaterialDetailDTO> lstMoveMaterialDtlDTO = new List<ToolAssetMoveMaterialDetailDTO>();

            ToolAssetMoveMaterialDetailDTO objMoveMtrlDtlDTO = null;
            foreach (var item in lstInvLocs)
            {

                objMoveMtrlDtlDTO = new ToolAssetMoveMaterialDetailDTO()
                {
                    CompanyID = objDTO.CompanyID,
                    CreatedBy = objDTO.CreatedBy,
                    CreatedOn = DateTime.Now,
                    GUID = Guid.Empty,
                    ID = 0,
                    ToolAssetQtyDetailGUID_Source = item.GUID,
                    LotNumber = item.LotNumber,
                    RoomID = objDTO.RoomID,
                    SerialNumber = item.SerialNumber,
                    Quantity = item.Quantity,
                    UpdatedBy = objDTO.UpdatedBy,
                    UpdatedOn = DateTime.Now,
                };

                lstMoveMaterialDtlDTO.Add(objMoveMtrlDtlDTO);
            }


            return lstMoveMaterialDtlDTO;
        }

        #endregion
    }
}
