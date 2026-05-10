using UnityEngine;

public enum occupier { Mine, Empty, Blocked }
public class Grid
{
    int layer = -1; // Default layer - change upon loading if implementing layers
    public float spaceWidth = 20;
    public int layerWidth = -1;
    public int layerHeight = -1;

    ScannerController scannerScript;

    public Space GetSpace(int x, int z) { return spaces[x, z]; }
    Space[,] spaces = null;

    public Grid() { }
    public Grid(ScannerController scannerController)
    {
        scannerScript = scannerController;
    }


    public void CreateSpaces(int[,] loadingGrid)
    {
        layerWidth = loadingGrid.GetLength(1);
        layerHeight = loadingGrid.GetLength(0);

        Space[,] tempSpaces = new Space[layerWidth, layerHeight];

        
        for (int column = 0; column < layerWidth; column++)
        {
            for (int row = 0; row < layerHeight; row++)
            {
                // Get Type
                occupier occ = GetSpaceType(loadingGrid[row, column]);

                // Create Space and set Type
                tempSpaces[column, row] = new Space(column, row, spaceWidth);
                tempSpaces[column, row].containType = occ;

                // Create Space Object
                tempSpaces[column, row].obj = CreateObject(occ, tempSpaces[column,row].GetWorldPos().x, tempSpaces[column, row].GetWorldPos().y);
            }
        }
        spaces = tempSpaces;
    }

    private occupier GetSpaceType(int id)
    {
        // 0 = Nothing, 1 = Mine, ...
        switch (id)
        {
            case 0:
                return occupier.Empty;
            case 1:
                return occupier.Mine;
            case 2:
                return occupier.Blocked;
            default:
                return occupier.Empty;
        }
    }

    public void OverwriteSpaceType(Vector2Int gridPos, int value)
    {
        Space space = spaces[gridPos.x, gridPos.y];
        space.containType = GetSpaceType(value);

        if (value == 1)
        {
            Vector2 worldPos = space.GetWorldPos();
            space.obj = scannerScript.CreateObject(space.containType, worldPos.x, worldPos.y);
        }
        else if (space.obj != null)
        {
            scannerScript.DestroyObj(space.obj);
        }
    }

    public GameObject CreateObject(occupier occ, float x, float z)
    {
        if (!scannerScript) return null;
        return scannerScript.CreateObject(occ, x, z); // Must do this because this isn't derived from monobehaviour
    }

    public int CheckSurroundingMines(int col, int row)
    {
        int mines = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            if (col + dx < 0 || col + dx >= layerWidth) continue; // In width bounds
            for (int dy = -1; dy <= 1; dy++)
            {
                if (row + dy < 0 || row + dy >= layerHeight) continue; // In height bounds
                if (dx == 0 && dy == 0) continue; // Not original space

                Space space = GetSpace(col + dx, row + dy);
                if (space.containType == occupier.Mine) mines++; // Increase number of surrounding mines
            }
        }
        return mines;
    }

    public Vector2Int WorldToGrid(Vector3 world)
    {
        /*int x = Mathf.RoundToInt(world.x / grid.spaceWidth);
        int z = Mathf.RoundToInt(world.z / grid.spaceWidth);

        x = Mathf.Clamp(x, 0, grid.layerWidth - 1);
        z = Mathf.Clamp(z, 0, grid.layerHeight - 1);

        return new Vector2Int(x, z);*/
        float xPos = world.x + (layerWidth * spaceWidth) / 2;
        float zPos = world.z - (layerHeight * spaceWidth) / 2;

        int col = (int)(xPos / spaceWidth);
        int row = -(int)(zPos / spaceWidth);

        return new Vector2Int(col, row);
    }


}

public class Space
{
    public occupier containType = occupier.Empty; // Defines what occupies the space. Future proofing (for if we add traps(?) other than the mines, or decide to show walls).

    public GameObject obj = null; // Object occuping the space (access to mine script)

    public Vector2 GetWorldPos() { return worldPos; }
    Vector2 worldPos = new(-1, -1); // Default worldPos (x, z) - set upon being loaded
    
    public Vector2Int GetRowCol() { return rowCol; }
    Vector2Int rowCol = new(-1, -1);

    public int mineNum = 0;
    public GameObject text = null;
    public GameObject light = null;

    public Space(int x, int z, float width) // Constructor
    {
        worldPos = new Vector2(x*width, -z*width);
        rowCol = new Vector2Int(x, z);
    }
}