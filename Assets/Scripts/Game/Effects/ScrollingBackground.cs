using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{

    // IN GAME BACGKROUND NOT MENU ****

    public GameObject[] backgrounds; // list of backgrounds
    public Transform player;
    public float scrolling_speed = 1f; // adjusging for backtorund speed
    private float background_height; // heigh of background 
    private Camera mainCamera;
    private Vector3 new_player_pos;

    void Start()
    {
        mainCamera = Camera.main;
        background_height = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
        new_player_pos = player.position;
    }

    void Update()
    {
        float playerMovementDelta = player.position.y - new_player_pos.y;

        if (Mathf.Abs(playerMovementDelta) > 0.01f)
        {
            foreach (GameObject bg in backgrounds)
            {
                bg.transform.position += Vector3.down * playerMovementDelta * scrolling_speed;
            }
            RepositionBackground();
        }

        new_player_pos = player.position;
    }

    void RepositionBackground()
    {

        System.Array.Sort(backgrounds, (a, b) => a.transform.position.y.CompareTo(b.transform.position.y));

        GameObject lowest = backgrounds[0];
        GameObject heighest = backgrounds[backgrounds.Length - 1];

        // move on top of the highest background
        if (lowest.transform.position.y <= mainCamera.transform.position.y - background_height)
        {
            lowest.transform.position = new Vector3(
                heighest.transform.position.x,
                heighest.transform.position.y + background_height,
                heighest.transform.position.z
            );
            ShiftArrayLeft();
        }

        // move below lowest bacgkround 
        if (heighest.transform.position.y >= mainCamera.transform.position.y + background_height)
        {
            heighest.transform.position = new Vector3(
                lowest.transform.position.x,
                lowest.transform.position.y - background_height,
                lowest.transform.position.z
            );
            ShiftArrayRight();
        }
    }


    // circle queue
    void ShiftArrayLeft()
    {
        GameObject first = backgrounds[0];

        for (int i = 0; i < backgrounds.Length - 1; i++)
        {
            backgrounds[i] = backgrounds[i + 1];
        }

        backgrounds[backgrounds.Length - 1] = first;
    }

    void ShiftArrayRight()
    {
        GameObject last = backgrounds[backgrounds.Length - 1];

        for (int i = backgrounds.Length - 1; i > 0; i--)
        {
            backgrounds[i] = backgrounds[i - 1];
        }

        backgrounds[0] = last;
    }
}
