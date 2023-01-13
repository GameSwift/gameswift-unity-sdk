# Getting started
In order to integrate your Unity game with GameSwift gaming ecosystem, [import](https://docs.unity3d.com/Manual/upm-ui-giturl.html) our package to your project by providing git url.
```
https://github.com/GameSwift/gameswift-sdk.git
```

After succesful import, you can handle GameSwift login in 2 ways: with or without [GameSwift launcher](https://launcher.gameswift.io/). As long as your game targets Windows or MacOS, we strongly recommend to [use data passed from our launcher](#logging-in-from-launcher). By doing so, you don't need to implement any login screen for your game as launcher already handles user credentials in a secure way. If you are building for mobile or web, you will need to create a login screen and [implement connection](#logging-in-without-launcher) with GameSwift backend manually.

## Logging in from launcher
1. Create script that launches at the very beggining of your game.
2. Within this script, call `GameSwiftSdkId.GetUserInformationFromLauncher` if game is not run from the editor.
3. Create success and failure handlers. `HandleSuccess` will be called if we succesful log in from launcher. `HandleFailure` will be called otherwise.

```cs
using GameSwiftSDK.Core;
using GameSwiftSDK.Id;
using GameSwiftSDK.Id.Responses;
using UnityEngine;

public class GameSwiftLauncherLogin : MonoBehaviour
{
	[SerializeField]
	private RectTransform _waitScreen;

	private void Awake()
	{
		if (Application.isEditor == false)
		{
			GameSwiftSdkId.GetUserInformationFromLauncher(HandleSuccess, HandleFailure);
		}
	}

	private void HandleSuccess(OauthUserInfoResponse response) 
	{
		_waitScreen.gameObject.SetActive(false);
	}

	private void HandleFailure(BaseSdkFailResponse response) 
	{
		Debug.LogError($"Log in error, code: {response.statusCode}, message: {response.Message}");
		Application.Quit();
	}
}
```

## Logging in without launcher
1. Create script that launches at the very beggining of your game.
2. Within this script, call `GameSwiftSdkId.Login`, passing read user credentials.
3. Create success and failure handlers. `HandleSuccess` will be called if we succesful log in from launcher. `HandleFailure` will be called otherwise.

```cs
using GameSwiftSDK.Core;
using GameSwiftSDK.Id;
using GameSwiftSDK.Id.Responses;
using UnityEngine;
using UnityEngine.UI;

public class GameSwiftManualLogin : MonoBehaviour
{
	[SerializeField]
	private RectTransform _loginScreen;

	[SerializeField]
	private InputField _emailOrNickname;

	[SerializeField]
	private InputField _password;

	private void Awake()
	{
		GameSwiftSdkId.Login(_emailOrNickname.text, _password.text, HandleSuccess, HandleFailure);
	}

	private void HandleSuccess(LoginResponse response)
	{
		_loginScreen.gameObject.SetActive(false);
	}

	private void HandleFailure(BaseSdkFailResponse response)
	{
		Debug.LogError($"Log in error, code: {response.statusCode}, message: {response.Message}");
		Application.Quit();
	}
}
```

# Troubleshooting
In case of any problems, feel free to contact us at [peter@gameswift.io](mailto:peter@gameswift.io).
