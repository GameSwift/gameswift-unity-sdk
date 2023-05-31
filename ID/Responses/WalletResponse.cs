namespace GameSwiftSDK.Id.Responses
{
	/// <summary>
	/// Response given after successful <a href="https://id.gameswift.io/swagger/#/default/WalletController_getWallets">GET /api/{idVersion}/wallet</a> request.
	/// </summary>
	public class WalletResponse
	{
		/// <summary>
		/// User's wallet address.
		/// </summary>
		public string address;

		/// <summary>
		/// Games assigned to user's wallet.
		/// </summary>
		public Game[] games;

		/// <summary>
		/// Wallet name.
		/// </summary>
		public string name;

		/// <summary>
		/// Unique wallet ID.
		/// </summary>
		public string walletId;

		/// <summary>
		/// Chain where user's wallet is assigned to.
		/// </summary>
		public Chain chain;

		/// <summary>
		/// Games information.
		/// </summary>
		public class Game
		{
			/// <summary>
			/// Game's unique ID.
			/// </summary>
			public string gameId;

			/// <summary>
			/// Game types.
			/// </summary>
			public string[] types;
		}

		/// <summary>
		/// Chain information.
		/// </summary>
		public class Chain
		{
			/// <summary>
			/// Unique chain ID.
			/// </summary>
			public string chainId;

			/// <summary>
			/// Chain's name.
			/// </summary>
			public string chainName;
		}
	}
}