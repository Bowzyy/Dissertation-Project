using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float jump_force = 10f;
    public float gravity = 1f;
    public float screen_padding = 0.1f; // padding for how far the player can jump from the screen

    // points for the player to jump towards
    public GameObject left_point;
    public GameObject right_point;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private float camera_width;

    private bool first_jump = true;
    public float number_of_jumps = 0;

    //stamina
    public Image stamina_bar;
    public float stamina;
    public float max_stamina;
    public float jump_cost;

    // animation and shadows

    public Sprite player_jump_sprite;
    public Sprite player_idle_sprite;
    public Sprite shadow_jump_sprite;
    public Sprite shadow_idle_sprite;

    private SpriteRenderer sprite_renderer;
    private SpriteRenderer shadow_sprite_renderer;

    public GameObject shadow_object;

    private float start_time;

    void Start()
    {
        start_time = Time.time;  // when game starts

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // set gravity to 0 so the player doesn't fall before the game starts

        // get values to ensure player doesn't go off screen 
        mainCamera = Camera.main;
        camera_width = mainCamera.orthographicSize * mainCamera.aspect;

        sprite_renderer = GetComponent<SpriteRenderer>();
        if (sprite_renderer != null && player_idle_sprite != null)
        {
            sprite_renderer.sprite = player_idle_sprite;
        }

        if (shadow_object != null)
        {
            shadow_sprite_renderer = shadow_object.GetComponent<SpriteRenderer>();
            if (shadow_sprite_renderer != null && shadow_idle_sprite != null)
            {
                shadow_sprite_renderer.sprite = shadow_idle_sprite;
            }
        }
    }

    void Update()
    {
        // get touch screen input to determine where to jump from
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touch_position = Input.GetTouch(0).position;
            float center_x = Screen.width / 2f;

            if (touch_position.x < center_x)  // left
            {
                Jump(true); // set to left point
            }
            else  // right
            {
                Jump(false); // set to right point
            }
        }

        KeepPlayerInBounds();
        AnimatePlayer();
        FlipPlayer();

        if (stamina < 0)
        {
            stamina = 0;
        }

        stamina_bar.fillAmount = stamina / max_stamina;
    }

    public void Jump(bool jump_left) // true = left tap, false = right tap
    {
        // dont jump firsr 3 secs to let lady frog do anumations
        if (Time.time - start_time < 3f)
        {
            Debug.Log("no jump wait for lady frog 'cutscne'");
            return;
        }

        if (stamina > 0)
        {
            if (first_jump)
            {
                rb.gravityScale = gravity;
                first_jump = false;
            }

            float center_x = Screen.width / 2f;    // center of screen
            float player_x = Camera.main.WorldToScreenPoint(transform.position).x; // where player is on screen

            // if on same side, jump directly up 
            if ((jump_left && player_x < center_x) || (!jump_left && player_x > center_x))
            {
                rb.velocity = Vector2.up * jump_force;
            }
            // if tap is on "opposite" side to the player, jump accross to that side
            else
            {
                Vector2 target_position = jump_left ? left_point.transform.position : right_point.transform.position;
                Vector2 jump_direction = (target_position - (Vector2)transform.position).normalized;
                rb.velocity = jump_direction * jump_force;
            }

            stamina -= jump_cost;
            number_of_jumps++;
        }
    }

    void KeepPlayerInBounds()
    {
        Vector2 player_pos = rb.position;
        if (player_pos.x - screen_padding < -camera_width)
        {
            player_pos.x = -camera_width + screen_padding;
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        else if (player_pos.x + screen_padding > camera_width)
        {
            player_pos.x = camera_width - screen_padding;
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        rb.position = player_pos;
    }

    void AnimatePlayer()
    {
        if (sprite_renderer != null && shadow_sprite_renderer != null)
        {
            if (rb.velocity.y > 0)
            {
                sprite_renderer.sprite = player_jump_sprite;
                shadow_sprite_renderer.sprite = shadow_jump_sprite;
            }
            else
            {
                sprite_renderer.sprite = player_idle_sprite;
                shadow_sprite_renderer.sprite = shadow_idle_sprite;
            }
        }
    }

    void FlipPlayer()
    {
        if (sprite_renderer != null && shadow_sprite_renderer != null)
        {
            if (rb.velocity.x > 0)
            {
                sprite_renderer.flipX = false;
                shadow_sprite_renderer.flipX = false;
            }
            else if (rb.velocity.x < 0)
            {
                sprite_renderer.flipX = true;
                shadow_sprite_renderer.flipX = true;
            }
        }
    }
}
