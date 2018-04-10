using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

/*
 * This class manages the starting screen and calls to server
 */

public class Connect_To_Server : MonoBehaviour {

    //Take in the display text and the input field
    [SerializeField]
    TMP_InputField iPInputFieldText = null;
    [SerializeField]
    TMP_InputField portNumberInputField = null;
    [SerializeField]
    string serverServiceTagName = "ServerService";
    [SerializeField]
    GameObject other = null;

    // Use this for initialization
    void Start () {
        if (iPInputFieldText != null)
            iPInputFieldText.text = PlayerPrefs.GetString("ServerIP", "127.0.0.1");
        if (portNumberInputField != null)
            portNumberInputField.text = PlayerPrefs.GetString("ServerPort", "7777");
    }

    private void FixedUpdate()
    {
        if(GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetStatus() == ConnectionStatus.CONNECTED)
        {
            other.SetActive(true);
            this.transform.parent.gameObject.SetActive(false);
        }
    }

    // This function is for the connection to the server
    public void ServerConnect()
    {
        //int portNumber = 7777;
        //int.TryParse(portNumberInputField.text, out portNumber);
        GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().ConnecToServer(iPInputFieldText.text, Convert.ToInt32(portNumberInputField.text, 10));
    }
}
