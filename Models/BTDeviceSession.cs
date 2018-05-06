namespace BTBaseWebAPI.Models
{
    public partial class BTDeviceSession
    {
        public long ID { get; set; }
        public string AccountId { get; set; }
        public string DeviceId { get; set; }
        public double LoginDateTs { get; set; }
        public double ReactiveDateTs { get; set; }
        public string DeviceName { get; set; }
        public string SessionKey { get; set; }
        public bool IsValid { get; set; }
    }

    public partial class BTDeviceSession
    {
        public static void OnDbContextModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BTDeviceSession>().HasKey(e => e.ID);
            modelBuilder.Entity<BTDeviceSession>().Property(e => e.AccountId).IsRequired();
            modelBuilder.Entity<BTDeviceSession>().Property(e => e.DeviceId).IsRequired();
            modelBuilder.Entity<BTDeviceSession>().Property(e => e.LoginDateTs);
            modelBuilder.Entity<BTDeviceSession>().Property(e => e.ReactiveDateTs);
            modelBuilder.Entity<BTDeviceSession>().Property(e => e.DeviceName);
            modelBuilder.Entity<BTDeviceSession>().Property(e => e.SessionKey);
        }
    }
}