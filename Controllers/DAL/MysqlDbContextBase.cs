using Microsoft.EntityFrameworkCore;

public class MysqlDbContextBase : DbContext
{
    public string ConnectionString { get; private set; }
    public MysqlDbContextBase(string connectionString) : base()
    {
        this.ConnectionString = connectionString;
    }
}