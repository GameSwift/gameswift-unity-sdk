namespace GameSwiftSDK.Id.Responses
{
	/// <summary>
	/// Response given after successful <a href="https://id.gameswift.io/swagger/#/default/HealthController_checkHealth">GET /api/{idVersion}/health</a> request.
	/// </summary>
	public class HealthResponse
	{
		/// <summary>
		/// API status.
		/// </summary>
		public string status;

		/// <summary>
		/// [Non relevant] API modules base information.
		/// </summary>
		public Info info;

		/// <summary>
		/// [Non relevant] API modules error information.
		/// </summary>
		public Error error;

		/// <summary>
		/// [Non relevant] API modules detailed information.
		/// </summary>
		public Details details;

		/// <summary>
		/// [Non relevant] API modules base information used in <a href="https://id.gameswift.io/swagger/#/default/HealthController_checkHealth">GET /api/{idVersion}/health</a> request.
		/// </summary>
		public class Info
		{
			// non relevant
		}

		/// <summary>
		/// [Non relevant] API modules error information used in <a href="https://id.gameswift.io/swagger/#/default/HealthController_checkHealth">GET /api/{idVersion}/health</a> request.
		/// </summary>
		public class Error
		{
			// non relevant
		}

		/// <summary>
		/// [Non relevant] Detailed information used in <a href="https://id.gameswift.io/swagger/#/default/HealthController_checkHealth">GET /api/{idVersion}/health</a> request.
		/// </summary>
		public class Details
		{
			// non relevant
		}
	}
}