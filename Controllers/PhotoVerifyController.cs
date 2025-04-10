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
    public class PhotoVerifyController(BioIDWebService.BioIDWebServiceClient bwsClient, ILogger<PhotoVerifyController> logger) : ControllerBase
    {
        // bws grpc client
        private readonly BioIDWebService.BioIDWebServiceClient _bwsClient = bwsClient;
        private readonly ILogger<PhotoVerifyController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> OnPostAsync([FromBody] PhotoVerifyRequestJson photoVerifyRequest)
        {
            // In this example, the input images are encoded in base64strings.
            try
            {
                byte[] photo = [], image1 = [], image2 = [];

                // Extract the optional request header 'Reference-Number'.
                Request.Headers.TryGetValue("Reference-Number", out var refHeader);

                // Retrieve live images and id photo from rest request
                // Verify whether the id photo has been transmitted.
                if (!string.IsNullOrEmpty(photoVerifyRequest.Photo))
                {
                    photo = Convert.FromBase64String(photoVerifyRequest.Photo);
                }

                // Verify whether the first live image has been transmitted.
                if (photoVerifyRequest.LiveImages.Count > 0)
                {
                    // Convert image from base64string.
                    image1 = Convert.FromBase64String(photoVerifyRequest.LiveImages[0].Image);
                }

                // Verify whether the second live image has been transmitted.
                if (photoVerifyRequest.LiveImages.Count > 1)
                {
                    image2 = Convert.FromBase64String(photoVerifyRequest.LiveImages[1].Image);
                }

                // Verify whether the request contains an id document and at least one live image.
                if (image1.Length == 0 && photo.Length == 0)
                {
                    _logger.LogError("Invalid parameter");
                    return BadRequest("Invalid parameter");
                }

                // Add id photo to the PhotoVerify grpc request.
                var verifyRequest = new PhotoVerifyRequest
                {
                    Photo = ByteString.CopyFrom(photo),
                    DisableLivenessDetection = photoVerifyRequest.DisableLivenessDetection
                };
                // Add live images to the PhotoVerify grpc request.
                verifyRequest.LiveImages.Add(new ImageData() { Image = ByteString.CopyFrom(image1) });

                if (image2.Length > 0)
                {
                    verifyRequest.LiveImages.Add(new ImageData() { Image = ByteString.CopyFrom(image2) });
                }

                // Call bws photoverify api via grpc.
                var photoVerifyCall = _bwsClient.PhotoVerifyAsync(verifyRequest, headers: new Metadata { { "Reference-Number", refHeader.ToString() } });

                // Read out the photoverify api response.
                var response = await photoVerifyCall.ResponseAsync.ConfigureAwait(false);

                _logger.LogInformation("Call to PhotoVerify API returned {StatusCode}.", response.Status);

                // Get grpc response metadata.
                var responseHeaders = await photoVerifyCall.ResponseHeadersAsync;

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
