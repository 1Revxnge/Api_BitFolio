using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Services.IService
{
    public interface IUserService
    {
        [ExcludeFromCodeCoverage]

        Task<bool> ExistsByEmailAsync(string email);
    }
}
