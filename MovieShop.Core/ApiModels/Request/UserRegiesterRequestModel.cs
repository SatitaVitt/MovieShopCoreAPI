using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Core.ApiModels.Request
{
    public class UserRegiesterRequestModel
    {
        //needs 4 property: email, user first name and last name, password
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }
}
