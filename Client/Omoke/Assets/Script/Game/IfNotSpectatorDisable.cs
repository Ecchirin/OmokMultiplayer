using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfNotSpectatorDisable : MonoBehaviour {

    [SerializeField]
    GameObject[] thingsToDisable = null;
    [SerializeField]
    string serverServiceTagName = "ServerService";
    // Use this for initialization
    void Start () {
		if(!GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().isSpectator)
        {
            foreach(GameObject go in thingsToDisable)
            {
                go.SetActive(false);
            }
        }
        this.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
