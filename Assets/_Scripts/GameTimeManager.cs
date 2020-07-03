using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649

public class GameTimeManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI CurrentTimeTextDay;
    [SerializeField] private TMPro.TextMeshProUGUI CurrentTimeTextMonth;
    [SerializeField] private TMPro.TextMeshProUGUI CurrentTimeTextYear;
    [SerializeField] private Image CurrentTimeFillImage;
    public int StartDay;
    public GameTimeDate.MonthName StartMonth;
    public int StartYear;
    public int StartGameSpeed = 0;
    public int SpeedBeforePause;
    private int SpeedInd;
    public int GameSpeed { get { return SpeedInd; } }

    private static float[] UpdateIntervals = { float.PositiveInfinity, 2f, 1f, 0.5f };
    public GameTimeDate CurrentTime;
    public static string[] MonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    public static int[] DaysInAMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    private float TimeSinceLastUpdate = 0f;
    public float DeltaTime { get { return Time.deltaTime / UpdateIntervals[SpeedInd]; } }

    public DayNightUICycle DayNightCycleCarousel;
    public TMPro.TextMeshProUGUI CurrentTimeText;
    private void Start()
    {
        CurrentTime = new GameTimeDate(0, StartDay, StartMonth, StartYear);

        SpeedInd = SpeedBeforePause = StartGameSpeed;

        FillDatesText();

        DayNightCycleCarousel.Init(this);
    }


    private void Update()
    {
        ButtonsPressed();

        
        //CurrentTimeFillImage.fillAmount = (m_currentTime.Hour + (Time.time - m_lastUpdateTime) / UpdateIntervals[SpeedInd]) / 24f;
    }
    private void FixedUpdate()
    {
        if (SpeedInd == 0)
            return;

        if (TimeSinceLastUpdate > 1f)
        {
            TimeSinceLastUpdate = 0f;
            if (SpeedInd > 0)
            {
                CurrentTime.Change();
                FillDatesText();
                CurrentTimeText.text = string.Format("{0}:00", CurrentTime.Hour);
            }
        }
        TimeSinceLastUpdate += DeltaTime;
    }

    public void FillDatesText()
    {
        CurrentTimeTextDay.text = (CurrentTime.Day + 1).ToString("0");
        CurrentTimeTextMonth.text = (CurrentTime.Month).ToString();
        CurrentTimeTextYear.text = (CurrentTime.Year).ToString("0");
    }

    public void ButtonsPressed()
    {
        if ((Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus)) && SpeedInd != 0)
        {
            SpeedInd += 1;
            SpeedInd = Mathf.Clamp(SpeedInd, 0, UpdateIntervals.Length - 1);
            Time.timeScale = 0.5f * (SpeedInd + 1);
        }

        if ((Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus)) && SpeedInd > 1)
        {
            SpeedInd += -1;
            SpeedInd = Mathf.Clamp(SpeedInd, 0, UpdateIntervals.Length - 1);
            Time.timeScale = 0.5f * (SpeedInd + 1);
        }

        if (Input.GetKeyDown((KeyCode.Space)))
        {
            if (SpeedInd != 0)
            {
                SpeedBeforePause = SpeedInd;
                SpeedInd = 0;
            }
            else
            {
                SpeedInd = SpeedBeforePause;
                SpeedBeforePause = -1;
            }
        }
    }
}





[System.Serializable]
public class GameTimeDate
{
    public int Hour;
    public int Day;
    public MonthName Month;
    public int Year;

    public GameTimeDate(int NewHour, int NewDay, MonthName NewMonth, int NewYear)
    {
        Hour = NewHour;
        Day = NewDay;
        Month = NewMonth;
        Year = NewYear;
        ClampValidate();
    }

    public GameTimeDate(GameTimeDate Other)
    {
        Hour = Other.Hour;
        Day = Other.Day;
        Month = Other.Month;
        Year = Other.Year;
        ClampValidate();
    }

    public void ClampValidate()
    {
        Hour = Mathf.Max(0, Hour);
        Hour = Mathf.Min(24, Hour);

        Day = Mathf.Max(1, Day);
        Day = Mathf.Min(30, Day);

        Month = (MonthName)Mathf.Max(0, (int)Month);
        Month = (MonthName)Mathf.Min(12, (int)Month);

        Year = Mathf.Max(0, Year);
        Year = Mathf.Min(2000, Year);
    }

    private void PassValidate()
    {
        if (Hour >= 24)
        {
            Hour = 0;
            Day++;
        }

        if (Day > GameTimeManager.DaysInAMonth[(int)Month])
        {
            Day = 0;
            Month++;
        }

        if ((int)Month >= 12)
        {
            Month = 0;
            Year++;
        }
    }

    public static GameTimeDate operator -(GameTimeDate C1, GameTimeDate C2)
    {
        var day = 0;
        var year = 0;//C1.Year - C2.Year;
        var month = 0;// C1.Month - C2.Month;

        day += C1.Day - C2.Day;
        if (day < 1)
        {
            day += 30;
            month--;
        }

        month += C1.Month - C2.Month;
        if (month < 0)
        {
            month += 12;
            year--;
        }

        year += C1.Year - C2.Year;

        return new GameTimeDate(0, day, (MonthName)month, year);
    }

    public static GameTimeDate ParseDaysIntoDate(int DayCount)
    {
        var day = DayCount % 30;
        var month = DayCount / 30;
        var year = DayCount / 360;

        return new GameTimeDate(0, day, (MonthName)month, year);
    }


    public void Change()
    {
        Hour += 1;
        PassValidate();
    }

    public string ToDateString()
    {
        return string.Format("{0} {1} {2}", Day, Month.ToString(), Year);
    }

    public string ToTimeLeftString()
    {
        if (Year == 0)
        {
            if (Month == 0)
            {
                return string.Format("{0} days", Day);
            }
            return string.Format("{0} months {1} days", Month, Day);
        }
        if (Month == 0)
            return string.Format("{0} years {1} days", Year, Day);

        return string.Format("{0} years {1} months {2} days", Year, Month, Day);
    }

    public enum MonthName
    {
        Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
    }
}
