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

    ServerConnection server = null;
    TextMeshProUGUI guiText = null;
    // Use this for initialization
    void Start()
    {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        guiText = this.GetComponent<TextMeshProUGUI>();
        if (server == null)
            Debug.LogError("There is no server found in object name: " + this.gameObject.name);
        if(guiText == null)
            Debug.LogError("There is no GUI found in object name: " + this.gameObject.name);
    }

    // Update is called once per frame
    void Update () {
        if (server == null || guiText == null)
            return;
        if (server.GetMyTurn())
            guiText.text = yourTurn;
        else
            guiText.text = enemyTurn;
	}
}
