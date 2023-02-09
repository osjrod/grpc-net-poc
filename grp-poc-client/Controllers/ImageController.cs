using Google.Protobuf;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace grp_poc_client.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImageController : ControllerBase
    {
        private Poc.PocClient client;
        private readonly ILogger<ImageController> _logger;

        public ImageController(ILogger<ImageController> logger)
        {
            _logger = logger;
            var channel = GrpcChannel.ForAddress("https://localhost:7069");
            client = new Poc.PocClient(channel);
        }

        [HttpPost(Name = "ResizeImage")]
        public async Task<IActionResult> Resize([FromForm] Models.Image imageModel)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageModel.File.CopyToAsync(memoryStream);

                    var reply = client.Resize(new ImageResizeRequest { Image = ByteString.CopyFrom(memoryStream.ToArray()), Width = imageModel.Width, Height = imageModel.Height });

                    return File(reply.Image.ToByteArray(), "image/jpeg");

                }
            }
            else
            {
                return Ok("Invalid");
            }
        }

    }
}
