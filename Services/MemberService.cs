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

        public BTMember GetProfile(BTBaseDbContext dbContext, string accountId)
        {
            var list = from u in dbContext.BTMember where u.AccountId == accountId select u;
            return list.Count() > 0 ? list.First() : null;
        }

        public bool RechargeMember(BTBaseDbContext dbContext, BTMemberOrder order)
        {
            var list = from u in dbContext.BTMember where u.AccountId == order.AccountId select u;
            var listOrdered = from o in dbContext.BTMemberOrder where o.ReceiptData == order.ReceiptData select o.ID;
            if (listOrdered.Count() > 0)
            {
                //The Order Is Finished, Can't Request Same Order Twice
                return false;
            }

            BTMember member;
            if (list.Count() == 0)
            {
                order.PreMemberType = BTMember.MEMBER_TYPE_FREE;
                order.PreExpiredTs = DateTimeUtil.UnixTimeSpanSec;
                member = new BTMember
                {
                    AccountId = order.AccountId,
                    FirstChargeDateTs = order.PreExpiredTs,
                    PreMemberType = BTMember.MEMBER_TYPE_FREE,
                    MemberType = order.ChargeMemberType,
                    ExpiredDateTs = order.PreExpiredTs + order.ChargeTimes
                };
                dbContext.BTMember.Add(member);
            }
            else
            {
                member = list.First();
                order.PreMemberType = member.MemberType;
                order.PreExpiredTs = member.ExpiredDateTs;
                member.ExpiredDateTs = Math.Max(order.PreExpiredTs, DateTimeUtil.UnixTimeSpanSec) + order.ChargeTimes;
                member.PreMemberType = member.MemberType;
                member.MemberType = order.ChargeMemberType;
                dbContext.BTMember.Update(member);
            }
            order.OrderDateTs = DateTimeUtil.UnixTimeSpanSec;
            order.ChargedExpiredDateTime = DateTimeUtil.UnixTimeSpanZeroDate().Add(TimeSpan.FromSeconds(member.ExpiredDateTs));
            dbContext.BTMemberOrder.Add(order);
            dbContext.SaveChanges();
            return true;
        }
    }
}