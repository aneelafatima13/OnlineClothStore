using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineClothStore.db;
using PagedList;
using PagedList.Mvc;


namespace OnlineClothStore.Controllers
{
    public class HomeController : Controller
    {
        OnlineClothStoreEntities db = new OnlineClothStoreEntities();
        public ActionResult Index()
        {

            List<Category> categories = db.Categories.ToList();
            return View(categories);


        }

        public ActionResult Shop(string searchBy, string search, int? page)
        {

            int pageSize = 10; // Number of items per page
            int pageNumber = (page ?? 1); // Current page number, default to 1 if not provided

            if (searchBy == "Product_Name")
            {
                var products = db.Products.Where(x => x.Product_Name.StartsWith(search)).ToList();
                var pagedData = products.ToPagedList(pageNumber, pageSize);
                return View(pagedData);
            }
            else if (searchBy == "Min_Product_Price")
            {
                int searchPrice;
                if (int.TryParse(search, out searchPrice))
                {
                    var products = db.Products.Where(x => x.Product_Price <= searchPrice).ToList();
                    var pagedData = products.ToPagedList(pageNumber, pageSize);
                    return View(pagedData);
                }
                else
                {
                    // Handle the case where parsing fails, e.g., return a view with an error message.
                    ViewBag.ErrorMessage = "Invalid age input. Please enter a valid number.";
                    return View();
                }

            }
            else if (searchBy == "Max_Product_Price")
            {
                int searchPrice;
                if (int.TryParse(search, out searchPrice))
                {
                    var products = db.Products.Where(x => x.Product_Price >= searchPrice).ToList();
                    var pagedData = products.ToPagedList(pageNumber, pageSize);
                    return View(pagedData);
                }
                else
                {
                    // Handle the case where parsing fails, e.g., return a view with an error message.
                    ViewBag.ErrorMessage = "Invalid age input. Please enter a valid number.";
                    return View();
                }

            }
            else if (searchBy == "Category_Name")
            {
                var products = db.Products.Where(x => x.Category.Category_Name.StartsWith(search)).ToList();
                var pagedData = products.ToPagedList(pageNumber, pageSize);
                return View(pagedData);
            }
            else
            {
                var products = db.Products.ToList();
                var pagedData = products.ToPagedList(pageNumber, pageSize);
                return View(pagedData);
            }
        }



        public ActionResult WhyUs()
        {
            return View();
        }

        public ActionResult Testimonial()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [Authorize]
        public ActionResult CartDetails()
        {
            return View();
        }
        public ActionResult AddtoCart(int ProductId, string url)
        {
            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();                             // For new item when cart is null
                Item item = new Item();
                item.Product = db.Products.Find(ProductId);
                item.Quantity = 1;
                cart.Add(item);
                //cart.Add(new Item() { Product = db.Products.Find(ProductId), Quantity = 1 });
                TempData["CartAddedMessage"] = "Your Product has been added!";
                Session["cart"] = cart;
            }
            else
            {

                List<Item> cart = (List<Item>)Session["cart"];                  // check item exist in cart already
                int index = IsInCart(ProductId);
                if (index != -1)                                                 // if yes
                {
                    var product = db.Products.Find(ProductId);
                    if (cart[index].Quantity + 1 <= product.Product_Quantity)   // check item q. is less then items actual q.
                    {
                        cart[index].Quantity++;                                 // if yes increase q.
                        TempData["CartAddedMessage"] = "Your Product has been added";
                    }
                    else
                    {
                        // if no Show an "Out of Stock" message to the user
                        TempData["OutOfStockMessage"] = "This product is out of Stock!!";
                        return RedirectToAction(url);
                    }


                }
                else
                {
                    var product = db.Products.Find(ProductId);                  // if second item is new not previous 
                    if (product.Product_Quantity >= 1)                          // check its q. >1 then add 1
                    {
                        cart.Add(new Item() { Product = product, Quantity = 1 });
                        TempData["CartAddedMessage"] = "Your Product has been added!";
                    }
                    else
                    {
                        // Show an "Out of Stock" message to the user          
                        TempData["OutOfStockMessage"] = "This product is out of Stock!!";
                        return RedirectToAction(url);
                    }

                }
                Session["cart"] = cart;
            }
            return RedirectToAction(url);
        }

        public int IsInCart(int ProductId)
        {                                                       // to check item already in the cart or no
            List<Item> cart = (List<Item>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)                 // compare selected item id to all the products ids
            {
                if (cart[i].Product.Product_Id == ProductId)    // if found return its id
                {
                    return i;
                }
            }
            return -1;                                          // else -1
        }

        public ActionResult RemoveFromcart(int ProductId)
        {
            List<Item> cart = (List<Item>)Session["cart"];

            int index = IsInCart(ProductId);
            if (index != -1)                                // if item exist then remove 
            {
                cart.RemoveAt(index);
                Session["cart"] = cart;
            }

            return RedirectToAction("CartDetails");
        }
        public ActionResult DecreaseQty(int productId)
        {
            if (Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                int index = IsInCart(productId);                // check item exist
                if (index != -1)                                // if yes
                {
                    //var product = db.Products.Find(productId);
                    if (cart[index].Quantity > 0)               // check user demand quantity > 0
                    {
                        cart[index].Quantity--;                 // - it
                    }
                    else
                    {
                        cart.RemoveAt(index);                   // otherwise remove it
                    }
                }
                //List<Item> cart = (List<Item>)Session["cart"];
                //var product = db.Products.Find(productId);
                //foreach (var item in cart)
                //{
                //    if (item.Product.Product_Id == productId)
                //    {
                //        int prevQty = item.Quantity;
                //        if (prevQty > 0)
                //        {
                //            cart.Remove(item);
                //            cart.Add(new Item()
                //            {
                //                Product = product,
                //                Quantity = prevQty - 1
                //            });
                //        }
                //        break;
                //    }
                //}
                Session["cart"] = cart;
            }
            return Redirect("CartDetails");
        }

    }

}