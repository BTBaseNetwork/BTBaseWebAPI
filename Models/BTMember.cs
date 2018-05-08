namespace BTBaseWebAPI.Models
{
    public partial class BTMember
    {
        public long ID { get; set; }
        public string AccountId { get; set; }
        public int MemberType { get; set; }
        public double ExpiredDateTs { get; set; }
        public double FirstChargeDateTs { get; set; }
    }

    public partial class BTMember
    {
        public static void OnDbContextModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BTMember>(ac =>
            {
                ac.HasKey(e => e.ID);
                ac.Property(e => e.AccountId).HasMaxLength(32).IsRequired();
                ac.Property(e => e.MemberType).IsRequired();
                ac.Property(e => e.ExpiredDateTs);
                ac.Property(e => e.FirstChargeDateTs);
                ac.HasIndex(e => e.AccountId);
            });
        }
    }
}