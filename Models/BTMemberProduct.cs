namespace BTBaseWebAPI.Models
{
    public partial class BTMemberProduct
    {
        /// Product Id : {app_identifier}.iap.member.{member_type_key}.{charge_times_seconds}
        /// eg: cn.game.bahamut.xgame.iap.member.2.2592000 
        ///Sample product id represent: member type: Premium(check BTServiceConst.cs), charge times: 30 days

        public string ProductId { get; set; }
        public int MemberType { get; set; }
        public double ChargeTimes { get; set; }
    }

    public partial class BTMemberProduct
    {
        public static BTMemberProduct ParseIAPProductId(string iapProductId)
        {
            if (CommonRegexTestUtil.TestPattern(iapProductId, @"^[0-9a-zA-Z-_.]+\.iap.member\.[0-9]\.[0-9]+$"))
            {
                var productInfo = System.Text.RegularExpressions.Regex.Replace(iapProductId, @"[0-9a-zA-Z-_.]+\.iap.member\.", "").Split(".");
                return new BTMemberProduct
                {
                    ProductId = iapProductId,
                    MemberType = int.Parse(productInfo[0]),
                    ChargeTimes = double.Parse(productInfo[1])
                };
            }
            return null;
        }

        public static bool TryParseIAPProductId(string iapProductId, out BTMemberProduct product)
        {
            product = ParseIAPProductId(iapProductId);
            return product != null;
        }
    }
}