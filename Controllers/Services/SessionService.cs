using System.Collections.Generic;
using System.Linq;
using BahamutCommon.Utils;

public class SessionService
{
    public string DbConnectionString { get; set; }

    public BTBaseDbContext GetDbContext()
    {
        return new BTBaseDbContext(DbConnectionString);
    }

    public BTDeviceSession GetSession(string deviceId)
    {
        using (var dbContext = GetDbContext())
        {
            var list = from s in dbContext.DbSetBTDeviceSession where s.IsValid && s.DeviceId == deviceId select s;
            try
            {
                return list.First();
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }

    public BTDeviceSession NewSession(BTDeviceSession session)
    {
        using (var dbContext = GetDbContext())
        {
            session.SessionKey = BahamutCommon.Utils.IDUtil.GenerateLongId().ToString();
            dbContext.DbSetBTDeviceSession.Add(session);
            dbContext.SaveChanges();
            return session;
        }
    }

    public void InvalidAllSession(string deviceId)
    {
        using (var dbContext = GetDbContext())
        {
            var list = (from s in dbContext.DbSetBTDeviceSession where s.IsValid && s.DeviceId == deviceId select s).ToList();
            foreach (var item in list)
            {
                item.IsValid = false;
            }
            dbContext.DbSetBTDeviceSession.UpdateRange(list);
            dbContext.SaveChanges();
        }
    }

    private IEnumerable<BTDeviceSession> GetAccountSessions(string accountId)
    {
        using (var dbContext = GetDbContext())
        {
            var list = (from s in dbContext.DbSetBTDeviceSession where s.IsValid && s.AccountId == accountId select s).ToList();
            return list.ToList();
        }
    }

    public void ReactiveSession(string accountId, string deviceId)
    {
        using (var dbContext = GetDbContext())
        {
            var list = (from s in dbContext.DbSetBTDeviceSession where s.IsValid && s.AccountId == accountId && s.DeviceId == deviceId select s).ToList();
            foreach (var item in list)
            {
                item.ReactiveDateTs = DateTimeUtil.UnixTimeSpanSec;
            }
            dbContext.UpdateRange(list);
            dbContext.SaveChanges();
        }
    }

    public void InvalidSessionAccountLimited(string accountId, int accountDeviceLimited)
    {
        using (var dbContext = GetDbContext())
        {
            var list = (from s in dbContext.DbSetBTDeviceSession where s.IsValid && s.AccountId == accountId select s).ToList();
            list.Sort((a, b) => { return a.ReactiveDateTs >= b.ReactiveDateTs ? -1 : 1; });
            var dirtyList = new List<BTDeviceSession>();
            for (int i = accountDeviceLimited; i < list.Count; i++)
            {
                list[i].IsValid = false;
                dirtyList.Add(list[i]);
            }
            if (dirtyList.Count > 0)
            {
                dbContext.UpdateRange(dirtyList);
                dbContext.SaveChanges();
            }
        }
    }

    private void InvalidAllSessions(IEnumerable<BTDeviceSession> sessions)
    {
        using (var dbContext = GetDbContext())
        {
            foreach (var item in sessions)
            {
                item.IsValid = false;
            }
            dbContext.DbSetBTDeviceSession.UpdateRange(sessions);
            dbContext.SaveChanges();
        }
    }
}