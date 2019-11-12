﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Device = AIR_API.InputMappings.Device;
using ExportableDevice = AIR_API.InputMappings.ExportableDevice;

namespace AIR_API
{
    public class GameConfig
    {
        public string FilePath = "";
        private JObject RawJSONObject;
        public Dictionary<string, Device> Devices;

        public string LoadLevel;
        public int? UseCharacters;
        public int? StartPhase;

        public List<Device> InputDevices
        {
            get => Devices.Values.ToList();
        }


        public GameConfig(FileInfo config)
        {
            FilePath = config.FullName;
            Devices = new Dictionary<string, Device>();
            string data = File.ReadAllText(FilePath);
            dynamic RawJSON = JsonConvert.DeserializeObject(data);
            RawJSONObject = JObject.Parse(data);

            if (RawJSONObject.Property("LoadLevel") != null) LoadLevel = (string)RawJSONObject.Property("LoadLevel").Value;
            if (RawJSONObject.Property("StartPhase") != null) StartPhase = (int)RawJSONObject.Property("StartPhase").Value;
            if (RawJSONObject.Property("UseCharacters") != null) UseCharacters = (int)RawJSONObject.Property("UseCharacters").Value;

            foreach (var device in RawJSON.InputDevices)
            {
                if (device is Newtonsoft.Json.Linq.JProperty)
                {
                    Newtonsoft.Json.Linq.JProperty deviceProp = device;
                    Devices.Add(deviceProp.Name,new Device(deviceProp));
                }
            }
        }

        public void ResetDevicesToDefault()
        {
            Devices.Clear();
            Devices.Add("Keyboard1", InputMappings.Keyboard1);
            Devices.Add("Keyboard2", InputMappings.Keyboard2);
            Devices.Add("XBoxController", InputMappings.XBoxController);
            Devices.Add("PS4Controller", InputMappings.PS4Controller);
            Devices.Add("LogitechController", InputMappings.LogitechController);
            Devices.Add("CustomController", InputMappings.CustomController);
        }

        public void ImportDevice(string filePath)
        {
            string data = File.ReadAllText(filePath);
            ExportableDevice deviceImport = Newtonsoft.Json.JsonConvert.DeserializeObject<ExportableDevice>(data);

            string intial_name = deviceImport.DeviceName;
            int copy_number = 0;
            while (Devices.ContainsKey(deviceImport.DeviceName))
            {
                copy_number++;
                deviceImport.DeviceName = string.Format("{0}{1}", intial_name, copy_number);
            }

            if (deviceImport.HasDeviceNames) deviceImport.DeviceValues.HasDeviceNames = deviceImport.HasDeviceNames;
            else if (deviceImport.DeviceValues.DeviceNames.Count > 0) deviceImport.DeviceValues.HasDeviceNames = true;
            deviceImport.DeviceValues.EntryName = deviceImport.DeviceName;
            Devices.Add(deviceImport.DeviceName, deviceImport.DeviceValues);

        }


        public void Save()
        {
            SetProperty("LoadLevel", LoadLevel);
            SetProperty("StartPhase", StartPhase);
            SetProperty("UseCharacters", UseCharacters);

            RawJSONObject["InputDevices"] = JProperty.Parse(JsonConvert.SerializeObject(Devices));
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

    public sealed class HexStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(uint).Equals(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue($"0x{value:x}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = reader.ReadAsString();
            if (str == null || !str.StartsWith("0x"))
                throw new JsonSerializationException();
            return Convert.ToUInt32(str);
        }
    }
}