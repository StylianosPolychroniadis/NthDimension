using NthDimension.Algebra;
using System.Collections.Generic;
using RoadGen;
using NthDimension.Rendering;
using NthDimension.Rendering.Geometry;
using System.Drawing;

public class InteractiveCityRenderer2D //: /*MonoBehaviour*/
{
    //public Color highwayStartColor = Color.red;
    //public Color highwayEndColor = Color.red;
    //public Color streetStartColor = Color.gray;
    //public Color streetEndColor = Color.gray;
    //public Texture2D intersectionIcon;
    //public Texture2D snapIcon;
    //public Texture2D intersectionRadiusIcon;
    public float iconSize = 0.333f;
    public float highwayWidth = 0.05f;
    public float streetWidth = 0.025f;
    public float downscaleFactor = 1000.0f;
    public float z = 1;
    public bool displayBuildings = true;
    public bool displayHighways = true;
    public bool displayStreets = true;
    RoadNetworkGenerator.InteractiveGenerationContext context;
    HashSet<Segment> visited;
    List<ApplicationObject> segmentsGOs;
    List<ApplicationObject> iconGOs;
    int mask;
    int action;
    bool step;
    bool end;
    int speed;

    bool Visitor(Segment segment)
    {
        throw new System.NotImplementedException();
        {
            //

            //GameObject segmentGO = new GameObject("Segment " + segment.Index);
            //segmentGO.AddComponent<MeshFilter>().mesh = CreateLineMesh(
            //    segment.Start / downscaleFactor,
            //    segment.End / downscaleFactor,
            //    z,
            //    segment.Highway ? highwayStartColor : streetStartColor,
            //    segment.Highway ? highwayEndColor : streetEndColor,
            //    segment.Highway ? highwayWidth : streetWidth);
            //segmentGO.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Custom/VertexColor"));
            //segmentsGOs.Add(segmentGO);
            //return true;
        }
    }

    void Start()
    {
        context = RoadNetworkGenerator.BeginInteractiveGeneration();
        visited = new HashSet<Segment>();
        mask = ((displayHighways) ? RoadNetworkTraversal.HIGHWAYS_MASK : 0) | ((displayStreets) ? RoadNetworkTraversal.STREETS_MASK : 0);
        segmentsGOs = new List<ApplicationObject>();
        iconGOs = new List<ApplicationObject>();
        action = 1;
    }

    void Update()
    {
        if (action == 0)
            return;

        if (action == 1)
        {
            if (!end)
                throw new System.NotImplementedException();
            {
                //
                //end = Input.GetKeyDown(KeyCode.F5);
            }

            if (!step)
                throw new System.NotImplementedException();
            {
                //
                //step = Input.GetKeyDown(KeyCode.F10);
            }

            if (!step && !end)
                return;

            if (end)
            {
                RoadNetworkGenerator.EndInteractiveGeneration(ref context);
                action = 0;
            }
            else
            {
                if (!RoadNetworkGenerator.InteractiveGenerationStep(speed, ref context))
                    action = 0;
            }

            foreach (Segment segment in context.segments)
                RoadNetworkTraversal.PreOrder(segment, Visitor, mask, ref visited);

            RemoveIcons();
            if (context.debugData.intersections.Count > 0 ||
                context.debugData.snaps.Count > 0 ||
                context.debugData.intersectionsRadius.Count > 0)
            {
                throw new System.NotImplementedException();
                {
                    //

                    //foreach (Vector2 intersection in context.debugData.intersections)
                    //    iconGOs.Add(CreateIcon(intersection, iconSize, downscaleFactor, z, intersectionIcon));
                    //foreach (Vector2 snap in context.debugData.snaps)
                    //    iconGOs.Add(CreateIcon(snap, iconSize, downscaleFactor, z, snapIcon));
                    //foreach (Vector2 intersectionRadius in context.debugData.intersectionsRadius)
                    //    iconGOs.Add(CreateIcon(intersectionRadius, iconSize, downscaleFactor, z, intersectionRadiusIcon));
                }
                context.debugData.intersections.Clear();
                context.debugData.snaps.Clear();
                context.debugData.intersectionsRadius.Clear();
            }

            step = false;
        }
    }

    void OnGUI()
    {
        throw new System.NotImplementedException();
        {
            ////

            //GUI.Label(new Rect(10, 10, 40, 20), "Speed");
            //if (!int.TryParse(GUI.TextField(new Rect(50, 10, 40, 20), speed + ""), out speed))
            //    speed = 1;
            //else
            //    speed = (int)MathHelper.Max(1, speed);
            //step = GUI.Button(new Rect(10, 40, 100, 20), "Step");
            //end = GUI.Button(new Rect(10, 70, 100, 20), "End");
            //GUI.Label(new Rect(10, 100, 200, 20), "Global Derivation Step: " + context.globalDerivationStep);
            //if (GUI.Button(new Rect(10, 130, 140, 20), "Remove Icons"))
            //    RemoveIcons();
        }
    }

    static MeshVbo CreateLineMesh(Vector2 start, Vector2 end, float z, Color startColor, Color endColor, float width)
    {
        float halfWidth = width * 0.5f;
        Vector2 direction = end - start;
        Vector2 side = new Vector2(direction.Y, -direction.X);
        side.Normalize();
        side *= halfWidth;
        Vector2[] positions = new Vector2[4];
        positions[0] = start - side;
        positions[1] = end - side;
        positions[2] = end + side;
        positions[3] = start + side;
        MeshVbo mesh = new MeshVbo();
        Vector3[] vertices = new Vector3[4];

        throw new System.NotImplementedException();
        {
            ////

            //for (int i = 0; i < 4; i++)
            //    vertices[i] = new Vector3(positions[i].X, positions[i].Y, z);
            //mesh.vertices = vertices;
            //mesh.normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
            //mesh.colors = new Color[] { startColor, endColor, endColor, startColor };
            //mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        }
        return mesh;
    }

    static MeshVbo CreateQuadMesh(Vector2 center, float width, float height, float z)
    {

        MeshVbo mesh = new MeshVbo();
        float halfWidth = width * 0.5f, halfHeight = height * 0.5f;

        throw new System.NotImplementedException();
        {
        //    //

        //    mesh.vertices = new Vector3[]
        //{
        //    new Vector3(center.X - halfWidth, center.Y - halfHeight, z),
        //    new Vector3(center.X - halfWidth, center.Y + halfHeight, z),
        //    new Vector3(center.X + halfWidth, center.Y + halfHeight, z),
        //    new Vector3(center.X + halfWidth, center.Y - halfHeight, z)
        //};
        //    mesh.normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
        //    mesh.uv = new Vector2[]
        //    {
        //    new Vector2(0,0),
        //    new Vector2(0,1),
        //    new Vector2(1,1),
        //    new Vector2(1,0)
        //    };
        //    mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        }
        return mesh;
    }

    void RemoveIcons()
    {
        foreach (var gameObject in iconGOs)
            throw new System.NotImplementedException();
        {
            //
            //Destroy(gameObject);
        }
        iconGOs.Clear();
    }

    //static GameObject CreateIcon(Vector2 intersection, float iconSize, float downscaleFactor, float z, Texture2D icon)
    //{
    //    throw new System.NotImplementedException();
    //    {
    //        //

    //        //GameObject gameObject = new GameObject("Icon");
    //        //gameObject.AddComponent<MeshFilter>().mesh = CreateQuadMesh(intersection / downscaleFactor, iconSize, iconSize, z - 1);
    //        //Material material = new Material(Shader.Find("Unlit/Transparent"));
    //        //material.SetTexture("_MainTex", icon);
    //        //gameObject.AddComponent<MeshRenderer>().material = material;
    //        //return gameObject;
    //    }
    //}

}
