using System.Text.Json.Serialization;

namespace BioID.RestGrpcForwarder.DataTypes
{
    // Re-implementation of the JSON data transfer objects as defined with the
    // BWS 3 protobuf messages and used by the RESTful JSON web API.
    // For a description of the elements please refer to the BWS 3 API reference
    // at https://developer.bioid.com/BWS/NewBws

    // see https://developer.bioid.com/bws/grpc/livenessdetection
    public class LivenessDetectionRequestJson
    {
        public List<ImageDataJson> LiveImages { get; set; } = [];
    }

    // see https://developer.bioid.com/bws/grpc/videolivenessdetection
    public class VideoLivenessDetectionRequestJson
    {
        public string Video { get; set; } = string.Empty;
    }

    // see https://developer.bioid.com/bws/grpc/livenessdetection
    // or https://developer.bioid.com/bws/grpc/videolivenessdetection
    public class LivenessDetectionResponseJson
    {
        public JobStatusJson Status { get; set; }
        public List<JobErrorJson> Errors { get; set; } = [];
        public List<ImagePropertiesJson> ImageProperties { get; set; } = [];
        public bool Live { get; set; }
        public double LivenessScore { get; set; }
    }

    // see https://developer.bioid.com/bws/grpc/photoverify
    public class PhotoVerifyRequestJson
    {
        public List<ImageDataJson> LiveImages { get; set; } = [];
        public required string Photo { get; set; }
        public bool DisableLivenessDetection { get; set; }
    }


    // see https://developer.bioid.com/bws/grpc/photoverify
    public class PhotoVerifyResponseJson
    {
        public JobStatusJson Status { get; set; }
        public List<JobErrorJson> Errors { get; set; } = [];
        public List<ImagePropertiesJson> ImageProperties { get; set; } = [];
        public ImagePropertiesJson PhotoProperties { get; set; } = new();
        public AccuracyLevelJson VerificationLevel { get; set; }
        public double VerificationScore { get; set; }
        public bool Live { get; set; }
        public double LivenessScore { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter<AccuracyLevelJson>))]
        public enum AccuracyLevelJson { NOT_RECOGNIZED = 0, LEVEL_1, LEVEL_2, LEVEL_3, LEVEL_4, LEVEL_5 }
    }

    // see https://developer.bioid.com/bws/grpc/JobStatus
    [JsonConverter(typeof(JsonStringEnumConverter<JobStatusJson>))]
    public enum JobStatusJson { SUCCEEDED = 0, FAULTED = 1, CANCELLED = 2 }

    // see https://developer.bioid.com/bws/grpc/JobError
    public class JobErrorJson
    {
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    // see https://developer.bioid.com/bws/grpc/ImageData
    public class ImageDataJson
    {
        public required string Image { get; set; }
        public List<string> Tags { get; set; } = [];
    }

    // see https://developer.bioid.com/bws/grpc/ImageProperties
    public class ImagePropertiesJson
    {
        public int Rotated { get; set; }
        public List<FaceJson> Faces { get; set; } = [];
        public double QualityScore { get; set; }
        public List<QualityAssessmentJson> QualityAssessments { get; set; } = [];
        public int FrameNumber { get; set; }
    }

    // see https://developer.bioid.com/bws/grpc/QualityAssessment
    public class QualityAssessmentJson
    {
        public string Check { get; set; } = string.Empty;
        public double Score { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // see https://developer.bioid.com/bws/grpc/ImageProperties
    public class FaceJson
    {
        public required PointDJson LeftEye { get; set; }
        public required PointDJson RightEye { get; set; }
        public double TextureLivenessScore { get; set; }
        public double MotionLivenessScore { get; set; }
        public double MovementDirection { get; set; }
    }
    public class PointDJson { public double X { get; set; } public double Y { get; set; } }

}
