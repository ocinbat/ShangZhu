namespace ShangZhu
{
    public interface IConfigius
    {
        string Get(string key);

        T Get<T>(string key);
    }
}
