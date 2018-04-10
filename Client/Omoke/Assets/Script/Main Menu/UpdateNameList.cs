using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class is use to refresh the player list in the main menu to know who is online.
/// Do not use this class for other things
/// </summary>

public class UpdateNameList : MonoBehaviour {

    [SerializeField]
    GameObject contentView = null;
    [SerializeField]
    GameObject exampleTextView = null;
    [SerializeField]
    string serverServiceTagName = "ServerService";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Awake()
    {
        refreshList();
    }

    public void refreshList()
    {
        if(exampleTextView == null || contentView == null)
        {
            Debug.Log("Error no exampleTextview or contentView found");
            return;
        }
        foreach (Transform child in contentView.transform)
        {
            //GameObject.Destroy(child.gameObject);
            Destroy(child.gameObject);
        }
        string players = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetPlayerList();
        string[] strArray = players.Split(',');
        Debug.Log(strArray.Length);
        Debug.Log(players);
        for(int i = 0; i < strArray.Length; ++i)
        {
            GameObject newObject = Instantiate(exampleTextView, contentView.transform);
            newObject.GetComponent<TextMeshProUGUI>().text = strArray[i];
        }
    }
}
