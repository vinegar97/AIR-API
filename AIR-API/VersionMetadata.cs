using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AIR_API
{
    public class VersionMetadata
    {
        public string FilePath = "";
        private dynamic RawJSON;

        public string VersionString;
        public Version Version;
        public string Game = "";
        public string Author = "";
        public VersionMetadata(FileInfo config)
        {
            FilePath = config.FullName;
            string data = File.ReadAllText(FilePath);
            dynamic RawJSON = JsonConvert.DeserializeObject(data);
            JObject RawJSONObject = JObject.Parse(data);

            if (RawJSONObject.Property("Version") != null) VersionString = (string)RawJSONObject.Property("Version").Value;
            if (RawJSONObject.Property("Game") != null) Game = (string)RawJSONObject.Property("Game").Value;
            if (RawJSONObject.Property("Author") != null) Author = (string)RawJSONObject.Property("Author").Value;

            Version.TryParse(VersionString, out Version);
        }
    }
}
