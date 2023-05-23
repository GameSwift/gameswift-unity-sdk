namespace GameSwiftSDK.Core
{
	/// <summary>
	/// Custom request headers needed to connect with GameSwift API.
	/// </summary>
	public enum CustomHeader
	{
		None,

		/// <summary>
		/// Adds "x-refresh-token" header
		/// </summary>
		RefreshToken,

		/// <summary>
		/// Adds "Authorization Bearer" header
		/// </summary>
		AccessToken,

		/// <summary>
		/// Sets "Content-Type" header to "application/x-www-form-urlencoded"
		/// </summary>
		WwwContentType
	}
}