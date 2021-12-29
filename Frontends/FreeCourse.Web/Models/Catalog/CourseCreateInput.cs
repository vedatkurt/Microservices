using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Catalog
{
    public class CourseCreateInput
    {
        [Display(Name = "Category Id")]
        public string CategoryId { get; set; }

        [Display(Name = "User Id")]
        public string UserId { get; set; }

        [Display(Name="Kurs adi")]
        public string Name { get; set; }

        [Display(Name = "Fiyati")]
        public decimal Price { get; set; }

        [Display(Name = "Aciklama")]
        public string Description { get; set; }

        public string Picture { get; set; }

        public FeatureViewModel Feature { get; set; }

        public IFormFile PhotoFile { get; set; }
    }
}