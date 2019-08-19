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

I have a utility that is capable of extracting the client credentials from the Android APK but I am _very_ hesitant to share. The client secrets get out and GM will freak out. So...

I have implemented a very small, very simple web service hosted with heroku (https://gmsigner.herokuapp.com/) that will sign token requests for you,
and I have implemented a version of the client that uses this service.
(Please note that you will be sending your login credentials to this service. I would highly advise reviewing the service source code here: https://github.com/q39JzrRa/GM-Vehicle-API-AuthUtil . It is deployed to Heroku using CD from the master branch so the code you see is the code that runs)

Quick note: I have purged the extraction code from the repo. Sorry about that :-P


# Quick Start
If you prefer not to use the Windows UI or examine how it works, here is how you might start your car if you only have one.

```
            // generate Device ID (GUID formatted as a string)

            var client = new GMClientNoKey("{deviceId}", Brand.Chevrolet, "https://gmsigner.herokuapp.com/");
            if (!await _client.Login("{ your username}", "{your password}")) { throw new InvalidOperationException("Login Failed"); }
            var vehicles = await _client.GetVehicles();
            if (vehicles == null || !vehicles.Any()) { throw new InvalidOperationException("No Vehicles on acccount"); }
            client.ActiveVehicle = vehicles.FirstOrDefault();
            if (!await client.UpgradeLogin("{Your OnStar PIN}")) { throw new InvalidOperationException("Login upgrade failed"); }
            if (!await client.Start()) { throw new InvalidOperationException("Start failed"); }
            Console.WriteLine("Start Success");
```


# Implemented Functionality
* Login
* Elevate credentials
* Enumerate Vehicles and vehicle capabilities
* Remote Lock and Unlock
* Remote Start and Stop
* Remote Alarm
* Get Diagnostics (including charge status and charge level)
* Get vehicle location
* Send turn-by-turn route


# TODO
This is very early, unpolished, incomplete code. No judgement please.

* Implement more commands
* consider using MS JWT implementation
* Implement secure means of saving onstar pin. If possible.
* recognize response from calling priv'd command without upgrade and trigger upgrade using saved pin.

Notes: The android app saves the onstar pin using biometrics to unlock - no difference in the api calls. It does not use a different token refresh mechanism after elevating permissions, but the elevation persists across a refresh. The upgrade request does not specify an expiration. Testing will be required to determine the lifespan of token upgrades.

