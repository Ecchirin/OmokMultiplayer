using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateRenju : MonoBehaviour {

    [SerializeField]
    GameObject activateSign = null;

    [SerializeField]
    string serverServiceTagName = "ServerService";
    private ServerConnection server = null;

    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if(server == null)
        {
            Debug.LogError("This object cannot find server: " + this.name);
            this.enabled = false;
        }
        activateSign.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        if (!server.isHost)
            this.GetComponent<Button>().interactable = false;
        else
            this.GetComponent<Button>().interactable = true;
        //Get raiju rules
        if (server.GetRenjuRules())
            activateSign.SetActive(true);
        else
            activateSign.SetActive(false);
	}

    public void ToggleRaijuRules ()
    {
        if(server.isHost)
        {
            server.SetRenjuRules(!server.GetRenjuRules());
        }
    }
}
