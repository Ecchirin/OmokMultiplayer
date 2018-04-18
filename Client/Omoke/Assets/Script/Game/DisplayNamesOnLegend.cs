using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayNamesOnLegend : MonoBehaviour {

    private TextMeshProUGUI thisText = null;
    [SerializeField]
    string serverServiceTagName = "ServerService";
    ServerConnection server = null;
    // Use this for initialization
    void Start () {
        thisText = this.GetComponent<TextMeshProUGUI>();
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (thisText == null || server == null)
        {
            Debug.LogError("Cannot find GUITMPRO: " + this.name);
            this.enabled = false;
        }

        //If it have continue means there is a server
        if (server.isSpectator)
        {
            thisText.text = server.GetPlayerOne() + " tile\n" + server.GetPlayerTwo() + " tile";
            this.enabled = false;
        }
        else
            this.enabled = false;
	}
}
