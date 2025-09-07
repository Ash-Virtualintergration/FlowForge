namespace FlowForge.Models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Store hash instead of plain text
    }
}
