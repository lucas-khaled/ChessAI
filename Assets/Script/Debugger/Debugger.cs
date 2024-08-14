using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class Debugger
{
    private static Dictionary<string, TimeRecord> m_timeRecordDict = new Dictionary<string, TimeRecord>();
    public static void LogStopwatch(Stopwatch stopwatch, string message = "", bool record = false)
    {
        if (record) 
            RecordTime(stopwatch, message);

        UnityEngine.Debug.Log($"<color=#7a378b> {message} in {stopwatch.ElapsedMilliseconds} ms</color>");
    }

    public static void LogTimeRecord(string tag, string message = "") 
    {
        var record = GetRecord(tag);
        if (record == null) return;

        LogTimeRecord(record, message);
    }

    public static void LogTimeRecord(TimeRecord record, string message = "") 
    {
        UnityEngine.Debug.Log($"<color=#800080><b>{message}: {record}</b></color>");
    }

    private static void RecordTime(Stopwatch stopwatch, string tag) 
    {
        if (m_timeRecordDict.ContainsKey(tag)) 
        {
            m_timeRecordDict[tag].AddTime(stopwatch.ElapsedMilliseconds);
            return;
        }

        m_timeRecordDict.Add(tag, new TimeRecord());
        m_timeRecordDict[tag].AddTime(stopwatch.ElapsedMilliseconds);
    }

    public static TimeRecord GetRecord(string tag)
    {
        if(m_timeRecordDict.ContainsKey(tag) is false)
        {
            UnityEngine.Debug.LogError($"There is no record tagged {tag}");
            return null;
        }
        return m_timeRecordDict[tag];
    }

    public class TimeRecord 
    {
        public int timesCalled = 0;
        public float totalTime = 0;

        public float GetTotalTime() => totalTime;
        public float GetMediumTime() => totalTime / timesCalled;
        public void AddTime(float time) 
        {
            timesCalled++;
            totalTime += time;
        }

        public override string ToString()
        {
            return $"Took {totalTime}ms and was called {timesCalled} times. Medium Time is {GetMediumTime()}ms";
        }
    }
}
