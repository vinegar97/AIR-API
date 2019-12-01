using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Device = AIR_API.InputMappings.Device;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using ExportableDevice = AIR_API.InputMappings.ExportableDevice;

namespace AIR_API
{
    public class InputDevices
    {
        public InputDevices()
        {
            KeyPairs = new Dictionary<string, Device>();
        }
        public Dictionary<string, Device> KeyPairs { get; set; }
        public List<Device> Items
        {
            get => KeyPairs.Values.ToList();
        }


        public static JToken GetRawJSONValue(InputDevices input)
        {
            return JProperty.Parse(JsonConvert.SerializeObject(input.KeyPairs));
        }


        public void ResetDevicesToDefault()
        {
            KeyPairs.Clear();
            KeyPairs.Add("Keyboard1", InputMappings.Keyboard1);
            KeyPairs.Add("Keyboard2", InputMappings.Keyboard2);
            KeyPairs.Add("XBoxController", InputMappings.XBoxController);
            KeyPairs.Add("PS4Controller", InputMappings.PS4Controller);
            KeyPairs.Add("LogitechController", InputMappings.LogitechController);
            KeyPairs.Add("CustomController", InputMappings.CustomController);
        }

        public void ImportDevice(string filePath)
        {
            string data = File.ReadAllText(filePath);
            ExportableDevice deviceImport = Newtonsoft.Json.JsonConvert.DeserializeObject<ExportableDevice>(data);

            string intial_name = deviceImport.DeviceName;
            int copy_number = 0;
            while (KeyPairs.ContainsKey(deviceImport.DeviceName))
            {
                copy_number++;
                deviceImport.DeviceName = string.Format("{0}{1}", intial_name, copy_number);
            }

            if (deviceImport.HasDeviceNames) deviceImport.DeviceValues.HasDeviceNames = deviceImport.HasDeviceNames;
            else if (deviceImport.DeviceValues.DeviceNames.Count > 0) deviceImport.DeviceValues.HasDeviceNames = true;
            deviceImport.DeviceValues.EntryName = deviceImport.DeviceName;
            KeyPairs.Add(deviceImport.DeviceName, deviceImport.DeviceValues);

        }
    }
}
