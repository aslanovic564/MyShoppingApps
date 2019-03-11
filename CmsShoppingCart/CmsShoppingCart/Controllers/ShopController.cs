using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Cart;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index","Pages");
        }
        // GET: Shop/CategoryMenuPartial
        public ActionResult CategoryMenuPartial()
        {
            //declare list of categoryVM

            List<CategoryVM> categoryVMList;

            //init the list
            using (Db db=new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            //Return partial view with the list
            return PartialView(categoryVMList);
        }
        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            //Declare alist of ProductVM
            List<ProductVM> productVMList;

            using (Db db=new Db())
            {
                //get category id
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;
                //init the list
                productVMList = db.Products.ToArray().Where(x => x.CategoryDTOId == catId).Select(x => new ProductVM(x)).ToList();

                //get the category name
                var productCat = db.Products.Where(x => x.CategoryDTOId == catId).FirstOrDefault();
                ViewBag.categoryName = productCat.CategoryName;


            }
            //return view with the list
            return View(productVMList);

        }
        // GET: /shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //Declare the VM ab DTO 
            ProductDTO dto;
            ProductVM model;
            //Init product id
            int id = 0;
            using (Db db = new Db()) 
            {
                //Ckeck if product exists
                if (!db.Products.Any(x=>x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }
                //Init product DTO
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();
                //get id 
                id = dto.Id;
                //init model
                model = new ProductVM(dto);

            }
            //get galery images 
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(b => Path.GetFileName(b));
            //return view with the model
            return View("ProductDetails", model);

        }
      
    }
}