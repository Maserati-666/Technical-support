using Technical_support.Models;

namespace Technical_support.ViewModel
{
    public class RequestDetail
    {
        public Request Request { get; set; }

        public IEnumerable<Response>? ListResponses { get; set; }
        public Response Response { get; set; }
    }
}
