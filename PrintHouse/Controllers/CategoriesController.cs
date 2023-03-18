using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PrintHouse.Models;

namespace PrintHouse.Controllers
{
    public class CategoriesController : Controller
    {
        private PrintHouseEntities db = new PrintHouseEntities();

        // GET: Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        [Authorize(Roles = "Admin")]

        public ActionResult AdminCategories(){
            return View(db.Categories.ToList());
        }
        [Authorize(Roles = "Admin")]

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [Authorize(Roles = "Admin")]

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "categoryId,categoryName,categoryDescription")] Category category, HttpPostedFileBase categoryImage)
        {
            if (ModelState.IsValid)
            {
                if (categoryImage != null && categoryImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(categoryImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    categoryImage.SaveAs(path);
                    category.categoryImage = fileName;
                }
                db.Categories.Add(category);
                
                db.SaveChanges();
                return RedirectToAction("AdminCategories", "Categories", category);
            }
            
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin")]

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            Session["categoryImage"] = category.categoryImage;
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "categoryId,categoryName,categoryDescription,categoryImage")] Category category, HttpPostedFileBase categoryImage)
        {
            if (ModelState.IsValid)
            {
                if (categoryImage != null && categoryImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(categoryImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    categoryImage.SaveAs(path);
                    category.categoryImage = fileName;
                }
                else{
                    category.categoryImage = Session["categoryImage"].ToString();
                }

                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminCategories");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("AdminCategories", "Categories");
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
