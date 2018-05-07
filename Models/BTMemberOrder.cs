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
            modelBuilder.Entity<BTMemberOrder>(ac =>
            {
                ac.HasKey(e => e.ID);
                ac.Property(e => e.AccountId).IsRequired();
                ac.Property(e => e.OrderDateTs);
                ac.Property(e => e.ProductId);
                ac.Property(e => e.ReceiptData);
                ac.Property(e => e.PreMemberType);
                ac.Property(e => e.ChargeMemberType);
                ac.Property(e => e.PreExpiredTs);
                ac.Property(e => e.ChargeTimes);
                ac.Property(e => e.ChargedExpiredDateTime);
            });
        }
    }
}