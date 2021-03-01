using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseIdentity.CustomValidator
{
    public class CustomIdentityValidator:IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError() {
                Code = "PasswordTooShort",
                Description = $"Paralo minimum{length} karakter olmalıdır."
            };
        }
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description="Parola bir alfanümerik karakter(!.~ vs.) içermelidir"
            };
        }
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError() {
                Code = "DuplicateUserName",
                Description = $"ilgili kullanıcı adı ({userName}) zaten sistemde kayıtlı"
            };
        }
    }
}
