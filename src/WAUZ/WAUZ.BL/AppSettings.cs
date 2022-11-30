using System.Xml;
using System.Xml.Linq;

namespace WAUZ.BL
{
    public sealed class AppSettings : IAppSettings
    {
        private readonly string xmlFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AppDataMBODM", "WAUZ.xml");

        public IDictionary<string, string> Settings { get; } = new Dictionary<string, string>();

        public void Load()
        {
            if (!File.Exists(xmlFile))
            {
                return;
            }

            var document = XDocument.Load(xmlFile);

            var elements = document?.Element("wauz")?.Elements() ?? throw new InvalidOperationException("Invalid file format.");

            foreach (var element in elements)
            {
                Settings.Add(element.Name.ToString(), element.Value.ToString());
            }
        }

        public void Save()
        {
            // In .NET the Path.GetDirectoryName() method is used to get that directory, which
            // contains the file or folder, a given path is pointing to. And keep in mind: The
            // method does not care if the file or folder (a given path is pointing to) really
            // exists and relies solely on the path string itself. Additional note: The method
            // can throw, in older .NET versions or if path is too long, but this is fine here.

            var folder = Path.GetDirectoryName(xmlFile);

            if (string.IsNullOrEmpty(folder))
            {
                throw new InvalidOperationException("Could not determine folder.");
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var document = new XDocument(new XElement("wauz", Settings.Select(kvp => new XElement(kvp.Key, kvp.Value))));

            using var writer = XmlWriter.Create(xmlFile, new()
            {
                Indent = true,
                IndentChars = "\t",
            });

            document.Save(writer);
        }
    }
}
