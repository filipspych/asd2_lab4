namespace ASD
{
    using ASD.Graphs;
    using System;
    using System.Collections.Generic;

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

        List<int> _QuarantineTargets(List<int> Z, Graph g, bool goBack)
        {
            bool[] infected = new bool[g.VerticesCount];
            double[] infectionTime = new double[g.VerticesCount];
            foreach (var v in Z)
            {
                infected[v] = true;
            }
            
            List<Edge> pQ = new List<Edge>();
            for (int i = 0; i < g.VerticesCount; i++)
            {
                if (infected[i])
                {
                    foreach (Edge e in g.OutEdges(i)) pQ.Add(e);
                } else
                {
                    foreach (Edge e in g.OutEdges(i))
                    {
                        if ((goBack || e.Weight != 0) && !infected[e.To])
                        {
                            pQ.Add(e);
                        }
                    }
                    
                }               
            }
                
            pQ.Sort((Edge e1, Edge e2) => { return Math.Sign(e1.Weight - e2.Weight); });
            if (goBack) pQ.Reverse();

            foreach (Edge e in pQ)
            {
                if (infected[e.To] != infected[e.From])
                {
                    if (infected[e.To] && (infectionTime[e.To] != e.Weight))
                    {
                        infected[e.From] = true;
                        infectionTime[e.From] = e.Weight;
                    }
                    else if (infectionTime[e.From] != e.Weight)
                    {
                        infected[e.To] = true;
                        infectionTime[e.To] = e.Weight;
                    }
                }
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
                abc
            }

            return new List<int>(prev);
        }
    }

}
