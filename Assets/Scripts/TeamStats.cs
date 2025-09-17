using UnityEngine;

public class TeamStats : MonoBehaviour
{
    private float posessionBlocker = 0f; // Counting how long the posession is blocked for the team
    [SerializeField] private bool hasPosession = true; // checking if the team has got the posession

    private void Update()
    {
        // it will block the team to get the posession until it goes down to 0
        if (posessionBlocker > 0)
        {
            posessionBlocker -= Time.deltaTime;
            posessionBlocker = Mathf.Max(posessionBlocker, 0);
        }
    }

    // blocking the team posession
    public void SetTeamPosessionBlocked()
    {
        posessionBlocker = 3f;
    }

    public bool GetIsTeamPosessionBlocked()
    {
        return (posessionBlocker > 0);
    }

    public void setHasPosession(bool value)
    {
        hasPosession = value;
    }

    public bool getHasPosession()
    {
        return hasPosession;
    }
}
