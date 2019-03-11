using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("tblOrderDetails")]
    public class OrderDetailsDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrdersId { get; set; }
        public int UsersId { get; set; }
        public int ProductsId { get; set; }
        public int Quantity { get; set; }


        [ForeignKey("OrdersId")]
        public OrderDTO Orders { get; set; }
        [ForeignKey("UsersId")]
        public UserDTO Users { get; set; }
        [ForeignKey("ProductsId")]
        public ProductDTO Products { get; set; }
    }
}