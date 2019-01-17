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
    public class UserAssetsController : Controller
    {
        private readonly BTBaseDbContext dbContext;
        private readonly UserAssetService userAssetService;

        public UserAssetsController(BTBaseDbContext dbContext, UserAssetService userAssetService)
        {
            this.dbContext = dbContext;
            this.userAssetService = userAssetService;
        }

        [Authorize]
        [HttpGet]
        public object GetAssets()
        {
            var content = userAssetService.GetAssets(dbContext, this.GetHeaderAccountId(), this.GetHeaderAppBundleId());
            return new ApiResult
            {
                code = 200,
                content = new
                {
                    count = content.Count(),
                    assets = content
                }
            };
        }

        [Authorize]
        [HttpGet("AssetsType/{assetsType}")]
        public object GetAssets(string assetsType)
        {
            var content = userAssetService.GetAssetsByCategory(dbContext, this.GetHeaderAccountId(), this.GetHeaderAppBundleId(), assetsType);
            return new ApiResult
            {
                code = 200,
                content = new
                {
                    count = content.Count(),
                    assets = content
                }
            };
        }

        [Authorize]
        [HttpGet("Id/{assetsId}")]
        public object GetAssetsById(string assetsId)
        {
            var content = userAssetService.GetAssets(dbContext, this.GetHeaderAccountId(), this.GetHeaderAppBundleId(), assetsId);
            return new ApiResult
            {
                code = 200,
                content = new
                {
                    count = content.Count(),
                    assets = content
                }
            };
        }

        [Authorize]
        [HttpPost("{assetsId}")]
        public object NewAssets(string assetsId, string assets, string category, int amount, long ts, string signature)
        {
            var account = this.GetHeaderAccountId();
            var session = this.GetHeaderSession();
            var bundleId = this.GetHeaderAppBundleId();

            var key = string.Format("{0}:{1}:{2}:{3}", bundleId, ts, account, session);
            key = BahamutCommon.Utils.StringUtil.Md5String(key);

            if (!BahamutCommon.Utils.SignatureUtil.TestStringParametersSignature(signature, key, assetsId, assets, category, amount.ToString()))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return new ApiResult
                {
                    code = Response.StatusCode,
                    msg = "Invalid Signature"
                };
            }

            var newAssets = new BTUserAsset
            {
                AccountId = this.GetHeaderAccountId(),
                AppBundleId = this.GetHeaderAppBundleId(),
                AssetsId = assetsId,
                Assets = assets,
                Category = category,
                Amount = amount,
                CreateDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            var newModel = userAssetService.AddAssets(dbContext, newAssets);
            return new ApiResult
            {
                code = newModel != null ? 200 : 403,
                content = newModel
            };
        }

        [Authorize]
        [HttpPost("Updates/{id}")]
        public object UpdateAssets(long id, string assetsId, string assets, string category, int amount, long ts, string signature)
        {
            var account = this.GetHeaderAccountId();
            var session = this.GetHeaderSession();
            var bundleId = this.GetHeaderAppBundleId();

            var key = string.Format("{0}:{1}:{2}:{3}", bundleId, ts, account, session);
            key = BahamutCommon.Utils.StringUtil.Md5String(key);

            if (!BahamutCommon.Utils.SignatureUtil.TestStringParametersSignature(signature, key, id.ToString(), assetsId, assets, category, amount.ToString()))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return new ApiResult
                {
                    code = Response.StatusCode,
                    msg = "Invalid Signature"
                };
            }

            var modifiedAssets = new BTUserAsset
            {
                Id = id,
                AssetsId = assetsId,
                AccountId = this.GetHeaderAccountId(),
                AppBundleId = this.GetHeaderAppBundleId(),
                Assets = assets,
                Category = category,
                Amount = amount,
                ModifiedDate = DateTime.UtcNow
            };
            var newModel = userAssetService.UpdateAssets(dbContext, modifiedAssets);
            return new ApiResult
            {
                code = newModel != null ? 200 : 403,
                content = newModel
            };
        }
    }
}