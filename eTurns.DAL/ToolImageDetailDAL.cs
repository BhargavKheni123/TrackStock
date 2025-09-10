using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eTurns.DAL
{
    public class ToolImageDetailDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public ToolImageDetailDAL()
        {

        }

        public ToolImageDetailDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ToolImageDetailDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Insert Record in the ToolImageDetail Table
        /// </summary>
        /// <param name="objDTO">DTO object</param>
        /// <returns>ID</returns>
        public long Insert(ToolImageDetailDTO objDTO)
        {

            objDTO.IsDeleted = objDTO.IsDeleted.HasValue ? objDTO.IsDeleted : false;
            objDTO.IsArchived = objDTO.IsArchived.HasValue ? objDTO.IsArchived : false;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Created = DateTimeUtility.DateTimeNow;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                ToolImageDetail obj = new ToolImageDetail();
                obj.ID = 0;
                obj.GUID = Guid.NewGuid();
                obj.ToolGuid = objDTO.ToolGuid;
                obj.SerialNumber = objDTO.SerialNumber;
                obj.ImagePath = objDTO.ImagePath;
                obj.ImageType = objDTO.ImageType;
                obj.CompanyId = objDTO.CompanyId;
                obj.RoomId = objDTO.RoomId;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.ReceivedOn = objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                obj.ReceivedOnWeb = objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                obj.IsDeleted = (bool)objDTO.IsDeleted;
                obj.IsArchived = (bool)objDTO.IsArchived;
                if (string.IsNullOrWhiteSpace(objDTO.AddedFrom))
                    obj.AddedFrom = objDTO.AddedFrom = "Web";
                obj.AddedFrom = objDTO.AddedFrom;
                if (string.IsNullOrWhiteSpace(objDTO.EditedFrom))
                    obj.EditedFrom = objDTO.EditedFrom = "Web";
                obj.EditedFrom = objDTO.EditedFrom;
                obj.WhatWhereAction = "From web";
                context.ToolImageDetails.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                objDTO.GUID = obj.GUID;

                return obj.ID;
            }
        }

        /// <summary>
        /// This method is used to get the tool images of specific tool
        /// </summary>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="ToolGuid"></param>
        /// <returns></returns>
        public IEnumerable<ToolImageDetailDTO> GetToolImages(Int64 RoomID, Int64 CompanyID, Guid ToolGuid)
        {
            IEnumerable<ToolImageDetailDTO> toolImages = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                toolImages = (from u in context.ToolImageDetails
                              where u.ToolGuid == ToolGuid && u.RoomId == RoomID && u.CompanyId == CompanyID && (u.IsDeleted ?? false) == false && (u.IsArchived ?? false) == false
                              select new ToolImageDetailDTO
                              {
                                  ImagePath = u.ImagePath,
                                  ImageType = u.ImageType,
                                  ID = u.ID,
                                  GUID = u.GUID
                              }).AsParallel().ToList().OrderByDescending(e => e.ID);

                return toolImages;
            }
        }

        public IEnumerable<ToolImageDetailDTO> GetSerialToolImages(int StartRowIndex, int MaxRows, long RoomID, long CompanyID, Guid ToolGuid, out long TotalRecordCount)
        {
            IEnumerable<ToolImageDetailDTO> toolImages = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                toolImages = (from u in context.ToolAssetQuantityDetails
                                  //join uc in context.ToolImageDetails on new { u.SerialNumber,u.ToolGUID } equals new { uc.SerialNumber,uc.ToolGuid }  into u_uc_join
                                  //from u_uc in u_uc_join.DefaultIfEmpty()
                              join f in context.ToolImageDetails on u.SerialNumber equals f.SerialNumber into fg
                              from fgi in fg.Where(f => f.ToolGuid == ToolGuid && f.IsDeleted == false && f.IsArchived == false).DefaultIfEmpty()
                              where u.ToolGUID == ToolGuid && u.RoomID == RoomID && u.CompanyID == CompanyID && u.IsDeleted == false && u.IsArchived == false && u.EditedOnAction != "Tool was written off from Web."
                              select new ToolImageDetailDTO
                              {
                                  SerialNumber = u.SerialNumber,
                                  ImagePath = fgi.ImagePath,
                                  ImageType = fgi.ImageType,
                                  ID = (long?)fgi.ID ?? 0,
                                  GUID = (Guid?)fgi.GUID ?? Guid.Empty
                              }).Distinct().AsParallel().ToList().OrderByDescending(e => e.ID);//.Skip(StartRowIndex).Take(MaxRows);

                TotalRecordCount = toolImages.Count();
                return toolImages.Skip(StartRowIndex).Take(MaxRows);
            }
        }

        public bool DeleteRecords(string IDs, long UserId, long CompanyID, long RoomID, Guid ToolGuid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                List<long> Ids = IDs.Split(',').Select(long.Parse).ToList();
                var toolImages = context.ToolImageDetails.Where(e => Ids.Contains(e.ID) && e.ToolGuid == ToolGuid && e.CompanyId == CompanyID && e.RoomId == RoomID).ToList();

                if (toolImages != null && toolImages.Any())
                {
                    foreach (var img in toolImages)
                    {
                        img.IsDeleted = true;
                        img.LastUpdatedBy = UserId;
                        img.WhatWhereAction = "From Web";
                        img.Updated = DateTimeUtility.DateTimeNow;
                        img.EditedFrom = "From Web";
                        //img.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    context.SaveChanges();
                }
            }
            return true;//return "Record(s) deleted successfully.";
        }

        public bool IsToolImageRecordExistForSerial(long CompanyID, long RoomID, Guid ToolGuid, string Serial)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var toolImage = context.ToolImageDetails.Where(e => e.ToolGuid == ToolGuid && e.SerialNumber == Serial && e.CompanyId == CompanyID && e.RoomId == RoomID).ToList();
                return (toolImage != null && toolImage.Any());
            }
        }

        /// <summary>
        /// This method is used to 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CompanyID"></param>
        /// <param name="RoomID"></param>
        /// <param name="ToolGuid"></param>
        /// <param name="Serial"></param>
        /// <param name="ImagePath"></param>
        /// <param name="ImageType"></param>
        /// <returns></returns>
        public long UpdateToolImageDetailForSerial(long UserId, long CompanyID, long RoomID, Guid ToolGuid, string Serial, string ImagePath, string ImageType)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var toolImage = context.ToolImageDetails.Where(e => e.ToolGuid == ToolGuid && e.CompanyId == CompanyID && e.RoomId == RoomID && e.SerialNumber == Serial).FirstOrDefault();

                if (toolImage != null && toolImage.ID > 0)
                {
                    toolImage.ImagePath = ImagePath;
                    toolImage.ImageType = ImageType;
                    toolImage.IsDeleted = false;
                    toolImage.IsArchived = false;
                    toolImage.LastUpdatedBy = UserId;
                    toolImage.Updated = DateTimeUtility.DateTimeNow;
                    toolImage.ReceivedOn = DateTimeUtility.DateTimeNow;
                    toolImage.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    toolImage.WhatWhereAction = "From Web";
                    toolImage.EditedFrom = "From Web";
                    context.SaveChanges();

                    return toolImage.ID;
                }
                return 0;
            }
        }

        /// <summary>
        /// This method is used to get the ToolMaster's ID field based on ToolImageDetails's ID field.
        /// </summary>
        /// <param name="ToolImageDetailId"></param>
        /// <returns></returns>
        public long GetToolIdBasedOnImageId(long ToolImageDetailId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var toolId = (from u in context.ToolImageDetails
                              join tm in context.ToolMasters on u.ToolGuid equals tm.GUID
                              where u.ID == ToolImageDetailId
                              select tm.ID).FirstOrDefault();


                return toolId;
            }
        }

        /// <summary>
        /// This method is used to get the ToolMaster's ID field based on ToolImageDetails's ID field.
        /// </summary>
        /// <param name="ToolImageDetailId"></param>
        /// <returns></returns>
        public bool DeleteToolImageBasedOnId(long Id, long UserId)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var toolImage = context.ToolImageDetails.Where(e => e.ID == Id).FirstOrDefault();
                if (toolImage != null && toolImage.ID > 0)
                {
                    toolImage.IsDeleted = true;
                    toolImage.LastUpdatedBy = UserId;
                    toolImage.Updated = DateTimeUtility.DateTimeNow;
                    toolImage.WhatWhereAction = "From Web";
                    toolImage.EditedFrom = "From Web";
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}
