using Microsoft.Ajax.Utilities;
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
    public class CategoryTotalPriceModel
    {
        public int CategoryId { get; set; }
        public decimal TotalPrice { get; set; }
        public string CategoryName { get; set; }
    }
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
                    idCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(idCookie);
                    quantityCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(quantityCookie);
                    return View();

                }
                else if (cartCount > 0 && idCookie != null && quantityCookie != null && !string.IsNullOrEmpty(idCookie.Value) && !string.IsNullOrEmpty(quantityCookie.Value)) {
                    idCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(idCookie);
                    quantityCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(quantityCookie);
                    return View();
                }

            }
            return View();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Statistics()
        {
            ViewBag.productsSold = db.OrderDetails.Sum(x => x.quantity);
            ViewBag.totalSales = Convert.ToInt32(db.OrderDetails.Sum(x => x.price));
            ViewBag.Customers = db.Orders.Select(x => x.userId).Distinct().Count();

            //        var items  = db.OrderDetails
            //.Join(db.Products, od => od.productId, p => p.productId, (od, p) => new { od, p })
            //.Join(db.Categories, t => t.p.categoryId, c => c.categoryId, (t, c) => new { t.od, c.categoryId })
            //.GroupBy(t => t.categoryId)
            //.Select(g => new { CategoryId = g.Key, TotalPrice = g.Sum(x => x.od.price) }).ToList();
            var items = db.OrderDetails
     .Join(db.Products, od => od.productId, p => p.productId, (od, p) => new { od, p })
     .Join(db.Categories, t => t.p.categoryId, c => c.categoryId, (t, c) => new { t.od, Category = c })
     .GroupBy(t => t.Category.categoryId)
     .Select(g => new { CategoryId = g.Key, CategoryName = g.FirstOrDefault().Category.categoryName, TotalPrice = g.Sum(x => x.od.price) })
     .ToList();

            List<CategoryTotalPriceModel> result = new List<CategoryTotalPriceModel>();


            foreach (var item in items)
            {
                // Perform some calculations to determine the category ID and total price
                int categoryId = item.CategoryId;
                decimal totalPrice = Convert.ToDecimal(item.TotalPrice);
                string categoryName = item.CategoryName;

                // Create a new CategoryTotalPriceModel object and add it to the list
                CategoryTotalPriceModel model = new CategoryTotalPriceModel { CategoryId = categoryId, TotalPrice = totalPrice, CategoryName = categoryName };
                result.Add(model);
            }

            // Pass the list of CategoryTotalPriceModel objects to the view
            ViewBag.result = result;
            var stats = db.OrderDetails.ToList();
        return View(stats);
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