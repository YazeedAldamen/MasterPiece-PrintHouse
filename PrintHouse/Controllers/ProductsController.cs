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
    public class ProductsController : Controller
    {
        private PrintHouseEntities db = new PrintHouseEntities();

        // GET: Products
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                var products = db.Products.Include(p => p.Category);
                return View(products.ToList());

            }
            else if (id != null)
            {
                var products = db.Products.Where(x => x.subCategoryId == id).Include(p => p.Category);
                return View(products.ToList());
            }
            return View();
        }

        public ActionResult AdminProducts()
        {
            var products = db.Products.ToList();
            return View(products);
        }
        public ActionResult SingleProduct(int id)
        {
            var singleProduct = db.Products.Where(x => x.productId == id).FirstOrDefault();
            return View(singleProduct);
        }
        [HttpPost]
        public ActionResult SingleProduct(int id, int quantity)
        {
            var userId = User.Identity.GetUserId();
            int cartCount = db.Carts.Where(x=>x.userId == userId).Count();

            if (User.Identity.IsAuthenticated )
            {
                Cart cart = new Cart();
                cart.productId = id;
                cart.userId = User.Identity.GetUserId();
                cart.quantity = quantity;
                cart.price = db.Products.Find(id).productPrice;
                cart.totalPrice = db.Products.Find(id).productPrice * quantity;
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("SingleProduct", "Products", new { id = id });
            }
            else
            {

                List<int> productIds = GetProductIdsFromCookie();
                List<int> quantities = GetProductQuantityFromCookie();
                productIds.Add(id);
                quantities.Add(quantity);
                SetProductIdsInCookie(productIds);
                SetProductQuantityCookie(quantities);

                //List<int> productIds = GetProductIdsFromSession();
                //productIds.Add(id);
                //Session["ProductIds"] = productIds;

                //Session["productId"] = id;
                //Session["quantity"] = quantity;
                ////TempData["SweetAlertMessage"] = "You need to log in before you can add this product to your cart.";
                ////TempData["SweetAlertType"] = "warning";
                //TempData["SweetAlertMessage"] = "Item have been added to cart";
                //TempData["SweetAlertType"] = "warning";


                ViewBag.ProductIds = productIds;
                ViewBag.ProductQuantity = quantities;

                var singleProduct = db.Products.Where(x => x.productId == id).FirstOrDefault();
                return View(singleProduct);



            }


        }
        private List<int> GetProductIdsFromCookie()
        {
            List<int> productIds = new List<int>();

            HttpCookie cookie = Request.Cookies["ProductIds"];

            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                productIds = cookie.Value.Split(',').Select(int.Parse).ToList();
            }

            return productIds;
        }

        private List<int> GetProductQuantityFromCookie(){

            List<int> quantity = new List<int>();
            HttpCookie cookie = Request.Cookies["ProductQuantity"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                quantity = cookie.Value.Split(',').Select(int.Parse).ToList();
            }
            return quantity;
        }

        private void SetProductIdsInCookie(List<int> productIds)
        {
            string value = string.Join(",", productIds);

            HttpCookie cookie = new HttpCookie("ProductIds", value);
            cookie.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Add(cookie);


        }

        private void SetProductQuantityCookie(List<int> quantities){
        string value = string.Join(",", quantities);
            HttpCookie cookie = new HttpCookie("ProductQuantity", value);
            cookie.Expires= DateTime.Now.AddDays(1); 
            Response.Cookies.Add(cookie);
        }
        //private List<int> GetProductIdsFromSession()
        //{
        //    List<int> productIds = (List<int>)Session["ProductIds"];

        //    if (productIds == null)
        //    {
        //        productIds = new List<int>();
        //        Session["ProductIds"] = productIds;
        //    }

        //    return productIds;
        //}








        //public ActionResult Carts(int id)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        Cart cart = new Cart();
        //        cart.productId = id;
        //        cart.userId = User.Identity.GetUserId();
        //        cart.quantity = 1;
        //        cart.price = db.Products.Find(id).productPrice;
        //        cart.totalPrice = db.Products.Find(id).productPrice * 100;
        //        db.Carts.Add(cart);
        //        db.SaveChanges();
        //        return RedirectToAction("SingleProduct", "Products", new { id = id });
        //    }
        //    else
        //    {
        //        Session["productId"] = id;
        //        TempData["SweetAlertMessage"] = "You need to log in before you can add this product to your cart.";
        //        TempData["SweetAlertType"] = "warning";
        //        return RedirectToAction("Login", "Account");

        //    }

        //}


        //public ActionResult Carts(int id){ 
        //Cart cart = new Cart();
        //cart.productId = id;
        //cart.customerId = User.Identity.GetUserId();
        //cart.quantity = 1;
        //cart.price = db.Products.Find(id).productPrice;
        //cart.totalPrice = db.Products.Find(id).productPrice * 3;
        //    return View();
        //}


        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName");
            ViewBag.categories = db.Categories.ToList();
            ViewBag.subcategories = db.subCategories.ToList();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productId,productName,productDescription,productImage1,productImage2,productImage3,productPrice,categoryId,stock")] Product product,int subCategoryId ,HttpPostedFileBase productImage1, HttpPostedFileBase productImage2, HttpPostedFileBase productImage3)
        {
            if (ModelState.IsValid)
            {
                if (productImage1 != null && productImage1.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(productImage1.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    productImage1.SaveAs(path);
                    product.productImage1 = fileName;
                }
                if (productImage2 != null && productImage2.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(productImage2.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    productImage2.SaveAs(path);
                    product.productImage2 = fileName;
                }
                if (productImage3 != null && productImage3.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(productImage3.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    productImage3.SaveAs(path);
                    product.productImage3 = fileName;
                }
                product.subCategoryId = subCategoryId;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("AdminProducts");
            }

            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", product.categoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            ;
            Session["productImage1"] = product.productImage1;
            Session["productImage2"] = product.productImage2;
            Session["productImage3"] = product.productImage3;


            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", product.categoryId);
            ViewBag.categories = db.Categories.ToList();
            ViewBag.subcategories = db.subCategories.ToList();
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "productId,productName,productDescription,productImage1,productImage2,productImage3,productPrice,categoryId,stock")] Product product, int subCategoryId, HttpPostedFileBase productImage1, HttpPostedFileBase productImage2, HttpPostedFileBase productImage3)
        {
            if (ModelState.IsValid)
            {
                if (productImage1 != null && productImage1.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(productImage1.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    productImage1.SaveAs(path);
                    product.productImage1 = fileName;
                }
                else{
                    product.productImage1 = Session["productImage1"].ToString();

                }
                if (productImage2 != null && productImage2.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(productImage2.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    productImage2.SaveAs(path);
                    product.productImage2 = fileName;
                }
                else{
                    product.productImage2 = Session["productImage2"].ToString();

                }
                if (productImage3 != null && productImage3.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(productImage3.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/assets/img"), fileName);
                    productImage3.SaveAs(path);
                    product.productImage3 = fileName;
                }
                else
                {
                    product.productImage3 = Session["productImage3"].ToString();

                }
                product.subCategoryId = subCategoryId;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminProducts");
            }
            ViewBag.categoryId = new SelectList(db.Categories, "categoryId", "categoryName", product.categoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("AdminProducts");
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
