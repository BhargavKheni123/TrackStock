using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace eTurns.DTO
{
    [Serializable]
    public class MinMaxDataTableInfo
    {
        private Int64 DateofOrderMaxDays = 73000;

        private int _decimalPointFromConfig;
        public int decimalPointFromConfig
        {
            get
            {
                _decimalPointFromConfig = 0;

                //System.Xml.Linq.XElement Settinfile = null;
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                {
                    //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                }
                else
                {
                    //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                }

                _decimalPointFromConfig = SiteSettingHelper.decimalPointFromConfig != string.Empty ? Convert.ToInt32(SiteSettingHelper.decimalPointFromConfig) : 0;

                return _decimalPointFromConfig;
            }
            set { _decimalPointFromConfig = value; }
        }


        private int _DisplayInventoryValueforReplenish;
        public int DisplayInventoryValueforReplenish
        {
            get
            {
                _DisplayInventoryValueforReplenish = 0;

                //System.Xml.Linq.XElement Settinfile = null;
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
                {
                    //Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
                }
                else
                {
                    //Settinfile = System.Xml.Linq.XElement.Load(Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteSettingspath"]));
                }

                _DisplayInventoryValueforReplenish = SiteSettingHelper.DisplayInventoryValueforReplenish != string.Empty ? Convert.ToInt32(SiteSettingHelper.DisplayInventoryValueforReplenish) : 0;

                return _DisplayInventoryValueforReplenish;
            }
            set { _DisplayInventoryValueforReplenish = value; }
        }

        public double YearDays = 365;

        private double _ItemInventoryValue;
        private double _ItemExtendedCostValue;
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
        private int _DayOfUsageToSample;
        private int _MinMaxDayOfAverage;
        private int _MinMaxMeasureMethod;
        private double _MinMaxMinNumberOfTimesMax;
        private double _AvgDailyPullValueUsage;
        private double _AvgDailyPullUsage;
        private double _AvgDailyOrderUsage;
        private double _PullValueTurn;
        private double _PullTurn;
        private double _OrderTurn;
        private double _CalulatedMinimum;
        private double _CalulatedMaximum;
        private int _MinMaxOptValue1;
        private int _MinMaxOptValue2;
        private double _AbsValDifCurrCalcMinimum;
        private double _AbsoluteMinPerCent;
        //private double _MinPerCentActual;
        private double _AbsDiffererntPercentForMin;
        private double _AbsValDifCurrCalcMax;
        private double _AbsoluteMAXPerCent;
        //private double _MAXPerCentActual;
        private double _AutoCurrentMin;
        private double _AutoCurrentMax;
        private double _AutoCurrentMinPercent;
        private double _AutoCurrentMaxPercent;
        private string _MinAnalysis;
        private string _MaxAnalysis;
        private string _ItemLocations;
        private bool? _IsItemLevelMinMaxQtyRequired;
        private double _AbsDiffererntPercentForMax;
        private double _AutoCalcInventoryValue;
        private double _CalcInventoryValue;
        private double _DemandPlanningQtyToOrder;
        private double _OptimizedInvValueUsesQOHofAvgCalcdMinMax;
        private double _OptimizedInvValueChange;
        private double _TrialCalcInvValueUsesQOHofAvgTrialMinMax;
        private double _TrialInvValueChange;
        private double _NoOfDaysUntilOrder;
        private string _DateofOrder;
        private DateTime _dtDateofOrder;
        private double _AverageOnHandQuantity;
        private double _AverageExtendedCost;
        private double _TransferQuantity;
        private double _TransferCost;
        public int NumberDecimalDigits { get; set; }
        public int TurnsAvgDecimalPoints { get; set; }

        public double Cost { get; set; }
        public int CostUOMValue { get; set; }
        public double DefaultReorderQuantity { get; set; }
        public string IsActive { get; set; }
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
        public long BinID { get; set; }
        public double AverageCost { get { return _AverageCost; } set { _AverageCost = value; } }
        public double OnHandQuantity { get { return _OnHandQuantity; } set { _OnHandQuantity = value; } }
        public double AverageOnHandQuantity { get { return _AverageOnHandQuantity; } set { _AverageOnHandQuantity = value; } }
        public double AverageExtendedCost { get { return _AverageExtendedCost; } set { _AverageExtendedCost = value; } }
        public double PullQuantity { get { return _PullQuantity; } set { _PullQuantity = value; } }
        public double PullCost
        {
            get
            {
                _PullCost = Math.Round(_PullCost, TurnsAvgDecimalPoints);
                return _PullCost;
            }
            set { _PullCost = value; }
        }
        public double ApprovedQuantity { get { return _ApprovedQuantity; } set { _ApprovedQuantity = value; } }
        public double RequestedQuantity { get { return _RequestedQuantity; } set { _RequestedQuantity = value; _OrderedQuantity = value; } }
        public double ReceivedQuantity { get { return _ReceivedQuantity; } set { _ReceivedQuantity = value; } }
        public double OrderedQuantity { get { return _OrderedQuantity; } }
        public double retApprovedQuantity { get { return _retApprovedQuantity; } set { _retApprovedQuantity = value; } }
        public double retRequestedQuantity { get { return _retRequestedQuantity; } set { _retRequestedQuantity = value; } }
        public double retReceivedQuantity { get { return _retReceivedQuantity; } set { _retReceivedQuantity = value; } }
        public double ItemInventoryValue { get { return _ItemInventoryValue; } set { _ItemInventoryValue = value; } }
        public double ItemExtendedCostValue { get { return _ItemExtendedCostValue; } set { _ItemExtendedCostValue = value; } }
        public int DayOfUsageToSample { get { return _DayOfUsageToSample; } set { _DayOfUsageToSample = value; } }
        public int MinMaxDayOfAverage { get { return _MinMaxDayOfAverage; } set { _MinMaxDayOfAverage = value; } }
        public int MinMaxMeasureMethod { get { return _MinMaxMeasureMethod; } set { _MinMaxMeasureMethod = value; } }
        public double MinMaxMinNumberOfTimesMax { get { return _MinMaxMinNumberOfTimesMax; } set { _MinMaxMinNumberOfTimesMax = value; } }
        public int MinMaxOptValue1 { get { return _MinMaxOptValue1; } set { _MinMaxOptValue1 = value; } }
        public int MinMaxOptValue2 { get { return _MinMaxOptValue2; } set { _MinMaxOptValue2 = value; } }
        public DateTime QuantumFromDate { get; set; }
        public DateTime QuantumToDate { get; set; }
        public string DateCreated { get; set; }
        public int MinSliderValue { get; set; }
        public int MaxSliderValue { get; set; }
        public bool? IsItemLevelMinMaxQtyRequired { get { return _IsItemLevelMinMaxQtyRequired; } set { _IsItemLevelMinMaxQtyRequired = value; } }
        public string ItemLocations { get { return _ItemLocations; } set { _ItemLocations = value; } }
        public byte ItemTrendingSetting { get; set; }
        public bool IsTrendingEnabledOnItem { get; set; }
        public bool IsTrendingEnabledOnRoom { get; set; }
        public string InventoryClassification { get; set; }
        public int LeadTimeInDays { get; set; }

        public byte TrendingSetting { get; set; }

        public int MinAnalysisOrderNumber
        {
            get
            {
                if (MinAnalysis == "Red")
                {
                    return 1;
                }
                else if (MinAnalysis == "Yellow")
                {
                    return 2;
                }
                else if (MinAnalysis == "Green")
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }

        }

        public int MaxAnalysisOrderNumber
        {
            get
            {
                if (MaxAnalysis == "Red")
                {
                    return 1;
                }
                else if (MaxAnalysis == "Yellow")
                {
                    return 2;
                }
                else if (MaxAnalysis == "Green")
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }

        }

        public double MinimumQuantity
        {
            get
            {
                return _MinimumQuantity;
            }
            set
            {
                //if (_IsItemLevelMinMaxQtyRequired == false)
                //{
                //    if (lstItemLocations.Any())
                //    {
                //        _MinimumQuantity = lstItemLocations.First().MinimumQuantity ?? 0;
                //    }
                //    else
                //    {
                //        _MinimumQuantity = 0;
                //    }
                //}
                //else
                //{
                //    _MinimumQuantity = value;
                //}

                _MinimumQuantity = value;
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
                //if (_IsItemLevelMinMaxQtyRequired == false)
                //{
                //    if (lstItemLocations.Any())
                //    {
                //        _MaximumQuantity = lstItemLocations.First().MaximumQuantity ?? 0;
                //    }
                //    else
                //    {
                //        _MaximumQuantity = 0;
                //    }
                //}
                //else
                //{
                //    _MaximumQuantity = value;
                //}

                _MaximumQuantity = value;
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
                //if (_IsItemLevelMinMaxQtyRequired == false)
                //{
                //    if (lstItemLocations.Any())
                //    {
                //        _CriticalQuantity = lstItemLocations.First().CriticalQuantity ?? 0;
                //    }
                //    else
                //    {
                //        _CriticalQuantity = 0;
                //    }
                //}
                //else
                //{
                //    _CriticalQuantity = value;
                //}

                _CriticalQuantity = value;
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
                if (_DayOfUsageToSample > 0)
                {
                    _AvgDailyPullValueUsage = ((_PullCost + _TransferCost) / _DayOfUsageToSample);
                }
                else
                {
                    _AvgDailyPullValueUsage = 0;
                }
                _AvgDailyPullValueUsage = Math.Round(_AvgDailyPullValueUsage, TurnsAvgDecimalPoints);
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
                if (_DayOfUsageToSample > 0)
                {
                    _AvgDailyPullUsage = ((_PullQuantity + _TransferQuantity) / _DayOfUsageToSample);
                }
                else
                {
                    _AvgDailyPullUsage = 0;
                }
                _AvgDailyPullUsage = Math.Round(_AvgDailyPullUsage, TurnsAvgDecimalPoints);
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
                if (_DayOfUsageToSample > 0)
                {
                    _AvgDailyOrderUsage = (_RequestedQuantity / _DayOfUsageToSample);
                }
                else
                {
                    _AvgDailyOrderUsage = 0;
                }
                _AvgDailyOrderUsage = Math.Round(_AvgDailyOrderUsage, TurnsAvgDecimalPoints);
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
                if (DisplayInventoryValueforReplenish == 1)
                {
                    if (_ItemInventoryValue > 0 && _DayOfUsageToSample > 0)
                    {
                        _PullValueTurn = ((_PullCost + _TransferCost ) / _ItemInventoryValue) * (YearDays / _DayOfUsageToSample);
                    }
                    else
                    {
                        _PullValueTurn = 0;
                    }
                }
                else
                {
                    if (_AverageExtendedCost > 0 && _DayOfUsageToSample > 0)
                    {
                        _PullValueTurn = ((_PullCost + _TransferCost) / _AverageExtendedCost) * (YearDays / _DayOfUsageToSample);
                    }
                    else
                    {
                        _PullValueTurn = 0;
                    }
                }
                _PullValueTurn = Math.Round(_PullValueTurn, TurnsAvgDecimalPoints);
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
                if (_AverageOnHandQuantity > 0 && DayOfUsageToSample > 0)
                {
                    _PullTurn = ((_PullQuantity + _TransferQuantity )/ _AverageOnHandQuantity) * (YearDays / DayOfUsageToSample);
                }
                else
                {
                    _PullTurn = 0;
                }
                _PullTurn = Math.Round(_PullTurn, TurnsAvgDecimalPoints);
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
                if (((_MinimumQuantity + _MaximumQuantity) / 2) > 0 && _DayOfUsageToSample > 0)
                {
                    _OrderTurn = (_RequestedQuantity / ((_MinimumQuantity + _MaximumQuantity) / 2)) * (YearDays / _DayOfUsageToSample);
                }
                else
                {
                    _OrderTurn = 0;
                }
                _OrderTurn = Math.Round(_OrderTurn, TurnsAvgDecimalPoints);
                return _OrderTurn;
            }
            set
            {
                _OrderTurn = value;
            }
        }

        public double CalulatedMinimum
        {
            get
            {
                if (_MinMaxMeasureMethod == 2)
                {
                    _CalulatedMinimum = AvgDailyPullUsage * _MinMaxDayOfAverage;
                }
                else if (_MinMaxMeasureMethod == 3)
                {
                    _CalulatedMinimum = AvgDailyOrderUsage * _MinMaxDayOfAverage;
                }
                else
                {
                    _CalulatedMinimum = 0;
                }
                if (IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) && _CalulatedMinimum > 0 && _CalulatedMinimum != DefaultReorderQuantity)
                {

                    if (_CalulatedMinimum < DefaultReorderQuantity)
                    {
                        _CalulatedMinimum = DefaultReorderQuantity;
                    }
                    else
                    {
                        var mod = (_CalulatedMinimum % DefaultReorderQuantity);
                        if (mod > 0)
                        {
                            _CalulatedMinimum = _CalulatedMinimum + (DefaultReorderQuantity - mod);
                        }
                    }
                }
                _CalulatedMinimum = Math.Round(_CalulatedMinimum, decimalPointFromConfig);
                return _CalulatedMinimum;
            }
            set
            {
                _CalulatedMinimum = value;
            }
        }

        public double CalulatedMaximum
        {
            get
            {
                _CalulatedMaximum = CalulatedMinimum * MinMaxMinNumberOfTimesMax;
                if (IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) && _CalulatedMaximum > 0 && _CalulatedMaximum != DefaultReorderQuantity)
                {

                    if (_CalulatedMaximum < DefaultReorderQuantity)
                    {
                        _CalulatedMaximum = DefaultReorderQuantity;
                    }
                    else
                    {
                        var mod = (_CalulatedMaximum % DefaultReorderQuantity);
                        if (mod > 0)
                        {
                            _CalulatedMaximum = _CalulatedMaximum + (DefaultReorderQuantity - mod);
                        }
                    }
                }
                _CalulatedMaximum = Math.Round(_CalulatedMaximum, decimalPointFromConfig);
                return _CalulatedMaximum;
            }
            set
            {
                _CalulatedMaximum = value;
            }
        }

        public double AbsValDifCurrCalcMinimum
        {
            get
            {
                _AbsValDifCurrCalcMinimum = CalulatedMinimum - MinimumQuantity;
                if (_AbsValDifCurrCalcMinimum < 0)
                {
                    _AbsValDifCurrCalcMinimum = _AbsValDifCurrCalcMinimum * (-1);
                }

                _AbsValDifCurrCalcMinimum = Math.Round(_AbsValDifCurrCalcMinimum, decimalPointFromConfig);
                return _AbsValDifCurrCalcMinimum;
            }
            set
            {
                _AbsValDifCurrCalcMinimum = value;
            }
        }

        public double AbsoluteMinPerCent
        {
            get
            {
                if (CalulatedMinimum > 0)
                {
                    _AbsoluteMinPerCent = (AbsValDifCurrCalcMinimum / CalulatedMinimum) * 100;
                }
                else
                {
                    _AbsoluteMinPerCent = 0;
                }
                //_MinPerCentActual = _AbsoluteMinPerCent;
                if (_AbsoluteMinPerCent < 0)
                {
                    _AbsoluteMinPerCent = _AbsoluteMinPerCent * (-1);
                }

                _AbsoluteMinPerCent = Math.Round(_AbsoluteMinPerCent, decimalPointFromConfig);
                return _AbsoluteMinPerCent;
            }
            set
            {
                _AbsoluteMinPerCent = value;
            }
        }


        //public double MinPerCentActual
        //{
        //    get { return _MinPerCentActual; }
        //    set { _MinPerCentActual = value; }
        //}

        public double AbsDiffererntPercentForMin
        {
            get
            {
                _AbsDiffererntPercentForMin = (_MinimumQuantity - _CalulatedMinimum) * ((float)MinSliderValue / 100);
                return _AbsDiffererntPercentForMin;
            }
            set { _AbsDiffererntPercentForMin = value; }
        }

        public double AbsDiffererntPercentForMax
        {
            get
            {
                _AbsDiffererntPercentForMax = (_MaximumQuantity - _CalulatedMaximum) * ((float)MaxSliderValue / 100);
                return _AbsDiffererntPercentForMax;
            }
            set { _AbsDiffererntPercentForMax = value; }
        }

        public double AbsValDifCurrCalcMax
        {
            get
            {
                _AbsValDifCurrCalcMax = CalulatedMaximum - MaximumQuantity;
                if (_AbsValDifCurrCalcMax < 0)
                {
                    _AbsValDifCurrCalcMax = _AbsValDifCurrCalcMax * (-1);
                }

                _AbsValDifCurrCalcMax = Math.Round(_AbsValDifCurrCalcMax, decimalPointFromConfig);
                return _AbsValDifCurrCalcMax;
            }
            set
            {
                _AbsValDifCurrCalcMax = value;
            }
        }

        public double AbsoluteMAXPerCent
        {
            get
            {
                if (CalulatedMaximum > 0)
                {
                    _AbsoluteMAXPerCent = (AbsValDifCurrCalcMax / CalulatedMaximum) * 100;
                }
                else
                {
                    _AbsoluteMAXPerCent = 0;
                }
                //_MAXPerCentActual = _AbsoluteMAXPerCent;
                if (_AbsoluteMAXPerCent < 0)
                {
                    _AbsoluteMAXPerCent = _AbsoluteMAXPerCent * (-1);
                }
                _AbsoluteMAXPerCent = Math.Round(_AbsoluteMAXPerCent, decimalPointFromConfig);
                return _AbsoluteMAXPerCent;
            }
            set
            {
                _AbsoluteMAXPerCent = value;
            }
        }

        //public double MAXPerCentActual
        //{
        //    get { return _MAXPerCentActual; }
        //    set { _MAXPerCentActual = value; }
        //}

        public double AutoCurrentMin
        {
            get
            {
                _AutoCurrentMin = _MinimumQuantity - AbsDiffererntPercentForMin;

                _AutoCurrentMin = Math.Round(_AutoCurrentMin, decimalPointFromConfig);
                return _AutoCurrentMin;
            }
            set
            {
                _AutoCurrentMin = value;
            }
        }

        public double AutoCurrentMax
        {
            get
            {
                _AutoCurrentMax = _MaximumQuantity - AbsDiffererntPercentForMax;

                _AutoCurrentMax = Math.Round(_AutoCurrentMax, decimalPointFromConfig);
                return _AutoCurrentMax;
            }
            set
            {
                _AutoCurrentMax = value;
            }
        }

        //public double AutoCurrentMinPercent
        //{
        //    get
        //    {
        //        _AutoCurrentMinPercent = 0;
        //        if (AutoCurrentMin > 0)
        //        {
        //            _AutoCurrentMinPercent = ((CalulatedMinimum - AutoCurrentMin) / (AutoCurrentMin)) * 100;
        //            if (_AutoCurrentMinPercent < 0)
        //            {
        //                _AutoCurrentMinPercent = _AutoCurrentMinPercent * (-1);
        //            }
        //        }
        //        return _AutoCurrentMinPercent;
        //    }
        //    set
        //    {
        //        _AutoCurrentMinPercent = value;
        //    }
        //}

        public double AutoCurrentMinPercent
        {
            get
            {
                _AutoCurrentMinPercent = 0;
                if (AutoCurrentMin > 0)
                {
                    _AutoCurrentMinPercent = ((CalulatedMinimum - AutoCurrentMin) / (AutoCurrentMin)) * 100;
                    if (_AutoCurrentMinPercent < 0)
                    {
                        _AutoCurrentMinPercent = _AutoCurrentMinPercent * (-1);
                    }
                }
                _AutoCurrentMinPercent = Math.Round(_AutoCurrentMinPercent, decimalPointFromConfig);
                return _AutoCurrentMinPercent;
            }
            set
            {
                _AutoCurrentMinPercent = value;
            }
        }

        public double AutoCurrentMaxPercent
        {
            get
            {
                _AutoCurrentMaxPercent = 0;
                if (AutoCurrentMax > 0)
                {
                    _AutoCurrentMaxPercent = ((CalulatedMaximum - AutoCurrentMax) / (AutoCurrentMax)) * 100;
                    if (_AutoCurrentMaxPercent < 0)
                    {
                        _AutoCurrentMaxPercent = _AutoCurrentMaxPercent * (-1);
                    }
                }
                _AutoCurrentMaxPercent = Math.Round(_AutoCurrentMaxPercent, decimalPointFromConfig);
                return _AutoCurrentMaxPercent;
            }
            set
            {
                _AutoCurrentMaxPercent = value;
            }
        }
        public double AutoCalcInventoryValue
        {
            get
            {
                _AutoCalcInventoryValue = ((AutoCurrentMin + AutoCurrentMax) / 2) * (Cost / CostUOMValue);
                return _AutoCalcInventoryValue;
            }
            set
            {
                _AutoCalcInventoryValue = value;
            }
        }

        public double CalcInventoryValue
        {
            get
            {
                _CalcInventoryValue = ((CalulatedMinimum + CalulatedMaximum) / 2) * (Cost / CostUOMValue);
                return _CalcInventoryValue;
            }
            set
            {
                _CalcInventoryValue = value;
            }
        }
        public string MinAnalysis
        {
            get
            {
                if (AutoCurrentMinPercent < _MinMaxOptValue1)
                {
                    _MinAnalysis = "Green";
                }
                else if (AutoCurrentMinPercent >= _MinMaxOptValue1 && AutoCurrentMinPercent <= _MinMaxOptValue2)
                {
                    _MinAnalysis = "Yellow";
                }
                else if (AutoCurrentMinPercent > _MinMaxOptValue2)
                {
                    _MinAnalysis = "Red";
                }
                else
                {
                    _MinAnalysis = "Pink";
                }
                return _MinAnalysis;
            }
            set
            {
                _MinAnalysis = value;
            }
        }

        public string MaxAnalysis
        {
            get
            {
                if (AutoCurrentMaxPercent < _MinMaxOptValue1)
                {
                    _MaxAnalysis = "Green";
                }
                else if (AutoCurrentMaxPercent >= _MinMaxOptValue1 && AutoCurrentMaxPercent <= _MinMaxOptValue2)
                {
                    _MaxAnalysis = "Yellow";
                }
                else if (AutoCurrentMaxPercent > _MinMaxOptValue2)
                {
                    _MaxAnalysis = "Red";
                }
                else
                {
                    _MaxAnalysis = "Pink";
                }
                return _MaxAnalysis;
            }
            set
            {
                _MaxAnalysis = value;
            }
        }

        public double QtyUntilOrder { get; set; }

        public double NoOfDaysUntilOrder
        {
            get
            {
                if (QtyUntilOrder > 0 && MinimumQuantity > 0)
                {
                    if (_MinMaxMeasureMethod == 2)
                    {
                        if (AvgDailyPullUsage > 0)
                        {
                            _NoOfDaysUntilOrder = (QtyUntilOrder / AvgDailyPullUsage);
                        }
                        else
                        {
                            _NoOfDaysUntilOrder = (QtyUntilOrder / 1);
                        }
                    }
                    else if (_MinMaxMeasureMethod == 3)
                    {
                        if (AvgDailyOrderUsage > 0)
                        {
                            _NoOfDaysUntilOrder = (QtyUntilOrder / AvgDailyOrderUsage);
                        }
                        else
                        {
                            _NoOfDaysUntilOrder = (QtyUntilOrder / 1);
                        }
                    }

                    _NoOfDaysUntilOrder = Math.Round(_NoOfDaysUntilOrder, NumberDecimalDigits);
                    return _NoOfDaysUntilOrder;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                _NoOfDaysUntilOrder = value;
            }
        }

        //public double NoOfDaysUntilOrder
        //{
        //    get
        //    {
        //        if (AvgDailyPullUsage > 0)
        //        {
        //            _NoOfDaysUntilOrder = (OnHandQuantity - (MinimumQuantity - 1)) / AvgDailyPullUsage;
        //        }
        //        else
        //        {
        //            _NoOfDaysUntilOrder = (OnHandQuantity - (MinimumQuantity - 1));
        //        }
        //        _NoOfDaysUntilOrder = Math.Round(_NoOfDaysUntilOrder, TurnsAvgDecimalPoints);
        //        return _NoOfDaysUntilOrder;
        //    }
        //    set
        //    {
        //        _NoOfDaysUntilOrder = value;
        //    }
        //}

        public double DemandPlanningQtyToOrder
        {
            get
            {
                if (MinimumQuantity != MaximumQuantity)
                {
                    _DemandPlanningQtyToOrder = (MaximumQuantity - MinimumQuantity);
                    if (IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) && _DemandPlanningQtyToOrder > 0 && _DemandPlanningQtyToOrder != DefaultReorderQuantity)
                    {
                        if (_DemandPlanningQtyToOrder < DefaultReorderQuantity)
                        {
                            _DemandPlanningQtyToOrder = DefaultReorderQuantity;
                        }
                        else
                        {
                            var mod = (_DemandPlanningQtyToOrder % DefaultReorderQuantity);
                            if (mod > 0)
                            {
                                _DemandPlanningQtyToOrder = _DemandPlanningQtyToOrder + (DefaultReorderQuantity - mod);
                            }
                        }
                    }

                    _DemandPlanningQtyToOrder = Math.Round(_DemandPlanningQtyToOrder, NumberDecimalDigits);
                }
                return _DemandPlanningQtyToOrder;
            }
            set
            {
                _DemandPlanningQtyToOrder = value;
            }
        }

        public bool? IsEnforceDefaultReorderQuantity { get; set; }
        public double OptimizedInvValueUsesQOHofAvgCalcdMinMax
        {
            get
            {
                //double DividedValue = 2 * (Cost / CostUOMValue);
                //if (DividedValue == 0)
                //{
                //    DividedValue = 1;
                //}

                //_OptimizedInvValueUsesQOHofAvgCalcdMinMax = (CalulatedMinimum + CalulatedMaximum) / DividedValue;

                _OptimizedInvValueUsesQOHofAvgCalcdMinMax = ((CalulatedMinimum + CalulatedMaximum) / 2) * (Cost / CostUOMValue);

                _OptimizedInvValueUsesQOHofAvgCalcdMinMax = Math.Round(_OptimizedInvValueUsesQOHofAvgCalcdMinMax, decimalPointFromConfig);
                return _OptimizedInvValueUsesQOHofAvgCalcdMinMax;

            }
            set
            {
                _OptimizedInvValueUsesQOHofAvgCalcdMinMax = value;
            }
        }
        public double OptimizedInvValueChange
        {
            get
            {
                if (DisplayInventoryValueforReplenish == 1)
                {
                    _OptimizedInvValueChange = ItemInventoryValue - OptimizedInvValueUsesQOHofAvgCalcdMinMax;
                }
                else
                {
                    _OptimizedInvValueChange = ItemExtendedCostValue - OptimizedInvValueUsesQOHofAvgCalcdMinMax;
                }

                _OptimizedInvValueChange = Math.Round(_OptimizedInvValueChange, decimalPointFromConfig);
                return _OptimizedInvValueChange;
            }
            set
            {
                _OptimizedInvValueChange = value;
            }
        }
        public double TrialCalcInvValueUsesQOHofAvgTrialMinMax
        {
            get
            {
                //double DividedValue = 2 * (Cost / CostUOMValue);
                //if (DividedValue == 0)
                //{
                //    DividedValue = 1;
                //}
                //_TrialCalcInvValueUsesQOHofAvgTrialMinMax = (AutoCurrentMin + AutoCurrentMax) / DividedValue;

                _TrialCalcInvValueUsesQOHofAvgTrialMinMax = ((AutoCurrentMin + AutoCurrentMax) / 2) * (Cost / CostUOMValue);

                _TrialCalcInvValueUsesQOHofAvgTrialMinMax = Math.Round(_TrialCalcInvValueUsesQOHofAvgTrialMinMax, decimalPointFromConfig);
                return _TrialCalcInvValueUsesQOHofAvgTrialMinMax;
            }
            set
            {
                _TrialCalcInvValueUsesQOHofAvgTrialMinMax = value;
            }
        }
        public double TrialInvValueChange
        {
            get
            {
                if (DisplayInventoryValueforReplenish == 1)
                {
                    _TrialInvValueChange = ItemInventoryValue - TrialCalcInvValueUsesQOHofAvgTrialMinMax;
                }
                else
                {
                    _TrialInvValueChange = ItemExtendedCostValue - TrialCalcInvValueUsesQOHofAvgTrialMinMax;
                }
                _TrialInvValueChange = Math.Round(_TrialInvValueChange, decimalPointFromConfig);
                return _TrialInvValueChange;
            }
            set
            {
                _TrialInvValueChange = value;
            }
        }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public string UDF3 { get; set; }
        public string UDF4 { get; set; }
        public string UDF5 { get; set; }
        public string UDF6 { get; set; }
        public string UDF7 { get; set; }
        public string UDF8 { get; set; }
        public string UDF9 { get; set; }
        public string UDF10 { get; set; }

        public string DateofOrder
        {
            get
            {
                //if (LeadTimeInDays > 0)
                //{

                bool isCalculate = true;

                if (_MinMaxMeasureMethod == 2)
                {
                    if (AvgDailyPullUsage <= 0)
                    {
                        isCalculate = false;
                    }
                }
                else if (_MinMaxMeasureMethod == 3)
                {
                    if (AvgDailyOrderUsage <= 0)
                    {
                        isCalculate = false;
                    }
                }

                if (MinimumQuantity == 0)
                {
                    isCalculate = false;
                }

                if (isCalculate)
                {
                    if (Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays) > 0 && Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays) > DateofOrderMaxDays)
                    {
                        _DateofOrder = FnCommon.ConvertDateByTimeZone(DateTime.UtcNow.AddDays(DateofOrderMaxDays), true, true);
                    }
                    else if (Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays) < 0 && Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays) < (-(DateofOrderMaxDays)))
                    {
                        _DateofOrder = FnCommon.ConvertDateByTimeZone(DateTime.UtcNow.AddDays(-(DateofOrderMaxDays)), true, true);
                    }
                    else
                    {
                        _DateofOrder = FnCommon.ConvertDateByTimeZone(DateTime.UtcNow.AddDays(Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays)), true, true);
                    }
                    //}
                    //else
                    //{
                    //    _DateofOrder = FnCommon.ConvertDateByTimeZone(DateTime.UtcNow.AddDays(Convert.ToInt64(_NoOfDaysUntilOrder)), true, true);
                    //}
                }
                return _DateofOrder;
            }
            set
            {
                _DateofOrder = value;
            }
        }

        public DateTime dtDateofOrder
        {
            get
            {
                bool isCalculate = true;
                if (_MinMaxMeasureMethod == 2)
                {
                    if (AvgDailyPullUsage <= 0)
                    {
                        isCalculate = false;
                    }
                }
                else if (_MinMaxMeasureMethod == 3)
                {
                    if (AvgDailyOrderUsage <= 0)
                    {
                        isCalculate = false;
                    }
                }

                if (MinimumQuantity == 0)
                {
                    isCalculate = false;
                }
                if (isCalculate)
                {
                    if (Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays) > 0 && Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays) > DateofOrderMaxDays)
                    {
                        _dtDateofOrder = DateTime.UtcNow.AddDays((DateofOrderMaxDays));
                    }
                    else if (Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays) < 0 && Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays) < (-(DateofOrderMaxDays)))
                    {
                        _dtDateofOrder = DateTime.UtcNow.AddDays(-(DateofOrderMaxDays));
                    }
                    else
                    {
                        _dtDateofOrder = DateTime.UtcNow.AddDays(Convert.ToInt64((NoOfDaysUntilOrder) - LeadTimeInDays));
                    }
                }
                return _dtDateofOrder;
            }
            set
            {
                _dtDateofOrder = value;
            }
        }

        public int ItemTrackingType { get; set; }
        public int ItemStockStatus { get; set; }
        public int ItemType { get; set; }

        public double ExtendedCost { get; set; }

        public DateTime DateUpdated { get; set; }
        public DateTime Created { get; set; }

        private DateTime? _DateUpdated;
        public DateTime? DateUpdatedDateStr
        {
            get
            {
                if (_DateUpdated == null)
                {
                    _DateUpdated = FnCommon.ConvertDateByTimeZoneReturnDate(DateUpdated, false, true);
                }
                return _DateUpdated;
            }
            set { this._DateUpdated = value; }
        }

        private DateTime? _Created;
        public DateTime? CreatedStr
        {
            get
            {
                if (_Created == null)
                {
                    _Created = FnCommon.ConvertDateByTimeZoneReturnDate(Created, false, true);
                }
                return _Created;
            }
            set { this._Created = value; }
        }

        public string CreatedByUser { get; set; }
        public string LastUpdatedByUser { get; set; }

        public double TransferQuantity { get { return _TransferQuantity; } set { _TransferQuantity = value; } }
        public double TransferCost
        {
            get
            {
                _TransferCost = Math.Round(_TransferCost, TurnsAvgDecimalPoints);
                return _TransferCost;
            }
            set { _TransferCost = value; }
        }
    }
}
