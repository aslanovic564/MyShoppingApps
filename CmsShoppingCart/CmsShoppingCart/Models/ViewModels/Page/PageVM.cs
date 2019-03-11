using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Models.ViewModels.Page
{
    public class PageVM
    {

        public PageVM()
        {
        }
        public PageVM(PageDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50,MinimumLength =3)]
        public string Title { get; set; }

        public String Slug { get; set; }

        [Required]
        [StringLength(int.MaxValue,MinimumLength =3)]
        [AllowHtml]
        public string Body { get; set; }

        public int Sorting { get; set; }

        public bool HasSidebar { get; set; }
    }
}