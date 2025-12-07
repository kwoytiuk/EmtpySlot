# EmptySlot - Best Practices & Standards

## Code Quality Standards

### TypeScript
- **Strict mode enabled**: No implicit any
- **Type everything**: Avoid using `any`, use `unknown` when needed
- **Generate types from Supabase**: `supabase gen types typescript`
- **Zod for runtime validation**: Validate external data
- **Shared types package**: Single source of truth

### Code Style
- **ESLint + Prettier**: Automated formatting
- **Consistent naming**:
  - Components: PascalCase
  - Functions/variables: camelCase
  - Constants: UPPER_SNAKE_CASE
  - Files: kebab-case for utilities, PascalCase for components
- **File organization**: Group by feature, not type

### Component Structure
```typescript
// 1. Imports
import { useState } from 'react'
import { Button } from '@/components/ui/button'
import type { Provider } from 'shared'

// 2. Types
interface ProviderCardProps {
  provider: Provider
  onBook: (id: string) => void
}

// 3. Component
export function ProviderCard({ provider, onBook }: ProviderCardProps) {
  // Hooks first
  const [isLoading, setIsLoading] = useState(false)

  // Event handlers
  const handleBook = () => {
    setIsLoading(true)
    onBook(provider.id)
  }

  // Render
  return (
    <div>
      {/* JSX */}
    </div>
  )
}
```

## Security Best Practices

### Authentication
- **Never store passwords**: Use Supabase Auth
- **JWT tokens**: httpOnly cookies for web, secure storage for mobile
- **Session management**: Auto-refresh tokens
- **OAuth providers**: Google, Apple, Facebook for easy onboarding
- **Email verification**: Required for providers

### Data Protection
- **Row Level Security (RLS)**: Every table must have RLS policies
- **Least privilege**: Users can only access what they need
- **Sensitive data**: Encrypt at rest (Supabase handles this)
- **PII handling**: GDPR compliance (data export, deletion)
- **SQL injection**: Use parameterized queries (Supabase client does this)

### API Security
- **Rate limiting**: Prevent abuse
- **Input validation**: Validate all user input with Zod
- **CORS**: Restrict to allowed origins
- **CSRF protection**: Token-based protection
- **Content Security Policy**: XSS prevention

### Payment Security
- **Never store card details**: Use Stripe tokens
- **PCI DSS**: Stripe handles compliance
- **Webhook verification**: Verify Stripe webhook signatures
- **Refund policies**: Clear and enforced

## Performance Best Practices

### Database
```typescript
// ✅ Good: Specific columns, indexed where clause
const { data } = await supabase
  .from('providers')
  .select('id, business_name, rating_average')
  .eq('verified', true)
  .limit(20)

// ❌ Bad: Select *, no limit, unindexed column
const { data } = await supabase
  .from('providers')
  .select('*')
  .eq('description', 'some text')
```

### Query Optimization
- **Use indexes**: On foreign keys, search fields, status fields
- **Limit results**: Always paginate
- **Avoid N+1 queries**: Use joins or batch fetching
- **Materialized views**: For complex aggregations
- **Cache frequently accessed data**: React Query with appropriate staleTime

### Frontend Performance
```typescript
// ✅ Good: Lazy loading
const ProviderDashboard = lazy(() => import('./ProviderDashboard'))

// ✅ Good: Memoization
const expensiveCalculation = useMemo(() =>
  calculateAvailableSlots(schedule, bookings),
  [schedule, bookings]
)

// ✅ Good: Debouncing search
const debouncedSearch = useDebounce(searchTerm, 300)
```

### Image Optimization
- **Next.js Image**: Automatic optimization for web
- **Expo Image**: For mobile
- **Responsive images**: Multiple sizes
- **WebP format**: With fallbacks
- **Lazy loading**: Below the fold images
- **CDN**: Serve from edge locations

## UX Best Practices

### Loading States
```typescript
// ✅ Good: Clear loading states
{isLoading && <ProviderCardSkeleton />}
{error && <ErrorMessage error={error} />}
{data && <ProviderCard provider={data} />}

// ❌ Bad: No loading feedback
{data && <ProviderCard provider={data} />}
```

### Error Handling
- **User-friendly messages**: No stack traces to users
- **Actionable errors**: Tell users how to fix it
- **Retry mechanisms**: For transient failures
- **Fallback UI**: When things break
- **Log errors**: Sentry for monitoring

### Form Validation
```typescript
// ✅ Good: Real-time validation with Zod + React Hook Form
const bookingSchema = z.object({
  date: z.date().min(new Date(), 'Date must be in the future'),
  serviceId: z.string().uuid(),
  notes: z.string().max(500).optional()
})

type BookingForm = z.infer<typeof bookingSchema>

const { register, handleSubmit, formState: { errors } } = useForm<BookingForm>({
  resolver: zodResolver(bookingSchema)
})
```

### Accessibility
- **Semantic HTML**: Use proper elements
- **ARIA labels**: For dynamic content
- **Keyboard navigation**: All interactive elements
- **Color contrast**: WCAG AA minimum (4.5:1)
- **Focus indicators**: Visible focus states
- **Screen reader testing**: Test with VoiceOver/NVDA
- **Alt text**: Descriptive alt text for images

## Testing Strategy

### Unit Tests
```typescript
// Test business logic
describe('calculateAvailableSlots', () => {
  it('should exclude booked slots', () => {
    const schedule = createMockSchedule()
    const bookings = [createMockBooking({ startTime: '10:00' })]

    const slots = calculateAvailableSlots(schedule, bookings)

    expect(slots).not.toContainEqual(expect.objectContaining({
      startTime: '10:00'
    }))
  })
})
```

### Integration Tests
```typescript
// Test API interactions
describe('Booking flow', () => {
  it('should create booking and send confirmation', async () => {
    const booking = await createBooking(mockBookingData)

    expect(booking.status).toBe('confirmed')
    expect(mockEmailService.send).toHaveBeenCalledWith(
      expect.objectContaining({ template: 'booking-confirmation' })
    )
  })
})
```

### E2E Tests
```typescript
// Test critical user flows
test('Customer can search and book appointment', async ({ page }) => {
  await page.goto('/search')
  await page.fill('[name="location"]', 'San Francisco')
  await page.click('[data-testid="search-button"]')

  await page.click('[data-testid="provider-card"]:first-child')
  await page.click('[data-testid="time-slot"]:first-child')
  await page.fill('[name="notes"]', 'Please call before arriving')
  await page.click('[data-testid="confirm-booking"]')

  await expect(page.locator('[data-testid="booking-confirmation"]')).toBeVisible()
})
```

## Git Workflow

### Branching Strategy
```
main (production)
  └── develop (staging)
      ├── feature/customer-search
      ├── feature/provider-dashboard
      └── fix/booking-validation
```

### Commit Messages
```
feat: add geospatial search with PostGIS
fix: resolve booking time slot conflicts
docs: update API documentation
style: format provider dashboard components
refactor: extract booking logic to shared package
test: add unit tests for availability calculator
chore: update dependencies
```

### Pull Request Template
```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed

## Screenshots (if applicable)

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No new warnings
- [ ] Tests pass locally
```

## Monitoring & Debugging

### Logging Best Practices
```typescript
// ✅ Good: Structured logging
logger.info('Booking created', {
  bookingId: booking.id,
  customerId: booking.customerId,
  providerId: booking.providerId,
  timestamp: new Date().toISOString()
})

// ❌ Bad: Unstructured logging
console.log('booking created')
```

### Error Tracking
- **Sentry**: For production error tracking
- **Source maps**: For debugging minified code
- **User context**: Attach user ID to errors
- **Breadcrumbs**: Track user actions before error
- **Release tracking**: Tag errors by version

### Performance Monitoring
- **Web Vitals**: Track Core Web Vitals
- **API response times**: Monitor endpoint performance
- **Database query times**: Identify slow queries
- **Real User Monitoring**: Actual user experience
- **Synthetic monitoring**: Automated checks

## Environment Management

### Environment Variables
```bash
# .env.local (never commit!)
NEXT_PUBLIC_SUPABASE_URL=https://xxx.supabase.co
NEXT_PUBLIC_SUPABASE_ANON_KEY=xxx
SUPABASE_SERVICE_ROLE_KEY=xxx
STRIPE_SECRET_KEY=sk_test_xxx
STRIPE_WEBHOOK_SECRET=whsec_xxx
NEXT_PUBLIC_MAPBOX_TOKEN=pk.xxx
RESEND_API_KEY=re_xxx
```

### Configuration
```typescript
// shared/src/config.ts
export const config = {
  supabase: {
    url: process.env.NEXT_PUBLIC_SUPABASE_URL!,
    anonKey: process.env.NEXT_PUBLIC_SUPABASE_ANON_KEY!,
  },
  stripe: {
    publicKey: process.env.NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY!,
    secretKey: process.env.STRIPE_SECRET_KEY!,
  },
  mapbox: {
    token: process.env.NEXT_PUBLIC_MAPBOX_TOKEN!,
  },
  app: {
    name: 'EmptySlot',
    url: process.env.NEXT_PUBLIC_APP_URL || 'http://localhost:3000',
  }
} as const

// Validate at startup
function validateConfig() {
  const required = [
    'NEXT_PUBLIC_SUPABASE_URL',
    'NEXT_PUBLIC_SUPABASE_ANON_KEY',
    // ... other required vars
  ]

  const missing = required.filter(key => !process.env[key])

  if (missing.length > 0) {
    throw new Error(`Missing required environment variables: ${missing.join(', ')}`)
  }
}
```

## Deployment Checklist

### Pre-Deploy
- [ ] All tests passing
- [ ] No console errors
- [ ] Environment variables configured
- [ ] Database migrations run
- [ ] Build succeeds locally
- [ ] Performance benchmarks met
- [ ] Security scan passed

### Deploy
- [ ] Deploy to staging first
- [ ] Smoke test on staging
- [ ] Run E2E tests
- [ ] Deploy to production
- [ ] Verify deployment
- [ ] Monitor error rates
- [ ] Check performance metrics

### Post-Deploy
- [ ] Verify critical flows
- [ ] Check error tracking
- [ ] Monitor server resources
- [ ] Review user feedback
- [ ] Document any issues
- [ ] Plan hotfixes if needed

## Documentation Standards

- **README**: Clear setup instructions
- **API docs**: Document all endpoints
- **Component docs**: Storybook for UI components
- **Architecture decisions**: ADR (Architecture Decision Records)
- **Inline comments**: Explain why, not what
- **Type documentation**: JSDoc for complex types
- **Database schema**: Keep schema docs updated
