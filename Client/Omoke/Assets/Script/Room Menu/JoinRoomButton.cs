using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinRoomButton : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";
    private ServerConnection serverService = null;

    public bool isSpectator = false;

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

        if(!isSpectator)
            serverService.JoinRoom(this.GetComponentInChildren<TextMeshProUGUI>().text);
        else
            serverService.JoinRoomSpectator(this.GetComponentInChildren<TextMeshProUGUI>().text);

    }
}
