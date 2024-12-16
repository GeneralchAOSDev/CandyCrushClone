using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Windows;
using DG.Tweening;
using System.Runtime.ConstrainedExecution;
public class Icon : MonoBehaviour
{
    Vector3 ogClick;
    Vector3 newClick;
    Vector3 oldPos;
    //int[] elPos = new int[2]; 
    int num;
    Grid grid;
    Regex regex = new Regex(@"\d+");
    private Animator mAnimator;
    //[SerializeField] private float maxDragDistance = 1.0f; // Maximum distance the object can move during drag

    Match match;
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        grid = GameObject.Find("background").GetComponent<Grid>();
        if (grid == null)
        {
            Debug.LogError("Grid component not found on the GameObject named 'background'.");
        }
    }

    void Update()
    {
        if (mAnimator != null)
        { 
            //mAnimator.SetInteger("iconType",1);
        }
    }
    private void OnMouseDown()
    {
        oldPos = transform.position;
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
                //Debug.Log("Moving Right");
                Vector3 animate = transform.position;
                animate = new Vector3(animate.x - 0.5f, animate.y, animate.z);
                if (animate.x - oldPos.x > -1f)
                {
                    transform.DOMove(animate, 1);
                }
                
            }
            else
            {
                //Debug.Log("Moving Left");
                Vector3 animate = transform.position;
                animate = new Vector3(animate.x + 0.5f, animate.y, animate.z);
                if (animate.x - oldPos.x < 1f)
                {
                    transform.DOMove(animate, 1);
                }
            }
        }
        else
        {
            if (tempY > 0)
            {
                //Debug.Log("Moving Down");
                Vector3 animate = transform.position;
                animate = new Vector3(animate.x, animate.y - 0.5f, animate.z);
                if (animate.y - oldPos.y > -1f)
                {
                    transform.DOMove(animate, 1);
                }
            }
            else
            {
                //Debug.Log("Moving Up");
                Vector3 animate = transform.position;
                animate = new Vector3(animate.x, animate.y + 0.5f, animate.z);
                if (animate.y - oldPos.y < 1f)
                {
                    transform.DOMove(animate, 1);
                }
            }


        }

    }

    private void OnMouseUp()
    {
        transform.DOKill();
        transform.position = oldPos;
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
