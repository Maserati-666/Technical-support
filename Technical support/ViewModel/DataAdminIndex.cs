using Technical_support.Models;

namespace Technical_support.ViewModel
{
    public class DataAdminIndex
    {
        
        public VidRequest ListVidRequest { get; set; }

        public TypeUser ListTypeUser { get; set; }
    }

    public class VidRequest
    {
        public virtual int RequestOpen { get; set; }
        public virtual int RequestProg { get; set; }
        public virtual int RequestClose { get; set; }
        public virtual int RequestAnnul { get; set; }
    }

    public class TypeUser
    {
        public virtual int Client { get; set; }
        public virtual int Manager { get; set; }
        public virtual int Admin { get; set; }
    }


}
