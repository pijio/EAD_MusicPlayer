using System;
using System.Collections.Generic;
using System.Text;
using EAD_MusicPlayer.Data.DomainModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EAD_MusicPlayer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Author> Authors { get; set; }
        
        public DbSet<Song> Songs { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}