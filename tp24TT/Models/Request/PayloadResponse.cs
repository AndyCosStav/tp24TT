namespace tp24TT.Models.Request
{
    public class PayloadResponse
    {
        public ReceivableRequest request { get; set; }

        public List<string> Errors { get; set; }
    }
}
