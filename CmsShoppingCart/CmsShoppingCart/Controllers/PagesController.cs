using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index{Pages}
        public ActionResult Index(string page="")
        {
            //get set page slug
            if (page == "")
                page = "home";
            //declare model andd DTO
            PageVM model;
            PageDTO dto;
            //check page exists
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }
            //get page DTO
            using (Db db=new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }
            //Set page title
            ViewBag.PageTitle = dto.Title;
            //check for sidebar
            if (dto.HasSidebar==true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }
            //init model
            model = new PageVM(dto);
            //Return view with the model
            return View(model);
        }

      
        public ActionResult PagesMenuPartial()
        {
            // Declare a list of PageVM
            List<PageVM> pageVMList;

            // Get all pages except home
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting).Where(x => x.Slug != "home").Select(x => new PageVM(x)).ToList();
            }
            // Return partial view with list
            return PartialView(pageVMList);
        }
        public ActionResult SidebarPartial()
        {
            //Declare  SidebarVM model
            SidebarVM model;

            //init model
            using (Db db=new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);
                model = new SidebarVM(dto);
            }
            //Return partialview with model

            return PartialView(model);
        }

    }
}