using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            //init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();
            //check if cart is empty
            if (cart.Count==0||Session["cart"]==null)
            {
                ViewBag.Message = "Sizin kartiniz bosdur !!!";
                return View();
            }
            //catlculate total and save the view bag 
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;

            }
            ViewBag.GrandTotal = total;
            //Return view with The model


            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //init cartVM

            CartVM model = new CartVM();
            //init quantity
            int qty=0;
            //init price 
            decimal price = 0m;
            //check for cart sesions
            if (Session["cart"]!=null)
            {
                //get total quantity and price
                var list =(List<CartVM>)Session["cart"];
                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Price * item.Quantity;
                }
                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                //or set quantity and price to 0
                model.Quantity = 0;
                model.Price = 0;
            }
            //return partial view with model
            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            // Init CartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Init CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                // Get the product
                ProductDTO product = db.Products.Find(id);

                // Check if the product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // If not, add new
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image=product.ImageName
                    });
                }
                else
                {
                    // If it is, increment
                    productInCart.Quantity++;
                }
            }

            // Get total qty and price and add to model

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Save cart back to session
            Session["cart"] = cart;

            // Return partial view with model
            return PartialView(model);
        }

        //GET/cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            //init cart list 
            List<CartVM> cart = Session["cart"]as List<CartVM>;
            using (Db db =new Db())
            {
                //get init cartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Increment qty 
                model.Quantity++;
                //Store newded data
                var result = new { qty = model.Quantity, price = model.Price };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        //return json vitw the model
        public JsonResult DecrementProduct(int productId)
        {
            //init cartVM list
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Db db=new Db())
            {
                //get model from list 
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                //decrement qty
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }
                //store need data 
                var result = new { qty = model.Quantity, price = model.Price };
                //return json vitw the model
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        public void RemoveProduct(int productId)
        {
            //init cartlist 
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db =new Db())
            {
                //get model from list 
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                //remove model from list
                cart.Remove(model);
            }
        }

        public  ActionResult PaypalPartial()
        {
            List<CartVM>cart=Session["cart"]as  List<CartVM>;
            return PartialView(cart);
        }

        //POST/cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            //Get cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            //Get username
            string username = User.Identity.Name;

            int orderId = 0;

            using (Db db=new Db())
            {
                //Init orderDTO
                OrderDTO orderDTO = new OrderDTO();


                //Get user id
                var q = db.Users.FirstOrDefault(x => x.Username == username);
                int userId = q.Id;


                //Add to OrderDTO and save 
                orderDTO.UserId = userId;
                orderDTO.CreatedAt = DateTime.Now;
                db.Orders.Add(orderDTO);
                db.SaveChanges();
                //Get insert id

                 orderId = orderDTO.OrderId;
                //init OrderDetalisDTO
                OrderDetailsDTO orderDetailsDTO = new OrderDetailsDTO();
                //Add to OrderDetalisDTO
                foreach (var item in cart )
                {
                    orderDetailsDTO.OrdersId = orderId;
                    orderDetailsDTO.UsersId = userId;
                    orderDetailsDTO.ProductsId = item.ProductId;
                    orderDetailsDTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDTO);

                    db.SaveChanges();
                }
            }
            //Email Admin
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("de934398f70c3a", "ef4676cd58434c"),
                EnableSsl = true
            };
            client.Send("admin@example.com", "admin@example.com", "Yeni order", "Sifarisinizin"+orderId );
            //Reset sessions
        }
    }
}