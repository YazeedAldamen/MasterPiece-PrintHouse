using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PrintHouse.Models;

namespace PrintHouse.Controllers
{
    public class FAQsController : Controller
    {
        private PrintHouseEntities db = new PrintHouseEntities();

        // GET: FAQs
        public ActionResult Index()
        {
            return View(db.FAQs.ToList());
        }
        [Authorize(Roles ="Admin")]
        public ActionResult AdminFaqs(){
            return View(db.FAQs.ToList());
        }
        // GET: FAQs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FAQ fAQ = db.FAQs.Find(id);
            if (fAQ == null)
            {
                return HttpNotFound();
            }
            return View(fAQ);
        }

        // GET: FAQs/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: FAQs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "faqId,question,answer")] FAQ fAQ)
        {
            if (ModelState.IsValid)
            {
                db.FAQs.Add(fAQ);
                db.SaveChanges();
                Session["SweetAlertMessage"] = "FAQ was Added Successfully";
                Session["SweetAlertType"] = "success";
                Session["fromDelete"] = "true";
                return RedirectToAction("AdminFaqs");
            }

            return RedirectToAction("AdminFaqs");
        }

        // GET: FAQs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FAQ fAQ = db.FAQs.Find(id);
            if (fAQ == null)
            {
                return HttpNotFound();
            }
            return View(fAQ);
        }

        // POST: FAQs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "faqId,question,answer")] FAQ fAQ)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fAQ).State = EntityState.Modified;
                db.SaveChanges();
                Session["SweetAlertMessage"] = "FAQ was Edited Successfully";
                Session["SweetAlertType"] = "success";
                Session["fromDelete"] = "true";
                return RedirectToAction("AdminFaqs");
            }
            return RedirectToAction("AdminFaqs");
        }

        // GET: FAQs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FAQ fAQ = db.FAQs.Find(id);
            if (fAQ == null)
            {
                return HttpNotFound();
            }
            return View(fAQ);
        }

        // POST: FAQs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FAQ fAQ = db.FAQs.Find(id);
            db.FAQs.Remove(fAQ);
            db.SaveChanges();
            Session["SweetAlertMessage"] = "FAQ was Deleted Successfully";
            Session["SweetAlertType"] = "success";
            Session["fromDelete"] = "true";
            return RedirectToAction("AdminFaqs");
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
