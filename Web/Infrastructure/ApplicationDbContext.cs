using MediaCommMvc.Web.Models;

using Microsoft.AspNet.Identity.EntityFramework;

namespace MediaCommMvc.Web.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}