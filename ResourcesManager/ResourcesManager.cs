using System.Collections.Generic;
using UnityEngine;

internal class ResourcesManager
{
    
    /// <summary>
    /// 这里改AB包的时候用AssetBundleManifest.GetAllDependencies()查询依赖关系，再加载依次加载ab包
    /// </summary>
    Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>(); 
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T Load<T>(string path) where T:Object
    {
        //AssetBundleManifest.GetAllDependencies()
        return Resources.Load<T>(path);
    }
}

