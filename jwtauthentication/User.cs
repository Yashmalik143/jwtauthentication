using Microsoft.AspNetCore.SignalR;

namespace jwtauthentication
{
    public class User
    {
        public int userId { get; set; }
        public string UserName { get; set; }=string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

    }
}
