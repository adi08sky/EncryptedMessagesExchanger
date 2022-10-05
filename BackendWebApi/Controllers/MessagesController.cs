using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackendWebApi.Services;

namespace BackendWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessagesService _messagesService;

        public MessageController(IMessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        [HttpGet]
        public string GetInfo()
        {
            return "To get a decrypted message, enter the message ID in the request URL path and the key in the request body";
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetMessage([FromRoute] int id, [FromBody] string key)
        {
            try
            {
                var message = await Task.Run(() => _messagesService.GetMessageById(id));
                var decryptedMessage = await Task.Run(() => _messagesService.GetDecryptedMessage(message, key));

                if (string.IsNullOrEmpty(decryptedMessage))
                    throw new Exception($"message with id:{id} could not be decrypted with key:{key}");

                HttpContext.Response.StatusCode = 418;
                return decryptedMessage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
