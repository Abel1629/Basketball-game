using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private GameObject PosessionManager;

    private PosessionChanger posessionChanger;

    private TextMeshPro text;
    private int homeScore = 0;
    private int guestScore = 0;

    private void Start()
    {
        text = GetComponent<TextMeshPro>();
        posessionChanger = PosessionManager.GetComponent<PosessionChanger>();
    }

    public void ChangeScore(int shotType) // shotType = 1- dunk, 2- two pointer, 3- three pointer
    {
        List<int> posessionList = new List<int>(posessionChanger.getTeamPosessionList()); // it will get a list containing two integers based on which team has the posession

        if (shotType == 1) // if the shot type is a dunk, it will count two points
            shotType = 2;

        homeScore += posessionList[0] * shotType; // adding the score
        guestScore += posessionList[1] * shotType; // adding the score

        text.text = homeScore.ToString("D2") + " - " + guestScore.ToString("D2"); // decimal with at least 2 digits, pad with zeros if needed

    }
}
