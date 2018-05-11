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
        [HttpPost("ExpiredDate/Order")]
        public async Task<object> RechargeAsync(string productId, string channel, string receiptData, bool sandbox)
        {
            switch (channel)
            {
                case BTServiceConst.CHANNEL_APP_STORE: return await VerifyReceiptAppStoreAsync(productId, receiptData, sandbox);
                default: return new ApiResult { code = this.SetResponseForbidden(), msg = "Unsupported Channel" };
            }
        }

        #region iTunes App Store
        private async Task<ApiResult> VerifyReceiptAppStoreAsync(string productId, string receiptData, bool sandbox)
        {
            var res = await SendReceiptAppStoreAsync(productId, receiptData, sandbox);
            if (res.code != 200)
            {
                if (res.error.code == 21007)
                {
                    return await SendReceiptAppStoreAsync(productId, receiptData, true);
                }
                else if (res.error.code == 21008)
                {
                    return await SendReceiptAppStoreAsync(productId, receiptData, false);
                }
            }
            return res;
        }

        private async Task<ApiResult> SendReceiptAppStoreAsync(string productId, string receiptData, bool sandbox)
        {
            //购买凭证验证地址  
            const string certificateUrl = "https://buy.itunes.apple.com/verifyReceipt";
            //测试的购买凭证验证地址   
            const string certificateUrlTest = "https://sandbox.itunes.apple.com/verifyReceipt";

            var url = sandbox ? certificateUrlTest : certificateUrl;
            var postBody = new Dictionary<string, string>() { { "receipt-data", receiptData } };

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(postBody, Formatting.None), System.Text.Encoding.UTF8, "application/json");
                var msg = await client.PostAsync(url, content);
                var result = await msg.Content.ReadAsStringAsync();
                var jsonResult = JsonConvert.DeserializeObject<JObject>(result);
                var statusCode = jsonResult.GetValue("status").Value<int>();
                if (statusCode == 0)
                {
                    string transactionId = null;
                    string receiptProductId = null;
                    var receipts = jsonResult["receipt"]["in_app"].HasValues ? jsonResult["receipt"]["in_app"] : jsonResult["receipt"];

                    foreach (var item in receipts.ToArray())
                    {
                        var product_id = item["product_id"].Value<string>();
                        if (product_id == productId)
                        {
                            transactionId = item["transaction_id"].Value<string>().ToString();
                            receiptProductId = product_id;
                        }
                    }

                    BTMemberProduct product = null;
                    if (!string.IsNullOrEmpty(receiptProductId) && BTMemberProduct.TryParseIAPProductId(productId, out product))
                    {
                        var suc = memberService.RechargeMember(dbContext, new BTMemberOrder
                        {
                            OrderKey = transactionId,
                            AccountId = this.GetHeaderAccountId(),
                            ProductId = productId,
                            ReceiptData = receiptData,
                            MemberType = product.MemberType,
                            ChargeTimes = product.ChargeTimes
                        });

                        if (suc)
                        {
                            return new ApiResult { code = 200 };
                        }
                        else
                        {
                            return new ApiResult { code = 404, error = new ErrorResult { code = 404, msg = "Is A Completed Order" } };
                        }
                    }
                    else
                    {
                        return new ApiResult { code = 400, error = new ErrorResult { code = 400, msg = "Unmatched Product Id" } };
                    }
                }
                else
                {
                    return new ApiResult { code = 400, error = new ErrorResult { code = statusCode } };
                }

            }
        }
        #endregion
    }
}