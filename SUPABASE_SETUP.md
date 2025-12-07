# Supabase Setup Guide

## Step 1: Create Supabase Account

1. Go to [https://supabase.com](https://supabase.com)
2. Click "Start your project"
3. Sign up with GitHub, Google, or email

## Step 2: Create New Project

1. Click "New Project"
2. Choose your organization (or create one)
3. Fill in project details:
   - **Name**: emptyslot
   - **Database Password**: (generate a strong password and save it!)
   - **Region**: Choose closest to your users
   - **Pricing Plan**: Free (perfect for development)
4. Click "Create new project"
5. Wait 2-3 minutes for project to provision

## Step 3: Get Your API Keys

1. In your Supabase dashboard, go to **Settings** → **API**
2. You'll see:
   - **Project URL**: `https://xxxxx.supabase.co`
   - **anon/public key**: `eyJhbG...` (safe to use in frontend)
   - **service_role key**: `eyJhbG...` (SECRET - never expose in frontend!)

## Step 4: Configure Environment Variables

1. Copy `.env.local.example` to `.env.local`:
   ```bash
   cp .env.local.example .env.local
   ```

2. Fill in your Supabase credentials:
   ```bash
   NEXT_PUBLIC_SUPABASE_URL=https://your-project.supabase.co
   NEXT_PUBLIC_SUPABASE_ANON_KEY=eyJhbG...
   SUPABASE_SERVICE_ROLE_KEY=eyJhbG...
   ```

## Step 5: Enable PostGIS Extension

1. In Supabase dashboard, go to **Database** → **Extensions**
2. Search for "postgis"
3. Enable the **postgis** extension
4. This enables geospatial queries for location-based search

## Step 6: Run Database Migrations

After setting up your environment variables, run:

```bash
npm install
npm run db:migrate
```

This will create all the necessary tables, indexes, and security policies.

## Step 7: Enable Authentication Providers

1. Go to **Authentication** → **Providers**
2. Enable **Email** (enabled by default)
3. Optional: Enable **Google** for OAuth
   - Follow the Google OAuth setup guide
   - Add credentials to Supabase

## Step 8: Configure Storage

1. Go to **Storage**
2. Create buckets:
   - `avatars` (for user profile pictures)
   - `provider-images` (for business photos)
   - `service-images` (for service photos)

3. Set bucket policies to public read:
   ```sql
   -- Run in SQL Editor
   INSERT INTO storage.buckets (id, name, public)
   VALUES ('avatars', 'avatars', true);

   INSERT INTO storage.buckets (id, name, public)
   VALUES ('provider-images', 'provider-images', true);

   INSERT INTO storage.buckets (id, name, public)
   VALUES ('service-images', 'service-images', true);
   ```

## Verification

Test your connection:
```bash
npm run dev:web
```

Open http://localhost:3000 and check the browser console. You should see no Supabase connection errors.

## Next Steps

Once Supabase is set up:
1. Database schema will be automatically created
2. You can start using authentication
3. Begin building the app features

## Troubleshooting

**"Invalid API key" error**
- Double-check your `.env.local` file
- Make sure you're using the `anon` key for `NEXT_PUBLIC_SUPABASE_ANON_KEY`
- Restart your dev server after changing env vars

**"Failed to fetch" error**
- Check your Project URL is correct
- Ensure your Supabase project is not paused (free tier pauses after 1 week of inactivity)

**Database connection issues**
- Wait a few minutes after project creation
- Check Supabase dashboard for any service issues
