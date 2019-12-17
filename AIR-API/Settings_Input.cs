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
    public class Settings_Input
    {
        public List<KeyValuePair<string, Device>> Devices { get => InputDevices.Items; set => InputDevices.Items = value; }
        public InputDevices InputDevices { get; set; }
        public string FilePath { get; set; } = "";
        private JObject RawJSONObject { get; set; }

        public Settings_Input(FileInfo settings)
        {
            FilePath = settings.FullName;
            if (settings.Exists) Load();
            else File.Create(settings.FullName).Close();
            Load();
        }

        private void Load()
        {
            string data = File.ReadAllText(FilePath);
            try
            {
                RawJSONObject = JObject.Parse(data);
                if (RawJSONObject.Property("InputDevices") != null) PraseInputDevices(data);
                else
                {
                    InputDevices = new InputDevices();
                    InputDevices.ResetDevicesToDefault();
                }
            }
            catch
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
                InputDevices = new InputDevices();
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
            if (RawJSONObject == null) RawJSONObject = new JObject();
            if (RawJSONObject.Property("InputDevices") != null) RawJSONObject["InputDevices"] = InputDevices.GetRawJSONValue(InputDevices);
            else RawJSONObject.Add("InputDevices", InputDevices.GetRawJSONValue(InputDevices));
            File.WriteAllText(FilePath, RawJSONObject.ToString());
        }
    }
}
