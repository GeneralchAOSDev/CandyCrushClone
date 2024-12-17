using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Matching
{
    // Fields (variables)
    public int temp, x, y, length;
    public bool vert;

    // Constructor
    public Matching(int x, int y, int length, bool vert)
    {
        this.x = x;
        this.y = y;
        this.length = length;
        this.vert = vert;
    }
}
public class Grid : MonoBehaviour
{

    const int COL = 6;
    const int ROW = 8;
    public int difficulty = 3;
    public GameObject[] icons;
    public Sprite[] sprites;
    int[] match = new int[16];
    int matNum = 0;
    //private int[,] = new map[COL, ROW];
    public int[,] map = new int[COL, ROW];
    List<string> dropAnim = new List<string>();
    List<Matching> popAnim = new List<Matching>();
    void Start()
    {
        DOTween.SetTweensCapacity(1250,50);
        int y = -1;
        int x = 0;
        for (int i = 0; i < icons.Length; i++)
        {
            x++;
            SpriteRenderer spriteRenderer = icons[i].GetComponent<SpriteRenderer>();
            int r = UnityEngine.Random.Range(0, difficulty);
            spriteRenderer.sprite = sprites[r];
            if(i%COL == 0)
            {
                y++;
                x = (x/COL);
                if (x != 0)
                {
                    //y--;
                    x--;
                }
            }

            //Debug.Log("x = " + x + " y = " + y + " i = " + i);

            ////////UnityEngine.Debug.Log("Icon Type " + r + " at Position : " + x + ", " + y);
            map[x, y] = r;
        }

        CheckMap();

    }

    void UpdateMap()
    {
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();

        toAnimate();
        int i = 0;
        for (int y = 0; y < ROW; y++)
        {
            //////UnityEngine.Debug.Log(" x = " + x);
            for (int x = 0; x < COL; x++)
            {
                //////UnityEngine.Debug.Log(" y = " + y);
                
                //////UnityEngine.Debug.Log(" i = " + i + "x,y = " + x + "," + y);
                SpriteRenderer spriteRenderer = icons[i].GetComponent<SpriteRenderer>();
                i++;
                int r = map[x, y];
                spriteRenderer.sprite = sprites[r];
            }
        }

        //stopwatch.Stop();
        //////UnityEngine.Debug.Log("Grid Updated in " + stopwatch.Elapsed.TotalSeconds + "s");
        //stopwatch.Reset();
    }
    public void debugMap()
    {
        string dMap = "";
        for (int y = 0; y < ROW; y++)
        {
            dMap += "\n";
            for (int x = 0; x < COL; x++)
            {
                dMap += map[x, y];
                dMap += " ";
            }
        }

        //UnityEngine.Debug.Log(dMap);
    }
    public void CheckMap()
    {
        Array.Clear(match, 0, match.Length); 
        matNum = 0;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        debugMap();

        for (int y = 0; y < ROW; y++)
        {
            int num = -1;
            for (int x = 0; x < COL; x++)
            {
                ////UnityEngine.Debug.Log("Looking at position (" + x + "," + y + ")");
                if (num == map[x, y])
                {
                    int count = 1;
                    while(num == map[x, y])
                    {
                        
                        count++;
                        x++;
                        if (x >= COL)
                            break;
                        ////UnityEngine.Debug.Log(count + " matching in a row");
                    }
                    if (count >= 3)
                    {
                        //////UnityEngine.Debug.Log(matNum + " horizontal matches recorded");
                        match[matNum] = (count * 1000) + (100 * (x - 1)) + (10 * y) + 1; // recording a code to represent the location of the match
                        matNum++;   // 1000 is left to right, 100 * x represents the x coordinate, 
                                    // 10 * y represents the y coordinate and count is how many are in the row
                    }
                    if(count == 2 && x < COL)
                    {
                        num = map[x, y];
                    }
                }
                else
                {
                    num = map[x, y];
                }
            }
        }
        for (int x = 0; x < COL; x++)
        {
            int num = -1;
            for (int y = 0; y < ROW; y++)
            {
               ////UnityEngine.Debug.Log("Looking at position (" + x + "," + y + ")");
                if (num == map[x, y])
                {
                    int count = 1;
                    while (num == map[x, y])
                    {
                        count++;
                        y++;
                        if (y >= ROW)
                            break;
                       // ////UnityEngine.Debug.Log(count + " matching in a row");
                    }
                    if (count >= 3)
                    {
                        //////UnityEngine.Debug.Log(matNum + " matches recorded");
                        match[matNum] = (count*1000) + (100 * x) + (10 * (y-1)) + 2; // recording a code to represent the location of the match
                        matNum++;   // 2000 is up to down, 100 * x represents the x coordinate, 
                                    // 10 * y represents the y coordinate and count is how many are in the row
                    }
                    if(count == 2 && y < ROW)
                    {
                        num = map[x, y];
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
            //////UnityEngine.Debug.Log("Code for match number " + (d + 1) + ": " + match[d]);
        }

        Array.Sort(match);
        Array.Reverse(match);


        
        stopwatch.Stop();
        //////UnityEngine.Debug.Log("Grid Updated in " + stopwatch.Elapsed.TotalSeconds + "s");
        //////UnityEngine.Debug.Log("Number of Matchinges found: " + matNum );
        for(int d = 0 ; d < matNum; d++)
        {
            //////UnityEngine.Debug.Log("Code for match number "+(d + 1)+": " + match[d]);
        }
        stopwatch.Reset();
        if(matNum > 0)
        {
           // ////UnityEngine.Debug.Log("Clearing " + matNum + " matches.");
            clearMatching();
        }
    }
    IEnumerator ResetSpriteAfterAnimation(GameObject icon, int x, int y)
    {
        Animator ani = icon.GetComponent<Animator>();
        UnityEngine.Debug.Log("Waiting....");
        // Wait for the animation to complete
        yield return new WaitForSeconds(1);
        UnityEngine.Debug.Log("Wait Over");
        ani.enabled = false;
        UpdateMap();
    }
    public void toAnimate()
    {
        List<GameObject> list = new List<GameObject>();
        int index = 0;
        foreach (var anim in popAnim)
        {
            for (int i = 0; i < anim.length; i++)
            {
                int tempX = anim.x;
                int tempY = anim.y;

                if (!anim.vert) tempX -= i;
                if (anim.vert) tempY -= i;
                int xLoc = tempX;
                xLoc = xLoc + (tempY * COL);
                GameObject lookup = GameObject.Find($"bolt ({xLoc})");
                if (lookup == null)
                    UnityEngine.Debug.Log($"bolt ({xLoc}) not found!");
                Animator ani = lookup.GetComponent<Animator>();
                if (ani != null)
                {
                    //UnityEngine.Debug.Log("DEBUG Animation: (obj, x, y) = (" + lookup.name + ", "+ tempX + ", " +tempY + ")");
                    ani.enabled = true;
                    ani.SetInteger("iconType", map[tempX,tempY]);
                    StartCoroutine( ResetSpriteAfterAnimation(lookup, tempX, tempY));
                }
            }   
        }
        popAnim.Clear();
            foreach (var anim in dropAnim)
        {
            // Use string interpolation to construct the name
            GameObject obj = GameObject.Find(anim);

            if (obj != null)
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    list.Add(obj);
                    index++;
                    //renderer.color = Color.red;
                }

                //UnityEngine.Debug.Log($"Found GameObject: {obj.name}");
            }
        }
        foreach (var twee in list)
        {
            // Store the original Y position
            float originalY = twee.transform.position.y;

            // Move up by 1 unit, then return to the original position
            twee.transform.DOMoveY(originalY + 1f, 0.0001f) // Move up by 1 unit over 0.5 seconds
                .OnComplete(() =>
                {
                    // Once the upward motion is complete, move back to the original position
                    twee.transform.DOMoveY(originalY, 0.5f);
                });
        }
        dropAnim.Clear();
    }
    public void clearMatching()
    {
        //UnityEngine.Debug.Log("Number of matches = " + matNum);
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
            bool vert = (dir == 2) ? true : false;
            Matching pop = new Matching(x, y, length, vert);
            popAnim.Add(pop);
            if (dir == 2) // Vertical match
            {

                // Start from the matched row and move up
                for (int p = y; p >= 0; p--)
                {
                    int xLoc = x;
                    xLoc = xLoc + (p * COL);
                    // Use string interpolation to construct the name
                    dropAnim.Add($"bolt ({xLoc})");

                    if (p - length >= 0)
                    {
                        // Move icons down from above
                        map[x, p] = map[x, p - length];
                    }
                    else
                    {
                        // Generate new icons for top rows
                        map[x, p] = UnityEngine.Random.Range(0, difficulty);
                    }
                }
            }

            if (dir == 1) // Horizontal match
            {
                for (int p = y; p > 0; p--)
                {
                    for (int q = x; q > x - length; q--)
                    {
                        int xLoc = q;
                        xLoc = xLoc + (p * COL);
                        // Use string interpolation to construct the name
                        dropAnim.Add($"bolt ({xLoc})");
                        map[q, p] = map[q, p - 1];
                    }
                }
                for (int q = x; q > x - length; q--)
                {
                    int xLoc = q;
                    // Use string interpolation to construct the name
                    dropAnim.Add($"bolt ({xLoc})");
                    map[q, 0] = UnityEngine.Random.Range(0, difficulty);
                }
            }
            UpdateMap();
        }
        matNum = 0;
        CheckMap();
    }


    public void Swap(int startX, int startY, int tarX, int tarY)
    {

        //////UnityEngine.Debug.Log("The starting coords (x,y) are: " + startX + "," + startY + ". And the target coords are: " + tarX + "," + tarY);
        int temp;
        
        temp = map[startX, startY];

        //////UnityEngine.Debug.Log("Temp, Start and Target values are: " +  temp + " ," +  map[startX, startY] + " ," + map[tarX, tarY]);

        map[startX, startY] = map[tarX, tarY];
        map[tarX, tarY] = temp;
        CheckMap();
        UpdateMap();
       
    }
}
