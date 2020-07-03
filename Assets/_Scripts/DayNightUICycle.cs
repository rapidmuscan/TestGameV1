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
     public double Radius = 50;
     public double MiddayPhase;
     public double MiddayDefaultValue;
     public double MidnightPhase;
     public double MidnightDefaultValue;
     public double DawnPhase;
     public double DawnDefaultValue;
     public double DuskPhase;
     public double DuskDefaultValue;
     public double Phase;

     public double GameTimeSpeedCoeff = 0.102;

     // public float SpeedCorrection = 0.95f;
     private GameTimeManager TimeManager;

     public void Init(GameTimeManager Manager)
     {
          TimeManager = Manager;
          GameTimeSpeedCoeff = Math.PI / 24;
          MiddayDefaultValue = MiddayPhase = -Math.PI * 0.5;
          MidnightDefaultValue = MidnightPhase = Math.PI * 0.5;
          DawnDefaultValue = DawnPhase = 0;
          DuskDefaultValue = DuskPhase = Math.PI;
     }

     private void LateUpdate()
     {
          var step = -TimeManager.DeltaTime * GameTimeSpeedCoeff;
          Phase += step;

          if (MiddayObject != null)
               UpdatePosition(MiddayObject, MiddayDefaultValue);

          if (MidnightObject != null)
               UpdatePosition(MidnightObject, MidnightDefaultValue);

          if (DuskObject != null)
               UpdatePosition(DuskObject, DuskDefaultValue);

          if (DawnObject != null)
               UpdatePosition(DawnObject, DawnDefaultValue);
     }
     
     public void UpdatePosition(RectTransform Object, double TimePhase)
     {
          double x = Math.Cos(Phase + TimePhase) * Radius;
          double y = Math.Sin(Phase + TimePhase) * Radius;
          Object.transform.localPosition = new Vector3((float) x, (float) y, 0f);
     }
}
