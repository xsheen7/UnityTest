using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class ProfilerTest : MonoBehaviour
{
    int _count = 0;

    void Start()
    {
        Profiler.logFile = "";
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.H))
        {
            StopAllCoroutines();
            _count = 0;
            StartCoroutine(SaveProfilerData());
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Profiler.enabled = false;
            Profiler.enableBinaryLog = false;
            Profiler.logFile = null;
            StopAllCoroutines();
        }
    }

    IEnumerator SaveProfilerData()
    {
        Debug.Log("start profiler");
        // keep calling this method until Play Mode stops
        while (true)
        {
            // generate the file path
            string filepath = Application.persistentDataPath + "/profilerLog" + _count;

            // set the log file and enable the profiler
            Profiler.logFile = filepath;
            Profiler.enableBinaryLog = true;
            Profiler.enabled = true;

            // count 300 frames
            for (int i = 0; i < 300; ++i)
            {
                yield return new WaitForEndOfFrame();

                // workaround to keep the Profiler working
                if (!Profiler.enabled)
                    Profiler.enabled = true;
            }

            Debug.Log("log finish:" + _count);
            // start again using the next file name
            _count++;
        }
    }
}