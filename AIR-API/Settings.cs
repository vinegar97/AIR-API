using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Device = AIR_API.InputMappings.Device;
using AIR_API.Raw.Settings.Structures;
using AIR_API.Raw.Settings.Interfaces;

namespace AIR_API
{
    public class Settings
    {
        public bool FailSafeMode { get => RawSettings.isFailSafeMode; set => RawSettings.isFailSafeMode = value; }
        public string Sonic3KRomPath { get => RawSettings.Sonic3KRomPath; set => RawSettings.Sonic3KRomPath = value; }
        public bool FixGlitches { get => RawSettings.FixGlitches; set => RawSettings.FixGlitches = value; }
        public bool EnableDebugMode { get => RawSettings.EnableDebugMode; set => RawSettings.EnableDebugMode = value; }
        public int Fullscreen { get => RawSettings.FullscreenMode; set => RawSettings.FullscreenMode = value; }
        public string AIREXEPath { get => RawSettings.AIREXEPath; set => RawSettings.AIREXEPath = value; }
        public Version Version { get => RawSettings.Version; }
        public bool HasEXEPath { get => RawSettings.HasEXEPath; }
        public enum FullscreenType : int
        {
            Windowed = 0,
            Fullscreen = 1,
            ExclusiveFS = 2
        }
        public InputDevices InputDevices { get; set; }
        public List<KeyValuePair<string, Device>> Devices { get => InputDevices.Items; set => InputDevices.Items = value; }

        public string FilePath = "";

        public AIRSettingsBase RawSettings;

        public Settings(FileInfo settings)
        {
            FilePath = settings.FullName;
            string data = File.ReadAllText(FilePath);
            AIRSettingsBase.PraseSettings(ref RawSettings, data);

            if (RawSettings is AIRSettingsMK2)
            {
                try
                {
                    PraseInputDevices(data);
                }
                catch
                {
                    InputDevices = new InputDevices();
                }
            }




        }

        private void PraseInputDevices(string data)
        {
            JObject RawJSONObject = JObject.Parse(data);
            if (!RawJSONObject.ContainsKey("InputDevices"))
            {
                if ((RawSettings as AIRSettingsMK2).HasEXEPath)
                {
                    try
                    {
                        FileInfo config = new FileInfo($"{Path.GetDirectoryName(RawSettings.AIREXEPath)}//config.json");
                        GameConfig gameConfig = new GameConfig(config);
                        if (gameConfig.InputDevices != null) InputDevices = gameConfig.InputDevices;
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
            AIRSettingsBase.SaveSettings(ref RawSettings, FilePath, InputDevices);
        }
    }
}
