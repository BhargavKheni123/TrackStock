using eTurns.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace eTurns.DAL
{
    public partial class ProjectMasterDAL : eTurnsBaseDAL
    {
        public IEnumerable<ProjectMasterDTO> GetProjectMaster(Int64 RoomID, Int64 CompanyID, DateTime? FromDate, DateTime? ToDate, string Status, Int32 StatusValue = 0)
        {
            IEnumerable<ProjectMasterDTO> lstProjectMaster;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramskts = new SqlParameter[] { new SqlParameter("@RoomID", RoomID),
                                                     new SqlParameter("@CompanyID", CompanyID),
                                                     new SqlParameter("@FromDate", Convert.ToString(FromDate) ?? (object)DBNull.Value ),
                                                     new SqlParameter("@ToDate", Convert.ToString(ToDate) ?? (object)DBNull.Value),
                                                     new SqlParameter("@Status", Status),
                                                     new SqlParameter("@StatusValue", StatusValue)
                };

                lstProjectMaster = (from u in context.Database.SqlQuery<ProjectMasterDTO>("exec [GetProjectMaster] @RoomID,@CompanyID,@FromDate,@ToDate,@Status,@StatusValue", paramskts)
                                    select new ProjectMasterDTO
                                    {
                                        ProjectSpendName = u.ProjectSpendName,
                                        DollarLimitAmount = u.DollarLimitAmount,
                                        DollarUsedAmount = u.DollarUsedAmount,
                                        CompanyID = u.CompanyID,
                                        Created = u.Created,
                                        Room = u.Room,
                                        ID = u.ID,
                                        Description = u.Description,
                                        ProjectSpendItems = new ProjectSpendItemsDAL(base.DataBaseName).GetHistoryRecordByProjectID(u.GUID).ToList()
                                    }).AsParallel().ToList();
            }
            return lstProjectMaster;
        }

        public IEnumerable<ProjectSpendItemsDTO> GetProjectMastersDashboard(int StartRowIndex, int MaxRows, out int TotalCount, string SearchTerm, string sortColumnName, long RoomId, long CompanyId, string WidgetType, bool UserConsignmentAllowed, long LoggedinUserId)
        {
            List<ProjectSpendItemsDTO> lstProjects = new List<ProjectSpendItemsDTO>();
            DataSet dsProjects = new DataSet();
            TotalCount = 0;
            string Connectionstring = DbConnectionHelper.GeteTurnsSQLConnectionString(base.DataBaseName, DbConnectionType.GeneralReadWrite.ToString("F"));

            if (Connectionstring == "")
            {
                return lstProjects;
            }

            SqlConnection EturnsConnection = new SqlConnection(Connectionstring);
            dsProjects = SqlHelper.ExecuteDataset(EturnsConnection, "GetPagedProjectsForDashboard", StartRowIndex, MaxRows, SearchTerm, sortColumnName, RoomId, CompanyId, WidgetType);

            if (dsProjects != null && dsProjects.Tables.Count > 0)
            {
                lstProjects = DataTableHelper.ToList<ProjectSpendItemsDTO>(dsProjects.Tables[0]);

                if (lstProjects != null && lstProjects.Count() > 0)
                {
                    TotalCount = lstProjects.ElementAt(0).TotalRecords;
                }
            }
            return lstProjects;
        }

        public CompanyConfigDTO GetCompanyConfig(Int64 CompanyID)
        {
            CompanyConfigDTO ObjCompanyConfigDTO;
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var paramskts = new SqlParameter[] { new SqlParameter("@CompanyID", CompanyID) };
                ObjCompanyConfigDTO = (from u in context.Database.SqlQuery<CompanyConfigDTO>("exec [GetCompanyConfig] @CompanyID", paramskts)
                                        select new CompanyConfigDTO
                                        {
                                            ID = u.ID,
                                            OperationalHoursBefore = u.OperationalHoursBefore,
                                            MileageBefore = u.MileageBefore,
                                            ScheduleDaysBefore = u.ScheduleDaysBefore,
                                            ProjectAmountExceed = u.ProjectAmountExceed,
                                            ProjectItemQuantitExceed = u.ProjectItemQuantitExceed,
                                            ProjectItemAmountExceed = u.ProjectItemAmountExceed,
                                            NOBackDays = u.NOBackDays,
                                            NODaysAve = u.NODaysAve,
                                            NOTimes = u.NOTimes,
                                            MinPer = u.MinPer,
                                            MaxPer = u.MaxPer,
                                            CompanyID = u.CompanyID
                                        }).FirstOrDefault();
            }
            return ObjCompanyConfigDTO;

        }
    }
}
