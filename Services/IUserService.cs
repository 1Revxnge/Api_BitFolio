namespace ApiJobfy.Services
{
    public interface IUserService
    {
        Task<bool> ExistsByEmailAsync(string email);
    }
}
