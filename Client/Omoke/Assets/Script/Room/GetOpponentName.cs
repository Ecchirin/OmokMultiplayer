using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
/// <summary>
/// This is to be attached to the name tag of the player 2
/// controls the enemy status
/// </summary>

public class GetOpponentName : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";
    [SerializeField]
    GameObject opponentDropDownMenu = null;
    [SerializeField]
    GameObject readyOrNotSign_P2 = null;
    [SerializeField]
    GameObject readyOrNotSign_P1 = null;
    [SerializeField]
    GameObject readyBtn = null;
    [SerializeField]
    GameObject cancelBtn = null;
    //[SerializeField]
    //TextMeshProUGUI opponentDisplayText = null;

    ServerConnection server = null;
    private bool initTimer = false;
    DateTime countDownTimer = DateTime.Now;
    DateTime targetTimer = DateTime.Now;
    private bool gamestart = false;
    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
	}
	
	// Update is called once per frame
	void Update () {
        if (server == null)
            return;
		if(server.opponentInRoom && opponentDropDownMenu.activeSelf)
        {
            this.GetComponent<TextMeshProUGUI>().text = server.opponentName;
            opponentDropDownMenu.SetActive(false);
            readyOrNotSign_P1.SetActive(false);
            readyBtn.SetActive(true);
            cancelBtn.SetActive(false);
        }
        else if (!server.opponentInRoom && !opponentDropDownMenu.activeSelf)
        {
            this.GetComponent<TextMeshProUGUI>().text = "Waiting for player";
            opponentDropDownMenu.SetActive(true);
            readyOrNotSign_P2.SetActive(false);
            readyOrNotSign_P1.SetActive(false);
            readyBtn.SetActive(true);
            cancelBtn.SetActive(false);
        }

        if(server.opponentInRoom && server.opponentIsReady && readyOrNotSign_P2 != null)
        {
            readyOrNotSign_P2.SetActive(true);
        }
        else if (server.opponentInRoom && !server.opponentIsReady && readyOrNotSign_P2 != null)
        {
            readyOrNotSign_P2.SetActive(false);
        }

        if(readyOrNotSign_P1.active == true && readyOrNotSign_P2.active == true)
        {
            if (!initTimer)
                InitCountdownTimer();
            countDownTimer = DateTime.Now;
            if (countDownTimer > targetTimer && !gamestart)
            {
                server.RoomStartGame();
                gamestart = true;
            }
        }
        else
        {
            initTimer = false;
            gamestart = false;
        }
	}
    void InitCountdownTimer()
    {
        initTimer = true;
        countDownTimer = DateTime.Now;
        targetTimer = DateTime.Now.AddSeconds(3);
    }
}
