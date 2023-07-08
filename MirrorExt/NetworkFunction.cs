using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class NetworkFunction : NetworkBehaviour
{
    [ClientRpc]
    public void AddForce(GameObject go, Vector2 dir)
    {
        go.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
    }

    [Command(requiresAuthority = false)]
    public void Destroy(GameObject go)
    {
        NetworkServer.Destroy(go);
    }
}
