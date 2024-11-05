using BioID.RestGrpcForwarder.DataTypes;
using BioID.Services;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioID.RestGrpcForwarder.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class VideoLivenessDetectionController(BioIDWebService.BioIDWebServiceClient bwsClient, ILogger<VideoLivenessDetectionController> logger) : ControllerBase
    {
        // bws grpc client
        private readonly BioIDWebService.BioIDWebServiceClient _bwsClient = bwsClient;
        private readonly ILogger<VideoLivenessDetectionController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> OnPostAsync([FromBody] VideoLivenessDetectionRequestJson videoLivenessDetectionRequest)
        {
            try
            {
                byte[] video = [];

                // Extract the optional request header 'Reference-Number'.
                Request.Headers.TryGetValue("Reference-Number", out var refHeader);

                // Extract video file from request.
                if (!string.IsNullOrWhiteSpace(videoLivenessDetectionRequest.Video))
                {
                    video = Convert.FromBase64String(videoLivenessDetectionRequest.Video);
                }

                // Verify if the request includes a video file.
                if (video.Length == 0)
                {
                    _logger.LogError("No video file provided.");
                    return BadRequest("No video file provided.");
                }

                // Add video sample to the grpc service request
                var videoRequest = new VideoLivenessDetectionRequest()
                {
                    Video = ByteString.CopyFrom(video)
                };
                var videoLivenessCall = _bwsClient.VideoLivenessDetectionAsync(videoRequest, headers: new Metadata { { "Reference-Number", refHeader.ToString() } });
                var response = await videoLivenessCall.ResponseAsync.ConfigureAwait(false);

                _logger.LogInformation("Call to videoLivedetection API returned {StatusCode}.", response.Status);

                // Get grpc response metadata.
                var responseHeaders = await videoLivenessCall.ResponseHeadersAsync;

                // Add the grpc response metadata to the rest response header.
                foreach (var header in responseHeaders)
                {
                    if (!Response.Headers.TryGetValue(header.Key, out var _)) { Response.Headers.Append(header.Key, header.Value); }
                }
                return Ok(response);
            }
            catch (RpcException ex)
            {
                _logger.LogError("gRPC error from calling service: {StatusCode} - {StatusDetail}", ex.Status.StatusCode, ex.Status.Detail);
                return BadRequest(ex.Status.Detail);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has occurred: {exception}", ex);
                return BadRequest(ex);
            }
        }
    }
}
