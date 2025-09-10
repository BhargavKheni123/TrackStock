using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class UDFOptionDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public UDFOptionDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public UDFOptionDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        {
            DataBaseName = DbName;
            DBServerName = DBServerNm;
            DBUserName = DBUserNm;
            DBPassword = DBPswd;
        }

        #endregion

        #region [Class Methods]
        public UDFOptionsDTO GetUDFOptionByIDPlain(long OptionID)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@OptionID", OptionID) };      
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFOptionsDTO>("exec [GetUDFOptionByIDPlain] @OptionID", params1).FirstOrDefault();
            }
        }
        public int BulkInsert(List<UDFOptionsDTO> items)
        {
            if (items == null || items.Count == 0)
                return 0;

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                try
                {
                    foreach (var objDTO in items)
                    {
                        UDFOption obj = new UDFOption
                        {
                            ID = 0,
                            UDFID = objDTO.UDFID,
                            UDFOption1 = objDTO.UDFOption,
                            CreatedBy = objDTO.CreatedBy,
                            LastUpdatedBy = objDTO.LastUpdatedBy,
                            Updated = DateTime.UtcNow,
                            Created = DateTime.UtcNow,
                            IsDeleted = false,
                            GUID = Guid.NewGuid(),
                            //CompanyID = objDTO.CompanyID // Uncomment if required
                        };

                        context.UDFOptions.Add(obj);
                    }

                    int insertedCount = context.SaveChanges();
                    return insertedCount;
                }
                catch
                {
                    throw;
                }
            }
        }

        public List<UDFOptionsDTO> GetUDFOptionsByUDFIDPlain(long UdfId)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UdfId", UdfId) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFOptionsDTO>("exec [GetUDFOptionsByUDFIDPlain] @UdfId", params1).ToList();
            }
        }

        public UDFOptionsDTO GetUDFOptionByUDFIDAndOptionNamePlain(long UdfId, string UDFOption)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UdfId", UdfId), new SqlParameter("@UDFOption", UDFOption ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFOptionsDTO>("exec [GetUDFOptionByUDFIDAndOptionName] @UdfId,@UDFOption", params1).FirstOrDefault();
            }
        }
        
        public List<UDFOptionsDTO> UDFOptionsByUDFIDNameSearchPlain(long UdfId, string OptionName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@UdfId", UdfId), new SqlParameter("@OptionName", OptionName ?? (object)DBNull.Value) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UDFOptionsDTO>("exec [UDFOptionsByUDFIDNameSearch] @UdfId,@OptionName", params1).ToList();
            }
        }
        
        public bool IsUdfOptionExistsInUDF(long UDfID, string optionValue)
        {
            UDFOptionsDTO objUDFOptionsDTO = GetUDFOptionByUDFIDAndOptionNamePlain(UDfID, optionValue);
            return (objUDFOptionsDTO != null && objUDFOptionsDTO.ID > 0);            
        }
        
        public Int64 Insert(UDFOptionsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDFOption obj = new UDFOption();
                obj.ID = 0;
                obj.UDFID = objDTO.UDFID;
                obj.UDFOption1 = objDTO.UDFOption;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.Created = DateTimeUtility.DateTimeNow;
                obj.IsDeleted = false;
                obj.GUID = Guid.NewGuid();
                obj.CompanyID = objDTO.CompanyID;
                obj.Room = objDTO.Room;
                context.UDFOptions.Add(obj);
                context.SaveChanges();
                objDTO.ID = obj.ID;
                return obj.ID;
            }
        }

        public bool Edit(UDFOptionsDTO objDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDFOption obj = new UDFOption();
                obj.ID = objDTO.ID;
                obj.UDFID = objDTO.UDFID;
                obj.UDFOption1 = objDTO.UDFOption;
                obj.CreatedBy = objDTO.CreatedBy;
                obj.LastUpdatedBy = objDTO.LastUpdatedBy;
                obj.Updated = DateTimeUtility.DateTimeNow;
                obj.Created = objDTO.Created;
                obj.IsDeleted = false;
                obj.GUID = objDTO.GUID;
                //obj.CompanyID = objDTO.CompanyID;
                //obj.Room = objDTO.Room;

                context.UDFOptions.Attach(obj);
                context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }
        
        public bool Edit(long ID, string UDFOption, long UserID, long CompanyID, bool fromeTurns = false)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDFDAL objUDFDAL = new UDFDAL(base.DataBaseName);
                UDFOptionDAL objUDFOptionsDAL;
                
                if (!fromeTurns)
                {
                    objUDFOptionsDAL = new UDFOptionDAL(base.DataBaseName);
                }
                else
                {
                    objUDFOptionsDAL = new UDFOptionDAL(CommonDAL.GeteTurnsDatabase());
                }
                //Update udf options
                if (!fromeTurns)
                {
                    //UDFOptionsDTO objDTO = GetRecord(ID, CompanyID);
                    UDFOptionsDTO objDTO = GetUDFOptionByIDPlain(ID);
                    objDTO.UDFOption = UDFOption;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = UserID;

                    objUDFOptionsDAL.Edit(objDTO);
                }
                else
                {
                   //UDFOptionDAL objUDFOptionDAL = new UDFOptionDAL(CommonDAL.GeteTurnsDatabase());
                    //UDFOptionsDTO objDTO = GetRecordeTurns(ID, CompanyID);
                    UDFOptionsDTO objDTO = GetUDFOptionByIDPlain(ID);
                    objDTO.UDFOption = UDFOption;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = UserID;

                    objUDFOptionsDAL.Edit(objDTO);
                }

                return true;
            }
        }

        public bool EditUserMaster(long ID, string UDFOption, long UserID, long CompanyID, bool fromeTurns = false, string EnterPriseDBName = "")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                UDFOptionDAL objUDFOptionsDAL;

                if (!string.IsNullOrWhiteSpace(EnterPriseDBName))
                {
                    objUDFOptionsDAL = new UDFOptionDAL(EnterPriseDBName);
                }
                else
                {
                    objUDFOptionsDAL = new UDFOptionDAL(EnterPriseDBName);
                }

                //Update udf options
                if (!fromeTurns)
                {
                    UDFOptionsDTO objDTO = GetUDFOptionByIDPlain(ID);
                    objDTO.UDFOption = UDFOption;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = UserID;

                    objUDFOptionsDAL.Edit(objDTO);
                }
                else
                {
                    //UDFOptionDAL objUDFOptionDAL = new UDFOptionDAL(CommonDAL.GeteTurnsDatabase());
                    UDFOptionsDTO objDTO = GetUDFOptionByIDPlain(ID);
                    objDTO.UDFOption = UDFOption;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = UserID;

                    objUDFOptionsDAL.Edit(objDTO);
                }

                return true;
            }
        }

        public bool Delete(long Id, long UserId, bool OthereTurns)
        {
            var eturnsDBName = "";
            var IsForEturns = false;

            if (!OthereTurns)
            {
                eturnsDBName = CommonDAL.GeteTurnsDatabase();
                IsForEturns = true;
            }

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@Id", Id),
                                                   new SqlParameter("@UserId", UserId),
                                                   new SqlParameter("@IsForEturns", IsForEturns),
                                                   new SqlParameter("@EturnsDBName", eturnsDBName),
                                                 };
                context.Database.ExecuteSqlCommand("exec [DeleteUdfOptionById] @Id,@UserId,@IsForEturns,@EturnsDBName", params1);
            }

            return true;
        }        

        public List<UDFMasterMain> SaveModuleUDF(List<UDFMasterMain> lstUdfs, long RoomID, long CompanyID, long UserID)
        {
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    lstUdfs.ForEach(l => { l.Reason = ""; l.Status = "success"; });
                    int iUDFMaxLength = 200;
                    int.TryParse(Convert.ToString(SiteSettingHelper.UDFMaxLength), out iUDFMaxLength);

                    if (lstUdfs != null && lstUdfs.Count > 0)
                    {
                        List<UDFMasterMain> lstUdfMasterRecs = (from udf in lstUdfs
                                                                group udf by new { udf.ModuleName, udf.ControlType, udf.UDFColumnName, udf.UDFName, udf.IsDeleted, udf.IncludeInNarrowSearch, udf.DefaultValue, udf.IsRequired } into groupedudf
                                                                select new UDFMasterMain
                                                                {
                                                                    ModuleName = groupedudf.Key.ModuleName,
                                                                    ControlType = groupedudf.Key.ControlType,
                                                                    UDFColumnName = groupedudf.Key.UDFColumnName,
                                                                    IsDeleted = groupedudf.Key.IsDeleted,
                                                                    IncludeInNarrowSearch = groupedudf.Key.IncludeInNarrowSearch,
                                                                    UDFName = groupedudf.Key.UDFName,
                                                                    DefaultValue = groupedudf.Key.DefaultValue,
                                                                    IsRequired = groupedudf.Key.IsRequired
                                                                }).ToList();

                        lstUdfMasterRecs.ForEach(t =>
                        {
                            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID),new SqlParameter("@RoomID", RoomID),new SqlParameter("@TableName", t.ModuleName),new SqlParameter("@UDFColumnName", t.UDFColumnName)};
                            UDF objUDFGetRecord= context.Database.SqlQuery<UDF>("exec [SP_UDFByColumnName] @CompanyID,@RoomID,@TableName,@UDFColumnName", params1).FirstOrDefault();
                            if (objUDFGetRecord != null)
                            {
                                objUDFGetRecord.LastUpdatedBy = UserID;
                                objUDFGetRecord.UDFColumnName = t.UDFColumnName;
                                objUDFGetRecord.UDFControlType = t.ControlType;
                                objUDFGetRecord.UDFDefaultValue = t.DefaultValue;
                                objUDFGetRecord.UDFIsRequired = t.IsRequired;
                                objUDFGetRecord.UDFIsSearchable = t.IncludeInNarrowSearch;
                                objUDFGetRecord.UDFTableName = t.ModuleName;
                            
                                if (t.ControlType.ToLower().Trim() == "textbox")
                                {
                                    objUDFGetRecord.IsDeleted = t.IsDeleted ?? false;
                                }

                                objUDFGetRecord.Updated = DateTime.UtcNow;
                                context.Entry(objUDFGetRecord).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                UDF objUDF = new UDF();
                                objUDF.CompanyID = CompanyID;
                                objUDF.Created = DateTime.UtcNow;
                                objUDF.CreatedBy = UserID;
                                objUDF.GUID = Guid.NewGuid();
                                objUDF.ID = 0;
                                objUDF.UDFControlType = t.ControlType;
                    
                                if (t.ControlType.ToLower().Trim() == "textbox")
                                {
                                    objUDF.IsDeleted = t.IsDeleted ?? false;
                                }
                                else
                                {
                                    objUDF.IsDeleted = false;
                                }
                                
                                objUDF.LastUpdatedBy = UserID;
                                objUDF.Room = RoomID;
                                objUDF.UDFColumnName = t.UDFColumnName;
                                objUDF.UDFDefaultValue = t.DefaultValue;
                                objUDF.UDFIsRequired = t.IsRequired;
                                objUDF.UDFIsSearchable = t.IncludeInNarrowSearch;
                                objUDF.UDFOptionsCSV = string.Empty;
                                objUDF.UDFTableName = t.ModuleName;
                                objUDF.Updated = DateTime.UtcNow;
                                objUDF.UDFMaxLength = iUDFMaxLength;
                                context.UDFs.Add(objUDF);
                            }
                        });
                        
                        context.SaveChanges();
                        
                        lstUdfMasterRecs = new List<UDFMasterMain>();
                        lstUdfMasterRecs = (from udf in lstUdfs
                                            group udf by new { udf.ModuleName, udf.UDFColumnName, udf.OptionName, udf.IsDeleted } into groupedudf
                                            select new UDFMasterMain
                                            {
                                                ModuleName = groupedudf.Key.ModuleName,
                                                UDFColumnName = groupedudf.Key.UDFColumnName,
                                                OptionName = groupedudf.Key.OptionName,
                                                IsDeleted = groupedudf.Key.IsDeleted
                                            }).ToList();

                        lstUdfMasterRecs.ForEach(t =>
                        {
                            var params1 = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID), new SqlParameter("@RoomID", RoomID), new SqlParameter("@TableName", t.ModuleName), new SqlParameter("@UDFColumnName", t.UDFColumnName) };
                            UDF objUDF = context.Database.SqlQuery<UDF>("exec [SP_UDFByColumnName] @CompanyID,@RoomID,@TableName,@UDFColumnName", params1).FirstOrDefault();
                            if (objUDF != null)
                            {
                                UDFOption udfOption = context.UDFOptions.FirstOrDefault(u => u.UDFID == objUDF.ID && u.UDFOption1 == t.OptionName);

                                if (udfOption == null)
                                {
                                    if (!string.IsNullOrWhiteSpace(t.OptionName))
                                    {
                                        UDFOption objUDFOption = new UDFOption();
                                        objUDFOption.Created = DateTime.UtcNow;
                                        objUDFOption.CreatedBy = UserID;
                                        objUDFOption.GUID = Guid.NewGuid();
                                        objUDFOption.ID = 0;
                                        objUDFOption.IsDeleted = t.IsDeleted ?? false;
                                        objUDFOption.UDFID = objUDF.ID;
                                        objUDFOption.Updated = DateTime.UtcNow;
                                        objUDFOption.UDFOption1 = t.OptionName;
                                        objUDFOption.LastUpdatedBy = UserID;
                                        objUDFOption.CompanyID = CompanyID;
                                        objUDFOption.Room = RoomID;

                                        context.UDFOptions.Add(objUDFOption);
                                    }
                                }
                                else
                                {
                                    udfOption.IsDeleted = t.IsDeleted ?? false;
                                    udfOption.Updated = DateTime.UtcNow;
                                    udfOption.LastUpdatedBy = UserID;
                                }
                            }

                        });
                
                    context.SaveChanges();

                    }
                }

            return lstUdfs;
        }
        #endregion
    }
}
