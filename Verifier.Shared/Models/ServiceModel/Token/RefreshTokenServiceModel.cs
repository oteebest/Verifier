using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Request.User
{
    public class RefreshTokenServiceModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
