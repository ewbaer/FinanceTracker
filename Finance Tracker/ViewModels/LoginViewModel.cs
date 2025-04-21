using System.IO;
using System.Text.Json;
using FinanceTracker.Models;

namespace FinanceTracker.ViewModels
{
    public class LoginViewModel
    {
        private const string UserFile = "Data/user.json";

        public bool Authenticate(string username, string password)
        {
            if (!File.Exists(UserFile))
                return false;

            var user = JsonSerializer.Deserialize<User>(File.ReadAllText(UserFile));
            return user != null && user.Username == username && user.Password == password;
        }
    }
}