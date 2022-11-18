using System.Diagnostics;
using WAUZ.BL;

Console.WriteLine("Hello, World!");
//var dir = @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns";
//if (Directory.Exists(dir))
//{
//    Console.WriteLine("passt");
//}
//dir = Path.GetFullPath(dir);
//var folders = Directory.GetDirectories(dir);
//var counter = 0;
//foreach (var folder in folders)
//{
//    var fullPath = Path.GetFullPath(folder);
//    Console.WriteLine(fullPath);
//    counter++;
//}
//Console.WriteLine(counter);



var testDir = @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns\WeakAurasTemplates";

if (Directory.Exists(testDir))
{
    Console.WriteLine("Dir exists.");
}

var sourceFolderEntries = Directory.EnumerateFileSystemEntries(testDir).Select(entry => Path.GetFileName(entry) ?? string.Empty);

sourceFolderEntries.ToList().ForEach(e => Console.WriteLine(Path.GetFileName(e) ?? "wuuuuz"));

//Console.WriteLine("Files--------------------");
//ListFiles(testDir);

//Console.WriteLine("Folders--------------------");
//ListFolders(testDir);

//static void ListFiles(string folder)
//{
//    var entries = Directory.GetFileSystemEntries(folder);
//    //var files = Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly);

//    foreach (var entry in entries)
//    {
//        if (entry != string.Empty)
//        {
//            Console.WriteLine(entry);
//        }
//    }
//}

//static void ListFolders(string folder)
//{
//    var folders = Directory.GetDirectories(folder);

//    foreach (var f in folders)
//    {
//        if (f != string.Empty)
//        {
//            Console.WriteLine(f);
//        }
//    }
//}

//Console.WriteLine("Folders:");
//var folderNames = Directory.GetDirectories(testDir).Select(f => Path.GetFileName(f));
//foreach (var f in folderNames) Console.WriteLine(f);
//Console.WriteLine("Files:");
//var fileNames = Directory.GetFiles(testDir).Select(f => Path.GetFileName(f));
//foreach (var f in fileNames) Console.WriteLine(f);

var businessLogic = new BusinessLogic(new SettingsHelper(), new ZipHelper(new FileSystemHelper(new PathHelper())));
businessLogic.LoadSettings();
Console.WriteLine($"SourceFolder: {businessLogic.SourceFolder}");
Console.WriteLine($"DestFolder: {businessLogic.DestFolder}");
businessLogic.SaveSettings();