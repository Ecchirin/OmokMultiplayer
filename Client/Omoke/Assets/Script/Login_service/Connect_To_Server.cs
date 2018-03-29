using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class manages the starting screen and calls to server
 */

//enum ConnectionStatus
//{
//    NOT_CONNECTING,
//    CONNECTING,
//    CONNECTED,
//};

public class Connect_To_Server : MonoBehaviour {

    //Take in the display text and the input field
    [SerializeField]
    Text displayText = null;
    [SerializeField]
    Text inputFieldText = null;
    [SerializeField]
    Text connectionDisplay = null;
    [SerializeField]
    string serverServiceTagName = "ServerService";

    //Current system status
    //ConnectionStatus newStatus = ConnectionStatus.NOT_CONNECTING;

    // Use this for initialization
    void Start () {
        //newStatus = ConnectionStatus.NOT_CONNECTING;
    }

    // This function is for the connection to the server
    public void ServerConnect()
    {
        if(inputFieldText.text.Length == 0)
        {
            StartCoroutine(DisplayText("Input a Server Address", 3));
            return;
        }
        StartCoroutine(DisplayText("Connecting to " + inputFieldText.text, 5));
        //newStatus = ConnectionStatus.CONNECTING;
        GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().ConnecToServer(inputFieldText.text);
    }

    public IEnumerator DisplayText(string text, float time) {
        connectionDisplay.GetComponent<Text>().enabled = true;
        connectionDisplay.GetComponent<Text>().text = text;
        yield return new WaitForSeconds(time);
        connectionDisplay.GetComponent<Text>().text = "";
        connectionDisplay.GetComponent<Text>().enabled = false;
        //if(newStatus == ConnectionStatus.CONNECTING)
        //{
        //    newStatus = ConnectionStatus.NOT_CONNECTING;
        //    StartCoroutine(DisplayText("Error Cannot Reach Server Try Again", 3));
        //}
    }
}
