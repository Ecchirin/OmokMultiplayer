﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentReadyOrNot : MonoBehaviour {
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

    public void SetReady(bool ready)
    {
        serverService.RoomSetReady(ready);
    }
}