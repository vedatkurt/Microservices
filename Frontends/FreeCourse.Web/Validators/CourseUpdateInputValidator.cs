using FluentValidation;
using FreeCourse.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Validators
{
    public class CourseUpdateInputValidator:AbstractValidator<CourseUpdateInput>
    {
        public CourseUpdateInputValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Isim bos gecilemez!");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Aciklama bos gecilemez!");
            RuleFor(x => x.Feature.Duration).InclusiveBetween(1, int.MaxValue).WithMessage("Sure bos gecilemez!");

            //$$$$.$$
            RuleFor(x => x.Price).NotEmpty().WithMessage("Fiyat bos gecilemez!").ScalePrecision(2, 6).WithMessage("Hatali para formati");
        }
    }
}
