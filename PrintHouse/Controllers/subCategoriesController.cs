using System;
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
    public class subCategoriesController : Controller
    {
        private PrintHouseEntities db = new PrintHouseEntities();

        // GET: subCategories
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                var subCategories = db.subCategories.Include(s => s.Category);
                return View(subCategories.ToList());
            }
            else if (id != null)
            {
                var subCategories = db.subCategories.Where(x => x.categoryId == id).Include(s => s.Category);
                return View(subCategories.ToList());
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult AdminsubCategories()
        {
            return View(db.subCategories.ToList());
        }

        // GET: subCategories/Details/5
        [Authorize(Roles = "Admin")]

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subCategory subCategory = db.subCategories.Find(id);
            
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            return View(subCategory);
        }





        // GET: subCategories/Create
        [Authorize(Roles = "Admin")]

        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName");
            return View();
        }

        // POST: subCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "subCategoryId,subCategoryName,subCategoryDescription,categoryId")] subCategory subCategory, HttpPostedFileBase subCategoryImage)
        {
            if (ModelState.IsValid)
            {
                if (subCategoryImage != null && subCategoryImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(subCategoryImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    subCategoryImage.SaveAs(path);
                    subCategory.subCategoryImage = fileName;
                }
                db.subCategories.Add(subCategory);
                db.SaveChanges();
                return RedirectToAction("AdminsubCategories", "subCategories", subCategory);
            }

            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", subCategory.categoryId);
            return View(subCategory);
        }

        // GET: subCategories/Edit/5
        [Authorize(Roles = "Admin")]

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subCategory subCategory = db.subCategories.Find(id);
            Session["categoryImage"] = subCategory.subCategoryImage;
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", subCategory.categoryId);
            ViewBag.categories = db.Categories.ToList();

            return View(subCategory);
        }

        // POST: subCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "subCategoryId,subCategoryName,subCategoryImage,subCategoryDescription,categoryId")] subCategory subCategory, HttpPostedFileBase subCategoryImage)
        {
            if (ModelState.IsValid)
            {
                if (subCategoryImage != null && subCategoryImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(subCategoryImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    subCategoryImage.SaveAs(path);
                    subCategory.subCategoryImage = fileName;
                }
                else
                {
                    subCategory.subCategoryImage = Session["categoryImage"].ToString();
                }
                db.Entry(subCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminsubCategories");
            }
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", subCategory.categoryId);
            return View(subCategory);
        }

        // GET: subCategories/Delete/5
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subCategory subCategory = db.subCategories.Find(id);
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            return View(subCategory);
        }

        // POST: subCategories/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            subCategory subCategory = db.subCategories.Find(id);
            db.subCategories.Remove(subCategory);
            db.SaveChanges();
            return RedirectToAction("AdminsubCategories","subCategories");
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
