using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    public class SessionsController : Controller
    {
        [HttpGet]
        public void Login(string userstring, string password)
        {

        }

        [HttpDelete]
        public void Logout()
        {

        }

        [HttpGet("DeviceSession")]
        public void GetDeviceSession()
        {
            
        }
    }
}