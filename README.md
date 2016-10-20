ShangZhu
=
[![Build status](https://ci.appveyor.com/api/projects/status/qbhgym8bdomfjdmo?svg=true)](https://ci.appveyor.com/project/ocinbat/shangzhu)

.NET client for [Configius](http://www.configius.com/).

Warning!
-
This project is not ready for production use yet.

Getting Started
-

First initialize an ```IConfigius``` instance to talk to the api.
```csharp
IConfigius configius = ShangZhu.Connect("appId", "appSecret");
```

Then you can fetch your settings via ```Get(string key)``` method.
```csharp
string configValue = configius.Get("test-config-key");
```
