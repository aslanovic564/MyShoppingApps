using CmsShoppingCart.Areas.Admin.Models.Shop;
using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //declare alist of model
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                // int the page
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            //return view vith the model

            return View(categoryVMList);
        }


        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Declare id
            string id;

            using (Db db = new Db())
            {
                //Check  that the category name is unique

                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }
                //Int DTO
                CategoryDTO dto = new CategoryDTO();

                //Add to DTO
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;
                // Save DTO 
                db.Categories.Add(dto);
                db.SaveChanges();
                // Get the id
                id = dto.Id.ToString();
            }
            //Return id
            return id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {

            using (Db db = new Db())
            {
                //Set initial count
                int count = 1;

                //declare categoryDTO
                CategoryDTO dto;

                //set sorting foreach category
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;

                }

            }
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeleteCategory(int? id)
        {
            using (Db db = new Db())
            {
                //Get the pageDTO
                CategoryDTO dto = db.Categories.Find(id);

                //Remove the page 
                db.Categories.Remove(dto);

                //Save 
                db.SaveChanges();
            }
            //Redirect
            return RedirectToAction("Categories");
        }
        // POST: Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                //Check the category name is unique
                if (db.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }
                // Get DTO  
                CategoryDTO dto = db.Categories.Find(id);
                //edit DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();
                //Save 
                db.SaveChanges();
            }
            //Return
            return "ok";
        }
        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Int model
            ProductVM model = new ProductVM();
            using (Db db = new Db())
            {
                //add selectlist of categories to model
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            }
            //return view vith the model
            return View(model);
        }
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }
            //Make sure producte name is unique

            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That producr name is taken!!!!");
                    return View(model);
                }
            }
            //declare product id
            int id;
            //int and save productDTO
            using (Db db = new Db())
            {
                ProductDTO dto = new ProductDTO();

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Price = model.Price;
                dto.Description = model.Description;
                dto.CategoryDTOId = model.CategoryDTOId;

                CategoryDTO catDto = db.Categories.FirstOrDefault(x => x.Id == model.CategoryDTOId);
                dto.CategoryName = catDto.Name;
                db.Products.Add(dto);
                db.SaveChanges();
                //Get insert id
                id = dto.Id;
            }
            //Set tempdata message
            TempData["SM"] = "You have added product";

            #region Upload Image
            //Create neecessary direction
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);
            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);
            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);
            //Check if file was uploaded
            if (file != null && file.ContentLength > 0)
            {

                //Get file extensien
                string ext = file.ContentType.ToLower();
                //verifay extension  
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not  uploaded--Wrong image exentions");
                        return View(model);
                    }

                }
                //init image name
                string imagename = file.FileName;
                //save image name to DTO
                using (Db db = new Db())
                {
                    ProductDTO proddto = db.Products.Find(id);
                    proddto.ImageName = imagename;
                    db.SaveChanges();
                }
                //St original and thumb image paths 
                var path = string.Format("{0}\\{1}", pathString2, imagename);
                var path2 = string.Format("{0}\\{1}", pathString3, imagename);
                //Save original 
                file.SaveAs(path);
                //Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            #endregion

            //redirect
            return RedirectToAction("AddProduct");
        }

        public ActionResult Products(int? page, int? catId)
        {

            //Declare a list of ProductVM
            List<ProductVM> listOfProductVM;

            //Set the page number 
            var pageNumber = page ?? 1;
            using (Db db = new Db())
            {
                //init the llist
                listOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryDTOId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();


                //populate The categories select list
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                //set selected category
                ViewBag.SelectedCat = catId.ToString();
                //Set Pagination
                var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
                ViewBag.OnePageOfProducts = onePageOfProducts;
            }
            //reedirect
            return View();
        }

        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            //Declare productVM
            ProductVM model;
            using (Db db = new Db())
            {
                //Get the product
                ProductDTO dto = db.Products.Find(id);
                //Make sure product exists
                if (dto == null)
                {
                    return Content("That product does not exists ..");
                }
                //Init  model
                model = new ProductVM(dto);
                //Make a select list
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                //Get all galery images
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                                    .Select(b => Path.GetFileName(b));
            }

            //Return View with the model

            return View(model);
        }

        // POST: Admin/Shop/EditProduct/id
       [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            // Get product id
            int id = model.Id;

            // Populate categories select list and gallery images
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                .Select(fn => Path.GetFileName(fn));

            // Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }

            // Update product
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryDTOId = model.CategoryDTOId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryDTOId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "You have edited the product!";

            #region Image Upload

            // Check for file upload
            if (file != null && file.ContentLength > 0)
            {

                // Get extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                        return View(model);
                    }
                }

                // Set uplpad directory paths
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // Delete files from directories

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                    file2.Delete();

                foreach (FileInfo file3 in di2.GetFiles())
                    file3.Delete();

                // Save image name

                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Save original and thumb images

                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion

            // Redirect
            return RedirectToAction("EditProduct");
        }

        // GET: Admin/Shop/DeleteProduct/id
        public ActionResult DeleteProduct(int id)
        {
            // Delete product from DB
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);

                db.SaveChanges();
            }
            //Delete product folder
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);
            //redirect
            return RedirectToAction("Products");
        }
        //POST//admin/Shop/SaveGalleryImage
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Loop through files
            foreach (string fileName in Request.Files)
            {
                // Init the file
                HttpPostedFileBase file = Request.Files[fileName];

                // Check it's not null
                if (file != null && file.ContentLength > 0)
                {
                    // Set directory paths
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // Set image paths
                    var path = string.Format("{0}\\{1}", pathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", pathString2, file.FileName);

                    // Save original and thumb

                    file.SaveAs(path);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);
                }

            }

        }
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName); 
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);
            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
        }
        
        //GET /Admin/Orders
        public ActionResult Orders()
        {
            //Init list ordersforAdmin
            List<OrdersForAdmin> ordersForAdmin = new List<OrdersForAdmin>();
            using (Db db =new Db())
            {
                //Init List orderVM
                List<OrderVM> orders = db.Orders.ToArray().Select(x => new OrderVM(x)).ToList();
                //loop Trought list of orderVM

                foreach (var order in orders)
                {
                    //init product dictionary
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                    //declare total
                    decimal total = 0;

                    //init list of orderDetilsDTO
                    List<OrderDetailsDTO> orderDetailsList = db.OrderDetails.Where(x => x.OrdersId == order.OrderId).ToList();

                    //Get userName
                    UserDTO user = db.Users.Where(x => x.Id == order.UserId).FirstOrDefault();
                    string username = user.Username;
                    //loop through OrderdetalisDTO
                    foreach (var orderDetails in orderDetailsList)
                    {
                        //get product 
                        ProductDTO product = db.Products.Where(x => x.Id == orderDetails.ProductsId).FirstOrDefault();
                        //get product price 
                        decimal price = product.Price;

                        //get product name 
                        string productName = product.Name;

                        //add to product dictionary
                        productsAndQty.Add(productName, orderDetails.Quantity);
                        //get total
                        total += orderDetails.Quantity * price;
                    }
                    ordersForAdmin.Add(new OrdersForAdmin() {
                        OrderNumber = order.OrderId,
                        Username = username,
                        Total = total,
                        ProductsAndQty=productsAndQty,
                        CreatedAt=order.CreatedAt

                    });
                }
            }

            //return View With the model
            return View(ordersForAdmin);
        }

    }
}