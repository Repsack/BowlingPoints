using System;
using System.Collections.Generic;

namespace BowlingPoints
{
    internal class ScoreCalculator
    {
        BowlingPointsData bpd;
        public List<int> scores { get; internal set; }
        private List<int> frameScores, boni;

        public ScoreCalculator(BowlingPointsData bopoda)
        {
            bpd = bopoda;
            frameScores = new List<int>();
            boni = new List<int>();
            frames();
            scores = frameScores;
        }

        private void frames()
        {
            foreach(List<int> l in bpd.points)
            {
                int n = 0;
                foreach (int i in l)
                {
                    n += i;
                }
                frameScores.Add(n);
            }
        }
    }
}