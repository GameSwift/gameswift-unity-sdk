using System;

namespace GameSwiftSDK.Id.Responses
{
    /// <summary>
    /// Get success response from default request
    /// </summary>
    [Serializable]
    public class MessageResponse
    {
        /// <summary>
        /// Retrieved status Code
        /// </summary>
        public long statusCode;
        /// <summary>
        /// Retrieved message
        /// </summary>
        public string message;
    }
}