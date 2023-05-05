<p>
  <a href="https://github.com/ghostkiller967/DiscordRP/releases/latest"><img alt="GitHub release (latest by date)" src="https://img.shields.io/github/v/tag/ghostkiller967/DiscordRP?color=19e2e2&label=latest&logo=github"></a>
  <a href="https://github.com/ghostkiller967/DiscordRP/releases/latest"><img alt="GitHub Releases" src="https://img.shields.io/github/downloads/ghostkiller967/DiscordRP/latest/total?color=19e2e2&label=downloads&logo=github"></a>
  <a href="https://github.com/ghostkiller967/DiscordRP/releases"><img alt="All GitHub Releases" src="https://img.shields.io/github/downloads/ghostkiller967/DiscordRP/total?color=19e2e2&label=total%20downloads&logo=github"></a>
</p>

# DiscordRP
![Preview](https://i.imgur.com/DENM02V.png)

This application allows you to customize your rich presence on Discord, this is based on [CustomRP](https://github.com/maximmax42/Discord-CustomRP) by [maximmax42](https://github.com/maximmax42).

# Installation

To install DiscordRP you first need to download the [latest release](https://github.com/ghostkiller967/DiscordRP/releases/latest), you can install it manually or use the installer. 

If you choose the manual option, you create the following file structure:
```
DiscordRP
└─Program
  ├─Resources
  │ ├─Discord.ico
  │ └─loading.mp4
  ├─DiscordRP.exe
  ├─DiscordRPC.dll
  ├─Newtonsoft.Json.dll
  └─Octokit.dll
```

# Setup

To get the client id you need to go to the [Developer Portal](https://discord.com/developers/applications) and create a new application, the name of the application will be the title which will show in the rich presence. Copy the `Application ID` and put it in the `Client ID` Box of DiscordRP.

Make sure to enable **Activity Privacy > Display current activity as a status message.** inside the user settings within Discord.

To add images you need to go to the Rich Presence tab in the Developer Portal, add an image, wait a couple minutes and it will show up in the dropdown box inside DiscordRP

# Credits
[DiscordRPC Library](https://github.com/Lachee/discord-rpc-csharp) by [Lachee](https://github.com/Lachee) <br />
[Tiny json library](https://github.com/zanders3/json) by [zanders3](https://github.com/zanders3)
