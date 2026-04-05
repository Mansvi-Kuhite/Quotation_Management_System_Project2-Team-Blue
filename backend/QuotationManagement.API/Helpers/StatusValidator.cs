namespace QuotationManagement.API.Helpers
{
    public static class StatusValidator
    {
        public static bool IsValidTransition(string current, string next)
        {
            return current switch
            {
                "Draft" => next == "Sent",
                "Sent" => next == "Viewed" || next == "Expired",
                "Viewed" => next == "Accepted" || next == "Rejected",
                _ => false
            };
        }

        public static bool IsEditable(string status)
            => status == "Draft";
    }
}