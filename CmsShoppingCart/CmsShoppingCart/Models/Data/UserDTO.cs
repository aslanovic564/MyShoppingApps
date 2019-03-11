using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("tblUsers")]
    public class UserDTO
    {
        public UserDTO()
        {
            OrderDTOs = new List<OrderDTO>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress  { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public List<OrderDTO> OrderDTOs { get; set; }
    }
}