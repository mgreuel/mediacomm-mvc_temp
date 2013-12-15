using System.Linq;
using System.Web.Mvc;

using MediaCommMvc.Web.Infrastructure;
using MediaCommMvc.Web.Infrastructure.DataInterfaces;
using MediaCommMvc.Web.Infrastructure.Nh;
using MediaCommMvc.Web.Models;
using MediaCommMvc.Web.Models.Users;
using MediaCommMvc.Web.ViewModels;

namespace MediaCommMvc.Web.Controllers
{
    public partial class ApprovalsController : Controller
    {
        private readonly IApprovalsRepository approvalsRepository;

        private readonly CurrentUserContainer currentUserContainer;

        public ApprovalsController(IApprovalsRepository approvalsRepository, CurrentUserContainer currentUserContainer)
        {
            this.approvalsRepository = approvalsRepository;
            this.currentUserContainer = currentUserContainer;
        }

        private MediaCommUser MediaCommUser
        {
            get
            {
                return this.currentUserContainer.User;
            }
        }

        [HttpPost]
        [SessionActionFilter]
        public void Approve(string approvedUrl)
        {
            if (string.IsNullOrWhiteSpace(approvedUrl))
            {
                return;
            }

            this.approvalsRepository.AddAproval(new Approval { ApprovedBy = this.MediaCommUser, ApprovedUrl = approvedUrl });
        }

        [HttpPost]
        [SessionActionFilter]
        public virtual ActionResult GetApprovalsForUrls(string[] approvalUrls)
        {
            ApprovalViewModel[] approvals =
                this.approvalsRepository.GetApprovalsForUrls(approvalUrls)
                        .Select(a => new ApprovalViewModel { Url = a.ApprovedUrl, ApprovedByUsername = a.ApprovedBy.UserName })
                        .ToArray();

            return this.Json(approvals, JsonRequestBehavior.AllowGet);
        }
    }
}
