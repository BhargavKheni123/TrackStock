using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace eTurns.DAL
{
    public partial class CommonDAL : eTurnsBaseDAL
    {
        public IEnumerable<CommonDTO> GetDDData(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID)
        {
            
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramData = new SqlParameter[] { new SqlParameter("@TableName", TableName), 
                                                     new SqlParameter("@TextFieldName", TextFieldName),
                                                     new SqlParameter("@CompnayID", CompanyID),
                                                     new SqlParameter("@RoomID", RoomID) };

                return (from u in context.Database.SqlQuery<CommonDTO>("exec [GetSupportData]  @TableName,@TextFieldName,@CompnayID,@RoomID", paramData)
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text,
                            Count = u.Count
                        }).ToList();
            }
        }

        public List<CommonDTO> GetDDDataAll(Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramData = new SqlParameter[]
                {
            new SqlParameter("@CompanyID", CompanyID),
            new SqlParameter("@RoomID", RoomID)
                };

                var data = context.Database.SqlQuery<CommonDTO>("exec [GetSupportData_All] @CompanyID, @RoomID", paramData).ToList();


                return data;
            }
        }

        public IEnumerable<CommonDTO> GetDDDataWithValue(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID, string ValueField = "")
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramData = new SqlParameter[] { new SqlParameter("@TableName", TableName),
                                                     new SqlParameter("@TextFieldName", TextFieldName),
                                                     new SqlParameter("@CompnayID", CompanyID),
                                                     new SqlParameter("@RoomID", RoomID), 
                                                     new SqlParameter("@ValueField", ValueField)   };

                return (from u in context.Database.SqlQuery<CommonDTO>("exec [GetSupportDataWithValue]  @TableName,@TextFieldName,@CompnayID,@RoomID,@ValueField", paramData)
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text,
                            Value = u.Value,
                            Count = u.Count
                        }).ToList();
            }
        }

        public IEnumerable<CommonDTO> GetDDData(string TableName, string TextFieldName, Int64 CompanyID, Int64 RoomID, bool isBom)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString)) 
            {
                var paramData = new SqlParameter[] { new SqlParameter("@TableName", TableName),
                                                     new SqlParameter("@TextFieldName", TextFieldName),
                                                     new SqlParameter("@CompnayID", CompanyID),
                                                     new SqlParameter("@RoomID", RoomID) };

                return (from u in context.Database.SqlQuery<CommonDTO>("exec [GetSupportDataBOM] @TableName,@TextFieldName,@CompnayID,@RoomID", paramData)
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text,
                            Count = u.Count
                        }).ToList();
            }
        }

        public IEnumerable<CommonDTO> GetDDData(string TableName, string TextFieldName, string WhereCondition, Int64 CompanyID, Int64 RoomID)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString)) 
            {
                var paramData = new SqlParameter[] { new SqlParameter("@TableName", TableName),
                                                     new SqlParameter("@TextFieldName", TextFieldName),
                                                     new SqlParameter("@CompnayID", CompanyID),
                                                     new SqlParameter("@RoomID", RoomID),
                                                     new SqlParameter("@WhereCondition", WhereCondition)  };


                return (from u in context.Database.SqlQuery<CommonDTO>("exec [GetSupportDataByCompanyRoomID] @TableName,@TextFieldName,@CompnayID,@RoomID,@WhereCondition", paramData)
                        select new CommonDTO
                        {
                            ID = u.ID,
                            Text = u.Text,
                            Count = u.Count
                        }).ToList();
            }
        }
        
    }
}
