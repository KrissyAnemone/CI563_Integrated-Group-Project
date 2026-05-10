using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AddToGrid : MonoBehaviour
{
    ScannerController scanControl;
    public int value = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (scanControl == null)
        {
            scanControl = FindObjectOfType<ScannerController>();
        }
    }

    private void Update()
    {
        if (scanControl.loadedGrid)
        {
            Vector3 pos = transform.position;
            Grid grid = scanControl.grid;
            Vector2Int gridPos = grid.WorldToGrid(pos);
            scanControl.grid.OverwriteSpaceType(gridPos, value);
            Destroy(gameObject);
        }
    }




}
