﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BaseIdentity.Models
{
    public class UserSignInViewModel
    {
        [Display(Name ="Kullanıcı Adı:")]
        [Required(ErrorMessage ="Kullanıcı Adı boş geçilemez")]
        public string UserName { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre boş geçilemez")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
