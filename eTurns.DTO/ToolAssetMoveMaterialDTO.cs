using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;


namespace eTurns.DTO
{

    public enum ToolMoveType
    {
        InvToInv = 1
    }

    public enum ToolMoveDialogOpenFrom
    {
        FromMove = 1,
        FromTool = 2

    }

    public class ToolAssetMoveMaterialDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Guid? ToolGUID { get; set; }

        public Guid? AssetGUID { get; set; }
        public Int64 SourceToolBinID { get; set; }


        public double MoveQuanity { get; set; }
        public int MoveType { get; set; }
        public Int64 DestToolBinID { get; set; }


        public string SourceLocation { get; set; }
        public string DestinationLocation { get; set; }



        public ToolMoveDialogOpenFrom OpenFrom { get; set; }

        public string MoveTypeDescription { get; set; }
        public string ToolName { get; set; }

        public string AssetName { get; set; }

        public string OnHandQuantity { get; set; }

        public string ToolQuantity { get; set; }


        public Int64 CreatedBy { get; set; }
        public Int64 UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Int64 RoomID { get; set; }
        public Int64 CompanyID { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsArchived { get; set; }

        public List<CommonDTO> MoveTypeList { get; set; }

        public System.String AddedFrom { get; set; }
        public System.DateTime ReceivedOnWeb { get; set; }
        public System.DateTime ReceivedOn { get; set; }
        public System.String EditedFrom { get; set; }
        public bool IsOnlyFromToolUI { get; set; }

        public bool LotNumberTracking { get; set; }
        public bool SerialNumberTracking { get; set; }

        public string LotNumber { get; set; }
        public string SerialNumber { get; set; }

        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }

        private string _createdOnDate;
        public string CreatedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_createdOnDate))
                {
                    _createdOnDate = FnCommon.ConvertDateByTimeZone(CreatedOn, true);
                }
                return _createdOnDate;
            }
        }

        private string _updatedOnDate;
        public string UpdatedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_updatedOnDate))
                {
                    _updatedOnDate = FnCommon.ConvertDateByTimeZone(UpdatedOn, true);
                }
                return _updatedOnDate;
            }
        }

        private string _ReceivedOnDate;
        public string ReceivedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnDate))
                {
                    _ReceivedOnDate = FnCommon.ConvertDateByTimeZone(ReceivedOn, true);
                }
                return _ReceivedOnDate;
            }
        }

        private string _ReceivedOnWeb;
        public string ReceivedOnDateWeb
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnWeb))
                {
                    _ReceivedOnWeb = FnCommon.ConvertDateByTimeZone(ReceivedOnWeb, true);
                }
                return _ReceivedOnWeb;
            }
        }

    }


    public class ToolAssetMoveMaterialDetailDTO
    {
        public Int64 ID { get; set; }
        public Guid GUID { get; set; }
        public Guid MoveMaterialGuid { get; set; }
        public double Quantity { get; set; }

        public string SerialNumber { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpireDate { get; set; }

        public Int64 CreatedBy { get; set; }
        public Int64 UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public Int64 RoomID { get; set; }
        public Int64 CompanyID { get; set; }

        public Guid ToolAssetQtyDetailGUID_Source { get; set; }
        public Guid ToolAssetQtyDetailGUID_Destination { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsArchived { get; set; }

        public System.String AddedFrom { get; set; }
        public System.DateTime ReceivedOnWeb { get; set; }
        public System.DateTime ReceivedOn { get; set; }
        public System.String EditedFrom { get; set; }
        public bool IsOnlyFromToolUI { get; set; }

        private string _ReceivedOn;
        public string ReceivedOnDate
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOn))
                {
                    _ReceivedOn = FnCommon.ConvertDateByTimeZone(ReceivedOn, true);
                }
                return _ReceivedOn;
            }
            set { this._ReceivedOn = value; }
        }

        private string _ReceivedOnWeb;
        public string ReceivedOnDateWeb
        {
            get
            {
                if (string.IsNullOrEmpty(_ReceivedOnWeb))
                {
                    _ReceivedOnWeb = FnCommon.ConvertDateByTimeZone(ReceivedOnWeb, true);
                }
                return _ReceivedOnWeb;
            }
            set { this._ReceivedOnWeb = value; }
        }

    }

    public class ResToolAssetMoveMaterial
    {
        private static string resourceFileName = "ResToolAssetMoveMaterial";

        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string LabelMoveType
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelMoveType", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string TabMove
        {
            get
            {
                return ResourceRead.GetResourceValue("TabMove", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string DailogTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("DailogTitle", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string PageTitle
        {
            get
            {
                return ResourceRead.GetResourceValue("PageTitle", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string PageHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("PageHeader", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string LabelSourceLacation
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelSourceLacation", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string LabelQuantityToMove
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelQuantityToMove", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string LabelStagingHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelStagingHeader", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string LabelDestinationLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelDestinationLocation", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string LabelAvailableQty
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelAvailableQty", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string LabelSourceStagingHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("LabelSourceStagingHeader", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string ButtonMove
        {
            get
            {
                return ResourceRead.GetResourceValue("ButtonMove", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string ButtonCancel
        {
            get
            {
                return ResourceRead.GetResourceValue("ButtonCancel", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string MoveTypeItemInvtoInv
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveTypeItemInvtoInv", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string MoveTypeItemInvtoStage
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveTypeItemInvtoStage", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string MoveTypeItemStageToInv
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveTypeItemStageToInv", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string MoveTypeItemStageToStage
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveTypeItemStageToStage", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string ErrorMsgSourceLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorMsgSourceLocation", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string ErrorMsgMoveQty
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorMsgMoveQty", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string ErrorMsgDestinationStagingHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorMsgDestinationStagingHeader", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string ErrorMsgDestinationLocation
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorMsgDestinationLocation", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string ErrorMsgSourceAndDestinationAreSame
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorMsgSourceAndDestinationAreSame", resourceFileName);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string ErrorMsgNotSuffucientQtyAtSource
        {
            get
            {
                return ResourceRead.GetResourceValue("ErrorMsgNotSuffucientQtyAtSource", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string MenuLinkMoveMaterial
        {
            get
            {
                return ResourceRead.GetResourceValue("MenuLinkMoveMaterial", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string GridButtonMove
        {
            get
            {
                return ResourceRead.GetResourceValue("GridButtonMove", resourceFileName);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Bin.
        /// </summary>
        public static string GridColumnHeaderForMove
        {
            get
            {
                return ResourceRead.GetResourceValue("GridColumnHeaderForMove", resourceFileName);
            }
        }

        public static string CreatedOn
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedOn", resourceFileName);
            }
        }

        public static string CreatedBy
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedBy", resourceFileName);
            }
        }

        public static string CreatedFrom
        {
            get
            {
                return ResourceRead.GetResourceValue("CreatedFrom", resourceFileName);
            }
        }

        public static string ReceivedOnServer
        {
            get
            {
                return ResourceRead.GetResourceValue("ReceivedOnServer", resourceFileName);
            }
        }

        public static string MoveQuantity
        {
            get
            {
                return ResourceRead.GetResourceValue("MoveQuantity", resourceFileName);
            }
        }

        public static string DestinationStagingHeader
        {
            get
            {
                return ResourceRead.GetResourceValue("DestinationStagingHeader", resourceFileName);
            }
        }

        public static string SerialNoNotBlank { get { return ResourceRead.GetResourceValue("SerialNoNotBlank", resourceFileName); } }
        public static string LotNoNotBlank { get { return ResourceRead.GetResourceValue("LotNoNotBlank", resourceFileName); } }
    }


}
