using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Domain.Enums
{
    public enum UserType
    {
        [Display(Name = "Free Plan")]
        FreePlan = 1,

        [Display(Name = "Premium")]
        Premium = 2,
    }
}
