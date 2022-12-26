# WAUZ
A tiny unzip tool for World of Warcraft addons

![WAUZ](screenshot.png)

### What it is
It´s a very simple and tiny .NET 6 application named WAUZ (**W**orld of Warcraft **A**ddons **U**n**Z**ip).

It´s used to unzip a bunch of downloaded zip files, residing in some folder, into another folder (in this case the zip files are addons for the popular [World of Warcraft](https://worldofwarcraft.com) MMORPG game).

It´s just a typical ".exe" file Windows application. Just download the newest release, unzip and run it. That´s it. No installer, setup or something like that. It´s sole purpose is to extract zip files into a folder. Nothing else. It´s just there to make your life a little bit easier. 

### How it works
- Download a bunch of World of Warcraft addons (typically from https://www.curseforge.com) into some folder.
- Start WAUZ
- Select the folder which contains the addon zip files (Source-Folder). Typically some temporary download folder.
- Select the folder to unzip the addons into (Destination-Folder). Typically the World of Warcraft AddOns folder.
- Press the "Unzip" button.
- Hint: Bookmark all the direct download sites, for every addon, in your browser. Especially for https://www.curseforge.com you can bookmark an addon´s download site directly. The addon download immediately starts, after clicking the bookmark. This helps a lot to get all the latest addon versions very fast, without much effort, and you quickly have all of them in a single folder. The process of manually unzipping them is way more time consuming. But that´s the moment when WAUZ comes to rescue. :wink:

:warning:

__Important notes, when using the new release version 1.0.3 or higher:__

The way WAUZ works and doing its unzip job /under the hood) has completely changed in the 1.0.3 release version! Therefore i need to explain this a bit in detail here, since this may be important for the user, when it comes down to "data loss" topics.

The short version:

WAUZ now always clears the destination folder (removing all files and folders inside it), before the unzip process even starts. This means: If in example a zip file is corrupted and WAUZ stops, you already lost all of your old/existing addons (inside the destination folder). In most cases this should be no problem, because the destination folder is normally just a "working folder" for binary stuff, solely used for the addon "applications". This applies also and foremost to the "World of Warcraft AddOns" folder (when used as destination folder in WAUZ). Normally there should not reside any sensible data, config files, or something like that in this folder. Even the addons themselfes store their config files in another folder (a specific folder inside the World of Warcraft installation folder). But if you stored anything important in the destination folder, for whatever reason, make sure you create a backup before pressing the "Unzip" button of WAUZ! If you want to know more about the "why?" just continue reading.

The long (technical background) version:



There are a few ways, the unzip process can be handled:
- Approach 1: Remove all content inside the destination folder (the old, already existing addon files and folders) first, before even start unzipping the new addons into this destination folder.

:warning:

### Why it exists
I developed a download manager for World of Warcraft addons, called [WADM](https://github.com/mbodm/wadm), over a decade ago. For many many years WADM handled all of my addon updating needs with ease. But since Curse/Overwolf changed their political stance, alternative download managers (like mine, Ajour, WowUp, or others) no longer works with the https://www.curseforge.com site, or their REST web service. The only option is to use their own addon download manager. Many of us don´t want that, for different reasons.

So, downloading the addons manually (which is not the time consuming bottleneck here) and unzipping them, with the help of a tool like WAUZ, is still not the worst alternative to a full featured download manager. For more information about "the end of all alternative addon download managers" follow the links on the GitHub site of my above mentioned WADM project, or use your GoogleFu.

### Requirements

- 64-bit Windows

There are not any other special requirements. All the release-binaries are compiled with "win-x64" as target platform, assuming you are using some 64-bit Windows (and that's quite likely).

You can choose between _self-contained_ and _framework-dependent_ .NET application builds, when downloading a release. If you want to run the _framework-dependent_ variant you need (as additional requirement) an installed .NET 6 runtime. You can find more information about that topic on the [Releases](https://github.com/mbodm/wauz/releases) page.

### Notes
- WAUZ saves your selected folders automatically in a settings file when you close the app.
- WAUZ loads your selected folders automatically from that settings file when you open the app.
- WAUZ writes a log file if some error happens.
- You can find both files (settings and log) in the "C:\Users\YOURUSERNAME\AppData\Local\MBODM" folder.
- WAUZ uses a timeout of 30 seconds, while unzipping the files.
- WAUZ unzips the files in parallel.
- WAUZ will not touch any other files or folders in Destination-Folder, besides the ones coming from the zip files.
- WAUZ is written in C# and developed with .NET 6, in Visual Studio 2022.
- WAUZ is using Windows.Forms as UI framework (yes, because "rapid development").
- There will be also a command line version of WAUZ. Soon.
- I never compiled WAUZ with other tools, like Rider or VS Code. I solely used Visual Studio 2022 Community.
- If you want to compile by yourself, you can just use i.e. Visual Studio 2022 (any edition) . You need nothing else.
- The release-binaries are compiled with "win-x64" as target platform (_self-contained_ and _framework-dependent_).
- The code is using the TAP pattern of .NET, including concurrency concepts like `async/await` and `IProgress<>`.
- The code is using a typical `CancellationToken` timeout approach.
- WAUZ is under MIT license. Feel free to use the source and do whatever you want. I assume no liability.
- WAUZ just exists, because i am lazy and made my life a bit easier, by writing this tool. :grin:

#### Have fun.

---

> ##### Personal ideas
> - This is a list of ideas, used as my personal todo list. Better centralized here, than somewhere else.
> - Add WAUZ command line version (easy, BusinessLogic already exists).
> - :heavy_check_mark: Maybe add navigation links to UI, to quickly open the selected folders in Explorer?
> - Maybe add navigation links to UI, to quickly open the settings/log files or folder?
> - :heavy_check_mark: Maybe implement some temp folder cleanup check, before unzipping?
> - :heavy_check_mark: To cleanup any existing/old temp folders if app was closed too early in previous session.
> - Add more log infos (logfile size?) to better reproduce what happened? Not meant as "log everything".
> - But maybe add additional infos when logging an exception (like the selected folders, etc).
> - Maybe implement option (checkbox) to remove all existing content from Destination-Folder before unzip?
> - Do users really want/need that? (Maybe the above "open in Explorer" option is enough here)
> - :heavy_check_mark: Missing or wrong selected folders lead to "Error occurred" UI status. Keep this behaivour?
> - Maybe add a ListView with CheckBoxes to select zip files, before unzipping?
> - :heavy_check_mark: Change TextBox placeholder text.
