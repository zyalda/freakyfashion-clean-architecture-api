using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationLayer.Dto
{
    public class LoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string PassWord { get; set; } = string.Empty;
    }
}
