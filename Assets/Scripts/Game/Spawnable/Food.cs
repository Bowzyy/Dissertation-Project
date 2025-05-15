using UnityEngine;

public class Food : MonoBehaviour
{
    public float add_stamina = 2f;

    
    public AudioClip eat_sound;
    public float sound_volume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("eaten food");

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && player.stamina < player.max_stamina)
            {
                player.stamina += add_stamina;
            }

            if (eat_sound != null)
            {
                AudioSource.PlayClipAtPoint(eat_sound, transform.position, sound_volume);
            }

            Destroy(gameObject);    // remove food
        }
    }
}
