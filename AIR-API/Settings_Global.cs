using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using Device = AIR_API.InputMappings.Device;

namespace AIR_API
{
    public class Settings_Global
    {
        public enum FullscreenType : int
        {
            Windowed = 0,
            Fullscreen = 1,
            ExclusiveFS = 2
        }



        public class GlobalSettings
        {
            public class Game_Settings
            {           
                public int SETTING_FIX_GLITCHES { get; set; }             
            }       
            public bool FailSafeMode { get; set; }
            public bool DevMode { get; set; }
            public int Fullscreen { get; set; }
            public string GameExePath { get; set; }
            public Game_Settings GameSettings { get; set; }
            public string GameVersion { get; set; }
            public InputDevices InputDevices { get; set; }
            public string RomPath { get; set; }
            public bool UseSoftwareRenderer { get; set; }
            public int? GameRecording { get; set; }
            public string LoadLevel { get; set; }
            public int? UseCharacters { get; set; }
            public int? StartPhase { get; set; }
        }

        public GlobalSettings Structure { get; set; }

        #region Variables

        public string RomPath 
        { 
            get 
            {
                if (Structure.RomPath == null) Structure.RomPath = SteamROMHandler.TryGetSteamRomPath();
                return Structure.RomPath;
            } 
            set 
            {
                Structure.RomPath = value;
            } 
        }
        public bool FixGlitches
        {
            get
            {
                if (Structure.GameSettings == null) Structure.GameSettings = new GlobalSettings.Game_Settings();
                if (Structure.GameSettings.SETTING_FIX_GLITCHES == null) Structure.GameSettings.SETTING_FIX_GLITCHES = 1;
                if (bool.TryParse(Structure.GameSettings.SETTING_FIX_GLITCHES.ToString(), out bool result))
                {
                    return result;
                }
                else return true;
            }
            set
            {
                if (Structure.GameSettings == null) Structure.GameSettings = new GlobalSettings.Game_Settings();
                Structure.GameSettings.SETTING_FIX_GLITCHES = (value ? 1 : 0);
            }
        }
        public bool FailSafeMode
        {
            get
            {
                if (Structure.FailSafeMode == null) Structure.FailSafeMode = false;
                return Structure.FailSafeMode;
            }
            set
            {
                Structure.FailSafeMode = value;
            }
        }
        public int Fullscreen
        {
            get
            {
                if (Structure.Fullscreen == null) Structure.Fullscreen = 0;
                return Structure.Fullscreen;
            }
            set
            {
                Structure.Fullscreen = value;
            }
        }
        public bool DevMode
        {
            get
            {
                if (Structure.DevMode == null) Structure.DevMode = false;
                return Structure.DevMode;
            }
            set
            {
                Structure.DevMode = value;
            }
        }
        public bool UseSoftwareRenderer
        {
            get
            {
                if (Structure.UseSoftwareRenderer == null) Structure.UseSoftwareRenderer = false;
                return Structure.UseSoftwareRenderer;
            }
            set
            {
                Structure.UseSoftwareRenderer = value;
            }
        }
        public int? GameRecording
        {
            get
            {
                if (Structure.GameRecording == null) Structure.GameRecording = -1;
                return Structure.GameRecording;
            }
            set
            {
                Structure.GameRecording = value;
            }
        }
        public string LoadLevel
        {
            get
            {
                return Structure.LoadLevel;
            }
            set
            {
                Structure.LoadLevel = value;
            }
        }
        public int? UseCharacters
        {
            get
            {
                return Structure.UseCharacters;
            }
            set
            {
                Structure.UseCharacters = value;
            }
        }
        public int? StartPhase
        {
            get
            {
                return Structure.StartPhase;
            }
            set
            {
                Structure.StartPhase = value;
            }
        }

        #endregion

        public string FilePath = "";

        public Settings_Global(FileInfo settings)
        {
            FilePath = settings.FullName;
            if (settings.Exists) Load();
            else File.Create(settings.FullName).Close();
            Load();
        }

        private string GetSteamRomPath()
        {
            return "";
        }


        private void Load()
        {
            string data = File.ReadAllText(FilePath);
            Structure = Newtonsoft.Json.JsonConvert.DeserializeObject<GlobalSettings>(data);
            if (Structure == null)
            {
                Structure = new GlobalSettings();
                Save();
            }
        }

        public void Save()
        {
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(Structure, Newtonsoft.Json.Formatting.Indented, NullSettings);
            File.WriteAllText(FilePath, output);
        }



        public JsonSerializerSettings NullSettings
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
            }

        }
    }
}
