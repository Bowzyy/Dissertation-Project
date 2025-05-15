using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalibrateTaps : MonoBehaviour
{
    public TextMeshProUGUI calibration_text;

    public bool is_calibrating = false;
    private int right_taps = 0;
    private int left_taps = 0;

    private float baseline_angle;   // base taken from beginning 
    private float total_right = 0f;
    private float total_left = 0f;

    // thresholds in mapped angle space
    private float right_threashold = 1f;
    private float left_threashold = 359f;

    public Rigidbody2D frog_left;
    public Rigidbody2D frog_right;

    public Sprite player_jump_sprite;
    public Sprite player_idle_sprite;
    public Sprite shadow_jump_sprite;
    public Sprite shadow_idle_sprite;

    public SpriteRenderer sprite_renderer_left;
    public SpriteRenderer shadow_sprite_renderer_left;
    public SpriteRenderer sprite_renderer_right;
    public SpriteRenderer shadow_sprite_renderer_right;

    public GameObject back_button;

    public float jump_force = 5f;
    public float tap_threashold = 0.5f;
    public float low_pass_filter = 0.1f;
    private Vector3 smoothed_acceleration;

    private float time_since_last_tap = 0f;
    private float tap_cooldown = 0.5f;

    private void Start()
    {
        Input.gyro.enabled = true;
        smoothed_acceleration = Input.acceleration;
        back_button.SetActive(false);
        BeginCalibrationAutomatically();
    }

    private void BeginCalibrationAutomatically()
    {
        is_calibrating = true;

        right_taps = 0;
        left_taps = 0;

        total_right = 0f;
        total_left = 0f;

        smoothed_acceleration = Input.acceleration;

        time_since_last_tap = 0f;

        baseline_angle = Input.gyro.attitude.eulerAngles.x;
        Debug.Log($"got base angle: {baseline_angle}°");

        calibration_text.text = "Calibration: Tap right 3 times";
    }

    private void StopCalibration()
    {
        is_calibrating = false;
        Debug.Log($"avergage right: {total_right / 3f}°, average left: {total_left / 3f}°");
        calibration_text.text = "Calibration complete!";
        back_button.SetActive(true);

        PlayerPrefs.SetFloat("right_bound_lower", (total_right / 3) - 0.05f); // ajust this as this is the "average tap", so the bound needs to be slightly lower than it
        PlayerPrefs.SetFloat("left_bound_lower", (total_left / 3) -0.05f);
        PlayerPrefs.SetFloat("baseline_angle", baseline_angle);
        PlayerPrefs.Save();
    }

    private void Update()
    {
        if (is_calibrating)
        {
            CalibrateTap();
        }

        AnimatePlayer();
    }

    private void CalibrateTap()
    {
        time_since_last_tap += Time.deltaTime;
        Vector3 acceleration = Input.acceleration;
        float current_angle = Input.gyro.attitude.eulerAngles.x;

        float shortest_angle = Mathf.DeltaAngle(baseline_angle, current_angle);

        float new_base;
        if (shortest_angle >= 0f)
        {
            new_base = shortest_angle;
        }
        else
        {
            new_base = 360f + shortest_angle;
        }

        baseline_angle = new_base; // save for adjustments in detectp script

        smoothed_acceleration = Vector3.Lerp(smoothed_acceleration, acceleration, low_pass_filter);
        Vector3 delta = acceleration - smoothed_acceleration;

        if (delta.sqrMagnitude > tap_threashold * tap_threashold && time_since_last_tap > tap_cooldown)
        {
            time_since_last_tap = 0f;

            if (right_taps < 3)
            {
                total_right += new_base;
                right_taps++;
                frog_right.AddForce(Vector2.up * jump_force, ForceMode2D.Impulse);
                calibration_text.text = $"Average: {total_right / right_taps}°";
                if (right_taps == 3)
                {
                    calibration_text.text = "Calibration: Tap left 3 times";
                }
            }
            else if (left_taps < 3)
            {
                total_left += new_base;
                left_taps++;
                frog_left.AddForce(Vector2.up * jump_force, ForceMode2D.Impulse);
                calibration_text.text = $"Average: {total_left / left_taps}°";
                if (left_taps == 3)
                {
                    StopCalibration();
                }
            }
        }
    }


    private void AnimatePlayer()
    {
        if (frog_left.velocity.y > 0.1f)
        {
            sprite_renderer_left.sprite = player_jump_sprite;
            shadow_sprite_renderer_left.sprite = shadow_jump_sprite;
        }
        else if (frog_right.velocity.y > 0.1f)
        {
            sprite_renderer_right.sprite = player_jump_sprite;
            shadow_sprite_renderer_right.sprite = shadow_jump_sprite;
        }
        else
        {
            sprite_renderer_left.sprite = player_idle_sprite;
            shadow_sprite_renderer_left.sprite = shadow_idle_sprite;
            sprite_renderer_right.sprite = player_idle_sprite;
            shadow_sprite_renderer_right.sprite = shadow_idle_sprite;
        }
    }
}
