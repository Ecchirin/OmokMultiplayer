using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBoard : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";
    [SerializeField]
    Color enemyTile = Color.red;
    [SerializeField]
    Color yourTile = Color.green;

    ServerConnection server = null;
    [SerializeField]
    bool isSpectator = false;

    //private GameObject[] childObjectBoard;

    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (server == null)
            Debug.LogError("There is no server found in UpdateBoard.cs object name: " + this.gameObject.name);

        isSpectator = server.isSpectator;
    }
	
	// Update is called once per frame
	void Update () {
        if (server == null)
            return;
		if(server.receiveNewCurrentGamePacket)
        {
            server.receiveNewCurrentGamePacket = false;
            updateTheBoard();
        }
	}

    void updateTheBoard()
    {
        int[] newMapData = server.GetMapData();
        Debug.Log("THIS IS A TABLE COUNT : " + newMapData.Length);
        int myIndexNumber = server.MyNumber();
        foreach (Transform tile in transform)
        {
            if(!isSpectator)
            {
                if (newMapData[int.Parse(tile.gameObject.name)] > 0)
                {
                    tile.gameObject.GetComponent<BoxCollider>().enabled = false;
                    tile.gameObject.GetComponent<SpriteRenderer>().color = (newMapData[int.Parse(tile.gameObject.name)] == myIndexNumber ? yourTile : enemyTile);
                }
            }
            else
            {
                if (newMapData[int.Parse(tile.gameObject.name)] > 0)
                {
                    tile.gameObject.GetComponent<BoxCollider>().enabled = false;
                    tile.gameObject.GetComponent<SpriteRenderer>().color = (newMapData[int.Parse(tile.gameObject.name)] == 1 ? yourTile : enemyTile);
                }
            }
        }
    }
}
