namespace GameSwiftSDK.Core
{
	/// <summary>
	/// Response given after failed request. Presented in a plain string message.
	/// </summary>
	public class SdkFailResponse : BaseSdkFailResponse
	{
		/// <summary>
		/// Failure message.
		/// </summary>
		public override string Message => message;

		/// <summary>
		/// Failure message.
		/// </summary>
		public string message;

		/// <summary>
		/// Constructor for SDK fail response.
		/// </summary>
		/// <param name="errorMessage">Passed fail message</param>
		public SdkFailResponse (string errorMessage)
		{
			message = errorMessage;
		}
	}
}