using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    public class AccountsController : Controller
    {
        [HttpPost]
        public void Rigist(string username, string password)
        {
            var newAccount = new BTAccount
            {
                UserName = username,
                Nick = username,
                Password = password,
            };
            
        }

        [HttpGet("Username")]
        public void CheckUsernameAvaiable(string username)
        {

        }

        [HttpPut("NickName")]
        public void UpdateNickName(string newNick)
        {

        }

        [HttpPut("Password")]
        public void UpdatePassword(string originPassword, string newPassword)
        {

        }
    }
}