using System;
using Microsoft.EntityFrameworkCore;
using BTBaseWebAPI.Models;

namespace BTBaseWebAPI.DAL
{
    public class BTBaseDbContext : MysqlDbContextBase
    {
        public virtual DbSet<BTAccount> BTAccount { get; set; }
        public virtual DbSet<BTDeviceSession> BTDeviceSession { get; set; }
        public virtual DbSet<BTMember> BTMember { get; set; }
        public virtual DbSet<BTMemberOrder> BTMemberOrder { get; set; }

        public BTBaseDbContext(string connectionString) : base(connectionString) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySQL(ConnectionString);
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