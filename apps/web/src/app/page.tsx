import Link from 'next/link';
import { categoriesApi, providersApi } from 'shared';

export default async function Home() {
  const categories = await categoriesApi.getCategories();
  const { providers } = await providersApi.searchProviders({
    latitude: 51.0447, // Calgary
    longitude: -114.0719,
    radius: 50,
  });

  const featuredProviders = providers?.slice(0, 6) || [];

  return (
    <div className="min-h-screen">
      {/* Navigation */}
      <nav className="absolute top-0 left-0 right-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <div className="flex justify-between items-center">
            <Link href="/" className="text-2xl font-black text-white">
              EmptySlot
            </Link>
            <div className="flex items-center gap-6">
              <Link href="/search" className="text-white hover:text-blue-200 font-medium">
                Find Services
              </Link>
              <Link href="/auth/login" className="text-white hover:text-blue-200 font-medium">
                Log In
              </Link>
              <Link
                href="/auth/signup"
                className="bg-white text-blue-600 px-6 py-2 rounded-full font-bold hover:bg-blue-50 transition-colors"
              >
                Sign Up
              </Link>
            </div>
          </div>
        </div>
      </nav>

      {/* Hero Section - Provocative */}
      <section className="relative bg-gradient-to-br from-violet-600 via-blue-600 to-cyan-600 text-white overflow-hidden">
        <div className="absolute inset-0 bg-[url('/grid.svg')] opacity-20"></div>
        <div className="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent"></div>

        <div className="relative max-w-7xl mx-auto px-4 py-32 sm:px-6 lg:px-8 lg:py-40">
          <div className="text-center">
            <div className="inline-block mb-6 px-4 py-2 bg-white/20 backdrop-blur-sm rounded-full text-sm font-semibold">
              ⚡ Stop wasting time in waiting rooms
            </div>

            <h1 className="text-5xl md:text-6xl lg:text-8xl font-black tracking-tight mb-6 leading-tight">
              Life's Too Short<br />
              <span className="bg-gradient-to-r from-yellow-300 via-pink-300 to-purple-300 bg-clip-text text-transparent">
                For Empty Slots
              </span>
            </h1>

            <p className="text-xl md:text-2xl text-blue-100 max-w-3xl mx-auto mb-12 leading-relaxed">
              That haircut you need <span className="font-bold text-white">today</span>.
              That burst pipe that won't wait.
              That nail appointment for <span className="font-bold text-white">tonight's date</span>.
              <br />
              <span className="text-2xl font-bold text-white mt-2 block">Book it. Now.</span>
            </p>

            {/* Search Bar */}
            <div className="max-w-4xl mx-auto mb-12">
              <div className="bg-white/95 backdrop-blur-md rounded-3xl shadow-2xl p-3">
                <div className="flex flex-col md:flex-row gap-3">
                  <input
                    type="text"
                    placeholder="What do you need? (haircut, plumber, dentist...)"
                    className="flex-1 px-8 py-5 text-gray-900 placeholder-gray-500 rounded-2xl focus:outline-none focus:ring-4 focus:ring-purple-500 font-medium text-lg"
                  />
                  <input
                    type="text"
                    placeholder="📍 Calgary, AB"
                    className="px-8 py-5 text-gray-900 placeholder-gray-500 rounded-2xl focus:outline-none focus:ring-4 focus:ring-purple-500 md:w-72 font-medium text-lg"
                  />
                  <Link
                    href="/search"
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-black px-12 py-5 rounded-2xl transition-all text-lg shadow-lg hover:shadow-xl hover:scale-105"
                  >
                    Find Slots →
                  </Link>
                </div>
              </div>
            </div>

            {/* Stats */}
            <div className="grid grid-cols-3 gap-8 max-w-3xl mx-auto">
              <div>
                <div className="text-4xl font-black text-yellow-300">10K+</div>
                <div className="text-blue-200 font-medium">Bookings Today</div>
              </div>
              <div>
                <div className="text-4xl font-black text-yellow-300">&lt;15min</div>
                <div className="text-blue-200 font-medium">Avg. Response</div>
              </div>
              <div>
                <div className="text-4xl font-black text-yellow-300">4.9★</div>
                <div className="text-blue-200 font-medium">User Rating</div>
              </div>
            </div>
          </div>
        </div>

        {/* Wave Divider */}
        <div className="absolute bottom-0 left-0 right-0">
          <svg viewBox="0 0 1440 120" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M0 120L60 105C120 90 240 60 360 45C480 30 600 30 720 37.5C840 45 960 60 1080 67.5C1200 75 1320 75 1380 75L1440 75V120H1380C1320 120 1200 120 1080 120C960 120 840 120 720 120C600 120 480 120 360 120C240 120 120 120 60 120H0Z" fill="white"/>
          </svg>
        </div>
      </section>

      {/* Social Proof Banner */}
      <section className="py-8 bg-white border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-center items-center gap-12 flex-wrap text-gray-600 text-sm font-medium">
            <div>✓ No subscription fees</div>
            <div>✓ Instant confirmation</div>
            <div>✓ Real-time availability</div>
            <div>✓ Secure payments</div>
          </div>
        </div>
      </section>

      {/* Problem-Solution Section */}
      <section className="py-20 bg-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid lg:grid-cols-2 gap-16 items-center">
            <div>
              <h2 className="text-5xl font-black text-gray-900 mb-6 leading-tight">
                Remember calling 47 places trying to find an appointment?
              </h2>
              <p className="text-xl text-gray-600 mb-8 leading-relaxed">
                Yeah, we don't do that here. EmptySlot shows you <span className="font-bold text-gray-900">exactly who's available</span>,
                <span className="font-bold text-gray-900"> exactly when</span>, and lets you book it in 60 seconds flat.
              </p>
              <div className="space-y-4">
                <div className="flex items-start gap-4">
                  <div className="text-3xl">⏰</div>
                  <div>
                    <h3 className="font-bold text-xl text-gray-900 mb-1">Same-Day Bookings</h3>
                    <p className="text-gray-600">Emergency haircut before that interview? We got you.</p>
                  </div>
                </div>
                <div className="flex items-start gap-4">
                  <div className="text-3xl">💰</div>
                  <div>
                    <h3 className="font-bold text-xl text-gray-900 mb-1">Transparent Pricing</h3>
                    <p className="text-gray-600">See prices upfront. No surprises, no hidden fees.</p>
                  </div>
                </div>
                <div className="flex items-start gap-4">
                  <div className="text-3xl">⭐</div>
                  <div>
                    <h3 className="font-bold text-xl text-gray-900 mb-1">Real Reviews</h3>
                    <p className="text-gray-600">From real people who actually showed up.</p>
                  </div>
                </div>
              </div>
            </div>

            <div className="relative">
              <div className="bg-gradient-to-br from-purple-500 to-pink-500 rounded-3xl p-8 shadow-2xl">
                <div className="bg-white rounded-2xl p-6 mb-4">
                  <div className="flex items-center gap-4 mb-4">
                    <div className="w-16 h-16 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full"></div>
                    <div>
                      <div className="font-bold text-lg">Kensington Hair Studio</div>
                      <div className="text-amber-500">★★★★★ 4.8 (127 reviews)</div>
                    </div>
                  </div>
                  <div className="text-2xl font-black text-gray-900 mb-4">Available Today:</div>
                  <div className="grid grid-cols-3 gap-2">
                    <div className="bg-green-100 text-green-800 font-bold py-3 px-4 rounded-xl text-center">
                      2:00 PM
                    </div>
                    <div className="bg-green-100 text-green-800 font-bold py-3 px-4 rounded-xl text-center">
                      3:30 PM
                    </div>
                    <div className="bg-green-100 text-green-800 font-bold py-3 px-4 rounded-xl text-center">
                      5:00 PM
                    </div>
                  </div>
                </div>
                <div className="text-white text-center font-bold">
                  🎉 3 people booked in the last hour
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Categories - Redesigned */}
      <section className="py-20 bg-gradient-to-b from-gray-50 to-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-5xl font-black text-gray-900 mb-4">
              Whatever You Need.<br />
              <span className="bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
                Whenever You Need It.
              </span>
            </h2>
            <p className="text-xl text-gray-600">
              From beauty to business, we've got Calgary covered.
            </p>
          </div>

          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-6">
            {categories.slice(0, 12).map((category) => (
              <Link
                key={category.id}
                href={`/search?category=${category.id}`}
                className="group bg-white rounded-2xl p-8 text-center hover:shadow-2xl transition-all border-2 border-gray-100 hover:border-purple-500 hover:scale-105"
              >
                <div className="text-5xl mb-4 group-hover:scale-110 transition-transform">
                  {getCategoryIcon(category.slug)}
                </div>
                <h3 className="font-bold text-gray-900 group-hover:text-purple-600 transition-colors">
                  {category.name}
                </h3>
              </Link>
            ))}
          </div>
        </div>
      </section>

      {/* Featured Providers */}
      {featuredProviders.length > 0 && (
        <section className="py-20 bg-white">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="text-center mb-16">
              <div className="inline-block mb-4 px-4 py-2 bg-red-100 text-red-600 rounded-full text-sm font-bold">
                🔥 HOT RIGHT NOW
              </div>
              <h2 className="text-5xl font-black text-gray-900 mb-4">
                Available in Calgary Today
              </h2>
              <p className="text-xl text-gray-600">
                These providers have open slots right now. First come, first served.
              </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
              {featuredProviders.map((provider) => (
                <Link
                  key={provider.id}
                  href={`/providers/${provider.id}`}
                  className="group bg-white rounded-3xl border-2 border-gray-200 overflow-hidden hover:shadow-2xl hover:border-purple-500 transition-all hover:scale-105"
                >
                  {/* Provider Image Placeholder */}
                  <div className="h-56 bg-gradient-to-br from-blue-500 via-purple-500 to-pink-500 relative overflow-hidden">
                    <div className="absolute inset-0 bg-black/20 group-hover:bg-black/10 transition-colors"></div>
                    <div className="absolute top-4 right-4 bg-white/90 backdrop-blur-sm px-3 py-1 rounded-full text-sm font-bold text-gray-900">
                      🔥 Available Now
                    </div>
                  </div>

                  <div className="p-6">
                    <div className="flex items-start justify-between mb-3">
                      <h3 className="font-black text-xl text-gray-900 group-hover:text-purple-600 transition-colors">
                        {provider.business_name}
                      </h3>
                      {provider.rating_average && (
                        <div className="flex items-center bg-amber-100 px-3 py-1 rounded-full">
                          <span className="text-amber-600 font-bold">
                            ⭐ {provider.rating_average.toFixed(1)}
                          </span>
                        </div>
                      )}
                    </div>

                    <p className="text-gray-600 mb-4 line-clamp-2">
                      {provider.description}
                    </p>

                    {provider.provider_locations?.[0] && (
                      <p className="text-sm text-gray-500 flex items-center mb-4">
                        <span className="mr-2">📍</span>
                        {provider.provider_locations[0].city},{' '}
                        {provider.provider_locations[0].state_province}
                      </p>
                    )}

                    {provider.services && provider.services.length > 0 && (
                      <div className="pt-4 border-t-2 border-gray-100 flex items-center justify-between">
                        <div>
                          <p className="text-xs text-gray-500 mb-1">Starting from</p>
                          <p className="text-3xl font-black text-gray-900">
                            ${Math.min(...provider.services.map((s) => s.price))}
                          </p>
                        </div>
                        <div className="bg-gradient-to-r from-purple-600 to-pink-600 text-white font-bold px-6 py-3 rounded-xl group-hover:shadow-lg transition-shadow">
                          Book Now →
                        </div>
                      </div>
                    )}
                  </div>
                </Link>
              ))}
            </div>

            <div className="text-center mt-12">
              <Link
                href="/search"
                className="inline-block bg-gradient-to-r from-purple-600 to-pink-600 text-white font-black px-12 py-5 rounded-2xl hover:shadow-2xl transition-all text-lg hover:scale-105"
              >
                See All Available Slots →
              </Link>
            </div>
          </div>
        </section>
      )}

      {/* How It Works - Redesigned */}
      <section className="py-20 bg-gradient-to-br from-purple-50 via-pink-50 to-blue-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-5xl font-black text-gray-900 mb-4">
              So Simple, It's Stupid
            </h2>
            <p className="text-xl text-gray-600">
              Three clicks. That's it. That's the whole thing.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-12">
            <div className="text-center">
              <div className="w-24 h-24 bg-gradient-to-br from-purple-600 to-pink-600 rounded-3xl flex items-center justify-center text-white text-4xl font-black mx-auto mb-6 shadow-xl">
                1
              </div>
              <h3 className="text-2xl font-black text-gray-900 mb-3">
                Search What You Need
              </h3>
              <p className="text-gray-600 text-lg">
                Type it in. Hit enter. See who's available. Revolutionary stuff.
              </p>
            </div>

            <div className="text-center">
              <div className="w-24 h-24 bg-gradient-to-br from-blue-600 to-purple-600 rounded-3xl flex items-center justify-center text-white text-4xl font-black mx-auto mb-6 shadow-xl">
                2
              </div>
              <h3 className="text-2xl font-black text-gray-900 mb-3">
                Pick Your Slot
              </h3>
              <p className="text-gray-600 text-lg">
                Click the time that works. Confirm. Done. Go do literally anything else.
              </p>
            </div>

            <div className="text-center">
              <div className="w-24 h-24 bg-gradient-to-br from-pink-600 to-red-600 rounded-3xl flex items-center justify-center text-white text-4xl font-black mx-auto mb-6 shadow-xl">
                3
              </div>
              <h3 className="text-2xl font-black text-gray-900 mb-3">
                Show Up & Get It Done
              </h3>
              <p className="text-gray-600 text-lg">
                Walk in. Get your service. Walk out. Leave a review if you're feeling generous.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Testimonials */}
      <section className="py-20 bg-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-5xl font-black text-gray-900 mb-4">
              Real People. Real Talk.
            </h2>
            <p className="text-xl text-gray-600">
              Don't take our word for it. Take theirs.
            </p>
          </div>

          <div className="grid md:grid-cols-3 gap-8">
            <div className="bg-gradient-to-br from-purple-50 to-pink-50 rounded-3xl p-8 border-2 border-purple-200">
              <div className="text-yellow-400 text-3xl mb-4">★★★★★</div>
              <p className="text-gray-900 font-medium text-lg mb-6 leading-relaxed">
                "My car broke down at 3pm. Found a mechanic on EmptySlot, got a slot at 4pm,
                car was fixed by 6pm. I literally cried from joy."
              </p>
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full"></div>
                <div>
                  <div className="font-bold text-gray-900">Sarah M.</div>
                  <div className="text-gray-600 text-sm">Calgary, AB</div>
                </div>
              </div>
            </div>

            <div className="bg-gradient-to-br from-blue-50 to-purple-50 rounded-3xl p-8 border-2 border-blue-200">
              <div className="text-yellow-400 text-3xl mb-4">★★★★★</div>
              <p className="text-gray-900 font-medium text-lg mb-6 leading-relaxed">
                "Needed a haircut for a date. Booked it during lunch break,
                looked amazing by 5pm. She said yes. Thanks EmptySlot."
              </p>
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 bg-gradient-to-br from-pink-500 to-red-600 rounded-full"></div>
                <div>
                  <div className="font-bold text-gray-900">Marcus T.</div>
                  <div className="text-gray-600 text-sm">Calgary, AB</div>
                </div>
              </div>
            </div>

            <div className="bg-gradient-to-br from-pink-50 to-red-50 rounded-3xl p-8 border-2 border-pink-200">
              <div className="text-yellow-400 text-3xl mb-4">★★★★★</div>
              <p className="text-gray-900 font-medium text-lg mb-6 leading-relaxed">
                "As a provider, EmptySlot fills my cancellations instantly.
                My calendar stays full, my bank account stays happy."
              </p>
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 bg-gradient-to-br from-purple-500 to-pink-600 rounded-full"></div>
                <div>
                  <div className="font-bold text-gray-900">Jennifer L.</div>
                  <div className="text-gray-600 text-sm">Salon Owner</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* For Providers CTA */}
      <section className="relative py-24 bg-gradient-to-br from-gray-900 via-purple-900 to-black text-white overflow-hidden">
        <div className="absolute inset-0 bg-[url('/grid.svg')] opacity-10"></div>
        <div className="relative max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <div className="inline-block mb-6 px-6 py-3 bg-white/10 backdrop-blur-sm rounded-full text-sm font-bold">
            💼 FOR SERVICE PROVIDERS
          </div>
          <h2 className="text-5xl md:text-6xl font-black mb-6 leading-tight">
            Empty Slots Are<br />
            <span className="bg-gradient-to-r from-yellow-300 via-pink-300 to-purple-300 bg-clip-text text-transparent">
              Empty Money
            </span>
          </h2>
          <p className="text-2xl text-gray-300 mb-12 leading-relaxed max-w-3xl mx-auto">
            Stop losing revenue to no-shows and cancellations.
            Fill your calendar in real-time with customers who actually show up.
          </p>
          <div className="flex flex-col sm:flex-row gap-6 justify-center">
            <Link
              href="/auth/signup"
              className="inline-block bg-gradient-to-r from-pink-500 to-purple-600 text-white font-black px-12 py-5 rounded-2xl hover:shadow-2xl transition-all text-lg hover:scale-105"
            >
              Start Filling Slots →
            </Link>
            <Link
              href="/auth/login"
              className="inline-block bg-white/10 backdrop-blur-sm text-white font-bold px-12 py-5 rounded-2xl hover:bg-white/20 transition-all text-lg border-2 border-white/20"
            >
              Provider Login
            </Link>
          </div>
          <p className="mt-8 text-gray-400">
            No setup fees. No monthly fees. Only pay when you get bookings.
          </p>
        </div>
      </section>

      {/* Final CTA */}
      <section className="py-20 bg-white">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="text-5xl md:text-6xl font-black text-gray-900 mb-6">
            What Are You<br />
            Waiting For?
          </h2>
          <p className="text-2xl text-gray-600 mb-12">
            Life's happening now. Not next week.
          </p>
          <Link
            href="/search"
            className="inline-block bg-gradient-to-r from-purple-600 to-pink-600 text-white font-black px-16 py-6 rounded-2xl hover:shadow-2xl transition-all text-xl hover:scale-105"
          >
            Find Your Slot Now →
          </Link>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-gray-900 text-gray-400 py-12">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid md:grid-cols-4 gap-8 mb-8">
            <div>
              <h3 className="text-white font-black text-xl mb-4">EmptySlot</h3>
              <p className="text-sm">
                Book last-minute appointments with local service providers.
              </p>
            </div>
            <div>
              <h4 className="text-white font-bold mb-4">For Customers</h4>
              <ul className="space-y-2 text-sm">
                <li><Link href="/search" className="hover:text-white">Find Services</Link></li>
                <li><Link href="/auth/signup" className="hover:text-white">Sign Up</Link></li>
                <li><Link href="/appointments" className="hover:text-white">My Bookings</Link></li>
              </ul>
            </div>
            <div>
              <h4 className="text-white font-bold mb-4">For Providers</h4>
              <ul className="space-y-2 text-sm">
                <li><Link href="/auth/signup" className="hover:text-white">Join EmptySlot</Link></li>
                <li><Link href="/provider/dashboard" className="hover:text-white">Dashboard</Link></li>
                <li><Link href="/auth/login" className="hover:text-white">Login</Link></li>
              </ul>
            </div>
            <div>
              <h4 className="text-white font-bold mb-4">Company</h4>
              <ul className="space-y-2 text-sm">
                <li><a href="#" className="hover:text-white">About Us</a></li>
                <li><a href="#" className="hover:text-white">Contact</a></li>
                <li><a href="#" className="hover:text-white">Privacy Policy</a></li>
              </ul>
            </div>
          </div>
          <div className="border-t border-gray-800 pt-8 text-center text-sm">
            <p>&copy; 2024 EmptySlot. All rights reserved. Stop wasting time. Start booking.</p>
          </div>
        </div>
      </footer>
    </div>
  );
}

function getCategoryIcon(slug: string): string {
  const icons: Record<string, string> = {
    'hair-salon': '💇',
    'nail-salon': '💅',
    'spa-massage': '💆',
    dental: '🦷',
    medical: '🏥',
    plumbing: '🔧',
    hvac: '🌡️',
    electrical: '⚡',
    cleaning: '🧹',
    automotive: '🚗',
    fitness: '💪',
    beauty: '✨',
  };
  return icons[slug] || '📋';
}
