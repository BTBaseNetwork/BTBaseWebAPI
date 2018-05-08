using System.Collections.Generic;
using System.Linq;
using BahamutCommon.Utils;
using BTBaseWebAPI.DAL;
using BTBaseWebAPI.Models;
namespace BTBaseWebAPI.Services
{
    public class SessionService
    {
        public BTDeviceSession GetSession(BTBaseDbContext dbContext, string deviceId)
        {
            var list = from s in dbContext.BTDeviceSession where s.IsValid && s.DeviceId == deviceId select s;
            try
            {
                return list.First();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public BTDeviceSession GetSession(BTBaseDbContext dbContext, string deviceId, string accountId, bool reactive)
        {
            var list = from s in dbContext.BTDeviceSession where s.IsValid && s.DeviceId == deviceId && s.AccountId == accountId select s;
            try
            {
                var session = list.First();
                if (reactive)
                {
                    session.ReactiveDateTs = DateTimeUtil.UnixTimeSpanSec;
                    dbContext.BTDeviceSession.Update(session);
                    dbContext.SaveChanges();
                }
                return session;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public BTDeviceSession TestSession(BTBaseDbContext dbContext, string deviceId, string accountId, string sessionKey, bool reactive)
        {
            var list = from s in dbContext.BTDeviceSession where s.IsValid && s.DeviceId == deviceId && s.AccountId == accountId && s.SessionKey == sessionKey select s;
            try
            {
                var session = list.First();
                if (reactive)
                {
                    session.ReactiveDateTs = DateTimeUtil.UnixTimeSpanSec;
                    dbContext.BTDeviceSession.Update(session);
                    dbContext.SaveChanges();
                }
                return session;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public BTDeviceSession NewSession(BTBaseDbContext dbContext, BTDeviceSession session)
        {
            session.SessionKey = BahamutCommon.Utils.IDUtil.GenerateLongId().ToString();
            session.IsValid = true;
            session.LoginDateTs = session.ReactiveDateTs = DateTimeUtil.UnixTimeSpanSec;
            dbContext.BTDeviceSession.Add(session);
            dbContext.SaveChanges();
            return session;
        }

        public IEnumerable<BTDeviceSession> InvalidAllSession(BTBaseDbContext dbContext, string accountId, string deviceId, string session)
        {
            var list = (from s in dbContext.BTDeviceSession where s.IsValid && s.DeviceId == deviceId && s.AccountId == accountId && s.SessionKey == session select s).ToList();
            foreach (var item in list)
            {
                item.IsValid = false;
            }
            dbContext.BTDeviceSession.UpdateRange(list);
            if (dbContext.SaveChanges() > 0)
            {
                return list;
            }
            else
            {
                return new BTDeviceSession[0];
            }
        }

        public BTDeviceSession ReactiveSession(BTBaseDbContext dbContext, string deviceId)
        {
            try
            {
                var session = (from s in dbContext.BTDeviceSession where s.IsValid && s.DeviceId == deviceId select s).First();
                session.ReactiveDateTs = DateTimeUtil.UnixTimeSpanSec;
                dbContext.BTDeviceSession.Update(session);
                dbContext.SaveChanges();
                return session;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public IEnumerable<string> InvalidSessionAccountLimited(BTBaseDbContext dbContext, string accountId, int accountDeviceLimited)
        {
            var list = (from s in dbContext.BTDeviceSession where s.IsValid && s.AccountId == accountId select s).ToList();
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
                return from s in dirtyList select s.DeviceName;
            }
            return new string[0];
        }

        private void InvalidAllSessions(BTBaseDbContext dbContext, IEnumerable<BTDeviceSession> sessions)
        {
            foreach (var item in sessions)
            {
                item.IsValid = false;
            }
            dbContext.BTDeviceSession.UpdateRange(sessions);
            dbContext.SaveChanges();
        }
    }
}