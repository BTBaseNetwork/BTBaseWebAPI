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

        [HttpPost]
        public object Regist([FromServices]BTBaseDbContext dbContext, [FromServices]AccountService accountService, string username, string password, string email)
        {
            if (!CommonRegexTestUtil.TestPattern(username, CommonRegexTestUtil.PATTERN_USERNAME))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "User Name Is Unsupport" };
            }

            if (!CommonRegexTestUtil.TestPattern(password, CommonRegexTestUtil.PATTERN_PASSWORD_HASH))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "Password Is Unsupport" };
            }

            if (!CommonRegexTestUtil.TestPattern(email, CommonRegexTestUtil.PATTERN_EMAIL))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "Email Is Unsupport" };
            }

            if (accountService.IsUsernameExists(dbContext, username))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "User Name Is Registed" };
            }

            var newAccount = new BTAccount
            {
                UserName = username,
                Nick = username,
                Password = password,
                Email = email
            };

            newAccount = accountService.CreateNewAccount(dbContext, newAccount);
            SendWelcomeSignUpEmail(newAccount);
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

        private void SendWelcomeSignUpEmail(BTAccount newAccount)
        {
            var mail = new AliApilUtils.AliMail
            {
                Action = "SingleSendMail",
                AccountName = "admin@btbase.mobi",
                ReplyToAddress = false,
                AddressType = AliApilUtils.AliMail.ADDR_TYPE_ACCOUNT,
                ToAddress = newAccount.Email,
                FromAlias = "Bluetime Admin",
                Subject = "Thanks for sign up Bluetime",
                HtmlBody = string.Format("<p>Account ID:{0}</p><br/><p>User Name:{1}</p><br/>", newAccount.AccountId, newAccount.UserName),
                ClickTrace = AliApilUtils.AliMail.CLICK_TRACE_OFF
            };

            var comFields = new AliApilUtils.CommonReqFields
            {
                AccessKeyId = Environment.GetEnvironmentVariable("ALIYUN_DM_ACCESSKEY"),
                RegionId = Environment.GetEnvironmentVariable("ALIYUN_DM_REGION"),
                AccesskeySecret = Environment.GetEnvironmentVariable("ALIYUN_DM_ACCESSKEY_SECRET"),
            };

            Task.Run(async () =>
            {
                await AliApilUtils.SendMailAsync(comFields, mail);
            });
        }

        [Authorize]
        [HttpGet("Profile")]
        public object GetAccountProfile([FromServices]BTBaseDbContext dbContext, [FromServices]AccountService accountService)
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
                        Email = ac.GetPartitialHidedEmail(),
                        Mobile = ac.GetPartitialHidedMobile(),
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
        public object CheckUsernameExists([FromServices]BTBaseDbContext dbContext, [FromServices]AccountService accountService, string username)
        {
            var found = accountService.IsUsernameExists(dbContext, username);
            return new ApiResult
            {
                code = found ? this.SetResponseOK() : this.SetResponseNotFound()
            };
        }

        [Authorize]
        [HttpPost("Nick")]
        public object UpdateNickName([FromServices]BTBaseDbContext dbContext, [FromServices]AccountService accountService, string newNick)
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
        public object UpdatePassword([FromServices]BTBaseDbContext dbContext, [FromServices]AccountService accountService, string password, string newPassword)
        {
            var updated = accountService.UpdatePassword(dbContext, this.GetHeaderAccountId(), password, newPassword);
            return new ApiResult
            {
                code = updated ? this.SetResponseOK() : this.SetResponseForbidden(),
                content = updated,
                error = updated ? null : new ErrorResult { code = 403, msg = "Origin Password Not Match" }
            };
        }

        [HttpPost("NewPassword")]
        public object ResetPassword([FromServices]BTBaseDbContext dbContext, [FromServices]AccountService accountService, [FromServices]SecurityCodeService SecurityCodeService,
        string securityCode, string accountId, string newPassword)
        {
            if (SecurityCodeService.VerifyCode(dbContext, accountId, BTSecurityCode.REQ_FOR_RESET_PASSWORD, securityCode))
            {
                var updated = accountService.ResetPassword(dbContext, accountId, newPassword);
                return new ApiResult
                {
                    code = updated ? this.SetResponseOK() : this.SetResponseForbidden(),
                    content = updated,
                    error = updated ? null : new ErrorResult { code = 403, msg = "Origin Password Not Match" }
                };
            }
            else
            {
                return new ApiResult
                {
                    code = this.SetResponseForbidden(),
                    error = new ErrorResult { code = 403, msg = "Authentication Verify Failed" }
                };
            }
        }

        [HttpPost("SecurityCode/NewPassword/Email")]
        public async Task<object> RequestResetPasswordVerifyCodeAsync([FromServices]BTBaseDbContext dbContext, [FromServices]AccountService accountService, [FromServices]SecurityCodeService SecurityCodeService,
        string accountId, string email)
        {
            return await SendVerifyCodeByAliMailAsync(dbContext, accountService, SecurityCodeService, accountId, email, BTSecurityCode.REQ_FOR_RESET_PASSWORD);
        }

        [Authorize]
        [HttpPost("NewEmail")]
        public object UpdateEmail([FromServices]BTBaseDbContext dbContext, [FromServices]AccountService accountService, [FromServices]SecurityCodeService SecurityCodeService,
        string securityCode, string newEmail)
        {
            if (SecurityCodeService.VerifyCode(dbContext, this.GetHeaderAccountId(), BTSecurityCode.REQ_FOR_RESET_EMAIL, securityCode))
            {
                var updated = accountService.UpdateEmail(dbContext, this.GetHeaderAccountId(), newEmail);
                return new ApiResult
                {
                    code = updated ? this.SetResponseOK() : this.SetResponseForbidden(),
                    content = updated,
                    error = updated ? null : new ErrorResult { code = 403, msg = "Origin Password Not Match" }
                };
            }
            else
            {
                return new ApiResult
                {
                    code = this.SetResponseForbidden(),
                    error = new ErrorResult { code = 403, msg = "Authentication Verify Failed" }
                };
            }
        }

        [Authorize]
        [HttpPost("SecurityCode/NewEmail/Email")]
        public async Task<object> RequestResetEmailVerifyCodeAsync([FromServices]BTBaseDbContext dbContext, [FromServices]AccountService accountService, [FromServices]SecurityCodeService securityCodeService, string email)
        {
            return await SendVerifyCodeByAliMailAsync(dbContext, accountService, securityCodeService, this.GetHeaderAccountId(), email, BTSecurityCode.REQ_FOR_RESET_EMAIL);
        }

        #region Send Verify Code
        private async Task<object> SendVerifyCodeByAliMailAsync(BTBaseDbContext dbContext, AccountService accountService, SecurityCodeService securityCodeService, string accountId, string email, int reqType)
        {
            var account = accountService.GetProfile(dbContext, accountId);
            if (account != null && !string.IsNullOrWhiteSpace(account.Email) && account.Email.ToLower() == email.ToLower())
            {
                var code = securityCodeService.RequestNewCode(dbContext, accountId, reqType, BTSecurityCode.REC_TYPE_EMAIL, email, TimeSpan.FromDays(1));
                var aliMailSenderInfo = new AliApilUtils.CommonReqFields
                {
                    AccessKeyId = Environment.GetEnvironmentVariable("ALIYUN_DM_ACCESSKEY"),
                    RegionId = Environment.GetEnvironmentVariable("ALIYUN_DM_REGION"),
                    AccesskeySecret = Environment.GetEnvironmentVariable("ALIYUN_DM_ACCESSKEY_SECRET"),
                };

                if (code != null && await securityCodeService.SendCodeAsync(code, aliMailSenderInfo))
                {
                    return new ApiResult
                    {
                        code = this.SetResponseOK()
                    };
                }
                else
                {
                    return new ApiResult
                    {
                        code = this.SetResponseServerError(),
                        error = new ErrorResult { code = 500, msg = "Request Error" }
                    };
                }
            }
            else
            {
                return new ApiResult
                {
                    code = this.SetResponseForbidden(),
                    error = new ErrorResult { code = 403, msg = "No Account Match Email" }
                };
            }
        }
        #endregion
    }
}