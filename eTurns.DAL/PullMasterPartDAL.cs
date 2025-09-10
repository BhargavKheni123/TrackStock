using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using System.Collections;
using System.Web;
using eTurns.DTO.Resources;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Configuration;
using System.Globalization;

namespace eTurns.DAL
{
    public partial class PullMasterDAL : eTurnsBaseDAL
    {
        public PullResponseDTO PullGeneralItem(PullRequestDTO objPullRequestDTO, long RoomID, long CompanyID)
        {
            PullResponseDTO objPullResponseDTO = new PullResponseDTO();
            if (objPullRequestDTO != null)
            {
                string SerialisedPullResponse = string.Empty;
                string SerialisedPullRequest = Newtonsoft.Json.JsonConvert.SerializeObject(objPullRequestDTO);
                var params1 = new SqlParameter[] { new SqlParameter("@JSONPullItem", SerialisedPullRequest ?? (object)DBNull.Value), new SqlParameter("@RoomID", RoomID), new SqlParameter("@CompanyID", CompanyID) };
                using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
                {
                    SerialisedPullResponse = context.ExecuteStoreQuery<string>("EXEC PullGeneralItem @JSONPullItem,@RoomID,@CompanyID", params1).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(SerialisedPullResponse))
                    {
                        objPullResponseDTO = Newtonsoft.Json.JsonConvert.DeserializeObject<PullResponseDTO>(SerialisedPullResponse);
                    }
                }
            }
            return objPullResponseDTO;
        }
    }
}
