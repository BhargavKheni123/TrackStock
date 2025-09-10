using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace eTurns.DAL
{
    public class CycleCountSettingDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        public CycleCountSettingDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CycleCountSettingDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        public CycleCountSettingDTO GetRecord(Int64 RoomID, Int64 CompanyID)
        {
            CycleCountSettingDTO objCycleCountSettingDTO = null;
            DateTime dtendate = new DateTime(DateTime.Now.Year, 12, 31);
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                objCycleCountSettingDTO = (from u in context.CycleCountSettings
                                           join rm in context.Rooms on u.RoomId equals rm.ID into u_rm_join
                                           from u_rm in u_rm_join.DefaultIfEmpty()
                                           join uc in context.UserMasters on u.CreatedBy equals uc.ID into u_uc_join
                                           from u_uc in u_uc_join.DefaultIfEmpty()
                                           join uu in context.UserMasters on u.LastUpdatedBy equals uu.ID into u_uu_join
                                           from u_uu in u_uu_join.DefaultIfEmpty()
                                           where u.RoomId == RoomID && u.CompanyId == CompanyID
                                           select new CycleCountSettingDTO
                                           {
                                               ID = u.ID,
                                               AClassFrequency = u.AClassFrequency,
                                               BClassFrequency = u.BClassFrequency,
                                               CClassFrequency = u.CClassFrequency,
                                               DClassFrequency = u.DClassFrequency,
                                               EClassFrequency = u.EClassFrequency,
                                               YearStartDate = u.YearStartDate,
                                               YearEndDate = u.YearEndDate ?? dtendate,
                                               EmailAddressesPreCountNotification = u.EmailAddressesPreCountNotification,
                                               EmailAddressesDailyCycle = u.EmailAddressesDailyCycle,
                                               Created = u.Created,
                                               Updated = u.Updated,
                                               CreatedBy = u.CreatedBy,
                                               LastUpdatedBy = u.LastUpdatedBy,
                                               IsDeleted = u.IsDeleted,
                                               IsArchived = u.IsArchived,
                                               GUID = u.GUID,
                                               CompanyId = u.CompanyId,
                                               RoomId = u.RoomId,
                                               CreatedByName = u_uc.UserName,
                                               UpdatedByName = u_uu.UserName,
                                               RoomName = u_rm.RoomName,
                                               RecurrringDays = u.RecurrringDays,
                                               CycleCountTime = u.CycleCountTime,
                                               NextRunDate = u.NextRunDate,
                                               CountFrequencyType = u.CountFrequencyType,
                                               CycleCountsPerCycle = u.CycleCountsPerCycle,
                                               IsActive = u.IsActive,
                                               RecurringType = u.RecurringType,
                                               MissedItemsEmailTime = u.MissedItemsEmailTime,
                                               MissedItemEmailPriorHours = u.MissedItemEmailPriorHours,
                                               CountType = u.CountType
                                           }).FirstOrDefault();
            }
            return objCycleCountSettingDTO;
        }
        public CycleCountSettingDTO SaveInventoryCountSettings(CycleCountSettingDTO objCycleCountSettingDTO)
        {
            CycleCountSetting objCycleCountSetting = null;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (objCycleCountSettingDTO.ID > 0)
                {
                    objCycleCountSetting = context.CycleCountSettings.FirstOrDefault(t => t.ID == objCycleCountSettingDTO.ID);
                    if (objCycleCountSetting != null)
                    {
                        objCycleCountSetting.AClassFrequency = objCycleCountSettingDTO.AClassFrequency;
                        objCycleCountSetting.BClassFrequency = objCycleCountSettingDTO.BClassFrequency;
                        objCycleCountSetting.CClassFrequency = objCycleCountSettingDTO.CClassFrequency;
                        objCycleCountSetting.DClassFrequency = objCycleCountSettingDTO.DClassFrequency;
                        objCycleCountSetting.EClassFrequency = objCycleCountSettingDTO.EClassFrequency;
                        objCycleCountSetting.FClassFrequency = objCycleCountSettingDTO.FClassFrequency;
                        objCycleCountSetting.EmailAddressesDailyCycle = objCycleCountSettingDTO.EmailAddressesDailyCycle ?? string.Empty;
                        objCycleCountSetting.EmailAddressesPreCountNotification = objCycleCountSettingDTO.EmailAddressesPreCountNotification ?? string.Empty;
                        objCycleCountSetting.YearStartDate = objCycleCountSettingDTO.YearStartDate;
                        objCycleCountSetting.YearEndDate = objCycleCountSettingDTO.YearEndDate;
                        objCycleCountSetting.CycleCountTime = objCycleCountSettingDTO.CycleCountTime;
                        objCycleCountSetting.CountFrequencyType = objCycleCountSettingDTO.CountFrequencyType;
                        objCycleCountSetting.RecurrringDays = objCycleCountSettingDTO.RecurrringDays;
                        objCycleCountSetting.NextRunDate = null;
                        objCycleCountSetting.CycleCountsPerCycle = objCycleCountSettingDTO.CycleCountsPerCycle;
                        objCycleCountSetting.IsActive = objCycleCountSettingDTO.IsActive;
                        objCycleCountSetting.RecurringType = objCycleCountSettingDTO.RecurringType;

                        objCycleCountSetting.MissedItemsEmailTime = objCycleCountSettingDTO.MissedItemsEmailTime;
                        objCycleCountSetting.MissedItemEmailPriorHours = objCycleCountSettingDTO.MissedItemEmailPriorHours;
                        objCycleCountSetting.CountType = objCycleCountSettingDTO.CountType;
                    }
                }
                else
                {
                    objCycleCountSetting = new CycleCountSetting();
                    objCycleCountSetting.AClassFrequency = objCycleCountSettingDTO.AClassFrequency;
                    objCycleCountSetting.BClassFrequency = objCycleCountSettingDTO.BClassFrequency;
                    objCycleCountSetting.CClassFrequency = objCycleCountSettingDTO.CClassFrequency;
                    objCycleCountSetting.DClassFrequency = objCycleCountSettingDTO.DClassFrequency;
                    objCycleCountSetting.EClassFrequency = objCycleCountSettingDTO.EClassFrequency;
                    objCycleCountSetting.FClassFrequency = objCycleCountSettingDTO.FClassFrequency;
                    objCycleCountSetting.EmailAddressesDailyCycle = objCycleCountSettingDTO.EmailAddressesDailyCycle ?? string.Empty;
                    objCycleCountSetting.EmailAddressesPreCountNotification = objCycleCountSettingDTO.EmailAddressesPreCountNotification ?? string.Empty;
                    objCycleCountSetting.YearStartDate = objCycleCountSettingDTO.YearStartDate;
                    objCycleCountSetting.YearEndDate = objCycleCountSettingDTO.YearEndDate;
                    objCycleCountSetting.CompanyId = objCycleCountSettingDTO.CompanyId;
                    objCycleCountSetting.Created = DateTimeUtility.DateTimeNow;
                    objCycleCountSetting.CreatedBy = objCycleCountSettingDTO.CreatedBy;
                    objCycleCountSetting.GUID = Guid.NewGuid();
                    objCycleCountSetting.IsArchived = false;
                    objCycleCountSetting.IsDeleted = false;
                    objCycleCountSetting.LastUpdatedBy = objCycleCountSettingDTO.LastUpdatedBy;
                    objCycleCountSetting.RoomId = objCycleCountSettingDTO.RoomId;
                    objCycleCountSetting.Updated = objCycleCountSettingDTO.Updated;
                    objCycleCountSetting.CountFrequencyType = objCycleCountSettingDTO.CountFrequencyType;
                    objCycleCountSetting.RecurrringDays = objCycleCountSettingDTO.RecurrringDays;
                    objCycleCountSetting.CycleCountsPerCycle = objCycleCountSettingDTO.CycleCountsPerCycle;
                    objCycleCountSetting.CycleCountTime = objCycleCountSettingDTO.CycleCountTime;
                    objCycleCountSetting.IsActive = objCycleCountSettingDTO.IsActive;
                    objCycleCountSetting.RecurringType = objCycleCountSettingDTO.RecurringType;
                    objCycleCountSetting.MissedItemsEmailTime = objCycleCountSettingDTO.MissedItemsEmailTime;
                    objCycleCountSetting.MissedItemEmailPriorHours = objCycleCountSettingDTO.MissedItemEmailPriorHours;
                    objCycleCountSetting.CountType = objCycleCountSettingDTO.CountType;
                    context.CycleCountSettings.Add(objCycleCountSetting);

                }
                context.SaveChanges();


                objCycleCountSettingDTO.ID = objCycleCountSetting.ID;
                objCycleCountSettingDTO.GUID = objCycleCountSetting.GUID;

                if (objCycleCountSettingDTO.lstCycleCountSetup != null && objCycleCountSettingDTO.lstCycleCountSetup.Count > 0)
                {
                    objCycleCountSettingDTO.lstCycleCountSetup.ForEach(r =>
                    {
                        r.CycleCountSettingId = objCycleCountSettingDTO.ID;
                        r.RecurringType = objCycleCountSettingDTO.RecurringType;
                    });

                    SaveCycleCountSetUp(objCycleCountSettingDTO.lstCycleCountSetup, objCycleCountSettingDTO.RoomId, objCycleCountSettingDTO.CompanyId, objCycleCountSettingDTO.LastUpdatedBy);
                }

                string strUpdateOnHand = "EXEC [dbo].[ScheduledCyclecountAvailableDates] " + objCycleCountSettingDTO.ID.ToString();
                context.Database.ExecuteSqlCommand(strUpdateOnHand);

                strUpdateOnHand = "EXEC [dbo].[cyclecountscheduledate] " + objCycleCountSettingDTO.ID.ToString();
                context.Database.ExecuteSqlCommand(strUpdateOnHand);

            }
            return objCycleCountSettingDTO;

        }
        public List<CycleCountSetUpDTO> GetCycleCountSetUpByRoomID(long RoomID, long CompanyID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CycleCountSetUpDTO>("EXEC GetCycleCountSetUp " + RoomID + "," + CompanyID + "").ToList();
            }
        }
        public List<CycleCountSetUpDTO> SaveCycleCountSetUp(List<CycleCountSetUpDTO> lstCycleCountSetUp, long RoomID, long CompanyID, long UserID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                if (lstCycleCountSetUp != null && lstCycleCountSetUp.Count > 0)
                {
                    lstCycleCountSetUp.ForEach(t =>
                    {
                        CycleCountSetUp objCycleCountSetUp = context.CycleCountSetUps.FirstOrDefault(at => at.ClassificationGUID == t.ICGUID);
                        if (objCycleCountSetUp != null)
                        {
                            objCycleCountSetUp.RecurringType = t.RecurringType;
                            objCycleCountSetUp.TimeBaseUnit = t.TimeBaseUnit;
                            objCycleCountSetUp.TimeBaseRecurfrequency = t.TimeBaseRecurfrequency;
                            objCycleCountSetUp.CycleCountFrequency = t.CycleCountFrequency;
                            objCycleCountSetUp.LastUpdatedBy = UserID;
                            objCycleCountSetUp.LastUpdated = DateTime.UtcNow;
                            objCycleCountSetUp.EditedFrom = "web";
                            objCycleCountSetUp.CycleCountSettingId = t.CycleCountSettingId;

                        }
                        else
                        {
                            objCycleCountSetUp = new DAL.CycleCountSetUp();
                            objCycleCountSetUp.AddedFrom = "web";
                            objCycleCountSetUp.ClassificationGUID = t.ICGUID;
                            objCycleCountSetUp.CompanyID = CompanyID;
                            objCycleCountSetUp.Created = DateTime.UtcNow;
                            objCycleCountSetUp.CreatedBy = UserID;
                            objCycleCountSetUp.CycleCountFrequency = t.CycleCountFrequency;
                            objCycleCountSetUp.EditedFrom = "web";
                            objCycleCountSetUp.GUID = Guid.NewGuid();
                            objCycleCountSetUp.ID = 0;
                            objCycleCountSetUp.IsArchived = false;
                            objCycleCountSetUp.IsDeleted = false;
                            objCycleCountSetUp.LastUpdated = DateTime.UtcNow;
                            objCycleCountSetUp.LastUpdatedBy = UserID;
                            objCycleCountSetUp.ReceivedOn = DateTime.UtcNow;
                            objCycleCountSetUp.ReceivedOnWeb = DateTime.UtcNow;
                            objCycleCountSetUp.Room = RoomID;
                            objCycleCountSetUp.CycleCountSettingId = t.CycleCountSettingId;
                            objCycleCountSetUp.TimeBaseUnit = t.TimeBaseUnit;
                            objCycleCountSetUp.TimeBaseRecurfrequency = t.TimeBaseRecurfrequency;
                            objCycleCountSetUp.RecurringType = t.RecurringType;
                            context.CycleCountSetUps.Add(objCycleCountSetUp);
                        }
                    });
                    context.SaveChanges();
                }
            }
            return GetCycleCountSetUpByRoomID(RoomID, CompanyID);
        }
        public List<CycleCountSettingDTO> GetAllcycleCountSettings(DateTime FromDate, DateTime ToDate)
        {
            List<CycleCountSettingDTO> lstSchedules = new List<CycleCountSettingDTO>();
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                lstSchedules = (from nm in context.CycleCountSettings
                                join rm in context.Rooms on nm.RoomId equals rm.ID
                                join cm in context.CompanyMasters on rm.CompanyID equals cm.ID
                                where nm.IsDeleted == false && nm.IsActive == true && nm.NextRunDate >= FromDate && nm.NextRunDate <= ToDate && rm.IsRoomActive == true && rm.IsDeleted == false && (cm.IsDeleted ?? false) == false
                                select new CycleCountSettingDTO
                                {
                                    AClassFrequency = nm.AClassFrequency,
                                    BClassFrequency = nm.BClassFrequency,
                                    CClassFrequency = nm.CClassFrequency,
                                    CompanyId = nm.CompanyId,
                                    CountFrequencyType = nm.CountFrequencyType,
                                    Created = nm.Created,
                                    CreatedBy = nm.CreatedBy,
                                    CycleCountsPerCycle = nm.CycleCountsPerCycle,
                                    CycleCountTime = nm.CycleCountTime,
                                    DClassFrequency = nm.DClassFrequency,
                                    EClassFrequency = nm.EClassFrequency,
                                    EmailAddressesDailyCycle = nm.EmailAddressesDailyCycle,
                                    EmailAddressesPreCountNotification = nm.EmailAddressesPreCountNotification,
                                    FClassFrequency = nm.FClassFrequency,
                                    GUID = nm.GUID,
                                    ID = nm.ID,
                                    IsActive = nm.IsActive,
                                    IsArchived = nm.IsArchived,
                                    IsDeleted = nm.IsDeleted,
                                    LastUpdatedBy = nm.LastUpdatedBy,
                                    NextRunDate = nm.NextRunDate,
                                    RecurrringDays = nm.RecurrringDays,
                                    RoomId = nm.RoomId,
                                    Updated = nm.Updated,
                                    YearEndDate = nm.YearEndDate ?? DateTime.UtcNow,
                                    YearStartDate = nm.YearStartDate,
                                    MissedItemsEmailTime = nm.MissedItemsEmailTime,
                                    MissedItemEmailPriorHours = nm.MissedItemEmailPriorHours
                                }).ToList();
            }

            return lstSchedules;
        }
        public List<InventoryCountDetailDTO> GenerateCycleCount(CycleCountSettingDTO objCycleCountSettingDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string QryToExec = "EXEC AutomatedCycleCount " + objCycleCountSettingDTO.ID + "";
                context.Database.CommandTimeout = 3600;
                return context.Database.SqlQuery<InventoryCountDetailDTO>(QryToExec).ToList();
            }
        }
        public void UpdateNextRunDateOfCycleCount(long CycleCountSettingID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string QryToExec = "EXEC cyclecountscheduledate " + CycleCountSettingID + "";
                context.Database.ExecuteSqlCommand(QryToExec);
            }
        }

    }
}


