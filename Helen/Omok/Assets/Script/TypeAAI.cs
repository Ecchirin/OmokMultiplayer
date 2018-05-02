using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rule Based
public class TypeAAI : OmokAI {

    bool isEnd;
	void Awake()
	{
		_manager = GameObject.Find("OmokManager").GetComponent<OmokManager>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (_manager.isPlaying)
		{
			if (_manager.isAITurn)
			{
				Debug.Log("AI");
				_manager.isAITurn = false;
				AI((int) _manager.PlayerStone, (int)_manager.AIStone);
			}
		}
	}

	public override void StonePos(int y, int x)
	{
		base.StonePos(y, x);
	}

	public override void ChoosePos()
	{
		base.ChoosePos();
	}

	void AI(int _enemystone, int _mystone)
	{
        isEnd = false;
        ///내4체크///
       if(!isEnd)
        isEnd = CheckFour(_mystone);
        ///상대방4체크///
        if(!isEnd)
        isEnd = CheckFour(_enemystone);
        ///상대방열린3체크///
       if(!isEnd)
         isEnd =CheckOpenThree(_enemystone);
        ///내열린3체크///
       if(!isEnd)
         isEnd =CheckOpenThree(_mystone);
       ///내 닫힌3 체크///
       if(!isEnd)
            isEnd = CheckCloseThree(_mystone);
       ///2체크///
       if(!isEnd)
            isEnd = CheckOpenTwo(_mystone);
       if(!isEnd)
            isEnd = CheckCloseTwo(_mystone);
       ///1체크///
       if(!isEnd)
            isEnd = CheckOne(_mystone);
       if(!isEnd)
            isEnd = CheckOne(_enemystone);
        if(!isEnd)
		    PutButtom();
	}


    bool CheckFour(int stone)
    {
        for(int i = 0; i<15; i++)
		{
			for(int j = 0; j<15; j++)
			{
				if( _manager.OmokBoard[i, j]==stone)
				{
					//가로
					if((j+3)<=14)
					{
						if (_manager.OmokBoard[i, j + 1] == stone && _manager.OmokBoard[i, j + 2] == stone && _manager.OmokBoard[i, j + 3] == stone)
						{
							if ((j - 1) >= 0 && _manager.OmokBoard[i, j - 1] == 0)
							{
								StonePos(i, j - 1);
								return true ;
							}
							else if((j+4)<=14&&_manager.OmokBoard[i, j+4]==0)
							{
								StonePos(i, j +4);
								return true;
							}
						}
					}
					else if((j+4)<=14)
					{
						int count0 = 0;
						int count1 = 0;
						int x = 0, y = 0;
						for (int k = 1; k <= 4; k++)
						{
							if (_manager.OmokBoard[i, j + k] == stone)
								count1++;
							else if (_manager.OmokBoard[i, j + k] == 0 && k != 4)
							{
								count0++;
								y = i;
								x = j + k;
							}
						}
						if (count1 == 3 && count0 == 1)
						{
							StonePos(y, x);
							return true;
						}
					}

					//세로
					if ((i + 3) <= 14)
					{
						if (_manager.OmokBoard[i+1, j] == stone && _manager.OmokBoard[i+2, j] == stone && _manager.OmokBoard[i+3, j] == stone)
						{
							if ((i - 1) >= 0 && _manager.OmokBoard[i-1, j] == 0)
							{
								StonePos(i-1, j);
								return true;
							}
							else if ((i + 4) <= 14 && _manager.OmokBoard[i+4, j] == 0)
							{
								StonePos(i+4, j);
								return true;
							}
						}
					}
					else if ((i + 4) <= 14)
					{
						int count0 = 0;
						int count1 = 0;
						int x = 0, y = 0;
						for (int k = 1; k <= 4; k++)
						{
							if (_manager.OmokBoard[i+k, j] == stone)
								count1++;
							else if (_manager.OmokBoard[i+k, j] == 0 && k != 4)
							{
								count0++;
								y = i+k;
								x = j;
							}
						}
						if (count1 == 3 && count0 == 1)
						{
							StonePos(y, x);
							return true;
						}
					}

					//왼쪽 대각선
					if ((j + 3) <= 14&&(i+3)<=14)
					{
						if (_manager.OmokBoard[i+1, j + 1] == stone && _manager.OmokBoard[i+2, j + 2] == stone && _manager.OmokBoard[i+3, j + 3] == stone)
						{
							if ((j - 1) >= 0 &&(i-1)>=0&& _manager.OmokBoard[i-1, j - 1] == 0)
							{
								StonePos(i-1, j - 1);
								return true;
							}
							else if ((j + 4) <= 14&&(i+4)<=14 && _manager.OmokBoard[i+4, j + 4] == 0)
							{
								StonePos(i+4, j + 4);
								return true;
							}
						}
					}
					else if ((j + 4) <= 14&&(i+4)<=14)
					{
						int count0 = 0;
						int count1 = 0;
						int x = 0, y = 0;
						for (int k = 1; k <= 4; k++)
						{
							if (_manager.OmokBoard[i+k, j + k] == stone)
								count1++;
							else if (_manager.OmokBoard[i+k, j + k] == 0 && k != 4)
							{
								count0++;
								y = i+k;
								x = j + k;
							}
						}
						if (count1 == 3 && count0 == 1)
						{
							StonePos(y, x);
							return true;
						}
					}

					//오른쪽대각선
					if ((j - 3)>=0&&(i+4)<=14)
					{
						if (_manager.OmokBoard[i+1, j - 1] == stone && _manager.OmokBoard[i+2, j - 2] == stone && _manager.OmokBoard[i+3, j-3] == stone)
						{
							if ((j + 1) <=14 &&(i-1)>=0 &&_manager.OmokBoard[i-1, j +1] == 0)
							{
								StonePos(i-1, j + 1);
								return true;
							}
							else if ((j - 4) >= 0&&(i+4)<=14 && _manager.OmokBoard[i+4, j - 4] == 0)
							{
								StonePos(i+4, j - 4);
								return true;
							}
						}
					}
					else if ((j - 4) >= 0 && (i + 4) <=14)
					{
						int count0 = 0;
						int count1 = 0;
						int x = 0, y = 0;
						for (int k = 1; k <= 4; k++)
						{
							if (_manager.OmokBoard[i+k, j - k] == stone)
								count1++;
							else if (_manager.OmokBoard[i+k, j- k] == 0 && k != 4)
							{
								count0++;
								y = i+k;
								x = j - k;
							}
						}
						if (count1 == 3 && count0 == 1)
						{
							StonePos(y, x);
							return true;
						}
					}
				}
			}
		}
        return false;
    }
    bool CheckOpenThree(int stone)
    {
        //가로
		for(int i = 0; i<15;i++)
		{
			for(int j = 0; j<15; j++)
			{
				if ((j + 4) <= 14 && _manager.OmokBoard[i, j] ==0) //다음 바둑돌이 세개 연속인지 확인
				{
					if ( _manager.OmokBoard[i, j + 1] == stone && _manager.OmokBoard[i, j + 2] == stone && _manager.OmokBoard[i, j + 3] == stone && _manager.OmokBoard[i, j + 4] == 0)
					{
						StonePos(i, j);
						return true;
					}
				}
				else if((j + 3) <= 14&&_manager.OmokBoard[i, j]==stone)
				{
					int count0 = 0;
					int count1 = 0;
					int x = 0, y = 0;
					for (int k = 1; k <= 3; k++)
					{
						if (_manager.OmokBoard[i, j + k] == stone)
							count1++;
						else if (_manager.OmokBoard[i, j + k] == 0&&k!=3)
						{
							count0++;
							y = i;
							x = j + k;
						}
					}
					if(count1==2&&count0==1)
					{
						StonePos(y, x);
						return true;
					}
				}
			}
		}

		//세로
		for(int i = 0; i<15; i++)
		{
			for (int j = 0; j < 15; j++)
			{
				if ((j + 4) <= 14 && _manager.OmokBoard[j, i] == 0)
				{
					if (_manager.OmokBoard[j + 1, i] == stone && _manager.OmokBoard[j + 2, i] == stone && _manager.OmokBoard[j + 3, i] == stone && _manager.OmokBoard[j + 4, i] == 0)
					{
						StonePos(j, i);
						return true;
					}

				}
				else if ((j + 3) <= 14 && _manager.OmokBoard[j,i] == stone)
				{
					int count0 = 0;
					int count1 = 0;
					int x = 0, y = 0;
					for (int k = 1; k <= 3; k++)
					{
						if (_manager.OmokBoard[j + k,i] == stone)
							count1++;
						else if (_manager.OmokBoard[j + k,i] == 0 && k != 3)
						{
							count0++;
							y = j + k;
							x = i;
						}
					}
					if (count1 == 2 && count0 == 1)
					{
						StonePos(y, x);
						return true;
					}
				}
			}
		}

		//왼쪽 대각선
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 15; j++)
			{
				if ((j + 4) <= 14 &&(i+4)<=14&& _manager.OmokBoard[i, j] == 0)
				{
					if (_manager.OmokBoard[i + 1, j+1] == stone && _manager.OmokBoard[i + 2, j+2] == stone && _manager.OmokBoard[i + 3, j+3] == stone && _manager.OmokBoard[i + 4, j+4] == 0)
					{
						StonePos(i, j);
						return true;
					}

				}
				else if ((j + 3) <= 14 && (i + 3) <= 14 && _manager.OmokBoard[i, j] == stone)
				{
					int count0 = 0;
					int count1 = 0;
					int x = 0, y = 0;
					for (int k = 1; k <= 3; k++)
					{
						if (_manager.OmokBoard[i + k, j+k] == stone)
							count1++;
						else if (_manager.OmokBoard[i+ k, j+k] == 0 && k != 3)
						{
							count0++;
							y = i + k;
							x = j+k;
						}
					}
					if (count1 == 2 && count0 == 1)
					{
						StonePos(y, x);
						return true;
					}
				}
			}
		}

		//오른쪽 대각선
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 15; j++)
			{
				if ((j - 4) >=0&& (i + 4) <= 14 && _manager.OmokBoard[i, j] == 0)
				{
					if (_manager.OmokBoard[i + 1, j -1] == stone && _manager.OmokBoard[i + 2, j -2] == stone && _manager.OmokBoard[i + 3, j - 3] == stone && _manager.OmokBoard[i + 4, j - 4] == 0)
					{
						StonePos(i, j);
						return true;
					}

				}
				else if ((j - 3) >=0 && (i + 3) <= 14 && _manager.OmokBoard[i, j] == stone)
				{
					int count0 = 0;
					int count1 = 0;
					int x = 0, y = 0;
					for (int k = 1; k <= 3; k++)
					{
						if (_manager.OmokBoard[i + k, j - k] == stone)
							count1++;
						else if (_manager.OmokBoard[i + k, j - k] == 0 && k != 3)
						{
							count0++;
							y = i + k;
							x = j - k;
						}
					}
					if (count1 == 2 && count0 == 1)
					{
						StonePos(y, x);
						return true;
					}
				}
			}
		}
        return false;
    }
    bool CheckCloseThree(int stone)
    {
         //가로
		for(int i = 0; i<15;i++)
		{
			for(int j = 0; j<15; j++)
			{
				if ((j + 4) <= 14 && _manager.OmokBoard[i, j] ==stone&& _manager.OmokBoard[i, j + 1] == stone && _manager.OmokBoard[i, j + 2] == stone) //다음 바둑돌이 세개 연속인지 확인
				{
					if ( (j-1)>=0&&_manager.OmokBoard[i, j-1]==0)
					{
						StonePos(i, j-1);
						return true;
					}
                    else if( (j +4 )<=14&&_manager.OmokBoard[i, j+4]==0)
                    {
                        StonePos(i, j+4);
						return true;
                    }
				}
				else if((j + 3) <= 14&&_manager.OmokBoard[i, j]==stone)
				{
					int count0 = 0;
					int count1 = 0;
					int x = 0, y = 0;
					for (int k = 1; k <= 3; k++)
					{
						if (_manager.OmokBoard[i, j + k] == stone)
							count1++;
						else if (_manager.OmokBoard[i, j + k] == 0&&k!=3)
						{
							count0++;
							y = i;
							x = j + k;
						}
					}
					if(count1==2&&count0==1)
					{
                        if( (( j - 1 ) >= 0 && _manager.OmokBoard[i, j - 1] == 0)|| ( ( j + 4 ) <= 14 && _manager.OmokBoard[i, j + 4] == 0 )) 
                        {
                            StonePos( y, x );
                            return true;
                        } 
						
					}
				}
			}
		}

		//세로
		for(int i = 0; i<15;i++)
		{
			for(int j = 0; j<15; j++)
			{
				if ((j + 4) <= 14 && _manager.OmokBoard[ j, i] ==stone&& _manager.OmokBoard[j + 1,i] == stone && _manager.OmokBoard[j + 2,i] == stone) //다음 바둑돌이 세개 연속인지 확인
				{
					if ( (j-1)>=0&&_manager.OmokBoard[j-1,i]==0)
					{
						StonePos(j-1, i);
						return true;
					}
                    else if( (j +4 )<=14&&_manager.OmokBoard[j+4, i]==0)
                    {
                        StonePos(j+4, i);
						return true;
                    }
				}
				else if((j + 3) <= 14&&_manager.OmokBoard[j, i]==stone)
				{
					int count0 = 0;
					int count1 = 0;
					int x = 0, y = 0;
					for (int k = 1; k <= 3; k++)
					{
						if (_manager.OmokBoard[j + k, i] == stone)
							count1++;
						else if (_manager.OmokBoard[j + k,i] == 0&&k!=3)
						{
							count0++;
							y = j+k;
							x = i;
						}
					}
					if(count1==2&&count0==1)
					{
                        if( (( j - 1 ) >= 0 && _manager.OmokBoard[j - 1, i] == 0)|| ( ( j + 4 ) <= 14 && _manager.OmokBoard[j + 4, i] == 0 )) 
                        {
                            StonePos( y, x );
                            return true;
                        } 
						
					}
				}
			}
		}

		//왼쪽 대각선
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 15; j++)
			{
				if ((j + 4) <= 14 &&(i+4)<=14&& _manager.OmokBoard[i, j] == stone&&_manager.OmokBoard[i + 1, j+1] == stone && _manager.OmokBoard[i + 2, j+2] == stone)
				{
					if ((i+4)<=14&&(j+4)<=14&&_manager.OmokBoard[i + 4, j+4] == 0)
					{
						StonePos(i+4, j+4);
						return true;
					}
                    else if((i-1)>=0&&(j-1)>=0&&_manager.OmokBoard[i-1, j-1]==0) 
                    {
                        StonePos(i-1, j-1);
						return true;
                    }
				}
				else if ((j + 3) <= 14 && (i + 3) <= 14 && _manager.OmokBoard[i, j] == stone)
				{
					int count0 = 0;
					int count1 = 0;
					int x = 0, y = 0;
					for (int k = 1; k <= 3; k++)
					{
						if (_manager.OmokBoard[i + k, j+k] == stone)
							count1++;
						else if (_manager.OmokBoard[i+ k, j+k] == 0 && k != 3)
						{
							count0++;
							y = i + k;
							x = j+k;
						}
					}
					if (count1 == 2 && count0 == 1)
					{
						 if( (( j - 1 ) >= 0 &&(i-1)>=0&& _manager.OmokBoard[i-1,j - 1 ] == 0)|| ( ( j + 4 ) <= 14&&(i+4)<=14 && _manager.OmokBoard[i+4,j + 4 ] == 0 )) 
                        {
                            StonePos( y, x );
                            return true;
                        } 
					}
				}
			}
		}

		//오른쪽 대각선
		for (int i = 0; i < 15; i++)
		{
			for (int j = 0; j < 15; j++)
			{
				if ((j - 4) >=0&& (i + 4) <= 14 && _manager.OmokBoard[i, j] == stone&&_manager.OmokBoard[i + 1, j -1] == stone && _manager.OmokBoard[i + 2, j -2] == stone)
				{
					if ((i+4)<=14&&(j-4)>=0&&_manager.OmokBoard[i + 4, j-4] == 0)
					{
						StonePos(i+4, j-4);
						return true;
					}
                    else if((i-1)>=0&&(j+1)<=14&&_manager.OmokBoard[i-1, j+1]==0) 
                    {
                        StonePos(i-1, j+1);
						return true;
                    }

				}
				else if ((j - 3) >=0 && (i + 3) <= 14 && _manager.OmokBoard[i, j] == stone)
				{
					int count0 = 0;
					int count1 = 0;
					int x = 0, y = 0;
					for (int k = 1; k <= 3; k++)
					{
						if (_manager.OmokBoard[i + k, j - k] == stone)
							count1++;
						else if (_manager.OmokBoard[i + k, j - k] == 0 && k != 3)
						{
							count0++;
							y = i + k;
							x = j - k;
						}
					}
					if (count1 == 2 && count0 == 1)
					{
						 if( (( j + 1 ) <= 14 &&(i-1)<=0&& _manager.OmokBoard[i-1,j + 1 ] == 0)|| ( ( j - 4 ) >= 0&&(i+4)<=14 && _manager.OmokBoard[i + 4, j-4] == 0 )) 
                        {
                            StonePos( y, x );
                            return true;
                        } 
					}
				}
			}
		}
        return false;
    }
    bool CheckOpenTwo( int stone )
    {
        for( int i = 0 ; i < 15 ; i++ ) 
        {
            for( int j = 0 ; j < 15 ; j++ ) 
            {
                if( _manager.OmokBoard[i, j] == stone ) 
                {
                    //가로
                    if( (j+1)<=14&&_manager.OmokBoard[i, j + 1] == stone ) 
                    {
                        if( ( j - 1 ) >= 0 && ( j + 2 ) <= 14 && _manager.OmokBoard[i, j - 1] == 0 && _manager.OmokBoard[i, j + 2] == 0 ) 
                        {
                            StonePos(i, j-1);
                            return true;
                        }
                    }

                    //세로
                    if( (i+1)<=14&&_manager.OmokBoard[i+1, j ] == stone ) 
                    {
                        if( ( i - 1 ) >= 0 && ( i + 2 ) <= 14 && _manager.OmokBoard[i-1, j] == 0 && _manager.OmokBoard[i+2, j] == 0 ) 
                        {
                            StonePos(i-1, j);
                            return true;
                        }
                    }

                    //왼쪽대각선
                    if( (j+1)<=14&&(i+1)<=14&&_manager.OmokBoard[i+1, j + 1] == stone ) 
                    {
                        if( (i-1)>=0&&(i+2)<=14&&( j - 1 ) >= 0 && ( j + 2 ) <= 14 && _manager.OmokBoard[i-1 , j - 1] == 0 && _manager.OmokBoard[i+2, j + 2] == 0 ) 
                        {
                            StonePos(i-1, j-1);
                            return true;
                        }
                    }

                      //오른쪽대각선
                    if( (j-1)<=14&&(i+1)<=14&&_manager.OmokBoard[i+1, j - 1] == stone ) 
                    {
                        if( (i-1)>=0&&(i+2)<=14&&( j +1 ) <= 14 && ( j - 2 ) >= 0 && _manager.OmokBoard[i-1 , j + 1] == 0 && _manager.OmokBoard[i+2, j - 2] == 0 ) 
                        {
                            StonePos(i-1, j+1);
                            return true;
                        }
                    }

                }
            }
        }
        return false;
    }
    bool CheckCloseTwo( int stone )
    {
        for( int i = 0 ; i < 15 ; i++ ) 
        {
            for( int j = 0 ; j < 15 ; j++ ) 
            {
                if( _manager.OmokBoard[i, j] == stone ) 
                {
                    //가로
                    if( (j+1)<=14&&_manager.OmokBoard[i, j + 1] == stone ) 
                    {
                        if( ( j - 1 ) >= 0 && _manager.OmokBoard[i, j - 1] == 0 ) {
                            StonePos( i, j - 1 );
                            return true;
                        } else if( ( j + 2 ) <= 14 && _manager.OmokBoard[i, j + 2] == 0 ) 
                        {
                            StonePos( i, j + 2 );
                            return true;
                        }
                    }

                    //세로
                    if( (i+1)<=14&&_manager.OmokBoard[i+1, j ] == stone ) 
                    {
                        if( ( i - 1 ) >= 0 && _manager.OmokBoard[i - 1, j] == 0 ) {
                            StonePos( i - 1, j );
                            return true;
                        } else if( ( i + 2 ) <= 14 && _manager.OmokBoard[i + 2, j] == 0 ) 
                       {
                             StonePos( i + 2, j );
                            return true;
                        }
                    }

                    //왼쪽대각선
                    if( (j+1)<=14&&(i+1)<=14&&_manager.OmokBoard[i+1, j + 1] == stone ) 
                    {
                        if( ( i - 1 ) >= 0 && ( j - 1 ) >= 0  && _manager.OmokBoard[i - 1, j - 1] == 0  ) {
                            StonePos( i - 1, j - 1 );
                            return true;
                        } 
                        else if( ( i + 2 ) <= 14 && ( j + 2 ) <= 14&& _manager.OmokBoard[i + 2, j + 2] == 0) 
                        {
                            StonePos( i +2, j + 2 );
                            return true;
                        }
                    }

                      //오른쪽대각선
                    if( (j-1)<=14&&(i+1)<=14&&_manager.OmokBoard[i+1, j - 1] == stone ) 
                    {
                        if( ( i - 1 ) >= 0 && ( j + 1 ) <= 14 && _manager.OmokBoard[i - 1, j + 1] == 0  ) {
                            
                        } 
                        else if(( i + 2 ) <= 14&& ( j - 2 ) >= 0&& _manager.OmokBoard[i + 2, j - 2] == 0  ) 
                        {
                            StonePos( i + 2, j - 2 );
                            return true;
                        }
                    }

                }
            }
        }
        return false;
    }
    bool CheckOne( int stone )
    {
        for( int i = 0 ; i < 15 ; i++ ) 
        {
            for( int j = 0 ; j < 15 ; j++ ) 
            {
                if( _manager.OmokBoard[i, j] == stone ) 
                {
                    if( ( i - 1 ) >= 0 ) {
                        if( ( j - 1 ) >= 0 && _manager.OmokBoard[i - 1, j - 1] == 0 ) {
                            StonePos( i - 1, j - 1 );
                            return true;
                        } else if( _manager.OmokBoard[i - 1, j] == 0 ) {
                            StonePos( i - 1, j );
                            return true;
                        } else if( ( j + 1 ) <= 14 && _manager.OmokBoard[i - 1, j + 1] == 0 ) {
                            StonePos( i - 1, j + 1 );
                            return true;
                        }
                    } else if( ( i + 1 ) <= 14 ) {
                        if( ( j - 1 ) >= 0 && _manager.OmokBoard[i + 1, j - 1] == 0 ) {
                            StonePos( i + 1, j - 1 );
                            return true;
                        } else if( _manager.OmokBoard[i + 1, j] == 0 ) {
                            StonePos( i + 1, j );
                            return true;
                        } else if( ( j + 1 ) <= 14 && _manager.OmokBoard[i + 1, j + 1] == 0 ) {
                            StonePos( i + 1, j + 1 );
                            return true;
                        }
                    } else 
                    {
                         if( ( j - 1 ) >= 0 && _manager.OmokBoard[i, j - 1] == 0 ) {
                            StonePos( i , j - 1 );
                            return true;
                        } else if( ( j + 1 ) <= 14 && _manager.OmokBoard[i , j + 1] == 0 ) {
                            StonePos( i , j + 1 );
                            return true;
                        }
                    }
                }

            }
        }
        return false;
    }
    void PutButtom()
    {
        StonePos(7, 7);
    }

}


