using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EightQueens : MonoBehaviour
{
    public GameObject CubePrefab;
    public bool UseQuickCheck;
    private GameObject[] _chessboardGameObjects;
    private int[] _queenPosition = new int[8];
    private int[] _queenQuickPosition = new int[8];
    private int _currentQuickIndex = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _chessboardGameObjects = new GameObject[8 * 8];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                _chessboardGameObjects[x * 8 + y] = Instantiate(CubePrefab, new Vector3(-3.5f + x, 0, 3.5f - y), Quaternion.identity);
                _chessboardGameObjects[x * 8 + y].GetComponent<Renderer>().material.color = (x + y) % 2 == 0 ? Color.black : Color.white;
            }

            _queenPosition[x] = 0;
            _queenQuickPosition[x] = -1;
        }
    }

    void GetNextQueenPosition(ref int[] queenPosition)
    {
        for (int index = 7; index >= 0; index--)
        {
            queenPosition[index] += 1;
            if (queenPosition[index] > 7)
            {
                queenPosition[index] = 0;
            }
            else
            {
                return;
            }
        }
    }

    bool CheckQueenPosition(int[] queenPosition)
    {
        int quickcheck = 0;
        for (int index = 0; index < 8; index++)
        {
            if (queenPosition[index] == -1)
            {
                return false;
            }
            if ((quickcheck & 1 << queenPosition[index]) != 0 )
            {
                return false;
            }

            quickcheck |=  1 << queenPosition[index];
            for (int checkIndex = index - 1; checkIndex >= 0; checkIndex--)
            {
                if (queenPosition[checkIndex] == queenPosition[index] - (index - checkIndex) ||
                    queenPosition[checkIndex] == queenPosition[index] + (index - checkIndex))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void OnNextClick()
    {
        if (UseQuickCheck)
        {
            GetNextStep(ref _queenQuickPosition);
            UpdateQueens(_queenQuickPosition);
            CheckQueenPosition(_queenQuickPosition);
        }
        else
        {
            UpdateQueens(_queenPosition);
            CheckQueenPosition(_queenPosition);
            GetNextQueenPosition(ref _queenPosition);
        }
        
    }

    public void OnAutoClick()
    {
        StartCoroutine(DoAutoNext());
    }

    private IEnumerator DoAutoNext()
    {
        bool isFound = false;
        while (!isFound)
        {
            if (UseQuickCheck)
            {
                isFound = GetNextStep(ref _queenQuickPosition);
                UpdateQueens(_queenQuickPosition);
                CheckQueenPosition(_queenQuickPosition);
            }
            else
            {
                UpdateQueens(_queenPosition);
                isFound = CheckQueenPosition(_queenPosition);
                GetNextQueenPosition(ref _queenPosition);
            }
            yield return null;
        }
    }

    private void UpdateQueens(int[] queenPosition)
    {
        for (int index = 0; index < _chessboardGameObjects.Length; index++)
        {
            _chessboardGameObjects[index].GetComponent<Queen>().ModifyQueen(queenPosition[index / 8] != -1 && queenPosition[index / 8] == index % 8);
        }
    }

    private int FFS(byte flag)
    {
        return (int)Math.Log(flag & ~(flag - 1), 2);
    }

    private bool CheckCross(int posX, int posY, int[] queenPosition)
    {
        for (int distance = 1; distance <= posY; distance++)
        {
            if (posX - distance == queenPosition[posY - distance] || posX + distance == queenPosition[posY - distance])
            {
                return false;
            }
        }

        return true;
    }

    private bool FindNextPos(int StartPos, int posY, byte flag, int[] queenPosition, out int pos)
    {
        while (true)
        { 
            pos = FFS((byte)~flag);
            if (pos > StartPos && CheckCross(pos, posY, queenPosition))
            {
                Debug.Log(pos);
                return true;
            }
            flag |= (byte)(1 << pos);
            
            if (flag == 255)
            {
                pos = -1;
                Debug.Log("Not found!");
                return false;
            }
        }
    }
    
    
    private bool GetNextStep(ref int[] queenPosition)
    {
        byte quickcheck = 0;
        foreach (var t in queenPosition)
        {
            if (t != -1)
            {
                quickcheck |= (byte)(1 << t);
            }
            else
            {
                break;
            }
        }

        if (!FindNextPos(queenPosition[_currentQuickIndex], _currentQuickIndex, quickcheck, queenPosition,
                out queenPosition[_currentQuickIndex]))
        {
            _currentQuickIndex -= 1;
            return false;
        }

        if (_currentQuickIndex == queenPosition.Length - 1) 
        {
            return true;
        }

        _currentQuickIndex++;
        return false;
    }
}
