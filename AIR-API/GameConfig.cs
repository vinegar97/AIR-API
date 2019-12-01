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



        public InputDevices InputDevices { get; set; }
        public Dictionary<string, Device> Devices { get => InputDevices.KeyPairs; set => InputDevices.KeyPairs = value; }



        public string LoadLevel;
        public int? UseCharacters;
        public int? StartPhase;


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

            if (RawJSONObject.Property("InputDevices") != null)
            {
                foreach (var device in RawJSON.InputDevices)
                {
                    if (device is Newtonsoft.Json.Linq.JProperty)
                    {
                        Newtonsoft.Json.Linq.JProperty deviceProp = device;
                        Devices.Add(deviceProp.Name, new Device(deviceProp));
                    }
                }
            }
        }

        public void Save()
        {
            SetProperty("LoadLevel", LoadLevel);
            SetProperty("StartPhase", StartPhase);
            SetProperty("UseCharacters", UseCharacters);

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
