using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace AIR_API
{
    public class Settings
    {
        public bool FailSafeMode { get => RawSettings.FailSafeMode; set => RawSettings.FailSafeMode = value; }
        public string Sonic3KRomPath { get => RawSettings.RomPath; set => RawSettings.RomPath = value; }
        public bool FixGlitches { get => RawSettings.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES != 0; set => RawSettings.GameplayTweaks.GAMEPLAY_TWEAK_FIX_GLITCHES = (value == true ? 1 : 0); }
        public bool EnableDebugMode { get => RawSettings.DebugMode; set => RawSettings.DebugMode = value; }
        public int Fullscreen { get => RawSettings.Fullscreen; set => RawSettings.Fullscreen = value; }
        public string AIREXEPath { get => RawSettings.GameExePath; set => RawSettings.GameExePath = value; }
        public Version Version { get => GetVersion(); }
        private Version GetVersion()
        {
            if (RawSettings.GameVersion != null) return new Version(RawSettings.GameVersion);
            else return null;
        }
        private bool DetectIfHasEXEPath()
        {
            if (RawSettings.GameExePath != null) return false;
            else return true;
        }

        public bool HasEXEPath { get => DetectIfHasEXEPath(); }
        public string FilePath = "";

        public AIRSettings RawSettings;

        #region Classes for Settings.JSON
        public class GameplayTweaks
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

        public class AIRSettings
        {
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

            #region Gameplay Tweaks Anti-Nulling
            public GameplayTweaks GameplayTweaks { get => GetGameplayTweaks(); set => SetGameplayTweaks(value); }

            #region Get/Set Methods
            [JsonIgnore]
            private GameplayTweaks _GameplayTweaks = new GameplayTweaks();
            private GameplayTweaks GetGameplayTweaks()
            {
                if (_GameplayTweaks == null) _GameplayTweaks = new GameplayTweaks();
                return _GameplayTweaks;
            }
            private void SetGameplayTweaks(GameplayTweaks value)
            {
                if (value != null) _GameplayTweaks = value;
            }
            #endregion
            #endregion

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
        #endregion

        public Settings(FileInfo settings)
        {
            FilePath = settings.FullName;
            string data = File.ReadAllText(FilePath);
            RawSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<AIRSettings>(data);
            if (RawSettings == null) RawSettings = new AIRSettings();
        }

        public void SaveSettings()
        {
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(RawSettings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(FilePath, output);
        }
    }
}
