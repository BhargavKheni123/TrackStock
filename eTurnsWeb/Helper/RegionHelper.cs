using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace eTurnsWeb.Helper
{
    public class RegionHelper
    {
        public static List<RegionLanguage> GetAllCultures()
        {
            ResourceDAL objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
            List<ResourceLanguageDTO> lstLangs = objDAL.GetCachedResourceLanguageData(SessionHelper.EnterPriceID).ToList();
            List<RegionLanguage> lstLans = new List<RegionLanguage>();
            if (lstLangs != null && lstLangs.Count > 0)
            {
                foreach (var item in lstLangs)
                {
                    lstLans.Add(new RegionLanguage() { CultureCode = item.Culture, CultureDisplayName = item.Language });
                }
            }
            return lstLans;
            //RegionLanguage objRegionLanguage = new RegionLanguage();

            //objRegionLanguage.CultureCode = "de-DE";
            //objRegionLanguage.CultureDisplayName = "German (Germany)";
            //lstLans.Add(objRegionLanguage);

            //objRegionLanguage = new RegionLanguage();
            //objRegionLanguage.CultureCode = "en-AU";
            //objRegionLanguage.CultureDisplayName = "English (Australia)";
            //lstLans.Add(objRegionLanguage);

            //objRegionLanguage = new RegionLanguage();
            //objRegionLanguage.CultureCode = "en-GB";
            //objRegionLanguage.CultureDisplayName = "English (United Kingdom)";
            //lstLans.Add(objRegionLanguage);

            //objRegionLanguage = new RegionLanguage();
            //objRegionLanguage.CultureCode = "en-IN";
            //objRegionLanguage.CultureDisplayName = "English (India)";
            //lstLans.Add(objRegionLanguage);

            //objRegionLanguage = new RegionLanguage();
            //objRegionLanguage.CultureCode = "en-US";
            //objRegionLanguage.CultureDisplayName = "English (United States)";
            //lstLans.Add(objRegionLanguage);

            //objRegionLanguage = new RegionLanguage();
            //objRegionLanguage.CultureCode = "fr-FR";
            //objRegionLanguage.CultureDisplayName = "French (France)";
            //lstLans.Add(objRegionLanguage);

            //objRegionLanguage = new RegionLanguage();
            //objRegionLanguage.CultureCode = "nl-BE";
            //objRegionLanguage.CultureDisplayName = "Dutch (Belgium)";
            //lstLans.Add(objRegionLanguage);

            //objRegionLanguage = new RegionLanguage();
            //objRegionLanguage.CultureCode = "nl-NL";
            //objRegionLanguage.CultureDisplayName = "Dutch (Netherlands)";
            //lstLans.Add(objRegionLanguage);





            //ResourceLaguage obj
            //List<string> list = new List<string>();
            //foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            //{

            //    string specName = "(none)";
            //    try { specName = CultureInfo.CreateSpecificCulture(ci.Name).Name; }
            //    catch { }
            //    list.Add(String.Format("{0,-12}{1,-12}{2}", ci.Name, specName, ci.EnglishName));
            //}


            //list.Sort();
            //return list;
            //return objCultureInfo.DateTimeFormat.GetAllDateTimePatterns().ToList();
        }

        public static eTurnsRegionInfo GetFormates(string CultureCode)
        {

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string _strCustomShortDatePatterns = "";
            //if (Settinfile.Element("CustomShortDatePatterns") != null)
            //{
            //    _strCustomShortDatePatterns = Convert.ToString(Settinfile.Element("CustomShortDatePatterns").Value);
            //}

            if (SiteSettingHelper.CustomShortDatePatterns != string.Empty)
            {
                _strCustomShortDatePatterns = Convert.ToString(SiteSettingHelper.CustomShortDatePatterns);
            }

            CultureInfo objCultureInfo = CultureInfo.CreateSpecificCulture(CultureCode);
            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
            //char[] formats = { 'd', 'D', 'f', 'F', 'g', 'G', 'm', 'o', 'r', 's', 't', 'T', 'u', 'U', 'y' };            
            objeTurnsRegionInfo.CultureCode = CultureCode;
            objeTurnsRegionInfo.CultureDisplayName = objCultureInfo.DisplayName;
            objeTurnsRegionInfo.CultureName = objCultureInfo.Name;
            objeTurnsRegionInfo.lstLongDatePatterns = objCultureInfo.DateTimeFormat.GetAllDateTimePatterns('D').ToList();
            objeTurnsRegionInfo.lstShortDatePatterns = objCultureInfo.DateTimeFormat.GetAllDateTimePatterns('d').ToList();
            objeTurnsRegionInfo.NumberDecimalDigits = objCultureInfo.NumberFormat.NumberDecimalDigits;
            objeTurnsRegionInfo.CurrencyDecimalDigits = objCultureInfo.NumberFormat.CurrencyDecimalDigits;
            objeTurnsRegionInfo.NumberDecimalSeparator = objCultureInfo.NumberFormat.NumberDecimalSeparator;
            objeTurnsRegionInfo.CurrencyGroupSeparator = objCultureInfo.NumberFormat.CurrencyGroupSeparator;
            objeTurnsRegionInfo.lstShortTimePatterns = objCultureInfo.DateTimeFormat.GetAllDateTimePatterns('t').ToList();
            objeTurnsRegionInfo.lstLongTimePatterns = objCultureInfo.DateTimeFormat.GetAllDateTimePatterns('T').ToList();

            if (!string.IsNullOrWhiteSpace(_strCustomShortDatePatterns))
            {
                if (objeTurnsRegionInfo.lstShortDatePatterns == null)
                {
                    objeTurnsRegionInfo.lstShortDatePatterns = new List<string>();
                }

                string[] strDatePattern = _strCustomShortDatePatterns.Split(',');
                if (strDatePattern != null && strDatePattern.Length > 0)
                {
                    foreach (string item in strDatePattern)
                    {
                        objeTurnsRegionInfo.lstShortDatePatterns.Add(item);
                    }

                }

            }

            return objeTurnsRegionInfo;
        }

        public static string FormatNumbers(double? ValueToFormat, int NumberType)
        {
            if (!ValueToFormat.HasValue)
            {
                return string.Empty;
            }
            string ReturnVal = ValueToFormat.ToString();
            string strFormat = string.Empty;

            switch (NumberType)
            {
                case 1://Cost
                    ReturnVal = string.Format(CurrencyFormates(Convert.ToInt32(SessionHelper.CurrencyDecimalDigits)), ValueToFormat);
                    break;
                case 2://Qty
                    ReturnVal = string.Format(CurrencyFormates(Convert.ToInt32(SessionHelper.NumberDecimalDigits)), ValueToFormat);
                    break;
                case 3://As It is
                    ReturnVal = ValueToFormat.ToString();
                    break;
                case 4: // Turns and Average Usage
                    ReturnVal = string.Format(CurrencyFormates(Convert.ToInt32(SessionHelper.NumberAvgDecimalPoints)), ValueToFormat);
                    break;
                case 5://Weight 
                    ReturnVal = string.Format(CurrencyFormates(Convert.ToInt32(SessionHelper.WeightDecimalPoints)), ValueToFormat);
                    break;
            }
            return ReturnVal;
        }
        public static string CurrencyFormates(int decimalDigits)
        {
            string ReturnFormat = "{0:0.00}";
            switch (decimalDigits)
            {
                case 0:
                    ReturnFormat = "{0:0}";
                    break;
                case 1:
                    ReturnFormat = "{0:0.0}";
                    break;
                case 2:
                    ReturnFormat = "{0:0.00}";
                    break;
                case 3:
                    ReturnFormat = "{0:0.000}";
                    break;
                case 4:
                    ReturnFormat = "{0:0.0000}";
                    break;
                case 5:
                    ReturnFormat = "{0:0.00000}";
                    break;
                case 6:
                    ReturnFormat = "{0:0.000000}";
                    break;
                case 7:
                    ReturnFormat = "{0:0.0000000}";
                    break;
                case 8:
                    ReturnFormat = "{0:0.00000000}";
                    break;
                case 9:
                    ReturnFormat = "{0:0.000000000}";
                    break;
                case 10:
                    ReturnFormat = "{0:0.0000000000}";
                    break;
                default:
                    ReturnFormat = "{0:0.00}";
                    break;
            }
            return ReturnFormat;
        }

        public static string FormatBoolean(bool? BoolVal)
        {
            if (BoolVal.HasValue)
            {
                if (BoolVal.Value)
                {
                    return "yes";
                }
                else if (!BoolVal.Value)
                {
                    return "no";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
        public static string FormatItemType(int? itemTypeVal)
        {
            if (itemTypeVal.HasValue == false)
            {
                return string.Empty;
            }
            string retVal = string.Empty;
            switch (itemTypeVal.Value)
            {
                case 1:
                    retVal = "Item";
                    break;
                case 2:
                    retVal = "Quick List";
                    break;
                case 3:
                    retVal = "Kit";
                    break;
                case 4:
                    retVal = "Labor";
                    break;
                default:
                    retVal = string.Empty;
                    break;
            }

            return retVal;

            //if (itemTypeVal.HasValue)
            //{
            //    if (itemTypeVal.Value == 1)
            //    {
            //        return "Item";
            //    }
            //    else if (itemTypeVal.Value == 2)
            //    {
            //        return "Quick List";
            //    }
            //    else if (itemTypeVal.Value == 3)
            //    {
            //        return "Kit";
            //    }
            //    else if (itemTypeVal.Value == 4)
            //    {
            //        return "Labor";
            //    }
            //    else
            //    {
            //        return string.Empty;
            //    }
            //}
            //else
            //{
            //    return string.Empty;
            //}
        }

        //public RegionInfo GetRegionInfoById(long RoomId, long CompanyId, long UserId)
        //{
        //    return new RegionInfo();
        //}

    }
}