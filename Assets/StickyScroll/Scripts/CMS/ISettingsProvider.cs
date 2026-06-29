namespace Game.CMS
{
    public interface ISettingsProvider
    {
        public T GetData<T>(string id);
    }

}