using NthDimension.Algebra;
using NthDimension.Rendering;
using NthDimension.Rendering.Culling;
using NthDimension.Rendering.Drawables;
using NthDimension.Rendering.Drawables.Models;
using NthDimension.Rendering.Geometry;
using NthDimension.Rasterizer.NanoVG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NthStudio.Gui.Displays.NanoGContext;
using System.Drawing;
using NthDimension.Rendering.Utilities;
using NthDimension.Physics.Dynamics;
using NthDimension.Physics.LinearMath;

namespace NthStudio
{
    public partial class StudioWindow
    {
        #region Perf Graphs
        int totalVertices = 0;
        long textureSize = 0L;
        long texturesActive = 0L;
        int drawableMeshes = 0;
        int totalVisibleMeshes = 0;
        int meshesSize = 0;
        DateTime diagUpd = DateTime.Now;
        #endregion Perf Graphs

        // Perf Graphs
        private PerfGraph graphFPS;
        private PerfGraph graphCPU;
        private PerfGraph graphVRAM;
        private PerfGraph graphRAM;
        private PerfGraph graphLatency;
        private PerfGraph graphDrawCalls;
        private PerfGraph graphVertices;
        private PerfGraph graphMaterials;
        private PerfGraph graphTextures;
        private PerfGraph graphTexturesMB;
        private PerfGraph graphGeometries;
        private PerfGraph graphGeometriesMB;

        // Keys
        private bool prevESC = false;
        private bool prevF6;
        private bool prevF12;
        private bool prevCtrlF1;
        private bool prevTilde;
        private bool prevPGUP;
        private bool prevPGDN;
        private bool prevHome = false;
        private bool prevEnd = false;
        // Frame Time
        private double perf_tprev = 0d;
        private float perf_tnow = 0f;
        private float perf_t = 0f;
        private float perf_Δt = 0f;


        private bool userSelectionMeshTimes = false;
        private bool userSelectionDrawableTimes = false;

        

        #region Perf Graphs
        private void initializePerformanceTrends()
        {
            graphFPS = new PerfGraph();
            graphFPS.InitGraph(GraphrenderStyle.GRAPH_RENDER_FPS, "Frame Time");

            graphCPU = new PerfGraph();
            graphCPU.FillColor = NanoVG.nvgRGBA(0, 255, 255, 128);
            graphCPU.InitGraph(GraphrenderStyle.GRAPH_RENDER_PERCENT, "CPU Time");

            graphVRAM = new PerfGraph();
            graphVRAM.FillColor = NanoVG.nvgRGBA(255, 255, 0, 128);
            graphVRAM.InitGraph(GraphrenderStyle.GRAPH_MEGABYTES, "VRAM Used");

            graphRAM = new PerfGraph();
            graphRAM.FillColor = NanoVG.nvgRGBA(0, 255, 0, 128);
            graphRAM.InitGraph(GraphrenderStyle.GRAPH_MEGABYTES, "RAM Used");

            graphLatency = new PerfGraph();
            graphLatency.FillColor = NanoVG.nvgRGBA(255, 0, 0, 128);
            graphLatency.InitGraph(GraphrenderStyle.GRAPH_RENDER_MS, "Network Latency");

            graphDrawCalls = new PerfGraph();
            graphDrawCalls.FillColor = NanoVG.nvgRGBA(128, 255, 255, 128);
            graphDrawCalls.InitGraph(GraphrenderStyle.GRAPH_DRAWCALLS, "Draw Calls");

            graphVertices = new PerfGraph();
            graphVertices.FillColor = NanoVG.nvgRGBA(0, 255, 128, 128);
            graphVertices.InitGraph(GraphrenderStyle.GRAPH_VERTICES, "Vertices");

            graphMaterials = new PerfGraph();
            graphMaterials.FillColor = NanoVG.nvgRGBA(255, 255, 128, 128);
            graphMaterials.InitGraph(GraphrenderStyle.GRAPH_MATERIALS, "Materials");

            graphTextures = new PerfGraph();
            graphTextures.FillColor = NanoVG.nvgRGBA(128, 255, 128, 128);
            graphTextures.InitGraph(GraphrenderStyle.GRAPH_TEXTURES, "Textures Active");

            graphTexturesMB = new PerfGraph();
            graphTexturesMB.FillColor = NanoVG.nvgRGBA(128, 255, 128, 128);
            graphTexturesMB.InitGraph(GraphrenderStyle.GRAPH_RATE_TEXTURES, "Texture rate");


            graphGeometries = new PerfGraph();
            graphGeometries.FillColor = NanoVG.nvgRGBA(255, 128, 128, 128);
            graphGeometries.InitGraph(GraphrenderStyle.GRAPH_GEOMETRIES, "Geometries Active");

            graphGeometriesMB = new PerfGraph();
            graphGeometriesMB.FillColor = NanoVG.nvgRGBA(255, 128, 128, 128);
            graphGeometriesMB.InitGraph(GraphrenderStyle.GRAPH_RATE_GEOMETRIES, "Geometry rate");


        }
        int maxVertices = 0;
        private void updatePerformanceTrends(double time)
        {
            #region Frame Time & Frame Time Delta
            perf_t = perf_Δt = 0;
            perf_t = (float)time;
            perf_tnow += perf_t;
            perf_Δt = (float)(perf_tnow - perf_tprev);
            perf_tprev = perf_tnow;
            #endregion Frame Time

            if (NthDimension.Settings.Instance.game.diagnostics)
            {
                this.updateDiagnostics();

                graphFPS.UpdateGraph(perf_Δt);

                if (NthDimension.Settings.Instance.game.diagnosticsFrame)
                {
                    graphCPU.UpdateGraph(Utilities.HardwareMonitor.CurrentCPUusage);
                    float usedVram = (float)StudioWindow.Instance.GetUsedDeviceMemoryBytes();
                    //graphVRAM.UpdateGraph(usedVram > 0f ? usedVram : 0f, TotalVideoRam);
                    graphVRAM.UpdateGraph(usedVram > 0f ? usedVram : 0f, 0f);
                    graphRAM.UpdateGraph(Utilities.HardwareMonitor.CurrentMemoryUsage, Utilities.HardwareMonitor.GetSystemRAM / 1024);

#if NETWORK
            if (null != Program.gameClient)
                graphLatency.UpdateGraph(Program.gameClient.Latency);
#endif


                    //if (VAR_AppState == ApplicationState.Playing)
                    {
                        if (maxVertices < totalVertices)
                            maxVertices = totalVertices;

                        graphDrawCalls.UpdateGraph(Scene.DrawCallTotal);
                        graphVertices.UpdateGraph(totalVertices, maxVertices);
                        graphMaterials.UpdateGraph(Scene.VisibleDrawables.Count, Scene.Drawables.Count);
                        graphTextures.UpdateGraph(texturesActive, TextureLoader.textures.Count);
                        graphTexturesMB.UpdateGraph(textureSize, 0);
                        graphGeometries.UpdateGraph(totalVisibleMeshes, drawableMeshes);
                        graphGeometriesMB.UpdateGraph(meshesSize);
                    }
                }

            }


        }
        private void drawPerformanceTrends()
        {
            int ypos = 70;
            int rightMargin = 5;
            //if (debug_ShowGraphs)
            //{

            if (NthDimension.Settings.Instance.game.diagnostics)
            {
                NanoVG.nvgBeginFrame(vg, Width, Height, 1f);
                
                graphFPS.RenderGraph(vg, this.Width - graphFPS.Width - rightMargin, ypos);

                if (NthDimension.Settings.Instance.game.diagnosticsFrame)
                {
                    graphCPU.RenderGraph(vg, this.Width - graphCPU.Width - rightMargin, ypos + this.graphFPS.Height + 1f);
                    graphVRAM.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f);
                    graphRAM.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f + this.graphVRAM.Height + 1f);

#if NETWORK
            if (null != Program.gameClient)
                graphLatency.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f + this.graphVRAM.Height + 1f + this.graphRAM.Height + 1f);
#endif


                    graphDrawCalls.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f + this.graphVRAM.Height + 1f + this.graphRAM.Height + 1f + graphLatency.Height);
                    graphVertices.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f + this.graphVRAM.Height + 1f + this.graphRAM.Height + 1f + graphLatency.Height + 1f + graphDrawCalls.Height);
                    graphMaterials.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f + this.graphVRAM.Height + 1f + this.graphRAM.Height + 1f + graphLatency.Height + 1f + graphDrawCalls.Height + 1f + graphVertices.Height);
                    graphTextures.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f + this.graphVRAM.Height + 1f + this.graphRAM.Height + 1f + graphLatency.Height + 1f + graphDrawCalls.Height + 1f + graphVertices.Height + graphMaterials.Height);
                    graphTexturesMB.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f + this.graphVRAM.Height + 1f + this.graphRAM.Height + 1f + graphLatency.Height + 1f + graphDrawCalls.Height + 1f + graphVertices.Height + 1f + graphMaterials.Height + 1f + graphTextures.Height);
                    graphGeometries.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f + this.graphVRAM.Height + 1f + this.graphRAM.Height + 1f + graphLatency.Height + 1f + graphDrawCalls.Height + 1f + graphVertices.Height + 1f + graphMaterials.Height + 1f + graphTextures.Height + 1f + graphTexturesMB.Height);
                    graphGeometriesMB.RenderGraph(vg, this.Width - graphLatency.Width - rightMargin, ypos + this.graphFPS.Height + 1f + this.graphCPU.Height + 1f + this.graphVRAM.Height + 1f + this.graphRAM.Height + 1f + graphLatency.Height + 1f + graphDrawCalls.Height + 1f + graphVertices.Height + 1f + graphMaterials.Height + 1f + graphTextures.Height + 1f + graphTexturesMB.Height + 1f + graphGeometries.Height);
                }
                
                NanoVG.nvgEndFrame(vg);
            }
            
            ////}
        }

        private void updateDiagnostics()
        {
            if (DateTime.Now > diagUpd)
            {
                //this.Title = string.Format("Drawables: {0} Frame Time: {1}", Scene.VisibleCount, FrameTime.ToString("###.###"));      // TODO:: Transmit to server????
                drawableMeshes = Scene.Drawables.SelectMany(x => x.meshes).Count();
                totalVisibleMeshes = Scene.VisibleDrawables.SelectMany(x => x.Value).Count();
                totalVertices = Scene.VisibleDrawables.SelectMany(meshes => meshes.Value)
                    .Sum(verts => verts.MeshData.Positions.Length);
                meshesSize = totalVertices * 3 * sizeof(float) / 1024 / 1024;
                int totalPolygons = Scene.VisibleDrawables.SelectMany(meshes => meshes.Value).Sum(verts =>
                    verts.MeshData.Faces != null ? verts.MeshData.Faces.Count : 0);

                //long textureSize = 0L;
                //long texturesActive = 0L;

                try
                {
                    textureSize = TextureLoader.textures.Sum(i => i.Size) / 1024 / 1024;
                }
                catch { }

                try
                {
                    texturesActive = TextureLoader.textures.Sum(i => (i.loaded ? 1L : 0L));
                }
                catch { }

                //this.Title = string.Format(
                //    @"{13} -  {4} DCalls | {0}/{1} Materials | {11} Textures @ {12} MB/Frame| {5} Vertices @ {10} MB/Frame | {6} Polys | {2}/{3} Meshes |  Frame {7} msec | Mouse X {8} Y {9}",
                //    Scene.VisibleDrawables.Count,                           // Visible Drawables                        // 0
                //    Scene.Drawables.Count,                                  // Drawables                                // 1
                //    totalVisibleMeshes,                                     // Total Visible Meshes                     // 2
                //    drawableMeshes,                                         // Total Meshes                             // 3
                //    GameBase.Instance.Scene.totalDrawCalls,                 // Draw Calls                               // 4
                //    totalVertices,                                          // Total Vertices                           // 5
                //    totalPolygons,                                          // Total Polygons                           // 6
                //    FrameTime.ToString("###.###"),                          // Frame Time                               // 7
                //    PlayerInput.MouseX,                                     // Mouse X Input                            // 8
                //    PlayerInput.MouseY,                                     // Mouse Y Input                            // 9
                //    meshesSize,                                             // Total Vertex Size (MB)                   // 10
                //    texturesActive,                                         // Active Texture Count                     // 11
                //    textureSize,                                            // Active Textures Size (MB)                // 12
                //    deviceOpenGL);                                          // Device                                   // 13

                //OpenTK.Input.MouseState mouse = OpenTK.Input.Mouse.GetCursorState();

                //Point p = PointToClient(new Point(mouse.X, mouse.Y));

                //int lx, ly = 0;
                //Rafa.Gui.Widgets.Widget w = ((ScreenUI)Screen2D).GetControlAt(p.X, p.Y, out lx, out ly);

                //if(null == w || w == ((ScreenUI)Screen2D) && ((ScreenUI)Screen2D).OverlayCurrent != null)
                //{
                //    w = ((ScreenUI)Screen2D).GetWidgetAtOverlay(((ScreenUI)Screen2D).OverlayCurrent, p.X, p.Y, out lx, out ly);
                //}

                var w = this.Screen2D.OverlayCurrent;


                //this.Title = string.Format(_titleFormatState, vendorOpenGL,
                //                                              deviceOpenGL,
                //                                              string.Format("{0}.{1}", majorGlsl, minorGlsl),
                //                                              null == w ? string.Empty : w.Name);
                //                                              //this.GameState.ToString());

                diagUpd = DateTime.Now.AddMilliseconds(500);

            }
        }
        #endregion Perf Graphs

#region Diagnostics Pages
//#if DEBUG
        public override void DrawMeshDrawTimes()
        {

            bool home = OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Home);
            if (home && !prevHome && this.Focused)
                userSelectionMeshTimes = !userSelectionMeshTimes;

            prevHome = home;

            if (!userSelectionMeshTimes) return;

            userSelectionDrawableTimes = false;
            x_meshes_time = 0;
            deducti_meshes_time = 0;
            column1_meshes_time = false;
            column2_meshes_time = false;


            int i = 1;

            NanoVG.nvgBeginFrame(vg, Width, Height, 1);
            foreach (MeshVbo m in DrawnMeshes.OrderByDescending(t => t.AverageDrawTime))
            {


                if (m.DrawTimeAllPassesPrevious < m.AverageDrawTime)
                    NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 0, 0, 255));
                if (m.DrawTimeAllPassesPrevious > m.AverageDrawTime)
                    NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(0, 255, 0, 255));
                if (m.DrawTimeAllPassesPrevious == m.AverageDrawTime)
                    NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 255, 255, 255));
                NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));


                m.DrawTimeAllPassesAccumulator += m.DrawTimeAllPasses;
                m.DrawTimeAllPassesPrevious = m.DrawTimeAllPasses;
                m.DrawTimeAllPasses = 0l;

                if (m.DrawCalls > 0) // avoids Divide by zero
                    m.AverageDrawTime = m.DrawTimeAllPassesAccumulator / m.DrawCalls;
                else
                    m.AverageDrawTime = 0;

                i++;

                if (10 + i * 15 > Height)
                {
                    //NanoVG.nvgEndFrame(vg);
                    //continue;

                    if (!column1_meshes_time)
                    {
                        deducti_meshes_time = i;
                        column1_meshes_time = true;
                        x_meshes_time = 610;
                    }

                    ii_meshes_time = i - deducti_meshes_time;
                }
                else
                {
                    ii_meshes_time = i;
                }

                MeshVbo.MeshLod mLod = m.CurrentLod;

                NanoVG.nvgText(vg, x_meshes_time + 10, y_meshes_time + 40 + ii_meshes_time * 15, string.Format("{0} {1} ft {2}", mLod, m.DistanceToCamera.ToString("F1"), m.Name.PadRight(220)));
                NanoVG.nvgText(vg, x_meshes_time + 10 + 530, y_meshes_time + 40 + ii_meshes_time * 15, (m.AverageDrawTime / 10).ToString().PadLeft(8) + " μSec");

            }
            NanoVG.nvgEndFrame(vg);
            DrawnMeshes.Clear();

        }
        public override void DrawDrawablesDrawTimes()
        {
            bool end = OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.End);
            if (end && !prevEnd && this.Focused)
                userSelectionDrawableTimes = !userSelectionDrawableTimes;
            prevEnd = end;

            if (!userSelectionDrawableTimes) return;

            userSelectionMeshTimes = false;
            x_drawables_time = 0;
            deducti_drawables_time = 0;
            column1_drawables_time = false;
            column2_drawables_time = false;

            int i = 1;

            NanoVG.nvgBeginFrame(vg, Width, Height, 1);
            foreach (Drawable d in Scene.Drawables.OrderByDescending(t => t.Performance.DrawTimeAllPasses))
            {
                if (!d.IsVisible) continue;

                if (d.Performance.DrawTimeAllPassesPrevious < d.Performance.DrawTimeAllPasses)
                    NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 0, 0, 255));
                if (d.Performance.DrawTimeAllPassesPrevious > d.Performance.DrawTimeAllPasses)
                    NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(0, 255, 0, 255));
                if (d.Performance.DrawTimeAllPassesPrevious == d.Performance.DrawTimeAllPasses)
                    NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 255, 255, 255));
                NanoVG.nvgTextAlign(vg, (int)(EAlign.ALIGN_LEFT | EAlign.ALIGN_MIDDLE));

                d.Performance.DrawTimeAllPassesPrevious = d.Performance.DrawTimeAllPasses;

                i++;

                if (10 + i * 15 > Height)
                {
                    //if(column1_drawables_time && !column2_drawables_time)
                    //{
                    //    column2_drawables_time = true;
                    //    x_drawables_time = 1060;
                    //}

                    if (!column1_drawables_time)
                    {
                        deducti_drawables_time = i;
                        column1_drawables_time = true;
                        x_drawables_time = 540;
                    }

                    ii_drawables_time = i - deducti_drawables_time;
                }
                else
                {
                    ii_drawables_time = i;
                }


                if (d.Materials.Count > 0)
                {
                    string scr = string.Format("[{0}]:{1}", d.Name, d.Materials[0]).PadRight(200);
                    //NanoVG.nvgText(vg, 180, 40 + i * 15, d.Name.PadRight(200));
                    NanoVG.nvgText(vg, x_drawables_time + 10, y_drawables_time + 40 + ii_drawables_time * 15, scr);
                    NanoVG.nvgText(vg, x_drawables_time + 10 + 450, y_drawables_time + 40 + ii_drawables_time * 15, (d.Performance.DrawTimeAllPasses / 10).ToString().PadLeft(8) + " μSec");
                }
            }
            NanoVG.nvgEndFrame(vg);

        }

        float x_meshes_time = 0;
        float y_meshes_time = 0;
        int ii_meshes_time = 0;
        int deducti_meshes_time = 0;
        bool column1_meshes_time = false;
        bool column2_meshes_time = false;

        float x_drawables_time = 0;
        float y_drawables_time = 0;
        int ii_drawables_time = 0;
        int deducti_drawables_time = 0;
        bool column1_drawables_time = false;
        bool column2_drawables_time = false;
//#endif
#endregion Diagnostics Pages

#region Game Debug Console
        // Game Console
        //protected DebuggingConsole debugConsole;
        //protected CommandManager cmdManager;
        //protected List<CommandDescriptor> cmds = new List<CommandDescriptor>();
        //protected GUI.TextBox						commandBox;

        private float startX = 150;
        private float startY = 50;

        private void initializeDebugConsole()
        {
#region Debug Console
            //this.cmdManager = new CommandManager();
            //List<CommandDescriptor> cmds = new List<CommandDescriptor>()
            //{
            //    new CommandDescriptor("help", "lists all available commands", false, CommandHandler_Help),
            //    new CommandDescriptor("clear", "clears console", false, CommandHandler_Clear),
            //};
            //this.debugConsole = new DebuggingConsole(cmds.ToArray(), new Debug.Console.Color(255, 255, 255));
#endregion


            ////         this.commandBox = new Widget();
            ////this.commandBox.WithSize(new Vector2(100, 20))
            ////							.WithLocalPosition(new Vector2(this.Width/2 - 100/2, this.Height - 30 - 20));
        }

#region Command Console Handlers
        //private void CommandHandler_Help(object sender, ExecuteCommandArgs args)
        //{

        //    int lines = 0;

        //    float asc = 0f;
        //    float desc = 0f;
        //    float lh = 0f;
        //    NanoVG.nvgTextMetrics(vg, ref asc, ref desc, ref lh);

        //    foreach (KeyValuePair<string, CommandDescriptor> command in debugConsole.Commands)
        //    {
        //        NanoVG.nvgText(StudioWindow.vg, startX, Height - (startY + (lines * lh) + 1f),
        //                 string.Format("{0}:{1}",
        //                                command.Key,
        //                                command.Value.Description));
        //        lines++;
        //    }
        //}
        //private void CommandHandler_Clear(object sender, ExecuteCommandArgs args)
        //{
        //    debugConsole.Clear();
        //}

#endregion
#endregion

        #region Debug Aids
        private void debug_aid_sunlight()
        {
#region SunLigh Debug
            try
            {


                Vector2 avaPos = UnProject(ApplicationBase.Instance.Player.Position,
                ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix,
                Width, Height);

                float asc = 0f;
                float desc = 0f;
                float lh = 0f;



                NanoVG.nvgTextMetrics(vg, ref asc, ref desc, ref lh);

                NanoVG.nvgText(StudioWindow.vg, avaPos.X, Height - avaPos.Y,
                    string.Format("{0} [{1}]", ApplicationBase.Instance.Player.AvatarInfo.AvatarName,
                        ApplicationBase.Instance.Player.Position.ToString()));

                Vector2 sunPos = UnProject(ApplicationBase.Instance.Scene.DirectionalLights[0].Position,
                    ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix,
                    Width, Height);
                Vector2 sunLook = UnProject(ApplicationBase.Instance.Scene.DirectionalLights[0].lookAt,
                    ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix,
                    Width, Height);
                NanoVG.nvgText(StudioWindow.vg, sunPos.X, Height - sunPos.Y,
                    string.Format("SunLight0 [{0}]", ApplicationBase.Instance.Scene.DirectionalLights[0].Position.ToString()));

                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgCircle(vg, sunPos.X, Height - sunPos.Y, 50);
                NanoVG.nvgClosePath(vg);
                NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(255, 0, 0, 255));
                NanoVG.nvgStrokeWidth(vg, 2.0f);
                NanoVG.nvgStroke(vg);

                NanoVG.nvgText(StudioWindow.vg, sunLook.X, Height - sunLook.Y,
                    string.Format("SunLight Target [{0}]", ApplicationBase.Instance.Scene.DirectionalLights[0].lookAt.ToString()));
                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgMoveTo(vg, sunPos.X, Height - sunPos.Y);
                NanoVG.nvgLineTo(vg, sunLook.X, Height - sunLook.Y);
                NanoVG.nvgClosePath(vg);
                NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(255, 0, 0, 255));
                NanoVG.nvgStrokeWidth(vg, 2.0f);
                NanoVG.nvgStroke(vg);

            }
            catch { }

#endregion
        }

        bool prevInsert = false;
        bool displayMeshBounds = false;
        private void DrawBounds()
        {
            bool Insert = OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Insert);
            if (Insert && !prevInsert)
                displayMeshBounds = !displayMeshBounds;
            prevInsert = Insert;

            if (!displayMeshBounds)
                return;

#region Scene Bounding Boxes Debug

            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(255, 255, 255, 70));
            NanoVG.nvgStrokeWidth(vg, 1.0f);
            foreach (Drawable d in Scene.VisibleDrawables.Keys)
            {
                if (d is StaticModel && d.IsVisible && !d.IgnoreCulling)
                    foreach (MeshVbo m in d.meshes)
                    {

                        if (null == m.BoundingBoxLocal)
                            continue;

                        BoundingAABB box = m.BoundingBoxLocal;

                        //float dist = (m.BoundingBoxLocal.Center - Player.Position).LengthFast;
                        float dist = (m.BoundingBoxLocal.Center - Scene.EyePos).LengthFast;

                        if (dist < 0)
                            dist *= -1f;

                        if (dist < 40 || dist > 150) continue;
                        try
                        {
                            //if (null == d.meshes[0].OctreeBounds)




                            Vector2 txtPos = UnProject(m.BoundingBoxLocal.Center + d.Position,
                                    ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix,
                                    ApplicationBase.Instance.Player.ViewInfo.projectionMatrix,
                                    Width, Height);

                            NanoVG.nvgText(StudioWindow.vg, txtPos.X, Height - txtPos.Y, string.Format("{0} - Detail {1} {2} μsec", m.Name,
                                                                                                                      m.CurrentLod.ToString(),
                                                                                                                      m.DrawTimeAllPassesAccumulator / m.DrawCalls));


                            Vector3[] corners = box.GetCorners();

                            Vector2[] scrCorners = new Vector2[corners.Length];

                            for (int i = 0; i < corners.Length; i++)
                            {
                                Vector3 dpos = d.Position + corners[i];
                                //Vector3 dpos = corners[i];
                                scrCorners[i] = UnProject(dpos,
                                    ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix,
                                    ApplicationBase.Instance.Player.ViewInfo.projectionMatrix,
                                    Width, Height);

                                scrCorners[i].Y = this.Height - scrCorners[i].Y;
                            }


                            if (scrCorners[0] == Vector2.Zero &&
                                scrCorners[1] == Vector2.Zero &&
                                scrCorners[2] == Vector2.Zero &&
                                scrCorners[3] == Vector2.Zero &&
                                scrCorners[4] == Vector2.Zero &&
                                scrCorners[5] == Vector2.Zero &&
                                scrCorners[6] == Vector2.Zero &&
                                scrCorners[7] == Vector2.Zero)
                                continue;



                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[0].X, scrCorners[0].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[1].X, scrCorners[1].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[1].X, scrCorners[1].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[2].X, scrCorners[2].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[2].X, scrCorners[2].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[3].X, scrCorners[3].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[3].X, scrCorners[3].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[0].X, scrCorners[0].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[4].X, scrCorners[4].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[5].X, scrCorners[5].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[5].X, scrCorners[5].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[6].X, scrCorners[6].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[6].X, scrCorners[6].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[7].X, scrCorners[7].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[7].X, scrCorners[7].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[4].X, scrCorners[4].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[0].X, scrCorners[0].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[4].X, scrCorners[4].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[3].X, scrCorners[3].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[7].X, scrCorners[7].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[2].X, scrCorners[2].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[6].X, scrCorners[6].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                            NanoVG.nvgBeginPath(vg);
                            NanoVG.nvgMoveTo(vg, scrCorners[1].X, scrCorners[1].Y);
                            NanoVG.nvgLineTo(vg, scrCorners[5].X, scrCorners[5].Y);
                            NanoVG.nvgClosePath(vg);
                            NanoVG.nvgStroke(vg);

                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }

            }




#endregion
        }

        public override void debug_aid_frustum(Vector3 n1, Vector3 n2, Vector3 n3, Vector3 n4, Vector3 f1, Vector3 f2, Vector3 f3, Vector3 f4)
        {
            Vector2 nn1 = UnProject(n1, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix, Width, Height);
            Vector2 nn2 = UnProject(n2, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix, Width, Height);
            debug_aid_line(nn1.X, nn1.Y, nn2.X, nn2.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
            Vector2 nn3 = UnProject(n3, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix, Width, Height);
            debug_aid_line(nn2.X, nn2.Y, nn3.X, nn3.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
            Vector2 nn4 = UnProject(n4, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix, Width, Height);
            debug_aid_line(nn3.X, nn3.Y, nn4.X, nn4.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
            debug_aid_line(nn4.X, nn4.Y, nn1.X, nn1.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);

            Vector2 fn1 = UnProject(f1, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix, Width, Height);
            Vector2 fn2 = UnProject(f2, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix, Width, Height);
            debug_aid_line(fn1.X, nn1.Y, fn2.X, fn2.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
            Vector2 fn3 = UnProject(f3, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix, Width, Height);
            debug_aid_line(fn2.X, fn2.Y, fn3.X, fn3.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
            Vector2 fn4 = UnProject(f4, ApplicationBase.Instance.Player.ViewInfo.modelviewMatrix, ApplicationBase.Instance.Player.ViewInfo.projectionMatrix, Width, Height);
            debug_aid_line(fn3.X, fn3.Y, fn4.X, fn4.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
            debug_aid_line(fn4.X, fn4.Y, fn1.X, fn1.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);


            debug_aid_line(nn1.X, nn1.Y, fn1.X, fn1.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
            debug_aid_line(nn2.X, nn2.Y, fn1.X, fn2.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
            debug_aid_line(nn3.X, nn3.Y, fn1.X, fn3.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
            debug_aid_line(nn4.X, nn4.Y, fn1.X, fn4.Y, NanoVG.nvgRGBA(255, 0, 0, 70), 1f);
        }
        private void debug_aid_player_bounds()
        {
            if (StudioWindow.Instance.Player.PlayerViewMode == ApplicationUser.enuPlayerViewMode.FirstPerson)
                return;

            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(255, 255, 255, 70));
            NanoVG.nvgStrokeWidth(vg, 1.0f);
            foreach (Drawable d in Scene.Drawables)
            {
                if (d is PlayerModel && d.IsVisible)
                    try
                    {

                        Vector2[] scrCorners = ((PlayerModel)d).ScreenBounds;

                        if (((PlayerModel)d).Selected > 0f)
                        {
                            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(255, 0, 0, 70));
                            //Rafa.Graphics.Utilities.ConsoleUtil.log(string.Format("Avatar intersection: {0}", ((PlayerModel)d).Name));
                        }
                        else
                            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(255, 255, 255, 70));

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[0].X, scrCorners[0].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[1].X, scrCorners[1].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[1].X, scrCorners[1].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[2].X, scrCorners[2].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[2].X, scrCorners[2].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[3].X, scrCorners[3].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[3].X, scrCorners[3].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[0].X, scrCorners[0].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[4].X, scrCorners[4].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[5].X, scrCorners[5].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[5].X, scrCorners[5].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[6].X, scrCorners[6].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[6].X, scrCorners[6].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[7].X, scrCorners[7].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[7].X, scrCorners[7].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[4].X, scrCorners[4].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[0].X, scrCorners[0].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[4].X, scrCorners[4].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[3].X, scrCorners[3].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[7].X, scrCorners[7].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[2].X, scrCorners[2].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[6].X, scrCorners[6].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                        NanoVG.nvgBeginPath(vg);
                        NanoVG.nvgMoveTo(vg, scrCorners[1].X, scrCorners[1].Y);
                        NanoVG.nvgLineTo(vg, scrCorners[5].X, scrCorners[5].Y);
                        NanoVG.nvgClosePath(vg);
                        NanoVG.nvgStroke(vg);

                    }
                    catch (Exception e)
                    {
                        continue;
                    }
            }
        }
        private void debug_aid_line(float x1, float y1, float x2, float y2, NVGcolor color, float width = 1.0f)
        {
            try
            {
                NanoVG.nvgBeginPath(vg);
                NanoVG.nvgMoveTo(vg, x1, Height - y1);
                NanoVG.nvgLineTo(vg, x2, Height - y2);
                NanoVG.nvgClosePath(vg);
                NanoVG.nvgStrokeColor(vg, color);
                NanoVG.nvgStrokeWidth(vg, width);
                NanoVG.nvgStroke(vg);
            }
            catch { }
        }
        #endregion

        

    }
}
