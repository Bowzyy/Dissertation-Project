using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemy_prefab;
    public GameObject food_prefab;


    public float range = 3f;
    public float camera_distance = 2f;
    public float radius = 1f;
    public int max_spawn_attempt = 5;

    public float min_gap = 2f;
    public float max_gap = 4f;



    public int enemy_on_screen = 3; // maximum enemies on screen at once
    public int max_objects = 10; // maximum amount of things (inc enemies)


    // how likely things are to spawn
    [Range(0f, 1f)]
    public float food_spawn = 0.7f;
    [Range(0f, 1f)]
    public float enemy_spawn = 0.4f;

    
    // if too many foods have spawned then spawn an enemy
    public int max_food = 3;

    private Camera mainCamera;
    private float next_spawn;
    private int current_enemies = 0;
    private int current_total = 0;
    private int food_since_enemy = 0;

    void Start()
    {
        mainCamera = Camera.main;
        next_spawn = mainCamera.transform.position.y + mainCamera.orthographicSize + camera_distance;
    }

    void Update()
    {
        float top = mainCamera.transform.position.y + mainCamera.orthographicSize + camera_distance;

        if (next_spawn <= top && current_total < max_objects)
        {
            if (TrySpawn(next_spawn))
            {
                next_spawn += Random.Range(min_gap, max_gap);
            }
            else
            {
                next_spawn += min_gap;
            }
        }
    }

    bool TrySpawn(float y)
    {
        for (int i = 0; i < max_spawn_attempt; i++)
        {
            float x = Random.Range(
                mainCamera.transform.position.x - range,
                mainCamera.transform.position.x + range
            );
            Vector2 pos = new Vector2(x, y);

            if (Physics2D.OverlapCircle(pos, radius) != null)
                continue;

            // spawn enemy or food
            bool force_enemy = food_since_enemy >= max_food;
            bool can_spawn_enemy = current_enemies < enemy_on_screen;
            bool random_enemy = Random.value < enemy_spawn && can_spawn_enemy;
            bool need_to_spawn_enemy = force_enemy || random_enemy;

            if (need_to_spawn_enemy)
            {
                if (enemy_prefab == null || enemy_prefab.Length == 0)
                {
                    return false;
                }

                Instantiate(enemy_prefab[Random.Range(0, enemy_prefab.Length)], pos, Quaternion.identity);

                current_enemies++;
                food_since_enemy = 0;
            }
            else
            {
                if (food_prefab == null)
                {
                    return false;
                }
                Instantiate(food_prefab, pos, Quaternion.identity);
                food_since_enemy++;
            }

            current_total++;
            return true;
        }

        // tried all attempts to spawn so
        return false;
    }
}
