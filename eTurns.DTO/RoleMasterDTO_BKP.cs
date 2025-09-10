using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class RoleMasterDTO_BKP
    {

        /*
         AdminPermission
         AssetPermission
         CatalogPermission
         CountPermission
         ExportPermission
         ImportPermission
         IssuePermission
         KittingPermission
         MaintenancePermission
         QuickListPermission
         Suggested OrderPermission
         Suggested TransferPermission
         OrderPermission
         ReceivingPermission
         ReportingPermission
         RequisitionPermission
         StagingPermission
         SupportingDataPermission
         TransferPermission
         WorkOrderPermission
         RoomAccessInformation
         TransferRoomAccessInformation
         AllowEditsConsignedItems
        CreateDefaultBin
        AutomaticallyCreatePurchasesFromScanList
        DefaultBinName
        WeightDecimalPointsSetting
        QuantityDecimalPointsSetting
        ReplenishmentDecimalPointsSetting
        AllowFlySupportingDataChanges
        eVMI_wait_command
        eVMI_wait_port
        AllowPull_to_completed
        AllowOrder_to_Submitted
        AllowConsignedItems_to_Ordered
        AllowLoginPassword_to_Stored
        AllowSuper-adminPermissions
    
        Item_View
        Job_View
        Checkout_View
        Purchase_View
        Tool_View
        Project_View
        Unit_View
        Schedule_Unit_View
        Customer_View        
        Hide_Cost
        Enable_Item_Rename
        Supplier_Filter

       
         */

        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public List<Rooms> RoomAccess { get; set; }
        public List<Rooms> ReplenishingRooms { get; set; }
        public List<SetPermissions> Permissions { get; set; }
        public List<string> PermissionsName { get; set; }
        public List<SetPermissions> MasterPermissions { get; set; }
        public List<CheckPermission> CheckPermissions { get; set; }

        List<string> PermissionsNameList = new List<string>();

        public class Rooms
        {
            public int RoomID { get; set; }
            public string RoomName { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime UpdatedDate { get; set; }
            public string User { get; set; }
        }

        public class SetPermissions
        {
            public string PermmisionName { get; set; }
            public string PermmisionTitle { get; set; }
            public bool Insert { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }
            public bool View { get; set; }
            public string PermissionText { get; set; }
        }

        public class CheckPermission
        {
            public string PermmisionName { get; set; }
            public string PermmisionTitle { get; set; }
            public bool isChecked { get; set; }

        }

        public RoleMasterDTO_BKP GetPermmisionsRecords()
        {
            RoleMasterDTO_BKP obj = new RoleMasterDTO_BKP();

            #region "Set default permissions "
            List<SetPermissions> objRole = new List<SetPermissions>();
            /*  AdminPermission,AssetPermission,CatalogPermission,CountPermission ,ExportPermission*/
            objRole.Add(SetPermission("AdminPermission", "Admin Permission", true, true, true, true, false));
            objRole.Add(SetPermission("AssetPermission", "Asset Permission", true, true, true, true, false));
            objRole.Add(SetPermission("CatalogPermission", "Catalog Permission", true, true, true, true, false));
            objRole.Add(SetPermission("CountPermission", "Count Permission", true, true, true, true, false));
            objRole.Add(SetPermission("ExportPermission", "Export Permission", true, true, true, true, false));

            /*ImportPermission,IssuePermission, KittingPermission,MaintenancePermission, QuickListPermission*/
            objRole.Add(SetPermission("ImportPermission", "Import Permission", true, true, true, true, false));
            objRole.Add(SetPermission("IssuePermission", "Issue Permission", true, true, true, true, false));
            objRole.Add(SetPermission("KittingPermission", "Kitting Permission", true, true, true, true, false));
            objRole.Add(SetPermission("MaintenancePermission", "Maintenance Permission", true, true, true, true, false));
            objRole.Add(SetPermission("QuickListPermission", "QuickList Permission", true, true, true, true, false));

            /*  Suggested OrderPermission, Suggested TransferPermission, OrderPermission,ReceivingPermission,  ReportingPermission, RequisitionPermission */
            objRole.Add(SetPermission("SuggestedOrderPermission", "Suggested Order Permission", true, true, true, true, false));
            objRole.Add(SetPermission("SuggestedTransferPermission", "Suggested Transfer Permission", true, true, true, true, false));
            objRole.Add(SetPermission("OrderPermission", "Order Permission", true, true, true, true, false));
            objRole.Add(SetPermission("ReceivingPermission", "Receiving Permission", true, true, true, true, false));
            objRole.Add(SetPermission("ReportingPermission", "Reporting Permission", true, true, true, true, false));
            objRole.Add(SetPermission("RequisitionPermission", "Requisition Permission", true, true, true, true, false));

            /*   StagingPermission,        SupportingDataPermission,        TransferPermission,        WorkOrderPermission, RoomAccessInformation */
            objRole.Add(SetPermission("StagingPermission", "Staging Permission", true, true, true, true, false));
            objRole.Add(SetPermission("SupportingDataPermission", "Supporting Data Permission", true, true, true, true, false));
            objRole.Add(SetPermission("TransferPermission", "Transfer Permission", true, true, true, true, false));
            objRole.Add(SetPermission("WorkOrderPermission", "Work Order Permission", true, true, true, true, false));

            ///*Item_View,Job_View,Checkout_View,Purchase_View,Tool_View,Project_View,Unit_View,Schedule_Unit_View,Customer_View,Hide_Cost,
            // Enable_Item_Rename,Supplier_Filter*/
            //objRole.Add(SetPermission("Item_View", "Item View", true, true, true, true));
            //objRole.Add(SetPermission("Job_View", "Job View", true, true, true, true));
            //objRole.Add(SetPermission("Checkout_View", "Checkout View", true, true, true, true));
            //objRole.Add(SetPermission("Purchase_View", "Purchase View", true, true, true, true));
            //objRole.Add(SetPermission("Tool_View", "Tool View", true, true, true, true));

            //objRole.Add(SetPermission("Project_View", "Project View", true, true, true, true));
            //objRole.Add(SetPermission("Unit_View", "Unit View", true, true, true, true));
            //objRole.Add(SetPermission("Schedule_Unit_View", "Schedule Unit View", true, true, true, true));
            //objRole.Add(SetPermission("Customer_View", "Customer View", true, true, true, true));
            //objRole.Add(SetPermission("Hide_Cost", "Hide View", true, true, true, true));
            //objRole.Add(SetPermission("Enable_Item_Rename", "Enable Item Rename", true, true, true, true));
            //objRole.Add(SetPermission("Supplier_Filter", "Supplier Filter", true, true, true, true));
            #endregion

            #region "Set default permissions "
            List<SetPermissions> objMasterRole = new List<SetPermissions>();

            /*Item_View,Job_View,Checkout_View,Purchase_View,Tool_View,Project_View,Unit_View,Schedule_Unit_View,Customer_View,Hide_Cost,
             Enable_Item_Rename,Supplier_Filter*/
            objMasterRole.Add(SetPermission("Item_View", "Item View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Job_View", "Job View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Checkout_View", "Checkout View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Purchase_View", "Purchase View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Tool_View", "Tool View", true, true, true, true, false));

            objMasterRole.Add(SetPermission("Project_View", "Project View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Unit_View", "Unit View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Schedule_Unit_View", "Schedule Unit View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Customer_View", "Customer View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Hide_Cost", "Hide View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Enable_Item_Rename", "Enable Item Rename", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Supplier_Filter", "Supplier Filter", true, true, true, true, false));
            #endregion
            obj.MasterPermissions = objMasterRole;
            obj.RoomAccess = SetRooms();
            obj.Permissions = objRole;
            obj.ReplenishingRooms = SetReplenishRooms();
            obj.CheckPermissions = GetCheckedPermmisionsRecords(false);
            return obj;
        }

        public RoleMasterDTO_BKP GetDefaultPermmisionsRecords()
        {
            RoleMasterDTO_BKP obj = new RoleMasterDTO_BKP();

            #region "Set default permissions "
            List<SetPermissions> objRole = new List<SetPermissions>();
            /*  AdminPermission,AssetPermission,CatalogPermission,CountPermission ,ExportPermission*/
            objRole.Add(SetPermission("AdminPermission", "Admin Permission", false, false, false, false, false));
            objRole.Add(SetPermission("AssetPermission", "Asset Permission", false, false, false, false, false));
            objRole.Add(SetPermission("CatalogPermission", "Catalog Permission", false, false, false, false, false));
            objRole.Add(SetPermission("CountPermission", "Count Permission", false, false, false, false, false));
            objRole.Add(SetPermission("ExportPermission", "Export Permission", false, false, false, false, false));

            /*ImportPermission,IssuePermission, KittingPermission,MaintenancePermission, QuickListPermission*/
            objRole.Add(SetPermission("ImportPermission", "Import Permission", false, false, false, false, false));
            objRole.Add(SetPermission("IssuePermission", "Issue Permission", false, false, false, false, false));
            objRole.Add(SetPermission("KittingPermission", "Kitting Permission", false, false, false, false, false));
            objRole.Add(SetPermission("MaintenancePermission", "Maintenance Permission", false, false, false, false, false));
            objRole.Add(SetPermission("QuickListPermission", "QuickList Permission", false, false, false, false, false));

            /*  Suggested OrderPermission, Suggested TransferPermission, OrderPermission,ReceivingPermission,  ReportingPermission, RequisitionPermission */
            objRole.Add(SetPermission("SuggestedOrderPermission", "Suggested Order Permission", false, false, false, false, false));
            objRole.Add(SetPermission("SuggestedTransferPermission", "Suggested Transfer Permission", false, false, false, false, false));
            objRole.Add(SetPermission("OrderPermission", "Order Permission", false, false, false, false, false));
            objRole.Add(SetPermission("ReceivingPermission", "Receiving Permission", false, false, false, false, false));
            objRole.Add(SetPermission("ReportingPermission", "Reporting Permission", false, false, false, false, false));
            objRole.Add(SetPermission("RequisitionPermission", "Requisition Permission", false, false, false, false, false));

            /*   StagingPermission,        SupportingDataPermission,        TransferPermission,        WorkOrderPermission, RoomAccessInformation */
            objRole.Add(SetPermission("StagingPermission", "Staging Permission", false, false, false, false, false));
            objRole.Add(SetPermission("SupportingDataPermission", "Supporting Data Permission", false, false, false, false, false));
            objRole.Add(SetPermission("TransferPermission", "Transfer Permission", false, false, false, false, false));
            objRole.Add(SetPermission("WorkOrderPermission", "Work Order Permission", false, false, false, false, false));

            #endregion

            #region "Set default permissions "
            List<SetPermissions> objMasterRole = new List<SetPermissions>();

            /*Item_View,Job_View,Checkout_View,Purchase_View,Tool_View,Project_View,Unit_View,Schedule_Unit_View,Customer_View,Hide_Cost,
             Enable_Item_Rename,Supplier_Filter*/
            objMasterRole.Add(SetPermission("Item_View", "Item View", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Job_View", "Job View", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Checkout_View", "Checkout View", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Purchase_View", "Purchase View", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Tool_View", "Tool View", false, false, false, false, false));

            objMasterRole.Add(SetPermission("Project_View", "Project View", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Unit_View", "Unit View", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Schedule_Unit_View", "Schedule Unit View", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Customer_View", "Customer View", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Hide_Cost", "Hide View", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Enable_Item_Rename", "Enable Item Rename", false, false, false, false, false));
            objMasterRole.Add(SetPermission("Supplier_Filter", "Supplier Filter", false, false, false, false, false));
            #endregion
            obj.MasterPermissions = objMasterRole;
            obj.RoomAccess = null;
            obj.Permissions = objRole;
            obj.ReplenishingRooms = null;
            obj.CheckPermissions = GetDefaultCheckedPermmisionsRecords(false);
            return obj;
        }
        public List<CheckPermission> GetDefaultCheckedPermmisionsRecords(bool AddPermissionName)
        {
            List<CheckPermission> objRole = new List<CheckPermission>();

            objRole.Add(SetCheckPermission("AllowEditsConsignedItems", "Allow Edits Consigned Items Permission", false, AddPermissionName));

            objRole.Add(SetCheckPermission("AutomaticallyCreatePurchasesFromScanList", "Automatically Create Purchases From ScanList", false, AddPermissionName));


            objRole.Add(SetCheckPermission("WeightDecimalPointsSetting", "Weight Decimal Points Setting", false, AddPermissionName));
            objRole.Add(SetCheckPermission("QuantityDecimalPointsSetting", "Quantity Decimal Points Setting", false, AddPermissionName));
            objRole.Add(SetCheckPermission("ReplenishmentDecimalPointsSetting", "Replenishment Decimal Points Setting", false, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowFlySupportingDataChanges", "Allow Fly Supporting Data Changes", false, AddPermissionName));


            objRole.Add(SetCheckPermission("AllowPull_to_completed", "Allow Pull to completed", false, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowOrder_to_Submitted", "Allow Order to Submitted", false, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowConsignedItems_to_Ordered", "Allow Consigned Items to Ordered", false, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowLoginPassword_to_Stored", "Allow Login Password to Stored", false, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowSuper-adminPermissions", "Allow Super-admin Permissions", false, AddPermissionName));

            objRole.Add(SetCheckPermission("Enable_Item_Rename", "Enable Item Rename", false, AddPermissionName));
            objRole.Add(SetCheckPermission("CreateDefaultBin", "Create Default Bin", false, AddPermissionName));

            return objRole;
        }

        public RoleMasterDTO_BKP GetPermmisionsRecords(int RoomID)
        {

            RoleMasterDTO_BKP obj = new RoleMasterDTO_BKP();

            if (RoomID == 1)
            {
                #region "Set default permissions "
                List<SetPermissions> objRole = new List<SetPermissions>();
                /*  AdminPermission,AssetPermission,CatalogPermission,CountPermission ,ExportPermission*/
                objRole.Add(SetPermission("AdminPermission", "Admin Permission", true, true, true, true, false));
                objRole.Add(SetPermission("AssetPermission", "Asset Permission", true, true, true, true, false));
                objRole.Add(SetPermission("CatalogPermission", "Catalog Permission", true, true, true, true, false));
                objRole.Add(SetPermission("CountPermission", "Count Permission", true, true, true, true, false));
                objRole.Add(SetPermission("ExportPermission", "Export Permission", true, true, true, true, false));

                /*ImportPermission,IssuePermission, KittingPermission,MaintenancePermission, QuickListPermission*/
                objRole.Add(SetPermission("ImportPermission", "Import Permission", true, true, true, true, false));
                objRole.Add(SetPermission("IssuePermission", "Issue Permission", true, true, true, true, false));
                objRole.Add(SetPermission("KittingPermission", "Kitting Permission", true, true, true, true, false));
                objRole.Add(SetPermission("MaintenancePermission", "Maintenance Permission", true, true, true, true, false));
                objRole.Add(SetPermission("QuickListPermission", "QuickList Permission", true, true, true, true, false));

                /*  Suggested OrderPermission, Suggested TransferPermission, OrderPermission,ReceivingPermission,  ReportingPermission, RequisitionPermission */
                objRole.Add(SetPermission("SuggestedOrderPermission", "Suggested Order Permission", true, true, true, true, false));
                objRole.Add(SetPermission("SuggestedTransferPermission", "Suggested Transfer Permission", true, true, true, true, false));
                objRole.Add(SetPermission("OrderPermission", "Order Permission", true, true, true, true, false));
                objRole.Add(SetPermission("ReceivingPermission", "Receiving Permission", true, true, true, true, false));
                objRole.Add(SetPermission("ReportingPermission", "Reporting Permission", true, true, true, true, false));
                objRole.Add(SetPermission("RequisitionPermission", "Requisition Permission", true, true, true, true, false));

                /*   StagingPermission,        SupportingDataPermission,        TransferPermission,        WorkOrderPermission, RoomAccessInformation */
                objRole.Add(SetPermission("StagingPermission", "Staging Permission", true, true, true, true, false));
                objRole.Add(SetPermission("SupportingDataPermission", "Supporting Data Permission", true, true, true, true, false));
                objRole.Add(SetPermission("TransferPermission", "Transfer Permission", true, true, true, true, false));
                objRole.Add(SetPermission("WorkOrderPermission", "Work Order Permission", true, true, true, true, false));

                #endregion

                #region "Set default permissions "
                List<SetPermissions> objMasterRole = new List<SetPermissions>();

                /*Item_View,Job_View,Checkout_View,Purchase_View,Tool_View,Project_View,Unit_View,Schedule_Unit_View,Customer_View,Hide_Cost,
                 Enable_Item_Rename,Supplier_Filter*/
                objMasterRole.Add(SetPermission("Item_View", "Item View", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Job_View", "Job View", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Checkout_View", "Checkout View", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Purchase_View", "Purchase View", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Tool_View", "Tool View", true, true, true, true, false));

                objMasterRole.Add(SetPermission("Project_View", "Project View", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Unit_View", "Unit View", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Schedule_Unit_View", "Schedule Unit View", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Customer_View", "Customer View", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Hide_Cost", "Hide View", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Enable_Item_Rename", "Enable Item Rename", true, true, true, true, false));
                objMasterRole.Add(SetPermission("Supplier_Filter", "Supplier Filter", true, true, true, true, false));
                #endregion
                obj.MasterPermissions = objMasterRole;
                obj.RoomAccess = SetRooms();
                obj.Permissions = objRole;
                obj.ReplenishingRooms = SetReplenishRooms();
                obj.CheckPermissions = GetCheckedPermmisionsRecords(false);
            }
            else
            {
                #region "Set default permissions "
                List<SetPermissions> objRole = new List<SetPermissions>();
                /*  AdminPermission,AssetPermission,CatalogPermission,CountPermission ,ExportPermission*/
                objRole.Add(SetPermission("AdminPermission", "Admin Permission", true, true, true, false, true));
                objRole.Add(SetPermission("AssetPermission", "Asset Permission", false, false, false, false, true));
                objRole.Add(SetPermission("CatalogPermission", "Catalog Permission", false, false, false, false, true));
                objRole.Add(SetPermission("CountPermission", "Count Permission", false, false, false, false, true));
                objRole.Add(SetPermission("ExportPermission", "Export Permission", false, false, false, false, true));

                /*ImportPermission,IssuePermission, KittingPermission,MaintenancePermission, QuickListPermission*/
                objRole.Add(SetPermission("ImportPermission", "Import Permission", false, false, false, false, true));
                objRole.Add(SetPermission("IssuePermission", "Issue Permission", false, false, false, false, true));
                objRole.Add(SetPermission("KittingPermission", "Kitting Permission", false, false, false, false, true));
                objRole.Add(SetPermission("MaintenancePermission", "Maintenance Permission", false, false, false, false, true));
                objRole.Add(SetPermission("QuickListPermission", "QuickList Permission", false, false, false, false, true));

                /*  Suggested OrderPermission, Suggested TransferPermission, OrderPermission,ReceivingPermission,  ReportingPermission, RequisitionPermission */
                objRole.Add(SetPermission("SuggestedOrderPermission", "Suggested Order Permission", true, true, true, true, true));
                objRole.Add(SetPermission("SuggestedTransferPermission", "Suggested Transfer Permission", true, true, true, true, true));
                objRole.Add(SetPermission("OrderPermission", "Order Permission", true, true, true, true, true));
                objRole.Add(SetPermission("ReceivingPermission", "Receiving Permission", true, true, true, true, true));
                objRole.Add(SetPermission("ReportingPermission", "Reporting Permission", true, true, true, true, true));
                objRole.Add(SetPermission("RequisitionPermission", "Requisition Permission", true, true, true, true, true));

                /*   StagingPermission,        SupportingDataPermission,        TransferPermission,        WorkOrderPermission, RoomAccessInformation */
                objRole.Add(SetPermission("StagingPermission", "Staging Permission", true, true, true, true, true));
                objRole.Add(SetPermission("SupportingDataPermission", "Supporting Data Permission", true, true, true, true, true));
                objRole.Add(SetPermission("TransferPermission", "Transfer Permission", true, true, true, true, true));
                objRole.Add(SetPermission("WorkOrderPermission", "Work Order Permission", true, true, true, true, true));

                #endregion

                #region "Set default permissions "
                List<SetPermissions> objMasterRole = new List<SetPermissions>();

                /*Item_View,Job_View,Checkout_View,Purchase_View,Tool_View,Project_View,Unit_View,Schedule_Unit_View,Customer_View,Hide_Cost,
                 Enable_Item_Rename,Supplier_Filter*/
                objMasterRole.Add(SetPermission("Item_View", "Item View", false, false, true, false, false));
                objMasterRole.Add(SetPermission("Job_View", "Job View", false, false, true, false, false));
                objMasterRole.Add(SetPermission("Checkout_View", "Checkout View", false, false, true, false, false));
                objMasterRole.Add(SetPermission("Purchase_View", "Purchase View", false, false, true, false, false));
                objMasterRole.Add(SetPermission("Tool_View", "Tool View", false, false, true, false, false));

                objMasterRole.Add(SetPermission("Project_View", "Project View", false, false, true, false, false));
                objMasterRole.Add(SetPermission("Unit_View", "Unit View", false, false, true, false, false));
                objMasterRole.Add(SetPermission("Schedule_Unit_View", "Schedule Unit View", false, false, true, false, false));
                objMasterRole.Add(SetPermission("Customer_View", "Customer View", false, false, true, true, true));
                objMasterRole.Add(SetPermission("Hide_Cost", "Hide View", false, false, true, true, true));
                objMasterRole.Add(SetPermission("Enable_Item_Rename", "Enable Item Rename", false, true, false, true, true));
                objMasterRole.Add(SetPermission("Supplier_Filter", "Supplier Filter", false, false, true, true, true));
                #endregion
                obj.MasterPermissions = objMasterRole;
                obj.RoomAccess = SetRooms();
                obj.Permissions = objRole;
                obj.ReplenishingRooms = SetReplenishRooms();
                obj.CheckPermissions = GetCheckedPermmisionsRecords(true);
            }

            return obj;
        }

        public List<CheckPermission> GetCheckedPermmisionsRecords(bool AddPermissionName)
        {
            List<CheckPermission> objRole = new List<CheckPermission>();

            objRole.Add(SetCheckPermission("AllowEditsConsignedItems", "Allow Edits Consigned Items Permission", true, AddPermissionName));

            objRole.Add(SetCheckPermission("AutomaticallyCreatePurchasesFromScanList", "Automatically Create Purchases From ScanList", true, AddPermissionName));


            objRole.Add(SetCheckPermission("WeightDecimalPointsSetting", "Weight Decimal Points Setting", true, AddPermissionName));
            objRole.Add(SetCheckPermission("QuantityDecimalPointsSetting", "Quantity Decimal Points Setting", true, AddPermissionName));
            objRole.Add(SetCheckPermission("ReplenishmentDecimalPointsSetting", "Replenishment Decimal Points Setting", true, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowFlySupportingDataChanges", "Allow Fly Supporting Data Changes", true, AddPermissionName));


            objRole.Add(SetCheckPermission("AllowPull_to_completed", "Allow Pull to completed", false, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowOrder_to_Submitted", "Allow Order to Submitted", true, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowConsignedItems_to_Ordered", "Allow Consigned Items to Ordered", false, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowLoginPassword_to_Stored", "Allow Login Password to Stored", false, AddPermissionName));
            objRole.Add(SetCheckPermission("AllowSuper-adminPermissions", "Allow Super-admin Permissions", false, AddPermissionName));

            objRole.Add(SetCheckPermission("Enable_Item_Rename", "Enable Item Rename", true, AddPermissionName));
            objRole.Add(SetCheckPermission("CreateDefaultBin", "Create Default Bin", true, AddPermissionName));
            //objRole.Add(SetCheckPermission("Supplier_Filter", "Supplier Filter", true, AddPermissionName));
            //objRole.Add(SetCheckPermission("DefaultBinName", "Default Bin Name", true, AddPermissionName));
            //objRole.Add(SetCheckPermission("eVMI_wait_command", "eVMI wait command", true, AddPermissionName));
            //objRole.Add(SetCheckPermission("eVMI_wait_port", "eVMI wait port", true, AddPermissionName));

            return objRole;
        }

        private SetPermissions SetPermission(string Name, string Title, bool Insert, bool Update, bool Delete, bool View, bool AddPermissionName)
        {
            if (AddPermissionName)
                PermissionsNameList.Add(Title);

            SetPermissions obj = new SetPermissions();
            obj.PermmisionName = Name;
            obj.PermmisionTitle = Title;
            obj.Insert = Insert;
            obj.Update = Update;
            obj.Delete = Delete;
            obj.View = View;
            string pText = string.Empty;

            if (Insert == true)
                pText = "Insert";
            if (Update == true)
            {
                if (string.IsNullOrEmpty(pText))
                    pText = "Update";
                else
                    pText += "/Update";
            }
            if (Delete == true)
            {
                if (string.IsNullOrEmpty(pText))
                    pText = "Delete";
                else
                    pText += "/Delete";
            }
            if (View == true)
            {
                if (string.IsNullOrEmpty(pText))
                    pText = "View";
                else
                    pText += "/View";
            }
            obj.PermissionText = pText;
            return obj;
        }

        private CheckPermission SetCheckPermission(string Name, string Title, bool isChecked, bool AddPermissionName)
        {
            if (AddPermissionName)
                PermissionsNameList.Add(Title);

            CheckPermission obj = new CheckPermission();
            obj.PermmisionName = Name;
            obj.PermmisionTitle = Title;
            obj.isChecked = isChecked;
            return obj;
        }

        private List<Rooms> SetRooms()
        {
            Rooms obj = new Rooms();
            obj.RoomID = 1;
            obj.RoomName = "Room 1";
            obj.CreatedDate = DateTime.Now;
            obj.UpdatedDate = DateTime.Now;
            obj.User = "Virat";

            List<Rooms> objlist = new List<Rooms>();
            objlist.Add(obj);

            obj = new Rooms();
            obj.RoomID = 2;
            obj.RoomName = "Room 2";
            obj.CreatedDate = DateTime.Now;
            obj.UpdatedDate = DateTime.Now;
            obj.User = "Virat";
            objlist.Add(obj);

            return objlist;
        }

        private List<Rooms> SetReplenishRooms()
        {
            Rooms obj = new Rooms();
            obj.RoomID = 3;
            obj.RoomName = "Room 3";
            obj.CreatedDate = DateTime.Now;
            obj.UpdatedDate = DateTime.Now;
            obj.User = "Virat";

            List<Rooms> objlist = new List<Rooms>();
            objlist.Add(obj);

            obj = new Rooms();
            obj.RoomID = 4;
            obj.RoomName = "Room 4";
            obj.CreatedDate = DateTime.Now;
            obj.UpdatedDate = DateTime.Now;
            obj.User = "Virat";
            objlist.Add(obj);

            return objlist;
        }

        #region "************ Rolewise Permmisions Records ***************"
        public List<RoleMasterDTO_BKP> GetRolewisePermmisionsRecords()
        {
            List<RoleMasterDTO_BKP> obj = new List<RoleMasterDTO_BKP>();

            #region " Role 1 Permissions "
            RoleMasterDTO_BKP objRole1 = new RoleMasterDTO_BKP();
            objRole1.RoleID = 1;
            objRole1.RoleName = "Role 1";
            #region "Set default permissions "
            List<SetPermissions> objRole = new List<SetPermissions>();
            /*  AdminPermission,AssetPermission,CatalogPermission,CountPermission ,ExportPermission*/
            objRole.Add(SetPermission("AdminPermission", "Admin Permission", true, true, true, true, true));
            objRole.Add(SetPermission("AssetPermission", "Asset Permission", true, true, true, true, true));
            objRole.Add(SetPermission("CatalogPermission", "Catalog Permission", true, true, true, true, true));
            objRole.Add(SetPermission("CountPermission", "Count Permission", true, true, true, true, true));
            objRole.Add(SetPermission("ExportPermission", "Export Permission", true, true, true, true, true));

            /*ImportPermission,IssuePermission, KittingPermission,MaintenancePermission, QuickListPermission*/
            objRole.Add(SetPermission("ImportPermission", "Import Permission", true, true, true, true, true));
            objRole.Add(SetPermission("IssuePermission", "Issue Permission", true, true, true, true, true));
            objRole.Add(SetPermission("KittingPermission", "Kitting Permission", true, true, true, true, true));
            objRole.Add(SetPermission("MaintenancePermission", "Maintenance Permission", true, true, true, true, true));
            objRole.Add(SetPermission("QuickListPermission", "QuickList Permission", true, true, true, true, true));

            /*  Suggested OrderPermission, Suggested TransferPermission, OrderPermission,ReceivingPermission,  ReportingPermission, RequisitionPermission */
            objRole.Add(SetPermission("SuggestedOrderPermission", "Suggested Order Permission", true, true, true, true, true));
            objRole.Add(SetPermission("SuggestedTransferPermission", "Suggested Transfer Permission", true, true, true, true, true));
            objRole.Add(SetPermission("OrderPermission", "Order Permission", true, true, true, true, true));
            objRole.Add(SetPermission("ReceivingPermission", "Receiving Permission", true, true, true, true, true));
            objRole.Add(SetPermission("ReportingPermission", "Reporting Permission", true, true, true, true, true));
            objRole.Add(SetPermission("RequisitionPermission", "Requisition Permission", true, true, true, true, true));

            /*   StagingPermission,        SupportingDataPermission,        TransferPermission,        WorkOrderPermission, RoomAccessInformation */
            objRole.Add(SetPermission("StagingPermission", "Staging Permission", true, true, true, true, true));
            objRole.Add(SetPermission("SupportingDataPermission", "Supporting Data Permission", true, true, true, true, true));
            objRole.Add(SetPermission("TransferPermission", "Transfer Permission", true, true, true, true, true));
            objRole.Add(SetPermission("WorkOrderPermission", "Work Order Permission", true, true, true, true, true));

            #endregion

            #region "Set default permissions "
            List<SetPermissions> objMasterRole = new List<SetPermissions>();

            /*Item_View,Job_View,Checkout_View,Purchase_View,Tool_View,Project_View,Unit_View,Schedule_Unit_View,Customer_View,Hide_Cost,
             Enable_Item_Rename,Supplier_Filter*/
            objMasterRole.Add(SetPermission("Item_View", "Item View", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Job_View", "Job View", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Checkout_View", "Checkout View", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Purchase_View", "Purchase View", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Tool_View", "Tool View", true, true, true, true, true));

            objMasterRole.Add(SetPermission("Project_View", "Project View", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Unit_View", "Unit View", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Schedule_Unit_View", "Schedule Unit View", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Customer_View", "Customer View", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Hide_Cost", "Hide View", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Enable_Item_Rename", "Enable Item Rename", true, true, true, true, true));
            objMasterRole.Add(SetPermission("Supplier_Filter", "Supplier Filter", true, true, true, true, true));
            #endregion
            objRole1.MasterPermissions = objMasterRole;
            objRole1.RoomAccess = null;
            objRole1.Permissions = objRole;
            objRole1.ReplenishingRooms = null;
            objRole1.CheckPermissions = GetCheckedPermmisionsRecords(true);

            obj.Add(objRole1);
            #endregion

            #region " Role 2 Permissions "
            RoleMasterDTO_BKP objRole2 = new RoleMasterDTO_BKP();
            objRole1.RoleID = 2;
            objRole2.RoleName = "Role 2";
            #region "Set default permissions "
            objRole = new List<SetPermissions>();
            /*  AdminPermission,AssetPermission,CatalogPermission,CountPermission ,ExportPermission*/
            objRole.Add(SetPermission("AdminPermission", "Admin Permission", true, true, true, true, false));
            objRole.Add(SetPermission("AssetPermission", "Asset Permission", true, true, true, true, false));
            objRole.Add(SetPermission("CatalogPermission", "Catalog Permission", true, true, true, true, false));
            objRole.Add(SetPermission("CountPermission", "Count Permission", true, true, true, true, false));
            objRole.Add(SetPermission("ExportPermission", "Export Permission", true, true, true, true, false));

            /*ImportPermission,IssuePermission, KittingPermission,MaintenancePermission, QuickListPermission*/
            objRole.Add(SetPermission("ImportPermission", "Import Permission", true, true, true, true, false));
            objRole.Add(SetPermission("IssuePermission", "Issue Permission", true, true, true, true, false));
            objRole.Add(SetPermission("KittingPermission", "Kitting Permission", true, true, true, true, false));
            objRole.Add(SetPermission("MaintenancePermission", "Maintenance Permission", true, true, true, true, false));
            objRole.Add(SetPermission("QuickListPermission", "QuickList Permission", true, true, true, true, false));

            /*  Suggested OrderPermission, Suggested TransferPermission, OrderPermission,ReceivingPermission,  ReportingPermission, RequisitionPermission */
            objRole.Add(SetPermission("SuggestedOrderPermission", "Suggested Order Permission", true, true, true, true, false));
            objRole.Add(SetPermission("SuggestedTransferPermission", "Suggested Transfer Permission", true, true, true, true, false));
            objRole.Add(SetPermission("OrderPermission", "Order Permission", true, true, true, true, false));
            objRole.Add(SetPermission("ReceivingPermission", "Receiving Permission", true, true, true, true, false));
            objRole.Add(SetPermission("ReportingPermission", "Reporting Permission", true, true, true, true, false));
            objRole.Add(SetPermission("RequisitionPermission", "Requisition Permission", true, true, true, true, false));

            /*   StagingPermission,        SupportingDataPermission,        TransferPermission,        WorkOrderPermission, RoomAccessInformation */
            objRole.Add(SetPermission("StagingPermission", "Staging Permission", true, true, true, true, false));
            objRole.Add(SetPermission("SupportingDataPermission", "Supporting Data Permission", true, true, true, true, false));
            objRole.Add(SetPermission("TransferPermission", "Transfer Permission", true, true, true, true, false));
            objRole.Add(SetPermission("WorkOrderPermission", "Work Order Permission", true, true, true, true, false));

            #endregion

            #region "Set default permissions "
            objMasterRole = new List<SetPermissions>();

            /*Item_View,Job_View,Checkout_View,Purchase_View,Tool_View,Project_View,Unit_View,Schedule_Unit_View,Customer_View,Hide_Cost,
             Enable_Item_Rename,Supplier_Filter*/
            objMasterRole.Add(SetPermission("Item_View", "Item View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Job_View", "Job View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Checkout_View", "Checkout View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Purchase_View", "Purchase View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Tool_View", "Tool View", true, true, true, true, false));

            objMasterRole.Add(SetPermission("Project_View", "Project View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Unit_View", "Unit View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Schedule_Unit_View", "Schedule Unit View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Customer_View", "Customer View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Hide_Cost", "Hide View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Enable_Item_Rename", "Enable Item Rename", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Supplier_Filter", "Supplier Filter", true, true, true, true, false));
            #endregion
            objRole2.MasterPermissions = objMasterRole;
            objRole2.RoomAccess = SetRooms();
            objRole2.Permissions = objRole;
            objRole2.ReplenishingRooms = SetReplenishRooms();
            objRole2.CheckPermissions = GetCheckedPermmisionsRecords(false);

            obj.Add(objRole2);
            #endregion

            #region " Role 3 Permissions "
            RoleMasterDTO_BKP objRole3 = new RoleMasterDTO_BKP();
            objRole1.RoleID = 3;
            objRole3.RoleName = "Role 3";
            #region "Set default permissions "
            objRole = new List<SetPermissions>();
            /*  AdminPermission,AssetPermission,CatalogPermission,CountPermission ,ExportPermission*/
            objRole.Add(SetPermission("AdminPermission", "Admin Permission", true, true, true, true, false));
            objRole.Add(SetPermission("AssetPermission", "Asset Permission", true, true, true, true, false));
            objRole.Add(SetPermission("CatalogPermission", "Catalog Permission", true, true, true, true, false));
            objRole.Add(SetPermission("CountPermission", "Count Permission", true, true, true, true, false));
            objRole.Add(SetPermission("ExportPermission", "Export Permission", true, true, true, true, false));

            /*ImportPermission,IssuePermission, KittingPermission,MaintenancePermission, QuickListPermission*/
            objRole.Add(SetPermission("ImportPermission", "Import Permission", true, true, true, true, false));
            objRole.Add(SetPermission("IssuePermission", "Issue Permission", true, true, true, true, false));
            objRole.Add(SetPermission("KittingPermission", "Kitting Permission", true, true, true, true, false));
            objRole.Add(SetPermission("MaintenancePermission", "Maintenance Permission", true, true, true, true, false));
            objRole.Add(SetPermission("QuickListPermission", "QuickList Permission", true, true, true, true, false));

            /*  Suggested OrderPermission, Suggested TransferPermission, OrderPermission,ReceivingPermission,  ReportingPermission, RequisitionPermission */
            objRole.Add(SetPermission("SuggestedOrderPermission", "Suggested Order Permission", true, true, true, true, false));
            objRole.Add(SetPermission("SuggestedTransferPermission", "Suggested Transfer Permission", true, true, true, true, false));
            objRole.Add(SetPermission("OrderPermission", "Order Permission", true, true, true, true, false));
            objRole.Add(SetPermission("ReceivingPermission", "Receiving Permission", true, true, true, true, false));
            objRole.Add(SetPermission("ReportingPermission", "Reporting Permission", true, true, true, true, false));
            objRole.Add(SetPermission("RequisitionPermission", "Requisition Permission", true, true, true, true, false));

            /*   StagingPermission,        SupportingDataPermission,        TransferPermission,        WorkOrderPermission, RoomAccessInformation */
            objRole.Add(SetPermission("StagingPermission", "Staging Permission", true, true, true, true, false));
            objRole.Add(SetPermission("SupportingDataPermission", "Supporting Data Permission", true, true, true, true, false));
            objRole.Add(SetPermission("TransferPermission", "Transfer Permission", true, true, true, true, false));
            objRole.Add(SetPermission("WorkOrderPermission", "Work Order Permission", true, true, true, true, false));

            #endregion

            #region "Set default permissions "
            objMasterRole = new List<SetPermissions>();

            /*Item_View,Job_View,Checkout_View,Purchase_View,Tool_View,Project_View,Unit_View,Schedule_Unit_View,Customer_View,Hide_Cost,
             Enable_Item_Rename,Supplier_Filter*/
            objMasterRole.Add(SetPermission("Item_View", "Item View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Job_View", "Job View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Checkout_View", "Checkout View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Purchase_View", "Purchase View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Tool_View", "Tool View", true, true, true, true, false));

            objMasterRole.Add(SetPermission("Project_View", "Project View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Unit_View", "Unit View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Schedule_Unit_View", "Schedule Unit View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Customer_View", "Customer View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Hide_Cost", "Hide View", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Enable_Item_Rename", "Enable Item Rename", true, true, true, true, false));
            objMasterRole.Add(SetPermission("Supplier_Filter", "Supplier Filter", true, true, true, true, false));
            #endregion
            objRole3.MasterPermissions = objMasterRole;
            objRole3.RoomAccess = null;
            objRole3.Permissions = objRole;
            objRole3.ReplenishingRooms = null;
            objRole3.CheckPermissions = GetCheckedPermmisionsRecords(false);
            objRole3.PermissionsName = PermissionsNameList;
            obj.Add(objRole3);
            #endregion
            return obj;
        }

        #endregion
    }
}
