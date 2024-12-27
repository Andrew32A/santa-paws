using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(LineRenderer))]
public class ShapeDetector : MonoBehaviour
{
    // static event for enemies to listen out for
    public static event Action<string> OnAnyShapeDrawn;

    public Material lineMaterial;
    public float lineAllowedDeviation = 0.2f;

    private List<Vector2> drawnPoints = new List<Vector2>();
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            drawnPoints.Clear();
            lineRenderer.positionCount = 0;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 screenPosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector3 drawPoint = new Vector3(worldPosition.x, worldPosition.y, 0f);

            drawnPoints.Add(drawPoint);

            lineRenderer.positionCount = drawnPoints.Count;
            for (int i = 0; i < drawnPoints.Count; i++)
            {
                lineRenderer.SetPosition(i, drawnPoints[i]);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            DetectShape();
        }
    }

    void DetectShape()
    {
        if (drawnPoints.Count < 2)
        {
            Debug.Log("Not enough points drawn to form a shape.");
            return;
        }

        // Convert Vector3 -> Vector2
        List<Vector2> points2D = new List<Vector2>();
        foreach (var p in drawnPoints)
        {
            points2D.Add(new Vector2(p.x, p.y));
        }

        // ORDER: Horizontal -> Vertical -> Generic line -> "V"
        // may need to adjust order of operations in future

        if (IsHorizontalLine(points2D))
        {
            Debug.Log("You drew a HORIZONTAL line!");
            OnAnyShapeDrawn?.Invoke("HorizontalLine");
        }
        else if (IsVerticalLine(points2D))
        {
            Debug.Log("You drew a VERTICAL line!");
            OnAnyShapeDrawn?.Invoke("VerticalLine");
        }
        else if (IsLine(points2D))
        {
            Debug.Log("You drew a LINE (generic)!");
            OnAnyShapeDrawn?.Invoke("Line");
        }
        else if (IsVShape(points2D))
        {
            Debug.Log("You drew a V!");
            OnAnyShapeDrawn?.Invoke("V");
        }
        else
        {
            Debug.Log("Shape not recognized.");
        }
    }

    // 1) Distance-based check to confirm it's a fairly straight line
    bool IsLine(List<Vector2> points)
    {
        if (points.Count < 2) return false;

        Vector2 startPoint = points[0];
        Vector2 endPoint = points[points.Count - 1];

        if ((endPoint - startPoint).sqrMagnitude < Mathf.Epsilon)
            return false;

        for (int i = 1; i < points.Count - 1; i++)
        {
            float distance = DistanceFromLineSegment(points[i], startPoint, endPoint);
            if (distance > lineAllowedDeviation)
            {
                return false;
            }
        }
        return true;
    }

    // 2) Horizontal line? Check angle vs. X-axis
    bool IsHorizontalLine(List<Vector2> points)
    {
        // Must first pass the "IsLine" test so we know it's not too wiggly
        if (!IsLine(points)) return false;

        Vector2 startPoint = points[0];
        Vector2 endPoint = points[points.Count - 1];

        Vector2 direction = (endPoint - startPoint).normalized;

        // Angle with the X-axis is how "horizontal" it is
        float angleFromX = Vector2.Angle(direction, Vector2.right);

        // If it's less than, say, 10° from the X-axis or close to 180°, call it horizontal
        // Adjust angleThreshold up/down to taste
        float angleThreshold = 10f;
        if (angleFromX <= angleThreshold || angleFromX >= 180f - angleThreshold)
        {
            return true;
        }
        return false;
    }

    // 3) Vertical line? Check angle vs. Y-axis
    bool IsVerticalLine(List<Vector2> points)
    {
        // Must first pass the "IsLine" test so we know it's not too wiggly
        if (!IsLine(points)) return false;

        Vector2 startPoint = points[0];
        Vector2 endPoint = points[points.Count - 1];

        Vector2 direction = (endPoint - startPoint).normalized;

        // Angle with the Y-axis
        float angleFromY = Vector2.Angle(direction, Vector2.up);

        // If it's less than ~10° from the Y-axis or close to 180°, call it vertical
        float angleThreshold = 10f;
        if (angleFromY <= angleThreshold || angleFromY >= 180f - angleThreshold)
        {
            return true;
        }
        return false;
    }

    // 4) One sharp corner => "V"
    bool IsVShape(List<Vector2> points)
    {
        int cornerCount = 0;

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

    // Distance from a point to a line segment
    float DistanceFromLineSegment(Vector2 point, Vector2 segStart, Vector2 segEnd)
    {
        float lineLengthSq = (segEnd - segStart).sqrMagnitude;
        if (lineLengthSq < Mathf.Epsilon)
        {
            return Vector2.Distance(point, segStart);
        }

        float t = Vector2.Dot(point - segStart, segEnd - segStart) / lineLengthSq;
        t = Mathf.Clamp01(t);

        Vector2 projection = segStart + t * (segEnd - segStart);
        return Vector2.Distance(point, projection);
    }
}
