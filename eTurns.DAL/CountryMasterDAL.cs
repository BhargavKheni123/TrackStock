using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public class CountryMasterDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]
        public CountryMasterDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public CountryMasterDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        #region [Class Methods]

        public List<CountryMasterDTO> GetAllCountryRecords()
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CountryMasterDTO>("exec [GetAllCountryRecords]").ToList();
            }
        }

        public CountryMasterDTO GetCountryDetailsByName(string CountryName)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CountryName", CountryName) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                return context.Database.SqlQuery<CountryMasterDTO>("exec [GetCountryRecordsByName] @CountryName", params1).FirstOrDefault();
            }
        }

        public void InsertCountry(CountryMasterDTO objDTO)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CountryName", objDTO.CountryName), new SqlParameter("@UserID", objDTO.CreatedBy) };
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC InsertCountry @CountryName,@UserID", params1);
            }
        }
        public void ABInsertCountryForNewEnt(CountryMasterDTO objDTO, bool isCountryUSA = false)
        {
            var params1 = new SqlParameter[] { new SqlParameter("@CountryName", objDTO.CountryName ?? (object)DBNull.Value),
                                               new SqlParameter("@UserID", objDTO.CreatedBy ?? (object)DBNull.Value),
                                               new SqlParameter("@CountryCode", objDTO.CountryCode ?? (object)DBNull.Value),
                                               new SqlParameter("@PhoneNunberFormat", objDTO.PhoneNunberFormat ?? (object) DBNull.Value),
                                               new SqlParameter("@isCountryUSA", isCountryUSA)};
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                context.Database.ExecuteSqlCommand("EXEC ABInsertCountryForNewEnt @CountryName,@UserID,@CountryCode,@PhoneNunberFormat,@isCountryUSA", params1);
            }
        }
        #endregion
    }
}
