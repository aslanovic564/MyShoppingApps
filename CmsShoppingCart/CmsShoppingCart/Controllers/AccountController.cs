using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Account;
using CmsShoppingCart.Models.ViewModels.Shop;
using CmsShoppingCart.Views.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CmsShoppingCart.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }
        // GET: Account/login
        [HttpGet]
        public ActionResult Login()
        {
            //Confirm user is not logged in
            string username = User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
                return RedirectToAction("user-profile");

            //return
            return View();
        }

        // POST: Account/login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //Check model satate
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //check if the user is valid 
            bool isValid = false;
            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }
            }
            if (!isValid)
            {
                ModelState.AddModelError("", "Usernam veya Sifre sehvdir");
                return View(model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
            }
            //return view with model 
        }

        // GET: Account/CreateAccount
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount", new UserVM());
        }

        // POST: Account/CreateAccount
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            //Check model satate 
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            //Check if password math 
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError(" ", "Parollar uygunlasmir!!!");
                return View("CreateAccount", model);
            }
            using (Db db = new Db())
            {
                //Make sure username iss unique
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError(" ", "Bele" + model.Username + "movcuddur");
                    model.Username = " ";
                    return View("CreateAccount", model);
                }
                //Create UserDTO
                UserDTO dto = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Username = model.Username,
                    Password = model.Password

                };
                //Add the DTO
                db.Users.Add(dto);
                //Save 
                db.SaveChanges();
                //Add the UserRolesDTO
                int id = dto.Id;
                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };
                db.UsersRoles.Add(userRoleDTO);
                db.SaveChanges();

            }
            //create a Tempdata message
            TempData["SM"] = "Qeydiyyatdan muveffeqiyyetle kecdiniz!!";
            //return 
            return Redirect("~/account/login");
        }


        // POST: Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/account/login");
        }

        [Authorize]
        public ActionResult UserNavPartial()
        {
            //Get user name 
            string username = User.Identity.Name;
            //Declare model
            UserNavPartialVM model;
            using (Db db = new Db())
            {
                //get the model
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username.Equals(username));

                //Build the model
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            //return view with the model

            return PartialView(model);
        }
        // GET: Account/UserProfile
        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            //Get username 
            string username = User.Identity.Name;
            //Declare model

            UserProfileVM model;
            using (Db db = new Db())
            {
                //get user
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username.Equals(username));

                //Build model
                model = new UserProfileVM(dto);

            }

            //Return view with the model
            return View("UserProfile", model);
        }

        // POST: Account/UserProfile    
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            //Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //check if passwor math if  nedd be 
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Sifre duzgun deyil");
                    return View(model);
                }
            }
            using (Db db = new Db())
            {

                //Get username
                string username = User.Identity.Name;

                //Make sure username is unique
                if (db.Users.Where(x=>x.Id==model.Id).Any(x=>x.Username==model.Username))
                {
                    ModelState.AddModelError("","Username"+model.Username+"artiq movcuddur");
                    model.Username = "";
                    return View("UserProfile", model);
                }
                //edidt dto
                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress= model.EmailAddress;
                dto.Username = model.Username;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }
                //save
                db.SaveChanges();

                //Set tempdata messages
                TempData["SM"] = "Profilinizi ugurla yenilediniz";
                //return redirect 
                return Redirect("~/account/user-profile");
            }

        }

        //GET//account/orders
        [Authorize(Roles ="User")]
        public ActionResult Orders()
        {
            //init list of OrdersforuserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();
            using (Db db = new Db())
            {
                //get user id
                UserDTO user = db.Users.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                // init list of orderVM
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId).ToArray().Select(x => new OrderVM(x)).ToList();
                //Loop throught lis of OrderVM
                foreach (var order in orders)
                {
                    //Init product dictionary
                    Dictionary<string, int> productsAndQyt = new Dictionary<string, int>();
                    //declare total
                    decimal total = 0m;
                    //init list of OrdersDetailsDTO
                    List<OrderDetailsDTO> orderDetailsDTO = db.OrderDetails.Where(x => x.OrdersId == order.OrderId).ToList();
                    //loop through list of OrderDetalisDTO
                    foreach (var orderDetails in orderDetailsDTO)
                    {
                        // get product
                        ProductDTO product = db.Products.Where(x => x.Id == orderDetails.ProductsId).FirstOrDefault();
                        //get product price
                        decimal price = product.Price;
                        //get product name
                        string productName = product.Name;

                        //add to products dictinary
                        productsAndQyt.Add(productName, orderDetails.Quantity);

                        //get total
                        total += orderDetails.Quantity * price;
                    }
                    //add to ordersForUserVM List
                    ordersForUser.Add(new OrdersForUserVM()
                    {
                        OrderNumber=order.OrderId,
                        ProductsAndQty=productsAndQyt,
                        CreatedAt=order.CreatedAt,
                        Total=total
                    });
                }
            }
            //Return view with listmodel
            return View(ordersForUser);
        }
    }
}