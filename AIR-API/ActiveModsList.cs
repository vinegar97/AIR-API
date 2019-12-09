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
    public class ActiveModsList
    {
        public AIRActiveMods ActiveClass { get; set; }
        public List<string> ActiveMods { get => ActiveClass.ActiveMods; set => ActiveClass.ActiveMods = value; }
        public bool UseLegacyLoading { get => ActiveClass.UseLegacyLoading; set => ActiveClass.UseLegacyLoading = value; }

        public string ConfigPath = "";
        public ActiveModsList(FileInfo config)
        {
            ConfigPath = config.FullName;
            Load();
        }

        public ActiveModsList(AIRActiveMods data, string path)
        {
            ActiveClass = data;
            ConfigPath = path;
        }

        public void Load()
        {
            try
            {
                string data = File.ReadAllText(ConfigPath);
                ActiveClass = Newtonsoft.Json.JsonConvert.DeserializeObject<AIRActiveMods>(data);
            }
            catch (Exception ex)
            {
                CreateFile(ConfigPath);
                Load();
            }

        }

        public void CreateFile(string filePath)
        {
            var myFile = File.Create(filePath);
            myFile.Close();
            string nL = Environment.NewLine;
            string bracketOpen = "{";
            string bracketClose = "}";
            string standardFile = $"{bracketOpen}{nL}\t\"ActiveMods\": [{nL}{nL}\t]{nL}{bracketClose}";
            using (StreamWriter writetext = new StreamWriter(filePath)) writetext.WriteLine(standardFile);
        }

        public ActiveModsList(string filePath)
        {
            CreateFile(filePath);
            ConfigPath = filePath;
            Load();
        }

        public void Save(AIRActiveMods CurrentActiveMods)
        {
            ActiveClass = CurrentActiveMods;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentActiveMods, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(ConfigPath, output);

        }

        public void Save(List<string> CurrentActiveMods = null)
        {
            if (CurrentActiveMods != null)
            {
                if (!UseLegacyLoading) ActiveMods = CurrentActiveMods;
            }

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(ActiveClass, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(ConfigPath, output);

        }


    }


    public class AIRActiveMods
    {
        public List<string> ActiveMods { get; set; } = new List<string>();
        public bool UseLegacyLoading { get; set; } = true;
    }
}
