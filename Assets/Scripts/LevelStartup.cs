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
    public int marchX = 0;
    public int marchY = 0;
    public int marchVal = 0;
    public Camera sceneCam;
    public Material straightPipe;
    public Material cornerPipe;
    public Material endPointPipe;
    public Material pipeBlank;

    // Start is called before the first frame update
    void Start()
    {
        sceneCam.gameObject.transform.position = new Vector3(gridSize/2f-.5f,gridSize/2f-.5f,-10);
        var objects = new GameObject[gridSize,gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y< gridSize; y++)
            {
                objects[x,y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                objects[x,y].gameObject.transform.position = new Vector3(x,y,0);
                objects[x,y].GetComponent<Renderer>().material = pipeBlank;
            }
        }
        List<Vector2Int> path = new List<Vector2Int>();
        int sway = Random.Range(0, 10);
        marchX = startX;
        marchY = startY;
        path.Add(new Vector2Int(marchX,marchY));
        while (marchX != endX || marchY != endY)
        {
            int signX = Math.Sign(endX-marchX);
            int signY = Math.Sign(endY-marchY);
            sway = Random.Range(0, 10);
            int coin = Random.Range(0,2);
            if (coin == 0 && signX != 0) // if coinflip says x and can move in x direction
            {
                marchX += signX;
                marchVal += 1;
            }
            else if(coin == 0 && signX == 0) // if coinflip says x but cannot move in x direction
            {
                marchY += signY;
                marchVal += 1;
            }
            else if (coin == 1 && signY != 0) // if coinflip says Y and can move in y direction
            {
                marchY += signY;
                marchVal += 1;
            }
            else if (coin == 1 && signY == 0) // if coinflip says Y and cannot move in Y direction
            {
                marchX += signX;
                marchVal += 1;
            }
            path.Add(new Vector2Int(marchX,marchY));
        }
        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0 || i == path.Count-1)
            {
                objects[path[i].x,path[i].y].GetComponent<Renderer>().material = endPointPipe;
            }
            else if (path[i].x == path[i-1].x +1 && path[i].x == path[i+1].x -1)
            {
                objects[path[i].x,path[i].y].GetComponent<Renderer>().material = straightPipe;
            }
            else if (path[i].y == path[i-1].y +1 && path[i].y == path[i+1].y -1)
            {
                objects[path[i].x,path[i].y].GetComponent<Renderer>().material = straightPipe;
            }
            else
            {
                objects[path[i].x,path[i].y].GetComponent<Renderer>().material = cornerPipe;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}