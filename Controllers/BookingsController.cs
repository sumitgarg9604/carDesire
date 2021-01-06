using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Assignment1_v9.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Syroot.Windows.IO;

namespace Assignment1_v9.Controllers
{
    public class BookingsController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Bookings
        [Authorize(Roles = "Administrator , Manager")]
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Car).Include(b => b.AspNetUser).Include(b => b.PickDropLoc);
            return View(bookings.ToList());
        }

        // GET: Bookings/Details/5
        [Authorize(Roles = "Administrator , Manager")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        [Authorize]
        public ActionResult MyBookings()
        {
            var userID = User.Identity.GetUserId();
            var b = db.Bookings.Where(c=>c.AspNetUserId == userID).ToList();
            return View(b);
        }

        // GET: Bookings/Create
        [Authorize(Roles = "Administrator , Manager")]
        public ActionResult Create()
        {
            ViewBag.CarsId = new SelectList(db.Cars, "Id", "CarMake");
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.PickDropLocId = new SelectList(db.PickDropLocs, "Id", "LocStreet");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator , Manager")]
        public ActionResult Create([Bind(Include = "Id,PickUpDate,DropOffDate,CarsId,AspNetUserId,PickDropLocId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CarsId = new SelectList(db.Cars, "Id", "CarMake", booking.CarsId);
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", booking.AspNetUserId);
            ViewBag.PickDropLocId = new SelectList(db.PickDropLocs, "Id", "LocStreet", booking.PickDropLocId);
            return View(booking);
        }

        public ActionResult CompleteBooking()
        {
            var PickUpDate = Session["PickUpDate"].ToString();
            var DropOffDate = Session["DropOffDate"].ToString();
            var CarId = Session["CarId"].ToString();
            var PickDropLocId = Session["PickDropLocId"].ToString();
            var totalPrice = Session["totalPrice"].ToString();

            return CompleteBooking(PickUpDate, DropOffDate, CarId, PickDropLocId, totalPrice);
        }

        [HttpPost]
        public ActionResult CompleteBooking(string PickUpDate, string DropOffDate, string CarId, string PickDropLocId, string totalPrice)
        {

            var userID = User.Identity.GetUserId();
            if (userID == null)
            {
                Session["PickUpDate"] = PickUpDate;
                Session["DropOffDate"] = DropOffDate;
                Session["CarId"] = CarId;
                Session["PickDropLocId"] = PickDropLocId;
                Session["totalPrice"] = totalPrice;
                return RedirectToAction("Login", "Account",
                           new { returnUrl = "/Bookings/CompleteBooking" });
            }

            //string PickUpDate1 = "08-24-2019 11:00 AM";
            DateTime pd = DateTime.ParseExact(PickUpDate, "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dd = DateTime.ParseExact(DropOffDate, "dd/MM/yyyy hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);




            int cId = int.Parse(CarId);
            int PDLId = int.Parse(PickDropLocId);

            Booking b = new Booking()
            {
                AspNetUserId = userID,
                CarsId = cId,
                PickDropLocId = PDLId,
                PickUpDate = pd,
                DropOffDate = dd
            };




            db.Bookings.Add(b);
            db.SaveChanges();

            ViewBag.Message = "Thanks for Booking with CarDesire !!!";

            // var id = db.Bookings.Where((c => (c.PickUpDate == pd && c.DropOffDate == dd && c.CarsId == cId && c.AspNetUserId == userID && c.PickDropLocId == PDLId))).Select(c=>c.Id).ToList();

            var cardetails = db.Cars.Where(c => (c.Id == cId)).ToList();
            var userdetails = db.AspNetUsers.Where(c => (c.Id == userID)).ToList();
            var locdetails = db.PickDropLocs.Where(c => (c.Id == PDLId)).ToList();
            ViewBag.carMake = cardetails[0].CarMake;
            ViewBag.userEmail = userdetails[0].Email;


            ViewBag.userName = userdetails[0].FirstName + " " + userdetails[0].LastName;
            ViewBag.carModel = cardetails[0].CarModel;
            ViewBag.carRC = cardetails[0].CarRC;
            ViewBag.carTransmission = cardetails[0].Transmission;
            ViewBag.carCapacity = cardetails[0].Capacity;
            ViewBag.carHourlyRate = cardetails[0].HourlyRate;
            ViewBag.carTotalCost = totalPrice;
            ViewBag.LocStreet = locdetails[0].LocStreet;
            ViewBag.LocSuburb = locdetails[0].LocSuburb;
            ViewBag.LocState = locdetails[0].LocState;
            ViewBag.fileName = cardetails[0].Path;
            
            string downloadsPath = new KnownFolder(KnownFolderType.Downloads).Path;
            ViewBag.pdfPath = "Your PDF will be downloaded at " + downloadsPath + "\\Reciept.pdf";


            // int id1  = db.Bookings.Where(c => (c.PickUpDate == pd && c.DropOffDate == dd && c.CarsId == cId && c.AspNetUserId == userID && c.PickDropLocId == PDLId)).Select(c=>(c.Id)).FirstOrDefault();
            // Booking bb = db.Bookings.Find(id1);
            //booking



            return View(b);

        }
        
     

        public ActionResult generatepdf(string userName,
                                string carMake,
                                string carModel,
                                string carRC,
                                string carTransmission,
                                string carCapacity,
                                string carHourlyRate,
                                string carTotalCost,
                                string userEmail,
                                string LocStreet,
                                string LocSuburb,
                                string LocState,
                                string fileName)
        {

            // using System.IO;
            //  using iTextSharp.text;
            // using iTextSharp.text.pdf;

            string serverPath = Server.MapPath("~/Uploads/");
            string downloadsPath = new KnownFolder(KnownFolderType.Downloads).Path;
          //  string appRootDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName;
            FileStream fs = new FileStream(downloadsPath + "/Reciept.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            doc.Add(new Paragraph("PNG"));

            Image gif = Image.GetInstance(serverPath + "CarDesire_Logo.png");
            doc.Add(gif);

            PdfPTable table = new PdfPTable(2);

            PdfPCell cell = new PdfPCell(new Phrase("CarDesire Reciept"));

            cell.Colspan = 2;

            cell.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right

            table.AddCell(cell);

            table.AddCell("Name");
            table.AddCell(userName);

            table.AddCell("Car Make");
            table.AddCell(carMake);

            table.AddCell("Car Model");
            table.AddCell(carModel);

            table.AddCell("Car Registration:");
            table.AddCell(carRC);

            table.AddCell("Transmission");
            table.AddCell(carTransmission);

            table.AddCell("Seating Capacity");
            table.AddCell(carCapacity);

            table.AddCell("Hourly Rate");
            table.AddCell(carHourlyRate);

            table.AddCell("Total Cost");
            table.AddCell(carTotalCost);

            table.AddCell("Email");
            table.AddCell(userEmail);

            table.AddCell("Location");
            table.AddCell(LocStreet + " " + LocSuburb + " " + LocState);

            doc.Add(table);
            doc.Add(new Paragraph("Thanks for choosing CarDesire !!"));
            doc.Close();

            ViewBag.info = "Your PDF will be downloaded at " + downloadsPath + "\\Reciept.pdf";
            return View();
        }

        //[HttpPost]
        [AllowAnonymous]
        public ActionResult SearchCars(PickDropLoc loc)
        {
            // int a = collection.Count;
            //var pickup = collection["PickUpDate"];
            // var dropoff = collection["DropOffDate"];
            //  if (a != 0)
            //  {
            //   var pickupdate = collection["PickUpDate"];
            //   var dropoffdate = collection["DropOffDate"];
            //   return RedirectToAction("../Cars/ViewCars", new {pick = pickupdate, drop = dropoffdate});
            // }
            // var id = collection["AspNetUserId"];
            //  var pickupdate = collection["PickUpDate"];
            // ViewBag.PUD = a;
            //  ViewBag.d = d;
            // ViewBag.CarsId = new SelectList(db.Cars, "Id", "CarMake");
            //   ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email");
            //var list1 = new List<>
            //               var items = new SelectList(db.PickDropLocs.Select(m => m.LocSuburb).Distinct()).ToList();
            //var S = db.PickDropLocs.Select(m => m.LocSuburb).Distinct().ToList(); 
            // var items = new SelectList(db.PickDropLocs.Select(m => m.LocSuburb).Distinct(),"LocSuburb","LocSuburb");//).ToList();

            var items = new SelectList(db.PickDropLocs.DistinctBy(m => m.LocSuburb), "LocSuburb", "LocSuburb").ToList();

            ViewBag.impmessage = TempData["impmessage"];
            


            //var items1 = db.PickDropLocs.ToList();
            // if (items !=null)
            // {
            ViewBag.PickDropLocData = new SelectList(db.PickDropLocs.DistinctBy(m => m.LocSuburb), "LocSuburb", "LocSuburb").ToList(); ;
          //  } 
            //ViewBag.PickDropLocId = new SelectList(db.PickDropLocs, "Id", "LocSuburb", loc.Id);
            return View();
        }



        // GET: Bookings/Edit/5
        [Authorize(Roles = "Administrator , Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.CarsId = new SelectList(db.Cars, "Id", "CarMake", booking.CarsId);
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", booking.AspNetUserId);
            ViewBag.PickDropLocId = new SelectList(db.PickDropLocs, "Id", "LocStreet", booking.PickDropLocId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator , Manager")]
        public ActionResult Edit([Bind(Include = "Id,PickUpDate,DropOffDate,CarsId,AspNetUserId,PickDropLocId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CarsId = new SelectList(db.Cars, "Id", "CarMake", booking.CarsId);
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Email", booking.AspNetUserId);
            ViewBag.PickDropLocId = new SelectList(db.PickDropLocs, "Id", "LocStreet", booking.PickDropLocId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        [Authorize(Roles = "Administrator , Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        [Authorize(Roles = "User")]
        public ActionResult DeleteMyBooking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("DeleteMyBooking")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public ActionResult DeleteConfirmed1(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("MyBookings");
        }




        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
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
