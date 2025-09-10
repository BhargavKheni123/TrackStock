using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class PdaColumnsetup : eTurnsBaseDAL
    {
        #region Constructor

        //public PdaColumnsetup(base.DataBaseName)
        //{

        //}

        public PdaColumnsetup(string dbName)
        {
            DataBaseName = dbName;
        }

        //public PdaColumnsetup(string dbName, string dbServerNm, string dbUserNm, string dbPswd)
        //{
        //    DataBaseName = dbName;
        //    DBServerName = dbServerNm;
        //    DBUserName = dbUserNm;
        //    DBPassword = dbPswd;
        //}

        public IEnumerable<PdaGridColumsDto> GetPdaGridList()
        {
            IEnumerable<PdaGridColumsDto> obj;
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                obj = (from u in context.Database.SqlQuery<PdaGridColumsDto>("EXEC GetPDAGridColumnListPlain")
                       select new PdaGridColumsDto
                       {
                           ListName = u.ListName,
                           TableName = u.TableName
                       }).Distinct(new ListComparer()).ToList();
            }
            return obj;
        }

        public List<PdaGridColumsDto> GetRecordbylistnameDestination(string listname, string tablename, string listids)
        {
            if (listids == string.Empty)
            {
                return null;
            }
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                Int64[] nums = listids.Split(',').Select(Int64.Parse).ToArray();
                IQueryable<PDAGridColumnList> tepquery = (from i in context.PDAGridColumnLists select i);

                var query1 = (from u in tepquery
                              where nums.Contains(u.ID) && u.IsActive
                              select new PdaGridColumsDto
                              {
                                  Id = u.ID,
                                  ListName = u.ListName,
                                  TableName = u.TableName,
                                  GridColumnName = u.GridColumnName,
                                  GridColumnValue = u.GridColumnValue,
                                  IsFixedColumn = u.IsFixedColumn,
                                  FixedColumnOrder = u.FixedColumnOrder,
                                  IsActive = u.IsActive,
                                  GridColumnValueWithoutAlias = u.GridColumnValueWithoutAlias,
                                  GridColumnAlias = u.GridColumnAlias,
                                  IsSearch = u.IsSearchFixed
                              }).ToList();

                var reordered = query1.OrderBy(c =>
                {
                    var index = Array.IndexOf(nums, c.Id);
                    return index < 0 ? int.MaxValue : index; // put elements that don't have matching ID in the end
                });

                return reordered.ToList();
            }
        }

        public List<PdaGridColumsDto> GetPDAGridColumnListByListNamePlain(string listname, string listids)
        {
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                if (listids == string.Empty)
                {
                    var param1 = new SqlParameter[] { new SqlParameter("@ListName", listname ?? (object)DBNull.Value) };
                    return context.Database.SqlQuery<PdaGridColumsDto>("exec GetPDAGridColumnListByListNamePlain @ListName", param1).ToList();
                }
                else
                {
                    var param = new SqlParameter[] { new SqlParameter("@ListName", listname ?? (object)DBNull.Value) , new SqlParameter("@Ids", listids) };
                    return context.Database.SqlQuery<PdaGridColumsDto>("exec GetPDAGridColumnListByListNameAndExcludeIdsPlain @ListName,@Ids", param).ToList();
                }                
            }
        }
        public PdaGridColumsDto GetSettingrecordbytable(string tablename, Int64? companyid, Int64? room, Int64? userid)
        {
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                return (from u in context.PDASearchSettings
                        where u.TableName.Equals(tablename) && u.CompanyID == companyid && u.Room == room && u.UserID == userid
                        select new PdaGridColumsDto
                        {
                            TableName = u.TableName,
                            ColumnNames = u.ColumnNames,
                            IsSearch = u.IsSearch,
                            CompanyID = u.CompanyID,
                            Room = u.Room,
                            UserID = u.UserID,
                            CreatedBy = u.CreatedBy,
                            ListIds = u.ListIds
                        }).FirstOrDefault();
            }
        }

        public void SaveData(List<PdaGridColumsDto> oPdaGridColumsDtoList, string tableName)
        {
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                string isSearchColumnNames = "";
                string isNotSearchColumnNames = "";

                long? CompanyID = oPdaGridColumsDtoList[0].CompanyID;
                long? Room = oPdaGridColumsDtoList[0].Room;
                long? UserID = oPdaGridColumsDtoList[0].UserID;

                var fixedcolumn = (from u in context.PDAGridColumnLists
                                   where u.TableName.Equals(tableName) && u.IsFixedColumn && u.IsSearchFixed == false && u.IsActive
                                   select u).ToList().OrderBy(c => c.FixedColumnOrder);

                if (fixedcolumn.Any())
                {
                    var strvalewithout = (string.Join("|", fixedcolumn.Select(x => x.GridColumnValueWithoutAlias).ToArray()));
                    if (!string.IsNullOrEmpty(strvalewithout))
                    {
                        isSearchColumnNames = strvalewithout + "|" + (string.Join("|", oPdaGridColumsDtoList.Where(o => o.IsSearch == false).Select(x => x.GridColumnValueWithoutAlias)));
                    }
                }

                //IOrderedEnumerable<PDAGridColumnList> fixedcolumn1 = (from u in context.PDAGridColumnLists
                //                                                      where (u.TableName.Equals(tableName)) && (u.IsFixedColumn || u.IsSearchFixed == true) && u.IsActive
                //                                                      select u).ToList().OrderBy(c => c.FixedColumnOrder);
                //Get fixed column list for NotSearchColumnsList
                IOrderedEnumerable<PDAGridColumnList> fixedcolumn1 = (from u in context.PDAGridColumnLists
                                                                      where (u.TableName.Equals(tableName)) && (u.IsFixedColumn) && u.IsActive
                                                                      select u).ToList().OrderBy(c => c.FixedColumnOrder);

                string strvale = string.Empty;
                if (fixedcolumn1.Any())
                {
                    strvale = (string.Join("|", fixedcolumn1.Select(x => x.GridColumnValueWithoutAlias + " AS [" + x.GridColumnAlias + "]").ToArray()));
                }
                else if (fixedcolumn.Any())
                {
                    strvale = (string.Join("|", fixedcolumn.Select(x => x.GridColumnValueWithoutAlias + " AS [" + x.GridColumnAlias + "]").ToArray()));
                }

                if (!string.IsNullOrEmpty(strvale))
                {
                    isNotSearchColumnNames = strvale + "|" + (string.Join("|", oPdaGridColumsDtoList.Select(x => x.GridColumnValueWithoutAlias + " AS [" + x.GridColumnAlias + "]")));
                }

                var item = (from u in context.PDASearchSettings
                            where u.TableName.Equals(tableName) && u.IsSearch == true && u.CompanyID == CompanyID && u.Room == Room && u.UserID == UserID
                            select u).FirstOrDefault();

                string ListIds = string.Join(",", oPdaGridColumsDtoList.Select(x => x.Id));

                if (item != null)
                {
                    item.ColumnNames = isSearchColumnNames;
                    item.ListIds = ListIds;
                    item.LastUpdatedBy = UserID;
                    item.Updated = DateTimeUtility.DateTimeNow;

                    context.SaveChanges();

                    var item1 = (from u in context.PDASearchSettings
                                 where u.TableName.Equals(tableName) && u.IsSearch == null && u.CompanyID == CompanyID && u.Room == Room && u.UserID == UserID
                                 select u).FirstOrDefault();
                    if (item1 != null)
                    {
                        item1.ColumnNames = isNotSearchColumnNames;
                        item1.LastUpdatedBy = UserID;
                        item1.Updated = DateTimeUtility.DateTimeNow;
                        item1.ListIds = ListIds;
                        context.SaveChanges();
                    }
                }
                else
                {
                    for (var i = 0; i <= 1; i++)
                    {
                        PDASearchSetting obj = new PDASearchSetting();
                        switch (i)
                        {
                            case 0:
                                obj.IsSearch = null;
                                obj.ColumnNames = isNotSearchColumnNames;
                                break;
                            case 1:
                                obj.IsSearch = true;
                                obj.ColumnNames = isSearchColumnNames;
                                break;
                        }

                        obj.TableName = tableName;
                        obj.ListIds = ListIds;
                        obj.CompanyID = CompanyID;
                        obj.Room = Room;
                        obj.UserID = UserID;
                        obj.Created = DateTimeUtility.DateTimeNow;
                        obj.CreatedBy = UserID;
                        context.PDASearchSettings.Add(obj);
                        context.SaveChanges();
                    }
                }

                if (tableName.ToLower() == "itemmaster")
                {
                    #region Item Cache List

                    var item1 = (from u in context.PDASearchSettings
                                 where u.TableName.Equals("ItemMasterCache") && u.IsSearch == null && u.CompanyID == CompanyID && u.Room == Room && u.UserID == UserID
                                 select u).FirstOrDefault();

                    var strvalewithout = (string.Join("|", fixedcolumn.Select(x => x.GridColumnValue + " AS [" + x.GridColumnAlias + "]").ToArray()));

                    strvale = strvalewithout + "|" + string.Join("|", oPdaGridColumsDtoList.Where(o => !string.IsNullOrEmpty(o.GridColumnValue)).Select(x => x.GridColumnValue + " AS [" + x.GridColumnAlias + "]"));
                    if (item1 == null)
                    {
                        var obj1 = new PDASearchSetting
                        {
                            TableName = "ItemMasterCache",
                            ColumnNames = strvale,
                            IsSearch = null,
                            ListIds = ListIds,
                            CompanyID = CompanyID,
                            Room = Room,
                            UserID = UserID,
                            Created = DateTime.Now,
                            CreatedBy = UserID,
                        };
                        context.PDASearchSettings.Add(obj1);
                        context.SaveChanges();
                    }
                    else
                    {
                        item1.ColumnNames = strvale;
                        item1.ListIds = ListIds;
                        item1.LastUpdatedBy = UserID;
                        item1.Updated = DateTimeUtility.DateTimeNow;

                        context.SaveChanges();
                    }

                    #endregion
                }
            }
        }

        public List<UserMasterDTO> GetUsersByRoomAndCompany(long CompanyId, long RoomId)
        {
            var params1 = new SqlParameter[] {  
                                                new SqlParameter("@CompanyId", CompanyId),
                                                new SqlParameter("@RoomId", RoomId)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("exec GetUsersByRoomAndCompany @CompanyId,@RoomId", params1).ToList();
            }
        }

        public List<UserMasterDTO> GetUsersByPDASearchSettingsTableName(string TableName, long CompanyId, long RoomId)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@CompanyId", CompanyId),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@TableName", TableName ?? (object)DBNull.Value)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<UserMasterDTO>("exec GetUsersByPDASearchSettingsTableName @CompanyId,@RoomId,@TableName", params1).ToList();
            }
        }

        public void RemoveSearchSettingColumns(string TableName, long CompanyId, long RoomId, string Ids)
        {
            var params1 = new SqlParameter[] {
                                                new SqlParameter("@CompanyId", CompanyId),
                                                new SqlParameter("@RoomId", RoomId),
                                                new SqlParameter("@TableName", TableName ?? (object)DBNull.Value),
                                                new SqlParameter("@UserIds", Ids)
                                            };

            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("exec DeletePDASearchSettings @CompanyId,@RoomId,@TableName,@UserIds", params1);
            }
        }

        #endregion
    }
    public class ListComparer : IEqualityComparer<PdaGridColumsDto>
    {
        public bool Equals(PdaGridColumsDto x, PdaGridColumsDto y)
        {
            if (x.ListName == y.ListName)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(PdaGridColumsDto obj)
        {
            return obj.ListName.GetHashCode();
        }
    }
}
