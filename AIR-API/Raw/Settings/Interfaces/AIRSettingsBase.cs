using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AIR_API.Raw.Settings.Structures;

namespace AIR_API.Raw.Settings.Interfaces
{
    public class AIRSettingsBase
    {

        [JsonIgnore]
        public virtual bool isFailSafeMode { get; set; }
        [JsonIgnore]
        public virtual string Sonic3KRomPath { get; set; }
        [JsonIgnore]
        public virtual bool FixGlitches { get; set; }
        [JsonIgnore]
        public virtual bool EnableDebugMode { get; set; }
        [JsonIgnore]
        public virtual int FullscreenMode { get; set; }
        [JsonIgnore]
        public virtual string AIREXEPath { get; set; }
        [JsonIgnore]
        public virtual Version Version { get; set; }
        [JsonIgnore]
        public virtual bool HasEXEPath { get; }
        [JsonIgnore]
        public virtual SettingsV0 Structure { get; set; }

        public static void PraseSettings(ref AIRSettingsBase baseClass, string data, bool allowFail = false)
        {
            try
            {
                var rawSettings = (JObject)JsonConvert.DeserializeObject(data);
                string rawVersion = rawSettings["GameVersion"].Value<string>();

                if (isVersionMK2(rawVersion))
                {
                    baseClass = new AIRSettingsMK2();
                    baseClass.Structure = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsV2>(data);
                    if (baseClass.Structure == null) baseClass.Structure = new SettingsV2();
                }
                else
                {
                    baseClass = new AIRSettingsMK1();
                    baseClass.Structure = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsV1>(data);
                    if (baseClass.Structure == null) baseClass.Structure = new SettingsV1();
                }
            }
            catch
            {
                if (baseClass == null) baseClass = new AIRSettingsMK2();
                if (baseClass.Structure == null) baseClass.Structure = new SettingsV2();
            }


        }

        public static void SaveSettings(ref AIRSettingsBase baseClass, string filePath, InputDevices inputDevices = null)
        {

            if (baseClass is AIRSettingsMK2)
            {
                (baseClass as AIRSettingsMK2).InputDevices = KeyPairListToDictionaryHelper.ToDictionary(inputDevices.Items, x => x.Key, x => x.Value);
                ((baseClass as AIRSettingsMK2).Structure as SettingsV2).GameSettings = (baseClass as AIRSettingsMK2).GameSettings;
            }


            string output = Newtonsoft.Json.JsonConvert.SerializeObject(baseClass.Structure, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, output);
        }



        public static bool isVersionMK2(string verString)
        {
            try
            {
                Version currentVersion = new Version(verString);
                Version versionToMeetOrBeat = new Version("19.11.30.0");
                return (currentVersion.CompareTo(versionToMeetOrBeat) >= 0 ? true : false);
            }
            catch
            {
                return false;
            }

        }
    }
}
