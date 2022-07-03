using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyWait : CustomYieldInstruction
{
    public bool finish;
    private float m_WaitTime;
    private UnityWebRequest m_Web;

    public MyWait(float time,UnityWebRequest web)
    {
        m_WaitTime = time;
        m_Web = web;
        m_Web.SendWebRequest();
    }

    //默认函数
    public override void Reset()
    {
        Debug.Log($"reset: {m_Web.downloadHandler.data.Length}");
        m_WaitTime = 0;
        finish = false;
    }

    public override bool keepWaiting
    {
        get
        {
            m_WaitTime -= Time.deltaTime;
            finish = m_Web.isDone;
            if (finish)
            {
                Reset();
                return false;
            }
            return true;
        }
    }

}
