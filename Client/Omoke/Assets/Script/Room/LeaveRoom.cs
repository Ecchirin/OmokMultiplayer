using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoom : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";

    public void LeaveCurrentRoom()
    {
        GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().LeaveTheRoom();
    }
}
