using System;
using System.Collections.Generic;
using Pomelo.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using camagruApp.Models;

namespace camagruApp.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options)
            :base(options)
        {}

        public DbSet<User> Users { get; set; }

        public DbSet<Img> Images { get; set; }


        public DbSet<Filter> Filters { get; set; }

        public DbSet<Comment> Comments { get; set; }
        
        public DbSet<LoginViewModel> LoginViewModel { get; set; }
    }
}