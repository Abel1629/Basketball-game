using UnityEngine;
using System.Collections.Generic;

public class PosessionChanger : MonoBehaviour
{
    // Refferrences
    [Header("Refferrences")]
    [SerializeField] private GameObject Team1;
    [SerializeField] private GameObject Team2;
    [SerializeField] private GameObject ShotClockTimerText;

    // Components
    private TeamStats team1Stats;
    private TeamStats team2Stats;
    private TimerController timerController;

    // Variables
    private bool team1Posession;
    private bool team2Posession;

    private void Start()
    {
        team1Stats = Team1.GetComponent<TeamStats>();
        team2Stats = Team2.GetComponent<TeamStats>();

        team1Posession = team1Stats.getHasPosession();
        team2Posession = team2Stats.getHasPosession();

        timerController = ShotClockTimerText.GetComponent<TimerController>();
    }

    public void ChangePosession() // when called it changes the posession
    {
        bool variable = team1Posession;
        team1Posession = team2Posession;
        team2Posession = variable;

        timerController.ResetShotClock(); // reseting the shot clock
    }

    // this is used in the score controller to check which team has the posession
    // it will return a list containing two integers
    // [0, 1] or [1, 0]
    //  ^  ^
    //  |  |
    //  |  if the enemy(guest) team has got the posession(they are attacking)
    //  if the players(home) team has got the posession(they are attacking)
    public List<int> getTeamPosessionList()
    {
        List<int> teamPosessionList = new List<int>() { 0, 0};

        if (team1Posession)
            teamPosessionList[0] = 1;

        else
            teamPosessionList[1] = 1;

        return teamPosessionList;
    }
}
