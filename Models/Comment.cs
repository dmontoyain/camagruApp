using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Pomelo.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace camagruApp.Models
{
    public partial class Comment
    {
        public int id { get; set; }

        public int imgid { get; set; }

        public Img img { get; set; }

        public DateTime? dateposted { get; set; }

        public string content { get; set; }

        public int userid { get; set; }

        public string username { get; set; }
    }
}