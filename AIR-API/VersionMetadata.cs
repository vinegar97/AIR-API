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
            RawJSON = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
            VersionString = RawJSON.Version;
            Version = new Version(VersionString);
            Game = RawJSON.Game;
            Author = RawJSON.Author;
        }
    }
}
