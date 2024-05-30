using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public Circulo(Objeto _paiRef, ref char _rotulo, Objeto objetoFilho = null) : base(_paiRef, ref _rotulo, objetoFilho)
        {
            // vem da classe abstrata Objeto
            PrimitivaTipo = PrimitiveType.Points;
            // define o tamanho dos pontos 
            PrimitivaTamanho = 5;
            List<Ponto4D> pontosCirculo = CalcularPontosCirculo();
            foreach (Ponto4D ponto in pontosCirculo) {
                PontosAdicionar(ponto);
            }

            // Adiciona a linha do centro até a extremidade do círculo
            Ponto4D pontoCentral = new Ponto4D(0, 0);
            PontosAdicionar(pontoCentral); // Adiciona o ponto central
            Atualizar();
        }

        public void Atualizar()
        {
            base.ObjetoAtualizar();
        }

        public static List<Ponto4D> CalcularPontosCirculo()
        {
            List<Ponto4D> pontos = [];
            
            // número de pontos cálculados ao longo do perímetro 
            // quanto maior o número de pontos mais suave o círculo
            int numPontos = 72;

            double raio = 0.5; // raio de 0.5 determina o tamanho do círculo

            // calcula as coordenadas x,y de cada ponto no círculo
            // A equação geral de um círculo no plano cartesiano é:
            // x^2 + y^2 = r^2
            // para calcular os pontos de um círculo, usar trigonometria para relacionar as coordenadas de um ponto com ângulo O
            
            // para cada ponto no círculo
            for (int i = 0; i < numPontos; i++)
            {
                // calcular o angulo correspondente 
                // divide 2pi (círculo completo) pelo número total de pontos, multiplicando pelo índice atual (i)
                double angulo = 360 / numPontos * i;
                // calcula as coordenadas com base no ângulo e no raio
                Ponto4D ponto = Matematica.GerarPtosCirculo(angulo, raio);
                pontos.Add(ponto);
            }

            return pontos;
        }

        #if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Círculo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
        #endif
    } 

}