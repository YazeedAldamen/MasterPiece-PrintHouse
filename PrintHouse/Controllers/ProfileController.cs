﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PrintHouse.Models;

namespace PrintHouse.Controllers
{
    public class ProfileController : Controller
    {
        private PrintHouseEntities db = new PrintHouseEntities();

        // GET: Profile
        public ActionResult Index()
        {
            return View(db.AspNetUsers.ToList());
        }

        // GET: Profile/Details/5
        [Authorize(Roles = "Admin")]

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            Session["customerImage"] = aspNetUser.customerImage;
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        [Authorize(Roles = "Customer")]
        public ActionResult CustomerProfile(string id){
        ViewBag.id = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(User.Identity.GetUserId() != id){
                return RedirectToAction("Index", "Home");
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            Session["customerImage"] = aspNetUser.customerImage;
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }


        [Authorize(Roles ="Admin")]
        public ActionResult AdminUsers(){

            var adminUsers = db.AspNetUsers.ToList();

            return View(adminUsers);
        }


        // GET: Profile/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Profile/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,customerFirstName,customerLastName,customerPhone")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetUser);
        }

        // GET: Profile/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Profile/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,customerFirstName,customerLastName,customerPhone")] AspNetUser aspNetUser, HttpPostedFileBase customerImage)
        {
            if (ModelState.IsValid)
            {
                if (customerImage != null && customerImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(customerImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    customerImage.SaveAs(path);
                    aspNetUser.customerImage = fileName;
                }
                else
                {
                    aspNetUser.customerImage = Session["customerImage"].ToString();

                }
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                if (User.IsInRole("Admin"))
                {
                    Session["SweetAlertMessage"] = "Your information has been changed";
                    Session["SweetAlertType"] = "success";
                    Session["fromDelete"] = "true";
                    return RedirectToAction("Details", "Profile", new { id = User.Identity.GetUserId() });

                }
                else if (User.IsInRole("Customer")){
                    Session["SweetAlertMessage"] = "Your information has been changed";
                    Session["SweetAlertType"] = "success";
                    Session["fromDelete"] = "true";
                    return RedirectToAction("CustomerProfile", "Profile", new { id = User.Identity.GetUserId() });

                }
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: Profile/Delete/5
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Profile/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
