using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentViewScaler : MonoBehaviour {

    //Default variables
    [SerializeField]
    float cellXSize, cellYSize;
    private float defaultX, defaultY;
    private int amountOfPlayers;
    private RectTransform thisGameobject;
    private int playerCount = 0;
	// Use this for initialization
	void Start () {
        defaultX = cellXSize;
        defaultY = cellYSize;
        cellXSize = this.GetComponent<GridLayoutGroup>().cellSize.x;
        cellYSize = this.GetComponent<GridLayoutGroup>().cellSize.y;
        thisGameobject = this.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        amountOfPlayers = this.transform.childCount;
        if (amountOfPlayers == playerCount)
            return;
        Debug.Log("New Child Count: " + amountOfPlayers);
        thisGameobject.sizeDelta = new Vector2(thisGameobject.sizeDelta.x, defaultY + (cellYSize * amountOfPlayers));
        playerCount = amountOfPlayers;
    }
}
