using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using CG_Biblioteca;
using System;

namespace gcgcg
{
    // Define a janela onde o Círculo será desenhado (individualmente)
    public class WindowCirculo : GameWindow
    {
        private static Circulo circulo = null;
        private char rotuloAtual = '?';

        private readonly float[] _sruEixos =
        {
            0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
            0.0f,  0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
            0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.0f, /* Z+ */
        };
        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;
        // objeto que armazena a linha vermelha
        private Shader _shaderVermelha;
        // objeto que armazena a linha verde
        private Shader _shaderVerde;
        // objeto que armazena a linha azul (inexistente nesse exercício)
        private Shader _shaderAzul;

        public WindowCirculo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            circulo ??= new Circulo(null, ref rotuloAtual); // padrão Singleton
            // aqui define cor amarela para os pontos
            circulo.shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
        }

        protected override void OnLoad() {
            base.OnLoad();
            // aqui troca a cor do plano de fundo
            GL.ClearColor(0.5f, 0.5f, 0.7f, 1.0f);

            // aqui define as retas do plano
            #region Eixos: SRU  

            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // aqui define as cores que busca na pasta shaders
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            #endregion
        }

        // Desenha o conteúdo na janela (o círculo e as linhas)
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // aqui constrói as linhas no circulo
            // EixoX
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.Use();
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);

            // Desenha o círculo
            circulo.Desenhar();

            SwapBuffers();
        }
    }
}
