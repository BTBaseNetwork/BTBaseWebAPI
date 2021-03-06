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
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class AccountsController : Controller
    {
        private readonly BTBaseDbContext dbContext;
        private readonly AccountService accountService;

        public AccountsController(BTBaseDbContext dbContext, AccountService accountService)
        {
            this.dbContext = dbContext;
            this.accountService = accountService;
        }


        [HttpPost]
        public object Regist(string username, string password, string email)
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
            var section = Startup.Configuration.GetSection($"emailTemplates:signup_{this.GetHeaderLangCode()}");
            if (!section.Exists())
            {
                section = Startup.Configuration.GetSection("emailTemplates:signup");
            }
            if (section.Exists())
            {
                var mail = new AliApilUtils.AliMail
                {
                    Action = "SingleSendMail",
                    AccountName = section["account"],
                    ReplyToAddress = false,
                    AddressType = AliApilUtils.AliMail.ADDR_TYPE_ACCOUNT,
                    ToAddress = newAccount.Email,
                    FromAlias = section["alias"],
                    Subject = section["subject"],
                    HtmlBody = string.Format(section["htmlbody"], newAccount.AccountId, newAccount.UserName),
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
            else
            {
                Console.WriteLine("No Email Template For Sign Up");
            }
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
                error = updated ? null : new ErrorResult { code = 400, msg = "No Account" }
            };
        }

        [Authorize]
        [HttpPost("Password")]
        public object UpdatePassword(string password, string newPassword)
        {
            var updated = accountService.UpdatePassword(dbContext, this.GetHeaderAccountId(), password, newPassword);
            return new ApiResult
            {
                code = updated ? this.SetResponseOK() : this.SetResponseForbidden(),
                error = updated ? null : new ErrorResult { code = 403, msg = "Origin Password Not Match" }
            };
        }

        [HttpPost("NewPassword")]
        public object ResetPassword([FromServices]SecurityCodeService SecurityCodeService, string securityCode, string accountId, string newPassword)
        {
            if (SecurityCodeService.VerifyCode(dbContext, accountId, BTSecurityCode.REQ_FOR_RESET_PASSWORD, securityCode))
            {
                var updated = accountService.ResetPassword(dbContext, accountId, newPassword);
                return new ApiResult
                {
                    code = updated ? this.SetResponseOK() : this.SetResponseNotFound(),
                    error = updated ? null : new ErrorResult { code = 404, msg = "No Account" }
                };
            }
            else
            {
                return new ApiResult
                {
                    code = this.SetResponseForbidden(),
                    error = new ErrorResult { code = 403, msg = "Code Verify Failed" }
                };
            }
        }

        [HttpPost("SecurityCode/NewPassword/Email")]
        public async Task<object> RequestResetPasswordVerifyCodeAsync([FromServices]SecurityCodeService SecurityCodeService, string accountId, string email)
        {
            return await SendVerifyCodeByAliMailAsync(SecurityCodeService, accountId, email, BTSecurityCode.REQ_FOR_RESET_PASSWORD);
        }

        [Authorize]
        [HttpPost("NewEmail")]
        public object UpdateEmail([FromServices]SecurityCodeService SecurityCodeService, string securityCode, string newEmail)
        {
            if (SecurityCodeService.VerifyCode(dbContext, this.GetHeaderAccountId(), BTSecurityCode.REQ_FOR_RESET_EMAIL, securityCode))
            {
                var updated = accountService.UpdateEmail(dbContext, this.GetHeaderAccountId(), newEmail);
                return new ApiResult
                {
                    code = updated ? this.SetResponseOK() : this.SetResponseNotFound(),
                    error = updated ? null : new ErrorResult { code = 404, msg = "No Account" }
                };
            }
            else
            {
                return new ApiResult
                {
                    code = this.SetResponseForbidden(),
                    error = new ErrorResult { code = 403, msg = "Code Verify Failed" }
                };
            }
        }

        [Authorize]
        [HttpPost("SecurityCode/NewEmail/Email")]
        public async Task<object> RequestResetEmailVerifyCodeAsync([FromServices]SecurityCodeService securityCodeService, string email)
        {
            return await SendVerifyCodeByAliMailAsync(securityCodeService, this.GetHeaderAccountId(), email, BTSecurityCode.REQ_FOR_RESET_EMAIL);
        }

        #region Send Verify Code
        private async Task<object> SendVerifyCodeByAliMailAsync(SecurityCodeService securityCodeService, string accountId, string email, int reqType)
        {
            var account = accountService.GetProfile(dbContext, accountId);
            if (account != null && !string.IsNullOrWhiteSpace(account.Email) && account.Email.ToLower() == email.ToLower())
            {
                var section = Startup.Configuration.GetSection($"emailTemplates:securityCode_{reqType}_{this.GetHeaderLangCode()}");
                if (!section.Exists())
                {
                    section = Startup.Configuration.GetSection($"emailTemplates:securityCode_{reqType}");
                }
                if (section.Exists())
                {
                    var code = securityCodeService.RequestNewCode(dbContext, accountId, reqType, BTSecurityCode.REC_TYPE_EMAIL, email, TimeSpan.FromDays(1));
                    var mail = new AliApilUtils.AliMail
                    {
                        Action = "SingleSendMail",
                        AccountName = section["account"],
                        ReplyToAddress = false,
                        AddressType = AliApilUtils.AliMail.ADDR_TYPE_ACCOUNT,
                        ToAddress = email,
                        FromAlias = section["alias"],
                        Subject = string.Format(section["subject"], code.Code),
                        HtmlBody = string.Format(section["htmlbody"], code.Code, code.Expires.ToString()),
                        ClickTrace = AliApilUtils.AliMail.CLICK_TRACE_OFF
                    };

                    var reqFields = new AliApilUtils.CommonReqFields
                    {
                        AccessKeyId = Environment.GetEnvironmentVariable("ALIYUN_DM_ACCESSKEY"),
                        RegionId = Environment.GetEnvironmentVariable("ALIYUN_DM_REGION"),
                        AccesskeySecret = Environment.GetEnvironmentVariable("ALIYUN_DM_ACCESSKEY_SECRET"),
                    };

                    if (await AliApilUtils.SendMailAsync(reqFields, mail))
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
                        code = this.SetResponseServerError(),
                        error = new ErrorResult { code = 500, msg = "No Email Template" }
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