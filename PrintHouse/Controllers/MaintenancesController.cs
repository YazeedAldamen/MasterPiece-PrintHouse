using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PrintHouse.Models;

namespace PrintHouse.Controllers
{
    public class MaintenancesController : Controller
    {
        private PrintHouseEntities db = new PrintHouseEntities();

        // GET: Maintenances
        public ActionResult Index()
        {
            var maintenances = db.Maintenances.Include(m => m.AspNetUser).Include(m => m.Product);
            return View(maintenances.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult MaintenancesRequests()
        {
            var maintenances = db.Maintenances.Where(m => m.done == false || m.done == null).Include(m => m.AspNetUser).Include(m => m.Product).OrderBy(m=>m.orderDate);
            return View(maintenances.ToList());
        }
        [Authorize(Roles = "Admin")]
        public ActionResult DoneMaintenances()
        {
            var maintenances = db.Maintenances.Where(m => m.done == true).Include(m => m.AspNetUser).Include(m => m.Product);
            return View(maintenances.ToList());
        }

        // GET: Maintenances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maintenance maintenance = db.Maintenances.Find(id);
            if (maintenance == null)
            {
                return HttpNotFound();
            }
            return View(maintenance);
        }

        // GET: Maintenances/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.productId = new SelectList(db.Products, "productId", "productName");
            return View();
        }

        // POST: Maintenances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "maintenanceId,orderDate,userId,maintencanceOrderDetails,productId")] Maintenance maintenance)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                maintenance.orderDate = DateTime.Now;
                maintenance.done = false;
                maintenance.userId = userId;
                db.Maintenances.Add(maintenance);
                db.SaveChanges();
                Session["SweetAlertMessage"] = "Your Request Has been Submitted";
                Session["SweetAlertType"] = "success";
                Session["fromDelete"] = "true";
                return RedirectToAction("Create");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", maintenance.userId);
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", maintenance.productId);
            return View(maintenance);
        }

        //// GET: Maintenances/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Maintenance maintenance = db.Maintenances.Find(id);
        //    if (maintenance == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", maintenance.userId);
        //    ViewBag.productId = new SelectList(db.Products, "productId", "productName", maintenance.productId);
        //    return View(maintenance);
        //}

        // POST: Maintenances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int maintenanceId, DateTime orderDate, string maintencanceOrderDetails, string done, int productId, string userId)
        {
            Maintenance maintenance = new Maintenance();

            if (ModelState.IsValid)
            {
                maintenance.maintenanceId = maintenanceId;
                maintenance.userId = userId;
                maintenance.done = Convert.ToBoolean(done);
                maintenance.orderDate = orderDate;
                maintenance.maintencanceOrderDetails = maintencanceOrderDetails;
                maintenance.productId = productId;


                db.Entry(maintenance).State = EntityState.Modified;
                db.SaveChanges();
                Session["SweetAlertMessage"] = "The request status has been changed to finished";
                Session["SweetAlertType"] = "success";
                Session["fromDelete"] = "true";
            }
            return RedirectToAction("MaintenancesRequests");

        }
        public ActionResult ReopenRequest(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maintenance maintenance = db.Maintenances.Find(id);
            if (maintenance == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", maintenance.userId);
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", maintenance.productId);
            return View(maintenance);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReopenRequest(int maintenanceId, DateTime orderDate, string maintencanceOrderDetails, string done, int productId, string userId)
        {
            Maintenance maintenance = db.Maintenances.Where(x => x.maintenanceId == maintenanceId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                maintenance.maintenanceId = maintenanceId;
                maintenance.userId = userId;
                maintenance.done = false;
                maintenance.orderDate = orderDate;
                maintenance.maintencanceOrderDetails = maintencanceOrderDetails;
                maintenance.productId = productId;


                db.Entry(maintenance).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("MaintenancesRequests");

        }
        // GET: Maintenances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maintenance maintenance = db.Maintenances.Find(id);
            if (maintenance == null)
            {
                return HttpNotFound();
            }
            return View(maintenance);
        }

        // POST: Maintenances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Maintenance maintenance = db.Maintenances.Find(id);
            db.Maintenances.Remove(maintenance);
            db.SaveChanges();
            Session["SweetAlertMessage"] = "The Record has been deleted successfully";
            Session["SweetAlertType"] = "success";
            Session["fromDelete"] = "true";
            return RedirectToAction("MaintenancesRequests");
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
