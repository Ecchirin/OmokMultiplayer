using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindOmok : MonoBehaviour {

	OmokManager _manager;
	void Awake()
	{
		_manager = GameObject.Find("OmokManager").GetComponent<OmokManager>();
	}


	public bool Find(int stone) //전체오목판 체크 8방향체크로 나중에 바꾸면 좋을 것같음
	{
		int five = 0;

		//가로줄
		for(int i = 0; i<15; i++)
		{
			five = 0;
			for (int j = 0; j<15; j++)
			{
				if (_manager.OmokBoard[i, j] == stone)
					five++;
				else
					five = 0;

				if (five == 5)
					break;
			}
			if (five == 5)
			{
				Debug.Log("Five!!");
				return true;
			}
			else five = 0;
		}
		//세로줄
		for (int i = 0; i < 15; i++)
		{
			five = 0;
			for (int j = 0; j < 15; j++)
			{
				if (_manager.OmokBoard[j, i] == stone)
					five++;
				else
					five = 0;

				if (five == 5)
					break;
			}
			if (five == 5)
			{
				Debug.Log("Five!!");
                return true;
			}
			else five = 0;
		}

		//대각선 왼쪽
		for(int i = 4; i<15; i++)
		{
			five = 0;
			int k = i;
			for(int j =0; j<=i; j++)
			{
				if (_manager.OmokBoard[k--, j] == stone)
					five++;
				else
					five = 0;

				if (five == 5)
					break;
			}
			if (five == 5)
			{
				Debug.Log("Five!!");
                return true;
			}
			else five = 0;
		}

		for(int i =1; i<=10; i++)
		{
			five = 0;
			int k = i;
			for(int j = 14; j>=i; j--)
			{
				if (_manager.OmokBoard[j, k++] == stone)
					five++;
				else
					five = 0;

				if (five == 5)
					break;
			}
			if (five == 5)
			{
				Debug.Log("Five!!");
                return true;
			}
			else five = 0;
		}
		

		//대각선 오른쪽
		for(int i =10; i>=0; i--)
		{
			five = 0;
			int k = i;
			for(int j = 0; j<= 14-i; j++)
			{
				if (_manager.OmokBoard[j, k++] == stone)
					five++;
				else
					five = 0;

				if (five == 5)
					break;
			}
			if (five == 5)
			{
				Debug.Log("Five!!");
			    return true;
			}
			else five = 0;
		}

		for(int i = 1; i<=10; i++)
		{
			five = 0;
			int k = i;
			for(int j = 0; j<=14-i; j++)
			{
				if (_manager.OmokBoard[k++, j] == stone)
					five++;
				else
					five = 0;

				if (five == 5)
					break;
			}
			if (five == 5)
			{
				Debug.Log("Five!!");
                return true;
			}
			else five = 0;
		}
        return false;

	}
}
