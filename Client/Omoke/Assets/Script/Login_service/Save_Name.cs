using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is use to save a name
/// </summary>

public class Save_Name : MonoBehaviour {

    //Take in the display text and the input field
    [SerializeField]
    Text displayText = null;
    [SerializeField]
    Text inputFieldText = null;
    [SerializeField]
    Text connectionDisplay = null;
    [SerializeField]
    string playerPrefNameTag = "";
    [SerializeField]
    string serverServiceTagName = "ServerService";

    // Use this for initialization
    void Start () {
        //nameFieldGroup = this.transform.parent.gameObject;
        if (inputFieldText != null)
            inputFieldText.transform.parent.GetComponent<InputField>().text = PlayerPrefs.GetString(playerPrefNameTag, "Default001");

    }

    public void SaveName()
    {
        if (inputFieldText.text.Length == 0)
        {
            StartCoroutine(DisplayText("Input a name", 3));
            return;
        }
        StartCoroutine(DisplayText("Name saved as: " + inputFieldText.text, 3));
        PlayerPrefs.SetString(playerPrefNameTag, inputFieldText.text);

    }

    public IEnumerator DisplayText(string text, float time)
    {
        connectionDisplay.GetComponent<Text>().enabled = true;
        connectionDisplay.GetComponent<Text>().text = text;
        yield return new WaitForSeconds(time);
        connectionDisplay.GetComponent<Text>().text = "";
        connectionDisplay.GetComponent<Text>().enabled = false;
    }

    public void SendName()
    {
        if (GameObject.FindGameObjectWithTag(serverServiceTagName) == null)
            return;

        GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().SendName(playerPrefNameTag);
    }
}
