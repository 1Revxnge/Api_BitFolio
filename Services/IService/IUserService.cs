namespace ApiJobfy.Services.IService
{
    public interface IUserService
    {
        Task<bool> ExistsByEmailAsync(string email);
    }
}
