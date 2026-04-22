using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScannerController : MonoBehaviour
{
    public Grid grid;
    [SerializeField] GameObject mapObj;
    [SerializeField] GameObject sonarPrefab;
    [SerializeField] GameObject testObstaclePrefab;
    [SerializeField] GameObject pc;
    [SerializeField] GameObject enemy;
    public int[,] mapToLoad =
    {
        { 0,0,0,0,0,0,0,0,0,2 },
        { 0,2,2,2,2,2,2,2,2,2 },
        { 0,0,0,0,0,0,0,0,0,2 },
        { 2,2,2,2,2,2,2,2,0,2 },
        { 2,0,0,0,0,0,0,0,0,2 },
        { 2,0,2,2,2,2,2,2,2,2 },
        { 2,0,0,0,0,0,0,0,0,2 },
        { 2,2,2,2,2,2,2,2,0,2 },
        { 2,1,0,0,0,0,0,0,0,2 },
        { 2,2,2,2,2,2,2,2,2,1 }
    };
    // Start is called before the first frame update
    void Start()
    {
        SonarMine sonarPS = sonarPrefab.GetComponent<SonarMine>();
        sonarPS.pc = pc;
        sonarPS.enemy = enemy;

        grid = new Grid(this);
        grid.CreateSpaces(mapToLoad);
        CreateMineText();

        FindObjectOfType<EnemyMine>().PassGrid(grid);

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
                pos = new(pos.x-((grid.layerWidth*grid.spaceWidth)/2), pos.y+(grid.layerHeight*grid.spaceWidth)/2 - grid.spaceWidth);

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

    public GameObject CreateObject(occupier occ, float x, float z) // Create objects for grid
    {
        if (occ == occupier.Mine)
        {
            if (sonarPrefab)
            {
                GameObject sonar = Instantiate(sonarPrefab);
                //sonar.transform.position = new Vector3(x-90,2,z+90); // The offset is originally -100 because thats half the side of the whole grid. 
                // It becomes 90 when converting from the bottom left of each space to the middle (each space is 20 wide, so its +10).
                // It is the opposite sign for z axis because the grid is stored from top to bottom, I believe. This was partially rectified by changing the real world pos to -z*width in the Space constructor.

                float xOffset = ((grid.layerWidth * grid.spaceWidth) - grid.spaceWidth) / 2;
                float zOffset = ((grid.layerHeight * grid.spaceWidth) - grid.spaceWidth) / 2;
                sonar.transform.position = new Vector3(x - xOffset, 2, z + zOffset);
                return sonar;
            }
        }
        else if (occ == occupier.Blocked)
        {
            GameObject obj = Instantiate(testObstaclePrefab);

            float xOffset = ((grid.layerWidth * grid.spaceWidth) - grid.spaceWidth) / 2;
            float zOffset = ((grid.layerHeight * grid.spaceWidth) - grid.spaceWidth) / 2;
            obj.transform.position = new Vector3(x - xOffset, 2, z + zOffset);

        }
        return null;
    }

    void CreateMineText()
    {
        // ------------------------- Create base object and add required components -------------------------

        GameObject obj = new GameObject();

        obj.AddComponent<CanvasRenderer>();

        RectTransform tf = obj.AddComponent<RectTransform>();
        TextMeshProUGUI txt = obj.AddComponent<TextMeshProUGUI>();

        // Set transform properties
        tf.sizeDelta = new(60, 60);
        tf.localPosition = new(-1111,-1111,0); // Move far out of way
        tf.anchorMax = new(0, 1);
        tf.anchorMin = new(0,1);

        // Set default text properties
        txt.text = "";
        txt.alignment = TextAlignmentOptions.Center;


        // ------------------------- Instantiate text objects at each space in the grid -------------------------

        float xStart = 29.5f, yStart = -29.5f;  // RowCol(0,0) = Position(29.5, -29.5)
        float dif = 62.5f; // Distance between spaces = 62.5

        for (int row = 0; row < grid.layerHeight; row++)
        {
            for (int column = 0; column < grid.layerWidth; column++)
            {
                Space space = grid.GetSpace(column, row);
                space.text = Instantiate(obj); // Instantiate Text object

                // Set transform properties
                RectTransform instance_tf = space.text.GetComponent<RectTransform>();
                instance_tf.SetParent(mapObj.transform);
                instance_tf.localScale = new(1, 1, 1);

                // For some reason its not setting the correct local position, its offset by 312.5, so I just move it back by that much...
                float xPos = xStart + (dif * column) - 312.5f;
                float yPos = yStart - (dif * row) + 312.5f;
                instance_tf.localPosition = new(xPos, yPos, 0);

                Button btn = space.text.gameObject.AddComponent<Button>();

                void funct() { SpaceClickEvent(space); } // Local function for this specific space
                btn.onClick.AddListener(funct); // Add function to click event
            }
        }
    }

    private void SpaceClickEvent(Space space)
    {
        GameObject textObj = space.text.gameObject;
        TextMeshProUGUI text = textObj.GetComponent<TextMeshProUGUI>();
        if (text.text != "") return;

        Debug.Log(space.GetRowCol());

        int num = grid.CheckSurroundingMines((int)space.GetRowCol().x, (int)space.GetRowCol().y); 
        text.text = num.ToString(); // Store number in space - possible use for saving
        space.mineNum = num;
    }
}
