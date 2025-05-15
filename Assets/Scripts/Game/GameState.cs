using UnityEngine;

public class GameState : MonoBehaviour
{
    public Transform player;
    public GameObject game_over_UI;

    private Camera mainCamera;
    private float camera_height;
    private bool game_over = false;

    void Start()
    {
        mainCamera = Camera.main;
        camera_height = mainCamera.orthographicSize;

        // hide ui at beginning
        if (game_over_UI != null)
            game_over_UI.SetActive(false);
    }

    void Update()
    {
        if (game_over || player == null)
            return;

        float bottomY = mainCamera.transform.position.y - camera_height;

        // check if the player has fallen below the screen
        if (player.position.y < bottomY)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        game_over = true;

        if (game_over_UI != null)
            game_over_UI.SetActive(true);

        GameObject detect_tap_object = GameObject.Find("DetectTap");
        if (detect_tap_object != null)
        {
            detect_tap_object.SetActive(false);
        }
    }
}
