/**
 * Seed 200 Providers with Authentication
 *
 * This script creates 200 test provider accounts with:
 * - Auth users with login credentials
 * - Provider profiles
 * - Locations
 * - Services
 * - Employees with schedules
 *
 * Run with: npm run seed:providers
 */

import 'dotenv/config'
import { createClient } from '@supabase/supabase-js'

const supabaseUrl = process.env.NEXT_PUBLIC_SUPABASE_URL || process.env.EXPO_PUBLIC_SUPABASE_URL || ''
const supabaseServiceKey = process.env.SUPABASE_SERVICE_ROLE_KEY || ''

if (!supabaseUrl || !supabaseServiceKey) {
  console.error('❌ Missing environment variables!')
  console.error('Required: SUPABASE_SERVICE_ROLE_KEY')
  console.error('Optional: NEXT_PUBLIC_SUPABASE_URL or EXPO_PUBLIC_SUPABASE_URL')
  console.error('\n📁 Make sure you have a .env file in the scripts directory with:')
  console.error('  NEXT_PUBLIC_SUPABASE_URL=https://your-project.supabase.co')
  console.error('  SUPABASE_SERVICE_ROLE_KEY=your-service-role-key')
  process.exit(1)
}

// Create admin client with service role key
const supabase = createClient(supabaseUrl, supabaseServiceKey, {
  auth: {
    autoRefreshToken: false,
    persistSession: false
  }
})

// Business types and their associated categories
const businessTypes = [
  { type: 'Hair Salon', category: 'hair-salon', services: ['Haircut', 'Color', 'Highlights', 'Balayage', 'Keratin Treatment'] },
  { type: 'Barber Shop', category: 'barber-shop', services: ['Men\'s Haircut', 'Beard Trim', 'Hot Towel Shave', 'Fade'] },
  { type: 'Nail Salon', category: 'nail-salon', services: ['Manicure', 'Pedicure', 'Gel Nails', 'Acrylic Nails', 'Nail Art'] },
  { type: 'Spa', category: 'spa', services: ['Facial', 'Body Massage', 'Body Scrub', 'Waxing'] },
  { type: 'Massage Therapy', category: 'massage', services: ['Swedish Massage', 'Deep Tissue', 'Sports Massage', 'Hot Stone'] },
  { type: 'Dental Clinic', category: 'dentist', services: ['Cleaning', 'Checkup', 'Filling', 'Whitening', 'Crown'] },
  { type: 'Physiotherapy', category: 'physiotherapy', services: ['Initial Assessment', 'Treatment Session', 'Acupuncture', 'Dry Needling'] },
  { type: 'Plumbing', category: 'plumbing', services: ['Drain Cleaning', 'Pipe Repair', 'Installation', 'Emergency Service'] },
  { type: 'HVAC', category: 'hvac', services: ['AC Repair', 'Furnace Service', 'Installation', 'Maintenance'] },
  { type: 'Car Repair', category: 'auto-repair', services: ['Oil Change', 'Brake Service', 'Tire Rotation', 'Diagnostic'] },
]

// Calgary neighborhoods
const neighborhoods = [
  'Kensington', 'Inglewood', 'Mission', 'Beltline', 'Eau Claire',
  '17th Avenue', 'Marda Loop', 'Bridgeland', 'Hillhurst', 'Sunnyside',
  'Ramsay', 'Victoria Park', 'East Village', 'Chinatown', 'Downtown',
  'Mount Royal', 'Elbow Park', 'Altadore', 'Bankview', 'Killarney',
  'Parkdale', 'West Hillhurst', 'Hounsfield Heights', 'Crescent Heights', 'Tuxedo Park',
  'Forest Lawn', 'Dover', 'Renfrew', 'Mayland Heights', 'Albert Park'
]

// Generate realistic Calgary addresses
const generateAddress = (index: number) => {
  const streets = ['Avenue', 'Street', 'Road', 'Drive', 'Trail', 'Way', 'Boulevard']
  const directions = ['NW', 'NE', 'SW', 'SE']
  const streetNum = 100 + (index * 13) % 9000
  const streetName = (index % 50) + 1
  const street = streets[index % streets.length]
  const direction = directions[index % directions.length]

  return {
    address: `${streetNum} ${streetName}${street.includes('Avenue') ? 'th' : ''} ${street} ${direction}`,
    postalCode: `T${2 + Math.floor(index / 100)}${String.fromCharCode(65 + (index % 26))} ${Math.floor(Math.random() * 9)}${String.fromCharCode(65 + ((index * 7) % 26))}${Math.floor(Math.random() * 9)}`,
    // Calgary coordinates roughly: lat 50.9-51.2, lng -114.2 to -113.9
    latitude: 50.9 + (Math.random() * 0.3),
    longitude: -114.2 + (Math.random() * 0.3)
  }
}

// Generate random phone number
const generatePhone = (index: number) => {
  return `(403) ${500 + (index % 499)}-${String(1000 + (index * 7) % 9000).padStart(4, '0')}`
}

// Generate employee names
const firstNames = ['Alex', 'Jordan', 'Taylor', 'Morgan', 'Casey', 'Riley', 'Quinn', 'Avery', 'Jamie', 'Drew', 'Cameron', 'Sage', 'Dakota', 'Phoenix', 'River']
const lastNames = ['Smith', 'Johnson', 'Williams', 'Brown', 'Jones', 'Garcia', 'Miller', 'Davis', 'Rodriguez', 'Martinez', 'Hernandez', 'Lopez', 'Gonzalez', 'Wilson', 'Anderson']

async function createProviders() {
  console.log('🚀 Starting seed process for 200 providers...\n')

  // Get category IDs
  const { data: categories, error: catError } = await supabase
    .from('service_categories')
    .select('id, slug, name')

  if (catError || !categories) {
    console.error('❌ Error fetching categories:', catError)
    return
  }

  const categoryMap = new Map(categories.map(c => [c.slug, c.id]))

  let successCount = 0
  let failCount = 0

  for (let i = 1; i <= 200; i++) {
    try {
      const businessType = businessTypes[i % businessTypes.length]
      const neighborhood = neighborhoods[i % neighborhoods.length]
      const address = generateAddress(i)
      const phone = generatePhone(i)

      const businessName = `${neighborhood} ${businessType.type} ${i > businessTypes.length * neighborhoods.length ? `#${Math.floor(i / (businessTypes.length * neighborhoods.length)) + 1}` : ''}`
      const email = `provider${i}@emptyslot.test`
      const password = `Provider${i}!Test` // Strong password

      console.log(`Creating ${i}/200: ${businessName}`)

      // 1. Create auth user
      const { data: authData, error: authError } = await supabase.auth.admin.createUser({
        email,
        password,
        email_confirm: true,
        user_metadata: {
          full_name: businessName,
          user_type: 'provider'
        }
      })

      if (authError || !authData.user) {
        console.error(`  ❌ Auth error for ${businessName}:`, authError?.message)
        failCount++
        continue
      }

      const userId = authData.user.id

      // 2. Create profile
      const { error: profileError } = await supabase
        .from('profiles')
        .insert({
          id: userId,
          full_name: businessName,
          phone,
          user_type: 'provider'
        })

      if (profileError) {
        console.error(`  ❌ Profile error:`, profileError.message)
        failCount++
        continue
      }

      // 3. Create provider
      const ratingAverage = 4.0 + (Math.random() * 1.0)
      const ratingCount = Math.floor(20 + (Math.random() * 200))

      const { data: provider, error: providerError } = await supabase
        .from('providers')
        .insert({
          user_id: userId,
          business_name: businessName,
          description: `Professional ${businessType.type.toLowerCase()} services in ${neighborhood}. Serving Calgary since ${2010 + (i % 14)}.`,
          phone,
          email,
          verified: true,
          rating_average: Number(ratingAverage.toFixed(1)),
          rating_count: ratingCount
        })
        .select()
        .single()

      if (providerError || !provider) {
        console.error(`  ❌ Provider error:`, providerError?.message)
        failCount++
        continue
      }

      // 4. Create location
      const { error: locationError } = await supabase
        .from('provider_locations')
        .insert({
          provider_id: provider.id,
          name: 'Main Location',
          address_line1: address.address,
          city: 'Calgary',
          state_province: 'AB',
          postal_code: address.postalCode,
          country: 'Canada',
          latitude: address.latitude,
          longitude: address.longitude,
          is_primary: true
        })

      if (locationError) {
        console.error(`  ❌ Location error:`, locationError.message)
      }

      // 5. Create services
      const categoryId = categoryMap.get(businessType.category)
      if (categoryId) {
        const servicesToCreate = businessType.services.slice(0, 3 + (i % 3)) // 3-5 services per provider

        for (const serviceName of servicesToCreate) {
          const duration = [30, 45, 60, 90, 120][Math.floor(Math.random() * 5)]
          const price = Math.floor(40 + (Math.random() * 160))

          await supabase
            .from('services')
            .insert({
              provider_id: provider.id,
              category_id: categoryId,
              name: serviceName,
              description: `Professional ${serviceName.toLowerCase()} service`,
              duration_minutes: duration,
              price,
              is_active: true
            })
        }
      }

      // 6. Create employees (2-5 per provider)
      const numEmployees = 2 + (i % 4)
      for (let e = 0; e < numEmployees; e++) {
        const firstName = firstNames[(i * numEmployees + e) % firstNames.length]
        const lastName = lastNames[(i * numEmployees + e) % lastNames.length]

        const { data: employee, error: empError } = await supabase
          .from('employees')
          .insert({
            provider_id: provider.id,
            first_name: firstName,
            last_name: lastName,
            email: `${firstName.toLowerCase()}.${lastName.toLowerCase()}${i}@emptyslot.test`,
            phone: generatePhone(i * 1000 + e),
            position: ['Senior', 'Junior', 'Lead', ''][e % 4] + ' ' + businessType.type.split(' ')[0] + ' Specialist',
            is_active: true,
            hire_date: new Date(2020 + (i % 5), (i % 12), 1).toISOString().split('T')[0]
          })
          .select()
          .single()

        if (employee && !empError) {
          // Create weekly schedule for employee
          const schedules = []
          for (let day = 1; day <= 5; day++) { // Monday to Friday
            schedules.push({
              employee_id: employee.id,
              day_of_week: day,
              start_time: '09:00:00',
              end_time: '17:00:00',
              is_available: true
            })
          }

          // Some employees work Saturdays
          if (e % 2 === 0) {
            schedules.push({
              employee_id: employee.id,
              day_of_week: 6,
              start_time: '10:00:00',
              end_time: '14:00:00',
              is_available: true
            })
          }

          await supabase.from('employee_schedules').insert(schedules)
        }
      }

      console.log(`  ✅ Created successfully (User: ${email}, Password: ${password})`)
      successCount++

      // Rate limit - small delay between creations
      await new Promise(resolve => setTimeout(resolve, 100))

    } catch (error: any) {
      console.error(`  ❌ Error creating provider ${i}:`, error.message)
      failCount++
    }
  }

  console.log('\n📊 Seed Complete!')
  console.log(`✅ Success: ${successCount}`)
  console.log(`❌ Failed: ${failCount}`)
  console.log(`\n🔑 Test Credentials:`)
  console.log(`   Email: provider1@emptyslot.test`)
  console.log(`   Password: Provider1!Test`)
  console.log(`   (Pattern: provider[1-200]@emptyslot.test / Provider[1-200]!Test)`)
}

// Run the seed
createProviders()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error('Fatal error:', error)
    process.exit(1)
  })
