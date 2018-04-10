using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetOpponentName : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";
    [SerializeField]
    GameObject opponentDropDownMenu = null;
    [SerializeField]
    TextMeshProUGUI opponentDisplayText = null;

    ServerConnection server = null;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
