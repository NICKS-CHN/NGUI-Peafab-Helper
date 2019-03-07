using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 将预设中的UIButton的颜色置白色
/// </summary>
public class NGUIResetButtonColor : SinglePrefabBaseOperation
{
    private List<UIButton> _prefaButtonList;
    private Vector3 scrollViewPos;

    private Color _defaultColor = Color.white;
    private Color _hoverColor = Color.white;
    private Color _pressColor = Color.white;
    private Color _disableColor = Color.white;



    public NGUIResetButtonColor()
    {
        InitData();
    }

    public override void InitData()
    {
        if (_TargetGameObject == null)
            return;

        _prefaButtonList = PrefabSpriteManager.GetTargetComponents<UIButton>(_TargetGameObject.transform, _prefaButtonList);
    }

    public override void ShowOperationPanel()
    {

        EditorGUILayout.BeginVertical();
        {
            _defaultColor = EditorGUILayout.ColorField("Default Color", _defaultColor);
            _hoverColor = EditorGUILayout.ColorField("Hover Color", _hoverColor);
            _pressColor = EditorGUILayout.ColorField("Press Color", _pressColor);
            _disableColor = EditorGUILayout.ColorField("Disable Color", _disableColor);
        }
        EditorGUILayout.EndVertical();

        if (_prefaButtonList.IsNullOrEmpty())
        {
            EditorGUILayout.LabelField("暂无搜索到UIButton组件");
            return;
        }
        EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(200));
        {
            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos, GUILayout.Height(200));
            {
                int index = 0;
                foreach (var item in _prefaButtonList)
                {
                    ++index;
                    EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                    {
                        string desc = item.gameObject.name;
                        EditorGUI.BeginDisabledGroup(CheckNeedToChange(item));

                        if (GUILayout.Button("重置", GUILayout.Width(50)))
                        {
                            ResetButtonColor(item);
                            EditorHelper.SelectObject(item);
                        }
                        EditorGUI.EndDisabledGroup();
                        if (GUILayout.Button(desc, "OL TextField"))
                        {
                            EditorHelper.SelectObject(item);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                }
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }

    public override void ExecuteOperation()
    {
        bool isActive = !_prefaButtonList.IsNullOrEmpty();
        EditorGUI.BeginDisabledGroup(!isActive);
        if (GUILayout.Button("一键设置", GUILayout.Height(50f)))
        {
            ExecuteSetting();
        }
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Apply", GUILayout.Height(50f)))
        {
            AppltPrefab();
        }
    }

    public override void ExecuteSetting()
    {
        if (_prefaButtonList == null)
            return;
        foreach (var uiButton in _prefaButtonList)
        {
            ResetButtonColor(uiButton);
        }
    }

    private void ResetButtonColor(UIButton uiButton)
    {
        uiButton.defaultColor = _defaultColor;
        uiButton.hover = _hoverColor;
        uiButton.pressed = _pressColor;
        uiButton.disabledColor = _disableColor;
    }

    private bool CheckNeedToChange(UIButton uiButton)
    {
        return uiButton.defaultColor == _defaultColor &&
        uiButton.hover == _hoverColor &&
        uiButton.pressed == _pressColor &&
        uiButton.disabledColor == _disableColor;
    }
}
