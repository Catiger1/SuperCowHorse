
using Assets.Scripts.Common;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameManager
{
    public class NetworkPrefabPoolManager:MonoSingleton<NetworkPrefabPoolManager>
    {
        public Dictionary<string, List<GameObject>> cache = new Dictionary<string, List<GameObject>>();
        public Dictionary<int, GameObject> readyForFireCache = new Dictionary<int, GameObject>();
        public override void Init()
        {
            base.Init();
            DontDestroyOnLoad(gameObject);
        }

        [ServerCallback]
        public GameObject CreateObject(string key, GameObject prefab, Vector3 pos, Quaternion rotate,GameObject owner)
        {
            GameObject go = FindUsableObject(key);
            if (go == null)
                go = AddObject(key, prefab);
            
            go.GetComponent<Bullet>().Owner = owner;
            UseObject(go, pos, rotate);
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

        private void UseObject(GameObject go, Vector3 pos, Quaternion rotate)
        {
            go.transform.position = pos;
            go.transform.rotation = rotate;
            go.transform.localScale = new Vector3(3, 3, 1);
            go.GetComponent<NetworkPoolObject>().EnableState = true;
            //执行组件下组件挂载的各种复位方式
            foreach (var prefab in go.GetComponents<IResetable>())
            {
                prefab.OnReset();
            }
        }

        /// <summary>
        /// 查找可用的组件
        /// </summary>
        private GameObject FindUsableObject(string key)
        {
            if (cache.ContainsKey(key))
                return cache[key].Find(s => !s.GetComponent<NetworkPoolObject>().EnableState);
            return null;
        }

        public List<GameObject> FindUsingObject(string key)
        {
            if (cache.ContainsKey(key))
                return cache[key].FindAll(s => s.GetComponent<NetworkPoolObject>().EnableState);
            return null;
        }
        public GameObject AddObject(string key, GameObject prefab)
        {
            GameObject go = Instantiate(prefab);
            if (!cache.ContainsKey(key))
                cache.Add(key, new List<GameObject>());
            NetworkServer.Spawn(go);
            cache[key].Add(go);
            return go;
        }
        public void Clear(string key)
        {
            for (int i = cache[key].Count - 1; i >= 0; i--)
            {
                NetworkServer.Destroy(cache[key][i]);
            }

            cache.Remove(key);
        }
        //清空所有
        [ServerCallback]
        public void ClearAll()
        {
            //将字典中所有键存入集合中再进行清空操作
            foreach (var key in new List<string>(cache.Keys))
            {
                Clear(key);
            }
        }
    }
}
