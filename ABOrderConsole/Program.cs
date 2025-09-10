using ABOrderConsole.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ABOrderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string URLi = "http://www.amazon.com/b2b/abws/applicationdetails/amzn1.sp.solution.d020becf-e998-4482-b4f4-2f820ef137b6";
            URLi = HttpUtility.UrlDecode(URLi);
            Uri myUri = new Uri(URLi);
            string host = myUri.Host;


            //Console.WriteLine("enter starting date");
            //var startDate = Console.ReadLine();
            //Console.WriteLine("Enter EndDate");
            //var endDate = Console.ReadLine();
            //CultureInfo provider = CultureInfo.InvariantCulture;

            //var t = Convert.ToDateTime(endDate);
            //var startingDate = DateTime.ParseExact(startDate, "mm/dd/yyyy", provider); //DateTime.Parse(startDate);
            //var endingDate = DateTime.ParseExact(endDate, "mm/dd/yyyy", provider); //DateTime.Parse(startDate);
            var orderHelper = new ABOrderHelper();

            //orderHelper.ImportABOrderToLocalDB(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            //orderHelper.GetInsertedABOrder();

            //orderHelper.SyncABOrderedItemsToRoomItems();

            //var zohoHelper = new ZohoHelper();
            //zohoHelper.TestRoleCreation();

            //var entSetUpHelper = new ABEnterpriseSetup();
            //entSetUpHelper.ABECRSetupProcess();

            orderHelper.SyncABOrderedItemsToRoom();
        }
    }
}
