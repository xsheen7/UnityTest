using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    private IEnumerator enumerator;
    
    // Start is called before the first frame update
    void Start()
    {
        enumerator = MyCoroutine();
        enumerator.MoveNext();
        Debug.Log("start :"+enumerator.Current.ToString());
    }

    private float timer;
    
    // Update is called once per frame
    void Update()
    {
        if (enumerator == null)
        {
            return;
        }
        
        timer += Time.deltaTime;
        if ((int)enumerator.Current <=timer )
        {
            if (!enumerator.MoveNext())
            {
                enumerator = null;
                Debug.Log("finish");
            }
        }
    }

    IEnumerator MyCoroutine()
    {
        yield return 3;
    }
}
