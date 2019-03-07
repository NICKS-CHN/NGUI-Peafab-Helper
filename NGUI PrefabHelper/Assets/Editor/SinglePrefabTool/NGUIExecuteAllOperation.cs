using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

public class NGUIExecuteAllOperation : SinglePrefabBaseOperation
{
      
    public NGUIExecuteAllOperation()
    {
    }

    public override void InitData()
    {
        if (_TargetGameObject == null)
            return;

    }

    public override void ShowOperationPanel()
    {

    }

    public override void ExecuteOperation()
    {
        if (GUILayout.Button("一键设置", GUILayout.Height(50f)))
        {
            ExecuteSetting();
        }

        if (GUILayout.Button("Apply", GUILayout.Height(50f)))
        {
            AppltPrefab();
        }
    }

    public override void ExecuteSetting()
    {
        foreach (var item in OperationDic)
        {
            Debug.Log("执行:"+GetOperationDesc(item.Key));
            item.Value.InitData();
            item.Value.ExecuteSetting();
        }
        if (_TargetGameObject.gameObject.transform.localPosition != Vector3.zero)
        {
            _TargetGameObject.gameObject.transform.localPosition = Vector3.zero;
            Debug.Log("已重置目标预制体Position为Vector3.zero");
        }
    }
}
