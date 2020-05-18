using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

using NthStudio.Plugins;
using NthDimension.FFMpeg;
using NthDimension.Utilities;
using NthDimension.Context;
using NthDimension.Rasterizer.Windows;
using NthDimension.Rendering.Configuration;

//using NthDimension.Service;
//using NthDimension.Forms;


namespace NthStudio
{
    partial class Program
    {
        // Application Scope
        private static PluginStore              pluginStore;
        public static PluginStore               PluginStore // TODO:: Refactor and move to Project Code (Each Project, it's own plugins? -OR- Plugins per Studio application?)
        {
            get { return pluginStore; }
            set { pluginStore = value; }
        }
        
        // OpenGL Scope
        static OpenTK.ToolkitOptions            tkOptions;
        static IList<OpenTK.DisplayDevice>      tkMonitors;
        static OpenTK.Graphics.GraphicsMode     appGfx;
        static int                              gfxColorFormatBpp   = 32;
        static int                              gfxDepthBits        = 24;
        static int                              gfxStencilBits      = 8;
        static int                              gfxAARequest        = 2;
        static int                              gfxBuffers          = 2;
        static bool                             gfxStereo           = false;

        static void Main(string[] args)
        {
            // TODO:: FUTURE USE
            // Look for missing dlls in specific folders (from morpho)
            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
           
            //System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            string title = string.Format("NthStudio (Development) : {0}", Environment.MachineName);
            Console.WriteLine(title);
            Console.WriteLine(string.Concat(new string[] 
                                            {
                                                new string('-', title.Length)
                                            }));
            //Plugins();

            if(null != pluginStore)
                if (pluginStore.Plugins.Count > 0)
                {
                    Console.WriteLine(":Plugins ({0}){1}", pluginStore.Plugins.Count, Environment.NewLine);                
                    foreach (PluginInfo pi in pluginStore.Plugins)
                        Console.WriteLine(string.Format(" - {3}{0}   {2}{0}   [ {4} ]{0}   [ {1}, Version {6} {7} ]{0}   [ Type: {5} ]{0}",
                                                            Environment.NewLine,
                                                            pi.AssemblyFile,
                                                            pi.Description,
                                                            pi.Name,
                                                            pi.InstallPath,
                                                            pi.Type,
                                                            pi.AssemblyVersion,
                                                            pi.AssemblyDate));
                }

            

            Graphics3D_Init();
            Graphics3D_Window();
        }

        private static void CurrentDomain_UnhandledException1(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        #region Assembly Resolve
        /// <summary>
        /// The following allows the plugin manager to load external assemblies
        /// that are referenced by plugins. These plugins are assumed to be
        /// in the plugins folder, and is resolved after the CLR heuristics
        /// for loading an assembly fails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //PluginStore pluginStore = PluginStoreManager.LoadPuginStore();
            pluginStore = PluginStoreManager.LoadPuginStore();
            if (null == pluginStore) return null;

            int index = args.Name.IndexOf(',');
            string assemblyName;
            if (index > 0)
            {
                assemblyName = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";
            }
            else
                assemblyName = args.Name + ".dll";

            foreach (PluginInfo plugin in pluginStore.Plugins)
            {
                string assemblyFile             = Path.Combine(plugin.InstallPath, assemblyName);
                assemblyFile                    = EnvironmentSettings.GetFullPath(Path.Combine(plugin.InstallPath, assemblyName));
                if (File.Exists(assemblyFile))
                {
                    Assembly asm        = Assembly.LoadFile(assemblyFile);
                    FileInfo fi         = new FileInfo(assemblyFile);

                    plugin.AssemblyVersion          = asm.ImageRuntimeVersion;
                    plugin.AssemblyDate             = fi.CreationTime.ToString();               // WARNING! BUGPRONE! No culture has been set

                    return asm;
                }
            }
            return null;
        }
        #endregion

        #region DIRegistration
        static void DIRegistration(IoCContainer container)
        {
            pluginStore = PluginStoreManager.LoadPuginStore();

            foreach (PluginInfo plugin in pluginStore.Plugins)
            {
                Assembly assembly       = null;
                string assemblyFile     = EnvironmentSettings.GetFullPath(plugin.AssemblyFile);
                string assemblyVersion  = "(Not Set)";
                string assemblyDate     = "(Not Set)";
                if (File.Exists(assemblyFile))
                    assembly            = Assembly.LoadFile(assemblyFile);

                if (null != assembly)
                {
                    FileInfo fi = new FileInfo(assemblyFile);
                    assemblyVersion = assembly.ImageRuntimeVersion;
                    assemblyDate    = fi.CreationTime.ToString();

                    var q = from t in assembly.GetTypes()
                            where t.IsClass
                            select t;

                    List<Type> assemblyClasses = q.ToList();

                    foreach (Type clss in assemblyClasses)
                    {
                        //TODO:: Populate IReaders list and handle later on control creation?

                        //if(clss is IReader)
                        //    container.Register<IReader, 
                        //        Activator.CreateInstance(Assembly.LoadFile(EnvironmentSettings.GetFullPath(plugin.AssemblyFile)).GetType(plugin.Type))
                        //        >(LifeTimeOptions.ContainerControlledLifeTimeOption);

                        //if(clss.GetType() is INotifyControl)
                        //    container.Register<INotifyControl, >(LifeTimeOptions.ContainerControlledLifeTimeOption);

                        //if (clss is BaseIocForm)
                        //{
                        //    if (clss is INotifyControl)
                        //    {

                        //    }

                        //}
                    }
                }
            }

            //container.Register<IReader, KeyboardReader>(LifeTimeOptions.ContainerControlledLifeTimeOption);
            //container.Register<IWriter, PrinterWriter>(LifeTimeOptions.TransientLifeTimeOption);
        }
        #endregion
      
        static void Plugins()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            // Create the container object
            IoCContainer        container           = new IoCContainer();
            List<Type>          interfaces          = PluginFactory.GetInversionInterfaces();

            DIRegistration(container);

            AppDomain.CurrentDomain.AssemblyResolve -= (CurrentDomain_AssemblyResolve);
        }
        static void Graphics3D_Init()
        {
            LoadSettings();

            ////GameWidth = Settings.Instance.video.windowWidth;
            ////GameHeight = Settings.Instance.video.windowHeight;

            ////if (GameWidth == 0) GameWidth = 640;
            ////if (GameHeight == 0) GameHeight = 480;

            ////Fullscreen = Settings.Instance.video.fullScreen;

            /*OpenTK.ToolkitOptions */
                tkOptions = new OpenTK.ToolkitOptions();

            if (Environment.OSVersion.Platform != PlatformID.Unix)
                tkOptions.Backend = OpenTK.PlatformBackend.PreferNative;
            else
                tkOptions.Backend = OpenTK.PlatformBackend.Default;

            tkOptions.EnableHighResolution = NthDimension.Settings.Instance.video.highResolution;

            OpenTK.Toolkit.Init(tkOptions);

            /*IList<OpenTK.DisplayDevice>*/ 
                tkMonitors = OpenTK.DisplayDevice.AvailableDisplays;

            /*OpenTK.Graphics.GraphicsMode*/ 
                appGfx = new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(gfxColorFormatBpp),             // Color
                                                    gfxDepthBits,                                                                                 // Depth
                                                    gfxStencilBits, //0                                                                              // Stencil (number of bits)
                                                    GetMaxAntiAliasingAvailable(gfxAARequest),                                                     // Samples (AA)
                                                    new OpenTK.Graphics.ColorFormat(0),                                                 // Accumulation
                                                    gfxBuffers,                                                                                  // Buffers
                                                    gfxStereo);            
        }
        static void Graphics3D_Window()
        {
            OpenTK.Graphics.GraphicsContextFlags appGfxFlags = //OpenTK.Graphics.GraphicsContextFlags.Default |
                                                                OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible;

            //new OpenTK.Graphics.GraphicsMode(32, 16, 8, 4, 0, 8, false),
            using (NthStudio.StudioWindow g = new StudioWindow(
                ".\\",
                "NthDimension Sample",
                new RendererGL3x(),
                new AudioGL3x(),
                NthDimension.Settings.Instance.video.windowWidth,
                NthDimension.Settings.Instance.video.windowHeight,
                NthDimension.Settings.Instance.video.fullScreen,
                appGfx,
                tkMonitors[0],
                3,
                0,
                appGfxFlags,
                true))
            {
                //VideoGL3x.Read(Path.Combine(DirectoryUtil.AssemblyDirectory, GameSettings.VideoFolder));


                int targetFps = NthDimension.Settings.Instance.video.targetFrameRate;

                if (targetFps < 1)
                    targetFps = 1;
                if (targetFps >= 600)
                    targetFps = 600;


                g.Run(targetFps);
            }
        }
        public static void LoadSettings()
        {
            if (!File.Exists(Path.Combine(DirectoryUtil.Documents, "settings.xml")))
                NthDimension.Settings.Instance.SaveSettings(Path.Combine(DirectoryUtil.Documents, "settings.xml")); // settings is not existing, lets save default ones
            NthDimension.Settings.Instance.LoadSettings(Path.Combine(DirectoryUtil.Documents, "settings.xml"));
        }
        protected static int GetMaxAntiAliasingAvailable(int aaDesired)
        {
            int aa = 0;
            int[] aaModes = CalculeAntiAliasingModes();
            foreach (int i in aaModes)
                if (i == aaDesired)
                {
                    aa = i;
                    break;
                }
                else if (i < aaDesired && i > aa)
                {
                    aa = i;
                }

            return aa;
        }
        protected static int[] AntiAliasingModes
        {
            get;
            private set;
        }

        public static int[] CalculeAntiAliasingModes()
        {
            List<int> aa_modes = new List<int>();
            int aa = 0;
            do
            {
                try
                {
                    OpenTK.Graphics.GraphicsMode mode = new OpenTK.Graphics.GraphicsMode(32, 0, 0, aa);
                    if (!aa_modes.Contains(mode.Samples))
                        aa_modes.Add(aa);
                }
                catch (Exception)
                {
                }
                finally
                {
                    aa += 2;
                }
            } while (aa <= 32);

            return aa_modes.ToArray();
        }

    }
}
