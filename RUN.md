# Run Instructions - ParcAuto_Web_App

This document provides instructions for running the ParcAuto_Web_App MAUI application and the backend API.

## Prerequisites

- .NET 9.0 SDK
- Platform-specific SDKs (Android SDK, Xcode for iOS/Mac, Windows SDK)

## Running the Backend API

Before running the mobile app, start the backend API server:

```bash
cd ../ParcAutoBackend
dotnet run
```

The backend will run on:

- **HTTPS**: https://localhost:7000
- **HTTP**: http://localhost:5000
- **Swagger UI**: https://localhost:7000/swagger

## Running the MAUI Application

Navigate to the ParcAuto_Web_App directory:

```bash
cd ParcAuto_Web_App
```

### Android

```bash
dotnet build -t:Run -f net9.0-android
```

**Note**: Android emulator connects to backend via `http://10.0.2.2:5000`

### iOS (Mac only)

```bash
dotnet build -t:Run -f net9.0-ios
```

**Note**: iOS simulator connects to backend via `http://localhost:5000`

### Windows

```bash
dotnet run -f net9.0-windows10.0.19041.0
```

**Note**: Windows connects to backend via `http://localhost:5000`

### macOS (MacCatalyst)

```bash
dotnet build -t:Run -f net9.0-maccatalyst
```

**Note**: MacCatalyst connects to backend via `http://localhost:5000`

## Platform-Specific Base URLs

The app automatically selects the correct base URL based on the platform:

- **Android Emulator**: `http://10.0.2.2:5000`
- **iOS Simulator**: `http://localhost:5000`
- **Windows**: `http://localhost:5000`
- **MacCatalyst**: `http://localhost:5000`

## Troubleshooting

### Backend Connection Issues

If the mobile app cannot connect to the backend:

1. Verify the backend is running on the correct port
2. Check firewall settings
3. For Android emulator, ensure `10.0.2.2` routes correctly
4. For physical devices, use your machine's IP address instead of localhost

### Build Issues

- **JDK Version** (Android): Requires JDK 21
- **Xcode Version** (iOS/MacCatalyst): May require specific Xcode version for .NET 9
- **Windows SDK**: Ensure Windows 10 SDK (19041) is installed

## Default Credentials

Check the backend documentation for default test user credentials.
