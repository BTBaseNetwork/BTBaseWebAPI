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
    public class WalletsController : Controller
    {
        private readonly BTBaseDbContext dbContext;
        private readonly WalletService walletService;

        public WalletsController(BTBaseDbContext dbContext, WalletService walletService)
        {
            this.dbContext = dbContext;
            this.walletService = walletService;
        }

        [Authorize]
        [HttpGet("Detail")]
        public object GetProfile()
        {
            var wallet = walletService.GetWallet(dbContext, this.GetHeaderAccountId());
            return new ApiResult
            {
                code = this.SetResponseOK(),
                content = new
                {
                    accountId = wallet.AccountId,
                    tokenMoney = wallet.TokenMoney
                }
            };
        }

        [Authorize]
        [HttpPost("TokenMoney/Order")]
        public async Task<object> RechargeTokenMoneyAsync(string productId, string channel, string receiptData, bool sandbox)
        {
            switch (channel)
            {
                case BTServiceConst.CHANNEL_APP_STORE:
                    return await RechargeTokenMoneyOfAppStoreAsync(this.GetHeaderAccountId(), productId, receiptData);
                default: return new ApiResult { code = this.SetResponseForbidden(), msg = "Unsupported Channel" };
            }
        }

        private async Task<object> RechargeTokenMoneyOfAppStoreAsync(string accountId, string productId, string receiptData)
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
                    Date = DateTime.Now
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
                            if (walletService.RechargeTokenMoneyByAppleIAP(dbContext, order))
                            {
                                trans.Commit();
                                dbContext.SaveChanges();
                                return new ApiResult { code = 200, msg = "Order Finished" };
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