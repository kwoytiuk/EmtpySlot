import { useState, useEffect } from 'react';
import {
  View,
  Text,
  TextInput,
  FlatList,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
} from 'react-native';
import { useRouter } from 'expo-router';
import { LinearGradient } from 'expo-linear-gradient';
import { providersApi, categoriesApi } from 'shared';
import type { Provider, ServiceCategory } from 'shared';

export default function SearchScreen() {
  const router = useRouter();
  const [providers, setProviders] = useState<Provider[]>([]);
  const [categories, setCategories] = useState<ServiceCategory[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    try {
      // Load categories
      const cats = await categoriesApi.getCategories();
      setCategories(cats);

      // Load all providers (in a real app, you'd use location)
      const { providers: provs } = await providersApi.searchProviders({
        latitude: 51.0447, // Calgary coordinates
        longitude: -114.0719,
        radius: 50, // 50km radius
      });
      setProviders(provs || []);
    } catch (error) {
      console.error('Error loading data:', error);
    } finally {
      setLoading(false);
    }
  };

  const filterProviders = () => {
    let filtered = providers;

    if (searchQuery) {
      filtered = filtered.filter((p) =>
        p.business_name.toLowerCase().includes(searchQuery.toLowerCase())
      );
    }

    if (selectedCategory) {
      filtered = filtered.filter((p) =>
        p.services?.some((s) => s.category_id === selectedCategory)
      );
    }

    return filtered;
  };

  const renderProvider = ({ item }: { item: Provider }) => {
    // Get min price from services
    const minPrice = item.services && item.services.length > 0
      ? Math.min(...item.services.map((s: any) => s.price))
      : null;

    return (
      <TouchableOpacity
        style={styles.providerCard}
        onPress={() => router.push(`/provider/${item.id}`)}
      >
        {/* Gradient Image Placeholder */}
        <LinearGradient
          colors={['#3b82f6', '#8b5cf6', '#ec4899']}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
          style={styles.providerImage}
        >
          {item.verified && (
            <View style={styles.verifiedBadge}>
              <Text style={styles.verifiedText}>✓ Verified</Text>
            </View>
          )}
        </LinearGradient>

        {/* Content */}
        <View style={styles.providerContent}>
          <View style={styles.providerHeader}>
            <Text style={styles.providerName}>{item.business_name}</Text>
          </View>

          {item.provider_locations && item.provider_locations.length > 0 && (
            <Text style={styles.providerLocation}>
              📍 {item.provider_locations[0].city}, {item.provider_locations[0].state_province}
            </Text>
          )}

          {/* Rating */}
          <View style={styles.ratingContainer}>
            <View style={styles.ratingBadge}>
              <Text style={styles.ratingText}>
                ★ {item.rating_average ? item.rating_average.toFixed(1) : 'New'}
              </Text>
            </View>
            {item.rating_count > 0 && (
              <Text style={styles.reviewCount}>
                ({item.rating_count} {item.rating_count === 1 ? 'review' : 'reviews'})
              </Text>
            )}
          </View>

          {item.description && (
            <Text style={styles.providerDescription} numberOfLines={2}>
              {item.description}
            </Text>
          )}

          {/* Price and Book Button */}
          <View style={styles.providerFooter}>
            {minPrice !== null ? (
              <View>
                <Text style={styles.price}>${minPrice.toFixed(0)}</Text>
                <Text style={styles.priceLabel}>and up</Text>
              </View>
            ) : (
              <Text style={styles.contactText}>Contact for pricing</Text>
            )}
            <View style={styles.bookButton}>
              <Text style={styles.bookButtonText}>Book Now</Text>
            </View>
          </View>
        </View>
      </TouchableOpacity>
    );
  };

  const renderCategory = ({ item }: { item: ServiceCategory }) => (
    <TouchableOpacity
      style={[
        styles.categoryChip,
        selectedCategory === item.id && styles.categoryChipActive,
      ]}
      onPress={() =>
        setSelectedCategory(selectedCategory === item.id ? null : item.id)
      }
    >
      <Text
        style={[
          styles.categoryText,
          selectedCategory === item.id && styles.categoryTextActive,
        ]}
      >
        {item.name}
      </Text>
    </TouchableOpacity>
  );

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color="#2563eb" />
        <Text style={styles.loadingText}>Loading providers...</Text>
      </View>
    );
  }

  const filteredProviders = filterProviders();

  return (
    <View style={styles.container}>
      <View style={styles.searchSection}>
        <TextInput
          style={styles.searchInput}
          placeholder="Search providers..."
          value={searchQuery}
          onChangeText={setSearchQuery}
        />
      </View>

      {categories.length > 0 && (
        <View style={styles.categoriesSection}>
          <FlatList
            horizontal
            data={categories}
            renderItem={renderCategory}
            keyExtractor={(item) => item.id}
            showsHorizontalScrollIndicator={false}
            contentContainerStyle={styles.categoriesList}
          />
        </View>
      )}

      <FlatList
        data={filteredProviders}
        renderItem={renderProvider}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.providersList}
        ListEmptyComponent={
          <View style={styles.emptyContainer}>
            <Text style={styles.emptyText}>No providers found</Text>
            <Text style={styles.emptySubtext}>Try adjusting your search or filters</Text>
          </View>
        }
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f9fafb',
  },
  centerContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    gap: 16,
  },
  loadingText: {
    fontSize: 16,
    color: '#666',
  },
  searchSection: {
    padding: 16,
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#e5e7eb',
  },
  searchInput: {
    borderWidth: 1,
    borderColor: '#d1d5db',
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
    backgroundColor: '#fff',
  },
  categoriesSection: {
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#e5e7eb',
  },
  categoriesList: {
    padding: 12,
    gap: 8,
  },
  categoryChip: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 20,
    backgroundColor: '#f3f4f6',
    borderWidth: 1,
    borderColor: '#d1d5db',
  },
  categoryChipActive: {
    backgroundColor: '#2563eb',
    borderColor: '#2563eb',
  },
  categoryText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#374151',
  },
  categoryTextActive: {
    color: '#fff',
  },
  providersList: {
    padding: 16,
    gap: 12,
  },
  providerCard: {
    backgroundColor: '#fff',
    borderRadius: 12,
    overflow: 'hidden',
    borderWidth: 1,
    borderColor: '#e5e7eb',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  providerImage: {
    height: 140,
    justifyContent: 'flex-end',
    alignItems: 'flex-end',
    padding: 12,
  },
  verifiedBadge: {
    backgroundColor: 'rgba(255, 255, 255, 0.95)',
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 16,
  },
  verifiedText: {
    fontSize: 12,
    fontWeight: '600',
    color: '#2563eb',
  },
  providerContent: {
    padding: 12,
  },
  providerHeader: {
    marginBottom: 4,
  },
  providerName: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#1a1a1a',
  },
  providerLocation: {
    fontSize: 13,
    color: '#666',
    marginBottom: 8,
  },
  ratingContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 8,
    gap: 6,
  },
  ratingBadge: {
    backgroundColor: '#2563eb',
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 6,
  },
  ratingText: {
    fontSize: 13,
    fontWeight: 'bold',
    color: '#fff',
  },
  reviewCount: {
    fontSize: 13,
    color: '#666',
  },
  providerDescription: {
    fontSize: 14,
    color: '#666',
    marginBottom: 12,
    lineHeight: 20,
  },
  providerFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingTop: 12,
    borderTopWidth: 1,
    borderTopColor: '#f3f4f6',
  },
  price: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#1a1a1a',
  },
  priceLabel: {
    fontSize: 12,
    color: '#666',
  },
  contactText: {
    fontSize: 13,
    color: '#666',
  },
  bookButton: {
    backgroundColor: '#dc2626',
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 8,
  },
  bookButtonText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#fff',
  },
  emptyContainer: {
    padding: 40,
    alignItems: 'center',
  },
  emptyText: {
    fontSize: 18,
    fontWeight: '600',
    color: '#374151',
    marginBottom: 8,
  },
  emptySubtext: {
    fontSize: 14,
    color: '#666',
  },
});
