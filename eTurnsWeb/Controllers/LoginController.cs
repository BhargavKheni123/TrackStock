using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    public class LoginController : eTurnsControllerBase
    {

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /LoginTest/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /LoginTest/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Login");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /LoginTest/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /LoginTest/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
