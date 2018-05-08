
public class CommonRegexTestUtil
{
    public const string PATTERN_CHINESE = @"[\u4e00-\u9fa5]";
    public const string PATTERN_HTML = @"<(\S*?)[^>]*>.*?</\1>|<.*? />";
    public const string PATTERN_EMAIL = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
    public const string PATTERN_URL = @"[a-zA-z]+://[^\s]*";
    public const string PATTERN_ACOUNT_ID = @"[1-9][0-9]{5,23}";
    public const string PATTERN_USERNAME = @"^[a-zA-Z][a-zA-Z0-9_]{4,19}$";
    public const string PATTERN_PASSWORD_HASH = @"^[a-zA-Z0-9_]{4,}$";
    public const string PATTERN_PHONE_NO = @"\+?[0-9]+[0-9\-]+[0-9]";
    public const string PATTERN_CHINESE_PHONE = @"\d{3}-\d{8}|\d{4}-\d{7}";
    public const string PATTERN_QQ = @"[1-9][0-9]{4,}";
    public const string PATTERN_CHINESE_POST_CODE = @"[1-9]\d{5}(?!\d)";
    public const string PATTERN_CHINESE_ID_CARD = @"\d{15}|\d{18}";
    public const string PATTERN_CHINESE_IP_ADDRESS = @"\d+\.\d+\.\d+\.\d+";

    public static bool TestPattern(string input, string pattern)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, pattern);
    }
}