using System;
using BahamutCommon.Utils;
namespace BTBaseWebAPI.Models
{
    public partial class BTMemberOrder
    {
        public long ID { get; set; }
        public string OrderKey { get; set; }
        public string AccountId { get; set; }
        public string ProductId { get; set; }
        public int MemberType { get; set; }
        public double ChargeTimes { get; set; }
        public string ReceiptData { get; set; }
        public double OrderDateTs { get; set; }
        public DateTime ChargedExpiredDateTime { get; set; }
    }

    public partial class BTMemberOrder
    {
        public static void OnDbContextModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BTMemberOrder>(ac =>
            {
                ac.HasKey(e => e.ID);
                ac.Property(e => e.OrderKey).HasMaxLength(512);
                ac.Property(e => e.AccountId).HasMaxLength(32).IsRequired();
                ac.Property(e => e.OrderDateTs);
                ac.Property(e => e.ProductId);
                ac.Property(e => e.ReceiptData);
                ac.Property(e => e.MemberType);
                ac.Property(e => e.ChargeTimes);
                ac.Property(e => e.ChargedExpiredDateTime);
                ac.HasIndex(e => e.AccountId);
                ac.HasIndex(e => e.OrderKey);
            });
        }
    }
}