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
            Items = new List<KeyValuePair<string, Device>>();
        }
        public List<KeyValuePair<string, Device>> Items { get; set; }



        public static JToken GetRawJSONValue(InputDevices input)
        {
            return JProperty.Parse(JsonConvert.SerializeObject(input.Items));
        }


        public void ResetDevicesToDefault()
        {
            Items.Clear();
            Items.Add(new KeyValuePair<string, Device>("Keyboard1", InputMappings.Keyboard1));
            Items.Add(new KeyValuePair<string, Device>("Keyboard2", InputMappings.Keyboard2));
            Items.Add(new KeyValuePair<string, Device>("XBoxController", InputMappings.XBoxController));
            Items.Add(new KeyValuePair<string, Device>("PS4Controller", InputMappings.PS4Controller));
            Items.Add(new KeyValuePair<string, Device>("LogitechController", InputMappings.LogitechController));
            Items.Add(new KeyValuePair<string, Device>("CustomController", InputMappings.CustomController));
        }

        public void ImportDevice(string filePath)
        {
            string data = File.ReadAllText(filePath);
            ExportableDevice deviceImport = Newtonsoft.Json.JsonConvert.DeserializeObject<ExportableDevice>(data);

            string intial_name = deviceImport.DeviceName;
            int copy_number = 0;
            while (Items.Exists(x => x.Key == deviceImport.DeviceName))
            {
                copy_number++;
                deviceImport.DeviceName = string.Format("{0}{1}", intial_name, copy_number);
            }

            if (deviceImport.HasDeviceNames) deviceImport.DeviceValues.HasDeviceNames = deviceImport.HasDeviceNames;
            else if (deviceImport.DeviceValues.DeviceNames.Count > 0) deviceImport.DeviceValues.HasDeviceNames = true;
            deviceImport.DeviceValues.EntryName = deviceImport.DeviceName;
            Items.Add(new KeyValuePair<string, Device>(deviceImport.DeviceName, deviceImport.DeviceValues));

        }
    }
}
