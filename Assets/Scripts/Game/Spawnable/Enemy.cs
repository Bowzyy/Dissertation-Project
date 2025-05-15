using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AudioClip hit_sound;
    public float sound_volume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("hit player");

            // play hit
            if (hit_sound != null)
            {
                AudioSource.PlayClipAtPoint(hit_sound, transform.position, sound_volume);
            }
            else
            {
                Debug.LogWarning("No hit_sound assigned on " + name);
            }

            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // disable the player controller so no more input can be done
                PlayerController player_controller = other.GetComponent<PlayerController>();
                if (player_controller != null)
                {
                    player_controller.enabled = false;
                }

                // disable the tap detection as can still jump using this after death
                GameObject detect_tap_object = GameObject.Find("DetectTap");
                if (detect_tap_object != null)
                {
                    detect_tap_object.SetActive(false);
                }

                playerRb.velocity = Vector2.zero;   //stop
            }
        }
    }
}
