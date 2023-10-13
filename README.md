# DiscordRP

<p>
  <a href="https://github.com/ghostkiller967/DiscordRP/releases/latest"><img alt="GitHub release (latest by date)" src="https://img.shields.io/github/v/tag/ghostkiller967/DiscordRP?color=5865f2&label=latest&logo=github"></a>
  <a href="https://github.com/ghostkiller967/DiscordRP/releases/latest"><img alt="GitHub Releases" src="https://img.shields.io/github/downloads/ghostkiller967/DiscordRP/latest/total?color=5865f2&label=downloads&logo=github"></a>
  <a href="https://github.com/ghostkiller967/DiscordRP/releases"><img alt="All GitHub Releases" src="https://img.shields.io/github/downloads/ghostkiller967/DiscordRP/total?color=5865f2&label=total%20downloads&logo=github"></a>
</p>

<p align="center">
  <img src="https://i.imgur.com/DENM02V.png" width="700">
  <p align="center">
This application allows you to customize your rich presence on Discord, this is based on
<a href="https://github.com/maximmax42/Discord-CustomRP">CustomRP</a>
  </p>
</p>

<h1 align="center">Installation</h1>

## Manual

### Advantages
* Version specific downloads.
* Doesn't automatically add a shortcut to the Start Menu in case you don't want it.
* Doesn't require Administrator privledges.

### Installation
* Download the zip file from the [latest release](https://github.com/sten-code/DiscordRP/releases/latest).
* Extract the zip content into a folder (the default in the installer is `C:\Program Files (x86)\DiscordRP`).
* Run `DiscordRP.exe`

## Installer

### Advantages
* Quicker and easier to use.
* Makes sure you download the latest version.
* Automatically adds a shortcut to the Start Menu.

### Installation
* Download `DiscordRPInstaller.exe` from the [latest release](https://github.com/sten-code/DiscordRP/releases/latest).
* Run it and select a folder where you want it to be downloaded in, the installer automatically downloads 
the latest version from the GitHub. An older installer will still download the latest version.
* It should automatically launch DiscordRP, but if it doesn't use the shortcut in the Start Menu.

<h1 align="center">Setup</h1>

## Client ID
* Go to the [Developer Portal](https://discord.com/developers/applications) and create a new application, the name of the application will be the title that shows up on your profile.
* Copy the Application ID and put it in the Client ID Box of DiscordRP.

## Adding Images
* Go to the `Rich Presence` tab in the [Developer Portal](https://discord.com/developers/applications).
* Add an image that is atleast 512x512, Discord doesn't allow lower resolution images.
* DiscordRP updates the image list every minute, Discord usually takes around a minute or 2 to process the it. After a couple minutes it *should* show up inside `Key` box in DiscordRP. If it doesn't, exit DiscordRP from the System Tray and restart it.

# Credits
[DiscordRPC Library](https://github.com/Lachee/discord-rpc-csharp) by [Lachee](https://github.com/Lachee) 
</br>
[Tiny Json](https://github.com/zanders3/json) by [zanders3](https://github.com/zanders3)
