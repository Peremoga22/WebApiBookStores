﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Data
{
    public class ApplicatinDbContext : IdentityDbContext<IdentityUser>    
    {
        public ApplicatinDbContext(DbContextOptions<ApplicatinDbContext> options) : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
    }
}
