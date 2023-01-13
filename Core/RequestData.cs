using System.Collections.Generic;
using GameSwiftSDK.Core;

/// <summary>
/// Helper for setting up request data.
/// </summary>
public class RequestData
{
    /// <summary>
    /// The link to send request
    /// </summary>
    public readonly string uri;
    /// <summary>
    /// The sent body to request
    /// </summary>
    public readonly string body;

    public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

    /// <summary>Constructor for response data</summary>
    /// <param name="uri">Url for request</param>
    /// <param name="body">The sent body to request</param>
    public RequestData(string uri, string body)
    {
        this.uri = uri;
        this.body = body;
    }

    /// <summary>Constructor for response data</summary>
    /// <param name="requestUri">Url for request</param>
    public RequestData (string requestUri)
    {
        uri = requestUri;
    }

    /// <summary>
    /// Add headers to dictionary
    /// </summary>
    /// <param name="customHeader">Type custom header</param>
    /// <param name="customHeaderText">Value custom header</param>
    public void SetupHeaders(CustomHeader customHeader, string customHeaderText)
    {
        Headers.Add("Content-Type", "application/json");
        Headers.Add("accept", "*/*");
        AddCustomHeader(customHeader, customHeaderText);
    }

    /// <summary>
    /// Add custom header to dictionary
    /// </summary>
    /// <param name="customHeader">Type custom header</param>
    /// <param name="customHeaderText">Value custom header</param>
    private void AddCustomHeader(CustomHeader customHeader, string customHeaderText)
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