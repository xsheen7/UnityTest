using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.WebRequest;
using UnityEngine;

public class GameFrameWorkTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameFrameworkEntry.GetModule<WebRequestManager>().AddWebRequest("www.baidu.com");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
