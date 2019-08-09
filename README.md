# GM-Vehicle-API
Remote API for supported General Motors vehicles

Reverse engineered C# API for accessing General Motors vehicles supporting OnStar connected services.
Obviously this is unsanctioned. Use at your own risk.

GM announced that they would be releasing the API in 2013. 6 years later they still refuse to respond to developer requests.
The API has been reverse engineered via decompilation and protocol interception in order to facilitate interoperability. No copywritten works have been duplicated.

To use this API you will require a valid client id and client secret. You may email developer@gm.com to request credentials but they have yet to provide any.
Alternatively you can extract and decrypt them from the Android .apk file.


This is very early, unpolished, incomplete code. No judgement please.



TODO: add vehicle selection
TODO: use diagnostic list from config, rather than hard-coded
TODO: implement lots more actions
TODO: determine how app elevates creds when using fingerprint - does the app save the pin?
TODO: there is a means of refreshing a token using a pin...
TODO: determine how long elevation lasts, keep track and re-elevate when required