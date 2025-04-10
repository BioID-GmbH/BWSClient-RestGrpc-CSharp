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
    public class LivenessDetectionController(BioIDWebService.BioIDWebServiceClient bwsClient, ILogger<LivenessDetectionController> logger) : ControllerBase
    {
        // bws grpc client
        private readonly BioIDWebService.BioIDWebServiceClient _bwsClient = bwsClient;
        private readonly ILogger<LivenessDetectionController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> OnPostAsync([FromBody] LivenessDetectionRequestJson livenessDetectionRequest)
        {
            // In this example, the input images are encoded in base64strings.
            try
            {
                byte[] image1 = [], image2 = [];

                // Extract the optional request header 'Reference-Number'.
                Request.Headers.TryGetValue("Reference-Number", out var refHeader);

                // Verify whether the first live image has been transmitted.
                if (livenessDetectionRequest.LiveImages.Count > 0)
                {
                    // Convert image from base64string.
                    image1 = Convert.FromBase64String(livenessDetectionRequest.LiveImages[0].Image);
                }

                // Verify whether the second live image has been transmitted.
                if (livenessDetectionRequest.LiveImages.Count > 1)
                {
                    image2 = Convert.FromBase64String(livenessDetectionRequest.LiveImages[1].Image);
                }

                // Create LivenessDetection request.
                var livenessRequest = new LivenessDetectionRequest();

                // Add live images to the LivenessDetection grpc request.
                livenessRequest.LiveImages.Add(new ImageData { Image = ByteString.CopyFrom(image1) });
                if (image2.Length > 0)
                {
                    var imgdata2 = new ImageData { Image = ByteString.CopyFrom(image2) };
                    // Add the tags to the request for challenge response
                    if (livenessDetectionRequest.LiveImages[1].Tags.Count > 0)
                    {
                        imgdata2.Tags.AddRange(livenessDetectionRequest.LiveImages[1].Tags);
                    }
                    livenessRequest.LiveImages.Add(imgdata2);
                }

                // Call bws LivenessDetection api via grpc.
                var call = _bwsClient.LivenessDetectionAsync(livenessRequest, headers: new Metadata { { "Reference-Number", refHeader.ToString() } });

                // Read out the LivenessDetection api response.
                var response = await call.ResponseAsync.ConfigureAwait(false);

                _logger.LogInformation("Call to LivenessDetection API returned {StatusCode}.", response.Status);

                // Get grpc response metadata.
                var responseHeaders = await call.ResponseHeadersAsync;

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
