using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LitonStudio.Plugin.AssetBundleEditor
{
    public class AssetBundleExplorer : EditorWindow
    {

        /// <summary>
        /// 已经加载的ab包
        /// </summary>
        private static Dictionary<string, AssetBundleHandle> _loadedAbDict = new Dictionary<string, AssetBundleHandle>();
        private static AssetBundleHandle _selectedHandle;
        public static float windowWidth = 600f;
        public static float windowHeight = 500f;


        private Vector2 _contentScrollPos;
        private Vector2 _abBarScrollPos;

        private Rect _assetBundleBarRect;
        private Rect _assetBundleBarCotentRect;
        private Rect _assetBarRect;
        private Rect _assetBarContentRect;

        public string abPath;

#if UNITY_2017

    [MenuItem("LitonTool/UnloadAllAssetBundle")]
    static void UnloadAllLoadedAseetBundle()
    {
        foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
            ab.Unload(true);
    }

#endif
        [MenuItem("LitonTool/OpenAbExplorer")]
        static void OpenWindow()
        {
            AssetBundleExplorer window = GetWindow<AssetBundleExplorer>("Ab Explorer");
            // GetWindowWithRect<AssetBundleExplorer>(new Rect(Screen.width * 0.2f, Screen.height * 0.2f, windowWidth,windowHeight), true, "Ab Explorer");
            window.Show();
            window.InitSetting();
        }





        private void InitSetting()
        {
            _assetBundleBarRect = _assetBundleBarCotentRect = new Rect(0f, 40f, windowWidth / 3f, windowHeight);
            _assetBarRect = _assetBarContentRect = new Rect(windowWidth / 3, 40f, windowWidth, windowHeight);

            _abBarScrollPos = Vector2.zero;
            _contentScrollPos = Vector2.zero;
        }

        /// <summary>
        /// 绘制界面
        /// </summary>
        private void OnGUI()
        {
            DrawToolbar();
            DrawBarHead();
            LeftBarGUI();
            ContentGUI();
        }

        /// <summary>
        /// 左边ab包栏
        /// </summary>
        private void LeftBarGUI()
        {
            _abBarScrollPos = GUI.BeginScrollView(_assetBundleBarRect, _abBarScrollPos, _assetBundleBarCotentRect, false, false);
            GUI.Box(_assetBundleBarCotentRect, new GUIContent(""));
            GUILayout.Space(5f);
            DrawLeftBarContent();
            GUI.EndScrollView();
        }

        /// <summary>
        /// 右边ab包内容列表栏
        /// </summary>
        private void ContentGUI()
        {
            _contentScrollPos = GUI.BeginScrollView(_assetBarRect, _contentScrollPos, _assetBarContentRect, false, false);
            DrawContentGUI();
            GUI.EndScrollView();
        }
        private void ContentBarDescript()
        {
            float xPos = 200f;
            float yPos = 40f;
            Rect buttonPos = new Rect(xPos, yPos, 200f, 20f);
            GUI.Label(buttonPos, "Asset Name", EditorStyles.toolbarButton);
            xPos += 200f;
            Rect typePos = new Rect(xPos, yPos, 100f, 20f);
            GUI.Label(typePos, "Asset Type", EditorStyles.toolbarButton);
            xPos += 100f;
            Rect operationPos = new Rect(xPos, yPos, 100f, 20f);
            GUI.Label(operationPos, "Operation", EditorStyles.toolbarButton);

        }

        /// <summary>
        /// 工具栏
        /// </summary>
        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("OpenAB", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                if (OpenAb()) LoadAb(abPath);
            }
            if (GUILayout.Button("UnloadAB", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                UnloadSelectedAb(true);
            }
#if UNITY_2017
        if (GUILayout.Button("UnloadAllAB", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            UnLoadAllAB();
        }


        if (GUILayout.Button("TestAsyncLoad", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            if (OpenAb())
                LoadAsyncAb();
        }
#endif

            GUILayout.Space(1000f);

            GUILayout.EndHorizontal();
        }

        private static void DrawBarHead()
        {


            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label(new GUIContent("AssetBundle列表"), EditorStyles.toolbarButton, GUILayout.Width(179f));
            GUILayout.Space(15f);
            //     string name = string.Format("{0} 包内资源列表", (_selectedHandle != null ? _selectedHandle.AbName : "AB"));
            //      GUILayout.Label(new GUIContent(name), EditorStyles.toolbarButton, GUILayout.Width(400f));
            GUILayout.Label("Asset Name", EditorStyles.toolbarButton, GUILayout.Width(200f));
            GUILayout.Label("Asset Type", EditorStyles.toolbarButton, GUILayout.Width(100f));
            GUILayout.Label("Operation", EditorStyles.toolbarButton, GUILayout.Width(85f));
            GUILayout.Space(200f);
            GUILayout.EndHorizontal();
        }
        private void DrawLeftBarContent()
        {

            if (_loadedAbDict.Count <= 0) return;

            foreach (AssetBundleHandle abHandle in _loadedAbDict.Values)
            {
                if (!abHandle.Avaliable) continue;
                bool selected = _selectedHandle == abHandle;

                if (GUILayout.Toggle(selected, new GUIContent(abHandle.AbName), EditorStyles.toolbarButton, GUILayout.Width(200f)))
                {
                    _selectedHandle = abHandle;
                    RecalculateScrollViewAreaSize();
                }
            }
        }
        private void DrawContentGUI()
        {
            if (_selectedHandle == null) return;
            _selectedHandle.DrawAssetsGUI(new Vector2(200f, 40f));

        }

        private void RecalculateScrollViewAreaSize()
        {
            float contentHeight = 200f;
            if (_selectedHandle != null)
                contentHeight = _selectedHandle.AssetHandles.Length * 20f;
            float leftBarHeight = _loadedAbDict.Count * 20f;

            _assetBarContentRect.height = Mathf.Max(contentHeight, _assetBundleBarCotentRect.height);
            _assetBundleBarCotentRect.height = Mathf.Max(_assetBundleBarCotentRect.height, leftBarHeight);
        }
        /// <summary>
        /// 加载指定路径的ab包
        /// </summary>
        /// <param name="abPath">ab包文件路径</param>
        private void LoadAb(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                ShowNotification(new GUIContent("文件路径错误"));
                return;
            }
            LoadAbAtPath(path);
        }
        private bool OpenAb()
        {
            string lastOpenPath = EditorPrefs.GetString("AbExplorerOpenPath");
            if (string.IsNullOrEmpty(lastOpenPath))
                lastOpenPath = Application.dataPath;

            string path = EditorUtility.OpenFilePanel("选择AB包", lastOpenPath, "*");
            if (path == null || path == string.Empty)
                return false;

            abPath = path;
            return true;
        }
#if UNITY_2017
    private void LoadAsyncAb()
    {
        if (abPath == null || abPath == string.Empty)
        {
            ShowNotification(new GUIContent("文件路径错误"));
            return;
        }
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(abPath);
        request.completed += OnCompleted;
    }
#endif

        private void OnCompleted(AsyncOperation ao)
        {
            AssetBundleCreateRequest request = ao as AssetBundleCreateRequest;
            if (request.assetBundle == null) return;
            _selectedHandle = AddToAbDict(abPath, request.assetBundle);
            RecalculateScrollViewAreaSize();
        }
        private void LoadAbAtPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (_loadedAbDict.ContainsKey(path))
            {
                this.ShowNotification(new GUIContent("ab包已在列表里"));
                _selectedHandle = _loadedAbDict[path];
                return;
            }
            try
            {
                AssetBundle ab = AssetBundle.LoadFromFile(path);
                if (ab == null) return;
                _selectedHandle = AddToAbDict(path, ab);
            }
            catch (System.Exception e)
            {
                ShowNotification(new GUIContent("文件不是Unity AssetBundle格式"));
                Debug.LogErrorFormat("{0}\n{1}", e.GetType().FullName, e.StackTrace);
            }
        }

        private static void UseAssetBundle(AssetBundle ab)
        {
            string[] assetNames = ab.GetAllAssetNames();

            foreach (string name in assetNames)
            {
                Debug.Log(name);
                if (name.IndexOf(".prefab") != -1)
                {
                    Instantiate(ab.LoadAsset(name));
                }
            }
        }

        /// <summary>
        /// 添加到ab包字典里
        /// </summary>
        /// <param name="abPath">ab包加载路径</param>
        /// <param name="ab">加载后的ab包</param>
        private AssetBundleHandle AddToAbDict(string abPath, AssetBundle ab)
        {
            if (_loadedAbDict.ContainsKey(abPath))
            {
                ShowNotification(new GUIContent("{0} 包已经在字典里了"));
                return _loadedAbDict[abPath];
            }
            AssetBundleHandle handle = new AssetBundleHandle(ab, abPath);
            _loadedAbDict.Add(abPath, handle);
            Debug.LogFormat("{0}添加进AB字典", ab.name);
            return handle;
        }

        private void UnloadSelectedAb(bool unload)
        {
            if (_selectedHandle == null || !_selectedHandle.Avaliable) return;

            _loadedAbDict.Remove(_selectedHandle.AbPath);

            _selectedHandle.Unload(unload);

            _selectedHandle = null;

            RecalculateScrollViewAreaSize();
        }

#if UNITY_2017
    private void UnLoadAllAB()
    {
        foreach (AssetBundleHandle handle in _loadedAbDict.Values)
        {
            if (handle.Avaliable)
                handle.Unload(true);
        }
        _loadedAbDict.Clear();

        foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
            ab.Unload(true);
        _selectedHandle = null;

        RecalculateScrollViewAreaSize();

    }
#endif
    }



}