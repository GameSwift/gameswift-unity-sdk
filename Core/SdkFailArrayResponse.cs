using System;

namespace GameSwiftSDK.Core
{
    /// <summary>
    /// Fail response with its reason described in an array.
    /// </summary>
    [Serializable]
    public class SdkFailArrayResponse : BaseSdkFailResponse
    {
        /// <summary>
        /// Get failure message
        /// </summary>
        public override string Message => string.Join("\n", message);
        
        /// <summary>
        /// Get failure message
        /// </summary>
        public string[] message;
    }
}
