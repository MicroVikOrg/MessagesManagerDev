using Microsoft.EntityFrameworkCore;
namespace MessagesManagerDev.Models
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
        }
        public DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("chats_pkey");

                entity.ToTable("chats");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Chatname)
                    .HasMaxLength(100)
                    .HasColumnName("chatname");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");

                entity.HasMany(d => d.Users).WithMany(p => p.Chats)
                    .UsingEntity<Dictionary<string, object>>(
                        "ChatMember",
                        r => r.HasOne<User>().WithMany()
                            .HasForeignKey("UserId")
                            .HasConstraintName("chat_members_user_id_fkey"),
                        l => l.HasOne<Chat>().WithMany()
                            .HasForeignKey("ChatId")
                            .HasConstraintName("chat_members_chat_id_fkey"),
                        j =>
                        {
                            j.HasKey("ChatId", "UserId").HasName("chat_members_pkey");
                            j.ToTable("chat_members");
                            j.IndexerProperty<Guid>("ChatId").HasColumnName("chat_id");
                            j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                        });
            });
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.MessageId).HasName("messages_pkey");

                entity.ToTable("messages");

                entity.Property(e => e.MessageId)
                    .ValueGeneratedNever()
                    .HasColumnName("message_id");
                entity.Property(e => e.ChatId).HasColumnName("chat_id");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.SenderId).HasColumnName("sender_id");
                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("timestamp");

                entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("messages_chat_id_fkey");

                entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("messages_sender_id_fkey");
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");

                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

                entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");
                entity.Property(e => e.Password)
                    .HasMaxLength(64)
                    .HasColumnName("password");
                entity.Property(e => e.Token).HasColumnName("token");
                entity.Property(e => e.Username)
                    .HasMaxLength(64)
                    .HasColumnName("username");
                entity.Property(e => e.Verified).HasColumnName("verified");
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
