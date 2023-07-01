using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathEditor : Editor
{
    [MenuItem("Tools/OpenDataPath")]
    private static void OpenPath()
    {
        string persistentDataPath = Application.persistentDataPath;
        EditorUtility.RevealInFinder(persistentDataPath);
    }
}
