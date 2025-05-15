using UnityEngine;

public class DetectTapInput : MonoBehaviour
{
    private Vector3 acceleration;
    public float tap_threashold = 0.5f; // detect spikes in acceleration
    public float low_pass_filter = 0.1f; // smooth sensitivity
    private Vector3 smoothed_acceleration;

    // bounds for left/right taps
    private float right_bound_lower;
    private float right_bound_upper;
    private float left_bound_lower;
    private float left_bound_upper;

    private float adjusted_angle;

    private float time_since_last_tap = 0f;
    private float tap_cooldown = 0.2f;

    public PlayerController playerController;

    private void Start()
    {
        Input.gyro.enabled = true;
        smoothed_acceleration = Input.acceleration;

        if (PlayerPrefs.HasKey("left_bound_lower"))
        {
            left_bound_lower = PlayerPrefs.GetFloat("left_bound_lower");
            right_bound_lower = PlayerPrefs.GetFloat("right_bound_lower");
            adjusted_angle = PlayerPrefs.GetFloat("baseline_angle");

            Debug.Log($"left [{left_bound_lower}] right [{right_bound_lower}]");
        }
        else
        {
            // use default values
            left_bound_lower = 359.5f;
            right_bound_lower = 0.5f;
            Debug.Log("No calibration data; using default values");
        }
    }

    private void Update()
    {
        time_since_last_tap += Time.deltaTime;
        DetectTap();
    }

    private void DetectTap()
    {
        acceleration = Input.acceleration;
        Quaternion rotation = Input.gyro.attitude;
        Vector3 rotationEuler = rotation.eulerAngles;

        smoothed_acceleration = Vector3.Lerp(smoothed_acceleration, acceleration, low_pass_filter);
        Vector3 accelerationDelta = acceleration - smoothed_acceleration;

        if (accelerationDelta.sqrMagnitude > (tap_threashold * tap_threashold) && time_since_last_tap > tap_cooldown)
        {
            time_since_last_tap = 0f;


            // quicker testing between rotations
            float Angle = rotationEuler.x;

                        // + adjusted angle
            if (Angle > 0.05 && Angle < 5)      // hardcoded values work better ??
            {
                playerController.Jump(false);
                Debug.Log("Tap to the right");
            }
            
            else if (Angle < 359.95 && Angle > 355)
            {
                playerController.Jump(true);
                Debug.Log("Tap to the left");
            }

            /*
            if(Angle > right_bound_lower && Angle < 5)      // hardcoded values work better ??
            {
                playerController.Jump(false);
                Debug.Log("Tap to the right");
            }


            else if (Angle < left_bound_lower && Angle > 355)
            {
                playerController.Jump(true);
                Debug.Log("Tap to the left");
            }
            */
            Debug.Log($"angle: {Angle}");
        }
    }
}
