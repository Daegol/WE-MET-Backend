﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Account
{
    public class ForgotPasswordRequest
    {
        [Required]
        public string UserName { get; set; }
    }
}
