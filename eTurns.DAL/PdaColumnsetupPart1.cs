using System;
using System.Collections.Generic;
using System.Linq;
using eTurns.DTO;

namespace eTurns.DAL
{
    public partial class PdaColumnsetup : eTurnsBaseDAL
    {
        public IEnumerable<PdaGridColumsDto> GetPdaGridList()
        {
            IEnumerable<PdaGridColumsDto> obj;
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                obj = (from u in context.ExecuteStoreQuery<PdaGridColumsDto>(@"SELECT ListName, TableName from PDAGridColumnList where IsActive=1")
                       select new PdaGridColumsDto
                       {
                           ListName = u.ListName,
                           TableName = u.TableName
                       }).Distinct(new ListComparer()).ToList();
            }
            return obj;
        }

        public List<PdaGridColumsDto> GetRecordbylistnameSource(string listname, string tablename, string listids)
        {
            //PdaGridColumsDto objsettings = GetSettingrecordbytable(tablename);
            //string listids = string.Empty;
            //if (objsettings != null)
            //{
            //    listids = objsettings.ListIds;
            //}
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                string query;
                //Select columns for source list where IsFixedColumn is 0
                if (listids == string.Empty)
                {
                    //query = @"SELECT A.* FROM PDAGridColumnList A WHERE A.IsFixedColumn=0 AND A.IsSearchFixed=0 AND A.ListName='" + listname + "' AND IsActive=1";
                    query = @"SELECT A.*,A.IsSearchFixed AS IsSearch FROM PDAGridColumnList A WHERE A.IsFixedColumn=0 AND A.ListName='" + listname + "' AND IsActive=1";
                }
                else
                {
                    //query = @"SELECT A.* FROM PDAGridColumnList A WHERE A.IsFixedColumn=0 AND A.IsSearchFixed=0 AND A.ListName='" + listname + "' AND IsActive=1 and Convert(varchar(20),A.Id) NOT IN (" + listids + ")";
                    query = @"SELECT A.*,A.IsSearchFixed AS IsSearch FROM PDAGridColumnList A WHERE A.IsFixedColumn=0 AND A.ListName='" + listname + "' AND IsActive=1 and Convert(varchar(20),A.Id) NOT IN (" + listids + ")";
                }
                return (from u in context.ExecuteStoreQuery<PdaGridColumsDto>(query)
                        select new PdaGridColumsDto
                        {
                            Id = u.Id,
                            ListName = u.ListName,
                            TableName = u.TableName,
                            GridColumnName = u.GridColumnName,
                            GridColumnValue = u.GridColumnValue,
                            IsFixedColumn = u.IsFixedColumn,
                            FixedColumnOrder = u.FixedColumnOrder,
                            IsActive = u.IsActive,
                            GridColumnValueWithoutAlias = u.GridColumnValueWithoutAlias,
                            GridColumnAlias = u.GridColumnAlias,
                            IsSearch = u.IsSearch
                        }).ToList();
            }
        }

        public List<UserMasterDTO> GetAllUser(Int64? companyid, Int64? room)
        {
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                string query;
                query = string.Format("Select Distinct(UM.ID), UM.UserName from userRoomAccess URA INNER JOIN UserMaster UM ON URA.UserID = UM.ID WHERE URA.CompanyID={0} AND UM.UserType=3 and URA.RoomID={1} and UM.IsDeleted=0 ORDER BY UM.UserName", companyid, room);
                return (from u in context.ExecuteStoreQuery<UserMasterDTO>(query)
                        select new UserMasterDTO
                        {
                            ID = u.ID,
                            UserName = u.UserName
                        }).ToList();
            }
        }

        public List<UserMasterDTO> GetUserByTableName(string tablename, Int64? companyid, Int64? room)
        {
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                string query;
                query = string.Format("select ID,UserName from UserMaster WHERE UserType=3 AND IsDeleted=0 and ID in(Select UserID from pdasearchsettings where CompanyID={0} AND Room={1} AND TableName='{2}')", companyid, room, tablename);
                return (from u in context.ExecuteStoreQuery<UserMasterDTO>(query)
                        select new UserMasterDTO
                        {
                            ID = u.ID,
                            UserName = u.UserName
                        }).ToList();
            }
        }

        public void RemoveSearchSettingColumns(string tablename, Int64? companyid, Int64? room, string Ids)
        {
            using (var context = new eTurnsEntities(DataBaseEntityConnectionString))
            {
                string query;
                query = string.Format("Delete from pdasearchsettings where CompanyID={0} AND Room={1} AND TableName='{2}' AND UserID in ({3}) ", companyid, room, tablename, Ids);
                context.ExecuteStoreCommand(query);
            }
        }
    }
}
