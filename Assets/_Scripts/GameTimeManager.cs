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
    }
    private void FixedUpdate()
    {
        if (SpeedInd == 0)
            return;

        if (TimeSinceLastUpdate > 1f)
        {
            TimeSinceLastUpdate -= 1f;
            if (SpeedInd > 0)
            {
                CurrentTime.Change();
                FillDatesText();
                CurrentTimeText.text = string.Format("{0}:00", CurrentTime.Hour);
            }
        }
        TimeSinceLastUpdate += DeltaTime;
    }
    public void ButtonsPressed()
    {
        if ((Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus)) && SpeedInd != 0)
            ChangeTimeFlow(1);

        if ((Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus)) && SpeedInd > 1)
            ChangeTimeFlow(-1);

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
            }
        }
    }
    public void FillDatesText()
    {
        CurrentTimeTextDay.text = (CurrentTime.Day + 1).ToString("0");
        CurrentTimeTextMonth.text = (CurrentTime.Month).ToString();
        CurrentTimeTextYear.text = (CurrentTime.Year).ToString("0");
    }
    void ChangeTimeFlow(int SpeedIndEnc)
    {
        SpeedInd += SpeedIndEnc;
        SpeedInd = Mathf.Clamp(SpeedInd, 0, UpdateIntervals.Length - 1);
        Time.timeScale = 0.5f * (SpeedInd + 1);
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
    public void ClampValidate()
    {
        ChangePox(ref Hour, 24, 0);
        ChangePox(ref Day,30,1);
        ChangePox(ref Month, 12, 0);
        ChangePox(ref Year, 2000, 0);
    }
    void ChangePox(ref int Obj, int ObjMax, int ObjMin)
    {
        Obj = Mathf.Max(ObjMin, Obj);
        Obj = Mathf.Min(ObjMax, Obj);
    }
    void ChangePox(ref MonthName month, int ObjMax, int ObjMin)
    {
        month = (MonthName)Mathf.Max(ObjMin, (int)month);
        month = (MonthName)Mathf.Min(ObjMin, (int)month);
    }

    private void PassValidate()
    {
        if (Hour >= 24)
        {
            Hour -= 24;
            Day++;
        }
        if (Day > GameTimeManager.DaysInAMonth[(int)Month])
        {
            Day -= GameTimeManager.DaysInAMonth[(int)Month];
            Month++;
        }
        if ((int)Month >= 12)
        {
            Month -= 12;
            Year++;
        }
    }
    public void Change()
    {
        Hour += 1;
        PassValidate();
    }
    public enum MonthName
    {
        Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
    }
}
