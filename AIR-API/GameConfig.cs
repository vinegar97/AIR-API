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
    public class GameConfig
    {
        public string FilePath = "";
        private JObject RawJSONObject;
        public List<KeyValuePair<string, Device>> Devices { get => InputDevices.Items; set => InputDevices.Items = value.ToList(); }


        public InputDevices InputDevices { get; set; }
        public string RomPath { get; set; }
        public string WindowSize { get; set; }
        public string AudioSampleRate { get; set; }
        public int? GameRecording { get; set; }
        public string LoadLevel { get; set; }
        public int? UseCharacters { get; set; }
        public int? StartPhase { get; set; }

        public GameConfig(FileInfo config)
        {
            FilePath = config.FullName;
            InputDevices = new InputDevices();
            string data = File.ReadAllText(FilePath);
            dynamic RawJSON = JsonConvert.DeserializeObject(data);
            RawJSONObject = JObject.Parse(data);

            if (RawJSONObject.Property("LoadLevel") != null) LoadLevel = (string)RawJSONObject.Property("LoadLevel").Value;
            if (RawJSONObject.Property("StartPhase") != null) StartPhase = (int)RawJSONObject.Property("StartPhase").Value;
            if (RawJSONObject.Property("UseCharacters") != null) UseCharacters = (int)RawJSONObject.Property("UseCharacters").Value;
            if (RawJSONObject.Property("GameRecording") != null) GameRecording = (int)RawJSONObject.Property("GameRecording").Value;
            if (RawJSONObject.Property("RomPath") != null) RomPath = (string)RawJSONObject.Property("RomPath").Value;
            if (RawJSONObject.Property("WindowSize") != null) WindowSize = (string)RawJSONObject.Property("WindowSize").Value;
            if (RawJSONObject.Property("AudioSampleRate") != null) AudioSampleRate = (string)RawJSONObject.Property("AudioSampleRate").Value;

            if (RawJSONObject.Property("InputDevices") != null)
            {
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
            SetProperty("GameRecording", GameRecording);
            SetProperty("LoadLevel", LoadLevel);
            SetProperty("StartPhase", StartPhase);
            SetProperty("UseCharacters", UseCharacters);

            SetProperty("AudioSampleRate", AudioSampleRate);
            SetProperty("WindowSize", WindowSize);
            SetProperty("RomPath", RomPath);

            RawJSONObject["InputDevices"] = InputDevices.GetRawJSONValue(InputDevices);
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
