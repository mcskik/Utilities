namespace Git8.Models
{
    /// <summary>
    /// Translator class.
    /// </summary>
    /// <remarks>
    /// This class contains all the methods to translate commands to and from command templates.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class Translator
    {
        private const string CHECKOUT_BRANCH_TOKEN = @"{CheckoutBranch}";
        private const string REMOTE_BRANCH_TOKEN = @"{RemoteBranch}";
        private const string LOCAL_BRANCH_TOKEN = @"{LocalBranch}";
        private const string FILESPEC_TOKEN = @"{FileSpec}";
        private const string HEAD_TOKEN = @"{Head}";
        private const string HEAD_PREFIX = @"HEAD~";
        private const string SHA_TOKEN = @"{Sha}";
        private const string COMMENT_TOKEN = @"{Comment}";
        private const string STASH_TOKEN = @"{Stash}";

        public static string TranslateFromTokens(string template, string checkoutBranchText, string remoteBranchText, string localBranchText, string fileSpecText, string headText, string shaText, string commentText, string stashText)
        {
            string result = template;
            result = TranslateFromToken(result, CHECKOUT_BRANCH_TOKEN, checkoutBranchText);
            result = TranslateFromToken(result, REMOTE_BRANCH_TOKEN, remoteBranchText);
            result = TranslateFromToken(result, LOCAL_BRANCH_TOKEN, localBranchText);
            result = TranslateFromToken(result, FILESPEC_TOKEN, fileSpecText);
            result = TranslateFromToken(result, HEAD_TOKEN, HEAD_PREFIX + headText.Trim());
            result = TranslateFromToken(result, SHA_TOKEN, shaText);
            result = TranslateFromToken(result, COMMENT_TOKEN, commentText);
            result = TranslateFromToken(result, STASH_TOKEN, stashText);
            return result;
        }

        private static string TranslateFromToken(string original, string token, string actual)
        {
            string result = original;
            actual = actual.Trim();
            if (actual.Length > 0)
            {
                result = original.Replace(token, actual);
            }
            return result;
        }

        public static string TranslateToTokens(string command, string checkoutBranchText, string remoteBranchText, string localBranchText, string fileSpecText, string headText, string shaText, string commentText, string stashText)
        {
            string result = command;
            result = TranslateToToken(result, checkoutBranchText, CHECKOUT_BRANCH_TOKEN);
            result = TranslateToToken(result, remoteBranchText, REMOTE_BRANCH_TOKEN);
            result = TranslateToToken(result, localBranchText, LOCAL_BRANCH_TOKEN);
            result = TranslateToToken(result, fileSpecText, FILESPEC_TOKEN);
            result = TranslateToToken(result, HEAD_PREFIX + headText.Trim(), HEAD_TOKEN);
            result = TranslateToToken(result, shaText, SHA_TOKEN);
            result = TranslateToToken(result, commentText, COMMENT_TOKEN);
            result = TranslateToToken(result, stashText, STASH_TOKEN);
            return result;
        }

        private static string TranslateToToken(string original, string actual, string token)
        {
            string result = original;
            actual = actual.Trim();
            if (actual.Length > 0)
            {
                result = original.Replace(actual, token);
            }
            return result;
        }
    }
}