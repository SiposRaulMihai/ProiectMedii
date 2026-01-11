# Project Report - ParcAuto_Web_App (MAUI Application)

## Overview

ParcAuto_Web_App is a .NET MAUI cross-platform mobile application that provides full CRUD operations for vehicle fleet management with secure authentication.

## Requirements Fulfillment

### 1. UI + CRUD Operations

#### Vehicles

- **List View**: `VehicleListPage.xaml` - CollectionView with pull-to-refresh
- **Details View**: `VehicleDetailsPage.xaml` - Display all vehicle properties
- **Create/Edit**: `VehicleEditPage.xaml` - Form with validation
- **CRUD Operations**: Full Create, Read, Update, Delete via `ApiService`

**API Endpoints**:

- GET /api/vehicles
- GET /api/vehicles/{id}
- POST /api/vehicles [Authorize]
- PUT /api/vehicles/{id} [Authorize]
- DELETE /api/vehicles/{id} [Authorize]

#### Reservations

- **List View**: `ReservationListPage.xaml` - CollectionView with pull-to-refresh
- **Details View**: `ReservationDetailsPage.xaml` - Display reservation info
- **Create/Edit**: `ReservationEditPage.xaml` - Form with Vehicle/Driver pickers
- **CRUD Operations**: Full Create, Read, Update, Delete via `ApiService`

**API Endpoints**:

- GET /api/reservations [Authorize]
- GET /api/reservations/{id} [Authorize]
- POST /api/reservations [Authorize]
- PUT /api/reservations/{id} [Authorize]
- DELETE /api/reservations/{id} [Authorize]
- GET /api/reservations/available-vehicles [Authorize]

#### Drivers

- **Read-only**: Used as picker source in ReservationEditPage
- GET /api/auth/drivers

### 2. Input Validation

#### Vehicle Validation

- Make, Model, Year, License Plate, VIN: **Required**
- Year: Valid range (1900 to current year + 1)
- Status: Picker with predefined values

#### Reservation Validation (ReservationEditViewModel)

- **VehicleId**: Required (picker selection)
- **DriverId**: Required (picker selection)
- **StartDate**: Required
- **EndDate**: Required
- **Critical**: `EndDate > StartDate` (strictly greater than)
- **Purpose**: Required and cannot be empty

**Validation Implementation**:

```csharp
if (EndDate <= StartDate)
{
    ErrorMessage = "End date must be after start date.";
    return false;
}
```

**UI Feedback**:

- Error messages displayed in red Frame with red border
- Required fields marked with `*` and bold labels
- Save button blocked if validation fails
- Server validation errors displayed in UI

### 3. Authentication & Authorization

#### JWT Token Management

- **Login**: POST /api/auth/login - Returns JWT token
- **Register**: POST /api/auth/register
- **Token Storage**: SecureStorage (platform-specific secure storage)
- **Token Attachment**: Automatic Bearer token on all authorized requests

**Implementation** (`ApiService.cs`):

```csharp
public async Task SetTokenAsync(string token)
{
    await SecureStorage.SetAsync(TokenKey, token);
    _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", token);
}
```

#### Session Management

- **Startup Check**: App checks SecureStorage for existing token
  - Token exists → Navigate to AppShell (main app)
  - No token → Show LoginPage
- **401 Unauthorized Handling**: Automatic logout and redirect to LoginPage
- **Logout**: Clears token from SecureStorage and returns to LoginPage

#### Secure Storage

- Platform-specific implementation:
  - **Android**: KeyStore
  - **iOS**: KeyChain
  - **Windows**: Data Protection API
  - **macOS**: KeyChain

### 4. Architecture

#### MVVM Pattern

- **Models**: Vehicle, Reservation, Driver
- **ViewModels**: CommunityToolkit.Mvvm with `[ObservableProperty]` and `[RelayCommand]`
- **Views**: XAML pages with data binding
- **Services**: ApiService for HTTP communication

#### Dependency Injection

All ViewModels and Views registered in `MauiProgram.cs`:

- Services: Singleton (ApiService, HttpClient)
- ViewModels: Transient
- Views: Transient

#### Navigation

- Shell-based navigation with Flyout menu
- Routes registered for detail and edit pages
- QueryProperty for passing parameters

### 5. Platform-Specific Features

#### Base URL Selection (Runtime)

```csharp
- Android Emulator: http://10.0.2.2:5000
- iOS Simulator: http://localhost:5000
- Windows: http://localhost:5000
- MacCatalyst: http://localhost:5000
```

#### Target Frameworks

- net9.0-android
- net9.0-ios
- net9.0-windows10.0.19041.0
- net9.0-maccatalyst

## Technologies Used

- **.NET 9.0**
- **MAUI** (Multi-platform App UI)
- **CommunityToolkit.Mvvm** - MVVM helpers
- **System.Net.Http** - REST API communication
- **SecureStorage** - Secure token storage
- **CollectionView** - List rendering with refresh

## Conclusion

ParcAuto_Web_App successfully implements all project requirements:
✅ Complete UI with CRUD operations for Vehicles and Reservations
✅ Comprehensive input validation including date range validation
✅ Secure JWT authentication with token persistence
✅ Authorization with automatic 401 handling
✅ Cross-platform support (Android, iOS, Windows, macOS)
✅ MVVM architecture with proper separation of concerns
