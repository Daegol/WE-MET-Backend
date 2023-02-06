using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Account
{
    public class RefreshTokenRequest
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
