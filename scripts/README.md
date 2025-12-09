# EmptySlot Database Seeding Scripts

This directory contains scripts for populating the database with test data.

## Seed 200 Providers Script

This script creates 200 complete provider accounts with:
- ✅ Authentication credentials (email/password)
- ✅ Provider profiles with realistic business information
- ✅ Locations in Calgary with real addresses
- ✅ Services for each provider (3-5 services each)
- ✅ Employees with weekly schedules (2-5 employees per provider)

### Prerequisites

1. **Supabase Project**: You need a running Supabase project
2. **Service Role Key**: Get this from your Supabase dashboard under Settings → API
3. **Migrations**: Run all database migrations first

### Setup

1. Install dependencies:
```bash
cd scripts
npm install
```

2. Set environment variables:
```bash
# Create .env file in scripts directory
echo "NEXT_PUBLIC_SUPABASE_URL=your-supabase-url" > .env
echo "SUPABASE_SERVICE_ROLE_KEY=your-service-role-key" >> .env
```

Or export them:
```bash
export NEXT_PUBLIC_SUPABASE_URL="https://your-project.supabase.co"
export SUPABASE_SERVICE_ROLE_KEY="your-service-role-key-here"
```

### Running the Script

```bash
# From scripts directory
npm run seed:providers
```

**What happens when you run the script:**
1. 🧹 **Automatic Cleanup** - First, the script removes all existing seed test accounts (provider1-200@emptyslot.test) and their associated data
2. 🚀 **Fresh Creation** - Then creates 200 new provider accounts with all their data

**Duration:** Approximately 5-10 minutes to complete (includes cleanup + creation with rate limiting)

**Note:** The script is safe to run multiple times - it always starts with a clean slate!

### Generated Credentials

The script creates 200 test accounts with predictable credentials:

**Pattern:**
- Email: `provider1@emptyslot.test` through `provider200@emptyslot.test`
- Password: `Provider1!Test` through `Provider200!Test`

**Examples:**
```
Email: provider1@emptyslot.test
Password: Provider1!Test

Email: provider50@emptyslot.test
Password: Provider50!Test

Email: provider200@emptyslot.test
Password: Provider200!Test
```

### What Gets Created

For each of the 200 providers:

1. **Auth User**: Supabase authentication account
2. **Profile**: User profile with contact info
3. **Provider**: Business profile with:
   - Business name (e.g., "Kensington Hair Salon")
   - Description
   - Phone and email
   - Verified status
   - Rating (4.0-5.0)
   - Review count (20-220)

4. **Location**: Calgary address with:
   - Real street addresses
   - Postal codes
   - GPS coordinates

5. **Services**: 3-5 services per provider with:
   - Names (Haircut, Massage, Plumbing, etc.)
   - Descriptions
   - Duration (30-120 minutes)
   - Pricing ($40-$200)

6. **Employees**: 2-5 employees per provider with:
   - Full names
   - Contact information
   - Position titles
   - Hire dates

7. **Employee Schedules**: Weekly schedules with:
   - Monday-Friday: 9 AM - 5 PM
   - Some work Saturdays: 10 AM - 2 PM

### Business Types Generated

The script rotates through these business types:
- Hair Salons
- Barber Shops
- Nail Salons
- Spas
- Massage Therapy
- Dental Clinics
- Physiotherapy
- Plumbing
- HVAC
- Auto Repair

### Calgary Neighborhoods

Providers are distributed across 30 Calgary neighborhoods including:
- Kensington
- Inglewood
- Mission
- Beltline
- 17th Avenue
- And many more...

### Troubleshooting

**Error: Missing environment variables**
- Make sure `SUPABASE_SERVICE_ROLE_KEY` is set
- Check that `NEXT_PUBLIC_SUPABASE_URL` or `EXPO_PUBLIC_SUPABASE_URL` is set

**Error: Auth user creation failed**
- Verify your service role key has proper permissions
- Check that email confirmations are disabled in Supabase (for test accounts)

**Error: Category not found**
- Run the `20240101000001_seed_categories.sql` migration first
- Ensure service categories exist in your database

**Rate limiting**
- The script includes 100ms delays between creations
- If you hit rate limits, increase the delay in the script

### Cleanup

**Good news!** The script automatically cleans up all previously seeded data before creating new accounts.

If you want to manually remove test data without running the script again, you can delete the auth users from the Supabase dashboard:
1. Go to Authentication → Users in your Supabase dashboard
2. Search for "@emptyslot.test"
3. Delete the test users (this will cascade delete all related data)

Or use the Supabase Auth Admin API to delete users programmatically.

### Notes

- All generated data is for testing purposes only
- Passwords follow the pattern: `Provider{N}!Test` where N is 1-200
- The script uses the Supabase Admin API for reliable user creation
- Employee schedules are realistic work hours
- Each provider has unique contact information
