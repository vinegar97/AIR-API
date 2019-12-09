using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AIR_API.Raw.Settings.Structures;
using Device = AIR_API.InputMappings.Device;

namespace AIR_API.Raw.Settings.Interfaces
{
    public partial class AIRSettingsMK2 : AIRSettingsBase
    {
        #region Base Methods
        [JsonIgnore]
        public override string Sonic3KRomPath { get => this._Struct.RomPath; set => this._Struct.RomPath = value; }
        [JsonIgnore]
        public override bool FixGlitches { get => this.GameSettings.SETTING_FIX_GLITCHES != 0; set => this.GameSettings.SETTING_FIX_GLITCHES = (value == true ? 1 : 0); }
        [JsonIgnore]
        public override bool EnableDebugMode { get => this.GameSettings.SETTING_DEBUG_MODE != 0; set => this.GameSettings.SETTING_DEBUG_MODE = (value == true ? 1 : 0); }
        [JsonIgnore]
        public override string AIREXEPath { get => this._Struct.GameExePath; set => this._Struct.GameExePath = value; }
        [JsonIgnore]
        public override Version Version { get => GetVersion(); }
        [JsonIgnore]
        public override bool HasEXEPath { get => DetectIfHasEXEPath(); }
        [JsonIgnore]
        public override bool isFailSafeMode { get => this._Struct.FailSafeMode; set => this._Struct.FailSafeMode = value; }
        [JsonIgnore]
        public override int FullscreenMode { get => this._Struct.Fullscreen; set => this._Struct.Fullscreen = value; }

        public SettingsV2.Game_Settings GameSettings { get => GetGameSettings(); set => SetGameSettings(value); }
        public Dictionary<string, Device> InputDevices { get => this._Struct.InputDevices; set => this._Struct.InputDevices = value; }

        #endregion

        #region Get/Set Methods
        private Version GetVersion()
        {
            if (this._Struct.GameVersion != null) return new Version(this._Struct.GameVersion);
            else return null;
        }
        public bool DetectIfHasEXEPath()
        {
            if (this._Struct.GameExePath != null) return true;
            else return false;
        }
        #endregion

        #region Game Settings Anti-Nulling


        [JsonIgnore]
        private SettingsV2.Game_Settings _GameSettings = new SettingsV2.Game_Settings();
        private SettingsV2.Game_Settings GetGameSettings()
        {
            if (_GameSettings == null) _GameSettings = new SettingsV2.Game_Settings();
            return _GameSettings;
        }
        private void SetGameSettings(SettingsV2.Game_Settings value)
        {
            if (value != null) _GameSettings = value;
        }

        #endregion

        #region Structure Overrides
        public override SettingsV0 Structure { get => _Struct; set => SetStruct(value); }
        private SettingsV2 _Struct { get; set; }

        private void SetStruct(SettingsV0 val)
        {
            if (val is SettingsV2)
            {
                _Struct = val as SettingsV2;
            }
        }

        public AIRSettingsMK2()
        {
            Structure = new SettingsV2();
        }
        #endregion
    }
}
