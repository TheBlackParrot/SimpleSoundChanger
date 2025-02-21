using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace SimpleSoundChanger.Patches
{
    [HarmonyPatch]
    internal class SoundPatch
    {
        private static string SoundsPath => Plugin.SoundsPath;
        
        private static AudioClip LoadAudio(string fileName)
        {
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip($"file:///{fileName}", AudioType.OGGVORBIS);
            UnityWebRequestAsyncOperation task = request.SendWebRequest();
            
            while (!task.isDone);

            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            return clip;
        }
        private static List<AudioClip> LoadDirectory(string directory)
        {
            List<AudioClip> result = new List<AudioClip>();

            string[] files = Directory.GetFiles(Path.Combine(SoundsPath, directory));
            foreach (string file in files)
            {
                result.Add(LoadAudio(file));
            }

            Plugin.Log.Info($"Found {result.Count} sounds in {directory}");
            return result;
        }

        private static List<AudioClip> _cachedHitSounds = new List<AudioClip>();
        private static AudioClip[] HitSounds
        {
            get
            {
                if (_cachedHitSounds.Count > 0)
                {
                    if (_cachedHitSounds[0].loadState == AudioDataLoadState.Loaded && _cachedHitSounds[0].samples > 0)
                    {
                        return _cachedHitSounds.ToArray();
                    }
                }

                _cachedHitSounds = LoadDirectory("hits");
                return _cachedHitSounds.ToArray();
            }
        }

        private static List<AudioClip> _cachedMissSounds = new List<AudioClip>();
        private static AudioClip[] MissSounds
        {
            get
            {
                if (_cachedMissSounds.Count > 0)
                {
                    if (_cachedMissSounds[0].loadState == AudioDataLoadState.Loaded && _cachedMissSounds[0].samples > 0)
                    {
                        return _cachedMissSounds.ToArray();
                    }
                }

                _cachedMissSounds = LoadDirectory("misses");
                return _cachedMissSounds.ToArray();
            }
        }

        private static List<AudioClip> _cachedClickSounds = new List<AudioClip>();
        private static AudioClip[] ClickSounds
        {
            get
            {
                if (_cachedClickSounds.Count > 0)
                {
                    if (_cachedClickSounds[0].loadState == AudioDataLoadState.Loaded && _cachedClickSounds[0].samples > 0)
                    {
                        return _cachedClickSounds.ToArray();
                    }
                }
                
                _cachedClickSounds = LoadDirectory("clicks");
                return _cachedClickSounds.ToArray();
            }
        }
        
        private static AudioClip _cachedMenuMusic;
        
        private static AudioClip MenuMusic
        {
            get
            {
                if (_cachedMenuMusic != null)
                {
                    if (_cachedMenuMusic.loadState == AudioDataLoadState.Loaded && _cachedMenuMusic.samples > 0)
                    {
                        return _cachedMenuMusic;
                    }
                }
                
                string file = Path.Combine(SoundsPath, "menu.ogg");
                AudioClip result = LoadAudio(file);
                
                _cachedMenuMusic = result;
                return result;
            }
        }

        // ReSharper disable InconsistentNaming
        [HarmonyPatch(typeof(NoteCutSoundEffectManager), "Start")]
        [HarmonyPrefix]
        public static void NoteCutPatch(NoteCutSoundEffectManager __instance,
            ref AudioClip[] ____longCutEffectsAudioClips, ref AudioClip[] ____shortCutEffectsAudioClips)
        {
            AudioClip[] hitSounds = HitSounds;

            ____shortCutEffectsAudioClips = hitSounds;
            ____longCutEffectsAudioClips = hitSounds;
        }

        [HarmonyPatch(typeof(NoteCutSoundEffect), "Awake")]
        [HarmonyPrefix]
        public static void MissPatch(ref AudioClip[] ____badCutSoundEffectAudioClips)
        {
            AudioClip[] missSounds = MissSounds;

            ____badCutSoundEffectAudioClips = missSounds;
        }

        [HarmonyPatch(typeof(BasicUIAudioManager), "Start")]
        [HarmonyPrefix]
        public static void ClickPatch(ref AudioClip[] ____clickSounds)
        {
            AudioClip[] clickSounds = ClickSounds;

            ____clickSounds = clickSounds;
        }

        [HarmonyPatch(typeof(SongPreviewPlayer), "Start")]
        [HarmonyPrefix]
        public static void MenuMusicPatch(ref AudioClip ____defaultAudioClip)
        {
            ____defaultAudioClip = MenuMusic;
        }
    }
}