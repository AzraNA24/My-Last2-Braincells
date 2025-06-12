using UnityEngine;

public class CritterIdleAnimation : MonoBehaviour
{
    private Vector3 originalPosition;
    [SerializeField] private float minFloatHeight = 0.2f; 
    [SerializeField] private float maxFloatHeight = 0.5f; 
    [SerializeField] private float minFloatSpeed = 0.8f;  
    [SerializeField] private float maxFloatSpeed = 1.5f;  

    private float floatHeight;  
    private float floatSpeed; 

    void Start()
    {
        originalPosition = transform.position;
        
        // Set random height dan speed
        floatHeight = Mathf.Lerp(minFloatHeight, maxFloatHeight, Random.Range(0f, 1f) * Random.Range(0f, 1f));
        floatSpeed = Mathf.Lerp(minFloatSpeed, maxFloatSpeed, Random.Range(0f, 1f) * Random.Range(0f, 1f));
    }

    void Update()
    {
        // Gerakan naik-turun menggunakan Mathf.Sin
        float newY = originalPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
    }
}