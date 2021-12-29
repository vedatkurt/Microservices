using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Catalog
{
    public class CourseUpdateInput
    {
        public string Id { get; set; }

        public string CategoryId { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Kurs Adi")]
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