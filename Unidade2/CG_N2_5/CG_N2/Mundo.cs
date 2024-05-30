//TODO: testar se estes DEFINEs continuam funcionado
#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
// #define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
// using OpenTK.Mathematics;

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private readonly float[] _sruEixos =
        {
            -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
            0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
            0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
        };
        private static Objeto mundo = null;
        private char rotuloAtual = '?';
        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;
        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;
        private Circulo circuloMaior;
        private Circulo circuloMenor;
        private Retangulo box;
        private Ponto pontoCentral;
        private BBox bbox;

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
        }

        protected override void OnLoad()
        {
            configPadrao();    
            criarComponentes(mundo, rotuloAtual);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

#if CG_Gizmo
            Sru3D();
#endif
            mundo.Desenhar();
            SwapBuffers();
        }

        private void configPadrao()
        {
            base.OnLoad();

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

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
            #endregion
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            double positivo = 0.01;
            double negativo = -0.01;
            double padrao = 0.0;

            base.OnUpdateFrame(e);

            #region Teclado
            var input = KeyboardState;
            //ir esquerda
        if (input.IsKeyPressed(Keys.E) || input.IsKeyPressed(Keys.Left)) 
            {
                // vai movimentar o ponto central do circulo que se move 
                moverPrincipal(negativo, padrao);
            }
            //ir direita
            if (input.IsKeyPressed(Keys.D) || input.IsKeyPressed(Keys.Right)) 
            {
                moverPrincipal(positivo, padrao);
            }
            //ir cima
            if (input.IsKeyPressed(Keys.C) || input.IsKeyPressed(Keys.Up)) 
            {
                moverPrincipal(padrao, positivo);
            }
            //ir baixo
            if (input.IsKeyPressed(Keys.B) || input.IsKeyPressed(Keys.Down)) 
            {
                moverPrincipal(padrao, negativo);
            }
            #endregion
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

#if CG_Gizmo
        private void Sru3D()
        {
#if CG_OpenGL && !CG_DirectX
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
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
        }
#endif

        private void criarComponentes(Objeto mundo, char rotuloAtual) 
        {
            pontoCentral = new Ponto(mundo, ref rotuloAtual, new Ponto4D(0.2, 0.2)) {
                PrimitivaTamanho = 3,
                shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag")
            };

            circuloMaior = new Circulo(mundo, ref rotuloAtual, 0.2, 0.2) {
                shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag")
            };

            circuloMenor = new Circulo(mundo, ref rotuloAtual, 0.05, 0.2) {
                shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag")
            };

            box = new Retangulo(mundo, ref rotuloAtual, circuloMaior.PontosId(9), circuloMaior.PontosId(45)) {
                PrimitivaTipo = PrimitiveType.LineLoop,
                shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag")
            };
       
            //adicionando um BBox no tamanho do quadrado
            List<Ponto4D> listaBBox = new List<Ponto4D> {circuloMaior.PontosId(9), circuloMaior.PontosId(45)};
            bbox = new BBox();
            bbox.Atualizar(new Transformacao4D(), listaBBox);
        }

        private void moverPrincipal(double deslocX, double deslocY) {
            
            Ponto4D aux = new Ponto4D(pontoCentral.PontosId(0).X + deslocX,  pontoCentral.PontosId(0).Y + deslocY);     
 
            // 0.04 é o quadrado da distância máxima permitida entre os centros dos dois círculos = 0,2 x 0,2
            if (circuloMaior.Bbox().Dentro(aux) && Matematica.distanciaQuadrado(aux, new Ponto4D(0.2, 0.2)) < 0.04)
            {
                //atualiza o ponto principal
                pontoCentral.PontosId(0).X += deslocX;
                pontoCentral.PontosId(0).Y += deslocY;
                pontoCentral.Atualizar();

                //gera os pontos do circulo menor novamente
                circuloMenor.deslocarCirculo(deslocX, deslocY);
                
                //valida se ainda está dentro do quadrado
                box.PrimitivaTipo = bbox.Dentro(pontoCentral.PontosId(0)) ?  PrimitiveType.LineLoop : PrimitiveType.Points;      
            }
        }
    }
}
