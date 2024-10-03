namespace MessagesManagerDev.Models
{
    public partial class Chat
    {
        public Guid Id { get; set; }

        public string Chatname { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}