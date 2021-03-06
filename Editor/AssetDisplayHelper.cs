using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LitonStudio.Plugin.AssetBundleEditor
{
    /// <summary>
    /// 资源内容展示工具
    /// </summary>
    public class AssetDisplayHelper
    {
        public static AudioSource _audio;
        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="clip">要播放的音频</param>
        public static void PlayAudio(AudioClip clip)
        {

            if (_audio == null)
            {
                _audio = Object.FindObjectOfType<AudioSource>();
                if (_audio == null)
                    _audio = new GameObject("AudioPlayer").AddComponent<AudioSource>();
            }
            _audio.Stop();
            _audio.PlayOneShot(clip);
        }

        /// <summary>
        /// 实例化一个Prefab
        /// </summary>
        /// <param name="prefab">要实例化的对象</param>
        public static void ShowPrefab(Object prefab)
        {
            Object obj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            UnityEditor.Selection.activeGameObject = obj as GameObject;
            //让Scene窗口的相机聚焦在该物体上
            UnityEditor.SceneView.lastActiveSceneView.LookAt(((GameObject)obj).transform.position);
            //这是设置相机焦距
            UnityEditor.SceneView.lastActiveSceneView.size = 1f;
        }
    }
}