using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [Header("Map/Player Tracking")]
    public Transform player;
    public RectTransform mapRect;
    public GameObject cursor;
    public Rect mapSize = new(0, 0, 50, 50);

    [Header("Scanner Input")]
    public RectTransform scrollView;
    public float zoomOutScale;

    // Scanner Input
    private CanvasGroup canvasGroup;
    private Vector2 oriAnchorMin;
    private Vector2 oriAnchorMax;
    private Vector2 oriPivot;
    private Vector2 oriAnchorPos;

    private bool isMapResized;

    // Map/Player Tracking
    private float mapWidth;
    private float mapHeight;

    void Start()
    {
        // Getting map size from mapRect
        mapWidth = mapRect.rect.width;
        mapHeight = mapRect.rect.height;

        // Setting if the minimap has been resized to false
        isMapResized = false;

        // Storing the minimap's position values
        oriAnchorMin = new Vector2(scrollView.anchorMin.x, scrollView.anchorMin.y);
        oriAnchorMax = new Vector2(scrollView.anchorMax.x, scrollView.anchorMax.y);
        oriPivot = new Vector2(scrollView.pivot.x, scrollView.pivot.y);
        oriAnchorPos = new Vector2(scrollView.anchoredPosition.x, scrollView.anchoredPosition.y);

        // Getting CanvasGroup
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        cursor.SetActive(false);
    }

    void Update()
    {
        UpdateMiniMap();

        // Checking for input
        if (InputManager.Instance.IsScannerDown())
        {
            Debug.Log("On");
            isMapResized = false;

            cursor.SetActive(true);

            UpdateSize();
        }

        if (InputManager.Instance.IsScannerUp())
        {
            Debug.Log("Off");
            isMapResized = true;

            cursor.SetActive(false);

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
        if (!isMapResized)
        {
            // New centre position
            scrollView.anchorMin = new Vector2(0.5f, 0.5f);
            scrollView.anchorMax = new Vector2(0.5f, 0.5f);
            scrollView.pivot = new Vector2(0.5f, 0.5f);

            scrollView.anchoredPosition = new Vector2(0f, 0f);

            // New map scaling
            scrollView.GetComponent<RectTransform>().localScale = new Vector2(5.5f, 5.5f);
            gameObject.GetComponent<RectTransform>().localScale = new Vector2(zoomOutScale, zoomOutScale);

            // New alpha
            canvasGroup.alpha = 0.5f;
        }
        else
        {
            // Reverting positions
            scrollView.anchorMin = oriAnchorMin;
            scrollView.anchorMax = oriAnchorMax;
            scrollView.pivot = oriPivot;

            scrollView.anchoredPosition = oriAnchorPos;

            // Reverting scales
            scrollView.GetComponent<RectTransform>().localScale = new Vector2(2f, 2f);
            gameObject.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);

            // Reverting alpha
            canvasGroup.alpha = 1f;
        }
    }
}
