using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Pomelo.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace camagruApp.Models
{
    public partial class User
    {
        public int id { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }

        public string email { get; set; }

        public string username { get; set; }

        public string passwd { get; set; }

        public bool IsEmailVerified { get; set; }

        public System.Guid ActivationCode { get; set; }
    }
}