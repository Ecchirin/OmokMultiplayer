using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script is use to save a name
/// </summary>

public class Save_Name : MonoBehaviour {

    //Take in the display text and the input field
    [SerializeField]
    TMP_InputField nameInputField = null;
    [SerializeField]
    string playerPrefNameTag = "";
    [SerializeField]
    string serverServiceTagName = "ServerService";
    [SerializeField]
    string nextScene = "Main menu";

    public TextDisplay showText = null;
    public SceneChange sceneManager = null;

    // Use this for initialization
    void Start () {
        //nameFieldGroup = this.transform.parent.gameObject;
        if (nameInputField != null)
            nameInputField.text = PlayerPrefs.GetString(playerPrefNameTag, "Default001");
    }

    public void SaveName()
    {
        if(nameInputField.text.Contains(","))
        {
            if(showText != null)
                showText.StartCoroutine(showText.DisplayText("User cannot input ,", 3));
            return;
        }
        PlayerPrefs.SetString(playerPrefNameTag, nameInputField.text);
        if (GameObject.FindGameObjectWithTag(serverServiceTagName) == null)
            return;
        GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().SendName(playerPrefNameTag);
        if (sceneManager != null)
            sceneManager.ChangeScene(nextScene);
    }
}
