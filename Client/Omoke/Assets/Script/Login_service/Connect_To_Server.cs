using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class manages the starting screen and calls to server
 */

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
    [SerializeField]
    GameObject other = null;

    // Use this for initialization
    void Start () {
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
        //if(inputFieldText.text.Length == 0)
        //{
        //    StartCoroutine(DisplayText("Input a Server Address", 3));
        //    return;
        //}
        //StartCoroutine(DisplayText("Connecting to " + inputFieldText.text, 5));
        GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().ConnecToServer(inputFieldText.text);
        //if (GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetStatus() == ConnectionStatus.CONNECTED)
        //{
        //    StartCoroutine(DisplayText("You are connected to: " + inputFieldText.text, 3));
        //}
        //if (GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetStatus() == ConnectionStatus.CONNECTING)
        //{
        //    StartCoroutine(DisplayText("Error Cannot Reach Server Try Again", 3));
        //}
    }

    //public IEnumerator DisplayText(string text, float time) {
    //    connectionDisplay.GetComponent<Text>().enabled = true;
    //    connectionDisplay.GetComponent<Text>().text = text;
    //    yield return new WaitForSeconds(time);
    //    connectionDisplay.GetComponent<Text>().text = "";
    //    connectionDisplay.GetComponent<Text>().enabled = false;
    //}
}
