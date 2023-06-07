using UnityEngine;

namespace Mirror.Examples.NetworkRoom
{
    internal class Spawner
    {
        [ServerCallback]
        internal static void InitialSpawn()
        {
            for (int i = 0; i < 0; i++)
                SpawnReward();
        }
        //[ServerCallback]
        //internal static void SpawnCursor(NetworkRoomManager manager)
        //{
        //    if (manager.netCursorList.Count > 0) return;
        //    GameObject go = ResourcesManager.Load<GameObject>("Prefab/Cursor");
        //    for(int i=0;i<4;i++)
        //    {
        //        GameObject newGo = GameObject.Instantiate(go);
        //        if(i>2)
        //            newGo.GetComponent<SpriteRenderer>().color = Color.red;
        //        else if(i>1)
        //            newGo.GetComponent<SpriteRenderer>().color = Color.blue;
        //        else if(i>0)
        //            newGo.GetComponent<SpriteRenderer>().color = Color.gray;

        //        manager.netCursorList.Add(newGo);
        //        newGo.SetActive(false);
        //    }
        //}
        [ServerCallback]
        internal static void SpawnReward()
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-19, 20), 1, Random.Range(-19, 20));
            NetworkServer.Spawn(Object.Instantiate(NetworkRoomManagerExt.singleton.rewardPrefab, spawnPosition, Quaternion.identity));
        }
    }
}
