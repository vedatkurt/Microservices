using FluentValidation;
using FreeCourse.Web.Models.Discount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Validators
{
    public class DiscountApplyInputValidator:AbstractValidator<DiscountApplyInput>
    {
        public DiscountApplyInputValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Indirim kodu bos olamaz");
        }
    }
}
