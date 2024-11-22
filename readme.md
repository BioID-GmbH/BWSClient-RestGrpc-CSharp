# ASP.NET service for receiving RESTful calls for BioID Web Service gRPC API

## Overview

The **BioID RestGrpcForwarder** project is an ASP.NET service that receives RESTful calls and forwards these calls 
to the gRPC endpoint of the BioID Web Service 3. 

This approach allows you to utilize the advantages of gRPC (e.g. low latency and protocol buffer) while providing a 
RESTful API that is used by your client app(s).

To get a first impression of how our new biometric services work, you can try them out on our [BioID Playground][playground].

## Technologies
- ASP.NET Core 8
- RESTful API
- gRPC
- C#

## Project structure
- `Properties/`
    - `launchsettings.json` - For configuring how the application is launched and debugged during development.
  The port on which the app runs can also be set here. It is only relevant if you start in Visual Studio.
- `Auth/` - Contains custom authentication handlers that manage user authentication and authorization.
- `Controllers/` - Contains the controllers for the RESTful API endpoints.
- `DataTypes/` - Data types represent the data structures with which the application works.
- `protos/` - Contains a ProtoBuf file that is used to define the structure of data for gRPC services.
- `RestApiClients/` - Contains predefined lightweight REST client files dedicated to testing the application's RESTful API endpoints.
- `appsettings.json` - This file is for application specific configurations in an ASP.NET Core application.
- `Program.cs` - This is the main entry point of the application.

## Get Started
This sample is built with [.NET 8][dotnet8] and runs on Windows, Linux, and macOS.

Download a [development tool][dotnettools] for Windows, Linux, or macOS. You can use your preferred development environment,
such as Visual Studio, Visual Studio Code, Visual Studio for Mac, the .NET Core CLI, or other .NET tools.

The ASP.NET service accepts RESTful requests, extracts e.g. input images (encoded as Base64 strings) from these requests,
and converts them into byte arrays. These images as byte array are then sent to the BioID Web Service (BWS) gRPC endpoint using a gRPC client. 
The response from the BWS is returned to this service via gRPC and then used as the response for the RESTful call.

Depending on which gRPC API is used, additional parameters can also be transferred.

If you want to use your images or videos, you can convert them to or from a base64 encoded string using an online service like [base64.guru]. 
This service can handle conversions in both directions. Any other service can also be used for the conversion.

> [!IMPORTANT]   
> Please use **base64** and not **base64url**.


> #### Before starting the service, follow these steps.

> - You need a **BioID Account** with a **confirmed** email address. If you don’t have one, [create a BioID account][bioidaccountregister].
> - You can create a free [trial subscription][trial] of the BioID Web Service (BWS) once you've created your BioID account.
> - After you have signed in to the BWS Portal and created the trial subscription with the help of a wizard, you still need to create a BWS 3 client.
> - The client can be created with the help of a creation wizard.
> - If you have created a client, click on `Show client keys` to open the dialog box that displays the `ClientId` and `Keys` for your client.

>  **The ClientId and Key will be explained in detail later on where to insert them.** 
 

### Installation
  
1. Clone the repository.
   ```cmd
    git clone https://github.com/BioID-GmbH/BWSClient-RestGrpc-CSharp.git
    ```

2. Navigate to the project folder and install the dependencies.
    ```cmd
    dotnet restore
    ```

3. Add your BWS gRPC client Id and access key to the `appsettings.json` file so that you can communicate with our BWS 3.
Instructions on where to obtain these are provided above.
The settings file is located in the root folder of the app.


![BWS gRPC endpoint, client Id and access key](/bwsSettings.png)


4. Build the app for your target platform. Insert your target platform without `< >` symbol.
    ```cmd
   dotnet build --configuration Release <target platform> --self-contained true
   ```

5. Launch the application.
    ```cmd
    dotnet run --project BioID.RestGrpcForwarder.csproj
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

[dotnet8]: https://dotnet.microsoft.com/download "Download .NET 8"
[dotnettools]: https://dotnet.microsoft.com/platform/tools ".NET Tools & Editors"
[base64.guru]: https://base64.guru/ "Base64 String online converter" 
[bioidaccountregister]: https://account.bioid.com/Account/Register "Register a BioID account" 
[trial]: https://bwsportal.bioid.com/ "Create a free trial subscription"
[bwsportal]: https://bwsportal.bioid.com "BWS Portal"
[liveness]: https://developer.bioid.com/bws/grpc/livenessdetection/ "Presentation attack detection."
[photoverify]: https://developer.bioid.com/bws/grpc/photoverify/ "PhotoVerify"
[videoliveness]: https://developer.bioid.com/bws/grpc/videolivenessdetection/ "Presentation attack detection in videos."
[playground]: https://playground.bioid.com "BioID Playground"
