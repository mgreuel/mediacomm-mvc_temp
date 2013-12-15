namespace MediaCommMvc.Web.ViewModels
{
    public class ApprovalPaneViewModel
    {
        public ApprovalPaneViewModel()
        {
            this.ShowButton = true;
        }

        public bool ShowButton { get; set; }
        public string Url { get; set; }
    }
}