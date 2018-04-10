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
    }

    // Update is called once per frame
    void Update()
    {
        //If either is not set then just return
        if (dropDownMenu == null || readyOrNot == null)
            return;

        if(currentValue != dropDownMenu.value)
        {
            currentValue = dropDownMenu.value;
            if (dropDownMenu.value == 0)
            {
                readyOrNot.SetActive(false);
                if(readyBtn != null && cancelBtn != null)
                {
                    readyBtn.SetActive(true);
                    cancelBtn.SetActive(false);
                }
                //Send a packet to server to tell it to not be ready
                if (server != null)
                    server.RoomSetReady(false);
            }
            else
            {
                readyOrNot.SetActive(true);
                //Send a packet to server to tell it to be ready
                if (readyBtn != null && cancelBtn != null)
                {
                    readyBtn.SetActive(false);
                    cancelBtn.SetActive(false);
                }
                if (server != null)
                    server.RoomSetReady(true);
            }
        }
    }
}
