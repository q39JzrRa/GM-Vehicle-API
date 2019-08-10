# GM-Vehicle-API
Remote API for supported General Motors vehicles in C#, .NET Standard
Includes a WPF demo app.

This is the API used by the myChevrolet, myBuick, myGMC, myCadillac, My Vauxhall and myOpel apps.

Reverse engineered C# API for accessing General Motors vehicles supporting OnStar connected services.
Obviously this is unsanctioned. Use at your own risk.

GM announced that they would be releasing the API in 2013. 6 years later they still refuse to respond to developer requests.
The API has been reverse engineered via decompilation and protocol interception in order to facilitate interoperability. No copywritten works have been duplicated.

You are accepting all responsibility and liability for the use of this content.

# Client Credentials
To use this API you will require a valid client id and client secret. The correct approach would be to request access from GM at https://developer.gm.com/ or by emailing them at developer.gm.com.

Alternatively (and because GM refuses to respond to developer requests) you can extract the credentials from the Android app's .apk file.
I am _NOT_ including the source code for this process, but I have included the capability. GM.SettingsReader.dll can do this. I have obfuscated the process.

IMPORTANT: The demo app requires a copy of the Android app's .apk file to be copied to the "apk" folder. It has been tested with the myChevrolet app, version 3.21.0.
VERY IMPORTANT: Unless you want an international incident on your hands DO NOT SHARE ANY OF THE CONTENTS OF THE SETTINGS FILE ANYWHERE _EVER_!!!!


# TODO
This is very early, unpolished, incomplete code. No judgement please.



TODO: implement lots more actions

TODO: determine how app elevates creds when using fingerprint - does the app save the pin?

TODO: there is a means of refreshing a token using a pin...

TODO: determine how long elevation lasts, keep track and re-elevate when required

TODO: consider using MS JWT implementation
