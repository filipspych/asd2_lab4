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
            return _QuarantineTargets(Z, g, false);
        }

        bool isRelevant(int to, double weight, double previousFrameTime, bool goBack, bool[] infected, bool[] newlyInfected)
        {
            if (goBack)
            {
                if (previousFrameTime <= weight) return false;
            } else
            {
                if (previousFrameTime >= weight) return false; 
            }

            return !infected[to] && !newlyInfected[to];
        }

        // moves newly infected to the infected and adds all new relevant edges to the queue (only edges from newly infected to (not infected and not newly infected), 
        // only edges lesser/greater than previousTimeFrame)
        void changeFrame(bool[] infected, bool[] newlyInfected, EdgesPriorityQueue edges, double previousFrameTime, Graph g, bool goBack)
        {
            int l = newlyInfected.Length;
            for (int i = 0; i < l; i++)
            {
                if (newlyInfected[i])
                {
                    foreach (Edge e in g.OutEdges(i)) if (isRelevant(e.To, e.Weight, previousFrameTime, goBack, infected, newlyInfected))
                    {
                        edges.Put(e);
                    }
                    // todo mozliwe usprawnienie - uzyc setow (contains ma O(1))
                    newlyInfected[i] = false;
                    infected[i] = true;
                }
            }
        }

        List<int> _QuarantineTargets(List<int> Z, Graph g, bool goBack)
        {
            bool[] infected = new bool[g.VerticesCount];
            bool[] newlyInfected = new bool[g.VerticesCount];
            EdgesPriorityQueue pQ;
            if (goBack) pQ = new EdgesMaxPriorityQueue();
            else pQ = new EdgesMinPriorityQueue();

            foreach (var v in Z)
            {
                newlyInfected[v] = true;
            }

            changeFrame(infected, newlyInfected, pQ, (goBack)?double.MaxValue:double.MinValue, g, goBack);

            if (pQ.Empty) return Z;

            double infectionFrame = pQ.Peek().Weight;
            while (!pQ.Empty)
            {
                Edge e = pQ.Get();
                if (infectionFrame != e.Weight)
                {
                    changeFrame(infected, newlyInfected, pQ, infectionFrame, g, goBack);
                    infectionFrame = e.Weight;
                }

                newlyInfected[e.To] = true;
                if (pQ.Empty) changeFrame(infected, newlyInfected, pQ, infectionFrame, g, goBack);
            }


            List<int> ret = new List<int>();
            for (int i = 0; i < g.VerticesCount; i++)
                if (infected[i]) ret.Add(i);

            return ret;
        }

        /// <summary>
        /// Część II: wyznaczenie zbioru potencjalnych pacjentów zero.
        /// </summary>
        /// <param name="S">Zbiór osób zakażonych przez potencjalnego pacjenta zero</param>
        /// <param name="G">Informacja o spotkaniach; wagi krawędzi są nieujemne i oznaczają czas spotkania</param>
        /// <returns>Lista potencjalnych pacjentów zero, uporządkowana rosnąco</returns>
        public List<int> PotentialPatientsZero(List<int> S, Graph g)
        {
            List<HashSet<int>> potentialZeros = new List<HashSet<int>>();

            foreach(int v in S)
            {
                List<int> z = new List<int>();
                z.Add(v);
                List<int> targets = _QuarantineTargets(z, g, true);
                var a = new HashSet<int>(targets);
                potentialZeros.Add(a);
            }

            if (potentialZeros.Count == 0) return new List<int>();

            HashSet<int> prev = new HashSet<int>(potentialZeros[0]);
            foreach (var s in potentialZeros)
            {
                prev.IntersectWith(s);
            }

            return new List<int>(prev);
        }
    }

}
