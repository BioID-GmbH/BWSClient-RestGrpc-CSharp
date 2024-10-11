# Rest to Grpc Forwarder 

## Overview

The **BioID RestGrpcForwarder** project is a .NET application that serves as an interface between REST APIs and gRPC services.
The application is designed to receive requests from REST endpoints and forward them to gRPC services.
This approach allows leveraging the advantages of gRPC (such as low latency and protocol buffers) while providing a REST API externally.
To get a first impression of how our new services work, you can try them out on our [BioID Playground][playground].

## Technologies
- ASP.NET Core 8
- gRPC
- Restful
- C#

## Project structure
- `Properties/`
    - `launchsettings.json` - For configuring how the application is launched and debugged during development.
  The port on which the app runs can also be set here.
- `Auth/` - Contains custom authentication handlers that manage user authentication and authorization.
- `Controllers/` - Contains the controllers for the REST API endpoints.
- `DataTypes/` - Data types represent the data structures with which the application works.
- `protos/` - Contains a ProtoBuf file that is used to define the structure of data for gRPC services.
- `RestApiClients/` - Contains predefined lightweight REST client files dedicated to testing the application's REST API endpoints.
- `appsettings.json` - This file is for application specific configurations in an ASP.NET Core application.
- `Program.cs` - This is the main entry point of the application.

## Get Started
This web application functions as a service. It accepts REST requests, extracts input images from these requests,
and converts them into byte arrays. These byte arrays are then sent to the BWS gRPC service using a gRPC client.
In the sample application, it is assumed that the input images are encoded as Base64 strings.

If you want to use your images or videos, you can convert them to or from a base64 encoded string using an online service like [base64.guru]. 
This service can handle conversions in both directions.Any other service can also be used for the conversion.

> [!IMPORTANT]   
> Please use **base64** and not **base64url**.


> #### Before starting the service, follow these steps.
> - You need a **BioID Account** with a **confirmed** email address. If you don’t have one [create BioID account][bioidaccountregister].
> - You can request a free [trial instance][trial] of the BioID Web Service (BWS) once you've created your BioID account.
> - Once you have received your trial access, log in to the [BWS Portal][bwsportal].
> - After logging in to the BWS portal, you will be given a trial subscription to bws. You should then create your own bws client
>  to communicate with the bws service.  The client can be created using a creation wizard.
>  - If you have created a Client, click on `Show Client Key` to open the dialog box that displays the `ClientId` and `Secret` for your Client.
>
>  **The ClientId and Secret will be explained in detail later on where to insert them.** 
 

### Installation
  
1. Clone the repository.
   ```cmd
    git clone https://github.com/BioID-GmbH/RestGrpcForwarder.git
    ```

2. Navigate to the project folder and install the dependencies.
    ```cmd
    dotnet restore
    ```

3. Add your BWS gRPC client ID and secret key to the `appsettings.json` file so that you can communicate with our BWS.
Instructions on where to obtain these are provided above.
The settings file is located in the root folder of the app.


![gRPC client ID and secret key](/bwsSettings.png)


4. Build the app for your target platform. Insert your target platform without `< >` symbol.
    ```cmd
   dotnet build  -configuration Release <target platform> --self-contained true
   ```

5. Launch the application.
    ```cmd
    dotnet run --project BioID.RestGrpcForwarder
    ```
 
#### Example endpoints
*The current version of the app includes three new BWS APIs: [LivenessDetection][liveness], [PhotoVerify][photoverify]
and [VideoLivenessDetection][videoliveness].*

 > .http files are a Visual Studio feature and can only be run in Visual Studio. These files act as REST clients that you can use to test endpoints.
 > Outside of Visual Studio, other tools like `Postman` are commonly used to test APIs.

- **Liveness Detection**:
  - `RestApiClients/livenessDetection.http` - *Active livenessdetetction with 2 live images.* 
  - `RestApiClients/passiveLivenessDetection.http` - *Passive livenessdetetction with 1 live image.*
  - `RestApiClients/challengeResponse.http` - *Active livenessdetetction with 2 live images and additionally challenge response mechanism.*
  
- **PhotoVerify**:
  - `RestApiClients/photoverify.http`
  
- **VideoLivenessDetection**:
  - `RestApiClients/videoLivenessDetection.http`





[base64.guru]: https://base64.guru/ "Base64 String online converter" 
[bioidaccountregister]: https://account.bioid.com/Account/Register "Register a BioID account" 
[trial]: https://bwsportal.bioid.com/register "Register for a trial instance"
[bwsportal]: https://bwsportal.bioid.com "BWS Portal"
[liveness]: https://developer.bioid.com/bws/grpc/livenessdetection/ "Presentation attack detection."
[photoverify]: https://developer.bioid.com/bws/grpc/photoverify/ "PhotoVerify"
[videoliveness]: https://developer.bioid.com/bws/grpc/videolivenessdetection/ "Presentation attack detection in videos."
[playground]: https://playground.bioid.com "BioID Playground"


