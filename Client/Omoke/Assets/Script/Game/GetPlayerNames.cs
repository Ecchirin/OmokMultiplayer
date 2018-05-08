using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetPlayerNames : MonoBehaviour {
    [SerializeField]
    string serverServiceTagName = "ServerService";
    [SerializeField]
    bool playerOne = false;
    [SerializeField]
    bool playerTwo = false;
    // Use this for initialization
    void Start () {
        if(playerOne)
            this.GetComponent<TextMeshProUGUI>().text = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetPlayerOne();
        if (playerTwo)
            this.GetComponent<TextMeshProUGUI>().text = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetPlayerTwo();
        this.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
