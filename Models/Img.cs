using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Pomelo.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace camagruApp.Models
{
    public partial class Img
    {
        public int id { get; set; }

        public byte[] img { get; set; }

        public int userid { get; set; }

        public string username { get; set; }

        public DateTime dateposted { get; set; }

        public string caption { get; set; }

        public int likes { get; set; }

        public List<Comment> Comments { get; set; }
    }
}