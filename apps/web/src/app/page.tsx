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
      {/* Hero Section */}
      <section className="relative bg-gradient-to-br from-blue-600 via-blue-700 to-blue-900 text-white">
        <div className="absolute inset-0 bg-black/20"></div>
        <div className="relative max-w-7xl mx-auto px-4 py-24 sm:px-6 lg:px-8">
          <div className="text-center">
            <h1 className="text-5xl font-bold tracking-tight sm:text-6xl lg:text-7xl mb-6">
              Book Last-Minute <br />
              <span className="text-blue-200">Appointments Near You</span>
            </h1>
            <p className="text-xl text-blue-100 max-w-2xl mx-auto mb-10">
              Find and book same-day appointments for hair, nails, dental, plumbing, HVAC, and more
            </p>

            {/* Search Bar */}
            <div className="max-w-3xl mx-auto">
              <div className="bg-white rounded-2xl shadow-2xl p-2">
                <div className="flex flex-col md:flex-row gap-2">
                  <input
                    type="text"
                    placeholder="Search for services..."
                    className="flex-1 px-6 py-4 text-gray-900 placeholder-gray-500 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                  <input
                    type="text"
                    placeholder="Calgary, AB"
                    className="px-6 py-4 text-gray-900 placeholder-gray-500 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500 md:w-64"
                  />
                  <Link
                    href="/search"
                    className="bg-red-600 hover:bg-red-700 text-white font-semibold px-8 py-4 rounded-xl transition-colors text-center"
                  >
                    Let's go
                  </Link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Categories */}
      <section className="py-16 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <h2 className="text-3xl font-bold text-gray-900 mb-8">
            Browse by Category
          </h2>
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
            {categories.slice(0, 12).map((category) => (
              <Link
                key={category.id}
                href={`/search?category=${category.id}`}
                className="bg-white rounded-xl p-6 text-center hover:shadow-lg transition-shadow border border-gray-200 hover:border-blue-500"
              >
                <div className="text-3xl mb-2">
                  {getCategoryIcon(category.slug)}
                </div>
                <h3 className="font-semibold text-gray-900 text-sm">
                  {category.name}
                </h3>
              </Link>
            ))}
          </div>
        </div>
      </section>

      {/* Featured Providers */}
      {featuredProviders.length > 0 && (
        <section className="py-16 bg-white">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex justify-between items-center mb-8">
              <div>
                <h2 className="text-3xl font-bold text-gray-900">
                  Available Now in Calgary
                </h2>
                <p className="text-gray-600 mt-2">
                  Book your appointment today
                </p>
              </div>
              <Link
                href="/search"
                className="text-blue-600 hover:text-blue-700 font-semibold"
              >
                View all →
              </Link>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {featuredProviders.map((provider) => (
                <Link
                  key={provider.id}
                  href={`/providers/${provider.id}`}
                  className="group bg-white rounded-xl border border-gray-200 overflow-hidden hover:shadow-xl transition-all"
                >
                  {/* Provider Image Placeholder */}
                  <div className="h-48 bg-gradient-to-br from-blue-500 to-purple-600 relative">
                    <div className="absolute inset-0 bg-black/20 group-hover:bg-black/10 transition-colors"></div>
                  </div>

                  <div className="p-5">
                    <div className="flex items-start justify-between mb-2">
                      <h3 className="font-bold text-lg text-gray-900 group-hover:text-blue-600 transition-colors">
                        {provider.business_name}
                      </h3>
                      {provider.rating_average && (
                        <div className="flex items-center bg-amber-50 px-2 py-1 rounded-lg">
                          <span className="text-amber-600 font-semibold">
                            ⭐ {provider.rating_average.toFixed(1)}
                          </span>
                        </div>
                      )}
                    </div>

                    <p className="text-gray-600 text-sm mb-3 line-clamp-2">
                      {provider.description}
                    </p>

                    {provider.provider_locations?.[0] && (
                      <p className="text-sm text-gray-500 flex items-center">
                        <span className="mr-1">📍</span>
                        {provider.provider_locations[0].city},{' '}
                        {provider.provider_locations[0].state_province}
                      </p>
                    )}

                    {provider.services && provider.services.length > 0 && (
                      <div className="mt-4 pt-4 border-t border-gray-100">
                        <p className="text-xs text-gray-500 mb-2">
                          Starting from
                        </p>
                        <p className="text-2xl font-bold text-gray-900">
                          ${Math.min(...provider.services.map((s) => s.price))}
                        </p>
                      </div>
                    )}
                  </div>
                </Link>
              ))}
            </div>
          </div>
        </section>
      )}

      {/* How It Works */}
      <section className="py-16 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <h2 className="text-3xl font-bold text-gray-900 text-center mb-12">
            How It Works
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="text-center">
              <div className="w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center text-white text-2xl font-bold mx-auto mb-4">
                1
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">
                Search
              </h3>
              <p className="text-gray-600">
                Find services near you by category or provider name
              </p>
            </div>
            <div className="text-center">
              <div className="w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center text-white text-2xl font-bold mx-auto mb-4">
                2
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">
                Book
              </h3>
              <p className="text-gray-600">
                Choose your preferred time slot and confirm your appointment
              </p>
            </div>
            <div className="text-center">
              <div className="w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center text-white text-2xl font-bold mx-auto mb-4">
                3
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">
                Enjoy
              </h3>
              <p className="text-gray-600">
                Show up to your appointment and enjoy the service
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-16 bg-gradient-to-r from-blue-600 to-blue-800 text-white">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="text-4xl font-bold mb-4">
            Are you a service provider?
          </h2>
          <p className="text-xl text-blue-100 mb-8">
            Join EmptySlot and fill your last-minute availability
          </p>
          <Link
            href="/auth/signup"
            className="inline-block bg-white text-blue-600 font-bold px-8 py-4 rounded-xl hover:bg-blue-50 transition-colors"
          >
            Sign Up as Provider
          </Link>
        </div>
      </section>
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
