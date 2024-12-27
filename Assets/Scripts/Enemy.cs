using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform santaPaws;
    public float moveSpeed = 2f;

    [Header("Required Shapes (in order)")]
    public List<string> requiredShapes; // e.g. ["HorizontalLine", "V", "Line"]

    [Header("Icons Above Head")]
    public Transform iconContainer;      // Assign your IconContainer in the Inspector
    public GameObject iconPrefab;        // The ShapeIcon prefab
    public float iconSpacing = 0.5f;     // Horizontal spacing between icons

    // Sprites for each shape name
    public Sprite horizontalLineSprite;
    public Sprite verticalLineSprite;
    public Sprite diagonalLineSprite;
    public Sprite vShapeSprite;
    // ... add more if needed

    void OnEnable()
    {
        // Subscribe to shape-drawn event
        ShapeDetector.OnAnyShapeDrawn += OnShapeDrawn;
    }

    void OnDisable()
    {
        // Unsubscribe
        ShapeDetector.OnAnyShapeDrawn -= OnShapeDrawn;
    }

    void Start()
    {
        // Build initial icons from the requiredShapes list
        RefreshIcons();
    }

    void Update()
    {
        MoveTowardSantaPaws();
    }

    void MoveTowardSantaPaws()
    {
        if (santaPaws == null) return;
        Vector2 direction = (santaPaws.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void OnShapeDrawn(string shapeName)
    {
        // If we have no shapes left, do nothing
        if (requiredShapes.Count == 0) return;

        // Compare drawn shape to the FIRST shape in our list
        string nextRequired = requiredShapes[0];
        if (shapeName == nextRequired)
        {
            // If correct, remove it
            requiredShapes.RemoveAt(0);
            Debug.Log($"Enemy {name}: Correct shape {shapeName}! Removing from list.");

            // Rebuild icons to reflect the new list
            RefreshIcons();

            // If none left, destroy self
            if (requiredShapes.Count == 0)
            {
                Debug.Log($"Enemy {name}: All shapes done. Enemy defeated!");
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log($"Enemy {name}: Wrong shape ({shapeName}), next required is ({nextRequired}).");
        }
    }

    /// <summary>
    /// Destroys old icons and re-builds the icon row above the enemy’s head.
    /// </summary>
    private void RefreshIcons()
    {
        // 1) Destroy old children in iconContainer
        for (int i = iconContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(iconContainer.GetChild(i).gameObject);
        }

        // 2) If no shapes, nothing to show
        if (requiredShapes.Count == 0) return;

        // 3) Calculate total width
        int n = requiredShapes.Count;
        float totalWidth = (n - 1) * iconSpacing;  // e.g. 2 shapes => 1 gap

        // 4) Create each icon and position them
        for (int i = 0; i < n; i++)
        {
            // Which shape are we showing?
            string shape = requiredShapes[i];

            // Instantiate the iconPrefab as a child of iconContainer
            GameObject iconGO = Instantiate(iconPrefab, iconContainer);

            // Center them around x=0. So the leftmost shape is at -totalWidth/2
            float xPos = -totalWidth * 0.5f + (i * iconSpacing);
            iconGO.transform.localPosition = new Vector3(xPos, 0f, 0f);

            // Optional: If your iconPrefab has a SpriteRenderer, set the sprite
            var sr = iconGO.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = GetSpriteForShape(shape);
            }

            // You could add more fancy stuff here (scale, rotation, etc.)
        }
    }

    /// <summary>
    /// Returns the correct sprite for a given shape name.
    /// </summary>
    private Sprite GetSpriteForShape(string shape)
    {
        switch (shape)
        {
            case "HorizontalLine": return horizontalLineSprite;
            case "VerticalLine": return verticalLineSprite;
            case "DiagonalLine": return diagonalLineSprite;
            case "V": return vShapeSprite;
            // Add more if needed
            default: return null;
        }
    }
}
