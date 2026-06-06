using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Domain.Enums
{
    public enum Type
    {
        [Display(Name = "Công khai")]
        Public = 1,

        [Display(Name = "Cá nhân")]
        Private = 2
    }
}
