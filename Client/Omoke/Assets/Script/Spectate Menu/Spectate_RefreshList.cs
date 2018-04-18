using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class is use to refresh the list for spectating of the game
/// </summary>


public class Spectate_RefreshList : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";
    private ServerConnection serverService = null;
    [SerializeField]
    GameObject contentView = null;
    [SerializeField]
    GameObject exampleRoomButton = null;

    // Use this for initialization
    void Start()
    {
        serverService = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (serverService == null)
            Debug.Log("Error: Server Service cannot be found");
        RefreshList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RefreshList()
    {
        if (exampleRoomButton == null || contentView == null || serverService == null)
        {
            Debug.Log("Error no exampleRoomButton or contentView found");
            return;
        }
        DestroyAllChild();
        string players = serverService.GetSpectateRoomList();
        string[] strArray = players.Split(',');
        Debug.Log(strArray.Length);
        Debug.Log(players);
        for (int i = 0; i < strArray.Length; ++i)
        {
            GameObject newObject = Instantiate(exampleRoomButton, contentView.transform);
            newObject.GetComponentInChildren<TextMeshProUGUI>().text = strArray[i];
            newObject.GetComponent<JoinRoomButton>().isSpectator = true;
        }
    }

    void DestroyAllChild()
    {
        foreach (Transform child in contentView.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
