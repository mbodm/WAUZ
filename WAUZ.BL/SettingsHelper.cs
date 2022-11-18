using System.Xml;
using System.Xml.Linq;

namespace WAUZ.BL
{
    public sealed class SettingsHelper : ISettingsHelper
    {
        public string Location { get; } = string.Empty;
        
        public IDictionary<string, string> Settings { get; } = new Dictionary<string, string>();

        public SettingsHelper()
        {
            Location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WAUZ.xml");
        }

        public void Load()
        {
            if (!File.Exists(Location))
            {
                return;
            }

            var document = XDocument.Load(Location);

            document?.Element("wauz")?.Elements()?.ToList().ForEach(element => Settings.Add(element.Name.ToString(), element.Value.ToString()));
        }

        public void Save()
        {
            var document = new XDocument(
                new XElement("wauz",
                    Settings.Select(kvp => new XElement(kvp.Key, kvp.Value))));

            EnsureClosingTags(document);

            var folder = Path.GetDirectoryName(Location) ?? string.Empty;

            if (folder == string.Empty)
            {
                throw new InvalidOperationException("Todo - GetDirectoryName() returned empty string.");
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using var writer = XmlWriter.Create(Location, new()
            {
                Indent = true,
                IndentChars = "\t",
            });

            document.Save(writer);
        }

        private static void EnsureClosingTags(XDocument document)
        {
            // The written XML should contain closing tags, also for empty elements.
            // To achieve this, it seems to be best practice in LINQ-to-XML, to put
            // an empty string into an empty element. This enforces the closing tag.
            // So we have to make sure every empty element contains an empty string.
            
            foreach (var element in document.Descendants())
            {
                if (element.IsEmpty)
                {
                    element.Value = string.Empty;
                }
            }
        }
    }
}
