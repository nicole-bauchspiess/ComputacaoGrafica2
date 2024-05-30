using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private static Objeto mundo = null;
        private char rotuloAtual = '?';
        private Spline spline;

        private readonly float[] _sruEixos =
        {
            -0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
            0.0f, -0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
            0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
        };

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;
        private Shader _shaderBranca;
        private Shader _shaderAmarela;
        private Shader _shaderCiano;
        private Objeto objetoSelecionado = null;

        private int index = 0;

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo ??= new Objeto(null, ref rotuloAtual); // padrão Singleton
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            #region Eixos: SRU  
            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            #endregion

            #region Objeto: Spline  
            spline = new Spline(mundo, ref rotuloAtual);
            #endregion
            
            
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            Sru3D();

            mundo.Desenhar();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e); 

            var input = KeyboardState;

            if (input.IsKeyPressed(Keys.Space)) {
                spline.vinculoObjeto();
                spline.selecionaPontoVermelho();
            } 
            
            if (input.IsKeyPressed(Keys.C)) { 
                spline.adicionarY();
                atualizarSegRetas();
                spline.atualizarSpline();
            }

            if (input.IsKeyPressed(Keys.B)) { 
                spline.diminuirY();
                atualizarSegRetas();
                spline.atualizarSpline();
            }

            if (input.IsKeyPressed(Keys.D)) { 
                spline.adicionarX();
                atualizarSegRetas();
                spline.atualizarSpline();
            }

            if (input.IsKeyPressed(Keys.E)) { 
                spline.diminuirX();
                atualizarSegRetas();
                spline.atualizarSpline();
            }

            if (input.IsKeyPressed(Keys.KeyPadAdd)) {
                spline.adicionarValorT();
                atualizarSegRetas();
                spline.atualizarSpline();
            }
 
            if (input.IsKeyPressed(Keys.Comma)) {
                spline.decrementarValorT();
                spline.atualizarSpline();
                atualizarSegRetas();
            }                            
        }

        private void atualizarSegRetas() {
            spline.segReta1.ObjetoAtualizar();
            spline.segReta2.ObjetoAtualizar();
            spline.segReta3.ObjetoAtualizar();
            spline.pontos[spline.indice].Atualizar();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

            GL.DeleteProgram(_shaderVermelha.Handle);
            GL.DeleteProgram(_shaderVerde.Handle);
            GL.DeleteProgram(_shaderAzul.Handle);

            base.OnUnload();
        }

        private void Sru3D()
        {
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // EixoX
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.Use();
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);
        }
    }
}
