'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import Link from 'next/link'

interface HomeSearchProps {
  categories: Array<{
    id: string
    name: string
    slug: string
    icon?: string
  }>
}

export function HomeSearch({ categories }: HomeSearchProps) {
  const router = useRouter()
  const [selectedCategory, setSelectedCategory] = useState('')
  const [location, setLocation] = useState('Calgary, AB')

  const handleSearch = () => {
    const params = new URLSearchParams()
    if (selectedCategory) {
      params.set('category', selectedCategory)
    }
    router.push(`/search?${params.toString()}`)
  }

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSearch()
    }
  }

  return (
    <div className="max-w-4xl mx-auto mb-12">
      <div className="bg-white/95 backdrop-blur-md rounded-3xl shadow-2xl p-3">
        <div className="flex flex-col md:flex-row gap-3">
          {/* Category Dropdown */}
          <select
            value={selectedCategory}
            onChange={(e) => setSelectedCategory(e.target.value)}
            onKeyPress={handleKeyPress}
            className="flex-1 px-8 py-5 text-gray-900 placeholder-gray-500 rounded-2xl focus:outline-none focus:ring-4 focus:ring-purple-500 font-medium text-lg bg-white border-0 cursor-pointer"
          >
            <option value="">What do you need? (All Services)</option>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.icon ? `${category.icon} ` : ''}{category.name}
              </option>
            ))}
          </select>

          {/* Location Input */}
          <input
            type="text"
            value={location}
            onChange={(e) => setLocation(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder="📍 Calgary, AB"
            className="px-8 py-5 text-gray-900 placeholder-gray-500 rounded-2xl focus:outline-none focus:ring-4 focus:ring-purple-500 md:w-72 font-medium text-lg"
          />

          {/* Search Button */}
          <button
            onClick={handleSearch}
            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-black px-12 py-5 rounded-2xl transition-all text-lg shadow-lg hover:shadow-xl hover:scale-105"
          >
            Find Slots →
          </button>
        </div>
      </div>
    </div>
  )
}
