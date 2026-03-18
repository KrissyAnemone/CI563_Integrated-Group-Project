using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    public Grid grid = new Grid();
    public int[,] ints =
    {
        {1,0,1,1,0,0,0 },
        {0,0,1,0,0,0,0 },
        {0,1,0,0,0,0,0 },
        {1,0,0,0,1,0,1 }
    };

    // Start is called before the first frame update
    void Start()
    {
        grid.CreateSpaces(ints);
        for (int row = 0; row < grid.layerHeight; row++)
        {
            for (int column = 0; column < grid.layerWidth; column++)
            {
                //Debug.Log(grid.GetSpace(column, row).containType);
                if (grid.GetSpace(column,row).containType == occupier.Mine)
                {
                    Debug.Log(column + " " + row);
                }
            }
        }
        Debug.Log("Width vs Height: " + grid.layerWidth + " vs " + grid.layerHeight);
        //Debug.Log(grid.GetSpace(2, 1).containType);
        //Debug.Log(grid.GetSpace(2, 1).GetRowCol());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        if (grid == null)
            grid = new Grid();

        for (int row = 0; row < grid.layerHeight; row++)
        {
            for (int column = 0; column < grid.layerWidth; column++)
            {
                Space s = grid.GetSpace(column, row);
                Vector2 pos = s.GetWorldPos();

                if (s.containType == occupier.Mine)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.cyan;

                Gizmos.DrawWireCube(
                    new Vector3(pos.x + grid.spaceWidth / 2, 0, pos.y + grid.spaceWidth / 2),
                    new Vector3(grid.spaceWidth, 0, grid.spaceWidth)
                );
            }
        }
    }
}
