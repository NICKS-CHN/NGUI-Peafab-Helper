using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NGUICopyScaleReset : SinglePrefabBaseOperation
{

    private List<Transform> _preafaTransformList;
    private Vector3 scrollViewPos;
    private int _depth = 10;
    public NGUICopyScaleReset()
    {
        InitData();
    }

    public override void InitData()
    {
        if (_TargetGameObject == null)
            return;

        _preafaTransformList = _TargetGameObject.transform.GetComponentsInChildren<Transform>().ToList();

    }

    public override void ShowOperationPanel()
    {
        //if (_preafaTransformList.IsNullOrEmpty())
        //{
        //    EditorGUILayout.LabelField("暂无搜索到Transform组件");
        //    return;
        //}

        //GUILayout.BeginHorizontal();
        //{
        //    GUILayout.Label("指定Depth：");
        //    _depth = EditorGUILayout.IntField(_depth, GUILayout.Height(20));
        //}
        //EditorGUILayout.EndHorizontal();


        //EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(200));
        //{
        //    scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos, GUILayout.Height(200));
        //    {
        //        int index = 0;
        //        foreach (var item in _preafaLabelList)
        //        {
        //            ++index;
        //            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
        //            {
        //                string desc = item.gameObject.name;
        //                EditorGUI.BeginDisabledGroup(CheckNeedToChange(item));

        //                if (GUILayout.Button("重置", GUILayout.Width(50)))
        //                {
        //                    ResetLabelDepth(item);
        //                    EditorHelper.SelectObject(item);
        //                }
        //                EditorGUI.EndDisabledGroup();
        //                if (GUILayout.Button(desc, "OL TextField"))
        //                {
        //                    EditorHelper.SelectObject(item);
        //                }
        //            }
        //            EditorGUILayout.EndHorizontal();

        //        }
        //    }
        //    EditorGUILayout.EndScrollView();
        //}
        //EditorGUILayout.EndVertical();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        _preafaTransformList = null;
    }

    public override void ExecuteOperation()
    {
        bool isActive = !_preafaTransformList.IsNullOrEmpty();
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
        if (_preafaTransformList == null)
            return;
        foreach (var transform in _preafaTransformList)
        {
            RetCopyScale(transform);
        }
    }

    private void RetCopyScale(Transform transform)
    {
        if (IsCopyScale(transform))
            transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private bool IsCopyScale(Transform transform)
    {
        if (transform.localScale == new Vector3(1f, 1f, 1f)) //0.9999999识别不出....
        {
            return true;
        }

        if (transform.localScale.x > 1.0f && transform.localScale.x < 1.01f)
            return true;
        if (transform.localScale.y > 1.0f && transform.localScale.y < 1.01f)
            return true;
        if (transform.localScale.z > 1.0f && transform.localScale.z < 1.01f)
            return true;

        if (transform.localScale.x < 1.0f && transform.localScale.x > 0.8f)
            return true;
        if (transform.localScale.y < 1.0f && transform.localScale.y > 0.8f)
            return true;
        if (transform.localScale.z < 1.0f && transform.localScale.z > 0.8f)
            return true;

        return false;

    }

}
