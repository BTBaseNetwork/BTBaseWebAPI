using System.Linq;
using System.Data;
using System;
using BahamutCommon.Utils;
using BTBaseWebAPI.DAL;
using BTBaseWebAPI.Models;
namespace BTBaseWebAPI.Services
{
    public class MemberService
    {
        public BTMemberProfile GetProfile(BTBaseDbContext dbContext, string accountId)
        {
            var list = from u in dbContext.BTMember where u.AccountId == accountId select u;
            var profile = new BTMemberProfile
            {
                AccountId = accountId,
                Members = list.ToArray()
            };
            foreach (var item in profile.Members)
            {
                item.ID = 0;
            }
            return profile;
        }

        public bool RechargeMember(BTBaseDbContext dbContext, BTMemberOrder order)
        {
            var listOrdered = from o in dbContext.BTMemberOrder where o.OrderKey == order.OrderKey select o.ID;
            if (listOrdered.Any())
            {
                //The Order Is Finished, Can't Request Same Order Twice
                return false;
            }

            var list = from u in dbContext.BTMember where u.AccountId == order.AccountId && u.MemberType == order.MemberType select u;
            BTMember member;
            var now = DateTimeUtil.UnixTimeSpanSec;
            if (list.Count() == 0)
            {
                member = new BTMember
                {
                    AccountId = order.AccountId,
                    FirstChargeDateTs = now,
                    MemberType = order.MemberType,
                    ExpiredDateTs = now + order.ChargeTimes
                };
                dbContext.BTMember.Add(member);
            }
            else
            {
                member = list.First();
                member.ExpiredDateTs = Math.Max(member.ExpiredDateTs, now) + order.ChargeTimes;
                member.MemberType = order.MemberType;
                dbContext.BTMember.Update(member);
            }
            order.OrderDateTs = now;
            order.ChargedExpiredDateTime = DateTimeUtil.UnixTimeSpanZeroDate().Add(TimeSpan.FromSeconds(member.ExpiredDateTs));
            dbContext.BTMemberOrder.Add(order);
            dbContext.SaveChanges();
            return true;
        }
    }
}