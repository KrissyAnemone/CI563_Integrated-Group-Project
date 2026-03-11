using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MineState
{
    Idle,
    Search,
    ReturnHome,
    Chase,
    Explode
}

public class EnemyMine : MonoBehaviour
{
    public Transform player;
    public Transform enemyHome;
    public MineState currentState = MineState.Idle;
    public float moveSpeed = 3f;
    public float waitTarget = 3f;

    public Grid grid;

    private EnemyPathfinding pathfinder;
    private List<Vector2Int> currentPath;
    private int pathIndex;

    Vector2Int currentGridPos;
    Vector2Int targetGridPos;

    void Start()
    {
        var testGrid = FindObjectOfType<TestGrid>();
        if (testGrid != null)
        {
            grid = testGrid.grid;
            pathfinder = new EnemyPathfinding(grid);
            UpdateGridPos();
        }
        else
            Debug.LogError("TestGrid not found!");
    }

    void Update()
    {
        TestInput();

        switch (currentState)
        {
            case MineState.Idle:
                Idle();
                break;

            case MineState.Search:
                Search();
                break;
            case MineState.ReturnHome:
                ReturnHome();
                break;
        }
    }

    void Idle()
    {
        // Waiting for sound trigger
    }

    void Search()
    {
        if (currentPath == null)
            return;

        if (pathIndex >= currentPath.Count)
        {
            StartCoroutine(WaitThenReturn());
            currentPath = null;
            return;
        }

        Vector2Int nextTile = currentPath[pathIndex];

        Vector3 targetWorld = GridToWorld(nextTile);

        transform.position = Vector3.MoveTowards(transform.position, targetWorld, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorld) < 0.05f)
            pathIndex++;
    }

    void BeginPath(Vector2Int target)
    {
        UpdateGridPos();

        targetGridPos = target;

        //Debug.Log("Start Grid: " + currentGridPos);
        //Debug.Log("Target Grid: " + targetGridPos);
        //Debug.Log("Grid Size: " + grid.layerWidth + " x " + grid.layerHeight);

        currentPath = pathfinder.FindPath(currentGridPos, targetGridPos);

        if (currentPath != null)
        {
            //Debug.Log("PATH FOUND length: " + currentPath.Count);
            pathIndex = 0;
            currentState = MineState.Search;
        }
    }

    void UpdateGridPos()
    {
        currentGridPos = WorldToGrid(transform.position);
    }

    Vector2Int WorldToGrid(Vector3 world)
    {
        int x = Mathf.RoundToInt(world.x / grid.spaceWidth);
        int z = Mathf.RoundToInt(world.z / grid.spaceWidth);

        x = Mathf.Clamp(x, 0, grid.layerWidth - 1);
        z = Mathf.Clamp(z, 0, grid.layerHeight - 1);

        return new Vector2Int(x, z);
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * grid.spaceWidth, transform.position.y, gridPos.y * grid.spaceWidth);
    }

    void ReturnHome()
    {
        if (currentPath != null)
            return;

        Vector2Int homeGrid = WorldToGrid(enemyHome.position);

        BeginPath(homeGrid);
    }

    IEnumerator WaitThenReturn()
    {
        currentState = MineState.Idle;

        yield return new WaitForSeconds(waitTarget);

        currentState = MineState.ReturnHome;
    }

    void TestInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector2Int noiseGrid = WorldToGrid(player.position);

            //Debug.Log("Player Grid: " + noiseGrid);

            BeginPath(noiseGrid);
        }
    }
}
