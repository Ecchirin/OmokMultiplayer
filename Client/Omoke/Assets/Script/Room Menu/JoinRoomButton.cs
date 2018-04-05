using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomButton : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";
    private ServerConnection serverService = null;

    // Use this for initialization
    void Start () {
        serverService = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (serverService == null)
            Debug.Log("Error: Server Service cannot be found");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void JoinRoom()
    {
        if (serverService == null)
            return;
        serverService.JoinRoom(this.GetComponentInChildren<Text>().text);
    }
}
