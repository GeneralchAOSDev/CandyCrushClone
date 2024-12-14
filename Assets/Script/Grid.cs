using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            //////UnityEngine.Debug.Log("Icon Type " + r + " at Position : " + x + ", " + y);
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
            ////UnityEngine.Debug.Log(" x = " + x);
            for (int x = 0; x < 6; x++)
            {
                ////UnityEngine.Debug.Log(" y = " + y);
                
                ////UnityEngine.Debug.Log(" i = " + i + "x,y = " + x + "," + y);
                SpriteRenderer spriteRenderer = icons[i].GetComponent<SpriteRenderer>();
                i++;
                int r = map[x, y];
                spriteRenderer.sprite = sprites[r];
            }
        }

        //stopwatch.Stop();
        ////UnityEngine.Debug.Log("Grid Updated in " + stopwatch.Elapsed.TotalSeconds + "s");
        //stopwatch.Reset();
    }
    public void debugMap()
    {
        string dMap = "";
        for (int y = 0; y < 8; y++)
        {
            dMap += "\n";
            for (int x = 0; x < 6; x++)
            {
                dMap += map[x, y];
                dMap += " ";
            }
        }

       // UnityEngine.Debug.Log(dMap);
    }
    public void CheckMap()
    {
        Array.Clear(match, 0, match.Length); 
        matNum = 0;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        debugMap();

        for (int y = 0; y < 8; y++)
        {
            int num = -1;
            for (int x = 0; x < 6; x++)
            {
                ////UnityEngine.Debug.Log("Looking at position (" + x + "," + y + ")");
                if (num == map[x, y])
                {
                    int count = 1;
                    while(num == map[x, y])
                    {
                        count++;
                        x++;
                        if (x >= 6)
                            break;
                        ////UnityEngine.Debug.Log(count + " matching in a row");
                    }
                    if (count >= 3)
                    {
                        ////UnityEngine.Debug.Log(matNum + " horizontal matches recorded");
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
            int num = -1;
            for (int y = 0; y < 8; y++)
            {
               // //UnityEngine.Debug.Log("Looking at position (" + x + "," + y + ")");
                if (num == map[x, y])
                {
                    int count = 1;
                    while (num == map[x, y])
                    {
                        count++;
                        y++;
                        if (y >= 8)
                            break;
                       // //UnityEngine.Debug.Log(count + " matching in a row");
                    }
                    if (count >= 3)
                    {
                        ////UnityEngine.Debug.Log(matNum + " matches recorded");
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
            ////UnityEngine.Debug.Log("Code for match number " + (d + 1) + ": " + match[d]);
        }

        Array.Sort(match);
        Array.Reverse(match);


        
        stopwatch.Stop();
        ////UnityEngine.Debug.Log("Grid Updated in " + stopwatch.Elapsed.TotalSeconds + "s");
        ////UnityEngine.Debug.Log("Number of Matches found: " + matNum );
        for(int d = 0 ; d < matNum; d++)
        {
            ////UnityEngine.Debug.Log("Code for match number "+(d + 1)+": " + match[d]);
        }
        stopwatch.Reset();
        if(matNum > 0)
        {
           // //UnityEngine.Debug.Log("Clearing " + matNum + " matches.");
            clearMatch();
        }
        
    }

    public void clearMatch()
    {
        UnityEngine.Debug.Log("Number of matches = " + matNum);
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
            //UnityEngine.Debug.Log("Match Number = " + (d + 1));
            //string direct = (dir == 2) ? "Verticle" : "Horizontal";
            //UnityEngine.Debug.Log("Direction = " + direct);
            //UnityEngine.Debug.Log("Length = " + length);
            //UnityEngine.Debug.Log("x coord = " + x);
            //UnityEngine.Debug.Log("y coord = " + y);
            if (dir == 2) // Vertical match
            {
                // Start from the matched row and move up
                for (int p = y; p >= 0; p--)
                {
                    if (p - length >= 0)
                    {
                        // Move icons down from above
                        map[x, p] = map[x, p - length];
                    }
                    else
                    {
                        // Generate new icons for top rows
                        map[x, p] = UnityEngine.Random.Range(0, 9);
                    }
                }
            }

            if (dir == 1) // Horizontal match
            {
                for (int p = y; p > 0; p--)
                {
                    for (int q = x; q > x - length; q--)
                    {
                        map[q, p] = map[q, p - 1];
                    }
                }
                for (int q = x; q > x - length; q--)
                {
                    map[0, q] = UnityEngine.Random.Range(0, 9);
                }
            }
        }
        matNum = 0;
        CheckMap();
    }


    public void Swap(int startX, int startY, int tarX, int tarY)
    {

        ////UnityEngine.Debug.Log("The starting coords (x,y) are: " + startX + "," + startY + ". And the target coords are: " + tarX + "," + tarY);
        int temp;
        
        temp = map[startX, startY];

        ////UnityEngine.Debug.Log("Temp, Start and Target values are: " +  temp + " ," +  map[startX, startY] + " ," + map[tarX, tarY]);

        map[startX, startY] = map[tarX, tarY];
        map[tarX, tarY] = temp;
        CheckMap();
        UpdateMap();
       
    }
}
