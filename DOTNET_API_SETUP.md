# .NET API Integration Guide

This project has been migrated to use React Native (Expo) for all three platforms (iOS, Android, and Web) with a .NET backend API.

## Architecture Overview

```
┌─────────────────────────────────────────────┐
│           React Native (Expo)                │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐     │
│  │   iOS   │  │ Android │  │   Web   │     │
│  └─────────┘  └─────────┘  └─────────┘     │
│       │            │            │           │
│       └────────────┴────────────┘           │
│                    │                        │
│            Shared Components                │
│                    │                        │
└────────────────────┼────────────────────────┘
                     │
                     ▼
          ┌──────────────────┐
          │   .NET API        │
          │  (Backend)        │
          └──────────────────┘
                     │
                     ▼
          ┌──────────────────┐
          │    Database       │
          │  (SQL Server /    │
          │   PostgreSQL)     │
          └──────────────────┘
```

## What Changed

### ✅ Completed Migrations

1. **API Client Infrastructure**
   - Created `packages/shared/src/lib/apiClient.ts` for HTTP requests
   - Supports authenticated and unauthenticated requests
   - Automatic token management
   - Error handling and type safety

2. **All APIs Migrated to .NET Endpoints**
   - ✅ Authentication (`/api/auth/*`)
   - ✅ Categories (`/api/categories/*`)
   - ✅ Providers (`/api/providers/*`)
   - ✅ Appointments (`/api/appointments/*`)
   - ✅ Reviews (`/api/reviews/*`)
   - ✅ Employees (`/api/employees/*`)

3. **Environment Configuration**
   - Updated to use `NEXT_PUBLIC_API_URL` / `EXPO_PUBLIC_API_URL`
   - Defaults to `http://localhost:5000/api`
   - Supabase dependencies commented out

4. **Expo Web Support**
   - Configured in `apps/mobile/app.json`
   - React Native now works for web, iOS, and Android

## Getting Started

### Prerequisites

1. **.NET 6.0 or higher** installed
2. **Node.js 18+** and npm
3. Your .NET API backend running

### Setup Instructions

#### 1. Configure Environment Variables

Update the `.env` files in your apps:

**For Web App** (`apps/web/.env.local`):
```bash
NEXT_PUBLIC_API_URL=http://localhost:5000/api
NEXT_PUBLIC_APP_URL=http://localhost:3000
```

**For Mobile App** (`apps/mobile/.env`):
```bash
EXPO_PUBLIC_API_URL=http://localhost:5000/api
EXPO_PUBLIC_APP_URL=http://localhost:3000
```

**For Production**, use your actual API URL:
```bash
NEXT_PUBLIC_API_URL=https://api.yourapp.com/api
EXPO_PUBLIC_API_URL=https://api.yourapp.com/api
```

#### 2. Install Dependencies

```bash
npm install
```

#### 3. Start Your .NET API

Make sure your .NET API is running on `http://localhost:5000` (or update the env vars above).

Your .NET API should have CORS configured to allow requests from:
- `http://localhost:3000` (web app)
- `http://localhost:8081` (Expo dev server)

```csharp
// In your .NET API Startup.cs or Program.cs
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins("http://localhost:3000", "http://localhost:8081")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
```

#### 4. Run the Applications

**Web (Next.js):**
```bash
npm run dev:web
```
Open [http://localhost:3000](http://localhost:3000)

**Mobile (All Platforms - iOS, Android, Web):**
```bash
npm run dev:mobile
```

Then:
- Press `w` for web
- Press `i` for iOS simulator
- Press `a` for Android emulator
- Scan QR code with Expo Go app on your phone

## .NET API Endpoints Reference

Your .NET API should implement these endpoints:

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login
- `GET /api/auth/me` - Get current user
- `GET /api/auth/profile` - Get user profile
- `PUT /api/auth/profile` - Update profile
- `PUT /api/auth/password` - Update password
- `POST /api/auth/forgot-password` - Request password reset

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `GET /api/categories/slug/{slug}` - Get category by slug

### Providers
- `GET /api/providers/search?lat={lat}&lng={lng}&radius={km}` - Search providers
- `GET /api/providers/{id}` - Get provider details
- `GET /api/providers/{id}/reviews` - Get provider reviews
- `GET /api/providers/me` - Get current user's provider
- `POST /api/providers` - Create provider (authenticated)
- `PUT /api/providers/{id}` - Update provider (authenticated)
- `GET /api/providers/{id}/available-slots` - Get available time slots
- `GET /api/providers/{id}/quick-slots` - Get quick booking slots (next 3 hours)

### Appointments
- `POST /api/appointments` - Create appointment (authenticated)
- `GET /api/appointments/my` - Get user's appointments (authenticated)
- `GET /api/appointments/{id}` - Get appointment details (authenticated)
- `PUT /api/appointments/{id}/cancel` - Cancel appointment (authenticated)
- `PUT /api/appointments/{id}/status` - Update appointment status (provider only)
- `GET /api/appointments/provider/{providerId}` - Get provider appointments (authenticated)

### Reviews
- `POST /api/reviews` - Create review (authenticated)
- `GET /api/providers/{id}/reviews` - Get provider reviews
- `GET /api/appointments/{id}/review` - Get review for appointment
- `PUT /api/reviews/{id}/respond` - Respond to review (provider only)
- `GET /api/reviews/my-provider` - Get reviews for current user's provider (authenticated)

### Employees
- `GET /api/employees` - Get provider's employees (authenticated)
- `GET /api/employees/{id}` - Get employee by ID (authenticated)
- `POST /api/employees` - Create employee (authenticated)
- `PUT /api/employees/{id}` - Update employee (authenticated)
- `PUT /api/employees/{id}/deactivate` - Deactivate employee (authenticated)
- `GET /api/employees/{id}/schedules` - Get employee schedules (authenticated)
- `POST /api/employees/schedules` - Create schedule (authenticated)
- `PUT /api/employees/schedules/{id}` - Update schedule (authenticated)
- `DELETE /api/employees/schedules/{id}` - Delete schedule (authenticated)
- `GET /api/employees/{id}/time-off` - Get employee time off (authenticated)
- `POST /api/employees/{id}/time-off` - Create time off (authenticated)
- `DELETE /api/employees/time-off/{id}` - Delete time off (authenticated)

## Authentication Flow

The app uses JWT token authentication:

1. User logs in via `POST /api/auth/login`
2. API returns `{ token: "jwt-token", user: {...} }`
3. Token is stored in localStorage (web) or AsyncStorage (mobile)
4. All authenticated requests include `Authorization: Bearer {token}` header
5. Token is automatically added by the `authApi` helper functions

## Data Models

All TypeScript interfaces are defined in:
- `packages/shared/src/api/auth.ts` - User, Profile, Auth types
- `packages/shared/src/api/providers.ts` - Provider, Service, Location types
- `packages/shared/src/api/categories.ts` - ServiceCategory types
- `packages/shared/src/api/appointments.ts` - Appointment, TimeSlot types
- `packages/shared/src/api/reviews.ts` - Review types
- `packages/shared/src/api/employees.ts` - Employee, Schedule, TimeOff types

### Key Naming Conventions

The API uses **camelCase** for JSON properties (standard for .NET APIs with `JsonNamingPolicy.CamelCase`):

```typescript
// Example: Provider response
{
  "id": "uuid",
  "businessName": "Business Name",  // camelCase
  "ratingAverage": 4.5,             // camelCase
  "locations": [...]
}
```

## Testing Your .NET API

Use the React Native app to test all endpoints:

1. **Test Authentication:**
   - Navigate to Sign Up/Login screens
   - Create an account
   - Verify token is stored and user is authenticated

2. **Test Providers:**
   - Browse providers on home screen
   - Search by location and category
   - View provider details

3. **Test Bookings:**
   - Select a provider and service
   - View available time slots
   - Create an appointment

4. **Test Reviews:**
   - Complete an appointment
   - Leave a review
   - Provider responds to review

## Troubleshooting

### CORS Errors

If you see CORS errors, ensure your .NET API has CORS configured (see setup instructions above).

### API Connection Refused

- Verify your .NET API is running on the correct port
- Check the `NEXT_PUBLIC_API_URL` / `EXPO_PUBLIC_API_URL` in your `.env` files
- For mobile testing, use your machine's IP address instead of `localhost`:
  ```bash
  EXPO_PUBLIC_API_URL=http://192.168.1.100:5000/api
  ```

### Authentication Errors

- Clear localStorage/AsyncStorage and try logging in again
- Verify your .NET API is returning tokens in the correct format
- Check token expiration settings in your .NET API

### TypeScript Errors

If you see TypeScript errors related to API responses:
1. The .NET API might be returning different property names
2. Update the TypeScript interfaces in `packages/shared/src/api/`
3. Ensure your .NET API uses `JsonNamingPolicy.CamelCase`

## Migration from Supabase

The Supabase client is still available but no longer used. All API calls now go through the .NET backend.

**Old way (Supabase):**
```typescript
import { supabase } from 'shared'
const { data } = await supabase.from('providers').select('*')
```

**New way (.NET API):**
```typescript
import { providersApi } from 'shared'
const { providers } = await providersApi.searchProviders({ ... })
```

## Next Steps

1. **Build your .NET API** to implement all the endpoints listed above
2. **Test each endpoint** using the React Native app
3. **Deploy your .NET API** to production (Azure, AWS, or your hosting provider)
4. **Update environment variables** in production builds
5. **Build mobile apps** using `expo build` or EAS Build

## Support

For issues or questions:
- Check the TypeScript interfaces for expected API responses
- Review the API client code in `packages/shared/src/lib/apiClient.ts`
- Ensure your .NET API matches the expected endpoint structure

Good luck with your .NET API integration! 🚀
