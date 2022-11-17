Console.WriteLine("Hello, World!");
var dir = @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns";
if (Directory.Exists(dir))
{
    Console.WriteLine("passt");
}
dir = Path.GetFullPath(dir);
var folders = Directory.GetDirectories(dir);
var counter = 0;
foreach (var folder in folders)
{
    var fullPath = Path.GetFullPath(folder);
    Console.WriteLine(fullPath);
    counter++;
}
Console.WriteLine(counter);