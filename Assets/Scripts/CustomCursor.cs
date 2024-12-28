using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite holdSprite;

    // Reference to our SpriteRenderer
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the SpriteRenderer component on this GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Make sure we start with the default sprite
        spriteRenderer.sprite = defaultSprite;

        // (Optional) Hide the OS cursor
        Cursor.visible = false;
    }

    void Update()
    {
        FollowMouse();

        if (Input.GetMouseButton(0))
        {
            // If the left mouse button is held, show holdSprite
            spriteRenderer.sprite = holdSprite;
        }
        else
        {
            // Otherwise, show defaultSprite
            spriteRenderer.sprite = defaultSprite;
        }
    }

    /// <summary>
    /// Moves this GameObject to follow the mouse position (in world space).
    /// </summary>
    private void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        // Convert screen space to world space
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);

        // Keep Z = 0 so it's visible in a 2D scene
        worldPos.z = 0f;

        // Update position
        transform.position = worldPos;
    }
}
