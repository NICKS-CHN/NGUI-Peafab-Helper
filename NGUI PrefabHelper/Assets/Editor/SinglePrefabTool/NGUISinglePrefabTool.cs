using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using OperationEnum = SinglePrefabBaseOperation.OperationEnum;

public class NGUISinglePrefabTool : EditorWindow
{
    private static NGUISinglePrefabTool _instance;
    [MenuItem("NGUI/预设辅助工具  &#f")]
    private static void Open()
    {
        if (_instance == null)
        {
            _instance = GetWindow<NGUISinglePrefabTool>();
            _instance.minSize = new Vector2(500, 500);
            _instance.Show();
        }
        else
        {
            _instance.Close();
            _instance = null;
        }       
    }

    private Transform _selectPrefab;
    private Transform _UIPrefabRoot;

    private SinglePrefabBaseOperation _singlePrefabBaseOperation;

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            PrefabPanel();

            EditorGUILayout.Space();

            ShowOperationTypePanel();

            EditorGUILayout.Space();

            ShowOperation();
        }
        EditorGUILayout.EndVertical();
    }


    #region OperationType

    private OperationEnum curOperationEnum = OperationEnum.None;

    #endregion

    #region 预制体
    private void PrefabPanel()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("选择预制体", GUILayout.Width(100));

            _selectPrefab =
                EditorGUILayout.ObjectField(_selectPrefab, typeof (Transform), true, GUILayout.ExpandWidth(false),
                    GUILayout.Width(140)) as Transform;
            if (_selectPrefab != null)
            {
                if (_UIPrefabRoot != _selectPrefab)
                {
                    if (PrefabInstanceCheck(_selectPrefab))
                    {
                        _selectPrefab = PrefabUtility.FindPrefabRoot(_selectPrefab.gameObject).transform;
                        if (_UIPrefabRoot != _selectPrefab)
                        {
                            _UIPrefabRoot = _selectPrefab;
                            tOperationEnum = OperationEnum.AllOperation;
                            this.RemoveNotification();
                        }
                        else
                        {
                            this.ShowNotification(new GUIContent("这是同一个PrefabInstance"));
                        }
                    }
                    else
                    {
                        this.ShowNotification(new GUIContent("这不是一个PrefabInstance"));
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool PrefabInstanceCheck(Object target)
    {
        PrefabType type = PrefabUtility.GetPrefabType(target);

        return type == PrefabType.PrefabInstance;
    }

    #endregion

    #region 功能选取
    OperationEnum tOperationEnum = OperationEnum.None;
    private void ShowOperationTypePanel()
    {
        bool isDisable = _UIPrefabRoot == null;
        EditorGUI.BeginDisabledGroup(isDisable);
        {
            EditorGUILayout.BeginHorizontal();
            {
                tOperationEnum = (OperationEnum)EditorGUILayout.EnumPopup("功能选择", tOperationEnum);
                if (curOperationEnum != tOperationEnum)
                {
                    curOperationEnum = tOperationEnum;
                    InitOperation();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Label(_singlePrefabBaseOperation != null ? "功能描述："+_singlePrefabBaseOperation.GetOperationDesc(curOperationEnum) : "请选择操作类型");
    }

    private void InitOperation()
    {
        _singlePrefabBaseOperation = SinglePrefabBaseOperation.CreateInstance(curOperationEnum, _UIPrefabRoot.gameObject);
    }


    #endregion

    #region 功能区域

    private void ShowOperation()
    {
        if (curOperationEnum == OperationEnum.None|| _singlePrefabBaseOperation == null)
            return;

        _singlePrefabBaseOperation.ShowOperationPanel();
        _singlePrefabBaseOperation.ExecuteOperation();
    }

    #endregion


}
