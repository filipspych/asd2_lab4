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
            for (int i = 0; i < g.VerticesCount; i++) foreach (Edge e in g.OutEdges(i)) if (e.To > i) pQ.Add(e);
            pQ.Sort((Edge e1, Edge e2) => { return Math.Sign(e1.Weight - e2.Weight); });

            foreach (Edge e in pQ)
            {
                if (infected[e.To] != infected[e.From])
                {
                    if (infected[e.To] && (infectionTime[e.To] != e.Weight))
                    {
                        infected[e.From] = true;
                        infectionTime[e.From] = e.Weight;
                    } else if (infected[e.From] && (infectionTime[e.From] != e.Weight))
                    {
                        infected[e.To] = true;
                        infectionTime[e.To] = e.Weight;
                    }
                }
            }

            for (int i = 0; i < g.VerticesCount; i++) if (infected[i]) ret.Add(i);
            return ret;
        }

        void _QuarantineTargetsBackward(List<int> Z, Graph g, int[] targets)
        {
            bool[] infected = new bool[g.VerticesCount];
            double[] infectionTime = new double[g.VerticesCount];
            List<Edge> pQ = new List<Edge>();

            foreach (var v in Z) infected[v] = true;
            for (int i = 0; i < g.VerticesCount; i++) foreach (Edge e in g.OutEdges(i)) if (e.To > i) pQ.Add(e);
            pQ.Sort((Edge e1, Edge e2) => { return Math.Sign(e2.Weight - e1.Weight); });

            foreach (Edge e in pQ)
            {
                if (infected[e.To] != infected[e.From])
                {
                    if (infected[e.To] && (infectionTime[e.To] != e.Weight))
                    {
                        infected[e.From] = true;
                        infectionTime[e.From] = e.Weight;
                    } else if (infected[e.From] && (infectionTime[e.From] != e.Weight))
                    {
                        infected[e.To] = true;
                        infectionTime[e.To] = e.Weight;
                    }
                }
            }

            for (int i = 0; i < targets.Length; i++) if (infected[i]) targets[i]++;
        }

        /// <summary>
        /// Część II: wyznaczenie zbioru potencjalnych pacjentów zero.
        /// </summary>
        /// <param name="S">Zbiór osób zakażonych przez potencjalnego pacjenta zero</param>
        /// <param name="G">Informacja o spotkaniach; wagi krawędzi są nieujemne i oznaczają czas spotkania</param>
        /// <returns>Lista potencjalnych pacjentów zero, uporządkowana rosnąco</returns>
        public List<int> PotentialPatientsZero(List<int> S, Graph g)
        {
            int[] potentialZeros = new int[g.VerticesCount];
            List<int> ret = new List<int>();

            foreach (int v in S) _QuarantineTargetsBackward(new List<int> { v }, g, potentialZeros);

            for (int i = 0; i < potentialZeros.Length; i++) if (potentialZeros[i] == S.Count) ret.Add(i);
            return ret;
        }
    }

}