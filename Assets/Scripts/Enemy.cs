using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform santaPaws;
    public float moveSpeed = 2f;

    [Header("Required Shapes (in order)")]
    public List<string> requiredShapes; // e.g. ["HorizontalLine", "V", "Line", "VerticalLine"] assign these in unity's inspector!

    // subscribe to the event when enabled
    void OnEnable()
    {
        ShapeDetector.OnAnyShapeDrawn += OnShapeDrawn;
    }

    // unsubscribe to avoid memory leaks or errors if object is disabled/destroyed
    void OnDisable()
    {
        ShapeDetector.OnAnyShapeDrawn -= OnShapeDrawn;
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

    // gets called whenever ShapeDetector fires an event
    private void OnShapeDrawn(string shapeName)
    {
        // f no shapes left, do nothing
        if (requiredShapes.Count == 0) return;

        // compare drawn shape to the first required shape
        string nextRequired = requiredShapes[0];
        if (shapeName == nextRequired)
        {
            requiredShapes.RemoveAt(0);
            Debug.Log($"Enemy {name}: Correct shape {shapeName}! Removing from list.");

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
}
