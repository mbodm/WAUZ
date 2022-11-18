using System.Xml;
using System.Xml.Linq;

namespace WAUZ.BL
{
    public sealed class SettingsHelper : ISettingsHelper
    {
        private readonly string xmlFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AppDataMBODM", "WAUZ.xml");

        private readonly IPathHelper pathHelper;

        public SettingsHelper(IPathHelper pathHelper)
        {
            this.pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));
        }

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
            var folder = pathHelper.GetParentDirectoryFromPath(xmlFile);

            if (folder == string.Empty)
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
