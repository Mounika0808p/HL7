namespace HL7.Helper
{
    public static class HL7ParserHelper
    {
        public static Dictionary<string, string> ParseHL7Message(string hl7Message)
        {
            var lines = hl7Message.Split("\n");
            var parsedData = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var segments = line.Split('|');
                if (segments.Length > 0)
                {
                    parsedData[segments[0]] = line.Trim();
                }
            }
            return parsedData;
        }
    }
}
