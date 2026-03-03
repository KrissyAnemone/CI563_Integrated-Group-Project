using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public Transform player;
    public RectTransform mapRect;
    public Rect mapSize = new(0, 0, 50, 50);

    private float mapWidth;
    private float mapHeight;

    // Start is called before the first frame update
    void Start()
    {
        // Get map size from mapRect
        mapWidth = mapRect.rect.width;
        mapHeight = mapRect.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMiniMap();
    }

    void UpdateMiniMap()
    {
        // Get player world position
        float playerX = player.position.x;
        float playerZ = player.position.z;

        // Convert world position to 0-1 range
        float normalizedX = (playerX + mapSize.width / 2f) / mapSize.width;
        float normalizedZ = (playerZ + mapSize.height / 2f) / mapSize.height;

        // Convert normalized position to UI space
        float mapPosX = normalizedX * mapWidth;
        float mapPosY = normalizedZ * mapHeight;

        // Move map in opposite direction to center player
        mapRect.anchoredPosition = new Vector2(-mapPosX + mapWidth / 2f, -mapPosY + mapHeight / 2f);
    }
}
