using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

public class Icon : MonoBehaviour
{
    Vector3 ogClick;
    Vector3 newClick;
    //int[] elPos = new int[2]; 
    int num;
    Grid grid;
    Regex regex = new Regex(@"\d+");

    Match match;
    void Start()
    {
        grid = GameObject.Find("background").GetComponent<Grid>();
        if (grid == null)
        {
            Debug.LogError("Grid component not found on the GameObject named 'background'.");
        }
    }

    void Update()
    {
        
    }
    private void OnMouseDown()
    {

        match = regex.Match(gameObject.name);
        if (match.Success)
        {
            num = int.Parse(match.Value);
            //Debug.Log("The Icon Number is bolt(" + num + ").");
        }
        else
        {
            //Debug.Log("The Icon Number is bolt(0).");
            num = 0;
        }
        //Debug.Log("Clicked on: " + gameObject.name);
        ogClick = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        ogClick.z = 0f; 


        //elPos[0] = num/6;
        //Debug.Log(elPos[0]);
        //elPos[1] = num%6;
        //Debug.Log(elPos[1]);

        //Debug.Log("Mouse Position Clicked: " + ogClick);
    }

    private void OnMouseDrag()
    {
        double tempX, tempY;
        newClick = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        newClick.z = 0f; 

        tempX = ogClick.x - newClick.x;
        tempY = ogClick.y - newClick.y;

        //Debug.Log("Mouse Position Dragged To:  " + tempX + ", " + tempY);

        if (Math.Abs(tempX) > Math.Abs(tempY))
        {
            if (tempX > 0)
            {
                Debug.Log("Moving Left");
                
            }
            else
            {
                Debug.Log("Moving Right");
                
            }
        }
        else
        {
            if (tempY > 0)
            {
                Debug.Log("Moving Down");
                
            }
            else
            {
                Debug.Log("Moving Up");
                
            }


        }

    }

    private void OnMouseUp()
    {
        double tempX, tempY;
        int neX, neY; // the position of the targeted element

        int i, j;
        neX = 0; neY = 0;

        i = num / 6;
        j = num % 6;

        //Debug.Log("X coord is: " + j);
        //Debug.Log("Y coord is: " + i);

        // i is the up and down scale, j is the left and right scale, basically the x and y coordinates in the 2d array

        //Debug.Log("DEBUG: i = " + i + " j = " + j);

        // compare the original position to the dragged to position on the x axis and then the y axis

        tempX = ogClick.x - newClick.x;
        tempY = ogClick.y - newClick.y;

        //Debug.Log("Mouse Position Dragged To:  " + tempX + ", " + tempY);

        if (Math.Abs(tempX) > Math.Abs(tempY))
        {
            if (tempX > 0)
            {
                //Debug.Log("Moving Left");
                if (j == 0)
                {
                    // Can't move left
                } else
                {
                    neX = j - 1;
                    neY = i;
                    //Debug.Log("The starting coords (x,y) are: " + j + "," + i + ". And the target coords are: " + neX + "," + neY);
                    grid.Swap(j, i, neX, neY);
                }
            }
            else
            {
                //Debug.Log("Moving Right");
                if (j == 5)
                {
                    // Can't move right
                }
                else
                {
                    neX = j + 1;
                    neY = i;
                    //Debug.Log("The starting coords (x,y) are: " + j + "," + i + ". And the target coords are: " + neX + "," + neY);
                    grid.Swap(j, i, neX, neY);
                }
            }
        }
        else
        {
            if (tempY > 0)
            {
                //Debug.Log("Moving Down");
                if (i == 7)
                {
                    // Can't move down
                }
                else
                {
                    neX = j;
                    neY = i + 1;
                   // Debug.Log("The starting coords (x,y) are: " + j + "," + i + ". And the target coords are: " + neX + "," + neY);
                    grid.Swap(j, i, neX, neY);
                }
            }
            else
            {
                //Debug.Log("Moving Up");
                if (i == 0)
                {
                    // Cant move up
                }
                else
                {
                    neX = j;
                    neY = i - 1;
                    //Debug.Log("The starting coords (x,y) are: " + j + "," + i + ". And the target coords are: " + neX + "," + neY);
                    grid.Swap(j, i, neX, neY);
                }
            }

            
        }
        //Debug.Log("The starting coords (x,y) are: " + j + "," + i + ". And the target coords are: " + neX + "," + neY);
        
    }
}
