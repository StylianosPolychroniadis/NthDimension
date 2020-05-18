using System;

namespace GLTerrain {
    class MainProgram {
        [STAThread]
        private static void Main(string[] args) {
            int width = 1440;
            int height = 900;

            using (SimWindow window = new SimWindow(width, height, false)) {
                window.Run();
            }
        }
    }
}
