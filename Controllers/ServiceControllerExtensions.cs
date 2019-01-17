using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers
{
    public static class ServiceControllerExtensions
    {
        public static string GetHeaderLangCode(this Controller controller)
        {
            return controller.Request.Headers["lang"];
        }

        public static string GetHeaderAccountId(this Controller controller)
        {
            return controller.Request.Headers["accountId"];
        }

        public static string GetHeaderDeviceId(this Controller controller)
        {
            return controller.Request.Headers["devId"];
        }

        public static string GetHeaderClientId(this Controller controller)
        {
            return controller.Request.Headers["clientId"];
        }

        public static string GetHeaderAppBundleId(this Controller controller)
        {
            return controller.Request.Headers["appBundleId"];
        }

        public static string GetHeaderSession(this Controller controller)
        {
            return controller.Request.Headers["session"];
        }

        public static string GetHeaderDeviceName(this Controller controller)
        {
            return controller.Request.Headers["devName"];
        }

        public static string GetHeaderPlatformId(this Controller controller)
        {
            return controller.Request.Headers["platId"];
        }

        public static string GetHeaderDeviceModel(this Controller controller)
        {
            return controller.Request.Headers["devModel"];
        }
    }
}