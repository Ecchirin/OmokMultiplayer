using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Stone {
Black= 8,
White = 9
}

public class OmokManager : MonoBehaviour {

	public Stone AIStone;
	public Stone PlayerStone;
    public Player _player;
    public OmokAI _ai;

	public FindOmok _find;

    public int TurnCount;

	public int[ , ]  OmokBoard= new int [15,15]; //오목 상태를 저장할 배열
    public bool isPlaying; // 게임이 실행중인지 판별
    public bool isPlayerTurn=true; //플레이어 차례인지 아닌지 판별
	public bool isAITurn = false;

    public GameObject BlackStone;
    public GameObject WhiteStone;


   [SerializeField]
   
   bool isOmok;

	void Awake()
	{
		_player = GameObject.Find("Player").GetComponent<Player>();
		_ai = GameObject.Find("OmokAI").GetComponent<OmokAI>();
		_find = GetComponent<FindOmok>();

		isPlaying = false;
	}

	void Start () {
		TurnCount = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PutStone(GameObject obj,  Stone stone) //오브젝트에서 좌표값을 받아와서 배열에 저장
    {
        string objName = obj.name;
        string objParentName = obj.transform.parent.name;
        int column=0;
        int row=0;
        bool Result;
        int number;

        Result = int.TryParse(objName, out number);
            if(Result){
            column = System.Convert.ToInt32( objName );
        }
        Result = int.TryParse(objParentName, out number);
            if(Result){
              row = System.Convert.ToInt32( objParentName ); 
        }

        if( OmokBoard[row, column] != (int)Stone.Black && OmokBoard[row, column] != (int)Stone.White ) {
            OmokBoard[row, column] = (int)stone;
            if( stone == Stone.Black )
                Instantiate( BlackStone, obj.transform.position, Quaternion.identity );
            else
                Instantiate( WhiteStone, obj.transform.position, Quaternion.identity );

            isOmok = _find.Find( (int)stone );

            if( !isOmok ) //오목일때 게임 종료
            { 
            isPlayerTurn = ( isPlayerTurn == true ) ? false : true;
            if( !isPlayerTurn ) isAITurn = true;
            TurnCount++;
            }
            else
            {
                isPlaying = false;
            }
		}
		else
			Debug.Log("Error:"+ OmokBoard[row, column] + row + ", " + column );
    }
}
