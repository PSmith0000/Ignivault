namespace ignivault.Core.Interface
{
    public interface IModalDialog<T>
    {
        Task SetVaultItem(T item);
        Task ShowAsync(); 
    }
}
