//#define LOG_MODULE
//#define LOG_REDPOINT
//#define LOG_Fish
using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public static class GameUtil
{

    public static void SafeRun(Action act, Action<Exception> onError = null)
    {
        if (act == null) return;
        try
        {
            act();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (onError != null) onError(e);
        }
    }

    public static void SafeRun<T>(Action<T> act, T param, Action<Exception> onError = null)
    {
        if (act == null) return;
        try
        {
            act(param);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (onError != null) onError(e);
        }
    }

    public static T SafeRun<V, T>(V v, Func<V, T> func, Action<Exception> onError = null)
    {
        if (func == null) return default(T);
        try
        {
            return func(v);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (onError != null) onError(e);
            return default(T);
        }
    }

    public static void SafeRun<T, R>(Action<T, R> act, T t, R r)
    {
        if (act != null)
            act(t, r);
    }

}

public static class CollectionExtension
{

    public static void AddIfNotExist<T>(
        this List<T> dataSet
        , T t)
    {
        if (dataSet == null || t == null)
        {
            return;
        }

        if (dataSet.IndexOf(t) < 0)
        {
            dataSet.Add(t);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> dataset, Action<T> act)
    {
        if (dataset == null) return;
        foreach (var item in dataset)
        {
            GameUtil.SafeRun<T>(act, item);
        }
    }

    public static void ForEachI<T>(this IEnumerable<T> dataset, Action<T, int> act)
    {
        if (dataset == null) return;
        if (act == null) return;

        int i = 0;
        foreach (var data in dataset)
        {
            GameUtil.SafeRun<T, int>(act, data, i);
            i++;
        }
    }

    private static T Find<T>(this IEnumerable<T> dataset, Predicate<T> predicate, out int idx)
    {
        idx = -1;

        if (dataset != null && predicate != null)
        {
            int i = 0;
            foreach (var item in dataset)
            {
                if (predicate(item))
                {
                    idx = i;
                    return item;
                }
                ++i;
            }
        }

        return default(T);
    }

    public static int FindElementIdx<T>(this IEnumerable<T> dataset, Predicate<T> predicate)
    {
        int idx = -1;
        dataset.Find(predicate, out idx);
        return idx;
    }

    public static R Find<T, R>(this IEnumerable<T> dataset, Predicate<T> predicate, Func<T, R> action)
    {
        int idx = -1;
        var data = dataset.Find(predicate, out idx);
        return action(data);
    }

    public static T Find<T>(this IEnumerable<T> dataset, Predicate<T> predicate)
    {
        int idx = -1;
        return dataset.Find(predicate, out idx);
    }

    public static List<T> ToList<T>(this IEnumerable<T> dataset)
    {
        List<T> list = new List<T>();
        if (dataset != null)
            foreach (T item in dataset)
            {
                list.Add(item);
            }
        return list;
    }



    public static bool IsNullOrEmpty<T>(this List<T> dataSet)
    {
        return dataSet == null || dataSet.Count <= 0;
    }

    public static bool IsNullOrEmpty(this ArrayList array)
    {
        return array == null || array.Count <= 0;
    }

    public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        return dict == null || dict.Count <= 0;
    }
}

public static class GameObjectExt
{
    public static T GetMissingComponent<T>(this GameObject go) where T : Component
    {
        var t = go.GetComponent<T>();
        if (t == null)
            t = go.AddComponent<T>();

        return t;
    }
}




