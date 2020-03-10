using System;
using System.Collections.Generic;

namespace Same8.Models
{
    /// <summary>
    /// Fuzzy matcher class.
    /// </summary>
    /// <remarks>
    /// Contains fuzzy matcher routines.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class Matcher
    {
        public double FuzzyFinalThresholdScore { get; set; }
        public double FuzzySignificantThresholdScore { get; set; }
        public double FuzzyExcellentThresholdScore { get; set; }
        public string FuzzyWordDelimiter { get; set; }
        public List<string> OldRows { get; set; }
        public List<string> NewRows { get; set; }
        public MatchingPairs OldMatches { get; set; }
        public MatchingPairs NewMatches { get; set; }

        public Matcher()
        {
            FuzzyFinalThresholdScore = 0.6;
            FuzzySignificantThresholdScore = 0.5;
            FuzzyExcellentThresholdScore = 0.9;
            FuzzyWordDelimiter = "ANY";
            OldRows = new List<string>();
            NewRows = new List<string>();
            OldMatches = new MatchingPairs();
            NewMatches = new MatchingPairs();
        }

        public Matcher(double fuzzyFinalThresholdScore, double fuzzySignificantThresholdScore, double fuzzyExcellentThresholdScore, string fuzzyWordDelimiter)
        {
            FuzzyFinalThresholdScore = fuzzyFinalThresholdScore;
            FuzzySignificantThresholdScore = fuzzySignificantThresholdScore;
            FuzzyExcellentThresholdScore = fuzzyExcellentThresholdScore;
            FuzzyWordDelimiter = fuzzyWordDelimiter;
            OldRows = new List<string>();
            NewRows = new List<string>();
            OldMatches = new MatchingPairs();
            NewMatches = new MatchingPairs();
        }

        /// <summary>
        /// Test.
        /// </summary>
        public void Test()
        {
            NewRows = new List<string>();
            NewRows.Add("FareportalPiranhaCheapoCA_YYZ_JFK_A1_C1_I1_S0_Economy_Return_20121001_114011.xml");
            NewRows.Add("FareportalPiranhaCheapoCA_YYZ_YVR_A1_C0_I0_S0_Economy_Return_20121001_114012.xml");
            NewRows.Add("FareportalPiranhaCheapoCA_YYZ_YYC_A1_C0_I0_S0_Economy_Return_20121001_114010.xml");
            NewRows.Add("FareportalPiranhaCheapoUS_JFK_LAX_A1_C0_I0_S0_Economy_Return_20121001_113932.xml");
            NewRows.Add("FareportalPiranhaCheapoUS_JFK_LHR_A1_C0_I0_S0_Economy_Return_20121001_113934.xml");
            NewRows.Add("FareportalPiranhaCheapoUS_JFK_MCO_A1_C1_I1_S0_Economy_Return_20121001_113933.xml");
            OldRows = new List<string>();
            OldRows.Add("CheapoPiranha_JFK_LAX_A1_C0_I0_S0_Economy_Return_20121001_113425.xml");
            OldRows.Add("CheapoPiranha_JFK_LHR_A1_C0_I0_S0_Economy_Return_20121001_113428.xml");
            OldRows.Add("CheapoPiranha_JFK_MCO_A1_C1_I1_S0_Economy_Return_20121001_113426.xml");
            OldRows.Add("CheapoPiranha_YYZ_JFK_A1_C1_I1_S0_Economy_Return_20121001_113645.xml");
            OldRows.Add("CheapoPiranha_YYZ_YVR_A1_C0_I0_S0_Economy_Return_20121001_113656.xml");
            OldRows.Add("CheapoPiranha_YYZ_YYC_A1_C0_I0_S0_Economy_Return_20121001_113631.xml");
            FuzzyMatch();
        }

        /// <summary>
        /// Fuzzy match.
        /// </summary>
        public void FuzzyMatch()
        {
            string oldKey = string.Empty;
            int oldRow = 0;
            OldMatches = new MatchingPairs();
            NewMatches = new MatchingPairs();
            for (oldRow = 0; oldRow < OldRows.Count; oldRow++)
            {
                oldKey = OldRows[oldRow];
                if (oldKey.Trim() != string.Empty)
                {
                    BestMatch(oldKey);
                }
            }
        }

        /// <summary>
        /// Find the best fuzzy match for the specified old key.
        /// </summary>
        /// <remarks>
        /// The old key is the key on the left hand side.
        /// </remarks>
        /// <param name="oldKey">Old key to fuzzy match.</param>
        /// <returns>Score for the best fuzzy match for the specified old key.</returns>
        public void BestMatch(string oldKey)
        {
            double bestScore = 0;
            string bestMatch = string.Empty;
            double thisScore = 0;
            MatchingPair oldMatch;
            MatchingPair newMatch;
            string newKey = string.Empty;
            int newRow = 0;
            bool matched = false;
            Match match = new Match();
            for (newRow = 0; newRow < NewRows.Count; newRow++)
            {
                newKey = NewRows[newRow];
                if (newKey.Trim() != string.Empty)
                {
                    thisScore = match.MatchFields(oldKey, newKey);
                    if (thisScore > bestScore)
                    {
                        bestScore = thisScore;
                        bestMatch = newKey;
                    }
                    if (thisScore == 100)
                    {
                        break;
                    }
                }
            }
            if (bestScore >= FuzzyFinalThresholdScore)
            {
                matched = true;
            }
            else
            {
                matched = false;
            }
            oldMatch = new MatchingPair();
            oldMatch.OldKey = oldKey;
            oldMatch.NewKey = bestMatch;
            oldMatch.Score = bestScore;
            oldMatch.Matched = matched;
            try
            {
                if (NewMatches.ContainsKey(bestMatch))
                {
                    newMatch = NewMatches[bestMatch];
                    if (!(newMatch == null))
                    {
                        if (oldMatch.Score > newMatch.Score)
                        {
                            OldMatches.Add(oldKey, oldMatch);
                            NewMatches.Remove(bestMatch);
                            NewMatches.Add(bestMatch, oldMatch);
                        }
                    }
                    else
                    {
                        OldMatches.Add(oldKey, oldMatch);
                        NewMatches.Add(bestMatch, oldMatch);
                    }
                }
                else
                {
                    OldMatches.Add(oldKey, oldMatch);
                    NewMatches.Add(bestMatch, oldMatch);
                }
            }
            catch (Exception ex)
            {
                OldMatches.Add(oldKey, oldMatch);
                NewMatches.Add(bestMatch, oldMatch);
            }
        }
    }
}