using System;
using Microsoft.EntityFrameworkCore;
using BTBaseWebAPI.Models;

namespace BTBaseWebAPI.DAL
{
    public class BTBaseDbContext : DbContext
    {
        public virtual DbSet<BTAccount> BTAccount { get; set; }
        public virtual DbSet<BTDeviceSession> BTDeviceSession { get; set; }
        public virtual DbSet<BTMember> BTMember { get; set; }
        public virtual DbSet<BTMemberOrder> BTMemberOrder { get; set; }

        public BTBaseDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            BTBaseWebAPI.Models.BTAccount.OnDbContextModelCreating(modelBuilder);
            BTBaseWebAPI.Models.BTDeviceSession.OnDbContextModelCreating(modelBuilder);
            BTBaseWebAPI.Models.BTMember.OnDbContextModelCreating(modelBuilder);
            BTBaseWebAPI.Models.BTMemberOrder.OnDbContextModelCreating(modelBuilder);
        }
    }
}