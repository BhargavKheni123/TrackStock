using eTurns.DTO;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace eTurnsWeb.Helper
{
    public class NotificationHelper
    {

        public bool OnlyAvailableToolsFilterCheck(string XMLValue)
        {

            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("OnlyAvailableTools").FirstOrDefault() != null && xDoc.Descendants("OnlyAvailableTools").FirstOrDefault().Value == "Yes")
                return true;

            return false;
        }
        public string GetCountAppliedFilterValue(string XMLValue)
        {
            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("CountAppliedFilter").FirstOrDefault() != null)
                return xDoc.Descendants("CountAppliedFilter").FirstOrDefault().Value;

            return "All";

        }

        public string GetUsageTypeFilterValue(string XMLValue)
        {
            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("UsageType").FirstOrDefault() != null)
                return xDoc.Descendants("UsageType").FirstOrDefault().Value;

            return "Consolidate";

        }

        public bool GetIsAllowedZeroPullUsageValue(string XMLValue)
        {

            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("IsAllowedZeroPullUsage").FirstOrDefault() != null && xDoc.Descendants("IsAllowedZeroPullUsage").FirstOrDefault().Value == "Yes")
                return true;

            return false;
        }

        public long GetMaximumAvgUseFilterValue(string XMLValue)
        {
            long MaximumAvguse = 30;
            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("MonthlyAverageUsage").FirstOrDefault() != null)
            {

                long.TryParse(xDoc.Descendants("MonthlyAverageUsage").FirstOrDefault().Value, out MaximumAvguse);

            }
            return MaximumAvguse;


        }


        //
        public bool GetOnlyExpirationItemsFilterValue(string XMLValue)
        {
            bool isExpItems = false;
            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("OnlyExpirationItems").FirstOrDefault() != null)
            {
                if ((Convert.ToString(xDoc.Descendants("OnlyExpirationItems").FirstOrDefault().Value) ?? string.Empty).ToLower() == "yes")
                {
                    isExpItems = true;
                }


            }
            return isExpItems;


        }
        public bool GetExcludeZeroOrdQtyFilterValue(string XMLValue)
        {
            bool isExcludeZeroOrdQty = false;
            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("ExcludeZeroOrdQty").FirstOrDefault() != null)
            {
                if ((Convert.ToString(xDoc.Descendants("ExcludeZeroOrdQty").FirstOrDefault().Value) ?? string.Empty).ToLower() == "yes")
                {
                    isExcludeZeroOrdQty = true;
                }
            }
            return isExcludeZeroOrdQty;
        }
        public bool GetAllcheckedouttoolsFilterValue(string XMLValue)
        {
            bool AllCheckedOutTools = false;
            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("AllCheckedOutTools").FirstOrDefault() != null)
            {
                if ((Convert.ToString(xDoc.Descendants("AllCheckedOutTools").FirstOrDefault().Value) ?? string.Empty).ToLower() == "true")
                {
                    AllCheckedOutTools = true;
                }
            }
            return AllCheckedOutTools;
        }
        public bool GetIsIncludeStockouttoolFilterValue(string XMLValue)
        {
            bool IsIncludeStockouttool = false;
            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("IsIncludeStockouttool").FirstOrDefault() != null)
            {
                if ((Convert.ToString(xDoc.Descendants("IsIncludeStockouttool").FirstOrDefault().Value) ?? string.Empty).ToLower() == "yes")
                {
                    IsIncludeStockouttool = true;
                }


            }
            return IsIncludeStockouttool;
        }


        public string GetReportRangeFilterValue(string XMLValue)
        {
            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("ReportRange").FirstOrDefault() != null)
                return xDoc.Descendants("ReportRange").FirstOrDefault().Value;

            return "";

        }

        public bool GetIsSelectAllRangeDataFilterValue(string XMLValue)
        {
            XDocument xDoc = XDocument.Parse("<Data>" + XMLValue + "</Data>");
            if (xDoc.Descendants("IsSelectAllRangeData").FirstOrDefault() != null)
                return Convert.ToBoolean(xDoc.Descendants("IsSelectAllRangeData").FirstOrDefault().Value);

            return false;

        }

        public void ReqAndWOStatusFilter(string XMLValue, out string RequisitionStatus, out string WOStatus, out string OrderStatus)
        {

            RequisitionStatus = string.Empty;
            WOStatus = string.Empty;
            OrderStatus = string.Empty;
            if (XMLValue.ToLower().IndexOf("status") >= 0)
            {

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
                XmlNodeList nodeList = xmldoc.SelectNodes("/Data/Status");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        for (int i = 1; i <= node.ChildNodes.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(node["Status" + i].InnerText))
                            {
                                if (!string.IsNullOrWhiteSpace(RequisitionStatus))
                                {
                                    RequisitionStatus += "," + node["Status" + i].InnerText;
                                }
                                else
                                {
                                    RequisitionStatus += node["Status" + i].InnerText;
                                }
                            }
                        }
                    }
                }
                nodeList = xmldoc.SelectNodes("/Data/WOStatus");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        for (int i = 1; i <= node.ChildNodes.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(node["WOStatus" + i].InnerText))
                            {
                                if (!string.IsNullOrWhiteSpace(WOStatus))
                                {
                                    WOStatus += "," + node["WOStatus" + i].InnerText;
                                }
                                else
                                {
                                    WOStatus += node["WOStatus" + i].InnerText;
                                }
                            }
                        }
                    }
                }

                nodeList = xmldoc.SelectNodes("/Data/OrderStatus");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        for (int i = 1; i <= node.ChildNodes.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(node["OrderStatus" + i].InnerText))
                            {
                                if (!string.IsNullOrWhiteSpace(OrderStatus))
                                {
                                    OrderStatus += "," + node["OrderStatus" + i].InnerText;
                                }
                                else
                                {
                                    OrderStatus += node["OrderStatus" + i].InnerText;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CheckWOStatusFilter(string XMLValue, out string WOStatus, out string RequisitionStatus, out string OrderStatus)
        {
            WOStatus = string.Empty;
            RequisitionStatus = string.Empty;
            OrderStatus = string.Empty;
            if (XMLValue.ToLower().IndexOf("wostatus") >= 0)
            {

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
                XmlNodeList nodeList = xmldoc.SelectNodes("/Data/WOStatus");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        for (int i = 1; i <= node.ChildNodes.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(node["WOStatus" + i].InnerText))
                            {
                                if (!string.IsNullOrWhiteSpace(WOStatus))
                                {
                                    WOStatus += "," + node["WOStatus" + i].InnerText;
                                }
                                else
                                {
                                    WOStatus += node["WOStatus" + i].InnerText;
                                }
                            }
                        }

                    }

                }
            }
            if (XMLValue.ToLower().IndexOf("orderstatus") >= 0)
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
                XmlNodeList nodeList = xmldoc.SelectNodes("/Data/OrderStatus");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        for (int i = 1; i <= node.ChildNodes.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(node["OrderStatus" + i].InnerText))
                            {
                                if (!string.IsNullOrWhiteSpace(OrderStatus))
                                {
                                    OrderStatus += "," + node["OrderStatus" + i].InnerText;
                                }
                                else
                                {
                                    OrderStatus += node["OrderStatus" + i].InnerText;
                                }
                            }
                        }

                    }

                }
            }
        }
        public void FilterExpiredItems(string XMLValue, out string OnlyExpiredItems, out string DaysUntilItemExpires, out string DaysToApproveOrder, out string ProjectExpirationDate)
        {
            OnlyExpiredItems = string.Empty;
            DaysUntilItemExpires = string.Empty;
            DaysToApproveOrder = string.Empty;
            ProjectExpirationDate = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/OnlyExpiredItems");
            if (nodeList != null && nodeList.Count > 0)
            {
                OnlyExpiredItems = nodeList[0].InnerText;
            }
            nodeList = xmldoc.SelectNodes("/Data/DaysUntilItemExpires");
            if (nodeList != null && nodeList.Count > 0)
            {
                DaysUntilItemExpires = nodeList[0].InnerText;
            }
            nodeList = xmldoc.SelectNodes("/Data/DaysToApproveOrder");
            if (nodeList != null && nodeList.Count > 0)
            {
                DaysToApproveOrder = nodeList[0].InnerText;
            }
            nodeList = xmldoc.SelectNodes("/Data/ProjectExpirationDate");
            if (nodeList != null && nodeList.Count > 0)
            {
                ProjectExpirationDate = nodeList[0].InnerText;
            }

        }
        public string FilterQtyType(string XMLValue)
        {
            string QtyType = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/QuantityType");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["Type" + i].InnerText))
                        {
                            if (!string.IsNullOrWhiteSpace(QtyType))
                            {
                                QtyType += "," + node["Type" + i].InnerText;
                            }
                            else
                            {
                                QtyType += node["Type" + i].InnerText;
                            }
                        }
                    }
                }
            }
            return QtyType;
        }

        public string FilterItemStatus(string XMLValue)
        {
            string ItemStatus = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/ItemStatus");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["IStatus" + i].InnerText))
                        {
                            if (!string.IsNullOrWhiteSpace(ItemStatus))
                            {
                                ItemStatus += "," + node["IStatus" + i].InnerText;
                            }
                            else
                            {
                                ItemStatus += node["IStatus" + i].InnerText;
                            }
                        }
                    }
                }
            }
            return ItemStatus;
        }

        public string FilterCartType(string XMLValue)
        {
            string CartType = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/CartType");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["CType" + i].InnerText))
                        {
                            if (!string.IsNullOrWhiteSpace(CartType))
                            {
                                CartType += "," + node["CType" + i].InnerText;
                            }
                            else
                            {
                                CartType += node["CType" + i].InnerText;
                            }
                        }
                    }
                }
            }
            return CartType;
        }
        public string FilterMoveType(string XMLValue)
        {
            string MoveType = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/MoveType");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["CType" + i].InnerText))
                        {
                            if (!string.IsNullOrWhiteSpace(MoveType))
                            {
                                MoveType += "," + node["CType" + i].InnerText;
                            }
                            else
                            {
                                MoveType += node["CType" + i].InnerText;
                            }
                        }
                    }
                }
            }
            return MoveType;
        }
        public string FilterActionCodes(string XMLValue)
        {
            string ActionCodes = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/ActionCodes");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["Code" + i].InnerText))
                        {
                            if (!string.IsNullOrWhiteSpace(ActionCodes))
                            {
                                ActionCodes += "," + node["Code" + i].InnerText;
                            }
                            else
                            {
                                ActionCodes += node["Code" + i].InnerText;
                            }
                        }
                    }
                }
            }
            return ActionCodes;
        }

        public string FilterInStockQOH(string XMLValue)
        {
            string FilterQOH = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/FilterQOH");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["FQOH" + i].InnerText))
                        {
                            if (!string.IsNullOrWhiteSpace(FilterQOH))
                            {
                                FilterQOH += "," + node["FQOH" + i].InnerText;
                            }
                            else
                            {
                                FilterQOH += node["FQOH" + i].InnerText;
                            }
                        }
                    }
                }
            }
            return FilterQOH;
        }

        //
        public string FilterInStockReportRangeData(string XMLValue)
        {
            string ReportRangeData = string.Empty;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<Data>" + XMLValue + "</Data>");
            XmlNodeList nodeList = xmldoc.SelectNodes("/Data/RangeData");
            if (nodeList != null && nodeList.Count > 0)
            {
                foreach (XmlNode node in nodeList)
                {
                    for (int i = 1; i <= node.ChildNodes.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(node["DataId" + i].InnerText))
                        {
                            if (!string.IsNullOrWhiteSpace(ReportRangeData))
                            {
                                ReportRangeData += "," + node["DataId" + i].InnerText;
                            }
                            else
                            {
                                ReportRangeData += node["DataId" + i].InnerText;
                            }
                        }
                    }
                }
            }
            return ReportRangeData;
        }

        public void SetReportParamsNotification(ref NotificationDTO objNotificationDTO)
        {
            if (objNotificationDTO != null && (objNotificationDTO.XMLValue) != null && (!string.IsNullOrWhiteSpace(objNotificationDTO.XMLValue)))
            {
                string RequisitionStatus = string.Empty;
                string WOStatus = string.Empty;
                string OrderStatus = string.Empty;
                string QtyType = string.Empty;
                string CartType = string.Empty;
                string OnlyExpiredItemsStr = string.Empty;
                string DaysUntilItemExpires = string.Empty;
                string DaysToApproveOrder = string.Empty;
                string ProjectExpirationDate = string.Empty;
                string MoveType = string.Empty;


                if ((objNotificationDTO.ReportMasterDTO != null && !string.IsNullOrEmpty(objNotificationDTO.ReportMasterDTO.ModuleName)
                   && objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "tool")
                  || (objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null
                   && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "tool"))
                   && objNotificationDTO.XMLValue.ToLower().IndexOf("onlyavailabletools") >= 0
                   )
                {
                    objNotificationDTO.OnlyAvailableTools = OnlyAvailableToolsFilterCheck(objNotificationDTO.XMLValue);
                }

                if ((objNotificationDTO.ReportMasterDTO != null && !string.IsNullOrEmpty(objNotificationDTO.ReportMasterDTO.ModuleName)
                 && objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "countmaster")
                 && objNotificationDTO.XMLValue.ToLower().IndexOf("countappliedfilter") >= 0)
                {
                    objNotificationDTO.CountAppliedFilter = GetCountAppliedFilterValue(objNotificationDTO.XMLValue);
                }

                if ((objNotificationDTO != null && objNotificationDTO.ReportName != null && (objNotificationDTO.ReportName.Trim().ToLower() == "requisition" || objNotificationDTO.ReportName.Trim().ToLower() == "requisition with lineitems"))
               || (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "consume_requisition" || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "range-consume_requisition"))
               || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "consume_requisition" || objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "range-consume_requisition")))
                {

                    ReqAndWOStatusFilter(objNotificationDTO.XMLValue, out RequisitionStatus, out WOStatus, out OrderStatus);
                    objNotificationDTO.Status = RequisitionStatus;
                    objNotificationDTO.WOStatus = WOStatus;
                    objNotificationDTO.OrderStatus = OrderStatus;
                }
                if ((objNotificationDTO != null && objNotificationDTO.ReportName != null && objNotificationDTO.ReportName.Trim().ToLower() == "work order") ||
              (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "workorder")
              || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "workorder"))

                {
                    CheckWOStatusFilter(objNotificationDTO.XMLValue, out WOStatus, out RequisitionStatus, out OrderStatus);
                    objNotificationDTO.Status = RequisitionStatus;
                    objNotificationDTO.WOStatus = WOStatus;
                    objNotificationDTO.OrderStatus = OrderStatus;
                }
                if ((objNotificationDTO != null && objNotificationDTO.ReportName != null && (objNotificationDTO.ReportName.Trim().ToLower() == "order" || objNotificationDTO.ReportName.Trim().ToLower() == "replenish_order")) ||
                    (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "order" || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "replenish_order"))
                    || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "order" || objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "replenish_order")))
                {
                    CheckWOStatusFilter(objNotificationDTO.XMLValue, out WOStatus, out RequisitionStatus, out OrderStatus);
                    objNotificationDTO.Status = RequisitionStatus;
                    objNotificationDTO.WOStatus = WOStatus;
                    objNotificationDTO.OrderStatus = OrderStatus;
                }

                if ((objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && objNotificationDTO.ReportMasterDTO.ModuleName != null && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "expiringitems"))
                   || (objNotificationDTO != null && objNotificationDTO.AttachedReportMasterDTO != null && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null && (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "expiringitems")))
                {
                    FilterExpiredItems(objNotificationDTO.XMLValue, out OnlyExpiredItemsStr, out DaysUntilItemExpires, out DaysToApproveOrder, out ProjectExpirationDate);
                    objNotificationDTO.OnlyExpiredItems = OnlyExpiredItemsStr == "Yes" ? true : false;
                    objNotificationDTO.DaysUntilItemExpires = DaysUntilItemExpires;
                    objNotificationDTO.DaysToApproveOrder = DaysToApproveOrder;
                    if (!string.IsNullOrWhiteSpace(ProjectExpirationDate))
                    {
                        objNotificationDTO.ProjectExpirationDate = FnCommon.ConvertDateByTimeZone(Convert.ToDateTime(ProjectExpirationDate), true, true);// ProjectExpirationDate.ToString(SessionHelper.RoomDateFormat);
                    }

                }
                if (objNotificationDTO.XMLValue.ToLower().IndexOf("quantitytype") >= 0
                    && (objNotificationDTO != null
                        && objNotificationDTO.ReportName != null
                        && (objNotificationDTO.ReportName.Trim().ToLower() == "pull"
                            || objNotificationDTO.ReportName.Trim().ToLower() == "pull summary"
                            || objNotificationDTO.ReportName.Trim().ToLower() == "pull summary by quarter"
                            || objNotificationDTO.ReportName.Trim().ToLower() == "not pulled report"
                            || objNotificationDTO.ReportName.Trim().ToLower() == "work order")
                     )
                    || (objNotificationDTO != null
                            && objNotificationDTO.ReportMasterDTO != null
                            && objNotificationDTO.ReportMasterDTO.ModuleName != null
                            && (objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "consume_pull"
                                    || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "not consume_pull"
                                    || objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "workorder")
                      )

                     || (objNotificationDTO != null
                            && objNotificationDTO.AttachedReportMasterDTO != null
                            && objNotificationDTO.AttachedReportMasterDTO.ModuleName != null
                            && (
                                (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "consume_pull")
                                || (objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "not consume_pull")
                                || objNotificationDTO.AttachedReportMasterDTO.ModuleName.Trim().ToLower() == "workorder")
                         )
                      )

                {
                    objNotificationDTO.QtyType = FilterQtyType(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("usagetype") >= 0 && (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && (objNotificationDTO.ReportMasterDTO.ParentReportName.Trim().ToLower() == "pull summary" || objNotificationDTO.ReportMasterDTO.ParentReportName.Trim().ToLower() == "pull summary by quarter")))
                {
                    objNotificationDTO.UsageType = GetUsageTypeFilterValue(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("isallowedzeropullusage") >= 0 && (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && (objNotificationDTO.ReportMasterDTO.ParentReportName.Trim().ToLower() == "pull summary by quarter")))
                {
                    objNotificationDTO.IsAllowedZeroPullUsage = GetIsAllowedZeroPullUsageValue(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO != null && objNotificationDTO.XMLValue.ToLower().IndexOf("actioncodes") >= 0)
                {
                    objNotificationDTO.ActionCodes = FilterActionCodes(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO != null && objNotificationDTO.XMLValue.ToLower().IndexOf("filterqoh") >= 0)
                {
                    objNotificationDTO.FilterQOH = FilterInStockQOH(objNotificationDTO.XMLValue);
                }


                if (objNotificationDTO.XMLValue.ToLower().IndexOf("monthlyaverageusage") >= 0)
                {
                    objNotificationDTO.MonthlyAverageUsage = GetMaximumAvgUseFilterValue(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("onlyexpirationitems") >= 0)
                {
                    objNotificationDTO.OnlyExpirationItems = GetOnlyExpirationItemsFilterValue(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("itemstatus") >= 0)
                {
                    objNotificationDTO.ItemStatus = FilterItemStatus(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("reportrange") >= 0)
                {
                    objNotificationDTO.ReportRange = GetReportRangeFilterValue(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("isselectallrangedata") >= 0)
                {
                    objNotificationDTO.SelectAllRangeData = GetIsSelectAllRangeDataFilterValue(objNotificationDTO.XMLValue);
                }


                if (objNotificationDTO != null && objNotificationDTO.XMLValue.ToLower().IndexOf("rangedata") >= 0)
                {
                    objNotificationDTO.ReportRangeData = FilterInStockReportRangeData(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("carttype") >= 0)
                {
                    objNotificationDTO.CartType = FilterCartType(objNotificationDTO.XMLValue);
                }
                if (objNotificationDTO.XMLValue.ToLower().IndexOf("movetype") >= 0)
                {
                    objNotificationDTO.MoveType = FilterMoveType(objNotificationDTO.XMLValue);
                }
                if ((objNotificationDTO.ReportMasterDTO != null && !string.IsNullOrEmpty(objNotificationDTO.ReportMasterDTO.ModuleName)
                    && objNotificationDTO.ReportMasterDTO.ModuleName.Trim().ToLower() == "ToolInStock".ToLower())
                    && objNotificationDTO.XMLValue.ToLower().IndexOf("isincludestockouttool") >= 0)
                {
                    objNotificationDTO.IsIncludeStockouttool = GetIsIncludeStockouttoolFilterValue(objNotificationDTO.XMLValue);
                }

                if (objNotificationDTO.XMLValue.ToLower().IndexOf("excludezeroordqty") >= 0)
                {
                    objNotificationDTO.ExcludeZeroOrdQty = GetExcludeZeroOrdQtyFilterValue(objNotificationDTO.XMLValue);
                }
                if (objNotificationDTO.XMLValue.ToLower().IndexOf("allcheckedouttools") >= 0 && (objNotificationDTO != null && objNotificationDTO.ReportMasterDTO != null && (objNotificationDTO.ReportMasterDTO.ParentReportName.Trim().ToLower() == "tools checked out")))
                {
                    objNotificationDTO.AllCheckedOutTools = GetAllcheckedouttoolsFilterValue(objNotificationDTO.XMLValue);
                }
            }

        }


        //public List<NotificationDTO> GetCurrentNotificationListByEventName(string eventName, long RoomID, long CompanyID, long UserID)
        //{
        //    eTurns.DAL.NotificationDAL objDAL = new eTurns.DAL.NotificationDAL(SessionHelper.EnterPriseDBName);
        //    List<NotificationDTO> lst = objDAL.GetCurrentNotificationListByEventName("OPC", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
        //    return lst;
        //}

        //public void SendMailForImmediatePull(List<NotificationDTO> lst)
        //{
        //    eTurns.DAL.NotificationDAL objDAL = new eTurns.DAL.NotificationDAL(SessionHelper.EnterPriseDBName);
        //    objDAL.SendMailForImmediate(lst,SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID);
        //}


    }
}