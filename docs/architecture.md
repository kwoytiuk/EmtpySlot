# EmptySlot - Technical Architecture

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Client Layer                            │
├───────────────────────────┬─────────────────────────────────┤
│   Next.js Web App         │   React Native Mobile App       │
│   - Customer Portal       │   - Customer App                │
│   - Provider Dashboard    │   - Provider App (future)       │
│   - Admin Panel           │                                 │
└───────────────────────────┴─────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                  Shared Package Layer                        │
│  - API Client (Supabase)                                    │
│  - TypeScript Types & Schemas                               │
│  - Business Logic & Validators                              │
│  - Utility Functions                                        │
│  - Constants & Configuration                                │
└─────────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                    Backend Services                          │
├────────────────┬────────────────┬──────────────┬────────────┤
│   Supabase     │   Stripe       │   Mapbox     │  Resend    │
│   - Database   │   - Payments   │   - Maps     │  - Email   │
│   - Auth       │   - Invoicing  │   - Geocode  │            │
│   - Storage    │                │   - Search   │            │
│   - Realtime   │                │              │            │
│   - Functions  │                │              │            │
└────────────────┴────────────────┴──────────────┴────────────┘
```

## Technology Stack Details

### Frontend

**Web (Next.js 14)**
```
apps/web/
├── src/
│   ├── app/                    # App Router pages
│   │   ├── (customer)/        # Customer routes
│   │   │   ├── search/
│   │   │   ├── booking/
│   │   │   └── profile/
│   │   ├── (provider)/        # Provider dashboard
│   │   │   ├── dashboard/
│   │   │   ├── services/
│   │   │   ├── calendar/
│   │   │   └── analytics/
│   │   └── api/               # API routes
│   ├── components/
│   │   ├── ui/                # shadcn/ui components
│   │   ├── customer/
│   │   └── provider/
│   ├── lib/
│   │   ├── supabase.ts        # Supabase client
│   │   ├── stripe.ts          # Stripe client
│   │   └── utils.ts
│   └── hooks/
│       ├── useAuth.ts
│       ├── useGeolocation.ts
│       └── useBooking.ts
```

**Mobile (React Native + Expo)**
```
apps/mobile/
├── app/                       # Expo Router
│   ├── (tabs)/               # Tab navigation
│   │   ├── search/
│   │   ├── bookings/
│   │   └── profile/
│   └── booking/
├── src/
│   ├── components/
│   │   ├── ui/
│   │   ├── search/
│   │   └── booking/
│   └── hooks/
```

**Shared Package**
```
packages/shared/
├── src/
│   ├── types/
│   │   ├── database.types.ts  # Generated from Supabase
│   │   ├── api.types.ts
│   │   └── index.ts
│   ├── api/
│   │   ├── supabase-client.ts
│   │   ├── providers.ts
│   │   ├── bookings.ts
│   │   ├── search.ts
│   │   └── payments.ts
│   ├── validators/
│   │   ├── booking.schema.ts  # Zod schemas
│   │   ├── provider.schema.ts
│   │   └── review.schema.ts
│   ├── utils/
│   │   ├── date.ts
│   │   ├── pricing.ts
│   │   ├── distance.ts        # Haversine formula
│   │   └── formatting.ts
│   └── constants/
│       ├── categories.ts
│       ├── time-slots.ts
│       └── config.ts
```

### Backend (Supabase)

**Database**: PostgreSQL 15 with PostGIS extension
**Authentication**: Supabase Auth with email/OAuth
**Storage**: Profile images, service photos, provider logos
**Edge Functions**: Complex business logic
**Realtime**: Live availability updates

**Key Edge Functions:**
1. `calculate-available-slots` - Complex availability calculation
2. `process-booking` - Handle booking with payment
3. `send-notifications` - Email/SMS notifications
4. `update-provider-rating` - Recalculate ratings
5. `geospatial-search` - Optimized location search

### Third-Party Services

**Stripe**
- Payment processing
- Connect for provider payouts
- Subscription management (future)

**Mapbox**
- Interactive maps
- Geocoding addresses
- Reverse geocoding
- Distance matrix API

**Resend**
- Transactional emails
- Booking confirmations
- Reminders
- Promotional emails

**Expo Notifications**
- Push notifications for mobile
- Booking reminders
- Status updates

## Data Flow Examples

### 1. Customer Searches for Services

```
1. Customer enters location + service type
2. Frontend geocodes location (Mapbox)
3. Query Supabase with PostGIS:
   ST_DWithin(location, ST_MakePoint(lng, lat)::geography, radius)
4. Filter by category, availability, rating
5. Return sorted results with distance
6. Display on map + list view
```

### 2. Customer Books Appointment

```
1. Customer selects time slot
2. Frontend validates availability
3. Create Stripe Payment Intent
4. Process payment (deposit or full)
5. Call Edge Function: process-booking
   - Verify slot still available (race condition check)
   - Create appointment record
   - Update provider's availability
   - Send confirmation emails
   - Create notification records
6. Return booking confirmation
7. Real-time update to provider dashboard
```

### 3. Provider Updates Availability

```
1. Provider modifies schedule/exception
2. Update availability_schedules/exceptions table
3. Trigger recalculation of available slots
4. Real-time subscription updates customer search results
5. Send push notification to favorited customers
```

## Security Implementation

### Authentication Flow
```
1. User signs up/logs in via Supabase Auth
2. Create profile record (trigger)
3. Set user_type (customer/provider)
4. RLS policies automatically enforced
5. JWT token in httpOnly cookie (web)
6. Secure storage (mobile)
```

### RLS Policies
- All tables protected by Row Level Security
- Customers can only access their own data
- Providers can only modify their business
- Admins have elevated permissions

### API Security
- All requests authenticated
- Rate limiting on Edge Functions
- Input validation with Zod schemas
- SQL injection prevented by Supabase client
- XSS prevention with proper escaping

## Performance Optimizations

### Database
- Spatial indexes for geospatial queries
- Composite indexes on common query patterns
- Materialized views for complex aggregations
- Connection pooling (Supabase handles this)

### Frontend
- React Query for server state caching
- Optimistic updates for better UX
- Image optimization (Next.js Image, Expo Image)
- Code splitting and lazy loading
- Service Worker for offline support (PWA)

### Caching Strategy
- Static pages: ISR (Incremental Static Regeneration)
- Dynamic data: React Query with stale-while-revalidate
- CDN caching for public assets (Vercel Edge Network)
- Database query caching in Supabase

## Scalability Plan

### Phase 1: MVP (0-1K users)
- Supabase free tier
- Vercel hobby plan (free)
- Basic features
- Single region

### Phase 2: Growth (1K-10K users)
- Supabase Pro ($25/month)
- Vercel Pro ($20/month)
- Add monitoring (Sentry)
- Implement caching layers

### Phase 3: Scale (10K+ users)
- Database read replicas
- CDN for media assets
- Multiple regions
- Queue system for background jobs
- Dedicated Edge Functions

## Monitoring & Analytics

**Application Monitoring**
- Sentry for error tracking
- Vercel Analytics for web vitals
- Supabase Dashboard for DB metrics

**Business Analytics**
- Plausible or PostHog (privacy-friendly)
- Custom provider analytics dashboard
- Revenue tracking
- Booking conversion rates

## Development Workflow

```
1. Local Development
   - Supabase local instance (Docker)
   - Next.js dev server
   - Expo dev client

2. Preview Deployments
   - Vercel preview for web
   - Expo OTA updates for testing
   - Supabase staging project

3. Production
   - Vercel production
   - Expo EAS builds
   - Supabase production
   - Database migrations via Supabase CLI
```

## Compliance & Legal

- GDPR compliance (data export, deletion)
- Terms of Service
- Privacy Policy
- Cookie consent
- PCI DSS (handled by Stripe)
- Accessibility (WCAG 2.1 AA)
