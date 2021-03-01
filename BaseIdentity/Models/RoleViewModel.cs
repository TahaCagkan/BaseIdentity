using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BaseIdentity.Models
{
    public class RoleViewModel
    {
        [Required(ErrorMessage ="Ad alanı gereklidir")]
        [Display(Name="Ad :")]
        public string Name { get; set; }
    }
}
