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
           

            var categoryInCarts = db.Carts.Where(x => x.Product.categoryId == id).ToList();
            var categoryInOrders = db.OrderDetails.Where(x => x.Product.categoryId == id).ToList();
            if (categoryInCarts.Count == 0 && categoryInOrders.Count == 0)
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                var subCategory = db.subCategories.Where(x => x.categoryId == id).ToList();
                foreach (var item in subCategory)
                {
                    db.subCategories.Remove(item);
                }
                var product = db.Products.Where(x => x.categoryId == id).ToList();
                foreach (var item in product)
                {
                    db.Products.Remove(item);
                }
                db.SaveChanges();

            }
            else
            {
                Session["SweetAlertMessage"] = "This category cannot be deleted as it's products is in the process of being fulfilled for customer orders.";
                Session["SweetAlertType"] = "success";
                Session["fromDelete"] = "true";
                return RedirectToAction("AdminCategories");
            }
            Session["SweetAlertMessage"] = "Category Have been deleted successfully";
            Session["SweetAlertType"] = "success";
            Session["fromDelete"] = "true";

           
            
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
