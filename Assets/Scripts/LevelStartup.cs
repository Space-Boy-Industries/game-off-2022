using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

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
    public Sprite straightPipe;
    public Sprite cornerPipe;
    public Sprite endPointPipe;
    public Sprite pipeBlank;
    public List<float[]> solution = new List<float[]>();

    // Start is called before the first frame update
    void Start()
    {
        sceneCam.gameObject.transform.position = new Vector3(gridSize/2f-.5f,gridSize/2f-.5f,-10);
        var objects = new GameObject[gridSize,gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y< gridSize; y++)
            {
                objects[x,y] = new GameObject();
                objects[x,y].AddComponent<SpriteRenderer>();
                objects[x,y].gameObject.transform.position = new Vector3(x,y,0);
                objects[x,y].GetComponent<SpriteRenderer>().sprite = pipeBlank;
                objects[x,y].AddComponent<BoxCollider>();
                int rot = Random.Range(0, 4);
                objects[x,y].gameObject.transform.rotation = Quaternion.Euler(0,0,rot*90);
            }
        }
        List<Vector2Int> path = new List<Vector2Int>();
        marchX = startX;
        marchY = startY;
        path.Add(new Vector2Int(marchX,marchY));
        while (marchX != endX || marchY != endY)
        {
            int signX = Math.Sign(endX-marchX);
            int signY = Math.Sign(endY-marchY);
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
                objects[path[i].x,path[i].y].GetComponent<SpriteRenderer>().sprite = endPointPipe;
                if (objects[path[i].x+1,path[i].y] && path[i].x+1 < gridSize) // right
                {
                    solution.Add(new float[] {90});
                }
                else if (objects[path[i].x-1,path[i].y] && path[i].x-1 > 0) // left
                {
                    solution.Add(new float[] {270});
                }
                else if (objects[path[i].x,path[i].y+1] && path[i].y+1 < gridSize) // up
                {
                    solution.Add(new float[] {0});
                }
                else if (objects[path[i].x-1,path[i].y] && path[i].y-1 > 0) // down
                {
                    solution.Add(new float[] {180});
                }
            }
            else if (path[i].x == path[i-1].x +1 && path[i].x == path[i+1].x -1)
            {
                objects[path[i].x,path[i].y].GetComponent<SpriteRenderer>().sprite = straightPipe;
                solution.Add(new float[] {90,180});
            }
            else if (path[i].y == path[i-1].y +1 && path[i].y == path[i+1].y -1)
            {
                objects[path[i].x,path[i].y].GetComponent<SpriteRenderer>().sprite = straightPipe;
                solution.Add(new float[] {0,270});
            }
            else
            {
                objects[path[i].x,path[i].y].GetComponent<SpriteRenderer>().sprite = cornerPipe;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
         if (Mouse.current.leftButton.wasPressedThisFrame)
        {
             RaycastHit hit;
             var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
             
             if (Physics.Raycast(ray, out hit)) 
             {
                var obj = hit.collider.gameObject;
                obj.transform.rotation = Quaternion.Euler(0, 0, obj.transform.rotation.eulerAngles.z - 90);
             }
        }     
    }
}