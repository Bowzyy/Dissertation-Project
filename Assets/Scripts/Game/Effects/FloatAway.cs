using UnityEngine;

public class FloatAway : MonoBehaviour
{
    // make the frog float away

    public float speed = 2f;  // speed lady frog will go up
    public float frequency = 2f; 
    public float amplitude = 0.5f; 
    private float start_point;

    void Start()
    {
        start_point = transform.position.x;
    }

    void Update()
    {
        // move frog up
        transform.position += Vector3.up * speed * Time.deltaTime;

        // make it wave side to side 
        float newX = start_point + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // remove when above camera bounds
        if (Camera.main != null && transform.position.y > Camera.main.orthographicSize + transform.localScale.y + 2)
        {
            Destroy(gameObject);
        }
    }
}
