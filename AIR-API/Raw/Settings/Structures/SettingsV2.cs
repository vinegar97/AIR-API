using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Device = AIR_API.InputMappings.Device;

namespace AIR_API.Raw.Settings.Structures
{
    public class SettingsV2 : SettingsV0
    {

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
        public Game_Settings GameSettings { get; set; }
        public string GameVersion { get; set; }
        public Gameplay_Tweaks GameplayTweaks { get; set; }
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
}
