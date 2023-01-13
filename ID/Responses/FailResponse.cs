using GameSwiftSDK.Core;

namespace GameSwiftSDK.Id.Responses
{
    /// <summary>
    /// Get failure request's response 
    /// </summary>
    public class FailResponse : BaseSdkFailResponse
    {
        /// <summary>
        /// The information result's request
        /// </summary>
        public override string Message { get; }
        
        /// <summary>
        /// Initialization message
        /// </summary>
        /// <param name="message">The information result's request</param>
        public FailResponse(string message)
        {
            Message = message;
        }
    }
}