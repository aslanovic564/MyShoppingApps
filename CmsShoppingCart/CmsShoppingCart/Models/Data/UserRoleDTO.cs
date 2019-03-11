using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("tblUserRoles")]
    public class UserRoleDTO
    {

        public UserDTO User { get; set; }
        [Key,Column(Order =0)]
        public int UserId { get; set; }
      
       

      
        public RolesDTO Role { get; set; }
        [Key, Column(Order = 1)]
        public int RoleId { get; set; }
    }
}