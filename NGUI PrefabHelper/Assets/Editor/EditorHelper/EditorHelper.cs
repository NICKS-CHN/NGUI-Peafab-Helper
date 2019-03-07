using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Debug = UnityEngine.Debug;

public static class EditorHelper
{
    #region GUILayout Helper
    public static void DrawBoxLabel(string title, string text, bool hasCopyBtn, bool needLayout = true)
    {
        if (needLayout) EditorGUILayout.BeginHorizontal();

        GUILayout.Box(title);
        GUILayout.Space(5f);
        GUILayout.Box(text);
        if (hasCopyBtn && GUILayout.Button("Copy", GUILayout.Width(60f)))
        {
            EditorGUIUtility.systemCopyBuffer = text;
        }

        if (needLayout) EditorGUILayout.EndHorizontal();
    }

    static public bool DrawHeader(string text) { return DrawHeader(text, text, false, false); }

    static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = EditorPrefs.GetBool(key, true);

        GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        text = "<b><size=11>" + text + "</size></b>";
        if (state) text = "\u25BC " + text;
        else text = "\u25BA " + text;
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    static public string DrawTextField(string title, string value, int width = 50, bool needLayout = true)
    {
        if (needLayout) EditorGUILayout.BeginHorizontal();

        GUILayout.Box(title);
        //EditorGUILayout.LabelField (title, GUILayout.MaxWidth(50));
        string newValue = EditorGUILayout.TextField("", value, GUILayout.MaxWidth(width), GUILayout.Height(20));

        if (needLayout) EditorGUILayout.EndHorizontal();
        return newValue;
    }

    static public int DrawIntField(string title, int value, int width = 30, bool needLayout = true)
    {
        if (needLayout) EditorGUILayout.BeginHorizontal();

        GUILayout.Box(title);
        int newValue = EditorGUILayout.IntField("", value, GUILayout.MaxWidth(width), GUILayout.Height(20));

        if (needLayout) EditorGUILayout.EndHorizontal();
        return newValue;
    }

    static public float DrawFloatField(string title, float value, int width = 30, bool needLayout = true)
    {
        if (needLayout) EditorGUILayout.BeginHorizontal();

        GUILayout.Box(title);
        float newValue = EditorGUILayout.FloatField("", value, GUILayout.MaxWidth(width), GUILayout.Height(20));

        if (needLayout) EditorGUILayout.EndHorizontal();

        return newValue;
    }

    static public bool DrawToggle(string title, bool value, int width = 30, bool needLayout = true)
    {
        if (needLayout) EditorGUILayout.BeginHorizontal();

        GUILayout.Box(title);
        bool newValue = EditorGUILayout.Toggle("", value, GUILayout.MaxWidth(width), GUILayout.Height(20));

        if (needLayout) EditorGUILayout.EndHorizontal();
        return newValue;
    }
    #endregion

    /// <summary>
    /// Creates the scriptable object asset.
    /// </summary>
    public static void CreateScriptableObjectAsset(ScriptableObject asset, string path, string fileName)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        AssetDatabase.CreateAsset(asset, path + fileName + ".asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Selection.activeObject = asset;
    }

    /// <summary>
    /// Loads the scriptable object asset.
    /// </summary>
    public static T LoadScriptableObjectAsset<T>(string fileName) where T : ScriptableObject
    {
        return AssetDatabase.LoadAssetAtPath(fileName + ".asset", typeof(T)) as T;
    }

    /// <summary>
    /// 获取指定目录下， 所有指定扩展名的文件名
    /// </summary>
    /// <param name="path"></param>
    /// <param name="extensions"></param>
    /// <returns></returns>
    public static List<string> GetAssetsList(string path, params string[] extensions)
    {
        if (!Directory.Exists(path))
        {
            Debug.Log("Directory  : " + path + " is not Exist");
            return null;
        }

        List<string> nameList = new List<string>();
        DirectoryInfo info = new DirectoryInfo(path);
        ListFiles(info, nameList, "Assets", true, extensions);
        return nameList;
    }

    public static List<string> GetAssetsList(string path, string beignString, bool needContainExtension, params string[] extensions)
    {
        if (!Directory.Exists(path))
        {
            Debug.Log("Directory  : " + path + " is not Exist");
            return null;
        }

        List<string> nameList = new List<string>();
        DirectoryInfo info = new DirectoryInfo(path);
        ListFiles(info, nameList, beignString, needContainExtension, extensions);
        return nameList;
    }


    private static void ListFiles(FileSystemInfo info, List<string> nameList, string beignString, bool needContainExtension, params string[] extensions)
    {
        if (!info.Exists) return;

        DirectoryInfo dir = info as DirectoryInfo;
        if (dir == null) return;

        FileSystemInfo[] files = dir.GetFileSystemInfos();
        if (files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                //is file
                if (files[i] is FileInfo)
                {
                    FileInfo file = files[i] as FileInfo;

                    string[] fileNames = file.FullName.Split('.');
                    string fileNameExtension = fileNames[fileNames.Length - 1];
                    fileNameExtension = fileNameExtension.ToLower();

                    foreach (string ext in extensions)
                    {
                        if (fileNameExtension == ext)
                        {
                            string fileReturnName = string.Empty;

                            if (needContainExtension)
                            {
                                fileReturnName = file.FullName;
                            }
                            else
                            {
                                int index = file.FullName.LastIndexOf(".");
                                if (index != -1)
                                {
                                    fileReturnName = file.FullName.Substring(0, index);
                                }
                                else
                                {
                                    fileReturnName = file.FullName;
                                }
                            }

                            fileReturnName = fileReturnName.Substring(fileReturnName.IndexOf(beignString));
                            fileReturnName = fileReturnName.Replace("\\", "/");

                            nameList.Add(fileReturnName);

                            break;
                        }
                    }
                }
                // is Directory
                else
                {
                    ListFiles(files[i], nameList, beignString, needContainExtension, extensions);
                }
            }
        }
    }

    public static void Space(int num)
    {
        for (int i = 0; i < num; i++)
        {
            EditorGUILayout.Space();
        }
    }

    public static void LocalToObjbect(UnityEngine.Object obj)
    {
        EditorGUIUtility.PingObject(obj);
    }

    public static bool ShowToggle(string str, bool flag, int stringWith, Color color)
    {
        GUI.color = color;
        EditorGUILayout.LabelField(str, GUILayout.Width(stringWith));
        bool returnFlag = EditorGUILayout.Toggle(flag, GUILayout.Width(40));

        GUI.color = Color.white;
        return returnFlag;
    }

    /// <summary>
    /// 分割线
    /// </summary>
    public static void PartitionLine()
    {
        EditorGUILayout.LabelField("========================================");
        Space(2);
    }

    public static void RepeatString(string str, int num, GUIStyle style = null)
    {
        for (int i = 0; i < num; i++)
        {
            if (style != null)
            {
                EditorGUILayout.LabelField(str, style);
            }
            else
            {
                EditorGUILayout.LabelField(str);
            }
        }
    }

    public static void LabelAndButton(string label, string button, Action buttonAction = null, GUIStyle style = null)
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (style != null)
            {
                EditorGUILayout.LabelField(label, style, GUILayout.Width(400));
            }
            else
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(400));
            }

            if (GUILayout.Button(button, GUILayout.Width(200)))
            {
                if (buttonAction != null) buttonAction();
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    /// <summary>
    /// 显示数组
    /// </summary>
    /// <param name="labelArray"></param>
    public static void LabelArray(string[] labelArray, Color bColor, Color eColor)
    {
        if (labelArray == null) return;

        GUI.color = bColor;
        foreach (string str in labelArray)
        {
            EditorGUILayout.LabelField(str, GUILayout.Width(400));
        }
        Space(1);
        GUI.color = eColor;
    }

    public static void LabelArray(string[] labelArray, Color bColor, Color eColor, GUIStyle style)
    {
        if (labelArray == null) return;

        GUI.color = bColor;
        foreach (string str in labelArray)
        {
            EditorGUILayout.LabelField(str, style, GUILayout.Width(400));
        }
        Space(1);
        GUI.color = eColor;
    }

    public static void ShowLabelList(List<string> list, ref Vector2 pos, int with, int height)
    {
        pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Width(with), GUILayout.Height(height));
        {
            if (list != null)
            {
                foreach (string str in list)
                {
                    EditorGUILayout.LabelField(str, GUILayout.Width((int)(with * 0.8)));
                }
            }
        }
        EditorGUILayout.EndScrollView();
        Space(1);
    }



    public delegate void CallBackLabelFunc<T>(T arg);
    public static void ShowList<T>(List<T> list, CallBackLabelFunc<T> fun, ref Vector2 pos, int with, int height)
    {
        pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Width(with), GUILayout.Height(height));
        {
            if (list != null)
            {
                foreach (T arg in list)
                {
                    fun(arg);
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    public delegate void CallBackDicLabelFunc<T1, T2>(T1 arg, T2 arg2);
    public static void ShowList<T1, T2>(Dictionary<T1, T2> dic, CallBackDicLabelFunc<T1, T2> fun, ref Vector2 pos, int with, int height)
    {
        pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Width(with), GUILayout.Height(height));
        {
            if (dic != null)
            {
                foreach (KeyValuePair<T1, T2> item in dic)
                {
                    fun(item.Key, item.Value);
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    #region 提示的辅助
    public enum Result
    {
        Success,
        Error,
    }

    public static void DisplayResultDialog(Result result = Result.Success,
        string msg = "完成！")
    {
        EditorUtility.DisplayDialog(result.ToString(), msg, "确定");
    }

    public static void Run(Action action, bool askConfirm = true, bool displayResultDialog = true)
    {
        try
        {
            if (askConfirm)
            {
                if (!EditorUtility.DisplayDialog("确认：", "谨慎操作啊亲！",
                    "确认", "取消"))
                {
                    return;
                }
            }
            action();
            if (displayResultDialog)
            {
                DisplayResultDialog();
            }
        }
        catch (Exception e)
        {
            DisplayResultDialog(Result.Error, e.Message);
            throw e;
        }

        AssetDatabase.Refresh();
    }
    #endregion

    public static byte[] HttpRequest(string url, int timeOut = 3000)
    {
        var request = HttpWebRequest.Create(url);
        request.Timeout = timeOut;

        var response = request.GetResponse();
        var datas = new byte[response.ContentLength];
        var value = 0;
        using (var sResp = response.GetResponseStream())
        {
            const int bufSize = 2048;
            byte[] srcbuff = new byte[bufSize];
            int size = sResp.Read(srcbuff, 0, bufSize);
            while (size > 0)
            {
                Buffer.BlockCopy(srcbuff, 0, datas, value, size);
                value += size;
                size = sResp.Read(srcbuff, 0, bufSize);
            }
        }

        return datas;
    }


    /// <summary>
    /// 获取所有的根节点，仅在Editor模式下能用
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<GameObject> SceneRoots()
    {
        var prop = new HierarchyProperty(HierarchyType.GameObjects);
        var expanded = new int[0];
        while (prop.Next(expanded))
        {
            yield return prop.pptrValue as GameObject;
        }
    }


    public static void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));
        var type = assembly.GetType("UnityEditorInternal.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }


    public static Vector2 GetTextureSize(TextureImporter importer)
    {
        object[] args = new object[2] { 0, 0 };
        var method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(importer, args);

        return new Vector2((int)args[0], (int)args[1]);
    }


    public static bool IsTextureMultipleOfFour(TextureImporter importer)
    {
        var size = GetTextureSize(importer);
        int result1;
        Math.DivRem((int)size[0], 4, out result1);
        int result2;
        Math.DivRem((int)size[1], 4, out result2);

        return result1 == 0 && result2 == 0;
    }

    public static void SelectObject(UnityEngine.Object obj)
    {
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
    }


    public static void RevealInFinder(string path)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = path.Replace("/", "\\");
            Process.Start("explorer.exe", "/select," + path);
        }
        else
        {
            EditorUtility.RevealInFinder(path);
        }
    }


    public static void ShowNotificationStr(this EditorWindow pEditorWindow, string pMessage)
    {
        Debug.Log("ShowNotification:" + pMessage);
        pEditorWindow.ShowNotification(new GUIContent(pMessage));
    }


    /// <summary>
    /// 保存预制体 相当于Apply
    /// </summary>
    /// <param name="go"></param>
    public static void SavePrefab(GameObject go)
    {
        GameObject obj = GetPrefabInstanceParent(go);
        UnityEngine.Object prefabAsset = null;
        if (obj != null)
        {
            prefabAsset = PrefabUtility.GetPrefabParent(obj);
            if (prefabAsset != null)
            {
                PrefabUtility.ReplacePrefab(obj, prefabAsset, ReplacePrefabOptions.ConnectToPrefab);
            }
        }
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 找到此预制体的父节点
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static GameObject GetPrefabInstanceParent(GameObject go)
    {
        if (go == null)
        {
            return null;
        }
        PrefabType pType = EditorUtility.GetPrefabType(go);
        if (pType != PrefabType.PrefabInstance)
        {
            Debug.Log(go.name + "is not a PrefabInstance, please check it.");
            return null;
        }
        if (go.transform.parent == null)
        {
            return go;
        }
        pType = EditorUtility.GetPrefabType(go.transform.parent.gameObject);
        if (pType != PrefabType.PrefabInstance)
        {
            return go;
        }
        return GetPrefabInstanceParent(go.transform.parent.gameObject);
    }
}
