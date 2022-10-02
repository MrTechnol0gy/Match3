using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BoardLayout : MonoBehaviour
{
    public LayoutRow[] allRows;

    public Gem[,] GetLayout() //a 2d array
    {
        Gem[,] theLayout = new Gem[allRows[0].gemsInRow.Length, allRows.Length]; //sets up our layout

        for(int y = 0; y < allRows.Length; y++)
        {
            for (int x = 0; x < allRows[y].gemsInRow.Length; x++)
            {
                if(x < theLayout.GetLength(0)) //checks to make sure x is within the range of the layout
                { 
                    if(allRows[y].gemsInRow[x] != null)
                    {
                        theLayout[x, allRows.Length - 1 -y] = allRows[y].gemsInRow[x];
                    }
                }
            }
        }



        return theLayout;
    }
}

[System.Serializable] //makes the below code recognizeable in Unity
public class LayoutRow
{
    public Gem[] gemsInRow; //array for creating custom gem layouts
}
