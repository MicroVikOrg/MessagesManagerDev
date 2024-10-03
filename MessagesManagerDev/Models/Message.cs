namespace MessagesManagerDev.Models
{
    public partial class Message
    {
        public Guid MessageId { get; set; }

        public Guid? ChatId { get; set; }

        public Guid? SenderId { get; set; }

        public string? Content { get; set; }

        public DateTime? Timestamp { get; set; }

        public virtual Chat? Chat { get; set; }

        public virtual User? Sender { get; set; }
    }
}