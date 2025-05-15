using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// destotry enemies and food when out of bounds

public class DestroyOffScreen : MonoBehaviour
{
    public float sprite_screen_offset = 0;

    void Update()
    {
        if (Camera.main != null && transform.position.y < Camera.main.transform.position.y - 
            Camera.main.orthographicSize - transform.localScale.y - sprite_screen_offset)
        {
            Destroy(gameObject);
        }
    }
}
