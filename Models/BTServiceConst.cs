namespace BTService
{
    public class BTServiceConst
    {
        /// Platform Id
        public const int PLATFORM_UNKNOW = -1;
        public const int PLATFORM_IOS = 0;
        public const int PLATFORM_ANDROID = 1;
        public const int PLATFORM_WINDOWS = 2;
        public const int PLATFORM_MACOS = 3;
        public const int PLATFORM_LINUX = 4;

        ///Store Channel Id
        public const string CHANNEL_UNKNOW = "UNKNOW";
        public const string CHANNEL_APP_STORE = "APPSTORE";
        public const string CHANNEL_GOOGLE_PLAY = "GOOGLEPLAY";
        public const string CHANNEL_MS_MARKET = "MSMARKET";
        public const string CHANNEL_TAP_TAP = "TAPTAP";

        /// Account Type
        public const int ACCOUNT_TYPE_LOGOUT = -1;
        public const int ACCOUNT_TYPE_BTPLATFORM = 0;
        public const int ACCOUNT_TYPE_GAME_PRODUCER = 1;
        public const int ACCOUNT_TYPE_GAME_PLAYER = 2;

        /// Account Const
        public const string USER_ID_UNLOGIN = "USERID_UNLOGIN";
        public const string ACCOUNT_ID_UNLOGIN = "000000";

        /// Member Type
        public const int MEMBER_TYPE_LOGOUT = -1;
        public const int MEMBER_TYPE_FREE = 0;
        public const int MEMBER_TYPE_EXPIRED = 1;
        public const int MEMBER_TYPE_PREMIUM = 2;
        public const int MEMBER_TYPE_ADVANCED = 3;
    }
}