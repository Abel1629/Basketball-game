using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject Ball;

    private BallController ballController;
    private float timerSeconds;
    private TextMeshPro timerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerText = GetComponent<TextMeshPro>();
        timerSeconds = 14f;
        timerText.text = timerSeconds.ToString();
        ballController = Ball.GetComponent<BallController>(); // referring to the basketball
    }

    // Update is called once per frame
    void Update()
    {
        if (timerSeconds > 0) // the shot clock will count towards zero
        {
            timerSeconds -= Time.deltaTime;
            timerSeconds = Mathf.Max(timerSeconds, 0); // it ensures the timer will never go below zero
        }

        if (timerSeconds < 10) // changing the format of the text
        {
            if (timerSeconds < 5) // changing the color of the text
            {
                timerText.color = Color.red;
            }
            timerText.text = timerSeconds.ToString("F1");
        }

        else
        {
            timerText.color = Color.white;
            float roundedSeconds = Mathf.Round(timerSeconds);
            timerText.text = roundedSeconds.ToString();
        }

    }

    public float GetShotclockTimer()
    {
        return timerSeconds;
    }

    public void ResetShotClock() // Reseting the shot clock
    {
        timerSeconds = 14f;
    }
}

