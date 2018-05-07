using BahamutCommon.Utils;
using BTBaseWebAPI.DAL;
using BTBaseWebAPI.Models;

namespace BTBaseWebAPI.Services
{
    public class AccountService
    {

        public BTAccount CreateNewAccount(BTBaseDbContext dbContext, BTAccount newAccount)
        {
            newAccount.AccountTypes = BTAccount.ACCOUNT_TYPE_GAME_PLAYER.ToString();
            newAccount.SignDateTs = DateTimeUtil.UnixTimeSpanSec;

            var res = dbContext.BTAccount.Add(newAccount).Entity;
            dbContext.SaveChanges();
            return res;
        }

        public bool UpdatePassword(BTBaseDbContext dbContext, string accountId, string originPassword, string newPassword)
        {
            var account = dbContext.BTAccount.Find(long.Parse(accountId));
            if (account != null && account.Password == originPassword)
            {
                account.Password = newPassword;
                dbContext.BTAccount.Update(account);
                return true;
            }
            return false;
        }

        public bool UpdateNick(BTBaseDbContext dbContext, string accountId, string newNick)
        {
            var account = dbContext.BTAccount.Find(long.Parse(accountId));
            if (account != null)
            {
                account.Nick = newNick;
                dbContext.BTAccount.Update(account);
                return true;
            }
            return false;
        }
    }
}