namespace Assignment_Endpoints.Services
{
    public class DiffService
    {
        // This function is asynchronous, just to show that I know how to do it. It's not really needed since GetDiff function isn't particularly heavy
        public async Task<Dictionary<string, object>> GetDiffAsync(string left, string right)
        {
            return await Task.Run(() => GetDiff(left, right));
        }

        public Dictionary<string, object> GetDiff(string left, string right)
        {
            var result = new Dictionary<string, object>();

            // First case; when inputs are completely equal
            if (left == right)
            {
                result["Identical"] = true;
                return result;
            }

            // Second case; when inputs are of different sizes
            if (left.Length != right.Length)
            {
                result["EqualSize"] = false;
                return result;
            }

            // Third case; when inputs are of same size, but different in content
            var diffs = new List<Dictionary<string, object>>();
            // Stores the first index of a series of differences
            int? diffStartIndex = null;

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    // Check if it's the first difference or an ongoing difference
                    if (!diffStartIndex.HasValue)
                    {
                        diffStartIndex = i;
                    }
                }
                // if chars at current index are same for both inputs
                else
                {
                    // If there's an ongoing difference, add it to the final list of diffs.
                    if (diffStartIndex.HasValue)
                    {
                        diffs.Add(new Dictionary<string, object> {
                            { "Offset", diffStartIndex.Value },
                            { "Length", i - diffStartIndex.Value }
                        });
                        // Reset the starting index to be able to find the next difference.
                        diffStartIndex = null;
                    }
                }
            }

            // "Special case", if the ending of two values was different. Hence, not yet added to "diffs" in the earlier loop.
            if (diffStartIndex.HasValue)
            {
                diffs.Add(new Dictionary<string, object> {
                    { "Offset", diffStartIndex.Value },
                    { "Length", left.Length - diffStartIndex.Value }
                });
            }

            result["EqualSize"] = true;
            result["Differences"] = diffs;

            return result;
        }
    }
}
