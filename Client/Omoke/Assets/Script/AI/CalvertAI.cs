using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Very simple block ai
/// </summary>

public class CalvertAI : MonoBehaviour {

    //Server is needed
    [SerializeField]
    string serverServiceTagName = "ServerService";
    ServerConnection server = null;

    [SerializeField]
    Vector2 rowAndCol = new Vector2(0, 0);

    [SerializeField]
    bool isAIturn = false;

    [SerializeField]
    bool isAIPlayer2 = false;

    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (server == null)
        {
            Debug.LogError("Cannot find Server: " + this.name);
            this.enabled = false;
            return;
        }
        Random.InitState(20);
    }
	
	// Update is called once per frame (Need do send to server)
	void Update () {
        if (!server.GetMyTurn() && isAIPlayer2)
            GetAIDecision();
        else if (!isAIPlayer2 && server.GetMyTurn())
            GetAIDecision();
	}


    //AI Decide where to put the tiles (Random number for now)
    private int GetAIDecision()
    {
        int[] board = server.GetMapData();
        int chosen = 0;
        do
        {
            chosen = Random.Range(0, board.Length);
        }
        while (board[chosen] != 0);
        Debug.Log("GetAIDecision: " + chosen);
        return chosen;
    }
}
