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

        public void Death()
        {
            if (isLocalPlayer)
            {
                CameraController cameraController = Camera.main.GetComponent<CameraController>();
                cameraController.ChangeLookAtTarget(null);
                cameraController.SetInitial();
            }
            isDeath = true;
        }

        public void AddScore()
        {
            score++;
        }
    }
}
