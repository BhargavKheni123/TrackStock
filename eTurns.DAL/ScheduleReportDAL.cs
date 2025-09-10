using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace eTurns.DAL
{
    public class ScheduleReportDAL : eTurnsBaseDAL
    {
        #region [Class Constructor]

        //public ScheduleReportDAL(base.DataBaseName)
        //{

        //}

        public ScheduleReportDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        //public ScheduleReportDAL(string DbName, string DBServerNm, string DBUserNm, string DBPswd)
        //{
        //    DataBaseName = DbName;
        //    DBServerName = DBServerNm;
        //    DBUserName = DBUserNm;
        //    DBPassword = DBPswd;
        //}

        #endregion

        /// <summary>
        /// Get Paged Records from the Room Schedule Table
        /// </summary>
        /// <param name="StartRowIndex">StartRowIndex</param>
        /// <param name="MaxRows">MaxRows</param>
        /// <param name="TotalCount">TotalCount</param>
        /// <param name="SearchTerm">SearchTerm</param>
        /// <param name="sortColumnName">sortColumnName</param>
        /// <returns></returns>
        public IEnumerable<SchedulerDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyId, bool IsDeleted, Int64 ScheduleFor)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                string strQuer = "";
                if (!string.IsNullOrEmpty(SearchTerm) && SearchTerm.Contains("[###]"))
                {

                    string Creaters = null;
                    string Updators = null;
                    string CreatedDateFrom = null;
                    string CreatedDateTo = null;
                    string UpdatedDateFrom = null;
                    string UpdatedDateTo = null;

                    //Get Cached-Media

                    string[] stringSeparators = new string[] { "[###]" };
                    string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);
                    string[] FieldsPara = Fields[1].Split('~');

                    if (!string.IsNullOrWhiteSpace(FieldsPara[0]))
                    {

                        string[] arrCreators = FieldsPara[0].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in arrCreators)
                        {
                            Creaters += "''" + item + "'',";
                        }
                        Creaters = Creaters.TrimEnd(',');


                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[1]))
                    {
                        //Updators = FieldsPara[1].TrimEnd(',');
                        string[] arrCreators = FieldsPara[1].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var item in arrCreators)
                        {
                            Updators += "''" + item + "'',";
                        }
                        Updators = Updators.TrimEnd(',');
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[2]))
                    {
                        CreatedDateFrom = Convert.ToDateTime(FieldsPara[2].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                        CreatedDateTo = Convert.ToDateTime(FieldsPara[2].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    }
                    if (!string.IsNullOrWhiteSpace(FieldsPara[3]))
                    {
                        UpdatedDateFrom = Convert.ToDateTime(FieldsPara[3].Split(',')[0]).Date.ToString("dd-MM-yyyy");
                        UpdatedDateTo = Convert.ToDateTime(FieldsPara[3].Split(',')[1]).Date.ToString("dd-MM-yyyy");
                    }

                    strQuer = @"EXEC [GetPagedSchedulerReportList] " + StartRowIndex + "," + MaxRows + ",'','"
                                   + sortColumnName + "','" + Creaters + "','" + Updators + "','" + CreatedDateFrom + "','" + CreatedDateTo
                                   + "','" + UpdatedDateFrom + "','" + UpdatedDateTo + "'," + (IsDeleted ? "1" : "0") + "," + RoomID + "," + CompanyId + "," + ScheduleFor;
                }
                else
                {
                    strQuer = @"EXEC [GetPagedSchedulerReportList] " + StartRowIndex + "," + MaxRows + ",'" + SearchTerm + "','" + sortColumnName + "','','','','','',''," + (IsDeleted ? "1" : "0") + "," + RoomID + "," + CompanyId + "," + ScheduleFor;
                }

                IEnumerable<SchedulerDTO> obj = (from u in context.Database.SqlQuery<SchedulerDTO>(strQuer)
                                                 select new SchedulerDTO
                                                 {
                                                     ScheduleID = u.ScheduleID,
                                                     ReportName = u.ReportName,
                                                     NextRunDate = u.NextRunDate,
                                                     Created = u.Created,
                                                     CreatedBy = u.CreatedBy,
                                                     LastUpdatedBy = u.LastUpdatedBy,
                                                     CreatedByName = u.CreatedByName,
                                                     UpdatedByName = u.UpdatedByName,
                                                     TotalRecords = u.TotalRecords,
                                                 }).AsParallel().ToList();
                TotalCount = 0;
                if (obj != null && obj.Count() > 0)
                {
                    TotalCount = obj.ElementAt(0).TotalRecords;
                }
                return obj;
            }


        }


    }
}
