using System;
using System.Collections.Generic;

namespace BowlingPoints
{
    internal class ScoreCalculator
    {
        List<List<int>> points; //the bowling points from which the whole calculation starts
        public List<int> scores { get; internal set; } //the end product, to be returned.
        private List<int> frameScores; //The bonus-less scores of each "turn" in the bowling game.
        private List<int> boni; //The bonus applied to each "turn", dependant on future turns.

        public ScoreCalculator(List<List<int>> points)
        {
            this.points = points;
            frameScores = new List<int>(); //a frame, is synonymous with a "turn" in bowling.
            boni = new List<int>();
            frameScoreCal(); //call to calculate the frame scores.
            boniCal();
        }

        private void frameScoreCal() //calculates what score the player has each turn, no bonus applied.
        {
            foreach(List<int> l in points) //each "turn" is a list of pins that got knocked down. like {4,6}
            {
                int n = 0; //this frames score starts at 0
                foreach (int i in l) //there are (mostly) 2 throws in one turn/frame, sometimes 3 throws.
                {
                    n += i; //each throw adds its value to the
                }
                frameScores.Add(n); //this score value is added to the empty list of scores.
            }
            //Now, all the scores are calculated, and each INT score corresponds to a List<INT> turn.

            //but the last turn may mean trouble..
            if (frameScores.Count>9 && frameScores[9] > 10) //a strike or a spare yields 10 points max!
            {
                //therefore, if there are 10 frames, the last turn MAY look like it scores more than 10 points.
                frameScores[9] = 10; //but the score is MAX 10, and the other 1-2 throws are added as "bonus" later.
            }
        }

        private void boniCal()
        {
            
        }
    }
}