using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment1_v9.Controllers
{
    public class SearchCarsController : Controller
    {
        // GET: SearchCars
        public ActionResult Index()
        {
            return View();
        }

        // GET: SearchCars/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SearchCars/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SearchCars/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SearchCars/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SearchCars/Edit/5
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

        // GET: SearchCars/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SearchCars/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
