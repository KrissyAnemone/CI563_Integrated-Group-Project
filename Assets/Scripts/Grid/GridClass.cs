using UnityEngine;

public enum occupier { Mine, Empty }
public class Grid
{
    int layer = -1; // Default layer - change upon loading if implementing layers
    public float spaceWidth = 20;
    public int layerWidth = -1;
    public int layerHeight = -1;

    public Space GetSpace(int x, int z) { return spaces[x, z]; }
    Space[,] spaces = null;

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
                tempSpaces[column, row].obj = CreateObject(occ, column*spaceWidth, row*spaceWidth);
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
            default:
                return occupier.Empty;
        }
    }

    public GameObject CreateObject(occupier occ, float x, float z) // WIP
    {
        if (occ == occupier.Mine)
        {
            // Instantiate Mine
            // Set Mine position
            // return mine object
        }
        return null;
    }
}

public class Space
{
    public occupier containType = occupier.Empty; // Defines what occupies the space. Future proofing (for if we add traps(?) other than the mines, or decide to show walls).

    public GameObject obj = null; // Object occuping the space (access to mine script)

    public Vector2 GetWorldPos() { return worldPos; }
    Vector2 worldPos = new(-1, -1); // Default worldPos (x, z) - set upon being loaded
    
    public Vector2 GetRowCol() { return rowCol; }
    Vector2 rowCol = new(-1, -1);
    
    public Space(int x, int z, float width) // Constructor
    {
        worldPos = new Vector2(x*width, z*width);
        rowCol = new Vector2(x, z);
    }
}