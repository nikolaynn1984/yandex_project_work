using Account.Application.Abstractions.Services;
using System.Security.Cryptography;
using System.Text;

namespace Account.Application;

public class PasswordHashing : IPasswordHashing
{
    public string Execure(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
