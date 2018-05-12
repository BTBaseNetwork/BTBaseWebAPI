using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BTBaseWebAPI.Controllers;
using BTBaseServices.DAL;
using BTBaseServices.Services;
using BTBaseServices.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class AccountsController : Controller
    {
        private readonly BTBaseDbContext dbContext;
        private readonly AccountService accountService;
        private readonly SessionService sessionService;

        public AccountsController(BTBaseDbContext dbContext, AccountService accountService, SessionService sessionService)
        {
            this.dbContext = dbContext;
            this.accountService = accountService;
            this.sessionService = sessionService;
        }

        [HttpPost]
        public object Regist(string username, string password)
        {
            if (!CommonRegexTestUtil.TestPattern(username, CommonRegexTestUtil.PATTERN_USERNAME))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "User Name Is Unsupport" };
            }

            if (!CommonRegexTestUtil.TestPattern(password, CommonRegexTestUtil.PATTERN_PASSWORD_HASH))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "Password Is Unsupport" };
            }

            if (accountService.IsUsernameExists(dbContext, username))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "User Name Is Registed" };
            }

            var newAccount = new BTAccount
            {
                UserName = username,
                Nick = username,
                Password = password
            };

            newAccount = accountService.CreateNewAccount(dbContext, newAccount);
            return new ApiResult
            {
                code = this.SetResponseOK(),
                msg = "Success",
                content = new
                {
                    AccountId = newAccount.AccountId,
                    UserName = newAccount.UserName
                }
            };

        }

        [Authorize]
        [HttpGet("Profile")]
        public object GetAccountProfile()
        {
            var ac = accountService.GetProfile(dbContext, this.GetHeaderAccountId());
            if (ac != null)
            {
                return new ApiResult
                {
                    code = this.SetResponseOK(),
                    content = new
                    {
                        AccountId = ac.AccountId,
                        UserName = ac.UserName,
                        AccountTypes = ac.AccountTypes,
                        Nick = ac.Nick,
                        Email = ac.Email,
                        Mobile = ac.Mobile,
                        SignDateTs = ac.SignDateTs
                    }
                };
            }
            else
            {
                return new ApiResult { code = this.SetResponseNotFound(), msg = "No Such Account" };
            }
        }

        [HttpGet("Username/{username}")]
        public object CheckUsernameExists(string username)
        {
            var found = accountService.IsUsernameExists(dbContext, username);
            return new ApiResult
            {
                code = found ? this.SetResponseOK() : this.SetResponseNotFound()
            };
        }

        [Authorize]
        [HttpPost("Nick")]
        public object UpdateNickName(string newNick)
        {
            var updated = accountService.UpdateNick(dbContext, this.GetHeaderAccountId(), newNick);
            return new ApiResult
            {
                code = updated ? this.SetResponseOK() : this.SetResponseNotFound(),
                content = updated,
                error = updated ? null : new ErrorResult { code = 400, msg = "No Account" }
            };
        }

        [Authorize]
        [HttpPost("Password")]
        public object UpdatePassword(string originPassword, string newPassword)
        {
            var updated = accountService.UpdatePassword(dbContext, this.GetHeaderAccountId(), originPassword, newPassword);
            return new ApiResult
            {
                code = updated ? this.SetResponseOK() : this.SetResponseForbidden(),
                content = updated,
                error = updated ? null : new ErrorResult { code = 403, msg = "Origin Password Not Match" }
            };
        }

        [Authorize]
        [HttpPost("Email")]
        public object RequestUpdateEmail(string newEmail)
        {

            

            

            return null;
        }

        [Authorize]
        [HttpPost("EmailVerifyCode")]
        public object ConfirmUpdateEmail(string verifyCode)
        {

            return null;
        }
    }
}