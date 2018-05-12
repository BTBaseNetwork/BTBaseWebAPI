using System.Net.Http;
using System.Threading.Tasks;

public class AliApilUtils
{


    public class CommonFields
    {
        //否	返回值的类型，支持 JSON 与 XML。默认为 XML。
        public string Format { get; set; }
        //是	API 版本号，为日期形式：YYYY-MM-DD。如果参数 RegionID 是 cn-hangzhou，则版本对应为2015-11-23；如参数 RegionID 是cn-hangzhou 以外其他 Region，比如 ap-southeast-1，则版本对应为2017-06-22。
        public string Version { get; set; }
        //是	阿里云颁发给用户的访问服务所用的密钥 ID。
        public string AccessKeyId { get; set; }
        //是	签名结果串，关于签名的计算方法，请参见签名机制。
        public string Signature { get; set; }
        //是	签名方式，目前支持 HMAC-SHA1。
        public string SignatureMethod { get; set; }
        //是	请求的时间戳。日期格式按照 ISO8601 标准表示，并需要使用 UTC 时间。格式为YYYY-MM-DDThh:mm:ssZ。 例如，2015-11-23T04:00:00Z（为北京时间 2015 年 11 月 23 日 12 点 0 分 0 秒）。
        public string Timestamp { get; set; }
        //是	签名算法版本，目前版本是1.0。
        public string SignatureVersion { get; set; }
        //是	唯一随机数，用于防止网络重放攻击。用户在不同请求间要使用不同的随机数值。
        public string SignatureNonce { get; set; }
        //否	机房信息 ，目前支持 cn-hangzhou、ap-southeast-1、ap-southeast-2。    
        public string RegionId { get; set; }
    }

    public class AliMail
    {
        public string AccountName { get; set; }
        public bool ReplyToAddress { get; set; }
        public int AddressType { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
    }
    public async Task SendMailAsync(AliMail mail)
    {
        // https://dm.aliyuncs.com/?Action=SingleSendMail
        // &AccountName=test@example.com
        // &ReplyToAddress=true
        // &AddressType=1   
        // &ToAddress=test1@example.com
        // &Subject=Subject
        // &HtmlBody=body
        // &<公共请求参数>

        using (var client = new HttpClient())
        {
            string url = "https://dm.aliyuncs.com/?Action=SingleSendMail";
            var msg = await client.GetAsync(url);
            var result = await msg.Content.ReadAsStringAsync();
        }
    }
}