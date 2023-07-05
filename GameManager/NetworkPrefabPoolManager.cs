
using Assets.Scripts.Common;
using Mirror;
using Mirror.Examples.NetworkRoomExt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.GameManager
{
    public class NetworkPrefabPoolManager:MonoSingleton<NetworkPrefabPoolManager>
    {
        //public Dictionary<string, List<GameObject>> cache = new Dictionary<string, List<GameObject>>();
        public Dictionary<int, List<GameObject>> readyForFireCache = new Dictionary<int, List<GameObject>>();
        public override void Init()
        {
            base.Init();
            DontDestroyOnLoad(gameObject);
        }

        [ServerCallback]
        public GameObject CreateObject(int index, GameObject prefab,Vector3 pos, Quaternion rotate,GameObject owner)
        {
            GameObject go = FindUsableObject(index);

            if (go == null)
                go = AddObject(index, prefab, pos, rotate);
            else
                go.transform.rotation = rotate;
            
            go.GetComponent<Bullet>().Owner = owner;
            go.GetComponent<Bullet>().FirePos = owner.FindChildByName("FirePos");
            UseObject(go);
            return go;
        }
        [ServerCallback]
        public GameObject CreateObjectAndHide(int index, GameObject prefab, Vector3 pos, Quaternion rotate, GameObject owner)
        {
            GameObject go = AddObject(index, prefab, pos, rotate);
            go.GetComponent<Bullet>().Owner = owner;
            go.GetComponent<Bullet>().FirePos = owner.FindChildByName("FirePos");
            go.transform.localScale = Vector3.zero;
            return go;
        }
        /// <summary>
        /// 回收对象池对象
        /// </summary>
        /// <param name="prefab">预制件</param>
        /// <param name="delay">延时</param>
        [ServerCallback]
        public void CollectObject(GameObject prefab,Vector3 hidePos ,float delay = 0)
        {
            //延时回收一个脚本只能回收一个
            //写两个函数是因为协程是下一帧执行，没法做到及时回收
            prefab.transform.localScale = Vector3.zero;
            if (delay == 0)
                MyCollectObject(prefab, hidePos);
            else
                StartCoroutine(MyCollectObject(prefab, delay, hidePos));
        }
        /// <summary>
        /// MyCollectObject用于回收对象池创建的对象
        /// </summary>
        /// <param name="prefab"></param>
        private void MyCollectObject(GameObject prefab, Vector3 hidePos)
        {
            if ((prefab != null))//&& (prefab.activeInHierarchy))
            {
                prefab.transform.position = hidePos;
                prefab.GetComponent<NetworkPoolObject>().EnableState = false;
            }
        }

        private IEnumerator MyCollectObject(GameObject prefab, float delay, Vector3 hidePos)
        {
            yield return new WaitForSeconds(delay);
            if ((prefab != null) && (prefab.activeInHierarchy))
                prefab.transform.position = hidePos;
        }

        private void UseObject(GameObject go)
        {
            //go.transform.position = pos;
            //go.transform.rotation = rotate;
            go.transform.localScale = new Vector3(3, 3, 1);
            go.GetComponent<NetworkPoolObject>().EnableState = true;
            //执行组件下组件挂载的各种复位方式
            foreach (var prefab in go.GetComponents<IResetable>())
            {
                prefab.OnReset();
            }
        }
        [ServerCallback]
        public void ReloadPrefab(int index,GameObject owner)
        {
            if (!readyForFireCache.ContainsKey(index))
                readyForFireCache[index] = new List<GameObject>();

            GameObject findGo = readyForFireCache[index].Find(s => !s.GetComponent<NetworkPoolObject>().EnableState);
            if (findGo != null)
            {
                readyForFireCache[index].Add(findGo);
                findGo.GetComponent<Bullet>().Owner = owner;
                findGo.GetComponent<Bullet>().FirePos = owner.FindChildByName("FirePos");
                findGo.transform.localScale = Vector3.zero;
            }
        }
        /// <summary>
        /// 查找可用的组件
        /// </summary>
        private GameObject FindUsableObject(int index)
        {
            if (readyForFireCache.ContainsKey(index))
            {
                GameObject findGo = readyForFireCache[index].Find(s => !s.GetComponent<NetworkPoolObject>().EnableState);
                if (findGo != null)
                    return findGo;
            }

            return null;
        }

        public GameObject FindUsingObject(int index)
        {
            if (readyForFireCache.ContainsKey(index))
            {
                GameObject findGo = readyForFireCache[index].Find(s => s.GetComponent<NetworkPoolObject>().EnableState);
                if (findGo != null)
                    return findGo;
            }
            return null;
        }
        //public GameObject AddObject(int index, GameObject prefab)
        //{
        //    GameObject go = Instantiate(prefab);
        //    if (!readyForFireCache.ContainsKey(index))
        //        readyForFireCache.Add(index, new List<GameObject>());
        //    NetworkServer.Spawn(go);
        //    readyForFireCache[index].Add(go);
        //    return go;
        //}
        public GameObject AddObject(int index, GameObject prefab,Vector3 pos,Quaternion rotation)
        {
            GameObject go = Instantiate(prefab,pos, rotation);
            if (!readyForFireCache.ContainsKey(index))
                readyForFireCache.Add(index, new List<GameObject>());
            NetworkServer.Spawn(go);
            readyForFireCache[index].Add(go);
            return go;
        }
        public void Clear(int key)
        {
            for (int i = readyForFireCache[key].Count - 1; i >= 0; i--)
            {
                NetworkServer.Destroy(readyForFireCache[key][i]);
            }

            readyForFireCache.Remove(key);
        }
        //清空所有
        [ServerCallback]
        public void ClearAll()
        {
            //将字典中所有键存入集合中再进行清空操作
            foreach (var key in new List<int>(readyForFireCache.Keys))
            {
                Clear(key);
            }
        }
    }
}
