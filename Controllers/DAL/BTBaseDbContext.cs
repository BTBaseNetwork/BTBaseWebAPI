using System;
using Microsoft.EntityFrameworkCore;

public class BTBaseDbContext : MysqlDbContextBase
{
    public virtual DbSet<BTAccount> DbSetBTAccount { get; set; }
    public virtual DbSet<BTDeviceSession> DbSetBTDeviceSession { get; set; }
    public virtual DbSet<BTAppLaunchRecord> DbSetBTAppLaunchRecord { get; set; }
    public virtual DbSet<BTMember> DbSetBTMember { get; set; }
    public virtual DbSet<BTMemberOrder> DbSetBTMemberOrder { get; set; }

    public BTBaseDbContext(string connectionString) : base(connectionString) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseMySQL(ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        BTAccount.OnDbContextModelCreating(modelBuilder);
        BTDeviceSession.OnDbContextModelCreating(modelBuilder);
        BTAppLaunchRecord.OnDbContextModelCreating(modelBuilder);
        BTMember.OnDbContextModelCreating(modelBuilder);
        BTMemberOrder.OnDbContextModelCreating(modelBuilder);
    }
}