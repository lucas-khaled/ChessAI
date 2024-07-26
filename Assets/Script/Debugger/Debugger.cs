using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Debugger
{
    private static Dictionary<string, TimeInfo> timeDict = new();

    public static void StartTimer(string timerTag) 
    {
        DateTime now = DateTime.Now;
        if (timeDict.ContainsKey(timerTag)) 
        {
            timeDict[timerTag].firstMeasureTime = now;
            timeDict[timerTag].lastMeasureTime = now;
            return;
        }

        TimeInfo info = new TimeInfo()
        {
            firstMeasureTime = now,
            lastMeasureTime = now
        };

        timeDict.Add(timerTag, info);
    }

    public static void StopTimer(string timerTag) 
    {
        if (timeDict.ContainsKey(timerTag) is false)
        {
            Debug.LogError($"No timer was initialized with the tag {timerTag}");
            return;
        }

        timeDict[timerTag].lastMeasureTime = DateTime.Now;
    }

    public static void DebugTimer(string timerTag, string message = "", bool alsoStop = false) 
    {
        if (timeDict.ContainsKey(timerTag) is false) 
        {
            Debug.LogError($"No timer was initialized with the tag {timerTag}");
            return;
        }

        if (alsoStop) 
        {
            StopTimer(timerTag);
        }

        Debug.Log(message + timeDict[timerTag].DifferenceMS + "ms");
    }

    private class TimeInfo 
    {
        public DateTime firstMeasureTime;
        public DateTime lastMeasureTime;
        public TimeSpan TimeDifference => lastMeasureTime - firstMeasureTime;
        public double DifferenceMS => TimeDifference.TotalMilliseconds;
    }
}
