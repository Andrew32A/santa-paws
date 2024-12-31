using UnityEngine;

public class DanglingEffect : MonoBehaviour
{
    // Amplitude of the swing (maximum angle in degrees)
    public float amplitude = 15f;
    // Speed of the swing
    public float swingSpeed = 2f;
    // Original rotation of the object
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial rotation of the object
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Calculate the swing angle using Mathf.Sin
        float angle = amplitude * Mathf.Sin(Time.time * swingSpeed);

        // Apply the swinging effect
        transform.rotation = initialRotation * Quaternion.Euler(0, 0, angle);
    }
}
