using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 将预设中的UILabel Depth设为指定值
/// </summary>
public class NGUIResetLabelDepthColor : SinglePrefabBaseOperation
{
    private List<UILabel> _preafaLabelList;
    private Vector3 scrollViewPos;
    private int _depth = 10;
    public NGUIResetLabelDepthColor()
    {
        InitData();
    }

    public override void InitData()
    {
        if (_TargetGameObject == null)
            return;

        _preafaLabelList = PrefabSpriteManager.GetTargetComponents<UILabel>(_TargetGameObject.transform, _preafaLabelList);
    }

    public override void ShowOperationPanel()
    {
        if (_preafaLabelList.IsNullOrEmpty())
        {
            EditorGUILayout.LabelField("暂无搜索到UIButton组件");
            return;
        }

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("指定Depth：");
            _depth = EditorGUILayout.IntField(_depth, GUILayout.Height(20));
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(200));
        {
            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos, GUILayout.Height(200));
            {
                int index = 0;
                foreach (var item in _preafaLabelList)
                {
                    ++index;
                    EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                    {
                        string desc = item.gameObject.name;
                        EditorGUI.BeginDisabledGroup(CheckNeedToChange(item));

                        if (GUILayout.Button("重置", GUILayout.Width(50)))
                        {
                            ResetLabelDepth(item);
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

    protected override void OnDispose()
    {
        base.OnDispose();
        _preafaLabelList = null;
    }

    public override void ExecuteOperation()
    {
        bool isActive = !_preafaLabelList.IsNullOrEmpty();
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
        if (_preafaLabelList == null)
            return;
        foreach (var uiButton in _preafaLabelList)
        {
            ResetLabelDepth(uiButton);
        }
    }

    private void ResetLabelDepth(UILabel uiLabel)
    {
        uiLabel.depth = _depth;
    }

    private bool CheckNeedToChange(UILabel uiLabel)
    {
        return uiLabel.depth == _depth;
    }

}
