using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This shows who has won the match
/// Disables itself after writing the text
/// </summary>

public class DisplayWhoWon : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";

    ServerConnection server = null;

    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (server == null)
            Debug.LogError("There is no server found in UpdateBoard.cs object name: " + this.gameObject.name);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Call once every few frames
    private void FixedUpdate()
    {
        if (server == null || server.GetWinner() == 0)
            return;

        if(server.GetWinner() == 1)
        {
            this.GetComponent<TextMeshProUGUI>().text = server.GetPlayerOne() + " has won the match";
        }
        else if (server.GetWinner() == 2)
        {
            this.GetComponent<TextMeshProUGUI>().text = server.GetPlayerTwo() + " has won the match";
        }
        if (server.GetWinner() != server.MyNumber())
        {
            //this.GetComponent<TextMeshProUGUI>().text = server.GetOpponentName() + " has won the match";
            this.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            //this.GetComponent<TextMeshProUGUI>().text = server.userName + " has won the match";
            this.GetComponent<TextMeshProUGUI>().color = Color.green;
        }
        this.enabled = false;
    }
}
