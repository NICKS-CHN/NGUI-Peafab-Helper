using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 在引用UIButton组件下添加/修改UIButtonScale 为指定配置
/// </summary>
public class NGUICheckButtonScale : SinglePrefabBaseOperation
{
    private List<UIButton> _prefaButtonList;
    private Vector3 scrollViewPos;

    private float hoverScale = 1;
    private float pressScale = 0.9f;
    private float duration = 0.2f;

    public NGUICheckButtonScale()
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
            hoverScale = EditorGUILayout.FloatField("Hover Scale", hoverScale);
            pressScale = EditorGUILayout.FloatField("Press Scale", pressScale);
            duration = EditorGUILayout.FloatField("Duration", duration);

        }
        EditorGUILayout.EndVertical();

        if (_prefaButtonList==null|| _prefaButtonList.Count<=0)
        {
            EditorGUILayout.LabelField("暂无搜索到UIButton组件,无需添加ButtonScale");
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
                            ResetButtonScale(item);
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
        bool isActive = _prefaButtonList != null || _prefaButtonList.Count > 0;
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
            ResetButtonScale(uiButton);
        }
    }

    private void ResetButtonScale(UIButton uiButton)
    {
        var buttonScale = uiButton.gameObject.GetMissingComponent<UIButtonScale>();
        buttonScale.hover = new Vector3(hoverScale, hoverScale, hoverScale);
        buttonScale.pressed = new Vector3(pressScale, pressScale, pressScale);
        buttonScale.duration = duration;
    }

    private bool CheckNeedToChange(UIButton uiButton)
    {
        var buttonScale = uiButton.gameObject.GetComponent<UIButtonScale>();
        if (buttonScale == null)
            return false;

        return buttonScale.hover == new Vector3(hoverScale, hoverScale, hoverScale)
       && buttonScale.pressed == new Vector3(pressScale, pressScale, pressScale)
        && buttonScale.duration == duration;

    }
}
