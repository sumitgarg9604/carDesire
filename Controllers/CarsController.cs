using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Assignment1_v9.Models;
using Newtonsoft.Json;


namespace Assignment1_v9.Content
{
    [Authorize(Roles = "Administrator")]
    public class CarsController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Cars
        public ActionResult Index()
        {
            var cars = db.Cars.Include(c => c.PickDropLoc);
            return View(cars.ToList());
        }

        [AllowAnonymous]
        public ActionResult CreateChart()
        {


            try
            {

                var c = db.Cars
                        .Select(s => new ViewCarsLocations.ChartCar()
                        { //<--HERE
                            ChartCarMake = s.CarMake,
                            ChartCarMileage = s.Mileage
                        }).ToList();

               // var c  = db.Cars.Select(m=> m.CarMake,m =>m.Mileage).ToList();
                ViewBag.DataPoints = JsonConvert.SerializeObject(c, _jsonSetting);

                return View();
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                return View("Error");
            }
            catch (System.Data.SqlClient.SqlException)
            {
                return View("Error");
            }

        }
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        [AllowAnonymous]
        public ActionResult ViewCars()
        {

            return View("~/Views/Home/Index.cshtml");
        }




        // GET: Cars
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ViewCars(Booking b,string PickUpDate,string DropOffDate,String PickDropLoc)
        {
          
            
            ViewBag.pick = PickUpDate;  
            ViewBag.drop = DropOffDate;
            ViewBag.loc = PickDropLoc;

            DateTime pd = DateTime.ParseExact(PickUpDate, "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dd = DateTime.ParseExact(DropOffDate, "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

            

            System.TimeSpan diff = dd.Subtract(pd);

            var unavailable_cars = db.Bookings.Where((c =>(c.PickDropLoc.LocSuburb == PickDropLoc) && ((c.PickUpDate <=  pd && c.DropOffDate >= pd) ||
                                                     (c.PickUpDate < dd && c.DropOffDate > dd)  ||
                                                    (c.PickUpDate > pd && c.DropOffDate < dd)))).ToList();


           // var unavailable_cars = db.Bookings.Where((c => (c.PickUpDate <= pd && c.DropOffDate >= pd) ||
             //                                        (c.PickUpDate < dd && c.DropOffDate > dd) ||
               //                                     (c.PickUpDate > pd && c.DropOffDate < dd))).ToList();


            var unavailable_carsID = unavailable_cars.Select(model => model.CarsId).ToList();


            var all_carsId = db.Cars.Select(model => model.Id).ToList();


            var available_carsID = all_carsId.Except(unavailable_carsID);
         //   var checklength = available_carsID.ToList();
          

            List<Cars> cars1 = new List<Cars>();
            
            foreach (var i in available_carsID)
            {
                var cc = db.Cars.Where(model => model.Id == i).FirstOrDefault();
                cars1.Add(cc);
            }

            using (Model1Container db = new Model1Container())
            {
                List<PickDropLoc> locations = db.PickDropLocs.Where(model => model.LocSuburb == PickDropLoc).ToList();
                //List<Cars> cars= db.Cars.ToList();

                var ViewCarsLocationsq = from c in cars1
                                        join l in locations on c.PickDropLocId equals l.Id into table1
                                        from l in table1.ToList()
                                        select new ViewCarsLocations.ViewModel
                                        {
                                            loc = l,
                                            car = c
                                            
                                         };

                if (ViewCarsLocationsq.Count() == 0)
                {
                    ViewBag.impmessage = "No Cars Available from " + PickUpDate + " to " + DropOffDate + " . Please try changing Pickup or DropOff Time";
                    
                }
                
                

                    ViewBag.diff = diff;
                    // var available_Cars = ViewCarsLocationsq.Where((model => model.car.Id) );
                    return View(ViewCarsLocationsq.ToList());
                
            }  
        }

        // GET: Cars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cars cars = db.Cars.Find(id);
            if (cars == null)
            {
                return HttpNotFound();
            }
            return View(cars);
        }

        // GET: Cars/Create
        public ActionResult Create()
        {
            ViewBag.PickDropLocId = new SelectList(db.PickDropLocs, "Id", "LocStreet");
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,CarMake,CarModel,CarRC,ChasisNum,Transmission,Capacity,HourlyRate,Mileage,PickDropLocId,Path")] Cars cars, HttpPostedFileBase postedFile)
        {
            StringBuilder make = new StringBuilder();
            make.Append(HttpUtility.HtmlEncode(cars.CarMake));
            cars.CarMake = make.ToString();

            StringBuilder model = new StringBuilder();
            model.Append(HttpUtility.HtmlEncode(cars.CarModel));
            cars.CarModel = model.ToString();

            StringBuilder trans = new StringBuilder();
            trans.Append(HttpUtility.HtmlEncode(cars.Transmission));
            cars.Transmission = trans.ToString();

            StringBuilder RC = new StringBuilder();
            RC.Append(HttpUtility.HtmlEncode(cars.CarRC));
            cars.CarRC = RC.ToString();

            StringBuilder chasis = new StringBuilder();
            chasis.Append(HttpUtility.HtmlEncode(cars.ChasisNum));
            cars.ChasisNum = chasis.ToString();

            ModelState.Clear();
            var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
            cars.Path = myUniqueFileName;
            TryValidateModel(cars);

            if (ModelState.IsValid)
            {
                if (postedFile != null)
                {
                    string serverPath = Server.MapPath("~/Uploads/");
                    string fileExtension = Path.GetExtension(postedFile.FileName);
                    string filePath = cars.Path + fileExtension;
                    cars.Path = filePath;
                    postedFile.SaveAs(serverPath + cars.Path);
                }
                db.Cars.Add(cars);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PickDropLocId = new SelectList(db.PickDropLocs, "Id", "LocStreet", cars.PickDropLocId);
            return View(cars);
        }

        // GET: Cars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cars cars = db.Cars.Find(id);
            if (cars == null)
            {
                return HttpNotFound();
            }
            ViewBag.PickDropLocId = new SelectList(db.PickDropLocs, "Id", "LocStreet", cars.PickDropLocId);
            return View(cars);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CarMake,CarModel,CarRC,ChasisNum,Transmission,Capacity,HourlyRate,Mileage,PickDropLocId")] Cars cars)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cars).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PickDropLocId = new SelectList(db.PickDropLocs, "Id", "LocStreet", cars.PickDropLocId);
            return View(cars);
        }

        // GET: Cars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cars cars = db.Cars.Find(id);
            if (cars == null)
            {
                return HttpNotFound();
            }
            return View(cars);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cars cars = db.Cars.Find(id);
            db.Cars.Remove(cars);
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
