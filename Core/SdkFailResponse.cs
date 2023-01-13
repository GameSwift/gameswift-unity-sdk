using System;

namespace GameSwiftSDK.Core
{
    /// <summary>
    /// Fail response with its reason described in a plain string.
    /// </summary>
    [Serializable]
    public class SdkFailResponse : BaseSdkFailResponse
    {
        /// <summary>
        /// Get failure message
        /// </summary>
        public override string Message => message;
        
        /// <summary>
        /// Get failure message
        /// </summary>
        public string message;

        public SdkFailResponse (string errorMessage)
        {
            message = errorMessage;
        }
    }
}