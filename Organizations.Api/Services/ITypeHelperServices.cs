namespace Organizations.Api.Services
{
    public interface ITypeHelperServices
    {
        bool TypeHasProperties<T>(string fields);
    }
}