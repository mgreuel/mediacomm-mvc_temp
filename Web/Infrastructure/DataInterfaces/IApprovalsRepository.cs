using System.Collections.Generic;

using MediaCommMvc.Web.Models;

namespace MediaCommMvc.Web.Infrastructure.DataInterfaces
{
    public interface IApprovalsRepository
    {
        void AddAproval(Approval approval);

        Approval[] GetApprovalsForUrls(string[] approvalUrls);

        Approval[] GetNewestApprovals(int count);

        IEnumerable<KeyValuePair<string, int>> GetUrlsWithMostApprovals(int count);
    }
}