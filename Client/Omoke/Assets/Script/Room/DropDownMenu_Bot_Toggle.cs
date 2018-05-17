using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropDownMenu_Bot_Toggle : MonoBehaviour {

    [SerializeField]
    GameObject readyOrNot = null;
    [SerializeField]
    GameObject readyBtn = null;
    [SerializeField]
    GameObject cancelBtn = null;
    [SerializeField]
    string serverServiceTagName = "ServerService";
    [SerializeField]
    bool secondPlayer = false;

    private TMP_Dropdown dropDownMenu = null;
    private ServerConnection server = null;

    private int currentValue = 0;

    // Use this for initialization
    void Start()
    {
        dropDownMenu = this.GetComponent<TMP_Dropdown>();
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();

        if (dropDownMenu == null)
            Debug.Log("ERROR: No TM dropdown menu found within the gameobject");
        if (server == null)
            Debug.Log("ERROR: No server object has been found");

        PlayerPrefs.SetInt("AIGAME", 0);
        PlayerPrefs.SetInt("Player2AI", 0);
        PlayerPrefs.SetInt("Player1AI", 0);
    }

    // Update is called once per frame
    void Update()
    {
        //If either is not set then just return
        if (dropDownMenu == null || readyOrNot == null)
            return;

        if(server.roomReset)
        {
            dropDownMenu.value = 0;
            server.roomReset = false;
        }

        if(currentValue != dropDownMenu.value)
        {
            currentValue = dropDownMenu.value;
            if (dropDownMenu.value == 0)
            {
                if (secondPlayer)
                {
                    server.UnSetAIGame();
                    PlayerPrefs.SetInt("AIGAME", 0);
                    PlayerPrefs.SetInt("Player2AI", 0);
                    server.SetAIPicture(0);
                }
                else
                {
                    PlayerPrefs.SetInt("Player1AI", 0);
                    server.SetPlayerPicture(currentValue);
                }
                readyOrNot.SetActive(false);
                if(readyBtn != null && cancelBtn != null)
                {
                    readyBtn.SetActive(true);
                    cancelBtn.SetActive(false);
                }
                //Send a packet to server to tell it to not be ready
                if (server != null && !secondPlayer)
                    server.RoomSetReady(false);
            }
            else
            {
                if (secondPlayer)
                {
                    server.SetAIGame();
                    PlayerPrefs.SetInt("AIGAME", 1);
                    PlayerPrefs.SetInt("Player2AI", currentValue);
                    server.SetAIPicture(currentValue);
                }
                else
                {
                    PlayerPrefs.SetInt("Player1AI", currentValue);
                    server.SetPlayerPicture(currentValue);
                }
                readyOrNot.SetActive(true);
                //Send a packet to server to tell it to be ready
                if (readyBtn != null && cancelBtn != null)
                {
                    readyBtn.SetActive(false);
                    cancelBtn.SetActive(false);
                }
                if (server != null && !secondPlayer)
                    server.RoomSetReady(true);
            }
        }
    }
}