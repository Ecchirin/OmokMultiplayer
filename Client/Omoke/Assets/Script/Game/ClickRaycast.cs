using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is to be put in the camera
/// </summary>

public class ClickRaycast : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";

    ServerConnection server = null;

    // Use this for initialization
    void Start () {
        //Debug.Log("Click script has started");
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
	}
	
	// Update is called once per frame
	void Update () {
        if(server == null)
        {
            Debug.Log("Error cannot connect to server");
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            GetTileName();
        }
    }

    void GetTileName()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
            if (hit.collider != null)
            {
                Debug.Log("Tile selected: " + hit.transform.gameObject.name);
                server.SetMoveOnBoard(int.Parse(hit.transform.gameObject.name));
                //Destroy(hit.transform.gameObject);
                //hit.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
                //hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                
            }
    }

}
