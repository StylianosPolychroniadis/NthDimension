﻿using NthDimension.Algebra;
using System.Collections.Generic;
using RoadGen;
using NthDimension.Rendering;

public class MockAllotment //: /*MonoBehaviour*/
{
    public ApplicationObject allotmentBuilderGameObject;
    public ApplicationObject heightmapGameObject;
    private IHeightmap heightmap;
    private List<IAllotmentBuilder> allotmentBuilders = null;
    private List<ApplicationObject> allotments = new List<ApplicationObject>();

    void Generate()
    {
        throw new System.NotImplementedException();
        {

            //foreach (GameObject allotmentGO in allotments)
            //    Destroy(allotmentGO);
            //allotments.Clear();
            //var allotment = new Allotment(default(Vector2),
            //        0,
            //        UnityEngine.Random.Range(Config.allotmentMinHalfDiagonal, Config.allotmentMaxHalfDiagonal),
            //        UnityEngine.Random.Range(Config.allotmentMinAspect, Config.allotmentMaxAspect));
            //foreach (var allotmentBuilder in allotmentBuilders)
            //{
            //    GameObject allotmentGO = allotmentBuilder.Build(allotment, heightmap);
            //    Vector3 position = allotmentGO.transform.position;
            //    allotmentGO.transform.position = position;
            //    allotments.Add(allotmentGO);
            //}
        }
    }

    void Start()
    {
        if (heightmapGameObject != null)
            heightmap = UnityEngineHelper.GetInterface<IHeightmap>(heightmapGameObject);

        if (heightmap == null)
        {
            //////Debug.LogError("MockAllotment needs a reference to a game object containing at least one component that implements IHeightmap");
            return;
        }

        if (!heightmap.Finished())
        {
            //////Debug.LogError("MockAllotment script can only execute after the components that implements IHeightmap (Configure execution order that in Edit > Project Settings > Script Execution Order)");
            return;
        }

        if (allotmentBuilderGameObject != null)
            allotmentBuilders = UnityEngineHelper.GetInterfaces<IAllotmentBuilder>(allotmentBuilderGameObject);
        if (allotmentBuilders == null)
        {
            //////Debug.LogError("MockAllotment needs a reference to a game object containing at least one component that implements IAllotmentBuilder");
            return;
        }

        Generate();
    }

    void OnGUI()
    {
        throw new System.NotImplementedException();
        {

            //if (GUI.Button(new Rect(10, 10, 140, 40), "Generate"))
            //    Generate();
        }
    }

}
