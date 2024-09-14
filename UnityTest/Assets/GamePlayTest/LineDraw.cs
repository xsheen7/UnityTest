using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class LineDraw : MonoBehaviour
{
    private List<List<Vector3>> m_Positions = new List<List<Vector3>>();
    private List<KeyPoint> m_KeyPoints = new List<KeyPoint>();
    
    private const string pathData =
        "M184.41,129.07V283.95M127.91,129H184.31M127.78,129H86.9C43.32,129 8,163.7 8,206.5C8,249.3 43.32,284 86.9,284H126.94M184.5,129H224.1C267.68,129 303,163.7 303,206.5C303,249.3 267.68,284 224.1,284H184.45M184.38,284H127.06M127.84,129.07C127.84,189.07 127,223.97 127,283.96M127.87,129.07L184.4,283.95";

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = 8f;
        lineRenderer.numCapVertices = 10;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;

        Reset();

        ParsePathData(pathData);

        for (int i = 0; i < m_Positions.Count; i++)
        {
            GameObject obj = new GameObject();
            LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            lineRenderer.widthMultiplier = 10f;
            lineRenderer.numCapVertices = 10;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;

            lineRenderer.positionCount = m_Positions[i].Count;
            lineRenderer.SetPositions(m_Positions[i].ToArray());
        }
    }

    private List<Vector3> movePos = new List<Vector3>();

    private float minXDif; //所有点最小x坐标差
    private float minYDif; //所有点最小y坐标差

    private List<NodeGroup> groups = new List<NodeGroup>();

    private void ParsePathData(string data)
    {
        m_Positions.Clear();
        movePos.Clear();
        groups.Clear();
        char currentCommand = ' ';
        string[] tokens = SplitPathData(data);
        List<Vector3> tempPos = new List<Vector3>();
        int groupId = 0;

        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i];
            if (token.Length == 0) continue;

            if (char.IsLetter(token[0]))
            {
                currentCommand = token[0];
                token = token.Substring(1).Trim();
            }

            string[] values = token.Split(new char[] { ' ', ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            switch (currentCommand)
            {
                case 'M':
                    tempPos = new List<Vector3>();
                    NodeGroup group = new NodeGroup();
                    group.id = groupId++;
                    group.nodes = tempPos;

                    groups.Add(group);
                    m_Positions.Add(tempPos);
                    tempPos.Add(new Vector3(float.Parse(values[0]), -float.Parse(values[1]), 0));
                    break;

                case 'L':
                    Vector3 startPos = tempPos.Last();
                    Vector3 endPos = new Vector3(float.Parse(values[0]), -float.Parse(values[1]), 0);
                    
                    List<Vector3> points = GetInterpolationPoints(startPos, endPos, 30);
                    tempPos.AddRange(points);
                    break;

                case 'C':
                {
                    Vector3 p0 = tempPos[tempPos.Count - 1];
                    Vector3 p1 = new Vector3(float.Parse(values[0]), -float.Parse(values[1]), 0);
                    Vector3 p2 = new Vector3(float.Parse(values[2]), -float.Parse(values[3]), 0);
                    Vector3 p3 = new Vector3(float.Parse(values[4]), -float.Parse(values[5]), 0);
                    List<Vector3> bezierPoints = CalculateBezierCurve(p0, p1, p2, p3);
                    // bezierPoints.RemoveAt(0); //去掉第一个重复了
                    tempPos.AddRange(bezierPoints);
                }
                    break;
                
                case 'H':
                    Vector3 last = tempPos.Last();
                    Vector3 target = new Vector3(float.Parse(values[0]), last.y, 0);

                    points = GetInterpolationPoints(last, target, 20);
                    tempPos.AddRange(points);
                    break;
                case 'V':
                    last = tempPos.Last();
                    target = new Vector3(last.x, -float.Parse(values[0]), 0);
              
                    points = GetInterpolationPoints(last, target, 20);
                    tempPos.AddRange(points);
                    break;
            }
        }

        //计算x,y最小差值
        minXDif = float.MaxValue;
        minYDif = float.MaxValue;
        for (int i = 0; i < m_Positions.Count; i++)
        {
            for (int j = 0; j < m_Positions[i].Count; j++)
            {
                if (j < m_Positions[i].Count - 1)
                {
                    float xDif = Mathf.Abs(m_Positions[i][j].x - m_Positions[i][j + 1].x);
                    float yDif = Mathf.Abs(m_Positions[i][j].y - m_Positions[i][j + 1].y);
                    if (xDif > 1 && xDif < minXDif)
                    {
                        minXDif = xDif;
                    }

                    if (yDif > 1 && yDif < minYDif)
                    {
                        minYDif = yDif;
                    }
                }
            }
        }

        LinkGroup();
    }
    
    public List<Vector3> GetInterpolationPoints(Vector3 start, Vector3 end, int count)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);
            points.Add(Lerp(start, end, t));
        }
        return points;
    }

    private Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(
            a.x + t * (b.x - a.x),
            a.y + t * (b.y - a.y)
        );
    }

    //尝试将各个组连接起来
    private void LinkGroup()
    {
        foreach (var group in groups)
        {
            foreach (var group1 in groups)
            {
                if (group == group1)
                    continue;

                if (Vector3.Distance(group1.nodes.First(), group.nodes.First()) <= 6
                    || Vector3.Distance(group1.nodes.First(), group.nodes.Last()) <= 6
                    || Vector3.Distance(group1.nodes.Last(), group.nodes.First()) <= 6
                    || Vector3.Distance(group1.nodes.Last(), group.nodes.Last()) <= 6)
                {
                    group.connectGroups.Add(group1);
                }
            }
        }
    }

    string[] SplitPathData(string data)
    {
        List<string> tokens = new List<string>();
        int start = 0;

        for (int i = 1; i < data.Length; i++)
        {
            if (char.IsLetter(data[i]))
            {
                tokens.Add(data.Substring(start, i - start).Trim());
                start = i;
            }
        }

        tokens.Add(data.Substring(start).Trim());

        return tokens.ToArray();
    }

    List<Vector3> CalculateBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segments = 20)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float u = 1 - t;
            Vector3 point = Mathf.Pow(u, 3) * p0 + 3 * Mathf.Pow(u, 2) * t * p1 + 3 * u * Mathf.Pow(t, 2) * p2 +
                            Mathf.Pow(t, 3) * p3;
            points.Add(point);
        }

        return points;
    }

    private List<GameObject> objs = new List<GameObject>();


    private float timer = 12;

    void OnDrawGizmos()
    {
        if (m_Positions.Count <= 0)
        {
            ParsePathData(pathData);
        }

        // timer -= Time.deltaTime;
        // if (timer <= 0)
        // {
        //     Reset();
        //     ParsePathData(pathData);
        //     timer = 12;
        //     Debug.Log("rebuilt path data!");
        // }

        Gizmos.color = Color.yellow;
        foreach (var pos in m_Positions)
        {
            if (pos != null && pos.Count >= 2)
            {
                Gizmos.DrawSphere(pos[0], 0.5f);
                for (int i = 1; i < pos.Count; i++)
                {
                    Gizmos.DrawLine(pos[i - 1], pos[i]);
                    Gizmos.DrawSphere(pos[i], 0.5f);
                }
            }
        }

        if (objs.Count == 0)
        {
            GameObject root = new GameObject("Objs");
            foreach (var points in m_Positions)
            {
                foreach (var p in points)
                {
                    GameObject obj = new GameObject(p.ToString());
                    obj.transform.parent = root.transform;
                    obj.transform.position = p;
                    objs.Add(obj);
                }
            }
        }

        Gizmos.DrawSphere(startPos, 10);
        Gizmos.DrawSphere(closestPoint, 10);
    }

    private void OnGUI()
    {
        // 设置按钮的位置和大小
        Rect buttonRect = new Rect(10, 10, 150, 50);

        // 绘制按钮
        if (GUI.Button(buttonRect, "Click Me"))
        {
            // 当按钮被点击时执行的代码
            Debug.Log("Button clicked!");
            Reset();
        }
    }

    private void Reset()
    {
        movePos.Clear();
        m_Positions.Clear();
        GameObject root = GameObject.Find("Objs");
        if (root)
        {
            DestroyImmediate(root);
        }

        objs.Clear();
    }

    //draw
    public LineRenderer lineRenderer; // 用于绘制线条

    private bool isDragging = false;
    private List<Vector3> pathPoints = new List<Vector3>();

    Vector3 FindClosestPoint(Vector3 position)
    {
        Vector3 minPoint = 10000 * Vector3.down;
        float minDis = 10000;
        foreach (var pos in m_Positions)
        {
            foreach (var point in pos)
            {
                float dis = Vector3.Distance(point, position);
                if (dis < minDis)
                {
                    minDis = dis;
                    minPoint = point;
                }
            }
        }

        Debug.Log("min point:" + minPoint);

        return minPoint;
    }

    Vector3 GetWorldPositionFromScreen(Vector3 screenPosition)
    {
        // 假设你的场景在z=0平面上
        screenPosition.z = 0;
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    //当前画到哪个group
    private NodeGroup currentGroup = null;
    private List<Vector3> tempNodes = new List<Vector3>();
    private List<NodeGroup> slideGroups = new List<NodeGroup>();

    private void FidNextPoint(Vector3 touchPos)
    {
        //加上纠错和重复点移除
        if (pathPoints.Remove(touchPos))
        {
            Debug.LogError("remove repeat point");
        }
        else
        {
            pathPoints.Add(touchPos);
            
            Vector3 lastPoint = pathPoints.Last();
            NodeGroup lastGroup = null;
            foreach (var group in groups)
            {
                if (group.nodes.Contains(lastPoint))
                {
                    lastGroup = group;
                    break;
                }
            }

            if (!currentGroup.IsLink(lastGroup))
            {
                Debug.LogError("不连续");
                return;
            }
            
            //处理连接处点点
            // int lastPointIndex = lastGroup.nodes.IndexOf(lastPoint);
            // if()
        }
        
        return;
        //todo 从当前点开始找下一个点
        // tempNodes.Clear();
        tempNodes.Add(startPos);
        if (currentGroup.nodes.Contains(touchPos))
        {
            bool front = false;

            foreach (var node in currentGroup.nodes)
            {
                if (node == startPos)
                {
                    front = true;
                    break;
                }
                else if (node == touchPos)
                {
                    front = false;
                    break;
                }
            }

            if (front)
            {
                bool add = false;
                foreach (var node in currentGroup.nodes)
                {
                    if (node == startPos)
                    {
                        add = true;
                    }

                    if (node == touchPos)
                    {
                        add = false;
                    }

                    if (add)
                    {
                        tempNodes.Add(node);
                    }
                }
            }
            else
            {
                bool add = false;
                for (int i = currentGroup.nodes.Count - 1; i >= 9; i--)
                {
                    if (currentGroup.nodes[i] == startPos)
                    {
                        add = true;
                    }

                    if (currentGroup.nodes[i] == touchPos)
                    {
                        add = false;
                    }

                    if (add)
                    {
                        tempNodes.Add(currentGroup.nodes[i]);
                    }
                }
            }
        }
        else
        {
            NodeGroup nextGroup = null;
            //找到当前点击位置落在哪个group
            foreach (var group in groups)
            {
                foreach (var node in group.nodes)
                {
                    //todo 可能相同点
                    if (node == touchPos)
                    {
                        nextGroup = group;
                        break;
                    }
                }
            }

            if (nextGroup == null)
                return;
            
            if(!slideGroups.Contains(nextGroup))
                slideGroups.Add(nextGroup);

            if (!currentGroup.IsLink(nextGroup))
            {
                Debug.LogError("不连续");
            }
            else
            {
                //找到nextgroup 离当前点击最近的点
                Vector3 nextLinkPos = GetGroupCloserHeadPoint(nextGroup, touchPos);
                Vector3 currentLinkPos = GetGroupCloserHeadPoint(currentGroup, startPos);
                int index = currentGroup.nodes.IndexOf(startPos);
                int curLinkPointIndex = currentGroup.nodes.IndexOf(currentLinkPos);
                if (curLinkPointIndex > index)
                {
                    for (int i = index; i <= curLinkPointIndex; i++)
                    {
                        tempNodes.Add(currentGroup.nodes[i]);
                    }
                }
                else
                {
                    for (int i = index; i >= curLinkPointIndex; i--)
                    {
                        tempNodes.Add(currentGroup.nodes[i]);
                    }
                }

                //add next group node
                int nextLinkPointIndex = nextGroup.nodes.IndexOf(nextLinkPos);
                int touchPointIndex = nextGroup.nodes.IndexOf(touchPos);
                if (nextLinkPointIndex < touchPointIndex)
                {
                    for (int i = nextLinkPointIndex; i <= touchPointIndex; i++)
                    {
                        tempNodes.Add(nextGroup.nodes[i]);
                    }
                }
                else
                {
                    for (int i = nextLinkPointIndex; i >= touchPointIndex; i--)
                    {
                        tempNodes.Add(nextGroup.nodes[i]);
                    }
                }
            }
        }

        pathPoints = tempNodes;
    }

    private Vector3 GetGroupCloserHeadPoint(NodeGroup group, Vector3 pos)
    {
        var disHead = Vector3.Distance(group.nodes.First(), pos);
        var disEnd = Vector3.Distance(group.nodes.Last(), pos);
        if (disHead < disEnd)
            return group.nodes.First();
        else
            return group.nodes.Last();
    }

    private Vector3 startPos;
    private Vector3 closestPoint;

    //todo 手指点位置直线时没法提供中间点
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition = GetWorldPositionFromScreen(Input.mousePosition);
            touchPosition.z = 0;
            startPos = FindClosestPoint(touchPosition);

            foreach (var group in groups)
            {
                //todo 可能存在重复的点
                if (group.nodes.Contains(startPos))
                    currentGroup = group;
            }

            Debug.Log("start pos:" + closestPoint + " group:" + currentGroup.id);
            isDragging = true;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 touchPosition = GetWorldPositionFromScreen(Input.mousePosition);
            touchPosition.z = 0;

            Debug.Log("mouse pos:" + touchPosition);

            closestPoint = FindClosestPoint(touchPosition);
            FidNextPoint(closestPoint);

            UpdateLineRenderer();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            pathPoints.Clear();
            UpdateLineRenderer();
        }
    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());
    }

    //点组
    private class NodeGroup
    {
        public int id;
        public List<Vector3> nodes = new List<Vector3>(); //组内所有点
        public List<NodeGroup> connectGroups = new List<NodeGroup>(); //连接的组

        public bool IsLink(NodeGroup group)
        {
            foreach (var connectGroup in connectGroups)
            {
                if (connectGroup == group)
                    return true;
            }

            return false;
        }
    }

    //关键点
    private class KeyPoint
    {
        public Vector3 pos;
        public List<NodeGroup> linkGroups = new List<NodeGroup>(); //连接点组
    }
}