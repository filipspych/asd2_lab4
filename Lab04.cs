namespace ASD
{
    using ASD.Graphs;
    using System;
    using System.Collections.Generic;

    static class ExtensionsToGraph
    {
        public static void Print(this Graph g)
        {
            Console.WriteLine($"GRAF. wierzcholkow: {g.VerticesCount}, krawedzi: {g.EdgesCount}");
            for (int i = 0; i < g.VerticesCount; i++)
            {
                Console.Write($"{i}->");
                if (g.OutDegree(i) == 0)
                {
                    Console.WriteLine("BRAK");
                    continue;
                }
                foreach (Edge e in g.OutEdges(i))
                {
                    Console.Write($"{e.To} ");
                }
                Console.WriteLine();
            }
        }

        public static void Image(this Graph g)
        {
            GraphExport graphExport = new GraphExport(true, null, "chrome", "", null);
            graphExport.Export(g);
        }
    }

    public class Lab04 : System.MarshalByRefObject
    {
        /// <summary>
        /// Część I: wyznaczenie zbioru potencjalnie zarazonych obywateli.
        /// </summary>
        /// <param name="Z">Zbiór osób zarażonych na początku epidemii, uporządkowany rosnąco</param>
        /// <param name="G">Informacja o spotkaniach; wagi krawędzi są nieujemne i oznaczają czas spotkania</param>
        /// <returns>Lista potencjalnie zarażonych obywateli, uporządkowana rosnąco</returns>
        public List<int> QuarantineTargets(List<int> Z, Graph g)
        {
            return _QuarantineTargets(Z, g.Clone());
        }

        List<int> _QuarantineTargets(List<int> Z, Graph g)
        {
            bool[] infected = new bool[g.VerticesCount];
            double[] infectionTime = new double[g.VerticesCount];
            List<Edge> pQ = new List<Edge>();
            List<int> ret = new List<int>();

            foreach (var v in Z) infected[v] = true;
            foreach (var v in Z) foreach (Edge e in g.OutEdges(v)) if (infected[e.To]) g.DelEdge(e);

            for (int i = 0; i < g.VerticesCount; i++)
                foreach (Edge e in g.OutEdges(i)) //if (!(e.To < i || (infected[e.To] && infected[e.From])))
                {
                    pQ.Add(e);
                    g.DelEdge(e);
                }
            pQ.Sort((Edge e1, Edge e2) => { return e1.Weight.CompareTo(e2.Weight); });

            foreach (Edge e in pQ)
            {
                if (infected[e.To] != infected[e.From])
                {
                    if (infected[e.To] && (infectionTime[e.To] != e.Weight))
                    {
                        infected[e.From] = true;
                        infectionTime[e.From] = e.Weight;
                    }
                    else if (infected[e.From] && (infectionTime[e.From] != e.Weight))
                    {
                        infected[e.To] = true;
                        infectionTime[e.To] = e.Weight;
                    }
                }
            }

            for (int i = 0; i < g.VerticesCount; i++) if (infected[i]) ret.Add(i);
            return ret;
        }

        void _QuarantineTargetsBackward(int z, List<Edge> pQ, bool[] couldBeZero)
        {
            bool[] infected = new bool[couldBeZero.Length];
            infected[z] = true;
            double[] infectionTime = new double[couldBeZero.Length];

            foreach (Edge e in pQ)
            {
                if (infected[e.To] != infected[e.From])
                {
                    if (infected[e.To] && (infectionTime[e.To] != e.Weight))
                    {
                        infected[e.From] = true;
                        infectionTime[e.From] = e.Weight;
                    }
                    else if (infected[e.From] && (infectionTime[e.From] != e.Weight))
                    {
                        infected[e.To] = true;
                        infectionTime[e.To] = e.Weight;
                    }
                }
            }

            for (int i = 0; i < couldBeZero.Length; i++) if (!infected[i]) couldBeZero[i] = false;
        }

        /// <summary>
        /// Część II: wyznaczenie zbioru potencjalnych pacjentów zero.
        /// </summary>
        /// <param name="S">Zbiór osób zakażonych przez potencjalnego pacjenta zero</param>
        /// <param name="G">Informacja o spotkaniach; wagi krawędzi są nieujemne i oznaczają czas spotkania</param>
        /// <returns>Lista potencjalnych pacjentów zero, uporządkowana rosnąco</returns>
        public List<int> PotentialPatientsZero(List<int> S, Graph g)
        {
            List<int> ret = new List<int>();
            List<Edge> pQ = new List<Edge>();
            bool[] couldBeZero = new bool[g.VerticesCount];

            for (int i = 0; i < couldBeZero.Length; i++) couldBeZero[i] = true;
            for (int i = 0; i < g.VerticesCount; i++) foreach (Edge e in g.OutEdges(i)) if (e.To > i) pQ.Add(e);
            pQ.Sort((Edge e1, Edge e2) => { return e2.Weight.CompareTo(e1.Weight); });

            foreach (int v in S) _QuarantineTargetsBackward(v, pQ, couldBeZero);

            for (int i = 0; i < couldBeZero.Length; i++) if (couldBeZero[i]) ret.Add(i);
            return ret;
        }
    }
}
