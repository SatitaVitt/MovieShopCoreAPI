﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Core.ApiModels.Request
{
    public class UserRegiesterRequestModel
    {
        //needs 4 property: email, user first name and last name, password
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
