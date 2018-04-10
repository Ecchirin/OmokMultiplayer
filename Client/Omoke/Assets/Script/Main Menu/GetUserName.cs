using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetUserName : MonoBehaviour {

    [SerializeField]
    string inGameName = "IGN";

    // Use this for initialization
    void Start () {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString(inGameName, "ERROR");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
