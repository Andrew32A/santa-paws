using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShapeType
{
    HorizontalLine,
    VerticalLine,
    DiagonalLine,
    V
}

public class Enemy : MonoBehaviour
{
    [Header("Success Sound")]
    public AudioSource audioSource;
    public AudioClip[] successSounds;

    // static flag so only one success sound can play each frame
    private static bool successSoundPlayedThisFrame = false;


    [Header("Movement Settings")]
    public Transform santaPaws;
    public float moveSpeed = 2f;

    [Header("Required Shapes (in order)")]
    public List<ShapeType> requiredShapes;

    [Header("Icons Above Head")]
    public Transform iconContainer;
    public GameObject iconPrefab;
    public float iconSpacing = 0.5f;

    [Header("Shape Sprites")]
    public Sprite horizontalLineSprite;
    public Sprite verticalLineSprite;
    public Sprite diagonalLineSprite;
    public Sprite vShapeSprite;

    void OnEnable()
    {
        ShapeDetector.OnAnyShapeDrawn += OnShapeDrawn;
    }

    void OnDisable()
    {
        ShapeDetector.OnAnyShapeDrawn -= OnShapeDrawn;
    }

    void Start()
    {
        RefreshIcons();
    }

    void Update()
    {
        MoveTowardSantaPaws();
    }

    /// resets the static flag once per frame so only one sound can play per frame.
    void LateUpdate()
    {
        // at the end of the frame, allow a success sound in the next frame again
        successSoundPlayedThisFrame = false;
    }

    void MoveTowardSantaPaws()
    {
        if (santaPaws == null) return;
        Vector2 direction = (santaPaws.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private void OnShapeDrawn(string shapeName)
    {
        // if we have no shapes left, do nothing
        if (requiredShapes.Count == 0) return;

        // convert the detected shape string to an enum (if possible)
        ShapeType? detected = ConvertToShapeType(shapeName);
        if (detected == null)
        {
            // e.g. shapeName didn't match anything in the enum
            Debug.Log($"Enemy {name}: Received unknown shapeName '{shapeName}'.");
            return;
        }

        // compare with the first shape in requiredShapes
        ShapeType nextRequired = requiredShapes[0];
        if (detected.Value == nextRequired)
        {
            requiredShapes.RemoveAt(0);
            Debug.Log($"Enemy {name}: Correct shape {shapeName}! Removing from list.");

            // play a success sound if not already played this frame
            PlaySuccessSoundOncePerFrame();

            RefreshIcons();

            // if none left, destroy self
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

    /// plays one random success sound if none has played this frame yet
    private void PlaySuccessSoundOncePerFrame()
    {
        // if a success sound has already been played this frame, do nothing
        if (successSoundPlayedThisFrame) return;

        successSoundPlayedThisFrame = true;

        if (audioSource != null && successSounds != null && successSounds.Length > 0)
        {
            AudioClip clip = successSounds[Random.Range(0, successSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Enemy {name}: No audioSource or no successSounds assigned!");
        }
    }

    /// helper to convert shapeName strings from ShapeDetector to enum values
    private ShapeType? ConvertToShapeType(string shapeName)
    {
        switch (shapeName)
        {
            case "HorizontalLine": return ShapeType.HorizontalLine;
            case "VerticalLine": return ShapeType.VerticalLine;
            case "DiagonalLine": return ShapeType.DiagonalLine;
            case "V": return ShapeType.V;
            // add more if needed
            default: return null;
        }
    }

    /// destroys old icons and re-builds the icon row above the enemy’s head.
    private void RefreshIcons()
    {
        for (int i = iconContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(iconContainer.GetChild(i).gameObject);
        }
        if (requiredShapes.Count == 0) return;

        int n = requiredShapes.Count;
        float totalWidth = (n - 1) * iconSpacing;

        for (int i = 0; i < n; i++)
        {
            ShapeType shape = requiredShapes[i];

            GameObject iconGO = Instantiate(iconPrefab, iconContainer);

            float xPos = -totalWidth * 0.5f + (i * iconSpacing);
            iconGO.transform.localPosition = new Vector3(xPos, 0f, 0f);

            var sr = iconGO.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = GetSpriteForShape(shape);
            }
        }
    }

    /// returns the correct sprite for a given shape type.
    private Sprite GetSpriteForShape(ShapeType shape)
    {
        switch (shape)
        {
            case ShapeType.HorizontalLine: return horizontalLineSprite;
            case ShapeType.VerticalLine: return verticalLineSprite;
            case ShapeType.DiagonalLine: return diagonalLineSprite;
            case ShapeType.V: return vShapeSprite;
            default: return null;
        }
    }
}
