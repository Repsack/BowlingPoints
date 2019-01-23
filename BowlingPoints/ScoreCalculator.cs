using System;
using System.Collections.Generic;

namespace BowlingPoints
{
    internal class ScoreCalculator
    {
        int originalCount;
        List<List<int>> points; //the bowling points from which the whole calculation starts
        public List<int> scores { get; internal set; } //the end product, to be returned.
        private List<int> frameScores; //The bonus-less scores of each "turn" in the bowling game.
        private List<int> boni; //The bonus applied to each "turn", dependant on future turns.

        public ScoreCalculator(List<List<int>> points)
        {
            originalCount = points.Count;
            this.points = points;
            for (int i = originalCount; i < 10; i++)
            {
                this.points.Add(new List<int>() { 0, 0 });
            }
            Console.WriteLine("Count of points after padding: " + this.points.Count);
            frameScores = new List<int>(); //a frame, is synonymous with a "turn" in bowling.
            boni = new List<int>() {0,0,0,0,0,0,0,0,0,0}; 
            frameScoreCal(); //call to calculate the frame scores.
            boniCal();
            scores = frameScores; //WROOOONGGGG!!
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
            int strikeA; //represents the bonus score awarded for the next-next throw.
            int strikeB; //Like strikeA, except it is the second case where the next-next throw is 2 turns ahead.
            //There are 2 parts to the bonus calculations. Strikes and spares.
            for (int i = 0; i < points.Count; i++)
            {
                if(points[i][0]+points[i][1] > 9) //its a spare!
                { 
                    boni[i] += getSpareScore(i); //will assume there IS a spare, and add the correct ballthrow as bonus.
                }
                if (points[i][0] > 9) //its a strike!
                {
                    strikeA = getStrikeScoreA(i);
                    strikeB = getStrikeScoreB(i);
                }
            }
        }

        private int getSpareScore(int spareIndex) //finds the score of the "next" ball
        {
            if (spareIndex < 10) //indicates that the spare happended before last round
            {
                return points[spareIndex + 1][0]; //first throw of the next turn 
            }
            else
            {
                return points[spareIndex][2]; //third throw of the last turn
            }
        }

        private int getStrikeScoreA(int strikeIndex) //case B means the nextnext ball is the second ball of the next throw.
        {
            if (strikeIndex < 9) //indicates the strike happended before the second last turn.
            {
            }
            else if (strikeIndex == 9) //indicates the strike happended IN the second last turn.
            {
            }
            else //the strike happended in the very last turn.
            {
            }
        }

        private int getStrikeScoreB(int i) //case B means the nextnext ball is the first ball of the nextnext throw.
        {
            if (strikeIndex < 9) //indicates the strike happended before the second last turn.
            {
            }
            else if (strikeIndex == 9) //indicates the strike happended IN the second last turn.
            {
            }
            else //the strike happended in the very last turn.
            {
            }
        }
    }
}