using System.Diagnostics;

namespace Quiz.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid RoleId { get; set; }

        public Role Role { get; set; }
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public ICollection<Attempt> Attemts { get; set; } = new List<Attempt>();
    }
}
