using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    [Table("tblCategories")]
    public class CategoryDTO
    {
        public CategoryDTO()
        {
            Products = new List<ProductDTO>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public int Sorting { get; set; }

        public List<ProductDTO> Products { get; set; }
    }
}