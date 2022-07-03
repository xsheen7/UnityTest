using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyCoroutineTest : MonoBehaviour
{
    public SpriteRenderer sp;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        Debug.Log("start my wait");
        UnityWebRequest request = UnityWebRequest.Get("https://gimg2.baidu.com/image_search/src=http%3A%2F%2F6.pic.pc6.com%2Fthumb%2Fn331nwy32s1w21th019%2F7ef615fe2655bd2a_600_0.jpeg&refer=http%3A%2F%2F6.pic.pc6.com&app=2002&size=f9999,10000&q=a80&n=0&g=0n&fmt=auto?sec=1659434720&t=96fcfe7ace0ba648c2f40cc258cdd795");

        // yield return request.SendWebRequest();
        // Debug.Log(request.downloadHandler.data.Length);
        
        MyWait myWait = new MyWait(3, request);
        yield return myWait;
        Texture2D texture2D = new Texture2D(500, 500);
        texture2D.LoadImage(request.downloadHandler.data);
        sp.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, 500, 300), new Vector2(0.5f, 0.5f));
        Debug.Log("stop my wait");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
