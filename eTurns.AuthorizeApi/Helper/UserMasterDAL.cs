using eTurns.AuthorizeApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using eTurns.AuthorizeApi.Entities;

namespace eTurns.AuthorizeApi.Helper
{
    public class UserMasterDAL
    {

        public UserMasterDTO AuthenticateUser(string UserName, string Password)
        {
            //Password = CommonUtility.getSHA15Hash(Password);
            var params1 = new SqlParameter[] { new SqlParameter("@UserName", UserName ?? (object)DBNull.Value), new SqlParameter("@Password", Password ?? (object)DBNull.Value) };
            DbContext objDbContext = new DbContext("eTurnsMasterDbConnection");
            UserMasterDTO objUser = objDbContext.Database.SqlQuery<UserMasterDTO>("[dbo].[AuthenticateUser] @UserName,@Password", params1).FirstOrDefault();
            return objUser;

            //string Con = System.Configuration.ConfigurationManager.ConnectionStrings["eTurnsMasterDbConnection"].ConnectionString;
            //SqlConnection objCon = new SqlConnection(Con);
            //DataSet ds = SqlHelper.ExecuteDataset(objCon, "AuthenticateUser", UserName, Password);
            //if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            //{
            //    DataTable DT = ds.Tables[0];
            //    DbContext objDbContext = new DbContext("eTurnsMasterDbConnection");




            //}

        }
        public Audience AddAudience(Audience objAudience)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@ClientId", objAudience.ClientId ?? (object)DBNull.Value), new SqlParameter("@Base64Secret", objAudience.Base64Secret ?? (object)DBNull.Value), new SqlParameter("@Name", objAudience.Name ?? (object)DBNull.Value) };
            DbContext objDbContext = new DbContext("eTurnsMasterDbConnection");
            objAudience = objDbContext.Database.SqlQuery<Audience>("[dbo].[InsertResourceServer] @ClientId,@Base64Secret,@Name", params1).FirstOrDefault();
            return objAudience;
        }

        public List<Audience> getAllAudience() {
            List<Audience> lstAudience = new List<Entities.Audience>();
            DbContext objDbContext = new DbContext("eTurnsMasterDbConnection");
            lstAudience = objDbContext.Database.SqlQuery<Audience>("[dbo].[getAllAudience]").ToList();
            return lstAudience;

        }


    }
}