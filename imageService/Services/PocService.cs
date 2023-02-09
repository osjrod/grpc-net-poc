using Grpc.Core;
using imageService;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using ImageResizer;
using System.IO;
using Google.Protobuf;
using SixLabors.ImageSharp;
using pocService;
using Google.Protobuf.WellKnownTypes;

namespace imageService.Services
{
    public class PocService : Poc.PocBase
    {
        private readonly ILogger<PocService> _logger;
        public PocService(ILogger<PocService> logger)
        {
            _logger = logger;
        }

        public override Task<ImageResizeReply> Resize(ImageResizeRequest request, ServerCallContext context)
        {
            byte[] original = request.Image.ToByteArray();
            var resized = Resizer.ResizeImage(original, request.Width,request.Height);
            return Task.FromResult(new ImageResizeReply
            {
                Image = ByteString.CopyFrom(resized)
            }); 
        }

        public override async Task Guess(IAsyncStreamReader<pocService.Number> requestStream,IServerStreamWriter<Response> responseStream,ServerCallContext context)
        {
            var numberToGuess = new Random().Next(1, 101);
            var round = 0;

            var response = new Response { Message = "Hey, guess a number between 1 and 100!" };
            await responseStream.WriteAsync(response);

            await foreach (var number in requestStream.ReadAllAsync())
            {
                round++;
                if (number.Value < numberToGuess)
                {
                    response.Message = $"My number is greather than {number.Value}";
                }
                else if (number.Value > numberToGuess)
                {
                    response.Message = $"My number is less than {number.Value}";
                }
                else
                {
                    response.Found = true;
                    response.Message = $"Hey, you found the number {number.Value} in {round} rounds!";
                }
                await responseStream.WriteAsync(response);
            }
        }

        public override async Task Receive(Empty _, IServerStreamWriter<pocService.Number> responseStream, ServerCallContext context)
        {
            var i = 0;
            while (!context.CancellationToken.IsCancellationRequested && i <= 50)
            {
                await Task.Delay(1000); 

                await responseStream.WriteAsync(new pocService.Number { Value = i });

                i++;
            }
        }
    }
}