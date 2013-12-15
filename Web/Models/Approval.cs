using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Models
{
    public class Approval
    {
        public virtual int Id { get; set; }

        public virtual MediaCommUser ApprovedBy { get; set; }

        public virtual string ApprovedUrl { get; set; }
    }
}