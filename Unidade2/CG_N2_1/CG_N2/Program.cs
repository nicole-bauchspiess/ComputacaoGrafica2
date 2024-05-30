using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using gcgcg;

// Este é o programa principal
namespace gcgcg
{
    public static class Program
    {
        private static void Main()
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(800, 800),
                Title = "CG_N2_Exemplo",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };

            //TODO: encontrar o comando certo para não dar problema de dobrar a resolução de tela no MacOS
            // ToolkitOptions.Default.EnableHighResolution = false;
            // modificado para abrir WindowCirculo ao invés de Mundo (que contém retangulo, poligono, ponto, reta) 
            using var window = new WindowCirculo(GameWindowSettings.Default, nativeWindowSettings);
            window.Run();
        }
    }
}
