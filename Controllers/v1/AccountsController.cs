using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTBaseWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BTBaseWebAPI.Controllers;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class AccountsController : Controller
    {
        private readonly DAL.BTBaseDbContext dbContext;
        private readonly Services.AccountService accountService;
        public AccountsController(DAL.BTBaseDbContext dbContext, Services.AccountService accountService)
        {
            this.dbContext = dbContext;
            this.accountService = accountService;
        }

        [HttpPost]
        public object Regist(string username, string password)
        {
            var newAccount = new BTAccount
            {
                UserName = username,
                Nick = username,
                Password = password,
            };
            newAccount = accountService.CreateNewAccount(dbContext, newAccount);
            return new ApiResult
            {
                code = 200,
                msg = "Success",
                content = new
                {
                    AccountId = newAccount.AccountId,
                    UserName = newAccount.UserName,
                    Nick = newAccount.Nick,
                    Email = newAccount.Email,
                    Mobile = newAccount.Mobile,
                    SignDateTs = newAccount.SignDateTs
                }
            };
        }

        [HttpGet("Username/{username}")]
        public object CheckUsernameAvaiable(string username)
        {
            return new ApiResult
            {
                code = 200,
                content = dbContext.BTAccount.Count(x => x.UserName == username)
            };
        }

        [HttpPut("Nick")]
        public object UpdateNickName([FromBody]string newNick)
        {
            return new ApiResult
            {
                code = 200,
                content = accountService.UpdateNick(dbContext, this.GetHeaderAccountId(), newNick)
            };
        }

        public class UpdatePasswordFrom
        {
            public string originPassword;
            public string newPassword;
        }

        [HttpPut("Password")]
        public object UpdatePassword([FromBody]UpdatePasswordFrom form)
        {
            return new ApiResult
            {
                code = 200,
                content = accountService.UpdatePassword(dbContext, this.GetHeaderAccountId(), form.originPassword, form.newPassword)
            };
        }
    }
}