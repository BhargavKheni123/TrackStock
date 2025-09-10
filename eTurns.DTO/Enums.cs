using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eTurns.DTO
{
    public class Enums
    {
        public enum UserType
        {
            /// <summary>
            /// 1
            /// </summary>
            SuperAdmin = 1,
            /// <summary>
            /// 2
            /// </summary>
            EnterpriseAdmin = 2,
            /// <summary>
            /// 3
            /// </summary>
            CompanyAdmin = 3,
            /// <summary>
            /// 4
            /// </summary>
            EnterpriseSystemUser = 4,
            /// <summary>
            /// 5
            /// </summary>
            eTurnsSystemUser = 5
        }

        //public enum BillingRoomTypeEnum
        //{
        //    AssetOnly = 1,
        //    eVMI = 2,
        //    Manage = 3,
        //    Replenish = 4,
        //    RFID = 5,
        //    ToolandAssetOnly = 6,
        //    ToolOnly = 7,
        //    Truck = 8,
        //    Optimize = 9
        //}
    }

    public enum EturnsDTOEnum
    {
        None = 0,
        ItemMasterDTO = 1,
        QuickListMasterDTO = 2,
        InventoryCountDTO = 3,
        CycleCountSettingDTO = 4,
        MaterialStagingDTO = 5,
        RequisitionMasterDTO = 6,
        WorkOrderDTO = 7,
        ProjectMasterDTO = 8,
        OrderMasterDTO = 9,
        TransferMasterDTO = 10,
        ToolMasterDTO = 11,
        AssetMasterDTO = 12,
        ToolsSchedulerDTO = 13,
        ToolsSchedulerMappingDTO = 14,
        ToolsMaintenanceDTO = 15,
        ToolAssetOrderMasterDTO = 16,
        ToolMasterDTO_17 = 17,
        AssetCategoryMasterDTO = 18,
        BarcodeMasterDTO = 19,
        CategoryMasterDTO = 20,
        CostUOMMasterDTO = 21,
        CustomerMasterDTO = 22,
        FTPMasterDTO = 23,
        GLAccountMasterDTO = 24,
        InventoryClassificationMasterDTO = 25,
        ItemMasterBinDTO = 26,
        LocationMasterDTO = 27,
        ManufacturerMasterDTO = 28,
        ShipViaDTO = 29,
        SupplierMasterDTO = 30,
        TechnicianMasterDTO = 31,
        ToolCategoryMasterDTO = 32,
        UnitMasterDTO = 33,
        VenderMasterDTO = 34, 
        BinMasterDTO = 35,
        QuoteMasterDTO = 36

    }

    public enum ResourceFilesEnum
    {
        ResItemMaster, 
        ResQuickList,
        ResInventoryCount, 
        ResCycleCountSetting,
        ResMaterialStaging,
        ResOrder,
        ResRequisitionMaster, 
        ResWorkOrder,
        ResProjectMaster,
        ResTransfer,
        ResToolMaster,
        ResAssetMaster,
        ResToolsScheduler,
        ResToolsSchedulerMapping,
        ResToolsMaintenance,
        ResKitToolMaster,
        ResAssetCategory,
        ResBarcodeMaster,
        ResCategoryMaster,
        ResCostUOMMaster,
        ResCustomer,
        ResFTPMaster,
        ResGLAccount,
        ResInventoryClassificationMaster,
        ResLocation,
        ResManufacturer,
        ResShipVia,
        ResSupplierMaster,
        ResTechnician,
        ResCommon,
        ResToolCategory,
        ResUnitMaster,
        ResVenderMaster,
        ResBin,
        ResQuoteMaster

    }

    public enum ValidationRuleType
    {
        None = 0,
        Required = 1
    }

    public enum NarrowSearchSaveListEnum {
        ItemMaster = 0,
        ItemBinMaster = 1
    }

    public enum EVMIPollErrorTypeEnum
    {
        None = 0,

        /// <summary>
        /// Any error while accessing com port
        /// </summary>
        COMPortError = 1,

        /// <summary>
        /// Exception occured during execution except com port command
        /// </summary>
        Exception = 2,

        /// <summary>
        /// Some validations failed on data but error is not severe
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Some validations failed on data and can not process data
        /// </summary>
        DataError = 4,

        COMPortError_AND_Warning = 5,
        COMPortError_AND_DataError = 6,
        Exception_AND_Warning = 7,

        /// <summary>
        /// Sensor Weight is less than Weight Per Piece. But It is within weight variance range.
        /// </summary>
        WithinWeightVarianceRange = 8
    }
}
