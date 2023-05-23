namespace GameSwiftSDK.Id.Responses
{
	/// <summary>
	/// Response given after successful <a href="https://id.gameswift.io/swagger/#/default/WalletController_getWalletBalance">GET /api/{idVersion}/wallet/{walletId}/balance</a> request.
	/// </summary>
	public class WalletBalanceResponse
	{
		/// <summary>
		/// Wallet's available NFTs.
		/// </summary>
		public Nft[] nfts;

		/// <summary>
		/// Wallet's available tokens.
		/// </summary>
		public Token[] tokens;

		/// <summary>
		/// Wallet's NFT information.
		/// </summary>
		public class Nft
		{
			/// <summary>
			/// NFT's contract address.
			/// </summary>
			public string address;

			/// <summary>
			/// NFT's balance.
			/// </summary>
			public int balance;

			/// <summary>
			/// NFT's unique ID.
			/// </summary>
			public string id;
		}

		/// <summary>
		/// Wallet's token information.
		/// </summary>
		public class Token
		{
			/// <summary>
			/// Token's contract address.
			/// </summary>
			public string address;

			/// <summary>
			/// Token's balance.
			/// </summary>
			public string balance;
		}
	}
}