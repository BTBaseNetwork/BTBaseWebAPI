using BTBaseWebAPI.DAL;
using BTBaseWebAPI.Models;

public class AccountService
{
    public string DbConnectionString { get; set; }

    public BTBaseDbContext GetDbContext()
    {
        return new BTBaseDbContext(DbConnectionString);
    }

    public BTAccount CreateNewAccount(BTAccount newAccount)
    {
        using (var dbContext = GetDbContext())
        {
            var res = dbContext.BTAccount.Add(newAccount).Entity;
            dbContext.SaveChanges();
            return res;
        }
    }

    public bool UpdatePassword(string accountId, string newPassword)
    {
        using (var dbContext = GetDbContext())
        {
            var account = dbContext.BTAccount.Find(long.Parse(accountId));
            if (account != null)
            {
                account.Password = newPassword;
                dbContext.BTAccount.Update(account);
                return true;
            }
        }
        return false;
    }

    public bool UpdateNick(string accountId, string newNick)
    {
        using (var dbContext = GetDbContext())
        {
            var account = dbContext.BTAccount.Find(long.Parse(accountId));
            if (account != null)
            {
                account.Nick = newNick;
                dbContext.BTAccount.Update(account);
                return true;
            }
        }
        return false;
    }
}