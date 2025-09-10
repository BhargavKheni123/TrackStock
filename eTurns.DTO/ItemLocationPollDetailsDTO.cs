using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class ItemLocationPollDetailsDTO
    {
		public long ID { get; set; }
		public long PollRequestID { get; set; }
		public double WeightReading { get; set; }
		public string ErrorDescription { get; set; }
		public DateTime? Created { get; set; }
		public DateTime? Updated { get; set; }
		public double? ItemWeightPerPiece { get; set; }
		public double? NewQuantity { get; set; }
		public bool Consignment { get; set; }
		public double? ConsignedQuantity { get; set; }
		public double? CustomerOwnedQuantity { get; set; }
		public string ActionType { get; set; }
		public double? PoolQuantity { get; set; }
	}
}
