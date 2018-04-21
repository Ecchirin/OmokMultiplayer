using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour {

    [SerializeField]
    GameObject[] prefabAI = null;

    [SerializeField]
    string serverServiceTagName = "ServerService";
    private ServerConnection server = null;

    // Use this for initialization
    void Start() {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (PlayerPrefs.GetInt("Player1AI", 0) != 0)
        {
            GameObject newAI = Instantiate(prefabAI[PlayerPrefs.GetInt("Player1AI", 0) - 1]);
            newAI.GetComponent<CalvertAI>().initialiseTheBot(server.GetMyTurn(), server.MyNumber());
        }
        if (PlayerPrefs.GetInt("Player2AI", 0) != 0 && PlayerPrefs.GetInt("AIGAME", 0) == 1)
        {
            GameObject newAI = Instantiate(prefabAI[PlayerPrefs.GetInt("Player2AI", 0) - 1]);
            Debug.Log("AI: " + PlayerPrefs.GetInt("Player2AI", 0));
            newAI.GetComponent<CalvertAI>().initialiseTheBot(!server.GetMyTurn(), (server.MyNumber() == 1 ? 2 : 1), true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
