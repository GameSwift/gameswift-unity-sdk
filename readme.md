# What is GameSwift Unity SDK?
GameSwift SDK is a Unity toolset created to integrate with GameSwift ID ecosystem. Our SDK is multiplatform - you can build your game for PC, mobile and even browser!
_Aside from GameSwift SDK, we are preparing GameSwift analytics_.

# Getting started
In order to integrate your Unity game with GameSwift gaming ecosystem, [import](https://docs.unity3d.com/Manual/upm-ui-giturl.html) our package to your project by providing git url.
```
https://github.com/GameSwift/gameswift-unity-sdk.git
```

You can handle GameSwift login in 2 ways: [with launcher](#logging-in-from-launcher) or [without launcher](#logging-in-without-launcher). You can download GameSwift launcher [here](https://launcher.gameswift.io/).
As long as your game targets Windows or MacOS, we strongly recommend to use data passed from our launcher. By doing so, you won't need to implement any login screen for your game as launcher already handles user credentials in a secure way.
If you are building for mobile or web, you will need to create a login screen and implement connection with GameSwift backend manually.

### Logging in from launcher
1. Create a login class, which will be launched on application startup.
2. Call a method `GameSwiftSdkId.ReadUserInfoFromLauncher` in order to retrieve `AccessToken` from launcher's command-line arguments and store it in the SDK's `GameSwiftSdkId.Instance.CmdAccessToken` field used later in the authorization step.
3. Create success and fail handlers for this method.
4. In the success handler call `GameSwiftSdkId.Authorize` method, which will perform all of the next steps for you automatically. Remember to use stored `GameSwiftSdkId.Instance.CmdAccessToken` here. Also, provide your `clientId` and `redirectUri`. If the process is finished successfully you will be authorized and a new `AccessToken` will be stored in the SDK's `GameSwiftSdkId.Instance.AccessToken` field. From now on you should be using this token in each request.

```cs
using GameSwiftSDK.Core;
using GameSwiftSDK.Id;
using GameSwiftSDK.Id.Responses;
using UnityEngine;

public class GameSwiftLauncherLogin : MonoBehaviour
{
	private const string CLIENT_ID = "yourClientId";
	private const string REDIRECT_URI = "yourRedirectUri";

	private void Awake()
	{
		if (Application.isEditor == false)
		{
			GameSwiftSdkId.ReadUserInfoFromLauncher(HandleSuccess, HandleFailure);
		}
	}

	private void HandleSuccess(OauthUserInfoResponse response) 
	{
		GameSwiftSdkId.Authorize(GameSwiftSdkId.Instance.CmdAccessToken,
			CLIENT_ID, REDIRECT_URI, HandleAuthorizeSuccess, HandleFailure);
	}

	private void HandleFailure(BaseSdkFailResponse response) 
	{
		Debug.LogError($"Login error, code: {response.statusCode}, message: {response.Message}");
		Application.Quit();
	}

	private void HandleAuthorizeSuccess(OauthUserInfoResponse response) 
	{
		// your code on success login
	}
}
```

Though we highly recommend using the above method, if you want to implement some specific for you project login use cases, you can execute authorization methods separately. Keep in mind though, that by doing this you will need to store `AccessToken` in your project by yourself as the last step.
In order to authorize this way, instead of instructions described in the point 4, in the `GameSwiftSdkId.ReadUserInfoFromLauncher` success handler call these methods in sequence, one by one in their respective success handlers:

- `GameSwiftSdkId.GetAuthorizationCode` - use stored `GameSwiftSdkId.Instance.CmdAccessToken` here and provide your `clientId` and `redirectUri`.
- `GameSwiftSdkId.RetrieveOauthToken` - provide `authorizationCode` retrieved from the previous method's response (`AuthorizeResponse`) and provide your `clientId` and `redirectUri` again. This will generate an `AccessToken` returned in the request's response (`TokenResponse`).
- `GameSwiftSdkId.GetOauthUserInformation` - use your newly generated `AccessToken` here to get your user's information and on success store it somewhere in your project. From now on you should be using this token in each request.

### Logging in without launcher
1. Create a login class, which will be attached to a login screen.
2. On a login event call a method `GameSwiftSdkId.LoginAndAuthorize` where you need to pass user's `emailOrNickname`, `password`, your `clientId` and `redirectUri` values.
3. Create success and fail handlers for this method.
4. If the process is finished successfully you will be logged in, authorized and a new `AccessToken` will be stored in the SDK's `GameSwiftSdkId.Instance.AccessToken` field. From now on you should be using this token in each request.

```cs
using GameSwiftSDK.Core;
using GameSwiftSDK.Id;
using GameSwiftSDK.Id.Responses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSwiftManualLogin : MonoBehaviour
{
	private const string CLIENT_ID = "yourClientId";
	private const string REDIRECT_URI = "yourRedirectUri";

	[SerializeField] private TMP_InputField emailOrNickname;
	[SerializeField] private TMP_InputField password;
	[SerializeField] private Button loginButton;

	private void OnEnable ()
	{
		loginButton.onClick.AddListener(LoginUser);
	}

	private void LoginUser ()
	{
		GameSwiftSdkId.LoginAndAuthorize(emailOrNickname.text, password.text,
			CLIENT_ID, REDIRECT_URI, HandleSuccess, HandleFailure);
	}

	private void HandleSuccess (OauthUserInfoResponse response)
	{
		// your code on success login
	}

	private void HandleFailure (BaseSdkFailResponse response)
	{
		Debug.LogError($"Login error, code: {response.statusCode}, message: {response.Message}");
	}

	private void OnDisable ()
	{
		loginButton.onClick.RemoveListener(LoginUser);
	}
}
```

Though we highly recommend using the above method, if you want to implement some specific for you project login use cases, you can execute separate login and authorization methods. In order to achieve this, instead of calling a `GameSwiftSdkId.LoginAndAuthorize` method call these methods in sequence, one by one in their respective success handlers:
- `GameSwiftSdkId.Login` - provide user's `emailOrNickname` and `password`.
- `GameSwiftSdkId.GetAuthorizationCode` - use the `AccessToken` retrieved from `GameSwiftSdkId.Login` method's response (`LoginResponse`) here. Also, provide your `clientId` and `redirectUri`.
- `GameSwiftSdkId.RetrieveOauthToken` - provide `authorizationCode` retrieved from the previous method's response (`AuthorizeResponse`) and provide your `clientId` and `redirectUri` again. This will generate an `AccessToken` returned in the request's response (`TokenResponse`).
- `GameSwiftSdkId.GetOauthUserInformation` - use your newly generated `AccessToken` here to get your user's information and on success store it somewhere in your project. From now on you should be using this token in each request.

### Multiple Logins Blocker
You need to have your client set up to block multiple login attempts for this component to work.
To configure `Multiple Logins Blocker` you need to edit `MultipleLoginsBlockerData.asset` scriptable object which should be automatically created on Unity asset refresh in the `Assets/Resources/GameSwiftSDK/` directory.
When SDK instance in created this component will start working automatically in the background (if is turned on in the config file). Every specified number of seconds it will be sending hearbeats do the server to keep the lock.
If you don't use our recommended login approaches, remember to call `GameSwiftSdkId.GetOauthUserInformation` method as the last step of login process. This will be your first sent heartbeat and will initialize the process.

# Hello Hero
Hello Hero is our sample application that shows how your game can be properly integrated with our SDK. In the aplication we can test requests to the `GameSwift ID` and we can see some basic results in the panel `Output`. Feel free to experiment with it!

# Contact Us
**piotr.sobiecki@gameswift.io**
