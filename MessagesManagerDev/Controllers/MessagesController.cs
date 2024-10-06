using MessagesManagerDev.Models;
using MessagesManagerDev.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Confluent.Kafka;
namespace MessagesManagerDev.Controllers
{
    [Route("api/msg")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IKafkaProducer _kafkaProducer;
        private ApplicationContext db;
        private ISubscriber sub;
        private IConfiguration _configuration;
        public MessagesController(IKafkaProducer kafkaProducer, ApplicationContext context, IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
        {
            _kafkaProducer = kafkaProducer;
            db = context;
            sub = connectionMultiplexer.GetSubscriber();
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] Message message)
        {
            message.MessageId = Guid.NewGuid();
            var sender = await db.Users.Include(e => e.Chats).FirstOrDefaultAsync(e => e.Id == message.SenderId);
            var chat = await db.Chats.Include(e => e.Users).FirstOrDefaultAsync(e => e.Id == message.ChatId);
            if (sender == null || chat == null || sender.Chats.FirstOrDefault(chat) == null) return BadRequest();
            await db.Messages.AddAsync(message);
            await db.SaveChangesAsync();
            var rvalue = new
            {
                method = "publish",
                @params = new
                {
                    channel = message.ChatId.ToString(),
                    data = new
                    {
                        message_id = message.MessageId,
                        sender_id = message.SenderId,
                        chat_id = message.ChatId,
                        content = message.Content
                    }
                }
            };
            
            using (var client = new HttpClient())
            {
                await client.PostAsync("http://centrifugo:8000/api",JsonContent.Create(rvalue));
            }
            
            await sub.PublishAsync(message.ChatId.ToString(), JsonConvert.SerializeObject(rvalue));
            return Ok(rvalue);
        }
        [Route("img")]
        [HttpPost]
        public async Task<IActionResult> SendImg(string chatId, [FromForm] IFormFile file)
        {
            var chat = await db.Chats.FirstOrDefaultAsync(e => e.Id == Guid.Parse(chatId));
            if (chat == null || file == null || file.Length <= 0) return BadRequest();
            var filePath = Path.Combine(_configuration["actualPath"]!, file.FileName);
            using (var filestream = System.IO.File.Create(filePath, 1024, FileOptions.SequentialScan))
            {
                await file.CopyToAsync(filestream);
            }
            var fileUrlBuilder = new UriBuilder(Request.GetEncodedUrl()) { Query = null, Fragment = null };
            fileUrlBuilder.Path = Path.Combine(fileUrlBuilder.Path, file.FileName);
            var rvalue = new
            {
                method = "publish",
                @params = new
                {
                    channel = chatId,
                    data = new
                    {
                        fileurl = fileUrlBuilder.Uri,
                    }
                }
            };

            using (var client = new HttpClient())
            {
                await client.PostAsync("http://centrifugo:8000/api", JsonContent.Create(rvalue));
            }
            return Ok(file);
        }
    }
}
