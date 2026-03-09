using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    Grid grid = new Grid();
    public int[,] ints;

    public float spaceWidth;
    public int boardWidth;
    public int boardHeight;
    public int mineCount;

    public Vector2Int playerStart = new Vector2Int(10, 10);

    // Start is called before the first frame update
    void Start()
    {
        grid.spaceWidth = spaceWidth;

        ints = GridGenerator.Generate(
            boardWidth,
            boardHeight,
            mineCount,
            playerStart,
            2 // Safe radius
        );

        grid.CreateSpaces(ints);

        for (int row = 0; row < grid.layerHeight; row++)
        {
            for (int column = 0; column < grid.layerWidth; column++)
            {
                //Debug.Log(grid.GetSpace(column, row).containType);
                if (grid.GetSpace(column,row).containType == occupier.Mine)
                {
                    //Debug.Log(column + " " + row);
                }
            }
        }
        //Debug.Log("Width vs Height: " + grid.layerWidth + " vs " + grid.layerHeight);
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
