namespace MediaCommMvc.Web.Infrastructure
{
    public interface IConfigAccessor
    {
        string GetConfigValue(string key);
    }
}