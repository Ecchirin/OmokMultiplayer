using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class manages the starting screen and calls to server
 */

enum ConnectionStatus
{
    NOT_CONNECTING,
    CONNECTING,
    CONNECTED,
};

public class Connect_To_Server : MonoBehaviour {

    //Take in the display text and the input field
    [SerializeField]
    Text displayText = null;
    [SerializeField]
    Text inputFieldText = null;
    [SerializeField]
    Text connectionDisplay = null;

    //Current system status
    ConnectionStatus newStatus = ConnectionStatus.NOT_CONNECTING;

	// Use this for initialization
	void Start () {
        newStatus = ConnectionStatus.NOT_CONNECTING;
    }
	
	// Update is called once per frame
	//void Update () {
		
	//}

    // This function is for the connection to the server
    public void ServerConnect()
    {
        StartCoroutine(DisplayText("TESTING", 3));
    }

    public IEnumerator DisplayText(string text, float time) {
        connectionDisplay.GetComponent<Text>().enabled = true;
        connectionDisplay.GetComponent<Text>().text = text;
        yield return new WaitForSeconds(time);
        connectionDisplay.GetComponent<Text>().text = "";
        connectionDisplay.GetComponent<Text>().enabled = false;
    }
}
