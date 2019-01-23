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
            //Console.WriteLine("Count of points after padding: " + this.points.Count);
            frameScores = new List<int>(); //a frame, is synonymous with a "turn" in bowling.
            boni = new List<int>() {0,0,0,0,0,0,0,0,0,0}; 
            frameScoreCal(); //call to calculate the frame scores.
            boniCal();
            scores = new List<int>();
            for (int i = 0; i < originalCount; i++)
            {
                scores.Add(frameScores[i] + boni[i]);
            }
            for (int i = 1; i < originalCount; i++)
            {
                scores[i] += scores[i - 1];
            }
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
        /*
        boniCal will not function correctly. it has been made under the assumption that the last
        throw can be a list of 3 ints, but the last throw is ONLY 2 long.
        there is an 11th BONUS throw that contains 2 more ints representing the final bonus.
        bonical needs to be reshapen to fit this criteria.
        */
        private void boniCal()
        {
            int strikeA; //represents the bonus score awarded for the next-next throw.
            int strikeB; //Like strikeA, except it is the second case where the next-next throw is 2 turns ahead.
            //There are 2 parts to the bonus calculations. Strikes and spares.
            for (int i = 0; i < points.Count; i++)
            {
                if(points[i][0]+points[i][1] > 9) //its a spare!
                { 
                    //THIS WILL SOMETIMES GO TO FAR!
                    boni[i] += getSpareScore(i); //will assume there IS a spare, and add the correct ballthrow as bonus.
                }
                if (points[i][0] > 9) //its a strike!
                {
                    strikeA = getStrikeScoreA(i); //strikeScore for when next ball is NOT a strike.
                    strikeB = getStrikeScoreB(i); //strikeScore for when next ball IS a strike;
                    if (i < 9) //the first 9 throws are fine like this.
                    {
                        if (points[i + 1][0] > 9) //means the first ball in the next throw was a strike.
                        {
                            boni[i] += strikeB; //the next ball was also a strike, so we pick B.
                        }
                        else
                        {
                            boni[i] += strikeA; //next ball was NOT a strike, we pick A.
                        }
                    }
                    else
                    {
                        boni[i] += strikeA; //it is fine either way, in the last round A = B
                    }
                }
                //HERE, boni should have a collected sum of all the bonuses there are!
            }
        }

        private int getSpareScore(int spareIndex) //finds the score of the "next" ball
        {
            if (spareIndex < 8) //indicates that the spare happended before last round
            {
                return points[spareIndex + 1][0]; //first throw of the next turn 
            }
            else
            {
                if (points[9].Count > 2) // there MAY not be a 3rd throw in the last round
                    return points[spareIndex][2]; //third throw of the last turn
                else
                    return 0; //if no 3rd throw, bonus is 0.
            }
        }

        private int getStrikeScoreA(int strikeIndex) //case B means the nextnext ball is the second ball of the next throw.
        {
            if (strikeIndex < 9) //indicates the strike happended before the last turn.
            {
                return points[strikeIndex + 1][1]; //second ball of next throw;
            }
            else //the strike happended in the very last turn.
            {
                return points[strikeIndex][1]; //!!!!SECOND!!!! ball of THIS throw
                //but WHY the SECOND ball!?!? Strike checks the nextnext ball, while Spare checks the next..
                //should this not mean that after ball [0]=10, the Strike bonus is from ball [2]?
                //well Yes.... except the SpareCheck assumes that ball[2] is where the SPARE bonus comes from!
                //-at least when it is ONLY a spare.. So when it is also a strike, the remaining bonus comes from ball[1].
                //usually.. spare is next ball, strike is nextnext.
                //but here in the last round, spare is nextnext when there is a strike, so the strike will ONLY HERE, deal with next ball.
                //and next ball here is ball[1]
            }
        }

        private int getStrikeScoreB(int strikeIndex) //case B means the nextnext ball is the first ball of the nextnext throw.
        {//this occurs when the "next" ball is ALSO a strike.
            if (strikeIndex < 8) //indicates the strike happended before the second last turn.
            {
                return points[strikeIndex + 2][0]; //first ball of nextnext throw;
            }
            else if (strikeIndex == 8) //indicates the strike happended IN the second last turn.
            {
                return points[strikeIndex + 1][1]; //second ball of next throw;
            }
            else //the strike happended in the very last turn.
            {
                return points[strikeIndex][1]; //!!!!SECOND!!!! ball of THIS throw;
                //but WHY the SECOND ball!?!? Strike checks the nextnext ball, while Spare checks the next..
                //should this not mean that after ball [0]=10, the Strike bonus is from ball [2]?
                //well Yes.... except the SpareCheck assumes that ball[2] is where the SPARE bonus comes from!
                //-at least when it is ONLY a spare.. So when it is also a strike, the remaining bonus comes from ball[1].
                //usually.. spare is next ball, strike is nextnext.
                //but here in the last round, spare is nextnext when there is a strike, so the strike will ONLY HERE, deal with next ball.
                //and next ball here is ball[1]
            }
        }
    }
}