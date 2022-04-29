using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeterEngland2.Data;
using PeterEngland2.Models;
using System.Collections.Generic;
using System.Linq;

namespace PeterEngland2.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext context;
        public static Ordered od;
        List<Ordered> li = new List<Ordered>();
        public CustomerController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            var l = context.Inventry.ToList();
            ViewBag.Products = l;
            return View(l);
        }
        [HttpGet]
        public IActionResult Order(int ProductId) 
         {
              var p = context.Inventry.Where(p=> p.ProductId==ProductId).SingleOrDefault();
            return View(p);
        }
        [HttpPost]
        public IActionResult Order(int qty,int id) 
        {
            
                var prod = context.Inventry.Where(p => p.ProductId == id).SingleOrDefault();
            Ordered od = new Ordered();
            if (prod != null)
            {

                od.ProductName = prod.ProductName;
                od.ProductId = prod.ProductId;
                od.ProductQuntity = qty;
                od.ProductPrice = prod.ProductPrice;
                od.ProductTotalBill = od.ProductQuntity * od.ProductPrice;
              // ViewBag.Order=od;
                HttpContext.Session.SetString("data", JsonConvert.SerializeObject(od));
              
                return RedirectToAction("Cart");
            }
            return RedirectToAction("Cart");
            //return View();
        }

        [HttpGet]
        public IActionResult Cart() 
        {
            var  data = HttpContext.Session.GetString("data");
           Ordered o= JsonConvert.DeserializeObject<Ordered>(data);
            ViewBag.obj = o;
            return View(o);
        }
          [HttpPost]
          public IActionResult Cart(Ordered ordered) 
        {
            ordered.UserId= (int)HttpContext.Session.GetInt32("UserId");
            context.Ordered.Add(ordered);
            int r = context.SaveChanges();

            if (r == 1) 
            {
                ViewBag.OrderPlaced = "<script> alert('Order Placed!') </script>"; 
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.OrderPlaced = "<script> alert('Failed to placed!') </script>";
                return View();
            }
            
        }
        [HttpGet]
        public IActionResult ViewOrder() 
           {
            //int id = (int)HttpContext.Session.GetInt32("UserId");
            //int id = 2;
            int id = (int) HttpContext.Session.GetInt32("Id");
            var orderlist = context.Ordered.Where(o => o.UserId == id).ToList();

            return View(orderlist);

        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
