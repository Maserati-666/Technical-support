using Microsoft.AspNetCore.Identity;
using Technical_support.Models;

namespace Technical_support.ViewModel
{
    public class RequestCloseInfo
    {
        public IEnumerable<ListRequest> RequestAccept { get; set; }
        public ListRequest ListRequest { get; set; }

        public IEnumerable<Response>? ListResponses { get; set; }
        public Response Response { get; set; }
    }
}
