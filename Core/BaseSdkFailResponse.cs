namespace GameSwiftSDK.Core
{
	/// <summary>
	/// Base class for fail responses. As for now 2 fail responses exist - normal and array.
	/// </summary>
	public abstract class BaseSdkFailResponse
	{
		/// <summary>
		/// Failure message.
		/// </summary>
		public abstract string Message { get; }

		/// <summary>
		/// Retrieved status code.
		/// </summary>
		public long statusCode;
	}
}