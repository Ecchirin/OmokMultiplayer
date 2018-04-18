using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is to be put in the camera
/// Find out what has been clicked
/// get the name of the object
/// if spectator turn this class off
/// </summary>

public class ClickRaycast : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";

    ServerConnection server = null;

    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (server == null)
            Debug.LogError("There is no server found in UpdateBoard.cs object name: " + this.gameObject.name);
        if (server.isSpectator)
            this.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if(server == null || server.GetWinner() != 0)
        {
            Debug.Log("Error cannot connect to server");
            this.enabled = false;
            return; 
        }
        if (Input.GetMouseButtonDown(0) && server.GetMyTurn())
        {
            SendTileName();
        }
    }

    void SendTileName()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            if (hit.collider != null)
            {
                Debug.Log("Tile selected: " + hit.transform.gameObject.name);
                server.SetMoveOnBoard(int.Parse(hit.transform.gameObject.name));
            }
    }
}
