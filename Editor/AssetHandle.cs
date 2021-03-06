using UnityEditor;
using UnityEngine;

namespace LitonStudio.Plugin.AssetBundleEditor
{
    /// <summary>
    /// 资源对象的操作类
    /// </summary>
    public class AssetHandle
    {
        private AssetBundleHandle _abHandle;
        public Object _obj { get; private set; }
        public AssetType _type { get; private set; }
        private string _tail;
        private bool _avaliable;
        public bool Avaliable
        {
            get { return _avaliable; }
        }
        public string Suffix
        {
            get { return _tail; }
        }


        public AssetHandle(AssetBundleHandle abHandle, string assetName)
        {
            _abHandle = abHandle;
            _obj = abHandle.assetBundle.LoadAsset(assetName);
            _type = ParseStringToAssetType(assetName, out _tail);
            _avaliable = _obj != null;
        }

        public static AssetType ParseStringToAssetType(string name, out string fuffix)
        {
            if (name == null || name == string.Empty || name.IndexOf('.') == -1)
            {
                fuffix = string.Empty;
                return AssetType.Unknow;
            }
            string tail = name.Substring(name.IndexOf('.'));
            if (tail == null || tail == string.Empty)
            {
                fuffix = string.Empty;
                return AssetType.Unknow;
            }
            fuffix = tail;
            switch (tail)
            {
                case ".prefab":
                    return AssetType.Prefab;
                case ".wav":
                case ".mp3":
                case ".ogg":
                    return AssetType.Audio;
                case ".txt":
                    return AssetType.Txt;
                case ".bytes":
                    return AssetType.Bytes;
                case ".mat":
                    return AssetType.Mat;
                default:
                    return AssetType.Unknow;
            }
        }

        public void DrawAssetGUI(Vector2 areaPos, int index)
        {
            if (!_avaliable) return;
            float xPos = areaPos.x;
            float yPos = areaPos.y + index * 20f;
            Rect buttonPos = new Rect(xPos, yPos, 200f, 20f);
            GUI.Label(buttonPos, _obj.name, EditorStyles.toolbarButton);
            xPos += 200f;
            Rect typePos = new Rect(xPos, yPos, 100f, 20f);
            string typeString = _type == AssetType.Unknow ? _tail : _type.ToString();
            GUI.Label(typePos, typeString, EditorStyles.toolbarButton);
            xPos += 100f;
            Rect selectPos = new Rect(xPos, yPos, 85f, 20f);
            if (GUI.Button(selectPos, GetBuiltinIcon(_type, _abHandle.lastSelectedAssetHandle == this), EditorStyles.toolbarButton))
            {
                _abHandle.lastSelectedAssetHandle = this;
                DisplayAssetContent();
            }

            Rect operationPos = new Rect(xPos + 85f, yPos, 100f, 20f);
            if (GUI.Button(operationPos, "Create", EditorStyles.toolbarButton))
            {
                //_abHandle.lastSelectedAssetHandle = this;
                Object.Instantiate(this._obj);
            }
        }
        /// <summary>
        /// 展示资源的内容
        /// </summary>
        private void DisplayAssetContent()
        {
            switch (_type)
            {
                case AssetType.Audio:
                    AssetDisplayHelper.PlayAudio((AudioClip)_obj);
                    break;
                case AssetType.Prefab:
                    AssetDisplayHelper.ShowPrefab(_obj);
                    break;
                default:
                    Selection.activeObject = _obj;
                    //  EditorWindow.GetWindow<AssetBundleExplorer>().ShowNotification(new GUIContent("还没有进行展示处理"));
                    break;
            }
        }

        public void Unload(bool unload)
        {
            if (unload)
            {
                _obj = null;
                _type = AssetType.Unknow;
                _tail = null;
                _avaliable = false;
            }
        }

        public static GUIContent GetBuiltinIcon(AssetType type, bool isDisplaying)
        {
            switch (type)
            {
                case AssetType.Audio:
                    return isDisplaying ? EditorGUIUtility.IconContent("preAudioPlayOn") : EditorGUIUtility.IconContent("preAudioPlayOff");
                case AssetType.Bytes:
                    return EditorGUIUtility.IconContent("TextAsset Icon");
                case AssetType.Mat:
                    return EditorGUIUtility.IconContent("PreMatSphere");
                case AssetType.Prefab:
                    return EditorGUIUtility.IconContent("PrefabNormal Icon");
                case AssetType.Txt:
                    return EditorGUIUtility.IconContent("TextAsset Icon");
                default:
                    return EditorGUIUtility.IconContent("ScriptableObject Icon");
            }
        }
    }
}