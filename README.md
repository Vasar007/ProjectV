# ThingAppraiser

[![License](https://img.shields.io/hexpm/l/plug.svg)](https://github.com/Vasar007/ThingAppraiser/blob/master/LICENSE)
[![AppVeyor branch](https://img.shields.io/appveyor/ci/Vasar007/ThingAppraiser/master.svg)](https://ci.appveyor.com/project/Vasar007/thingappraiser)

Evaluate your things (movies, games, books e.t.c.) automatically based on popular databases with ratings.

## Dependencies

Target .NET Standard is 2.0 for libraries, .NET Core is 2.2 for web services and .NET Framework is 4.7.2 for desktop app. Version of C# is 7.1.

You can install all dependencies using NuGet package manager.

## Installation guide

### .NET Framework

Download and install .NET Framework 4.7.2 ([official link](https://dotnet.microsoft.com/download/dotnet-framework/net472)).

### .NET Core

Download and install .NET Core 2.2 ([official link](https://dotnet.microsoft.com/download)). Feel free to install any minor version of .NET Core but we recommend to use .NET Core SDK latest version that is compatible with Visual Studio 2017 ([download here](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.107-windows-x64-installer)).

### Environment variables

Depends on what services you want to use, almost always you need to get API keys/tokens for them. All keys and tokens set as environment variable value with key "ThingAppraiser" (we are going to add other ways to configure service later). Further you can find some instructions how to get credentials for some of the services with which the ThingAppraiser can interact.

You should set environment variable for project on your machine. Variable name (key) is "ThingAppraiser", variable value is depend on services. Example of some services keys/tokens format (for Windows): "BotToken=YOUR_BOT_TOKEN;TmdbApiKey=YOUR_TMDB_API_KEY;OmdbApiKey=YOUR_OMDB_API_KEY;SteamApiKey=YOUR_STEAM_API_KEY".

#### Settings for Telegram bot service

If you want to use Telegram bot as one of the ThingAppraiser means of communication with the user, you should create bot and set token in environment variable values (remember that variable name (key) is "ThingAppraiser"). Official guide [how to create bot and get token](https://telegrambots.github.io/book/1/quickstart.html). Example: "BotToken=YOUR_BOT_TOKEN".

Next step is set Webhook because through it Telegram sends notifications and other messages to ThingAppraiser. Full instruction [here](https://github.com/TelegramBots/telegram.bot.examples/tree/master/Telegram.Bot.Examples.DotNetCoreWebHook).

In addition, Telegram is banned in some countries. So you should configure your VPN or Proxy to get around the lockdown (now we does npt support such option).

#### TMDB service

For movie and series processing ThingAppraiser can use TMDB service. Official introduction guide you can read [here](https://developers.themoviedb.org/3/getting-started/introduction). Also you should set your API key in environment variable values (remember that variable name (key) is "ThingAppraiser"). Example: "TmdbApiKey=YOUR_TMDB_API_KEY".

Please, do not forget that such services as TMDB (or OMDB) have own limits on API calls per day or month. More information you can read in official resources of services that you use in ThingAppraiser.

#### OMDB

Other way to work with movie and series for ThingAppraiser is OMDB. Official guide you can read [here](http://www.omdbapi.com/apikey.aspx). Set your API key in environment variable values (remember that variable name (key) is "ThingAppraiser"). Example: "OmdbApiKey=YOUR_OMDB_API_KEY".

#### Steam

It is important to mention that ThingAppraiser do not use Steam API key for API calls because Steam do not forbid such calls. But if you want to use all opportunities of Steam API, which is not officially documented by the way, you should get API key. Instructions you can find [here](https://steamcommunity.com/dev). Set your API key in environment variable values (remember that variable name (key) is "ThingAppraiser"). Example: "SteamApiKey=YOUR_STEAM_API_KEY".

### Launch

ThingAppraiser has three ways of communication with user:

- Desktop application;
- Console application;
- Telegram bot service.

Desktop app and Telegram bot service cannot appraise data and used for data visualization. They send requests to ThingAppraiser Web services and process responses. On the other hand, Console app uses ThingAppraiser Lib directly (at this moment). That is why you should lauch desktop app and Telegram bot service with Web services but Console can be started without other services.

## License information

This project is licensed under the terms of the [Apache License 2.0](LICENSE).
