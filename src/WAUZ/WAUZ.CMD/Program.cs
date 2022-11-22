// Get a list of invalid path characters.
char[] invalidPathChars = Path.GetInvalidPathChars();

Console.WriteLine("The following characters are invalid in a path:");
ShowChars(invalidPathChars);
Console.WriteLine();

// Get a list of invalid file characters.
char[] invalidFileChars = Path.GetInvalidFileNameChars();

Console.WriteLine("The following characters are invalid in a filename:");
ShowChars(invalidFileChars);

static void ShowChars(char[] charArray)
{
    Console.WriteLine("Char\tHex Value");
    // Display each invalid character to the console.
    foreach (char someChar in charArray)
    {
        if (Char.IsWhiteSpace(someChar))
        {
            Console.WriteLine(",\t{0:X4}", (int)someChar);
        }
        else
        {
            Console.WriteLine("{0:c},\t{1:X4}", someChar, (int)someChar);
        }
    }
}
