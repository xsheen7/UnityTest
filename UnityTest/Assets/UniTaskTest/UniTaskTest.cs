using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UniTaskTest : MonoBehaviour
{

    public GameObject obj;
    
    // Start is called before the first frame update
    void Start()
    {
        TestTaskVoid(CancellationToken.None).Forget();
        Destroy(gameObject,0.5f);//不设置 cancle异步接着执行
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
    }

    async UniTask TestTask()
    {
        await UniTask.DelayFrame(10);
    }

    async UniTaskVoid TestTaskVoid(CancellationToken cancellationToken)
    {
        Debug.Log("start");
        await UniTask.Delay(1000);
        Debug.Log("wait 1s");
        async UniTask<int> GetIntAsync()
        {
            await UniTask.Delay(1000, cancellationToken: cancellationToken);
            return 2;
        }

        int value = await GetIntAsync();
        Debug.Log("wait 2s value:" + value);
        
        Debug.Log("obj :" + obj.name);
    }
}
