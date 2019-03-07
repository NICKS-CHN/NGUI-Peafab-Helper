using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class SinglePrefabBaseOperation : EditorWindow
{
    #region OperationEnum 功能增减需改动部分

    /// <summary>
    /// 功能枚举
    /// </summary>
    public enum OperationEnum
    {
        None,
        UIButtonRefreshColor,
        UILabelResetDepth,
        UIAddButtonScale,
        CopyScaleReset,
        AllOperation
    }
    protected static Dictionary<OperationEnum, SinglePrefabBaseOperation> OperationDic;

    /// <summary>
    /// 增加功能时需要增加实例
    /// </summary>
    private static void InitOperationInstance()
    {
        OperationDic = new Dictionary<OperationEnum, SinglePrefabBaseOperation>()
        { {OperationEnum.UIAddButtonScale, new NGUICheckButtonScale()},
          {OperationEnum.UIButtonRefreshColor, new NGUIResetButtonColor()},
          {OperationEnum.UILabelResetDepth, new NGUIResetLabelDepthColor()},
          {OperationEnum.CopyScaleReset, new NGUICopyScaleReset()},
        };
    }

    /// <summary>
    /// 得到功能描述
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetOperationDesc(OperationEnum type)
    {
        switch (type)
        {
            case OperationEnum.UIButtonRefreshColor:
                return "重置UIButton点击为指定颜色 (默认为白色)";
            case OperationEnum.UILabelResetDepth:
                return "重置UILabel为指定层级 (默认Depth：10)";
            case OperationEnum.UIAddButtonScale:
                return "在UIButton组件下添加/修改UIButtonScale 为指定配置数据 (默认Press：0.9f)";
            case OperationEnum.CopyScaleReset:
                return "恢复复制操作引起的缩放变化（scale：1.0008）";
            case OperationEnum.AllOperation:
                return "执行所有操作：\n" + GetOperationDesc();
            case OperationEnum.None:
            default:
                return "请选择操作类型";
        }
    }
    private string GetOperationDesc()
    {
        string desc = string.Empty;
        int index = 1;
        foreach (var item in OperationDic)
        {
            desc += index + ":" + GetOperationDesc(item.Key) + "\n";
            index++;
        }

        return desc;
    }

    #endregion

    //目标对象
    protected static GameObject _TargetGameObject;

    //返回功能实例
    public static SinglePrefabBaseOperation CreateInstance(OperationEnum type, GameObject go)
    {
        InitOperationInstance();
        _TargetGameObject = go;
        //这个特殊处理
        if (type == OperationEnum.AllOperation)
            return new NGUIExecuteAllOperation();

        return OperationDic[type];
    }

    #region interface

    /// <summary>
    /// 数据初始化
    /// </summary>
    public virtual void InitData()
    {
    }


    /// <summary>
    /// 功能界面
    /// </summary>
    public virtual void ShowOperationPanel()
    {

    }


    /// <summary>
    /// 执行界面
    /// </summary>
    public virtual void ExecuteOperation()
    {

    }
    /// <summary>
    /// 功能一键设置
    /// </summary>
    public virtual void ExecuteSetting()
    {

    }

    protected virtual void OnDispose()
    {

    }

    #endregion

    #region Common method
    /// <summary>
    ///应用预设 
    /// </summary>
    protected void AppltPrefab()
    {
        EditorHelper.SavePrefab(_TargetGameObject);
    }

    #endregion
}
