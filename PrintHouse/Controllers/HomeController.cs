using Microsoft.AspNet.Identity;
using PrintHouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PrintHouse.Controllers
{
    public class HomeController : Controller
    {
        PrintHouseEntities db = new PrintHouseEntities();
        public ActionResult Index()
        {
            if(User.Identity.IsAuthenticated) { 
            var userId = User.Identity.GetUserId();
            var cartCount = db.Carts.Where(x => x.userId == userId).Count();
            var idCookie = Request.Cookies["ProductIds"];
            var quantityCookie = Request.Cookies["ProductQuantity"];
                if (cartCount == 0 && idCookie != null && quantityCookie != null && !string.IsNullOrEmpty(idCookie.Value) && !string.IsNullOrEmpty(quantityCookie.Value))
                {
                    List<int> quantity = new List<int>();
                    quantity = quantityCookie.Value.Split(',').Select(int.Parse).ToList();
                    List<int> productIds = new List<int>();
                    productIds = idCookie.Value.Split(',').Select(int.Parse).ToList();
                    for (int i = 0; i < quantity.Count; i++)
                    {
                        Cart cart = new Cart();

                        cart.productId = productIds[i];
                        cart.userId = User.Identity.GetUserId();
                        cart.quantity = quantity[i];
                        cart.price = db.Products.Find(productIds[i]).productPrice;
                        cart.totalPrice = db.Products.Find(productIds[i]).productPrice * quantity[i];

                        db.Carts.Add(cart);
                        db.SaveChanges();
                    }
                    
                }
                idCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(idCookie);
                quantityCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(quantityCookie);



            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}