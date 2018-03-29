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
        PlayerPrefs.SetString(playerPrefNameTag, inputFieldText.text);
        if (GameObject.FindGameObjectWithTag(serverServiceTagName) == null)
            return;

        GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().SendName(playerPrefNameTag);
    }

    //public void SendName()
    //{
    //    if (GameObject.FindGameObjectWithTag(serverServiceTagName) == null)
    //        return;

    //    GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().SendName(playerPrefNameTag);
    //}
}
