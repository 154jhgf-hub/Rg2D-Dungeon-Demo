using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AbstractDungeonGenerator),true)]
public class RandomDungeonGeneratorEditor : Editor
{
    private AbstractDungeonGenerator generator;
    private TileMapVisualizer visualizer;
   void Awake()
   {
        generator = (AbstractDungeonGenerator)target;
   }
   
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Craet Dungeon"))
        {
            generator.DungeonGenerator();
        }
    }

}
