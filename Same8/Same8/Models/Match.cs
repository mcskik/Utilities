namespace Same8.Models
{
    /// <summary>
    /// Fuzzy match class.
    /// </summary>
    /// <remarks>
    /// Contains fuzzy match routines.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class Match
    {
        const string SPECIAL = @" ¦`¬!""£$%^&*()-_=+[]{};:'@#~\|,.<>/?";
        public double FuzzySignificantThresholdScore { get; set; }
        public double FuzzyExcellentThresholdScore { get; set; }
        public string FuzzyWordDelimiter { get; set; }

        public Match()
        {
            FuzzySignificantThresholdScore = 0.55;
            FuzzyExcellentThresholdScore = 0.9;
            FuzzyWordDelimiter = "ANY";
        }

        public Match(double fuzzySignificantThresholdScore, double fuzzyExcellentThresholdScore, string fuzzyWordDelimiter)
        {
            FuzzySignificantThresholdScore = fuzzySignificantThresholdScore;
            FuzzyExcellentThresholdScore = fuzzyExcellentThresholdScore;
            FuzzyWordDelimiter = fuzzyWordDelimiter;
        }

        /// <summary>
        /// Test.
        /// </summary>
        public void Test()
        {
            string field1 = string.Empty;
            string field2 = string.Empty;
            double matchCount = 0;
            field1 = "Water dehydration";
            field2 = "Dehydration";
            matchCount = MatchFields(field1, field2);
            field1 = "Snow cooling";
            field2 = "Cooling";
            matchCount = MatchFields(field1, field2);
            field1 = "Oven cooling";
            field2 = "Oven Cooling";
            matchCount = MatchFields(field1, field2);
            field1 = "No.jacket pockets";
            field2 = "Number of pockets";
            matchCount = MatchFields(field1, field2);
            field1 = "No.jacket linings";
            field2 = "Number of linings";
            matchCount = MatchFields(field1, field2);
            field1 = "No.jacket trousers";
            field2 = "Jacket input report";
            matchCount = MatchFields(field1, field2);
            field1 = "No. trouser legs";
            field2 = "Number of legs";
            matchCount = MatchFields(field1, field2);
            field1 = "Glue solv.";
            field2 = "Glue solvent";
            matchCount = MatchFields(field1, field2);
        }

        /// <summary>
        /// Return a count of words in the input string.
        /// </summary>
        /// <param name="textString">Input string.</param>
        /// <param name="textDelimiter">Used to determine where words begin and end.</param>
        /// <returns>Count of words in the input string.</returns>
        public int WordCount(string textString, string textDelimiter)
        {
            string special = string.Empty;
            string delimiter = string.Empty;
            string text = string.Empty;
            string word = string.Empty;
            string letter = string.Empty;
            int pos = 0;
            int len = 0;
            int count = 0;
            special = SPECIAL;
            text = textString.Trim() + new string(' ', 2);
            if (text.CompareTo(new string(' ', 2)) <= 0)
            {
                return 0;
            }
            switch (textDelimiter.ToUpper())
            {
                case "SPACE":
                    delimiter = new string(' ', 1);
                    break;
                case "SPACES":
                    delimiter = new string(' ', 1);
                    break;
                case "SPECIAL":
                    delimiter = special;
                    break;
                case "ANY":
                    delimiter = special;
                    break;
                case "":
                    delimiter = special;
                    break;
                default:
                    delimiter = textDelimiter;
                    break;
            }
            count = 0;
            pos = 0;
            len = text.Length;
            letter = text.Substring(pos, 1);
            do
            {
                word = string.Empty;
                while (pos < len - 1 && delimiter.IndexOf(letter) != -1)
                {
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                while (pos < len - 1 && delimiter.IndexOf(letter) == -1)
                {
                    word = word + letter;
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                if (word.CompareTo(string.Empty) > 0)
                {
                    count++;
                }
            } while (word != string.Empty);
            return count;
        }

        /// <summary>
        /// Return a count of the words in the input string and store all words found
        /// in consecutive locations of the output word array.
        /// </summary>
        /// <param name="textString">Input string.</param>
        /// <param name="wordArray">Output word array.</param>
        /// <param name="textDelimiter">Used to determine where words begin and end.</param>
        /// <returns>Count of words in the input string.</returns>
        public int WordArray(string textString, ref string[] wordArray, string textDelimiter)
        {
            string special = string.Empty;
            string delimiter = string.Empty;
            string text = string.Empty;
            string word = string.Empty;
            string letter = string.Empty;
            int pos = 0;
            int len = 0;
            int count = 0;
            special = SPECIAL;
            text = textString.Trim() + new string(' ', 2);
            if (text.CompareTo(new string(' ', 2)) <= 0)
            {
                return 0;
            }
            switch (textDelimiter.ToUpper())
            {
                case "SPACE":
                    delimiter = new string(' ', 1);
                    break;
                case "SPACES":
                    delimiter = new string(' ', 1);
                    break;
                case "SPECIAL":
                    delimiter = special;
                    break;
                case "ANY":
                    delimiter = special;
                    break;
                case "":
                    delimiter = special;
                    break;
                default:
                    delimiter = textDelimiter;
                    break;
            }
            count = 0;
            pos = 0;
            len = text.Length;
            letter = text.Substring(pos, 1);
            do
            {
                word = string.Empty;
                while (pos < len - 1 && delimiter.IndexOf(letter) != -1)
                {
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                while (pos < len - 1 && delimiter.IndexOf(letter) == -1)
                {
                    word = word + letter;
                    pos++;
                    letter = text.Substring(pos, 1);
                }
                if (word.CompareTo(string.Empty) > 0)
                {
                    count++;
                    word = LookupAbbreviation(word);
                    wordArray[count - 1] = word;
                }
            } while (word != string.Empty);
            return count;
        }

        /// <summary>
        /// Return a real number representing how well the two input words match.
        /// The return value will be between 0 and 1.
        /// </summary>
        /// <param name="word1">First word.</param>
        /// <param name="word2">Second word.</param>
        /// <returns>Number representing how well the two input words match.</returns>
        public double MatchWords(string word1, string word2)
        {
            if (word1.CompareTo(word2) == 0)
            {
                return 1;
            }
            string smallWord = string.Empty;
            string smallChar = string.Empty;
            int smallFst = 0;
            int smallPtr = 0;
            int smallLen = 0;
            string largeWord = string.Empty;
            string largeChar = string.Empty;
            int largeFst = 0;
            int largePtr = 0;
            int largeLen = 0;
            string currentChain = string.Empty;
            string longestChain = string.Empty;
            int longestChainLen = 0;
            if (word1.Length > word2.Length)
            {
                smallWord = word2;
                largeWord = word1;
            }
            else
            {
                smallWord = word1;
                largeWord = word2;
            }
            longestChain = string.Empty;
            longestChainLen = longestChain.Length;
            smallLen = smallWord.Length;
            largeLen = largeWord.Length;
            for (smallFst = 0; smallFst < smallLen; smallFst++)
            {
                currentChain = string.Empty;
                largeFst = 0;
                for (smallPtr = smallFst; smallPtr < smallLen; smallPtr++)
                {
                    smallChar = smallWord.Substring(smallPtr, 1);
                    for (largePtr = largeFst; largePtr < largeLen; largePtr++)
                    {
                        largeChar = largeWord.Substring(largePtr, 1);
                        if (largeChar == smallChar)
                        {
                            currentChain = currentChain + smallChar;
                            largeFst = largePtr + 1;
                            break;
                        }
                    }
                    if (largeFst > largeLen)
                    {
                        break;
                    }
                }
                if (currentChain.Length > longestChainLen)
                {
                    longestChain = currentChain;
                    longestChainLen = longestChain.Length;
                }
                if (longestChainLen > (smallLen - smallFst))
                {
                    break;
                }
            }
            if (longestChainLen > largeLen)
            {
            }
            return (double)longestChainLen / (double)largeLen;
        }

        /// <summary>
        /// Return a real number representing how well the two input fields match.
        /// The return value will be between 0 and 1 for partial matches
        /// but is 100 for exact matches.
        /// </summary>
        /// <param name="field1">First field.</param>
        /// <param name="field2">Second field.</param>
        /// <returns>Number representing how well the two input fields match</returns>
        public double MatchFields(string field1, string field2)
        {
            field1 = field1.Trim().ToUpper();
            field2 = field2.Trim().ToUpper();
            if (field1.CompareTo(field2) == 0)
            {
                return 100;
            }
            string smallField = string.Empty;
            string smallWord = string.Empty;
            string[] smallArray = new string[50];
            int smallPtr = 0;
            int smallEnd = 0;
            string largeField = string.Empty;
            string largeWord = string.Empty;
            string[] largeArray = new string[50];
            int largePtr = 0;
            int largeEnd = 0;
            double wordScore = 0;
            double fieldScore = 0;
            double highestFieldScore = 0;
            double cumulativeFieldScore = 0;
            double maxFieldScore = 0;
            int totalSmallWordLetters = 0;
            int totalLargeWordLetters = 0;
            int totalNoOfWordLetters = 0;
            if (WordCount(field1, FuzzyWordDelimiter) > WordCount(field2, FuzzyWordDelimiter))
            {
                smallField = field2;
                largeField = field1;
            }
            else
            {
                smallField = field1;
                largeField = field2;
            }
            smallEnd = WordArray(smallField, ref smallArray, FuzzyWordDelimiter);
            largeEnd = WordArray(largeField, ref largeArray, FuzzyWordDelimiter);
            totalSmallWordLetters = 0;
            for (smallPtr = 0; smallPtr < smallEnd; smallPtr++)
            {
                totalSmallWordLetters += smallArray[smallPtr].Length;
            }
            totalLargeWordLetters = 0;
            for (largePtr = 0; largePtr < largeEnd; largePtr++)
            {
                totalLargeWordLetters += largeArray[largePtr].Length;
            }
            if (totalSmallWordLetters < totalLargeWordLetters)
            {
                totalNoOfWordLetters = totalSmallWordLetters;
            }
            else
            {
                totalNoOfWordLetters = totalLargeWordLetters;
            }
            fieldScore = 0;
            highestFieldScore = 0;
            cumulativeFieldScore = 0;
            maxFieldScore = 0;
            for (largePtr = 0; largePtr < largeEnd; largePtr++)
            {
                largeWord = largeArray[largePtr];
                maxFieldScore += (1 * largeWord.Length);
            }
            for (smallPtr = 0; smallPtr < smallEnd; smallPtr++)
            {
                fieldScore = 0;
                highestFieldScore = 0;
                smallWord = smallArray[smallPtr];
                for (largePtr = 0; largePtr < largeEnd; largePtr++)
                {
                    largeWord = largeArray[largePtr];
                    wordScore = MatchWords(smallWord, largeWord);
                    if (wordScore >= FuzzySignificantThresholdScore)
                    {
                        fieldScore += (wordScore * largeWord.Length);
                        if (fieldScore > highestFieldScore)
                        {
                            highestFieldScore = fieldScore;
                        }
                    }
                    if (wordScore > FuzzyExcellentThresholdScore)
                    {
                        break;
                    }
                }
                cumulativeFieldScore += highestFieldScore;
            }
            if (maxFieldScore < 1)
            {
                maxFieldScore = 1;
            }
            return cumulativeFieldScore / maxFieldScore;
            //if (totalNoOfWordLetters < 1)
            //{
            //    totalNoOfWordLetters = 1;
            //}
            //return fieldScore / (double)totalNoOfWordLetters;
        }

        /// <summary>
        /// Lookup abbreviations.
        /// </summary>
        /// <param name="abbreviation">Abbreviation</param>
        /// <returns>Equivalent fuul word</returns>
        private string LookupAbbreviation(string abbreviation)
        {
            string answer = string.Empty;
            abbreviation = abbreviation.Trim().ToUpper();
            switch (abbreviation)
            {
                case "NO":
                    answer = "NUMBER";
                    break;
                default:
                    answer = abbreviation;
                    break;
            }
            return answer;
        }
    }
}