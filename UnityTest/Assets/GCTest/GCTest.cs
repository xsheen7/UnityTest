using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GCTest : MonoBehaviour
{
    public List<People> peoples = new List<People>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Serializable]
    public class People
    {
        public string name;
        public int age;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            peoples.Add(new People()
            {
                age = i,
                name = "people_"+i
            });
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            peoples.Clear();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            System.GC.Collect();
        }
    }
}
