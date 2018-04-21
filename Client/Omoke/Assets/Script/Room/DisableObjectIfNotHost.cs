using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectIfNotHost : MonoBehaviour {

    [SerializeField]
    GameObject[] thingsToDisable = null;

    [SerializeField]
    string serverServiceTagName = "ServerService";
    ServerConnection server = null;

    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (server == null)
        {
            Debug.LogError("There is no server found in object name: " + this.gameObject.name);
            this.enabled = false;
        }

        //Check if the user is host
        if(!server.isHost)
        {
            foreach(GameObject obj in thingsToDisable)
            {
                obj.SetActive(false); 
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
