using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineClothStore.db;
using PagedList;
using PagedList.Mvc;

namespace OnlineClothStore.Controllers
{
    public class ProductsController : Controller
    {
        private OnlineClothStoreEntities db = new OnlineClothStoreEntities();

        // GET: Products
        public ActionResult Index(string searchBy, string search, int? page)
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

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            Session["Image2"] = product.Product_Image.ToString();
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_Id", "Category_Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create(Product product, HttpPostedFileBase Image)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        if (Image != null && Image.ContentLength <= 1000000)
        //        {
        //            string imagePath = Server.MapPath("~/images/") + Path.GetFileName(Image.FileName);
        //            Image.SaveAs(imagePath);
        //            product.Product_Image = "~/images/" + Path.GetFileName(Image.FileName);

        //        }
        //        db.Products.Add(product);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Category_ID = new SelectList(db.Categories, "Category_Id", "Category_Name", product.Category_ID);
        //    return View(product);
        //}
        public ActionResult Create(Product product, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(Image.FileName);
                    string extention = Path.GetExtension(Image.FileName);
                    
                    if (extention.ToLower() == ".jpg" || extention.ToLower() == ".png" || extention.ToLower() == ".jpeg")
                    {
                        
                        if (Image.ContentLength <= 1000000)
                        {
                            filename = filename + extention;
                            product.Product_Image = "~/HomeStyling/images/" + filename;
                            filename = Path.Combine(Server.MapPath("~/HomeStyling/images/"), filename);
                            Image.SaveAs(filename);
                            db.Products.Add(product);
                            int a = db.SaveChanges();
                            
                            if (a > 0)
                            {
                                TempData["InsertionMessage"] = "<script>alert('Data Inserted!!')</script>";
                                ModelState.Clear();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["FailedInsertionMessage"] = "<script>alert('Data does not Inserted!!')</script>";
                            }
                        
                        }
                        else
                        {
                            TempData["LengthSizeMessage"] = "<script>alert('Image Size should be less than 1MB!!')</script>";
                        
                        }
                    }
                    else
                    {
                        TempData["ExtentionMessage"] = "<script>alert('Format not Supported!!')</script>";
                    }
                
                }
                else
                {
                    TempData["NullMessage"] = "<script>alert('Image Required!!')</script>";
                }
            
            }

            ViewBag.Category_ID = new SelectList(db.Categories, "Category_Id", "Category_Name", product.Category_ID);
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
            Session["Image"] = product.Product_Image;
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_Id", "Category_Name", product.Category_ID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(Image.FileName);
                    string extention = Path.GetExtension(Image.FileName);

                    if (extention.ToLower() == ".jpg" || extention.ToLower() == ".png" || extention.ToLower() == ".jpeg")
                    {

                        if (Image.ContentLength <= 1000000)
                        {
                            filename = filename + extention;
                            product.Product_Image = "~/HomeStyling/images/" + filename;
                            filename = Path.Combine(Server.MapPath("~/HomeStyling/images/"), filename);
                            Image.SaveAs(filename);
                            db.Entry(product).State = EntityState.Modified;
                            int a = db.SaveChanges();

                            if (a > 0)
                            {
                                string ImagePath = Request.MapPath(Session["Image"].ToString());
                                if (System.IO.File.Exists(ImagePath))
                                {
                                    System.IO.File.Delete(ImagePath);
                                }
                                TempData["UpdationMessage"] = "<script>alert('Data Updated!!')</script>";
                                ModelState.Clear();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["FailedUpdationMessage"] = "<script>alert('Data does not Update!!')</script>";
                            }

                        }
                        else
                        {
                            TempData["LengthSizeMessage"] = "<script>alert('Image Size should be less than 1MB!!')</script>";

                        }
                    }
                    else
                    {
                        TempData["ExtentionMessage"] = "<script>alert('Format not Supported!!')</script>";
                    }

                }
                else
                {
                    product.Product_Image= Session["Image"].ToString();
                    db.Entry(product).State = EntityState.Modified;
                    int a = db.SaveChanges();

                    if (a > 0)
                    {
                        TempData["UpdationMessage"] = "<script>alert('Data Updated!!')</script>";
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["FailedUpdationMessage"] = "<script>alert('Data does not Update!!')</script>";
                    }
                }
            }
            ViewBag.Category_ID = new SelectList(db.Categories, "Category_Id", "Category_Name", product.Category_ID);
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
            int a = db.SaveChanges();
            if(a > 0)
            {
                string ImagePath = Request.MapPath(product.Product_Image.ToString());
                if(System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }
            }
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
