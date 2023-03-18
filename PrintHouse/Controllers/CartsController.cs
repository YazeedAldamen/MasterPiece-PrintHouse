﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            ViewBag.userId = id;
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
        public async Task<ActionResult> CheckOut(string Email, string customerPhone, string address, int postalCode, string city)
        {
            Shipping ship = new Shipping();
            Order order = new Order();
            OrderDetail orderDetail = new OrderDetail();

            var id = User.Identity.GetUserId();
            var shippingInfo = db.Shippings.Where(x => x.userId == id).FirstOrDefault();
            var user = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
            var cart = db.Carts.Where(x => x.userId == id).ToList();

            decimal totalAmount = 0;

            foreach (var item in cart)
            {
                totalAmount += Convert.ToDecimal(item.totalPrice);

            }


            ship.userId = id;
            ship.address = address;
            ship.postalCode = postalCode;
            ship.email = Email;
            ship.firstName = user.customerFirstName;
            ship.lastName = user.customerLastName;
            ship.city = city;
            ship.phone = customerPhone;

            if (shippingInfo == null)
            {
                db.Shippings.Add(ship);

            }
            await db.SaveChangesAsync();

            var orderShipping = db.Shippings.Where(x => x.userId == id).FirstOrDefault();
            order.shippingId = orderShipping.shippingId;
            order.orderDate = DateTime.Now;
            order.totalAmount = totalAmount;
            order.userId = id;
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var orderDetailOrder = db.Orders.Where(x => x.userId == id).OrderByDescending(x=>x.orderId).FirstOrDefault();
            foreach (var item in cart)
            {
                
                orderDetail.orderId = orderDetailOrder.orderId;
                orderDetail.productId = item.productId;
                orderDetail.quantity = item.quantity;
                orderDetail.price = item.price * item.quantity;
                db.OrderDetails.Add(orderDetail);
                await db.SaveChangesAsync();

                db.Carts.Remove(item);
            }
            await db.SaveChangesAsync();

            return RedirectToAction("Index","Home");

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
