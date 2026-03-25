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
    [Header("Enemy Stats")]
    public Transform enemyHome;
    public MineState currentState = MineState.Idle;
    public MineState lastState = MineState.Idle;
    public float moveSpeed = 3f;
    public float waitTarget = 3f;
    public float hearDis = 6f;

    [Header("Enemy Vision")]
    public LayerMask obstacleMask;
    public LayerMask playerMask;
    public float viewDis = 12f;
    public float explodeDis = 5f;
    public float viewAngle = 180f;

    [Header("Other Object")]
    public Transform player;
    public Grid grid;

    // Pathfinding
    private EnemyPathfinding pathfinder;
    private List<Vector2Int> currentPath;
    private int pathIndex;

    // Grid Positions
    private Vector2Int currentGridPos;
    private Vector2Int targetGridPos;
    private Vector2Int lastPlayerGrid;

    // Chase
    private float chaseTimer;
    private float chaseUpdateRate = 0.5f;

    void Start()
    {
        var scanner = FindObjectOfType<ScannerController>();
        if (scanner != null)
        {
            grid = scanner.grid;
            pathfinder = new EnemyPathfinding(grid);
            UpdateGridPos();
        }
        else
            Debug.LogError("Scanner not found!");
    }

    void Update()
    {
        TestInput();

        UpdateGridPos();

        if (CanSeePlayer())
        {
            currentState = MineState.Chase;
        }
        else if (CanHearPlayer())
        {
            Vector2Int noiseGrid = WorldToGrid(player.position);
            BeginPath(noiseGrid);
        }

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
                lastState = MineState.ReturnHome;
                break;

            case MineState.Chase:
                ChasePC();
                lastState = MineState.Chase;
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
            if (lastState != MineState.ReturnHome) StartCoroutine(WaitThenReturn());
            currentPath = null;
            lastState = MineState.Search;
            return;
        }

        Vector2Int nextTile = currentPath[pathIndex];

        Vector3 targetWorld = GridToWorld(nextTile);
        targetWorld.x += grid.spaceWidth / 2;
        targetWorld.z -= grid.spaceWidth / 2;

        Vector3 moveDir = (targetWorld - transform.position);

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 8f * Time.deltaTime);
        }

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
            Debug.Log("GO SEARCH");
        }
        else
        {
            Debug.Log("NULLL");
        }
    }

    void UpdateGridPos()
    {
        currentGridPos = WorldToGrid(transform.position);
    }

    Vector2Int WorldToGrid(Vector3 world)
    {
        /*int x = Mathf.RoundToInt(world.x / grid.spaceWidth);
        int z = Mathf.RoundToInt(world.z / grid.spaceWidth);

        x = Mathf.Clamp(x, 0, grid.layerWidth - 1);
        z = Mathf.Clamp(z, 0, grid.layerHeight - 1);

        return new Vector2Int(x, z);*/
        float xPos = world.x + (grid.layerWidth*grid.spaceWidth)/2;
        float zPos = world.z - (grid.layerHeight * grid.spaceWidth)/2;

        int col = (int)(xPos / grid.spaceWidth);
        int row = -(int)(zPos / grid.spaceWidth);

        return new Vector2Int(col, row);
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        //return new Vector3(gridPos.x * grid.spaceWidth, transform.position.y, gridPos.y * grid.spaceWidth);

        float xPos = gridPos.x * grid.spaceWidth;
        float zPos = gridPos.y * -grid.spaceWidth;

        float worldX = xPos - (grid.layerWidth * grid.spaceWidth) / 2;
        float worldZ = zPos + (grid.layerHeight * grid.spaceWidth) / 2;

        return new Vector3(worldX, 1.125f, worldZ);

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

    void ChasePC()
    {
        chaseTimer -= Time.deltaTime;

        if (chaseTimer <= 0f)
        {
            Vector2Int playerGrid = WorldToGrid(player.position);

            if (playerGrid != lastPlayerGrid)
            {
                BeginPath(playerGrid);
                lastPlayerGrid = playerGrid;
            }

            chaseTimer = chaseUpdateRate;
             
        }

        Search();
    }

    bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.down * 0.5f;
        Vector3 target = player.position + Vector3.up * 0.9f;

        Vector3 dirPlayer = (target - origin);
        float dis = dirPlayer.magnitude;
        dirPlayer.Normalize();

        if (dis > viewDis)
            return false;
        if (dis < explodeDis)
        {
            Explode();
            return false;
        }

        dirPlayer.Normalize();

        float angle = Vector3.Angle(transform.forward, dirPlayer);

        if (angle > viewAngle * 0.5f)
            return false;

        if (Physics.Raycast(origin, dirPlayer, out RaycastHit hit, viewDis, obstacleMask | playerMask))
        {
            if (hit.transform == player)
                return true;
        }

        return false;
    }

    bool CanHearPlayer()
    {
        float dis = Vector3.Distance(transform.position, player.position);

        PCMovement pc = player.GetComponent<PCMovement>();

        if (pc.IsCrouching)
            return false;

        return dis < hearDis;
    }

    void Explode()
    {
        Debug.Log("You are dead.");
        // Implement death screen
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDis);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeDis);

        Vector3 left = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, left * viewDis);
        Gizmos.DrawRay(transform.position, right * viewDis);
        Gizmos.DrawRay(transform.position, transform.forward * viewDis);
    }

    void TestInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector2Int playerGrid = WorldToGrid(player.position);
            Vector2Int homeGrid = WorldToGrid(enemyHome.position);

            Debug.Log("Player Grid: " + playerGrid);

            //if (playerGrid != lastPlayerGrid)
            {
                BeginPath(playerGrid);
                lastPlayerGrid = playerGrid;
            }
        }
    }

    public void SonarTriggered(Transform tf)
    {
        Vector2Int gridPos = WorldToGrid(tf.position);
        Debug.Log("Sonar Grid: " + gridPos);
        BeginPath(gridPos);
    }
}
