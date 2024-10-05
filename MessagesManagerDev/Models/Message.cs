using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MessagesManagerDev.Models
{
    public partial class Message
    {
        [JsonPropertyName("message_id")]
        public Guid MessageId { get; set; }
        [JsonPropertyName("chat_id")]

        public Guid? ChatId { get; set; }
        [JsonPropertyName("sender_id")]

        public Guid? SenderId { get; set; }
        [JsonPropertyName("content")]

        public string? Content { get; set; }
        [JsonPropertyName("timestamp")]

        public DateTime? Timestamp { get; set; }

        public virtual Chat? Chat { get; set; }

        public virtual User? Sender { get; set; }
    }
}