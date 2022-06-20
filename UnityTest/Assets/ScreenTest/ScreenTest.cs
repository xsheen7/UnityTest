using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"screen width={Screen.width},screen height={Screen.height},screen resolution={Screen.currentResolution},screen dpi={Screen.dpi},screen safe={Screen.safeArea}");
        Debug.Log($"lan={Application.systemLanguage}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
