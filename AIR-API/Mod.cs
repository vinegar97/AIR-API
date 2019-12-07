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
    public class Mod
    {
        public class AIRMod
        {
            public AIRMod()
            {
                Metadata = new Mod_Metadata();
            }

            public Mod_Metadata Metadata { get; set; }
            public class Mod_Metadata
            {
                public string Name { get; set; }
                public string Author { get; set; }
                public string Description { get; set; }
                public string URL { get; set; }
                public string ModVersion { get; set; }
                public string GameVersion { get; set; }
            }
        }

        private AIRMod ModClass;


        public string FileLocation { get; set; }
        public string Author { get => ModClass.Metadata.Author; set => ModClass.Metadata.Author = value; }
        public string Name { get => ModClass.Metadata.Name; set => ModClass.Metadata.Name = value; }
        public string TechnicalName { get; set; }
        public string Description { get => ModClass.Metadata.Description; set => ModClass.Metadata.Description = value; }
        public string FolderName;
        public string FolderPath;
        public string URL { get => ModClass.Metadata.URL; set => ModClass.Metadata.URL = value; }
        public string ModVersion { get => ModClass.Metadata.ModVersion; set => ModClass.Metadata.ModVersion = value; }
        public string GameVersion { get => ModClass.Metadata.GameVersion; set => ModClass.Metadata.GameVersion = value; }
        public bool EnabledLocal { get; set; }
        public bool IsEnabled { get; set; }
        public override string ToString() { return Name; }


        public Mod()
        {
            ModClass = new AIRMod();
            Author = "N/A";
            Name = "N/A";
            URL = "NULL";
            Description = "No Description Provided.";
            GameVersion = "N/A";
            ModVersion = "N/A";

            FileLocation = "N/A";
            FolderName = "N/A";
            FolderPath = "N/A";
            TechnicalName = $"[{FolderName.Replace("#", "")}]";

        }

        public void Save()
        {
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(ModClass, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(FileLocation, output);
        }

        public Mod(FileInfo mod)
        {
            string data = File.ReadAllText(mod.FullName);
            dynamic stuff = JRaw.Parse(data);
            ModClass = new AIRMod();
            //Author
            Author = stuff.Metadata.Author;
            if (Author == null) Author = "N/A";
            //Name
            Name = stuff.Metadata.Name;
            if (Name == null) Name = "N/A";
            //Description
            Description = stuff.Metadata.Description;
            if (Description == null) Description = "No Description Provided.";
            //Mod URL
            URL = stuff.Metadata.URL;
            if (URL == null) URL = "NULL";
            //ModVersion
            ModVersion = stuff.Metadata.ModVersion;
            if (ModVersion == null) ModVersion = "N/A";
            //GameVersion
            GameVersion = stuff.Metadata.GameVersion;
            if (GameVersion == null) GameVersion = "N/A";

            FileLocation = mod.FullName;
            FolderName = mod.Directory.Name;
            FolderPath = mod.Directory.FullName;
            TechnicalName = $"[{FolderName.Replace("#", "")}]";

        }
    }
}
