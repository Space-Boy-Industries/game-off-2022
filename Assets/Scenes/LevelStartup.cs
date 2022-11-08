using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class LevelStartup : MonoBehaviour
{
    public int gridSize = 4;
    public int startX = 0;
    public int startY = 0;
    public int endX = 3;
    public int endY = 3;
    public int pathRand = 10;
    public int marchX = 0;
    public int marchY = 0;
    public Camera sceneCam;

    // Start is called before the first frame update
    void Start()
    {
        sceneCam.gameObject.transform.position = new Vector3(gridSize/2f-.5f,gridSize/2f-.5f,-10);
        List<List<int>> mTable = new List<List<int>>();
        List<List<GameObject>> objects = new List<List<GameObject>>();
        for (int i = 0; i < gridSize; i++)
        {
            mTable.Add(new List<int>());
            objects.Add(new List<GameObject>());
            for (int x = 0; x < gridSize; x++)
            {
                mTable[i].Add(0);
                objects[i].Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
                objects[i][x].transform.position = new Vector3(i,x,0);
            }
        }
        int sway = Random.Range(0, 10);
        marchX = startX;
        marchY = startY;
        objects[marchX][marchY].GetComponent<Renderer>().material.color = Color.red;
        while (marchX != endX || marchY != endY)
        {
            int signX = Math.Sign(endX-marchX);
            int signY = Math.Sign(endY-marchY);
            sway = Random.Range(0, 10);
            mTable[startX].RemoveAt(startY);
            mTable[startX].Insert(startY,1);
            int coin = Random.Range(0,2);
            if (sway <= pathRand)
            {
                if (coin == 0 && signX != 0) // if coinflip says x and can move in x direction
                {
                    marchX += signX;
                    Debug.Log(marchX + " "+ marchY);
                    mTable[marchX].RemoveAt(marchY);
                    mTable[marchX].Insert(marchY,1);
                }
                else if(coin == 0 && signX == 0) // if coinflip says x but cannot move in x direction
                {
                    marchY += signY;
                    Debug.Log(marchX + " "+ marchY);
                    mTable[marchX].RemoveAt(marchY);
                    mTable[marchX].Insert(marchY,1);
                }
                else if (coin == 1 && signY != 0) // if coinflip says Y and can move in y direction
                {
                    marchY += signY;
                    Debug.Log(marchX + " "+ marchY);
                    mTable[marchX].RemoveAt(marchY);
                    mTable[marchX].Insert(marchY,1);
                }
                else if (coin == 1 && signY == 0) // if coinflip says Y and cannot move in Y direction
                {
                    marchX += signX;
                    Debug.Log(marchX + " "+ marchY);
                    mTable[marchX].RemoveAt(marchY);
                    mTable[marchX].Insert(marchY,1);
                }
                objects[marchX][marchY].GetComponent<Renderer>().material.color = Color.red;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}