using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    public class AppListsController : Controller
    {
        [HttpGet]
        public void GetAppList(string deviceId, int platform, string uniqueId, string channel, string bundleId, string urlSchemes)
        {

        }
    }
}