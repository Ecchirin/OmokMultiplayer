using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is used to setup the game board only
 * It controls the size of the board and what it should render as the game background for the board
*/

public class Game_Setup : MonoBehaviour {

    //Position of the board (this use percentage)
    [SerializeField]
    float startX = 0.5f;
    [SerializeField]
    float startY = 0.5f;

    //The camera object
    [SerializeField]
    Camera mainCamera = null;

    //Transform of tiles
    [SerializeField]
    Transform tiles = null;

    //Size of board
    [SerializeField]
    int boardSize = 1;
    [SerializeField]
    float scaleOfTile = 1;

    //Should i declare board container here? //Nope

	// Use this for initialization
	void Start () {
        if (scaleOfTile < 0.0f)
            scaleOfTile = 0;
        CreateABoard();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Create the visual of the board
    void CreateABoard() {
        //Size of a tile
        float tileX = tiles.GetComponent<SpriteRenderer>().bounds.size.x * scaleOfTile;
        float tileY = tiles.GetComponent<SpriteRenderer>().bounds.size.y * scaleOfTile;
        //Get where to start from main camera
        float coordY = mainCamera.ViewportToWorldPoint((new Vector3(0, startY, 10))).y;
        //Spawn the board
        for (int row = 0; row < boardSize; ++row)
        {
            //Get the x coordinate starting from camera
            float coordX = mainCamera.ViewportToWorldPoint(new Vector3(startX, 0, 10)).x;
            for (int col = 0; col < boardSize; ++col)
            {
                //Set position and spawn
                Vector3 tilePosition = new Vector3(coordX, coordY, 0);
                Transform go = Instantiate(tiles, tilePosition, Quaternion.identity);
                go.parent = this.transform;
                go.localScale = new Vector3(scaleOfTile, scaleOfTile, 1);
                coordX += tileX;
            }
            coordY -= tileY;
        }
    }
}
