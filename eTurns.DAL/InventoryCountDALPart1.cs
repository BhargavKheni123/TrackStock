using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DAL
{
    public class InventoryCountDAL : eTurnsBaseDAL
    {
        public List<CommonDTO> GetInventoryListCount(long companyId, long roomid)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuery = @"Select ic.ID,ic.CountName as Text,case when CountType='M' then 'Manual' else case when CountType='O' then 'Open' else case when CountType='C' then 'Closed' end end end as CountType ,case when CountStatus='M' then 'Manual' else case when CountStatus='O' then 'Open' else case when CountStatus='C' then 'Closed' end end end as CountStatus ,Count(icd.GUID) As inventorycount from InventoryCount IC inner join  InventoryCountDetail ICD on IC.GUID =ICD.InventoryCountGUID Where ic.IsClosed=0 and ic.IsDeleted=0 and ic.IsArchived=0 and ic.CompanyId=" + companyId + " and ic.RoomId=" + roomid + "  and icd.RoomId=" + roomid + " and icd.CompanyId = " + companyId + " and icd.IsApplied=0 and icd.IsDeleted =0  group by ic.ID,ic.countname,CountType,CountStatus";
                return (from u in context.ExecuteStoreQuery<CommonDTO>(strQuery)
                        select new CommonDTO
                        {
                            Text = u.Text.ToString(),
                            Count = u.Count
                        }).ToList();
            }

        }

        public void ApplyCountHeader(Guid ICGuid, Int64 RoomID, Int64 CompanyID, string EditedFrom)
        {
            InventoryCountDTO objInvCount = new InventoryCountDTO();
            objInvCount = GetInventoryCountByGUId(ICGuid, RoomID, CompanyID);
            if (objInvCount != null)
            {
                List<InventoryCountDetailDTO> LstDtl = new List<InventoryCountDetailDTO>();
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    IEnumerable<InventoryCountDetailDTO> obj = (from u in context.ExecuteStoreQuery<InventoryCountDetailDTO>(@"SELECT ID,IsApplied FROM InventoryCountDetail where CompanyID = " + CompanyID.ToString() + @" AND RoomID = " + RoomID.ToString() + " AND InventoryCountGUID='" + ICGuid.ToString() + "' and IsApplied=0 and IsDeleted=0 and IsArchived=0 ")
                                                                select new InventoryCountDetailDTO
                                                                {
                                                                    ID = u.ID,
                                                                    IsApplied = u.IsApplied,
                                                                }).AsParallel().ToList();

                    InventoryCount objEntCnt = context.InventoryCounts.Where(x => x.ID == objInvCount.ID).FirstOrDefault();
                    if (obj != null && obj.Count() == 0)
                    {
                        if (objEntCnt != null)
                        {
                            objEntCnt.Updated = DateTimeUtility.DateTimeNow;
                            objEntCnt.IsApplied = true;
                            if (string.IsNullOrWhiteSpace(EditedFrom))
                            {
                                objEntCnt.EditedFrom = "Web";
                            }
                            else
                            {
                                objEntCnt.EditedFrom = EditedFrom;
                            }
                            objEntCnt.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                    }
                    else
                    {
                        objEntCnt.Updated = DateTimeUtility.DateTimeNow;
                        objEntCnt.IsApplied = false;
                        if (string.IsNullOrWhiteSpace(EditedFrom))
                        {
                            objEntCnt.EditedFrom = "Web";
                        }
                        else
                        {
                            objEntCnt.EditedFrom = EditedFrom;
                        }
                        objEntCnt.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }

                    context.SaveChanges();

                }
            }
        }
    }
}
