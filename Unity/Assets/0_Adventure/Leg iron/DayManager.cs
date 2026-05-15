using UnityEngine;

public class DayManager : MonoBehaviour
{
    public int CurrentDay = 1;
    public void GoToNextDay() => CurrentDay++;
}
