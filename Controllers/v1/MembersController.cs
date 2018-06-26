using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BTBaseServices;
using BTBaseServices.Models;
using BTBaseServices.DAL;
using BTBaseServices.Services;
using Microsoft.AspNetCore.Authorization;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class MembersController : Controller
    {

        private readonly BTBaseDbContext dbContext;
        private readonly MemberService memberService;

        public MembersController(BTBaseDbContext dbContext, MemberService memberService)
        {
            this.dbContext = dbContext;
            this.memberService = memberService;
        }

        [Authorize]
        [HttpGet("Profile")]
        public object GetProfile()
        {
            var profile = memberService.GetProfile(dbContext, this.GetHeaderAccountId());
            return new ApiResult
            {
                code = this.SetResponseOK(),
                content = profile
            };
        }

        [Authorize]
        [HttpPost("ExpiredDate/TokenMoney")]
        public object RechargeAsync(string productId)
        {
            BTMemberProduct product = null;
            if (!string.IsNullOrEmpty(productId) && BTMemberProduct.TryParseIAPProductId(productId, out product))
            {
                var order = new BTMemberOrder
                {
                    AccountId = this.GetHeaderAccountId(),
                    PaymentType = BTMemberOrder.PAYMENT_TYPE_BTOKEN_MONTY,
                    MemberType = product.MemberType,
                    ChargeTimes = product.ChargeTimes
                };
                throw new NotImplementedException();
            }

            return new ApiResult
            {
                code = 400,
                error = new ErrorResult
                {
                    code = 400,
                    msg = "Invalid Product Id"
                }
            };
        }

        [Authorize]
        [HttpPost("ExpiredDate/Order")]
        public async Task<object> RechargeAsync(string productId, string channel, string receiptData, bool sandbox)
        {
            switch (channel)
            {
                case BTServiceConst.CHANNEL_APP_STORE:
                    return await VerifyReceiptByAppStoreAndRechargeAsync(this.GetHeaderAccountId(), productId, receiptData, sandbox);
                default: return new ApiResult { code = this.SetResponseForbidden(), msg = "Unsupported Channel" };
            }
        }

        private async Task<object> VerifyReceiptByAppStoreAndRechargeAsync(string accountId, string productId, string receiptData, bool sandbox)
        {

            var verifyRes = await AppleStoreIAPUtils.VerifyReceiptAppStoreAsync(productId, receiptData);
            if (verifyRes.ResponseCode == 200)
            {
                if (string.IsNullOrWhiteSpace(verifyRes.MatchedProductId))
                {
                    return new ApiResult { code = 400, error = new ErrorResult { code = 400, msg = "Unmatched Product Id" } };
                }

                var order = new AppleStoreIAPOrder
                {
                    OrderKey = verifyRes.TransactionId,
                    AccountId = accountId,
                    ProductId = verifyRes.MatchedProductId,
                    ReceiptData = verifyRes.ReceiptData,
                    Date = DateTime.Now,
                    SandBox = verifyRes.IsSandBox
                };

                if (AppleStoreIAPManager.IsAppleStoreOrderExists(dbContext, order))
                {
                    return new ApiResult { code = 200, msg = "Is A Completed Order" };
                }
                else
                {
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            order = AppleStoreIAPManager.AddAppleStoreOrder(dbContext, order);
                            BTMemberProduct product = null;
                            if (!string.IsNullOrEmpty(order.ProductId) && BTMemberProduct.TryParseIAPProductId(order.ProductId, out product))
                            {
                                var suc = memberService.RechargeMember(dbContext, new BTMemberOrder
                                {
                                    AccountId = accountId,
                                    PaymentType = BTMemberOrder.PAYMENT_TYPE_APPLE_IAP,
                                    MemberType = product.MemberType,
                                    ChargeTimes = product.ChargeTimes
                                });

                                if (suc)
                                {
                                    trans.Commit();
                                    dbContext.SaveChanges();
                                    return new ApiResult { code = 200, msg = "Order Finished" };
                                }
                                else
                                {
                                    return new ApiResult { code = 200, msg = "Is A Completed Order" };
                                }
                            }
                            else
                            {
                                trans.Rollback();
                                return new ApiResult { code = 400, error = new ErrorResult { code = 400, msg = "Unmatched Product Id" } };
                            }
                        }
                        catch (System.Exception)
                        {
                            trans.Rollback();
                            return new ApiResult { code = (int)System.Net.HttpStatusCode.InternalServerError };
                        }
                    }
                }
            }
            else
            {
                return new ApiResult { code = 400, error = new ErrorResult { code = 400, msg = "App Store Error" } };
            }


        }
    }
}