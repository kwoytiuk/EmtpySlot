# EmptySlot - .NET MAUI Migration

This is the complete .NET MAUI rewrite of the EmptySlot appointment booking application.

## Technology Stack

- **Backend**: ASP.NET Core 8.0 Web API
- **Database**: SQL Server
- **Mobile**: .NET MAUI (iOS, Android)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT Bearer Tokens
- **Architecture**: MVVM with CommunityToolkit.Mvvm

## Project Structure

```
EmptySlot/
├── EmptySlot.API/          # ASP.NET Core Web API
│   ├── Controllers/        # API endpoints
│   ├── Data/              # DbContext and migrations
│   └── Services/          # Business logic
├── EmptySlot.Mobile/       # .NET MAUI mobile app
│   ├── Pages/             # XAML pages
│   ├── ViewModels/        # MVVM view models
│   └── Services/          # API client services
├── EmptySlot.Shared/       # Shared models and DTOs
│   ├── Models/            # Entity models
│   └── Enums/             # Enumerations
└── Database/              # SQL scripts
    ├── CreateSchema.sql   # Database schema
    └── SeedData.sql       # Sample data
```

## Prerequisites

1. **Visual Studio 2022** (17.8 or later) with:
   - ASP.NET and web development workload
   - .NET Multi-platform App UI development workload

2. **.NET 8 SDK** - Download from https://dotnet.microsoft.com/download

3. **SQL Server** (LocalDB, Express, or full version)
   - Download SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads
   - Or use SQL Server LocalDB (included with Visual Studio)

4. **SQL Server Management Studio (SSMS)** (optional but recommended)
   - Download from: https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms

## Setup Instructions

### 1. Database Setup

#### Option A: Using SQL Server Management Studio (SSMS)

1. Open SSMS and connect to your SQL Server instance
2. Open `Database/CreateSchema.sql`
3. Execute the script (F5)
4. Verify the database `EmptySlotDb` was created
5. (Optional) Run `Database/SeedData.sql` to add sample categories

#### Option B: Using Visual Studio

1. Open **View > SQL Server Object Explorer**
2. Connect to your SQL Server instance
3. Right-click on **Databases** > **Add New Database** > Name it `EmptySlotDb`
4. Right-click on the database > **New Query**
5. Paste the contents of `Database/CreateSchema.sql` and execute

#### Option C: Using Entity Framework Migrations

```bash
cd EmptySlot.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 2. API Configuration

1. Open `EmptySlot.API/appsettings.json`

2. Update the connection string to match your SQL Server:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=EmptySlotDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

   **Common connection string examples:**

   - **LocalDB**: `Server=(localdb)\\mssqllocaldb;Database=EmptySlotDb;Trusted_Connection=True;`
   - **SQL Server Express**: `Server=localhost\\SQLEXPRESS;Database=EmptySlotDb;Trusted_Connection=True;TrustServerCertificate=True;`
   - **Remote SQL Server**: `Server=your-server;Database=EmptySlotDb;User Id=your-username;Password=your-password;TrustServerCertificate=True;`

3. **Important**: Change the JWT secret key in production:
   ```json
   {
     "JwtSettings": {
       "SecretKey": "CHANGE-THIS-TO-A-SECURE-RANDOM-KEY-AT-LEAST-32-CHARACTERS-LONG"
     }
   }
   ```

### 3. Mobile App Configuration

1. Open `EmptySlot.Mobile/Services/ApiService.cs`

2. Update the `BaseUrl` to point to your API:
   ```csharp
   private const string BaseUrl = "https://your-machine-ip:7001/api";
   ```

3. For local development:
   - **Android Emulator**: Use `https://10.0.2.2:7001/api`
   - **iOS Simulator**: Use `https://localhost:7001/api`
   - **Physical Device**: Use your machine's IP address (e.g., `https://192.168.1.100:7001/api`)

4. Also update `BaseUrl` in `EmptySlot.Mobile/Services/AuthService.cs`

### 4. Running the Application

#### Start the API

1. In Visual Studio, right-click on `EmptySlot.sln` > **Properties**
2. Select **Multiple startup projects**
3. Set `EmptySlot.API` to **Start**
4. Click **OK**
5. Press **F5** to start the API
6. The API will open Swagger UI at `https://localhost:7001/swagger`

#### Start the Mobile App

1. Set `EmptySlot.Mobile` as the startup project
2. Select your target platform (Android or iOS)
3. Select a device or emulator
4. Press **F5** to build and deploy

## API Endpoints

The API includes the following endpoints:

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Providers
- `GET /api/providers/search` - Search providers
- `GET /api/providers/{id}` - Get provider details
- `GET /api/providers/{id}/reviews` - Get provider reviews

### Appointments
- `GET /api/appointments` - Get user's appointments
- `POST /api/appointments` - Create appointment
- `GET /api/appointments/available-slots` - Get available time slots
- `PATCH /api/appointments/{id}/cancel` - Cancel appointment

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category details

## Data Migration from Supabase

To migrate your existing data from Supabase to SQL Server:

1. Export data from Supabase:
   ```sql
   -- In Supabase SQL Editor, run for each table:
   COPY (SELECT * FROM profiles) TO STDOUT WITH CSV HEADER;
   COPY (SELECT * FROM providers) TO STDOUT WITH CSV HEADER;
   -- etc.
   ```

2. Import into SQL Server using SSMS or `bcp` utility

3. Or use the provided migration script (coming soon)

## Troubleshooting

### API won't start
- Check if SQL Server is running
- Verify connection string in `appsettings.json`
- Check if port 7001 is available

### Mobile app can't connect to API
- Ensure API is running and accessible
- Check firewall settings
- For Android emulator, use `10.0.2.2` instead of `localhost`
- For physical devices, ensure they're on the same network

### Database errors
- Verify SQL Server is running
- Check connection string
- Ensure database exists and schema is created
- Check SQL Server error logs

## Development Notes

### Adding New Entity Framework Migrations

```bash
cd EmptySlot.API
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

### Testing the API

Use Swagger UI at `https://localhost:7001/swagger` to test endpoints without the mobile app.

### Mobile App Hot Reload

.NET MAUI supports XAML Hot Reload for faster development. Make changes to XAML files and see them instantly without rebuilding.

## Next Steps

1. ✅ Database schema created
2. ✅ API endpoints implemented
3. ✅ Mobile app structure created
4. ⏳ Add authentication flow
5. ⏳ Implement booking flow
6. ⏳ Add payment integration
7. ⏳ Add push notifications
8. ⏳ Deploy to production

## Benefits of This Stack

Compared to the previous React Native + Supabase setup:

- **No Metro bundler issues** - Native compilation
- **Type safety** - C# is strongly typed
- **Better performance** - Native apps, no JavaScript bridge
- **Simpler dependencies** - NuGet is more reliable than npm
- **Better tooling** - Visual Studio debugging is excellent
- **Full control** - Your own API and database
- **Offline support** - Easier to implement with EF Core

## Support

For questions or issues:
1. Check the troubleshooting section
2. Review API documentation in Swagger
3. Check Entity Framework Core documentation
4. Review .NET MAUI documentation

## License

[Your License Here]
