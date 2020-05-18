using System;
using System.IO;

namespace NthStudio.Compiler.VisualStudio.Project
{
    class Project
    {

        void analyze()
        {
            try
            {

                //analyzeProject( @"C:\Users\Administrator\Desktop\Test Solution\ConsoleApp1\ConsoleApp1",
                //                "ConsoleApp1",
                //                true,
                //                true
                //              );


                var analyserParams                          = new AnalyserParams();

                analyserParams.Path                         = @"C:\Users\Administrator\Desktop\Test Solution\ConsoleApp1";  // Source Code dir
                analyserParams.AssemblyToAnalyse            = "ConsoleApp1";
                analyserParams.IncludeSystemDependencies    = true;
                analyserParams.Summary                      = true;
                analyserParams.RecurseDependencies          = true;
                analyserParams.RecurseDependenciesMaxDepth  = 3;

                if (Directory.Exists(analyserParams.Path) == false)
                {
                    Console.WriteLine("Invalid directory {0}", analyserParams.Path);
                    return;
                }

                using (var sw = new StreamWriter(Console.OpenStandardOutput()))
                {
                    sw.AutoFlush = true;
                    var outt = Console.Out;
                    Console.SetOut(sw);

                    //// HERE IS THE INTERESTING PART
                    //// Identical to my Handler.Process() routine

                    //(new Analyser()).Process(analyserParams, sw);
                    Analyser a = new Analyser();
                    a.Process(analyserParams, sw);

                    Console.SetOut(outt);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.Write("\n\nDone! Press Enter to continue...");
            Console.ReadLine();
        }

    }
}
