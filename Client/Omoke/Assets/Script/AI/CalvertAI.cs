using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCPServer;
using System.Linq;



using System;
/// <summary>
/// Very simple block ai
/// </summary>

public class CalvertAI : MonoBehaviour {

    //Server is needed
    [SerializeField]
    string serverServiceTagName = "ServerService";
    ServerConnection server = null;

    //[SerializeField]
    //Vector2 rowAndCol = new Vector2(0, 0);

    [SerializeField]
    bool isAIturn = false;

    [SerializeField]
    bool isAIPlayer2 = false;
    [SerializeField]
    int botIndex = 0;
    [SerializeField]
    bool isMyTurn = false;

    [SerializeField]
    bool initialised = false;

    //System.DateTime current = System.DateTime.Now;

    List<int> historyOfMoves = new List<int>();

    float delay = 1;
    
    // Use this for initialization
    void Start () {
        server = GameObject.FindGameObjectWithTag(serverServiceTagName).GetComponent<ServerConnection>();
        if (server == null)
        {
            Debug.LogError("Cannot find Server: " + this.name);
            this.enabled = false;
            return;
        }
        UnityEngine.Random.InitState(Guid.NewGuid().GetHashCode());
    }
	
	// Update is called once per frame (Need do send to server)
	void Update ()
    {
        if (!initialised)
            return;



        if (server.GetWinner() != 0)
            this.enabled = false;
        if (!server.GetMyTurn() && isAIPlayer2)
        {
            delay -= Time.deltaTime;

            if (delay <= 0)
            {
                delay = 1;
                GetAIDecision();
            }
        }
        else if (!isAIPlayer2 && server.GetMyTurn())
        {
            delay -= Time.deltaTime;

            if (delay <= 0)
            {
                delay = 1;
                GetAIDecision();
            }
        }
    }


    //AI Decide where to put the tiles (Random number for now)
    private int GetAIDecision()
    {
        int[] board = server.GetMapData();
        int x, y;
        x = y = 0;
        int[] xOffsets = { -1, 0, 1, 1, 1, 0, -1, -1 };
        int[] yOffsets = { -1, -1, -1, 0, 1, 1, 1, 0 };

        //Get all opponent color position in board
        List<int>listOfOpponentPlacement = FindAllIndexOf(board, (botIndex == 1 ? 2 : 1));
        List<int> tempListCopy = new List<int>(listOfOpponentPlacement);
        Queue<int> removeablePositions = new Queue<int>();

        if (historyOfMoves.Any())
        {
            //Run through list to check where is the next available location to put a point
            foreach (int thePosition in historyOfMoves)
            {
                ConnectionClass.ConvertArrayPositionToXY(thePosition, out x, out y);
                for (int i = 0; i < 8; ++i)
                {
                    if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]), board.Length) &&
                        board[ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i])] == 0)
                    {
                        if (ForbiddenPointFinder.IsFive(x + xOffsets[i], y + yOffsets[i], board, botIndex))
                        {
                            server.SetMoveOnBoard(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                            historyOfMoves.Add(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                            Debug.Log("AI Found row of five at : " + ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                            return ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]);
                        }
                    }
                }
            }
        }

        while (tempListCopy.Any())
        {
            foreach(int thePosition in tempListCopy)
            {
                
                ConnectionClass.ConvertArrayPositionToXY(thePosition, out x, out y);

                removeablePositions.Enqueue(thePosition);
                if (ForbiddenPointFinder.IsFour(x, y, board, (botIndex == 1 ? 2 : 1)))
                {
                    Debug.Log("In is 4 (Check Enemy)");
                    int chosen = ForLoopCheckEmpty(x, y, 4, (botIndex == 1 ? 2 : 1), board, ref removeablePositions);
                    if (chosen != -1)
                    {
                        server.SetMoveOnBoard(chosen);
                        historyOfMoves.Add(chosen);
                        Debug.Log("AI Found row of four, Blocking at : " + chosen);
                        return chosen;
                    }
                }
            }

            while (removeablePositions.Count > 0)
                tempListCopy.Remove(removeablePositions.Dequeue());
        }

        tempListCopy.Clear();
        tempListCopy = new List<int>(listOfOpponentPlacement);

        //if reach here, no isfours were found. Check for isThrees
        while (tempListCopy.Any())
        {
            foreach (int thePosition in tempListCopy)
            {
                ConnectionClass.ConvertArrayPositionToXY(thePosition, out x, out y);

                removeablePositions.Enqueue(thePosition);
                if (ForbiddenPointFinder.IsThree(x, y, board, (botIndex == 1 ? 2 : 1)))
                {
                    Debug.Log("In is 3 (Check Enemy)");
                    int chosen = ForLoopCheckEmpty(x, y, 3, (botIndex == 1 ? 2 : 1), board, ref removeablePositions);
                    if (chosen != -1)
                    {
                        server.SetMoveOnBoard(chosen);
                        historyOfMoves.Add(chosen);
                        Debug.Log("AI Found row of three, Blocking at : " + chosen);
                        return chosen;
                    }
                }
            }

            while (removeablePositions.Count > 0)
                tempListCopy.Remove(removeablePositions.Dequeue());
        }

        //If reached here, means AI should place a piece, not defensively
        if (historyOfMoves.Any())
        {
            //Run through list to check where is the next available location to put a point
            foreach (int thePosition in historyOfMoves)
            {
                ConnectionClass.ConvertArrayPositionToXY(thePosition, out x, out y);
                for (int i = 0; i < 8; ++i)
                {
                    if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]), board.Length) &&
                        board[ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i])] == 0)
                    {
                        if (ForbiddenPointFinder.IsFour(x + xOffsets[i], y + yOffsets[i], board, botIndex))
                        {
                            server.SetMoveOnBoard(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                            historyOfMoves.Add(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                            Debug.Log("AI Found row of four at " + ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                            return ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]);
                        }
                        else if (ForbiddenPointFinder.IsThree(x + xOffsets[i], y + yOffsets[i], board, botIndex))
                        {
                            server.SetMoveOnBoard(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                            historyOfMoves.Add(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                            Debug.Log("AI Found row of three at : " + ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                            return ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]);
                        }
                    }
                }
            }

            //if not chain stones
            List<int> tempList = new List<int>(historyOfMoves);

            do
            {
                int locationPicked = tempList[(int)UnityEngine.Random.Range(0, historyOfMoves.Count - 1)];
                ConnectionClass.ConvertArrayPositionToXY(locationPicked, out x, out y);

                for (int i = 0; i < 8; ++i)
                {
                    if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]), board.Length) &&
                        board[ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i])] == 0)
                    {
                        server.SetMoveOnBoard(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                        historyOfMoves.Add(ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                        Debug.Log("AI Found row of two at " + ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]));
                        return ConnectionClass.ConvertXYPositionToIndex(x + xOffsets[i], y + yOffsets[i]);
                    }
                }

                tempList.Remove(locationPicked);

                //if (!tempList.Any())
                //    break;
            }
            while (tempList.Any());
        }

        int randLocation = 0;
        do
        {
            randLocation = UnityEngine.Random.Range(0, board.Length);
        }
        while (board[randLocation] != 0);
        Debug.Log("No availble placement, adding at random location : " + randLocation);
        server.SetMoveOnBoard(randLocation);
        historyOfMoves.Add(randLocation);
        return randLocation;

        //for(int i = 0; i < board.Length; ++i)
        //{
        //    if (board [i] == botIndex || board[i] == 0)
        //        continue;

        //    //If not your color
        //    ConnectionClass.ConvertArrayPositionToXY(i, out x, out y);
        //    //Check got isFour
        //    if(ForbiddenPointFinder.IsFour(x,y,board, (botIndex == 1 ? 2 : 1)))
        //    {

        //    }
        //}



        //Debug.Log("AI IS CHOOSING SPACE00");
        
        //int chosen = 0;
        //do
        //{
        //    chosen = UnityEngine.Random.Range(0, board.Length);
        //}
        //while (board[chosen] != 0);
        //Debug.Log("GetAIDecision: " + chosen);
        //server.SetMoveOnBoard(chosen);
        //historyOfMoves.Add(chosen);
        //return chosen;
    }

    public void initialiseTheBot(bool isBotTurn, int indexNumber, bool isBotPlayer2 = false)
    {
        initialised = true;
        isMyTurn = isBotTurn;
        botIndex = indexNumber;
        isAIPlayer2 = isBotPlayer2;
    }

    private int ForLoopCheckEmpty(int x, int y, int counts, int opponentColor, int[] board, ref Queue<int> theQueue)
    {
        //int tempX = x;
        //int tempY = y;
        //Left to right
        Queue<int> queueToReturn = new Queue<int>();

        for (int currCount = 0, tempX = x, tempY = y; currCount < counts && IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length) && board[ConnectionClass.ConvertXYPositionToIndex(tempX, tempY)] == opponentColor; ++currCount, ++tempX)
        {


            queueToReturn.Enqueue(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY));

            if(currCount == counts - 1)
            {
                while (queueToReturn.Count > 0)
                {
                    theQueue.Enqueue(queueToReturn.Dequeue());
                }

                tempX++;
                if(IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length))
                {
                    if(board[ConnectionClass.ConvertXYPositionToIndex(tempX, tempY)] == 0)
                    {
                        return ConnectionClass.ConvertXYPositionToIndex(tempX, tempY);
                    }
                }
                if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(x - 1, y), board.Length))
                {
                    if (board[ConnectionClass.ConvertXYPositionToIndex(x - 1, y)] == 0)
                    {
                        return ConnectionClass.ConvertXYPositionToIndex(x - 1, y);
                    }
                }
            }
        }

        //2nd direction. Diagonal Left to right
        for (int currCount = 0, tempX = x, tempY = y; currCount < counts && IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length) && board[ConnectionClass.ConvertXYPositionToIndex(tempX, tempY)] == opponentColor; ++currCount, ++tempX, ++tempY)
        {
            //if (!IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length))
            //    break;

            queueToReturn.Enqueue(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY));

            if (currCount == counts - 1)
            {
                while (queueToReturn.Count > 0)
                {
                    theQueue.Enqueue(queueToReturn.Dequeue());
                }

                tempX++;
                tempY++;
                if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length))
                {
                    if (board[ConnectionClass.ConvertXYPositionToIndex(tempX, tempY)] == 0)
                    {
                        return ConnectionClass.ConvertXYPositionToIndex(tempX, tempY);
                    }
                }
                if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(x - 1, y - 1), board.Length))
                {
                    if (board[ConnectionClass.ConvertXYPositionToIndex(x - 1, y - 1)] == 0)
                    {
                        return ConnectionClass.ConvertXYPositionToIndex(x - 1, y - 1);
                    }
                }
            }
        }

        //3rd Direction, down
        for (int currCount = 0, tempX = x, tempY = y; 
            currCount < counts && 
            IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length) && 
            board[ConnectionClass.ConvertXYPositionToIndex(tempX, tempY)] == opponentColor; 
            ++currCount, ++tempY)
        {
            //if (!IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length))
            //    break;

            queueToReturn.Enqueue(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY));

            if (currCount == counts - 1)
            {
                while (queueToReturn.Count > 0)
                {
                    theQueue.Enqueue(queueToReturn.Dequeue());
                }

                tempY++;
                if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length))
                {
                    if (board[ConnectionClass.ConvertXYPositionToIndex(tempX, tempY)] == 0)
                    {
                        return ConnectionClass.ConvertXYPositionToIndex(tempX, tempY);
                    }
                }
                if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(x, y - 1), board.Length))
                {
                    if (board[ConnectionClass.ConvertXYPositionToIndex(x, y - 1)] == 0)
                    {
                        return ConnectionClass.ConvertXYPositionToIndex(x, y - 1);
                    }
                }
            }
        }

        //4th direction, diagonal right to left
        for (int currCount = 0, tempX = x, tempY = y; currCount < counts && IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length) && board[ConnectionClass.ConvertXYPositionToIndex(tempX, tempY)] == opponentColor; ++currCount, --tempX, ++tempY)
        {
            //if (!IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length))
            //    break;

            queueToReturn.Enqueue(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY));

            if (currCount == counts - 1)
            {
                while (queueToReturn.Count > 0)
                {
                    theQueue.Enqueue(queueToReturn.Dequeue());
                }

                tempX--;
                tempY++;
                if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(tempX, tempY), board.Length))
                {
                    if (board[ConnectionClass.ConvertXYPositionToIndex(tempX, tempY)] == 0)
                    {
                        return ConnectionClass.ConvertXYPositionToIndex(tempX, tempY);
                    }
                }
                if (IsInBoard(ConnectionClass.ConvertXYPositionToIndex(x + 1, y - 1), board.Length))
                {
                    if (board[ConnectionClass.ConvertXYPositionToIndex(x + 1, y - 1)] == 0)
                    {
                        return ConnectionClass.ConvertXYPositionToIndex(x + 1, y - 1);
                    }
                }
            }
        }

        return -1;
    }

    private bool IsInBoard(int arrayPosition, int boardlength)
    {
        return (arrayPosition >= 0 && arrayPosition < boardlength);
    }

    //static int[] FindAllIndexOf<T>(this IEnumerable<T> value, T val)
    //{
    //    return Enumerable.Range(0, value).Where(i => theArray[i] == val).ToArray();
    //}

    static List<int> FindAllIndexOf(int[] theArray, int val)
    {
        return Enumerable.Range(0, theArray.Length).Where(i => theArray[i] == val).ToList();
    }

}
