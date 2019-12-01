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


        public InputDevices InputDevices { get; set; }
        public Dictionary<string, Device> Devices { get => InputDevices.KeyPairs; set => InputDevices.KeyPairs = value; }


        public string FilePath = "";

        public AIRSettingsBase RawSettings;

        public Settings(FileInfo settings)
        {
            FilePath = settings.FullName;
            string data = File.ReadAllText(FilePath);
            AIRSettingsBase.PraseSettings(ref RawSettings, data);

            if (RawSettings is AIRSettingsMK2) PraseInputDevices(data);


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
                        InputDevices = gameConfig.InputDevices;
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
                        Devices.Add(deviceProp.Name, new Device(deviceProp));
                    }
                }
            }

        }

        public void SaveSettings()
        {
            AIRSettingsBase.SaveSettings(ref RawSettings, FilePath, InputDevices);
        }
    }


    #region AIR Settings Classes

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

        public static void PraseSettings(ref AIRSettingsBase baseClass, string data, bool allowFail = false)
        {
            try
            {
                var rawSettings = (JObject)JsonConvert.DeserializeObject(data);
                string rawVersion = rawSettings["GameVersion"].Value<string>();

                if (isVersionMK2(rawVersion))
                {
                    baseClass = Newtonsoft.Json.JsonConvert.DeserializeObject<AIRSettingsMK2>(data);
                    if (baseClass == null) baseClass = new AIRSettingsMK2();
                }
                else
                {
                    baseClass = Newtonsoft.Json.JsonConvert.DeserializeObject<AIRSettingsMK1>(data);
                    if (baseClass == null) baseClass = new AIRSettingsMK1();
                }
            }
            catch
            {
                if (baseClass == null) baseClass = new AIRSettingsMK2();
            }


        }

        public static void SaveSettings(ref AIRSettingsBase baseClass, string filePath, InputDevices inputDevices = null)
        {
            if (baseClass is AIRSettingsMK2) (baseClass as AIRSettingsMK2).InputDevices = inputDevices.KeyPairs;


            string output = Newtonsoft.Json.JsonConvert.SerializeObject(baseClass, Newtonsoft.Json.Formatting.Indented);
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
    public partial class AIRSettingsMK1 : AIRSettingsBase
    {
        #region Base Methods
        [JsonIgnore]
        public override string Sonic3KRomPath { get => this.RomPath; set => this.RomPath = value; }
        [JsonIgnore]
        public override bool FixGlitches { get => this.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES != 0; set => this.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES = (value == true ? 1 : 0); }
        [JsonIgnore]
        public override bool EnableDebugMode { get => this.DebugMode; set => this.DebugMode = value; }
        [JsonIgnore]
        public override string AIREXEPath { get => this.GameExePath; set => this.GameExePath = value; }
        [JsonIgnore]
        public override Version Version { get => GetVersion(); }
        [JsonIgnore]
        public override bool HasEXEPath { get => DetectIfHasEXEPath(); }
        [JsonIgnore]
        public override bool isFailSafeMode { get => this.FailSafeMode; set => this.FailSafeMode = value; }
        [JsonIgnore]
        public override int FullscreenMode { get => this.Fullscreen; set => this.Fullscreen = value; }


        #endregion

        #region Get/Set Methods

        private Version GetVersion()
        {
            if (this.GameVersion != null) return new Version(this.GameVersion);
            else return null;
        }
        public bool DetectIfHasEXEPath()
        {
            if (this.GameExePath != null) return false;
            else return true;
        }

        #endregion

        #region Gameplay Tweaks Anti-Nulling
        [JsonIgnore]
        private Gameplay_Tweaks _GameplayTweaks = new Gameplay_Tweaks();
        private Gameplay_Tweaks GetGameplayTweaks()
        {
            if (_GameplayTweaks == null) _GameplayTweaks = new Gameplay_Tweaks();
            return _GameplayTweaks;
        }
        private void SetGameplayTweaks(Gameplay_Tweaks value)
        {
            if (value != null) _GameplayTweaks = value;
        }
        #endregion


        public class Gameplay_Tweaks
        {
            public int GAMEPLAY_TWEAK_AIZ_BLIMPSEQUENCE { get; set; }
            public int GAMEPLAY_TWEAK_BS_COUNTDOWN_RINGS { get; set; }
            public int GAMEPLAY_TWEAK_BS_REPEAT_ON_FAIL { get; set; }
            public int GAMEPLAY_TWEAK_CAMERA_OUTRUN { get; set; }
            public int GAMEPLAY_TWEAK_CANCEL_FLIGHT { get; set; }
            public int GAMEPLAY_TWEAK_DISABLE_GHOST_SPAWN { get; set; }
            public int GAMEPLAY_TWEAK_EXTENDED_CAMERA { get; set; }
            public int GAMEPLAY_TWEAK_EXTENDED_HUD { get; set; }
            public int GAMEPLAY_TWEAK_FIX_GLITCHES { get; set; }
            public int GAMEPLAY_TWEAK_HYPER_TAILS { get; set; }
            public int GAMEPLAY_TWEAK_ICZ_NIGHTTIME { get; set; }
            public int GAMEPLAY_TWEAK_INFINITE_LIVES { get; set; }
            public int GAMEPLAY_TWEAK_INFINITE_TIME { get; set; }
            public int GAMEPLAY_TWEAK_LBZ_BIGARMS { get; set; }
            public int GAMEPLAY_TWEAK_LEVELLAYOUTS { get; set; }
            public int GAMEPLAY_TWEAK_RANDOM_MONITORS { get; set; }
            public int GAMEPLAY_TWEAK_RANDOM_SPECIALSTAGES { get; set; }
            public int GAMEPLAY_TWEAK_SMOOTH_ROTATION { get; set; }
            public int GAMEPLAY_TWEAK_SPEEDUP_AFTERIMGS { get; set; }
            public int GAMEPLAY_TWEAK_SSZ_BOSS_TRACKS { get; set; }
            public int GAMEPLAY_TWEAK_SUPERFAST_RUNANIM { get; set; }
            public int GAMEPLAY_TWEAK_SUPER_CANCEL { get; set; }
            public int GAMEPLAY_TWEAK_TAILS_ASSIST_MODE { get; set; }
        }
        public int ActiveSoundtrack { get; set; }
        public double Audio_MusicVolume { get; set; }
        public double Audio_SoundVolume { get; set; }
        public int BackgroundBlur { get; set; }
        public bool DebugMode { get; set; }
        public bool DropDashActive { get; set; }
        public bool FailSafeMode { get; set; }
        public int Filtering { get; set; }
        public int Fullscreen { get; set; }
        public string GameExePath { get; set; }
        public string GameVersion { get; set; }
        public Gameplay_Tweaks GameplayTweaks { get => GetGameplayTweaks(); set => SetGameplayTweaks(value); }
        public int GfxAntiFlicker { get; set; }
        public int MusicSelect_ExtraLifeJingle { get; set; }
        public int MusicSelect_HiddenPalaceMusic { get; set; }
        public int MusicSelect_InvincibilityTheme { get; set; }
        public int MusicSelect_KnucklesTheme { get; set; }
        public int MusicSelect_MiniBossTheme { get; set; }
        public int MusicSelect_SuperTheme { get; set; }
        public int MusicSelect_TitleTheme { get; set; }
        public int Region { get; set; }
        public string RomPath { get; set; }
        public int Scanlines { get; set; }
        public int SpecialStageVisuals { get; set; }
        public bool SuperPeelOutActive { get; set; }
        public int TimeAttackGhosts { get; set; }
        public int Upscaling { get; set; }
        public bool UseSoftwareRenderer { get; set; }
        public double Volume { get; set; }
    }
    public partial class AIRSettingsMK2 : AIRSettingsBase
    {
        #region Base Methods
        [JsonIgnore]
        public override string Sonic3KRomPath { get => this.RomPath; set => this.RomPath = value; }
        [JsonIgnore]
        public override bool FixGlitches { get => this.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES != 0; set => this.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES = (value == true ? 1 : 0); }
        [JsonIgnore]
        public override bool EnableDebugMode { get => this.DebugMode; set => this.DebugMode = value; }
        [JsonIgnore]
        public override string AIREXEPath { get => this.GameExePath; set => this.GameExePath = value; }
        [JsonIgnore]
        public override Version Version { get => GetVersion(); }
        [JsonIgnore]
        public override bool HasEXEPath { get => DetectIfHasEXEPath(); }
        [JsonIgnore]
        public override bool isFailSafeMode { get => this.FailSafeMode; set => this.FailSafeMode = value; }
        [JsonIgnore]
        public override int FullscreenMode { get => this.Fullscreen; set => this.Fullscreen = value; }


        #endregion

        #region Get/Set Methods
        private Version GetVersion()
        {
            if (this.GameVersion != null) return new Version(this.GameVersion);
            else return null;
        }
        public bool DetectIfHasEXEPath()
        {
            if (this.GameExePath != null) return true;
            else return false;
        }
        #endregion

        #region Gameplay Tweaks Anti-Nulling

        [JsonIgnore]
        private Gameplay_Tweaks _GameplayTweaks = new Gameplay_Tweaks();
        private Gameplay_Tweaks GetGameplayTweaks()
        {
            if (_GameplayTweaks == null) _GameplayTweaks = new Gameplay_Tweaks();
            return _GameplayTweaks;
        }
        private void SetGameplayTweaks(Gameplay_Tweaks value)
        {
            if (value != null) _GameplayTweaks = value;
        }

        #endregion

        #region Game Settings Anti-Nulling


        [JsonIgnore]
        private Game_Settings _GameSettings = new Game_Settings();
        private Game_Settings GetGameSettings()
        {
            if (_GameSettings == null) _GameSettings = new Game_Settings();
            return _GameSettings;
        }
        private void SetGameSettings(Game_Settings value)
        {
            if (value != null) _GameSettings = value;
        }

        #endregion

        public class Game_Settings
        {
            public int SETTING_AIZ_BLIMPSEQUENCE { get; set; }
            public int SETTING_AUDIO_EXTRALIFE_JINGLE { get; set; }
            public int SETTING_AUDIO_HPZ_MUSIC { get; set; }
            public int SETTING_AUDIO_INVINCIBILITY_THEME { get; set; }
            public int SETTING_AUDIO_KNUCKLES_THEME { get; set; }
            public int SETTING_AUDIO_MINIBOSS_THEME { get; set; }
            public int SETTING_AUDIO_SUPER_THEME { get; set; }
            public int SETTING_AUDIO_TITLE_THEME { get; set; }
            public int SETTING_BS_COUNTDOWN_RINGS { get; set; }
            public int SETTING_BS_REPEAT_ON_FAIL { get; set; }
            public int SETTING_BS_VISUAL_STYLE { get; set; }
            public int SETTING_CAMERA_OUTRUN { get; set; }
            public int SETTING_CANCEL_FLIGHT { get; set; }
            public int SETTING_CNZ_PROTOTYPE_MUSIC { get; set; }
            public int SETTING_DISABLE_GHOST_SPAWN { get; set; }
            public int SETTING_EXTENDED_CAMERA { get; set; }
            public int SETTING_EXTENDED_HUD { get; set; }
            public int SETTING_FIX_GLITCHES { get; set; }
            public int SETTING_GFX_ANTIFLICKER { get; set; }
            public int SETTING_HYPER_TAILS { get; set; }
            public int SETTING_ICZ_NIGHTTIME { get; set; }
            public int SETTING_ICZ_PROTOTYPE_MUSIC { get; set; }
            public int SETTING_INFINITE_LIVES { get; set; }
            public int SETTING_INFINITE_TIME { get; set; }
            public int SETTING_INSTA_SHIELD { get; set; }
            public int SETTING_LBZ_BIGARMS { get; set; }
            public int SETTING_LBZ_PROTOTYPE_MUSIC { get; set; }
            public int SETTING_LEVELLAYOUTS { get; set; }
            public int SETTING_MONITOR_STYLE { get; set; }
            public int SETTING_RANDOM_MONITORS { get; set; }
            public int SETTING_RANDOM_SPECIALSTAGES { get; set; }
            public int SETTING_REGION_CODE { get; set; }
            public int SETTING_SHIELD_TYPES { get; set; }
            public int SETTING_SMOOTH_ROTATION { get; set; }
            public int SETTING_SPEEDUP_AFTERIMGS { get; set; }
            public int SETTING_SSZ_BOSS_TRACKS { get; set; }
            public int SETTING_SUPERFAST_RUNANIM { get; set; }
            public int SETTING_SUPER_CANCEL { get; set; }
            public int SETTING_TAILS_ASSIST_MODE { get; set; }
            public int SETTING_TIME_ATTACK_GHOSTS { get; set; }
        }
        public class Gameplay_Tweaks
        {
            public int GAMEPLAY_TWEAK_AIZ_BLIMPSEQUENCE { get; set; }
            public int GAMEPLAY_TWEAK_BS_COUNTDOWN_RINGS { get; set; }
            public int GAMEPLAY_TWEAK_BS_REPEAT_ON_FAIL { get; set; }
            public int GAMEPLAY_TWEAK_CAMERA_OUTRUN { get; set; }
            public int GAMEPLAY_TWEAK_CANCEL_FLIGHT { get; set; }
            public int GAMEPLAY_TWEAK_DISABLE_GHOST_SPAWN { get; set; }
            public int GAMEPLAY_TWEAK_EXTENDED_CAMERA { get; set; }
            public int GAMEPLAY_TWEAK_EXTENDED_HUD { get; set; }
            public int GAMEPLAY_TWEAK_FIX_GLITCHES { get; set; }
            public int GAMEPLAY_TWEAK_HYPER_TAILS { get; set; }
            public int GAMEPLAY_TWEAK_ICZ_NIGHTTIME { get; set; }
            public int GAMEPLAY_TWEAK_INFINITE_LIVES { get; set; }
            public int GAMEPLAY_TWEAK_INFINITE_TIME { get; set; }
            public int GAMEPLAY_TWEAK_LBZ_BIGARMS { get; set; }
            public int GAMEPLAY_TWEAK_LEVELLAYOUTS { get; set; }
            public int GAMEPLAY_TWEAK_RANDOM_MONITORS { get; set; }
            public int GAMEPLAY_TWEAK_RANDOM_SPECIALSTAGES { get; set; }
            public int GAMEPLAY_TWEAK_SMOOTH_ROTATION { get; set; }
            public int GAMEPLAY_TWEAK_SPEEDUP_AFTERIMGS { get; set; }
            public int GAMEPLAY_TWEAK_SSZ_BOSS_TRACKS { get; set; }
            public int GAMEPLAY_TWEAK_SUPERFAST_RUNANIM { get; set; }
            public int GAMEPLAY_TWEAK_SUPER_CANCEL { get; set; }
            public int GAMEPLAY_TWEAK_TAILS_ASSIST_MODE { get; set; }
        }

        public int ActiveSoundtrack { get; set; }
        public double Audio_MusicVolume { get; set; }
        public double Audio_SoundVolume { get; set; }
        public bool DebugMode { get; set; }
        public int BackgroundBlur { get; set; }
        public int CleanupSettings { get; set; }
        public bool DropDashActive { get; set; }
        public bool FailSafeMode { get; set; }
        public int Filtering { get; set; }
        public int Fullscreen { get; set; }
        public string GameExePath { get; set; }
        public Game_Settings GameSettings { get => GetGameSettings(); set => SetGameSettings(value); }
        public string GameVersion { get; set; }
        public Gameplay_Tweaks GameplayTweaks { get => GetGameplayTweaks(); set => SetGameplayTweaks(value); }
        public int GfxAntiFlicker { get; set; }
        public Dictionary<string, Device> InputDevices { get; set; }
        public int MusicSelect_ExtraLifeJingle { get; set; }
        public int MusicSelect_HiddenPalaceMusic { get; set; }
        public int MusicSelect_InvincibilityTheme { get; set; }
        public int MusicSelect_KnucklesTheme { get; set; }
        public int MusicSelect_MiniBossTheme { get; set; }
        public int MusicSelect_SuperTheme { get; set; }
        public int MusicSelect_TitleTheme { get; set; }
        public int Region { get; set; }
        public string RomPath { get; set; }
        public int Scanlines { get; set; }
        public int SpecialStageVisuals { get; set; }
        public bool SuperPeelOutActive { get; set; }
        public int TimeAttackGhosts { get; set; }
        public int Upscaling { get; set; }
        public bool UseSoftwareRenderer { get; set; }
        public double Volume { get; set; }
    }


    #endregion
}
