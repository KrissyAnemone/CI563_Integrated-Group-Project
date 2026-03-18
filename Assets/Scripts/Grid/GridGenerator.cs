using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridGenerator
{
    public static int[,] Generate(int width, int height, int mineCount, Vector2Int safeCell, int safeRadius = 1)
    {
        int[,] grid = new int[height, width];

        int placed = 0;

        while (placed < mineCount)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            // Skip if already a mine
            if (grid[y, x] == 1)
                continue;

            // Prevent mines near starting area
            if (Mathf.Abs(x - safeCell.x) <= safeRadius &&
                Mathf.Abs(y - safeCell.y) <= safeRadius)
                continue;

            grid[y, x] = 1;
            placed++;
        }

        return grid;
    }
}
