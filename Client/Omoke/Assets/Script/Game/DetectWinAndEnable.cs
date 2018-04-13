using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is just to activate the win screen and after it will disable itself to prevent recalls.
/// </summary>

public class DetectWinAndEnable : MonoBehaviour {

    [SerializeField]
    GameObject[] gameobjectToEnable = null;
    [SerializeField]
    string serverServiceTagName = "ServerService";

    ServerConnection server = null;

    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (server == null)
            Debug.LogError("There is no server found in UpdateBoard.cs object name: " + this.gameObject.name);
    }
	
	// Update is called once per frame
	void Update () {
        if (server == null || server.GetWinner() == 0)
            return;
        foreach(GameObject obj in gameobjectToEnable)
        {
            obj.SetActive(true);
        }
        this.enabled = false;
    }
}
