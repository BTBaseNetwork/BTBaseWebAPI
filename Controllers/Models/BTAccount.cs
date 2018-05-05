public partial class BTAccount
{
    public long AccountRawId { get; set; }
    public string AccountId { get { return AccountRawId.ToString(); } }
    public string AccountTypes { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Nick { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public double SignDateTs { get; set; }
}

public partial class BTAccount
{
    public static void OnDbContextModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BTAccount>().HasKey(e => e.AccountRawId);
        modelBuilder.Entity<BTAccount>().Property(e => e.AccountTypes);
        modelBuilder.Entity<BTAccount>().Property(e => e.UserName).IsRequired();
        modelBuilder.Entity<BTAccount>().Property(e => e.Password).IsRequired();
        modelBuilder.Entity<BTAccount>().Property(e => e.Nick);
        modelBuilder.Entity<BTAccount>().Property(e => e.Email);
        modelBuilder.Entity<BTAccount>().Property(e => e.Mobile);
        modelBuilder.Entity<BTAccount>().Property(e => e.SignDateTs);
    }
}

public partial class BTAccount
{

    public const int ACCOUNT_TYPE_LOGOUT = -1;
    public const int ACCOUNT_TYPE_BTPLATFORM = 0;
    public const int ACCOUNT_TYPE_GAME_PRODUCER = 1;
    public const int ACCOUNT_TYPE_GAME_PLAYER = 2;
    public const string USER_ID_UNLOGIN = "USERID_UNLOGIN";
    public const string ACCOUNT_ID_UNLOGIN = "000000";
}