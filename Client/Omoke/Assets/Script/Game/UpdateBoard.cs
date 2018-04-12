using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBoard : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";
    [SerializeField]
    Color enemyTile;
    [SerializeField]
    Color yourTile;

    ServerConnection server = null;

    //private GameObject[] childObjectBoard;

    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
    }
	
	// Update is called once per frame
	void Update () {
		if(server.receiveNewCurrentGamePacket)
        {
            server.receiveNewCurrentGamePacket = false;
            updateTheBoard();
        }
	}

    void updateTheBoard()
    {
        int[] newMapData = server.GetMapData();
        int myIndexNumber = server.MyNumber();
        foreach (Transform tile in transform)
        {
            if(newMapData[int.Parse(tile.gameObject.name)] > 0)
            {
                tile.gameObject.GetComponent<BoxCollider>().enabled = false;
                tile.gameObject.GetComponent<SpriteRenderer>().color = (newMapData[int.Parse(tile.gameObject.name)] == myIndexNumber ? yourTile : enemyTile);
            }
        }
    }
}
