public partial class BTMember
{
    public long ID { get; set; }
    public string AccountId { get; set; }
    public int MemberType { get; set; }
    public int PreMemberType { get; set; }
    public double ExpiredDateTs { get; set; }
    public double FirstChargeDateTs { get; set; }
}
public partial class BTMember
{
    public static void OnDbContextModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BTMember>().HasKey(e => e.ID);
        modelBuilder.Entity<BTMember>().Property(e => e.AccountId).IsRequired();
        modelBuilder.Entity<BTMember>().Property(e => e.MemberType);
        modelBuilder.Entity<BTMember>().Property(e => e.PreMemberType);
        modelBuilder.Entity<BTMember>().Property(e => e.ExpiredDateTs);
        modelBuilder.Entity<BTMember>().Property(e => e.FirstChargeDateTs);
    }
}

public partial class BTMember
{
    public const int MEMBER_TYPE_LOGOUT = -1;
    public const int MEMBER_TYPE_FREE = 0;
    public const int MEMBER_TYPE_EXPIRED = 1;
    public const int MEMBER_TYPE_PREMIUM = 2;
    public const int MEMBER_TYPE_ADVANCED = 3;
}