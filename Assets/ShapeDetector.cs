using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ShapeDetector : MonoBehaviour
{
    // Assign this in the Inspector with a simple material (e.g. Default-Line).
    public Material lineMaterial;

    public double lineAllowedDeviation = 0.2; // how "strict" you want to be

    // We'll keep track of all the drawing points here.
    private List<Vector2> drawnPoints = new List<Vector2>();

    // Reference to the LineRenderer we’ll use to visualize the drawing.
    private LineRenderer lineRenderer;

    void Start()
    {
        // Grab or add a LineRenderer component (because of [RequireComponent]).
        lineRenderer = GetComponent<LineRenderer>();

        // Set some defaults for how we want our line to look.
        // Adjust these to suit your style (thickness, color, etc.).
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;    // No points yet
        lineRenderer.useWorldSpace = true; // We'll feed it world positions
    }

    void Update()
    {
        // Detect when the player STARTS drawing:
        if (Input.GetMouseButtonDown(0))
        {
            // Clear any old points
            drawnPoints.Clear();

            // Reset line positions
            lineRenderer.positionCount = 0;
        }

        // WHILE the player is drawing (mouse held down):
        if (Input.GetMouseButton(0))
        {
            // Get the current mouse position in screen space
            Vector3 screenPosition = Input.mousePosition;

            // Convert screen space to world space
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            // We only really need (x, y) in 2D, but we'll keep z=0 for the line
            // so it’s visible in the scene.
            Vector3 drawPoint = new Vector3(worldPosition.x, worldPosition.y, 0f);

            // Only add the point if it's not too close to the last point,
            // to avoid extra jitter. But for simplicity, let's add every frame.
            drawnPoints.Add(drawPoint);

            // Update the LineRenderer to show all our points:
            lineRenderer.positionCount = drawnPoints.Count;
            for (int i = 0; i < drawnPoints.Count; i++)
            {
                // Convert our Vector2/Vector3 to line positions
                lineRenderer.SetPosition(i, drawnPoints[i]);
            }
        }

        // Detect when the player STOPS drawing:
        if (Input.GetMouseButtonUp(0))
        {
            // Now that we've finished drawing, let's check the shape:
            DetectShape();
        }
    }

    // This function tries to figure out what shape was drawn
    void DetectShape()
    {
        if (drawnPoints.Count < 2)
        {
            Debug.Log("Not enough points drawn to form a shape.");
            return;
        }

        // Convert our Vector3 points to Vector2 for shape checks
        List<Vector2> points2D = new List<Vector2>();
        foreach (var p in drawnPoints)
            points2D.Add(new Vector2(p.x, p.y));

        // We can do different checks here (line, V, etc.)
        if (IsLine(points2D))
        {
            Debug.Log("You drew a LINE!");
        }
        else if (IsVShape(points2D))
        {
            Debug.Log("You drew a V!");
        }
        else
        {
            Debug.Log("Shape not recognized.");
        }
    }

    // Example: Check if the points form (approximately) a straight line
    bool IsLine(List<Vector2> points)
    {
        // If the user drew fewer than 2 points, we can't form a line
        if (points.Count < 2)
            return false;

        Vector2 startPoint = points[0];
        Vector2 endPoint = points[points.Count - 1];

        // If start and end are effectively the same, can’t form a line
        if ((endPoint - startPoint).sqrMagnitude < Mathf.Epsilon)
            return false;

        // Check the distance of each intermediate point from the line segment
        for (int i = 1; i < points.Count - 1; i++)
        {
            float distance = DistanceFromLineSegment(points[i], startPoint, endPoint);
            if (distance > lineAllowedDeviation)
            {
                return false;
            }
        }

        // If no point was too far off, it's (probably) a line
        return true;
    }

    // Helper function: distance from a point to a line segment
    float DistanceFromLineSegment(Vector2 point, Vector2 segStart, Vector2 segEnd)
    {
        // Length squared of the segment
        float lineLengthSq = (segEnd - segStart).sqrMagnitude;
        if (lineLengthSq < Mathf.Epsilon)
        {
            // Degenerate line: start and end are basically the same
            return Vector2.Distance(point, segStart);
        }

        // Project 'point' onto the line [segStart->segEnd], clamped to [0..1]
        float t = Vector2.Dot(point - segStart, segEnd - segStart) / lineLengthSq;
        t = Mathf.Clamp01(t);

        // Find the projection point on the segment
        Vector2 projection = segStart + t * (segEnd - segStart);

        // Distance between the actual point and the projected point
        return Vector2.Distance(point, projection);
    }


    // Example: Check if the points form a "V" (one sharp turn)
    bool IsVShape(List<Vector2> points)
    {
        int cornerCount = 0;

        // More forgiving angles:
        float minAngle = 20f;
        float maxAngle = 160f;

        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector2 prev = points[i - 1];
            Vector2 current = points[i];
            Vector2 next = points[i + 1];

            Vector2 dirA = (prev - current).normalized;
            Vector2 dirB = (next - current).normalized;

            float angle = Vector2.Angle(dirA, dirB);

            if (angle > minAngle && angle < maxAngle)
            {
                cornerCount++;
                if (cornerCount > 1)
                    return false;
            }
        }

        return (cornerCount == 1);
    }
}