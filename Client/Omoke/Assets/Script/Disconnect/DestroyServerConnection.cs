using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyServerConnection : MonoBehaviour {
    [SerializeField]
    string serverServiceTagName = "ServerService";

    ServerConnection server = null;

    // Use this for initialization
    void Start() {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (server == null)
        {
            Debug.LogError("Server cannot be found by: " + this.name);
            this.enabled = false;
        }
    }

    public void DestroyServer()
    {
        server.ServerDestroy();
    }
}
