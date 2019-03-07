using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class PrefabSpriteManager
{

    /// <summary>
    /// 获取制定对象下所有指定组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="trsnsform"></param>
    /// <param name="ComponentList"></param>
    /// <returns></returns>
    public static List<T> GetTargetComponents<T>(Transform trsnsform, List<T> ComponentList)
        where T : MonoBehaviour
    {
        T component;
        int childCount = trsnsform.childCount;
        if(ComponentList == null)
            ComponentList = new List<T>();
        if (childCount == 0)
        {
            component = trsnsform.GetComponent<T>();
            if (component != null && !ComponentList.Contains(component))
                ComponentList.Add(component);
        }

        Transform target;
        for (int i = 0; i < childCount; i++)
        {
            target = trsnsform.GetChild(i);
            ComponentList = GetTargetComponents(target, ComponentList);
            component = target.GetComponent<T>();
            if (component != null && !ComponentList.Contains(component))
                ComponentList.Add(component);
        }

        return ComponentList;
    } 



}
