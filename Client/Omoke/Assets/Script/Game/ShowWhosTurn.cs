using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowWhosTurn : MonoBehaviour {

    [SerializeField]
    string yourTurn = "Your Turn";
    [SerializeField]
    string enemyTurn = "Enemy Turn";

    [SerializeField]
    string serverServiceTagName = "ServerService";

    [SerializeField]
    TextMeshProUGUI whoIsBlackOrWhite = null;

    ServerConnection server = null;
    TextMeshProUGUI playerGuiText = null;

    private bool isSpectator = false;
    private string playerOne = "";
    private string playerTwo = "";
    // Use this for initialization
    void Start()
    {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        playerGuiText = this.GetComponent<TextMeshProUGUI>();
        if (server == null)
        {
            Debug.LogError("There is no server found in object name: " + this.gameObject.name);
            this.enabled = false;
        }
        if(playerGuiText == null)
            Debug.LogError("There is no GUI found in object name: " + this.gameObject.name);
        isSpectator = server.isSpectator;
        playerOne = server.GetPlayerOne();
        playerTwo = server.GetPlayerTwo();
        if (server.GetMyTurn() && whoIsBlackOrWhite && !isSpectator)
        {
            whoIsBlackOrWhite.text = server.userName + " is black\n" + server.opponentName + " is white";
        }
        else if (!server.GetMyTurn() && whoIsBlackOrWhite && !isSpectator)
        {
            whoIsBlackOrWhite.text = server.userName + " is white\n" + server.opponentName + " is black";
        }
        if (server.GetMyTurn() && whoIsBlackOrWhite && isSpectator)
        {
            whoIsBlackOrWhite.text = playerOne + " is black\n" + playerTwo + " is white";
        }
        else if (!server.GetMyTurn() && whoIsBlackOrWhite && isSpectator)
        {
            whoIsBlackOrWhite.text = playerOne + " is white\n" + playerTwo + " is black";
        }
    }

    // Update is called once per frame
    void Update () {
        if (server == null || playerGuiText == null)
            return;
        if(!isSpectator)
        {
            if (server.GetMyTurn())
                playerGuiText.text = yourTurn;
            else
                playerGuiText.text = enemyTurn;
        }
        else
        {
            if (server.GetMyTurn())
                playerGuiText.text = playerOne + " turn";
            else
                playerGuiText.text = playerTwo + " turn";
        }
	}
}
