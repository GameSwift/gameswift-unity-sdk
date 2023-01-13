using System.Collections.Generic;

namespace GameSwiftSDK.Id.Responses
{
    /// <summary>
    /// Response from "GET /api/health" request
    /// </summary>
    public class HealthResponse 
    {
        /// <summary>
        /// Retrieved status information
        /// </summary>
        public string status;

        /// <summary>
        /// Retrieved information
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> info;

        /// <summary>
        /// Retrieved error information
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> error;

        /// <summary>
        /// Retrieved details information
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> details;
    }
}