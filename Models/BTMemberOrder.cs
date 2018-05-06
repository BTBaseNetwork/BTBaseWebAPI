using System;
using BahamutCommon.Utils;
namespace BTBaseWebAPI.Models
{
    public partial class BTMemberOrder
    {
        public long ID { get; set; }
        public string AccountId { get; set; }
        public string ProductId { get; set; }
        public int ChargeMemberType { get; set; }
        public double ChargeTimes { get; set; }
        public string ReceiptData { get; set; }
        public int PreMemberType { get; set; }
        public double PreExpiredTs { get; set; }
        public double OrderDateTs { get; set; }

        public DateTime ChargedExpiredDateTime { get; set; }
    }

    public partial class BTMemberOrder
    {
        public static void OnDbContextModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BTMemberOrder>().HasKey(e => e.ID);
            modelBuilder.Entity<BTMemberOrder>().Property(e => e.AccountId).IsRequired();
            modelBuilder.Entity<BTMemberOrder>().Property(e => e.OrderDateTs);
            modelBuilder.Entity<BTMemberOrder>().Property(e => e.ProductId);
            modelBuilder.Entity<BTMemberOrder>().Property(e => e.ReceiptData);
            modelBuilder.Entity<BTMemberOrder>().Property(e => e.PreMemberType);
            modelBuilder.Entity<BTMemberOrder>().Property(e => e.ChargeMemberType);
            modelBuilder.Entity<BTMemberOrder>().Property(e => e.PreExpiredTs);
            modelBuilder.Entity<BTMemberOrder>().Property(e => e.ChargeTimes);
            modelBuilder.Entity<BTMemberOrder>().Property(e => e.ChargedExpiredDateTime);
        }
    }
}