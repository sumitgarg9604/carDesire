using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Assignment1_v9.Models;

namespace Assignment1_v9.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PickDropLocsController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: PickDropLocs
        public ActionResult Index()
        {
            return View(db.PickDropLocs.ToList());
        }

        // GET: PickDropLocs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickDropLoc pickDropLoc = db.PickDropLocs.Find(id);
            if (pickDropLoc == null)
            {
                return HttpNotFound();
            }
            return View(pickDropLoc);
        }

        // GET: PickDropLocs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PickDropLocs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,LocStreet,LocSuburb,LocState,Landmark,Longitude,Latitude")] PickDropLoc pickDropLoc)
        {
            StringBuilder street = new StringBuilder();
            street.Append(HttpUtility.HtmlEncode(pickDropLoc.LocStreet));
            pickDropLoc.LocStreet = street.ToString();

            StringBuilder state = new StringBuilder();
            state.Append(HttpUtility.HtmlEncode(pickDropLoc.LocState));
            pickDropLoc.LocState = state.ToString();

            StringBuilder suburb = new StringBuilder();
            suburb.Append(HttpUtility.HtmlEncode(pickDropLoc.LocSuburb));
            pickDropLoc.LocSuburb = suburb.ToString();

            StringBuilder landm = new StringBuilder();
            landm.Append(HttpUtility.HtmlEncode(pickDropLoc.Landmark));
            pickDropLoc.Landmark = landm.ToString();


            if (ModelState.IsValid)
            {
                db.PickDropLocs.Add(pickDropLoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pickDropLoc);
        }

        // GET: PickDropLocs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickDropLoc pickDropLoc = db.PickDropLocs.Find(id);
            if (pickDropLoc == null)
            {
                return HttpNotFound();
            }
            return View(pickDropLoc);
        }

        // POST: PickDropLocs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LocStreet,LocSuburb,LocState,Landmark,Longitude,Latitude")] PickDropLoc pickDropLoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pickDropLoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pickDropLoc);
        }

        // GET: PickDropLocs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickDropLoc pickDropLoc = db.PickDropLocs.Find(id);
            if (pickDropLoc == null)
            {
                return HttpNotFound();
            }
            return View(pickDropLoc);
        }

        // POST: PickDropLocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PickDropLoc pickDropLoc = db.PickDropLocs.Find(id);
            db.PickDropLocs.Remove(pickDropLoc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
