namespace BTBaseWebAPI.Models
{
    public partial class BTAccount
    {
        public long AccountRawId { get; set; }
        public string AccountId { get { return AccountRawId.ToString(); } }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AccountTypes { get; set; }
        public string Nick { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public double SignDateTs { get; set; }
    }

    public partial class BTAccount
    {
        public static void OnDbContextModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BTAccount>(ac =>
            {
                ac.HasKey(e => e.AccountRawId);
                ac.Property(e => e.UserName).HasMaxLength(32);
                ac.HasIndex(e => e.UserName).IsUnique();
                ac.Property(e => e.Password).HasMaxLength(512).IsRequired();
                ac.Property(e => e.AccountTypes);
                ac.Property(e => e.Nick);
                ac.Property(e => e.Email);
                ac.Property(e => e.Mobile);
                ac.Property(e => e.SignDateTs);
            });
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
}