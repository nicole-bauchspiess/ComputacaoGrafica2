#define CG_DEBUG
#define CG_Gizmo      
#define CG_OpenGL      
// #define CG_OpenTK
// #define CG_DirectX      
#define CG_Privado  

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Linq;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;

    private char rotuloAtual = '?';
    private Objeto objetoSelecionado = null;

    private readonly float[] _sruEixos =
    [
       0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f,  0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f,  0.0f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    ];

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    private int _vertexBufferObject_bbox;
    private int _vertexArrayObject_bbox;

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;

    private List<Ponto4D> pontos = new List<Ponto4D>(); //poligonpoints
    private Objeto novoPoligonoTela; //vai ser um poligono
    private Shader[] cores = new Shader[4];
    private int indexCor = 0;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      Utilitario.Diretivas();
#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      #endregion


      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion

      #region Objeto: polígono qualquer  
      List<Ponto4D> pontosPoligonoBandeira = new List<Ponto4D>();
      pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.25));  // A = (0.25, 0.25)
      pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.25));  // B = (0.75, 0.25)
      pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.75));  // C = (0.75, 0.75)
      pontosPoligonoBandeira.Add(new Ponto4D(0.50, 0.50));  // D = (0.50, 0.50)
      pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.75));  // E = (0.25, 0.75)
  //    objetoSelecionado = new Poligono(mundo, ref rotuloAtual, pontosPoligonoBandeira);
      #endregion
      #region declara um objeto filho ao polígono
      List<Ponto4D> pontosPoligonoTriangulo = new List<Ponto4D>();
      pontosPoligonoTriangulo.Add(new Ponto4D(0.50, 0.50)); // F = (0.50, 0.50)
      pontosPoligonoTriangulo.Add(new Ponto4D(0.75, 0.75)); // G = (0.75, 0.75)
      pontosPoligonoTriangulo.Add(new Ponto4D(0.25, 0.75)); // H = (0.25, 0.75)
  //    objetoSelecionado = new Poligono(objetoSelecionado, ref rotuloAtual, pontosPoligonoTriangulo);
      #endregion
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      mundo.Desenhar(new Transformacao4D());

#if CG_Gizmo      
      Gizmo_Sru3D();
      //Gizmo_BBox();
#endif
      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyDown(Keys.Escape))
        Close();
      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
        if (objetoSelecionado == null)
          objetoSelecionado = mundo;
        // objetoSelecionado.shaderObjeto = _shaderBranca;
        objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
        // objetoSelecionado.shaderObjeto = _shaderAmarela;
      }
      if (estadoTeclado.IsKeyPressed(Keys.G))                 //TODO: testar com grafo maior ,, irmãos
        mundo.GrafocenaImprimir("");
      if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
        Console.WriteLine(objetoSelecionado.ToString());
      if (estadoTeclado.IsKeyPressed(Keys.M) && objetoSelecionado != null)
        objetoSelecionado.MatrizImprimir();
      //TODO: não está atualizando a BBox com as transformações geométricas
      if (estadoTeclado.IsKeyPressed(Keys.I) && objetoSelecionado != null)
        objetoSelecionado.MatrizAtribuirIdentidade();
      if (estadoTeclado.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
      if (estadoTeclado.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
      if (estadoTeclado.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
      if (estadoTeclado.IsKeyPressed(Keys.Home) && objetoSelecionado != null)   //FIXME: problema depois de usa escala pto qquer, pois escala pto fixo não usa o novo centro da BBOX
        objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
      if (estadoTeclado.IsKeyPressed(Keys.End) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
      if (estadoTeclado.IsKeyPressed(Keys.D1) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(10);
      if (estadoTeclado.IsKeyPressed(Keys.D2) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(-10);
      if (estadoTeclado.IsKeyPressed(Keys.D3) && objetoSelecionado != null)   //FIXME: problema depois de usa rotação pto qquer, não usa o novo centro da BBOX
          objetoSelecionado.MatrizRotacaoZBBox(10);
      if (estadoTeclado.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(-10);
      // enter limpa tudo
      if (estadoTeclado.IsKeyPressed(Keys.Enter)) 
      {
        objetoSelecionado = new Poligono(mundo, ref rotuloAtual, new List<Ponto4D>(pontos));
        pontos.Clear();
      }

      if (estadoTeclado.IsKeyPressed(Keys.D)) 
      {
        removerPoligono();
      }
      if (estadoTeclado.IsKeyDown(Keys.V) && objetoSelecionado != null) {
        moverPoligonoVertice();
      }
      if (estadoTeclado.IsKeyPressed(Keys.E)) 
      {
        removerPontoDoPoligono();
      }

//questão 8 -> mudar de cor
      if (estadoTeclado.IsKeyPressed(Keys.R)) 
      {
        objetoSelecionado.ShaderObjeto = _shaderVermelha;
      }
      if (estadoTeclado.IsKeyPressed(Keys.G)) 
      {
         objetoSelecionado.ShaderObjeto = _shaderVerde;
      }
      if (estadoTeclado.IsKeyPressed(Keys.B)) 
      {
        objetoSelecionado.ShaderObjeto = _shaderCiano;
      }

      #endregion

      #region  Mouse

      if (MouseState.IsButtonPressed(MouseButton.Left))
      {
        objetoSelecionado = mundo;
      }

      if (MouseState.IsButtonReleased(MouseButton.Right))
      {
        desenharPoligono();  
      }

      #endregion

    }

//BOTAO DIREITO MOUSE
    private void desenharPoligono() {
      pontos.Add(getMouseLocation());
        
        // se conter dois pontos, significa que tem que mostrar algo em tela, está iniciando o desenho de um novo polígono
        if (pontos.Count == 2) {
          Objeto objetoPai = mundo;

          if(objetoSelecionado!= null){
            objetoPai = objetoSelecionado;
          }

          novoPoligonoTela = new Poligono(objetoPai, ref rotuloAtual, new List<Ponto4D>(pontos));
          objetoSelecionado = novoPoligonoTela;

        // se for maior que 2, significa que está incrementando a quantidade de pontos do polígono.
        // pega o último ponto guardado e mostra na tela
        } else if (pontos.Count > 2) {
          objetoSelecionado.PontosAdicionar(pontos.Last());
          objetoSelecionado.ObjetoAtualizar();
        }
    }


//TECLA D -> questao 3 -> remove poligono
    private void removerPoligono() {
      mundo.RemoverObjeto(objetoSelecionado);
      objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);

      if (objetoSelecionado == null) {
        objetoSelecionado = mundo;
      }
    }

 
 //TECLA V -> questao 4 -> move o vertice mais proximo encontrado
    private void moverPoligonoVertice(){
      int indice = objetoSelecionado.PontoMaisPerto(getMouseLocation());
      objetoSelecionado.PontosAlterar(getMouseLocation(), indice);
    }


//TECLA E-> questão 5 -> remove vertice do poligono
    private void removerPontoDoPoligono() {
        objetoSelecionado.RemoverPontoMaisPertoMouse(getMouseLocation());

      if (objetoSelecionado.tamanhoLista() == 1) {
        mundo.RemoverObjeto(objetoSelecionado);
      }
    }



    private Ponto4D getMouseLocation() {
      int janelaLargura = Size.X;
      int janelaAltura = Size.Y;
      
      Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);

      return Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
      GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteBuffer(_vertexBufferObject_bbox);
      GL.DeleteVertexArray(_vertexArrayObject_bbox);

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("transform", transform);
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("transform", transform);
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

#if CG_Gizmo
    private void Gizmo_BBox()   //FIXME: não é atualizada com as transformações globais
    {
      if (objetoSelecionado != null)
      {

#if CG_OpenGL && !CG_DirectX

        float[] _bbox =
        {
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // A
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // B
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f, // C
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f  // D
      };

        _vertexBufferObject_bbox = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bbox);
        GL.BufferData(BufferTarget.ArrayBuffer, _bbox.Length * sizeof(float), _bbox, BufferUsageHint.StaticDraw);
        _vertexArrayObject_bbox = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject_bbox);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        var transform = Matrix4.Identity;
        GL.BindVertexArray(_vertexArrayObject_bbox);
        _shaderAmarela.SetMatrix4("transform", transform);
        _shaderAmarela.Use();
        GL.DrawArrays(PrimitiveType.LineLoop, 0, (_bbox.Length / 3));

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
      }
    }
#endif    

  }
}
