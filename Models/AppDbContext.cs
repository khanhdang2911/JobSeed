using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JobSeed.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
                base.OnConfiguring(builder);
        } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UsersRole>(options=>{
                options.HasKey(u=>new{u.UsersId, u.RoleId});
            });
        }
        public DbSet<Job> jobs { get; set; }
        public DbSet<JobType> jobTypes{set;get;}
        public DbSet<Role> roles{set;get;}
        public DbSet<Users> users {set;get;}
        public DbSet<UsersRole> usersRoles {set;get;}
    }
}