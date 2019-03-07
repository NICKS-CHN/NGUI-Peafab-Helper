using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEditor.SceneManagement;

namespace BaseClassNS
{
    public abstract class BaseEditorWindow : EditorWindow
    {
        #region GUIStyle
        public const int NormalFontSize = 20;

        private static GUIStyle _leftButtonStyle;

        public static GUIStyle LeftButtonStyle
        {
            get
            {
                if (_leftButtonStyle == null)
                {
                    _leftButtonStyle = new GUIStyle(GUI.skin.button);
                    _leftButtonStyle.alignment = TextAnchor.MiddleLeft;
                }
                return _leftButtonStyle;
            }
        }

        private static GUIStyle _normalButtonStyle;

        public static GUIStyle NormalButtonStyle
        {
            get
            {
                if (_normalButtonStyle == null)
                {
                    _normalButtonStyle = new GUIStyle(GUI.skin.button);
                    _normalButtonStyle.fontSize = NormalFontSize;
                }
                return _normalButtonStyle;
            }
        }

        private static GUIStyle _searchTextField;

        public static GUIStyle SearchTextField
        {
            get
            {
                if (_searchTextField == null)
                {
                    _searchTextField = new GUIStyle("SearchTextField");
                }
                return _searchTextField;
            }
        }

        private static GUIStyle _redLabelStyle;
        public static GUIStyle RedLabelStyle
        {
            get
            {
                if (_redLabelStyle == null)
                {
                    _redLabelStyle = new GUIStyle(GUI.skin.label);
                    _redLabelStyle.normal.textColor = Color.red;
                }
                return _redLabelStyle;
            }
        }

        private static GUIStyle _yellowLabelStyle;
        public static GUIStyle YellowLabelStyle
        {
            get
            {
                if (_yellowLabelStyle == null)
                {
                    _yellowLabelStyle = new GUIStyle(GUI.skin.label);
                    _yellowLabelStyle.normal.textColor = Color.yellow;
                }
                return _yellowLabelStyle;
            }
        }

        #endregion

        protected Vector2 _scrollPos;

        protected static T Open<T>() where T : BaseEditorWindow
        {
            return GetWindow<T>(typeof(T).Name.Replace("EditorWindow", ""));
        }


        protected virtual void OnGUI()
        {
            StartOnGUI();
            Space();
            CustomOnGUI();
            Space();
            EndOnGUI();
        }


        protected virtual void StartOnGUI()
        {
            _scrollPos = BeginScrollView(_scrollPos);
        }

        protected abstract void CustomOnGUI();

        protected virtual void EndOnGUI()
        {
            EditorGUILayout.EndScrollView();
        }


        #region 常用封装
        protected virtual void Space()
        {
            EditorGUILayout.Space();
        }

        protected virtual void DrawLine()
        {
            LabelField("", GUI.skin.horizontalSlider);
        }

        protected virtual void FlexibleSpace()
        {
            GUILayout.FlexibleSpace();
        }


        protected virtual Rect BeginVertical(params GUILayoutOption[] options)
        {
            return EditorGUILayout.BeginVertical(options);
        }

        protected virtual void EndVertical()
        {
            EditorGUILayout.EndVertical();
        }

        protected virtual Rect BeginHorizontal(params GUILayoutOption[] options)
        {
            return EditorGUILayout.BeginHorizontal(options);
        }

        protected virtual void EndHorizontal()
        {
            EditorGUILayout.EndHorizontal();
        }


        protected virtual Vector2 BeginScrollView(Vector2 pos, params GUILayoutOption[] options)
        {
            return EditorGUILayout.BeginScrollView(pos, options);
        }

        protected virtual void EndScrollView()
        {
            EditorGUILayout.EndScrollView();
        }


        protected virtual void TitleField(string title, string label = null)
        {
            EditorGUILayout.LabelField(title, label);
        }


        protected virtual void LabelField(string label, GUIStyle style = null)
        {
            style = style ?? GUI.skin.label;
            EditorGUILayout.LabelField(label, style);
        }

        protected virtual string TextField(string label, string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            style = style ?? GUI.skin.textField;

            return EditorGUILayout.TextField(label, text, style, options);
        }

        protected virtual int IntSlider(string title, int value, int lValue, int rValue)
        {
            return EditorGUILayout.IntSlider(title, value, lValue, rValue);
        }


        public const int LimitedButtonClickTime = 100;
        protected long _lastButtonClickTime;

        protected virtual void Button(string text, Action callback = null, GUIStyle style = null)
        {
            style = style ?? NormalButtonStyle;
            if (GUILayout.Button(text, style))
            {
                if (DateTime.UtcNow.ToFileTimeUtc() - _lastButtonClickTime < LimitedButtonClickTime)
                {
                    return;
                }

                if (callback != null)
                {
                    callback();
                }

                _lastButtonClickTime = DateTime.UtcNow.ToFileTimeUtc();
            }
        }


        protected virtual T EnumPopup<T>(string title, Enum selectedEnum, GUIStyle style = null)
        {
            style = style ?? EditorStyles.popup;
            return (T)Convert.ChangeType(EditorGUILayout.EnumPopup(title, selectedEnum, style), typeof(T));
        }


        protected virtual Object ObjectField(string title, Object obj, Type type = null, bool allowSceneObjects = false)
        {
            return EditorGUILayout.ObjectField(title, obj, type, allowSceneObjects);
        }

        protected virtual bool Toggle(string label, bool value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            style = style ?? GUI.skin.toggle;
            return EditorGUILayout.Toggle(label, value, style, options);
        }

        protected virtual bool ToggleLeft(string label, bool value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            style = style ?? GUI.skin.toggle;
            return EditorGUILayout.ToggleLeft(label, value, style, options);
        }

        #endregion

        #region 滑动界面专用
        protected Dictionary<string, Vector2> _tempScrollPosDict = new Dictionary<string, Vector2>();

        protected virtual void BeginTempScrollView(string name, params GUILayoutOption[] options)
        {
            if (!_tempScrollPosDict.ContainsKey(name))
            {
                _tempScrollPosDict[name] = Vector2.zero;
            }

            _tempScrollPosDict[name] = BeginScrollView(_tempScrollPosDict[name], options);
        }

        protected virtual void EndTempScrollView()
        {
            EndScrollView();
        }
        #endregion

        #region 一些运行前的判断
        protected virtual bool IsAppropriateRun(string label = null)
        {
            var flag = !EditorApplication.isPlayingOrWillChangePlaymode &&
                   !EditorApplication.isCompiling &&
                   !EditorApplication.isUpdating;

            if (!flag)
            {
                label = label ?? "当前不适宜运行！";
                EditorUtility.DisplayDialog("错误：", label, "确定");
            }
            return flag;
        }

        protected virtual bool IsCurrentSceneSave(string label = null)
        {
            var flag = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (!flag)
            {
                label = label ?? "当前操作必须保存场景！";
                EditorUtility.DisplayDialog("错误：", label, "确定");
            }
            return flag;
        }
        #endregion
    }
}
