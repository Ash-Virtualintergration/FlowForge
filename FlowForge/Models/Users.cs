namespace FlowForge.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }   // plain text for now (you asked to remove hashing)
        public bool IsAdmin { get; set; }
    }
}
