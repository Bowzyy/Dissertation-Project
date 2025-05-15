using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // player 
    public float offset = 2f; 

    void Update()
    {
        if (player.position.y > transform.position.y - offset)
        {
            // follow the player
            transform.position = new Vector3(transform.position.x, player.position.y + offset, transform.position.z);
        }
    }
}
