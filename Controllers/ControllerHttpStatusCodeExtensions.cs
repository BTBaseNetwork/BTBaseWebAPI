using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers
{
    public static class ControllerHttpStatusCodeExtensions
    {
        public static int SetResponseStatusCode(this Controller controller, HttpStatusCode statusCode)
        {
            controller.Response.StatusCode = (int)statusCode;
            return (int)statusCode;
        }

        public static int SetResponseOK(this Controller controller)
        {
            return SetResponseStatusCode(controller, HttpStatusCode.OK);
        }

        public static int SetResponseForbidden(this Controller controller)
        {
            return SetResponseStatusCode(controller, HttpStatusCode.Forbidden);
        }

        public static int SetResponseNotFound(this Controller controller)
        {
            return SetResponseStatusCode(controller, HttpStatusCode.NotFound);
        }
    }
}