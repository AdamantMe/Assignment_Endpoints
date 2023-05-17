namespace Assignment_Endpoints.Services
{
    public class EncodingService
    {
        public string DecodeBase64(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid base64 string.");
            }
        }
    }
}
