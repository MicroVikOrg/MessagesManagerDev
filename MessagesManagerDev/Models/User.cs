using System;

namespace MessagesManagerDev.Models
{
    public partial class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public bool? Verified { get; set; }

        public string? Token { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

        public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();
    }
}
