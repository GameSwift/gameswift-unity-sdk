using System;

namespace GameSwiftSDK.Core
{
    /// <summary>
    /// Base class for fail responses. As for now 2 fail responses exist - normal and array. 
    /// </summary>
    [Serializable]
    public abstract class BaseSdkFailResponse
    {
        /// <summary>
        /// Get failure message
        /// </summary>
        public abstract string Message { get; }
        
        /// <summary>
        /// Get number code failure
        /// </summary>
        public long statusCode;
    }
}
