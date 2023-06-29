using UnityEngine;

namespace Mirror.Examples.NetworkRoomExt
{
    public class PlayerScore : NetworkBehaviour
    {
        [SyncVar]
        public int index;

        [SyncVar]
        public uint score;

        [SyncVar]
        public bool isDeath;

        //[SyncVar]
        //public bool dogfall;

        public void AddScore()
        {
            score++;
        }
        //[Command]
        //public void IndexChange(int _,int newIndex)
        //{
        //    GetComponent<PlayerCursor>().ChangeCursorColor(newIndex);
        //}
        //void OnGUI()
        //{
        //    GUI.Box(new Rect(10f + (index * 110), 10f, 100f, 25f), $"P{index}: {score:0000000}");
        //}
    }
}
