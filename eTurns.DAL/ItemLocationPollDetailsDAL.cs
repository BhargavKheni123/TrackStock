using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DTO;
using System.Data;
using System.Web;
using eTurns.DTO.Resources;
using System.Data.SqlClient;


namespace eTurns.DAL
{
    public class ItemLocationPollDetailsDAL : eTurnsBaseDAL
    {
        public ItemLocationPollDetailsDAL(string DbName)
        {
            base.DataBaseName = DbName;
        }

        public bool InsertItemLocationPollDetails(ItemLocationPollDetailsDTO detailsDTO)
        {
            using (var context = new eTurnsEntities(base.DataBaseEntityConnectionString))
            {
                var params1 = new SqlParameter[] { new SqlParameter("@PollRequestID",detailsDTO.PollRequestID),
                        new SqlParameter("@WeightReading",detailsDTO.WeightReading),
                        new SqlParameter("@ErrorDescription",detailsDTO.ErrorDescription.ToDBNull()),
                        new SqlParameter("@ItemWeightPerPiece",detailsDTO.ItemWeightPerPiece.ToDBNull()),
                        new SqlParameter("@NewQuantity",detailsDTO.NewQuantity.ToDBNull()),
                        new SqlParameter("@Consignment",detailsDTO.Consignment),
                        new SqlParameter("@ConsignedQuantity",detailsDTO.ConsignedQuantity.ToDBNull()),
                        new SqlParameter("@CustomerOwnedQuantity",detailsDTO.CustomerOwnedQuantity.ToDBNull()),
                        new SqlParameter("@ActionType",detailsDTO.ActionType),
                        new SqlParameter("@PoolQuantity",detailsDTO.PoolQuantity.ToDBNull())
                };

                long id = context.Database.SqlQuery<long>("EXEC [InsertItemLocationPollDetails] " +
                    "@PollRequestID,@WeightReading,@ErrorDescription,@ItemWeightPerPiece,@NewQuantity,@Consignment," +
                    "@ConsignedQuantity,@CustomerOwnedQuantity,@ActionType,@PoolQuantity", params1).FirstOrDefault();

                detailsDTO.ID = id;
                return true;
            }
        }

    }//class
}
