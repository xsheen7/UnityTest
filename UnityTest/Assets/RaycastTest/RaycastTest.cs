using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Mesh mesh = GetComponent<MeshFilter>().mesh;
        // for (int i = 0; i < mesh.vertexCount; i++)
        // {
        //     Debug.Log("vertext "+i+": vertex="+mesh.vertices[i]+" uv="+mesh.uv[i]);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("pos:"+hit.point+" uv: "+hit.textureCoord);//物体必须有mesh collider
            }
        }
    }
}
