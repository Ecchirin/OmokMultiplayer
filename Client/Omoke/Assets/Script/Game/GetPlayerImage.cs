using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetPlayerImage : MonoBehaviour {

    [SerializeField]
    string serverServiceTagName = "ServerService";
    [SerializeField]
    bool player1 = false;
    [SerializeField]
    bool player2 = false;

    [SerializeField]
    int imageNumber = 0;

    [SerializeField]
    Texture[] image;

    // Use this for initialization
    void Start () {
        Debug.Log("Image Number1 : " + GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetPlayerOnePicture());
        Debug.Log("Image Number2 : " + GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetPlayerTwoPicture());
        if (player1)
            imageNumber = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetPlayerOnePicture();
        else
            imageNumber = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>().GetPlayerTwoPicture();

        if(imageNumber < image.Length && imageNumber >= 0)
            this.GetComponent<RawImage>().texture = image[imageNumber];

        this.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
