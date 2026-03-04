using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [Header("Map/Player Tracking")]
    public Transform player;
    public RectTransform mapRect;
    public Rect mapSize = new(0, 0, 50, 50);

    [Header("Scanner Input")]
    public RectTransform scrollView;

    private Vector2 oriAnchorMin;
    private Vector2 oriAnchorMax;
    private Vector2 oriPivot;
    private Vector2 oriAnchorPos;

    private float mapWidth;
    private float mapHeight;

    private bool isMapSized;

    // Start is called before the first frame update
    void Start()
    {
        // Get map size from mapRect
        mapWidth = mapRect.rect.width;
        mapHeight = mapRect.rect.height;

        isMapSized = false;

        oriAnchorMin = new Vector2(scrollView.anchorMin.x, scrollView.anchorMin.y);
        oriAnchorMax = new Vector2(scrollView.anchorMax.x, scrollView.anchorMax.y);
        oriPivot = new Vector2(scrollView.pivot.x, scrollView.pivot.y);
        oriAnchorPos = new Vector2(scrollView.anchoredPosition.x, scrollView.anchoredPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMiniMap();

        if (InputManager.Instance.IsScannerDown())
        {
            Debug.Log("On");
            isMapSized = false;

            UpdateSize();
        }

        if (InputManager.Instance.IsScannerUp())
        {
            Debug.Log("Off");
            isMapSized = true;

            UpdateSize();
        }
    }

    private void UpdateMiniMap()
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

    private void UpdateSize()
    {
        if (!isMapSized)
        {
            // New position to middle of the screen
            scrollView.anchorMin = new Vector2(0.5f, 0.5f);
            scrollView.anchorMax = new Vector2(0.5f, 0.5f);
            scrollView.pivot = new Vector2(0.5f, 0.5f);

            scrollView.anchoredPosition = new Vector2(0f, 0f);
        }
        else
        {
            // Reverting to old position
            scrollView.anchorMin = oriAnchorMin;
            scrollView.anchorMax = oriAnchorMax;
            scrollView.pivot = oriPivot;

            scrollView.anchoredPosition = oriAnchorPos;
        }
    }
}
