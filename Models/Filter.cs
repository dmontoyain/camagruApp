using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Pomelo.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace camagruApp.Models
{
    public partial class Filter
    {
        public int id { get; set; }

        public byte[] img { get; set; }

        public string name { get; set; }
    }
}