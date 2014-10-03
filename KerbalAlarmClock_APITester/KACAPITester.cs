﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

using KACAPITester_KACWrapper;

namespace KerbalAlarmClock_APITester
{
    [KSPAddon(KSPAddon.Startup.Flight, false),
    WindowInitials(Visible = true, Caption = "KAC API Tester", DragEnabled = true)]
    public class KACAPITester : MonoBehaviourWindow
    {
        internal override void Start()
        {
            LogFormatted("Start");
            KACWrapper.InitKACWrapper();

            //Register the event handler
            KACWrapper.KAC.onAlarmStateChanged += KAC_onAlarmStateChanged;
        }

        void KAC_onAlarmStateChanged(KACWrapper.KACAPI.AlarmStateChangedEventArgs e)
        {
            //output whats happened
            LogFormatted("{0}->{1}", e.alarm.Name, e.eventType);
        }


        internal override void Awake()
        {
            WindowRect = new Rect(600, 100, 300, 200);
        }

        internal override void OnDestroy()
        {
            //destroy the event hook
            KACWrapper.KAC.onAlarmStateChanged -= KAC_onAlarmStateChanged;
        }

        internal override void DrawWindow(int id)
        {
            GUILayout.Label("Assembly: " + KACWrapper.AssemblyExists.ToString());
            GUILayout.Label("Instance: " + KACWrapper.InstanceExists.ToString());
            GUILayout.Label("APIReady: " + KACWrapper.APIReady.ToString());

            //ifthe API hooked
            if (KACWrapper.APIReady)
            {
                //Draw the alarms
                foreach (KACWrapper.KACAPI.KACAlarm a in KACWrapper.KAC.Alarms)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(String.Format("{0}-{1}-{2} ({3})", a.Name, a.AlarmType,a.Notes,a.ID  ));
                    
                    //Option to delete each alarm
                    if (GUILayout.Button("Delete",GUILayout.Width(50))) {
                            KACWrapper.KAC.DeleteAlarm(a.ID);
                        }
                    GUILayout.EndHorizontal();
                }

                //option to create a new alarm
                if (GUILayout.Button("Create One"))
                {
                    String aID = KACWrapper.KAC.CreateAlarm(KACWrapper.KACAPI.AlarmTypeEnum.TransferModelled, "Test",Planetarium.GetUniversalTime()+900);
                    
                    KACWrapper.KAC.Alarms.First(z=>z.ID==aID).Notes = "FRED FLINTSTONE";

                }
            }
        }
    }
}
