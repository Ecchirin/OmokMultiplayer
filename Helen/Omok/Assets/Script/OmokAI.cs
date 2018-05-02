using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmokAI : MonoBehaviour {

    public OmokManager _manager;
	public GameObject stonePos;

	public Stone AIStone;


    void Awake()
    {
        _manager = GameObject.Find("OmokManager").GetComponent<OmokManager>();
    }
	
	// Update is called once per frame
	void Update () {
		if (_manager.isPlaying)
		{
			if (_manager.isAITurn)
			{
				Debug.Log("AI");
				_manager.isAITurn = false;
				ChoosePos();
			}
		}
	}

	public virtual void ChoosePos() //돌을 둘 위치를 선정 (현재는 랜덤, AI 작동함수)
    {
		bool endflag = false;
		for(int n =0; n<3; n++)
		{ 
			int rand1 = Random.Range(0, 14);
			int rand2 = Random.Range(0, 14);
			Debug.Log(n);
			if (_manager.OmokBoard[rand1, rand2] != (int)Stone.Black && _manager.OmokBoard[rand1, rand2] != (int)Stone.White)
			{
				StonePos(rand1, rand2);
				Debug.Log("Random"+ rand1 + ", " + rand2);
				break;
			}
			else if (n == 2)
			{
				for (int j = 0; j < 15; j++)
				{
					for (int k = 0; k < 15; k++)
					{
						if (_manager.OmokBoard[j, k] != (int)Stone.Black && _manager.OmokBoard[j, k] != (int)Stone.White)
						{
							StonePos(j, k);
							endflag = true;
							Debug.Log("NotRandom"+j + ", " + k);
							break;
						}
					}
					if (endflag)
						break;
				}
				break;
			}
			else continue;
		}
	}


	public virtual void StonePos(int y, int x) //돌의 위치를 전달하는 함수 (안변함)
	{
		GameObject board = GameObject.Find("Board");
		Transform[] boardChild1 = new Transform[board.transform.childCount];
		for (int i = 0; i < board.transform.childCount; i++)
			boardChild1[i] = board.transform.GetChild(i);

		Transform[] boardChild2 = new Transform[boardChild1[y].childCount];
		for (int i = 0; i < boardChild1[y].childCount; i++)
			boardChild2[i] = boardChild1[y].transform.GetChild(i);

		stonePos = boardChild2[x].gameObject;
		_manager.PutStone(stonePos, AIStone);
	}

        
 }



