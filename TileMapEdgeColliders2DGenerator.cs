using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CompositeCollider2D)), RequireComponent(typeof(TilemapCollider2D))]
public class TileMapEdgeColliders2DGenerator : MonoBehaviour
{
    // Inspector
    [SerializeField] private float verticalTolerance = 0.1f;
    [SerializeField] private bool logPaths = false;
    
    // Fields
    private CompositeCollider2D compositeCollider2D;
    private TilemapCollider2D tilemapCollider2D;
    
    // Methods
    
    public void DetectEdges()
    {
        RemoveAllEdges();
        
        compositeCollider2D =  GetComponent<CompositeCollider2D>();
        tilemapCollider2D = GetComponent<TilemapCollider2D>();
        tilemapCollider2D.enabled = true;
        
        List<Vector2> path = new List<Vector2>();

        int edgesCnt = 0;
        
        for (int i = 0; i < compositeCollider2D.pathCount; i++)
        {
            compositeCollider2D.GetPath(i, path);
            edgesCnt+=DetectEdgesFromPath(path);
            
            if (logPaths) LogPath(path);
        }
        
        Debug.Log($"{edgesCnt} edge(s) have been detected.");
        tilemapCollider2D.enabled = false;
    }

    private int DetectEdgesFromPath(List<Vector2> path)
    {
        int cnt = 0;
        Vector2 p1 = path[0];
        for (int i = 1; i <= path.Count; i++)
        {
            Vector2 p2 = path[i % path.Count];

            if (Mathf.Abs(p1.x-p2.x) > verticalTolerance && p2.x < p1.x)
            {
                gameObject.AddComponent<EdgeCollider2D>().SetPoints(new List<Vector2>() {p2, p1});
                cnt++;
            }

            p1 = p2;
        }

        return cnt;
    }

    public void RemoveAllEdges()
    {
        EdgeCollider2D[] edgeColliders = GetComponents<EdgeCollider2D>();
        foreach (var edge  in edgeColliders)
        {
            DestroyImmediate(edge);
        }
    }

    private void LogPath(List<Vector2> path)
    {
        string res = "";
        foreach (var p in path)
        {
            res += $"X:{Mathf.Round(p.x)} Y:{Mathf.Round(p.y)}\n";
        }
            
        Debug.Log(res);
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(TileMapEdgeColliders2DGenerator))]
public class CustomInspectorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileMapEdgeColliders2DGenerator edgeColliders2DGenerator = (TileMapEdgeColliders2DGenerator)target;
        if (GUILayout.Button ("Detect Edges"))
        {
            edgeColliders2DGenerator.DetectEdges();
        }
        if (GUILayout.Button ("Remove All Edges"))
        {
            edgeColliders2DGenerator.RemoveAllEdges();
        }
    }
}
#endif