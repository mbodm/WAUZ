# WAUZ
An unzip tool for World of Warcraft addons

![WAUZ](screenshot.png)

### What it is
It´s a very simple and tiny .NET 6 application named WAUZ (**W**orld of Warcraft **A**ddon **U**n**Z**ip). It´s used to unzip a bunch of downloaded zip files  inside a folder into another folder (in this case the zip files are addons for the popular MMORPG game *World of Warcraft*). It´s sole purpose is just to unzip the files into some folder, to make your life a little bit easier. Nothing else.

### How it works
- Download a bunch of World of Warcraft addons (typically from https://www.curseforge.com) into some download folder.
- Hint: Bookmark all the direct download sites, for every addon, in your browser. Especially for https://www.curseforge.com you can bookmark an addon´s download site directly. The addon download immediately starts, when clicking the bookmark. This helps a lot to get all the latest addon versions fast, without much effort. The process of manually unzipping them all, is way more time consuming. But there WAUZ will come to rescue. :)
- Start WAUZ
- Select the folder which contains the downloaded addon zip files (Source-Folder). Typically some temporary download folder.
- Select the folder to unzip the addons into (Destination-Folder). Typically the World of Warcraft AddOns folder.





- When started, `wingetupd.exe` searches for a so-called _package-file_. The package-file is simply a file named "packages.txt", located in the same folder as the `wingetupd.exe`. The package-file contains a list of WinGet package-id´s (__not__ package-names, this is important, see [Notes](#Notes) section below).
- So, when `wingetupd.exe` is started and it founds a package-file, it just checks for each WinGet package-id listed in the package-file, if that package exists, if that package is installed and if that package has an update. If so, it updates the package. `wingetupd.exe` does all of this, by using WinGet internally.
- This means: All you have to do, is to edit the package-file and insert the WinGet package-id´s of your installed Windows applications you want to update. When `wingetupd.exe` is executed, it will try to update all that packages (aka "your Windows applications").

### Parameters
`wingetupd.exe` knows the following parameters:
- `--no-log`
- `--no-confirm`
- `--help`

`--no-log` prevents `wingetupd.exe` from creating a log file. The log file contains all the internally used WinGet calls and their output, so you can exactly see how WinGet was used. Sometimes you just don´t want a log file (for whatever reason). Or maybe `wingetupd.exe` can´t create a log file, because it resides in a folder that has no write permissions (see [Notes](#Notes) section below). In such situations this parameter is to the rescue.

`--no-confirm` prevents `wingetupd.exe` from asking the user for confirmation, if it should update the updatable packages. So you can use this parameter to automatically update all updatable packages, without asking for confirmation. This parameter is useful when running `wingetupd.exe` in scripts.

`--help` shows the usage screen and lists the `wingetupd.exe` parameters.

### Requirements
There are not any special requirements, besides having WinGet installed on your machine. `wingetupd.exe` is just a typical command line ".exe" file for Windows. Just download the newest release, from the [Releases](https://github.com/MBODM/wingetupd/releases) page, unzip and run it. All the releases are compiled for x64, assuming you are using some 64-bit Windows (and that's quite likely).

### Notes
- When `wingetupd.exe` starts, it creates a log file named "wingetupd.log" in the same folder.
- So keep in mind: That folder needs security permissions for writing files in it.
- Some locations like "C:\\" or "C:\ProgramFiles" don´t have such security permissions (for a good reason).
- If you don´t wanna run `wingetupd.exe` just from Desktop, "C:\Users\USERNAME\AppData\Local" is fine too.
- You can also use the `--no-log` parameter, to prevent the creation of the log file (`wingetupd.exe --no-log`).
- All internally used WinGet calls are based on exact WinGet package-id´s (WinGet parameters: `--exact --id`).
- Use `winget search`, to find out the package-id´s (you put into the package-file) of your installed applications.
- Use the `--no-confirm` parameter, to automatically update packages, if `wingetupd.exe` is used inside a script.
- `wingetupd.exe` uses a timeout of 30 seconds, when waiting for WinGet to finish.
- Since some installations can take rather long, this timeout is increased to 60 minutes, while updates occur.
- The release binaries also contain a `wingetupd.bat` file, so you can run `wingetupd.exe` by a simple doubleclick.
- Q: _Why this tool and not just `winget --upgrade-all` ?_ A: Often you don´t wanna update all stuff (i.e. runtimes).
- Q: _Why this tool and not just some .bat or .ps script ?_ A: Maybe this is some better "out of the box" approach.
- At time of writing, the package-id _Zoom.Zoom_ seems to missmatch the corresponding installed _Zoom_ package.
- I assume the WinGet-Team will correct this wrong behaviour in their [packages repository](https://github.com/microsoft/winget-pkgs/tree/master/manifests) soon.
- WinGet doesn´t support being called in parallel. If you fork: Don´t use concurrency, like `Task.WhenAll()`.
- `wingetupd.exe` is written in C#, is using .NET 6 and is built with Visual Studio 2022.
- If you wanna compile the source by your own, you just need Visual Studio 2022 (any edition). Nothing else.
- The release-binaries are compiled as _self-contained_ .NET 6 .exe files, with "win-x64" as target.
- _Self-contained_: That´s the reason why the binariy-size is 15 MB and why there is no framework requirement.
- The _.csproj_ source file contains some MSBUILD task, to create a zip file, when publishing with Visual Studio 2022.
- GitHub´s default _.gitignore_ excludes VS publish-profiles, so i added a [publish-settings screenshot](img/screenshot-publish-settings.png) to repo.
- The code is using the TAP pattern of .NET, including concurrency concepts like `async/await` and `IProgress<>`.
- `wingetupd.exe` just exists, because i am lazy and made my life a bit easier, by writing this tool. :grin:

#### Have fun.



### How it works
- WinGetDotNet is released as .NET assembly (.dll). Just download and add it to your existing project.
- WinGetDotNet is released as .NET NuGet package (.nupkg). Just add it to your NuGet package source and use it.
- WinGetDotNet offers a method that asynchronously starts a _WinGet_ process as `Task`, to `await` the process.
- WinGetDotNet returns the _WinGet_ console output and exit code, after the awaitable process has finished.
- WinGetDotNet is using the `System.Diagnostics.Process` class to run _WinGet_.
- WinGetDotNet is using the TAP pattern, including typical `async/await` and `CancellationToken` concepts.
- WinGetDotNet is using a typical `CancellationToken` timeout pattern.
- WinGetDotNet is developed by using the [SOLID principles](https://en.wikipedia.org/wiki/SOLID) in a typical manner.

### Quick overview

Here are some excerpts, to give a quick overview on how it looks like, for a first impression:

- Property to verify if _WinGet_ is installed:
    ```csharp
    bool WinGetIsInstalled { get; }
    ```
- Method to run _WinGet_ asynchronous:
    ```csharp
    Task<WinGetResult> RunWinGetAsync(string parameters, CancellationToken cancellationToken = default)
    ```
- Method to run _WinGet_ asynchronous, with a given timeout:
    ```csharp
    Task<WinGetResult> RunWinGetAsync(string parameters, TimeSpan timeout, CancellationToken cancellationToken = default)
    ```
- Result model, containing data about how _WinGet_ was called, as well as it´s output and it´s exit code:
    ```csharp
    record WinGetResult(string ProcessCall, string ConsoleOutput, int ExitCode);
    ```
For more information, just use the IntelliSense tooltips of Visual Studio, or take a look into the source code. It´s really a rather small and simple library and not much more than what you see above. 😉

### Requirements
There aren´t any special requirements, besides having _WinGet_ installed on your machine. WinGetDotNet is just a typical .NET assembly, released as assembly DLL and NuGet package. Just download the newest release, from the [Releases](https://github.com/MBODM/WinGetDotNet/releases) page, unzip and add it to your project. All the releases are compiled for x64 Windows, assuming you are using some 64-bit Windows (and that's quite likely).

### Notes
- WinGetDotNet is under MIT license. Feel free to use the source and do whatever you want. I assume no liability.
- WinGetDotNet is written in C#, is using .NET 6 and is built with Visual Studio 2022.
- To compile the source by your own, you just need some Visual Studio 2022 edition. Nothing else.
- I never compiled it with other tools, like Rider or VS Code. I solely used Visual Studio 2022 Community.
- The release-binaries are compiled as _self-contained_ .NET 6 assemblies, with "_x64 Windows_" as target.
- _Self-contained_: That´s the reason why the binary-size may be bigger and why there is no framework dependency.
- GitHub´s default _.gitignore_ excludes Visual Studio 2022 publish-profiles, so i added a [screenshot](img/screenshot-publish-settings.png) to repo.
- WinGetDotNet offers a `Task` based approach, but the _WinGet_ app itself can´t run concurrently, without errors.
- So keep above fact in mind, before writing real concurrent code, by using i.e. `Task.WhenAll()` etc.
- WinGetDotNet just exists, because it started as a sidecar project of my  [wingetupd](https://github.com/MBODM/wingetupd) tool. :grin:

#### Have fun.

