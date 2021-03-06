using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LitonStudio.Plugin.AssetBundleEditor
{
    /// <summary>
    /// 操作ab包的句柄
    /// </summary>
    public class AssetBundleHandle
    {
        private string _abPath;
        private AssetBundle _ab;
        private bool _avaliable;
        private AssetHandle[] _assets;
        public AssetHandle lastSelectedAssetHandle;
        public string AbPath
        {
            get { return _abPath; }
        }
        public AssetHandle[] AssetHandles
        {
            get
            {
                if (_assets == null)
                    _assets = _ab == null ? null : GetAssetHandles(this);
                return _assets;
            }
        }
        public string AbName
        {
            get
            {
                if (_ab == null) return string.Empty;
                return _ab.name;
            }
        }
        public bool Avaliable
        {
            get { return _avaliable; }
        }
        public AssetBundle assetBundle
        {
            get { return _ab; }
        }
        public AssetBundleHandle(AssetBundle ab, string path)
        {
            this._abPath = path;
            this._ab = ab;
            _avaliable = _ab != null;
        }

        /// <summary>
        /// 卸载ab包
        /// </summary>
        /// <param name="unloadReflactionResource"></param>
        public void Unload(bool unloadReflactionResource)
        {
            if (_ab == null) return;

            _avaliable = !unloadReflactionResource;

            if (!_avaliable) Debug.Log(string.Format("{0}已经被完全卸载", _ab.name));
            else Debug.Log(string.Format("{0}的压缩文件已经被卸载", _ab.name));

            foreach (AssetHandle assetHandle in _assets)
            {
                assetHandle.Unload(unloadReflactionResource);
            }
            _ab.Unload(unloadReflactionResource);

            if (unloadReflactionResource) _ab = null;
        }


        public void DrawAssetsGUI(Vector2 areaPos)
        {
            if (AssetHandles != null && AssetHandles.Length > 0)
            {
                for (int i = 0; i < _assets.Length; ++i)
                {
                    _assets[i].DrawAssetGUI(areaPos, i);
                }
            }
            else
                GUILayout.Label(new GUIContent("这个AssetBundle内没有资源"));
        }

        /// <summary>
        /// 为一个AB包里的所有资源生成资源操作句柄
        /// </summary>
        /// <param name="abHandle">AB包的操作句柄</param>
        /// <returns></returns>
        public static AssetHandle[] GetAssetHandles(AssetBundleHandle abHandle)
        {
            if (abHandle == null)
            {
                Debug.LogError("ab包不能为空");
                return null;
            }
            string[] names = abHandle.assetBundle.GetAllAssetNames();
            List<AssetHandle> handles = new List<AssetHandle>();
            foreach (string name in names)
            {
                handles.Add(new AssetHandle(abHandle, name));
            }
            return handles.ToArray();
        }

    }

    public enum AssetType
    {
        Unknow,
        Audio,
        Prefab,
        Txt,
        Bytes,
        Mat,
    }

}