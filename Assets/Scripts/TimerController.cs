using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject Ball;

    private BallController ballController;
    private float timerSeconds;
    private TextMeshPro timerText;

    //private bool selfPosession = true;
    //private bool enemyPosession = false;

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
        if (ballController.getShotClockReset()) // if the shot clock needs to be reset
        {
            timerSeconds = 14f;
            ballController.setShotClockReset(false);
        }
            

        else if (timerSeconds > 0) // the shot clock will count towards zero
        {
            timerSeconds -= Time.deltaTime;
            timerSeconds = Mathf.Max(timerSeconds, 0);
        }

        if (timerSeconds < 10)
        {
            if (timerSeconds < 5)
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
}

