using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Assignment1_v9.Models;
using Assignment1_v9.Utils;
using SendGrid.Helpers.Mail;

namespace Assignment1_v9.Controllers
{//[Authorize(Roles ="Administrator")]
    public class ContactUsController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: ContactUs
        public ActionResult Index()
        {
            return View(db.ContactUs.ToList());
        }

        // GET: ContactUs/Details/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = db.ContactUs.Find(id);
            if (contactUs == null)
            {
                return HttpNotFound();
            }
            return View(contactUs);
        }
        [AllowAnonymous]
        // GET: ContactUs/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles ="Manager")]
        public ActionResult BulkMarketing(BulkEmail model, HttpPostedFileBase postedFile,string EmailValues)
        {
            
            
            if (ModelState.IsValid)
            {
                try
                {
                    String path = null;
                    if (postedFile != null)
                    {
                        ModelState.Clear();
                        var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
                        string Path1 = myUniqueFileName;
                        TryValidateModel(model);

                        string serverPath = Server.MapPath("~/Uploads/");
                        string fileExtension = Path.GetExtension(postedFile.FileName);
                        string filePath = Path1 + fileExtension;
                        Path1 = serverPath + filePath;
                        postedFile.SaveAs(Path1);
                         path = Path1;
                    }
                    String name = model.Name;
                 //   String toEmail = model.Email;
                    String subject = model.Subject;
                    String contents = model.Query;
                    
                    EmailSender es = new EmailSender();
                    es.SendBulk(EmailValues, subject, contents, name, path);

                    ViewBag.Result = "Email has been sent.";
                    ViewBag.AspNetUser_Id = new MultiSelectList(db.AspNetUsers, "Email", "Email").ToList();
                    ModelState.Clear();

                    return View(new BulkEmail());
                }
                catch
                {
                    ViewBag.AspNetUser_Id = new MultiSelectList(db.AspNetUsers, "Email", "Email").ToList();
                    return View();
                }
            }
            return View();
        }

        // POST: ContactUs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Create(ContactUs model,HttpPostedFileBase postedFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    String path = null;
                    if (postedFile != null)
                    {
                        ModelState.Clear();
                        var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
                        string Path1 = myUniqueFileName;
                        TryValidateModel(model);

                        string serverPath = Server.MapPath("~/Uploads/");
                        string fileExtension = Path.GetExtension(postedFile.FileName);
                        string filePath = Path1 + fileExtension;
                        Path1 = serverPath + filePath;
                        postedFile.SaveAs(Path1);
                        path = Path1;
                    }

                    String name = model.Name;
                    String toEmail = model.Email;
                    String subject = model.Subject;
                    String contents = model.Query;
                    
                    EmailSender es = new EmailSender();
                    es.Send(toEmail, subject, contents,name,path);

                    ViewBag.Result = "Email has been sent.";

                    ModelState.Clear();

                    return View(new ContactUs());
                }
                catch
                {
                    return View();
                }
            }

            return View();
        }

        // GET: ContactUs/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = db.ContactUs.Find(id);
            if (contactUs == null)
            {
                return HttpNotFound();
            }
            return View(contactUs);
        }

        // POST: ContactUs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Subject,Query")] ContactUs contactUs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactUs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactUs);
        }

        // GET: ContactUs/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = db.ContactUs.Find(id);
            if (contactUs == null)
            {
                return HttpNotFound();
            }
            return View(contactUs);
        }

        // POST: ContactUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            ContactUs contactUs = db.ContactUs.Find(id);
            db.ContactUs.Remove(contactUs);
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
