using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declare list of PageVM
            List<PageVM> pagesList;
            using (Db db = new Db())
            {
                //Int the list

                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            // Return View vith list
            return View(pagesList);
        }


        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }


        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Check model satate
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //Declare slug
                string slug;

                //Int pageDTO

                PageDTO dto = new PageDTO();
                //Dto title
                dto.Title = model.Title;

                //check for and set slug if need be 
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                //make sure title and slug are unique
                if (db.Pages.Any(x=>x.Title==model.Title)||db.Pages.Any(x=>x.Slug==slug))
                {
                     ModelState.AddModelError(" ", "That title or slug already exists");
                    return View(model);
                }
                //DTO the rest
                dto.Slug = slug;
                dto.Sorting =100; 
                dto.HasSidebar = model.HasSidebar;
                dto.Body = model.Body;
                //save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            // set Tempdata message 
            TempData["SM"] = "You have added a new page";
            //Redirect
            return RedirectToAction("AddPage");
        }


        // GET: Admin/Pages/EditPage/id
        public ActionResult EditPage(int? id)
        {
            //Declare Page
            PageVM model;

            using (Db db=new Db())
            {
               
                //Get the page 
                PageDTO dto = db.Pages.Find(id);
                //Confirm page exists
                if (dto == null)
                {
                    return Content("The page does not exists");
                }
                //int pageVM
                model = new PageVM(dto);
            }
            //Return view with model
            return View(model);
        }
        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //check model state 
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db= new Db())
            {
                // Get page id  
                int id = model.Id;
                // Declare slug 
                string slug="home";
                // Get the page  
                PageDTO dto = db.Pages.Find(id);
                //DTO the title
                dto.Title = model.Title;
                //Check for slug and set it if need be
                if (model.Slug!="home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                //Make sure Title and Slug arev unique
                if (db.Pages.Where(x=>x.Id!=id).Any(x=>x.Title==model.Title)||
                    db.Pages.Where(x=>x.Id!=id).Any(x=>x.Slug==slug))
                {
                    ModelState.AddModelError(" ", "That title or slug already exists");
                    return View(model);

                }
                //DTO the rest 
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Save the DTO
                db.SaveChanges();
            }
            //Set TempDate message
            TempData["SM"] = "You have edit the page!";  
            //Redirect
            return RedirectToAction("AddPage");
        }
        // GET: Admin/Pages/pageDetalis/id
        public ActionResult PageDetails(int? id)
        {
            // Declare PageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get the page
                PageDTO dto = db.Pages.Find(id);
                //Confirm Page exists
                if (dto == null)
                {
                    return Content("That page does not existss");
                }
                //Int PageVM
                model = new PageVM(dto);
            }
            //Return view with model
            return View(model);
        }
        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int? id)
        {
            using (Db  db=new Db())
            {
                // Get the page
                PageDTO dto=db.Pages.Find(id);

                // Remove the page
                db.Pages.Remove(dto);

                // Save
                db.SaveChanges();

            }
            //Redirect
            return RedirectToAction("Index");

        }
        // POST: Admin/Pages/ReorderPages/id
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db=new Db())
            {
                //set initial count 
                int count = 1;
                //declare pageDTO
                PageDTO dto;
                //set sorting foreach page 
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }
        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // declare model
            SidebarVM model;
            using (Db db=new Db())
            {
                //get the dto
                SidebarDTO dto = db.Sidebars.Find(1);
                // int model
                model = new SidebarVM(dto);
                //return view vith model
                return View(model);
            }
        }
        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db=new Db())
            {
                //get the dto
                SidebarDTO dto = db.Sidebars.Find(1);
                //dto body
                dto.Body = model.Body;
                //save 
                db.SaveChanges();
            }
            //set tempdata mesage
            TempData["SM"] = "You have dited the page..";
            //redirectToAction
            return RedirectToAction("EditSidebar");
        }

    }
}