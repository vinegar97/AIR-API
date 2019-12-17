using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Device = AIR_API.InputMappings.Device;

namespace AIR_API
{
    public class Settings
    {
        public enum FullscreenType : int
        {
            Windowed = 0,
            Fullscreen = 1,
            ExclusiveFS = 2
        }



        public string Sonic3KRomPath { get; set; }
        public bool FixGlitches { get; set; }
        public string AIREXEPath { get; set; }
        public Version Version;
        public string GameVersionString { get; set; }
        public bool HasEXEPath { get => DetectIfHasEXEPath(); }
        public bool isFailSafeMode { get; set; }
        public int FullscreenMode { get; set; }
        public bool DetectIfHasEXEPath()
        {
            if (this.AIREXEPath != null) return true;
            else return false;
        }
        public List<KeyValuePair<string, Device>> Devices { get => InputDevices.Items; set => InputDevices.Items = value; }
        public InputDevices InputDevices { get; set; }

        public string FilePath = "";

        private JObject RawJSONObject;

        public Settings(FileInfo settings)
        {
            FilePath = settings.FullName;
            if (settings.Exists) Load();
            else File.Create(settings.FullName);
        }


        private void Load()
        {
            string data = File.ReadAllText(FilePath);
            try
            {
                RawJSONObject = JObject.Parse(data);
            }
            catch
            {
                RawJSONObject = new JObject();
            }

            if (RawJSONObject.Property("GameVersion") != null)
            {
                GameVersionString = (string)RawJSONObject.Property("GameVersion").Value;
                Version.TryParse(GameVersionString, out Version);
            }
            if (RawJSONObject.ContainsKey("GameSettings")) 
            {
                string subData = RawJSONObject.Property("GameSettings").Value.ToString();
                JObject subObject = JObject.Parse(subData);
                if (subObject.ContainsKey("SETTING_FIX_GLITCHES")) FixGlitches = (bool)subObject.Property("SETTING_FIX_GLITCHES").Value;
            }
            if (RawJSONObject.Property("Fullscreen") != null) FullscreenMode = (int)RawJSONObject.Property("Fullscreen").Value;
            if (RawJSONObject.Property("RomPath") != null) Sonic3KRomPath = (string)RawJSONObject.Property("RomPath").Value;
            if (RawJSONObject.Property("GameExePath") != null) AIREXEPath = (string)RawJSONObject.Property("GameExePath").Value;
            if (RawJSONObject.Property("FailSafeMode") != null) isFailSafeMode = (bool)RawJSONObject.Property("FailSafeMode").Value;

            if (RawJSONObject.Property("InputDevices") != null) PraseInputDevices(data);
            else
            {
                InputDevices = new InputDevices();
                InputDevices.ResetDevicesToDefault();
            }



        }

        private void PraseInputDevices(string data)
        {
            RawJSONObject = JObject.Parse(data);
            if (!RawJSONObject.ContainsKey("InputDevices"))
            {
                if (HasEXEPath)
                {
                    try
                    {
                        FileInfo config = new FileInfo($"{Path.GetDirectoryName(AIREXEPath)}//config.json");
                        GameConfig gameConfig = new GameConfig(config);
                        if (gameConfig.InputDevices != null) Devices = gameConfig.InputDevices.Items;
                        else InputDevices = new InputDevices();
                    }
                    catch
                    {
                        InputDevices = new InputDevices();
                    }
 
                }
                else InputDevices = new InputDevices();
            }
            else
            {
                InputDevices = new InputDevices();
                dynamic RawJSON = JsonConvert.DeserializeObject(data);
                foreach (var device in RawJSON.InputDevices)
                {
                    if (device is Newtonsoft.Json.Linq.JProperty)
                    {
                        Newtonsoft.Json.Linq.JProperty deviceProp = device;
                        Devices.Add(new KeyValuePair<string, Device>(deviceProp.Name, new Device(deviceProp)));
                    }
                }
            }

        }

        public void Save()
        {
            SetProperty("GameVersion", GameVersionString);
            if (RawJSONObject.ContainsKey("GameSettings"))
            {
                string subData = RawJSONObject.Property("GameSettings").Value.ToString();
                JObject subObject = JObject.Parse(subData);
                if (subObject.ContainsKey("SETTING_FIX_GLITCHES")) subObject["SETTING_FIX_GLITCHES"] = (FixGlitches ? 1 : 0);
                else
                {
                    subObject.Add("SETTING_FIX_GLITCHES", (FixGlitches ? 1 : 0));
                }
                RawJSONObject["GameSettings"] = subObject;
            }
            SetProperty("Fullscreen", FullscreenMode);
            SetProperty("RomPath", Sonic3KRomPath);
            SetProperty("GameExePath", AIREXEPath);
            SetProperty("FailSafeMode", isFailSafeMode);
            if (RawJSONObject.Property("InputDevices") != null)
            {
                RawJSONObject["InputDevices"] = InputDevices.GetRawJSONValue(InputDevices);
            }
            else
            {
                RawJSONObject.Add("InputDevices", InputDevices.GetRawJSONValue(InputDevices));
            }
            File.WriteAllText(FilePath, RawJSONObject.ToString());


        }


        private void SetProperty(string key, object keyValue)
        {
            if (keyValue != null)
            {
                JToken value = JToken.FromObject(keyValue);
                if (!RawJSONObject.ContainsKey(key))
                {
                    RawJSONObject.Add(key, value);
                }
                else
                {
                    RawJSONObject.Property(key).Value = value;
                }

            }
            else
            {
                if (RawJSONObject.ContainsKey(key))
                {
                    RawJSONObject.Remove(key);
                }

            }
        }
    }
}
