using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

#pragma warning disable 0649


public class DayNightUICycle : MonoBehaviour
{

    [SerializeField] private RectTransform MiddayObject;
    [SerializeField] private RectTransform MidnightObject;
    [SerializeField] private RectTransform DawnObject;
    [SerializeField] private RectTransform DuskObject;
    private double Radius = 150;
    private double MiddayPhase = -Math.PI * 0.5;
    private double MiddayDefaultValue = -Math.PI * 0.5;
    private double MidnightPhase = Mathf.PI * 0.5;
    private double MidnightDefaultValue = Math.PI * 0.5;
    private double DawnPhase = 0;
    private double DawnDefaultValue = 0;
    private double DuskPhase = Mathf.PI;
    private double DuskDefaultValue = Math.PI;
    private double Phase = 0;
    private double GameTimeSpeedCoeff = Math.PI / 12;
    private GameTimeManager TimeManager;
    public void Init(GameTimeManager Manager)
    {
        TimeManager = Manager;
    }
    private void LateUpdate()
    {
        var step = -TimeManager.DeltaTime * GameTimeSpeedCoeff;
        Phase += step;

        UpdatePosition(MiddayObject, MiddayDefaultValue, MiddayObject);
        UpdatePosition(MidnightObject, MidnightDefaultValue, MidnightObject);
        UpdatePosition(DuskObject, DuskDefaultValue, DuskObject);
        UpdatePosition(DawnObject, DawnDefaultValue, DawnObject);
    }
    public void UpdatePosition(RectTransform Object, double TimePhase, RectTransform CurObj)
    {
        if (CurObj != null)
        {
            double x = Math.Cos(Phase + TimePhase) * Radius;
            double y = Math.Sin(Phase + TimePhase) * Radius;
            Object.transform.localPosition = new Vector3((float)x, (float)y, 0f);
        }
    }
}
