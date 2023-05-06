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
    public class FeaturedsController : Controller
    {
        private PrintHouseEntities db = new PrintHouseEntities();

        // GET: Featureds
        public ActionResult Index()
        {
            var featureds = db.Featureds.Include(f => f.Product);
            return View(featureds.ToList());
        }
        [Authorize(Roles ="Admin")]
        public ActionResult AdminFeatured(){
            var featureds = db.Featureds.Include(f => f.Product);
            return View(featureds.ToList());
        }
        // GET: Featureds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Featured featured = db.Featureds.Find(id);
            if (featured == null)
            {
                return HttpNotFound();
            }
            return View(featured);
        }

        // GET: Featureds/Create
        [Authorize(Roles = "Admin")]

        public ActionResult Create()
        {
            ViewBag.productId = new SelectList(db.Products, "productId", "productName");
            return View();
        }

        // POST: Featureds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public ActionResult Create([Bind(Include = "featuredId,productId")] Featured featured)
        {
            if (ModelState.IsValid)
            {
                db.Featureds.Add(featured);
                db.SaveChanges();
                Session["SweetAlertMessage"] = "Featured Item was Added Successfully";
                Session["SweetAlertType"] = "success";
                Session["fromDelete"] = "true";
                return RedirectToAction("AdminFeatured");
            }

            ViewBag.productId = new SelectList(db.Products, "productId", "productName", featured.productId);
            return View(featured);
        }

        // GET: Featureds/Edit/5
        [Authorize(Roles = "Admin")]

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Featured featured = db.Featureds.Find(id);
            if (featured == null)
            {
                return HttpNotFound();
            }
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", featured.productId);
            return View(featured);
        }

        // POST: Featureds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "featuredId,productId")] Featured featured)
        {
            if (ModelState.IsValid)
            {
                db.Entry(featured).State = EntityState.Modified;
                db.SaveChanges();
                Session["SweetAlertMessage"] = "Featured Item was Edited Successfully";
                Session["SweetAlertType"] = "success";
                Session["fromDelete"] = "true";
                return RedirectToAction("AdminFeatured");
            }
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", featured.productId);
            return View(featured);
        }

        // GET: Featureds/Delete/5
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Featured featured = db.Featureds.Find(id);
            if (featured == null)
            {
                return HttpNotFound();
            }
            return View(featured);
        }

        // POST: Featureds/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Featured featured = db.Featureds.Find(id);
            db.Featureds.Remove(featured);
            db.SaveChanges();
            Session["SweetAlertMessage"] = "Featured Item was Deleted Successfully";
            Session["SweetAlertType"] = "success";
            Session["fromDelete"] = "true";
            return RedirectToAction("AdminFeatured");
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
