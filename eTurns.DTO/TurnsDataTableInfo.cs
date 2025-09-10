using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace eTurns.DTO
{
    public class TurnsDataTableInfo
    {
        public double YearMonths = 12;
        private double _ItemInventoryValue;
        private double _OnHandQuantity;
        private double _CriticalQuantity;
        private double _MinimumQuantity;
        private double _MaximumQuantity;
        private double _PullQuantity;
        private double _PullCost;
        private double _ApprovedQuantity;
        private double _RequestedQuantity;
        private double _ReceivedQuantity;
        private double _OrderedQuantity;
        private double _retApprovedQuantity;
        private double _retRequestedQuantity;
        private double _retReceivedQuantity;
        private double _AverageCost;
        private double _AvgDailyPullValueUsage;
        private double _AvgDailyPullUsage;
        private double _AvgDailyOrderUsage;
        private double _PullValueTurn;
        private double _PullTurn;
        private double _OrderTurn;
        //private double _CalulatedMinimum;
        //private double _CalulatedMaximum;
        private int _TurnsMonthsOfUsageToSample;
        private int _TurnsDaysOfUsageToSample;
        private bool? _IsItemLevelMinMaxQtyRequired;
        private string _ItemLocations;

        public long ID { get; set; }
        public Guid GUID { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string SupplierName { get; set; }

        public string ManufacturerNumber { get; set; }

        public string Manufacturer { get; set; }

        public string SupplierPartNo { get; set; }

        public string BinNumber { get; set; }
        public double AverageCost { get { return _AverageCost; } set { _AverageCost = value; } }
        public double OnHandQuantity { get { return _OnHandQuantity; } set { _OnHandQuantity = value; } }
        public double PullQuantity { get { return _PullQuantity; } set { _PullQuantity = value; } }
        public double PullCost { get { return _PullCost; } set { _PullCost = value; } }
        public double ApprovedQuantity { get { return _ApprovedQuantity; } set { _ApprovedQuantity = value; } }
        public double RequestedQuantity { get { return _RequestedQuantity; } set { _RequestedQuantity = value; _OrderedQuantity = value; } }
        public double ReceivedQuantity { get { return _ReceivedQuantity; } set { _ReceivedQuantity = value; } }
        public double OrderedQuantity { get { return _OrderedQuantity; } }
        public double retApprovedQuantity { get { return _retApprovedQuantity; } set { _retApprovedQuantity = value; } }
        public double retRequestedQuantity { get { return _retRequestedQuantity; } set { _retRequestedQuantity = value; } }
        public double retReceivedQuantity { get { return _retReceivedQuantity; } set { _retReceivedQuantity = value; } }
        public double ItemInventoryValue { get { return _ItemInventoryValue; } set { _ItemInventoryValue = value; } }
        public bool? IsItemLevelMinMaxQtyRequired { get { return _IsItemLevelMinMaxQtyRequired; } set { _IsItemLevelMinMaxQtyRequired = value; } }
        public byte TurnsMeasureMethod { get; set; }
        public int TurnsMonthsOfUsageToSample { get { return _TurnsMonthsOfUsageToSample; } set { _TurnsMonthsOfUsageToSample = value; } }
        public int TurnsDaysOfUsageToSample { get { return _TurnsDaysOfUsageToSample; } set { _TurnsDaysOfUsageToSample = value; } }
        public DateTime QuantumFromDate { get; set; }
        public DateTime QuantumToDate { get; set; }
        public string ItemLocations { get { return _ItemLocations; } set { _ItemLocations = value; } }



        public double MinimumQuantity
        {
            get
            {
                return _MinimumQuantity;
            }
            set
            {
                if (_IsItemLevelMinMaxQtyRequired == false)
                {
                    if (lstItemLocations.Any())
                    {
                        _MinimumQuantity = lstItemLocations.First().MinimumQuantity ?? 0;
                    }
                    else
                    {
                        _MinimumQuantity = 0;
                    }
                }
                else
                {
                    _MinimumQuantity = value;
                }
            }
        }
        public double MaximumQuantity
        {
            get
            {
                return _MaximumQuantity;
            }
            set
            {
                if (_IsItemLevelMinMaxQtyRequired == false)
                {
                    if (lstItemLocations.Any())
                    {
                        _MaximumQuantity = lstItemLocations.First().MaximumQuantity ?? 0;
                    }
                    else
                    {
                        _MaximumQuantity = 0;
                    }
                }
                else
                {
                    _MaximumQuantity = value;
                }
            }
        }
        public double CriticalQuantity
        {
            get
            {
                return _CriticalQuantity;
            }
            set
            {
                if (_IsItemLevelMinMaxQtyRequired == false)
                {
                    if (lstItemLocations.Any())
                    {
                        _CriticalQuantity = lstItemLocations.First().CriticalQuantity ?? 0;
                    }
                    else
                    {
                        _CriticalQuantity = 0;
                    }
                }
                else
                {
                    _CriticalQuantity = value;
                }
            }
        }
        public List<BinMasterDTO> lstItemLocations
        {
            get
            {
                List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                if (!string.IsNullOrWhiteSpace(_ItemLocations))
                {
                    XElement ilXElement = XElement.Parse(_ItemLocations);
                    if (ilXElement.HasElements)
                    {

                        ilXElement.Elements("ItemLocation").ToList().ForEach(t =>
                        {
                            Guid _itmguid = Guid.Empty;
                            double _minQty = 0;
                            double _maxQty = 0;
                            double _critQty = 0;
                            int _isDefault = 0;

                            BinMasterDTO objil = new BinMasterDTO();
                            if (Guid.TryParse(Convert.ToString(t.Element("ItemGUID").Value), out _itmguid))
                            {
                                objil.ItemGUID = _itmguid;
                            }
                            objil.BinNumber = t.Element("BinNumber").Value;
                            if (double.TryParse(Convert.ToString(t.Element("MinimumQuantity").Value), out _minQty))
                            {
                                objil.MinimumQuantity = _minQty;
                            }
                            if (double.TryParse(Convert.ToString(t.Element("MaximumQuantity").Value), out _maxQty))
                            {
                                objil.MaximumQuantity = _maxQty;
                            }
                            if (double.TryParse(Convert.ToString(t.Element("CriticalQuantity").Value), out _critQty))
                            {
                                objil.CriticalQuantity = _critQty;
                            }
                            if (t.Elements("IsDefault").Any())
                            {
                                if (int.TryParse(Convert.ToString(t.Element("IsDefault").Value), out _isDefault))
                                {
                                    if (_isDefault == 1)
                                    {
                                        objil.IsDefault = true;
                                    }
                                    else
                                    {
                                        objil.IsDefault = false;
                                    }

                                }
                            }
                            lstBins.Add(objil);
                        });


                    }
                    return lstBins.OrderBy(t => t.BinNumber).ToList();
                }
                else
                {
                    return lstBins;
                }
            }
        }

        public double AvgDailyPullValueUsage
        {
            get
            {
                if (_TurnsMonthsOfUsageToSample > 0)
                {
                    _AvgDailyPullValueUsage = (_PullCost / _TurnsMonthsOfUsageToSample);
                }
                else
                {
                    _AvgDailyPullValueUsage = 0;
                }
                return _AvgDailyPullValueUsage;
            }
            set
            {
                _AvgDailyPullValueUsage = value;
            }
        }
        public double AvgDailyPullUsage
        {
            get
            {
                if (_TurnsMonthsOfUsageToSample > 0)
                {
                    _AvgDailyPullUsage = (_PullQuantity / _TurnsMonthsOfUsageToSample);
                }
                else
                {
                    _AvgDailyPullUsage = 0;
                }
                return _AvgDailyPullUsage;
            }
            set
            {
                _AvgDailyPullUsage = value;
            }
        }
        public double AvgDailyOrderUsage
        {
            get
            {
                if (_TurnsMonthsOfUsageToSample > 0)
                {
                    _AvgDailyOrderUsage = (_RequestedQuantity / _TurnsMonthsOfUsageToSample);
                }
                else
                {
                    _AvgDailyOrderUsage = 0;
                }
                return _AvgDailyOrderUsage;
            }
            set
            {
                _AvgDailyOrderUsage = value;
            }
        }
        public double PullValueTurn
        {
            get
            {
                if (_ItemInventoryValue > 0)
                {
                    _PullValueTurn = (_PullCost / _ItemInventoryValue) * (YearMonths / _TurnsMonthsOfUsageToSample);
                }
                else
                {
                    _PullValueTurn = 0;
                }
                return _PullValueTurn;
            }
            set
            {
                _PullValueTurn = value;
            }
        }
        public double PullTurn
        {
            get
            {
                if (OnHandQuantity > 0)
                {
                    _PullTurn = (_PullQuantity / OnHandQuantity) * (YearMonths / _TurnsMonthsOfUsageToSample);
                }
                else
                {
                    _PullTurn = 0;
                }
                return _PullTurn;
            }
            set
            {
                _PullTurn = value;
            }
        }
        public double OrderTurn
        {
            get
            {
                if (((_MinimumQuantity + _MaximumQuantity) / 2) > 0)
                {
                    _OrderTurn = (_RequestedQuantity / ((_MinimumQuantity + _MaximumQuantity) / 2)) * (YearMonths / _TurnsMonthsOfUsageToSample);
                }
                else
                {
                    _OrderTurn = 0;
                }
                return _OrderTurn;
            }
            set
            {
                _OrderTurn = value;
            }
        }

        public string InventoryClassification { get; set; }

        //public int TotalRecords { get; set; }
    }
}
