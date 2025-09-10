using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Xml.Linq;

namespace eTurnsWeb.Helper
{
    public class ResourceUtils
    {
        public static string GetResource(string ResourceFileName, string resourceKey, bool isUDFName, bool OtherFromeTurns = true, bool ForEnterPriseSetup = false)
        {
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            int ResourceSaveMethod = (int)eTurns.DTO.ResourceReadType.File;
            //int.TryParse(Convert.ToString(Settinfile.Element("ResourceRead").Value), out ResourceSaveMethod); 
            int.TryParse(Convert.ToString(eTurns.DTO.SiteSettingHelper.ResourceRead), out ResourceSaveMethod); 

            if (ResourceSaveMethod == (int)eTurns.DTO.ResourceReadType.File)
            {
                return eTurns.DTO.Resources.ResourceHelper.GetResourceValue(resourceKey, ResourceFileName, isUDFName, OtherFromeTurns: OtherFromeTurns, ForEnterPriseSetup: ForEnterPriseSetup);
            }
            else
            {
                return eTurns.DTO.Resources.ResourceModuleHelper.GetResourceValue(resourceKey, ResourceFileName, isUDFName, OtherFromeTurns: OtherFromeTurns, ForEnterPriseSetup: ForEnterPriseSetup);
            }
        }


    }
}
