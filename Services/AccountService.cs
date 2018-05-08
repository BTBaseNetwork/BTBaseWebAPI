using System;
using System.Linq;
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

        public bool IsUsernameAvaiable(BTBaseDbContext dbContext, string username)
        {
            return dbContext.BTAccount.Count(x => x.UserName == username) == 0;
        }

        public BTAccount GetProfile(BTBaseDbContext dbContext, string accountId)
        {
            return dbContext.BTAccount.Find(long.Parse(accountId));
        }

        public BTAccount GetValidateProfile(BTBaseDbContext dbContext, string userstring, string password)
        {
            BTAccount account = null;
            if (account == null && CommonRegexTestUtil.TestPattern(userstring, CommonRegexTestUtil.PATTERN_ACOUNT_ID))
            {
                var query = from a in dbContext.BTAccount where a.Password == password && a.AccountId == userstring select a;
                try { account = query.First(); } catch (System.Exception) { }
            }

            if (account == null && CommonRegexTestUtil.TestPattern(userstring, CommonRegexTestUtil.PATTERN_EMAIL))
            {
                var query = from a in dbContext.BTAccount where a.Password == password && a.Email == userstring select a;
                try { account = query.First(); } catch (System.Exception) { }
            }

            if (account == null && CommonRegexTestUtil.TestPattern(userstring, CommonRegexTestUtil.PATTERN_USERNAME))
            {
                var query = from a in dbContext.BTAccount where a.Password == password && a.UserName == userstring select a;
                try { account = query.First(); } catch (System.Exception) { }
            }

            if (account == null && CommonRegexTestUtil.TestPattern(userstring, CommonRegexTestUtil.PATTERN_PHONE_NO))
            {
                var query = from a in dbContext.BTAccount where a.Password == password && a.Mobile == userstring select a;
                try { account = query.First(); } catch (System.Exception) { }
            }

            return account;
        }
    }
}