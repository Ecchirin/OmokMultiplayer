using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetUserName : MonoBehaviour {

    [SerializeField]
    string inGameName = "IGN";
    [SerializeField]
    string serverServiceTagName = "ServerService";

    // Use this for initialization
    void Start () {
        //this.gameObject.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString(inGameName, "ERROR");
        this.gameObject.GetComponent<TextMeshProUGUI>().text = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().userName;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
