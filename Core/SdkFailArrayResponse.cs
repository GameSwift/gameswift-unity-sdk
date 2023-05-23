namespace GameSwiftSDK.Core
{
	/// <summary>
	/// Response given after failed request. Presented in an array of messages.
	/// </summary>
	public class SdkFailArrayResponse : BaseSdkFailResponse
	{
		/// <summary>
		/// Failure messages joined.
		/// </summary>
		public override string Message => string.Join("\n", message);

		/// <summary>
		/// Failure messages.
		/// </summary>
		public string[] message;
	}
}