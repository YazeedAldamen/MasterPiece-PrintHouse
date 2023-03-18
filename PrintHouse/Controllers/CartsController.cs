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
    public class CartsController : Controller
    {
        private PrintHouseEntities db = new PrintHouseEntities();

        // GET: Carts
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            var carts = db.Carts.Where(x=>x.userId == id).Include(c => c.AspNetUser).Include(c => c.Product);
            return View(carts.ToList());
        }

        // GET: Carts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: Carts/Create
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.productId = new SelectList(db.Products, "productId", "productName");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cartId,userId,productId,quantity,price,totalPrice")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", cart.userId);
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", cart.productId);
            return View(cart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CartEdit( int quantity, int cartId, string userId, int productId, decimal price, decimal totalPrice)
        {
            //cartId = Convert.ToInt32(cartId);
            // quantity = int.Parse(Request.Form["quantity_" + cartId]);
            // userId = Request.Form["userId_" + cartId];
            //quantity = int.Parse(Request.Form["productId_" + cartId]);
            // price = decimal.Parse(Request.Form["price_" + cartId]);
            // totalPrice = decimal.Parse(Request.Form["totalPrice_" + cartId]);
            var cart = db.Carts.Where(x => x.cartId == cartId).FirstOrDefault();
            if (ModelState.IsValid)
            {
                
                cart.userId = userId;
                cart.productId = productId;
                cart.price = price;
                cart.quantity = quantity;
                cart.totalPrice = quantity * price;
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", cart.userId);
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", cart.productId);
            return View("Index","Carts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(int quantity, int cartId, string userId, int productId, decimal price, decimal totalPrice)
        {

            
            return View("Index", "Home");

        }


        // GET: Carts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", cart.userId);
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", cart.productId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cartId,userId,productId,quantity,price,totalPrice")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", cart.userId);
            ViewBag.productId = new SelectList(db.Products, "productId", "productName", cart.productId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
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
