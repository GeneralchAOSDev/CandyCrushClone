using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Runtime.CompilerServices;
using UnityEditor.SceneTemplate;
using UnityEngine;


public class Grid : MonoBehaviour
{
    public GameObject[] icons;
    public Sprite[] sprites;
    int[] match = new int[16];
    int matNum = 0;
    //private int[,] = new map[6, 8];
    public int[,] map = new int[6, 8];
    

    void Start()
    {
        int y = -1;
        int x = 0;
        for (int i = 0; i < icons.Length; i++)
        {
            x++;
            SpriteRenderer spriteRenderer = icons[i].GetComponent<SpriteRenderer>();
            int r = UnityEngine.Random.Range(0, 9);
            spriteRenderer.sprite = sprites[r];
            if(i%6 == 0)
            {
                y++;
                x = (x/6);
                if (x != 0)
                {
                    //y--;
                    x--;
                }
            }

            //Debug.Log("x = " + x + " y = " + y + " i = " + i);

            UnityEngine.Debug.Log("Icon Type " + r + " at Position : " + x + ", " + y);
            map[x, y] = r;
        }

        CheckMap();

    }

    void UpdateMap()
    {
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();

        int i = 0;
        for (int y = 0; y < 8; y++)
        {
            //UnityEngine.Debug.Log(" x = " + x);
            for (int x = 0; x < 6; x++)
            {
                //UnityEngine.Debug.Log(" y = " + y);
                
                //UnityEngine.Debug.Log(" i = " + i + "x,y = " + x + "," + y);
                SpriteRenderer spriteRenderer = icons[i].GetComponent<SpriteRenderer>();
                i++;
                int r = map[x, y];
                spriteRenderer.sprite = sprites[r];
            }
        }

        //stopwatch.Stop();
        //UnityEngine.Debug.Log("Grid Updated in " + stopwatch.Elapsed.TotalSeconds + "s");
        //stopwatch.Reset();
    }

    public void CheckMap()
    {
        Array.Clear(match, 0, match.Length); 
        matNum = 0;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        int num = -1; 

        for (int y = 0; y < 8; y++)
        {
                
            for (int x = 0; x < 6; x++)
            {
                UnityEngine.Debug.Log("Looking at position (" + x + "," + y + ")");
                if (num == map[x, y])
                {
                    int count = 1;
                    while(num == map[x, y])
                    {
                        count++;
                        x++;
                        if (x >= 6)
                            break;
                        UnityEngine.Debug.Log(count + " matching in a row");
                    }
                    if (count >= 3)
                    {
                        UnityEngine.Debug.Log(matNum + " matches recorded");
                        match[matNum] = (count * 1000) + (100 * (x - 1)) + (10 * y) + 1; // recording a code to represent the location of the match
                        matNum++;   // 1000 is left to right, 100 * x represents the x coordinate, 
                                    // 10 * y represents the y coordinate and count is how many are in the row
                    }
                }
                else
                {
                    num = map[x, y];
                }
            }
        }

        for (int x = 0; x < 6; x++)
        {

            for (int y = 0; y < 8; y++)
            {
                UnityEngine.Debug.Log("Looking at position (" + x + "," + y + ")");
                if (num == map[x, y])
                {
                    int count = 1;
                    while (num == map[x, y])
                    {
                        count++;
                        y++;
                        if (y >= 8)
                            break;
                        UnityEngine.Debug.Log(count + " matching in a row");
                    }
                    if (count >= 3)
                    {
                        UnityEngine.Debug.Log(matNum + " matches recorded");
                        match[matNum] = (count*1000) + (100 * x) + (10 * (y-1)) + 2; // recording a code to represent the location of the match
                        matNum++;   // 2000 is up to down, 100 * x represents the x coordinate, 
                                    // 10 * y represents the y coordinate and count is how many are in the row
                    }
                }
                else
                {
                    num = map[x, y];
                }
            }
        }

        for (int d = 0; d < matNum; d++)
        {
            UnityEngine.Debug.Log("Code for match number " + (d + 1) + ": " + match[d]);
        }

        Array.Sort(match);
        Array.Reverse(match);


        
        stopwatch.Stop();
        UnityEngine.Debug.Log("Grid Updated in " + stopwatch.Elapsed.TotalSeconds + "s");
        UnityEngine.Debug.Log("Number of Matches found: " + matNum );
        for(int d = 0 ; d < matNum; d++)
        {
            UnityEngine.Debug.Log("Code for match number "+(d + 1)+": " + match[d]);
        }
        stopwatch.Reset();
        if(matNum > 0)
        {
            UnityEngine.Debug.Log("Clearing " + matNum + " matches.");
            clearMatch();
        }
        
    }

    public void clearMatch()
    {
        // take the match code and find out with columns to move down
        // find if its vert or horizontal
        // drop it by the count if vertical, drop by 1 if horizontal
        // generate random icons for the empty grids

        for (int d = 0; d < matNum; d++)
        {
            int temp, x, y, length, dir;
            temp = match[d];
            dir = temp % 10;
            temp /= 10;
            y = temp % 10;
            temp /= 10;
            x = temp % 10;
            temp /= 10;
            length = temp % 10;
            UnityEngine.Debug.Log("DEBUG: dir x y length: " + dir + " " + x + " " + y + " " + length);

            if (dir == 2) // if true it's verticle
            {
                for (int p = y; p > 0; p--)
                {
                    UnityEngine.Debug.Log("DEBUG: p = " + p);
                    if (p - length >= 0)
                    {
                        UnityEngine.Debug.Log("DEBUG: p minus length = " + (p - length));
                        map[x, p] = map[x, p - length];
                    }
                    else
                    {
                        map[x, p] = UnityEngine.Random.Range(0, 9);
                    }
                }
            }
            if (dir == 1)
            {
                for (int p = y; p > 0; p--)
                {
                    for (int g = x; g > 0; g--)
                    {
                        UnityEngine.Debug.Log("DEBUG: p = " + p + " , g = " + g);
                        if (p > 0 && g > 0)
                            map[g, p] = map[g, p - 1];
                        else
                            map[g, p] = UnityEngine.Random.Range(0, 9);
                    }
                }
            }
        }
        CheckMap();
    }


    public void Swap(int startX, int startY, int tarX, int tarY)
    {

        UnityEngine.Debug.Log("The starting coords (x,y) are: " + startX + "," + startY + ". And the target coords are: " + tarX + "," + tarY);
        int temp;
        
        temp = map[startX, startY];

        //UnityEngine.Debug.Log("Temp, Start and Target values are: " +  temp + " ," +  map[startX, startY] + " ," + map[tarX, tarY]);

        map[startX, startY] = map[tarX, tarY];
        map[tarX, tarY] = temp;
        CheckMap();
        UpdateMap();
       
    }
}
