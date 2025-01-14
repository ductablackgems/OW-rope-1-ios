using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BG_Library.Audio.Editor
{
    public class SingleAudioSOEdt
    {
        [MenuItem("Assets/Create/AudioSO/SingleAudio %#&E", false, 1)]
        private static void CreateSO()
        {
            Object[] target = Selection.objects;
            if (target.Length <= 0) return;

            foreach (var a in target)
            {
                if (a == null || a.GetType() != typeof(AudioClip)) // && target.GetType() != typeof(SpriteAtlas)))
                {
                    Debug.LogWarning("An audio clip must first be selected in order to create an Audio Asset.");
                    continue;
                }

                string filePathWithName = AssetDatabase.GetAssetPath(a);
                string fileNameWithExtension = Path.GetFileName(filePathWithName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);
                string filePath = filePathWithName.Replace(fileNameWithExtension, "");
                PlayAudioChannelSO audioSingleChanel = ScriptableObject.CreateInstance<PlayAudioChannelSO>();
                AssetDatabase.CreateAsset(audioSingleChanel, filePath + fileNameWithoutExtension + ".asset");

                AudioClip clip = a as AudioClip;
                audioSingleChanel.Clip = clip;
                audioSingleChanel.Confs = new[] { new AudioPlayer.AudCommonConf() };
                EditorUtility.SetDirty(audioSingleChanel);
                AssetDatabase.SaveAssets();

                AssetDatabase.ImportAsset(
                    AssetDatabase.GetAssetPath(audioSingleChanel));
            }
        }
        
        
        [MenuItem("Assets/Create/AudioSO/ListChanelAud", false, 1)]
        private static void CreateChanelListSO()
        {
            Object[] target = Selection.objects;
            if (target.Length <= 0) return;

            string filePathWithName = AssetDatabase.GetAssetPath(target[0]);
            string fileNameWithExtension = Path.GetFileName(filePathWithName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);
            string filePath = filePathWithName.Replace(fileNameWithExtension, "");
            PlayListAudioChannelSO audioListChanel = ScriptableObject.CreateInstance<PlayListAudioChannelSO>();
            AssetDatabase.CreateAsset(audioListChanel, filePath + fileNameWithoutExtension + ".asset");
            List<AudioClip> tempClips = new List<AudioClip>();
            foreach (var a in target)
            {
                if (a == null || a.GetType() != typeof(AudioClip)) // && target.GetType() != typeof(SpriteAtlas)))
                {
                    Debug.LogWarning("An audio clip must first be selected in order to create an Audio Asset.");
                    continue;
                }
                AudioClip clip = a as AudioClip;
                tempClips.Add(clip);
            }

            audioListChanel.Clip = tempClips.ToArray();
            EditorUtility.SetDirty(audioListChanel);
            AssetDatabase.SaveAssets();

            AssetDatabase.ImportAsset(
                AssetDatabase.GetAssetPath(audioListChanel));
        }
    }
}