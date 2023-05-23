using System.Collections.Generic;

namespace GameSwiftSDK.Core
{
	/// <summary>
	/// Helper class for setting up request data.
	/// </summary>
	public class RequestData
	{
		/// <summary>
		/// Request's url.
		/// </summary>
		public readonly string uri;

		/// <summary>
		/// Request's body.
		/// </summary>
		public readonly string body;

		/// <summary>
		/// Request's headers.
		/// </summary>
		public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

		/// <summary>
		/// Constructor for response data.
		/// </summary>
		/// <param name="uri">Request's url</param>
		/// <param name="body">Request's body</param>
		public RequestData (string uri, string body)
		{
			this.uri = uri;
			this.body = body;
		}

		/// <summary>
		/// Constructor for the response data.
		/// </summary>
		/// <param name="requestUri">Request's uri</param>
		public RequestData (string requestUri)
		{
			uri = requestUri;
			body = "";
		}

		/// <summary>
		/// Setups request's headers.
		/// </summary>
		/// <param name="customHeader">Type of the custom header</param>
		/// <param name="customHeaderText">Value of the custom header</param>
		public void SetupHeaders (CustomHeader customHeader, string customHeaderText)
		{
			Headers.Add("Content-Type", "application/json");
			Headers.Add("accept", "*/*");
			AddCustomHeader(customHeader, customHeaderText);
		}

		/// <summary>
		/// Adds custom headers to the Headers dictionary.
		/// </summary>
		/// <param name="customHeader">Type of the custom header</param>
		/// <param name="customHeaderText">Value of the custom header</param>
		private void AddCustomHeader (CustomHeader customHeader, string customHeaderText)
		{
			customHeaderText = string.IsNullOrEmpty(customHeaderText) ? string.Empty : customHeaderText;

			switch (customHeader)
			{
				case CustomHeader.AccessToken:
					Headers.Add("Authorization", $"Bearer {customHeaderText}");
					break;
				case CustomHeader.RefreshToken:
					Headers.Add("x-refresh-token", customHeaderText);
					break;
				case CustomHeader.WwwContentType:
					Headers["Content-Type"] = "application/x-www-form-urlencoded";
					break;
			}
		}
	}
}