namespace GameSwiftSDK.Id.Responses
{
    /// <summary>
    /// Success response from "GET /api/wallet/{userId}" request
    /// </summary>
    public class WalletResponse
    {
        /// <summary>
        /// Retrieved address
        /// </summary>
        public string address;
        /// <summary>
        /// Retrieved wallet type
        /// </summary>
        public string walletType;
    }
}