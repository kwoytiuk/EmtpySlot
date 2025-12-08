import { useState, useEffect } from 'react';
import {
  View,
  Text,
  ScrollView,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
} from 'react-native';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { providersApi } from 'shared';
import type { Provider, Service } from 'shared';

export default function ProviderDetailsScreen() {
  const { id } = useLocalSearchParams();
  const router = useRouter();
  const [provider, setProvider] = useState<Provider | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      loadProvider();
    }
  }, [id]);

  const loadProvider = async () => {
    setLoading(true);
    try {
      const prov = await providersApi.getProviderById(id as string);
      setProvider(prov);
    } catch (error) {
      console.error('Error loading provider:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color="#2563eb" />
      </View>
    );
  }

  if (!provider) {
    return (
      <View style={styles.centerContainer}>
        <Text style={styles.errorText}>Provider not found</Text>
      </View>
    );
  }

  const renderService = (service: Service) => (
    <TouchableOpacity
      key={service.id}
      style={styles.serviceCard}
      onPress={() =>
        router.push({
          pathname: '/booking',
          params: {
            providerId: provider.id,
            serviceId: service.id,
            serviceName: service.name,
            price: service.price,
            duration: service.duration_minutes,
          },
        })
      }
    >
      <View style={styles.serviceHeader}>
        <Text style={styles.serviceName}>{service.name}</Text>
        <Text style={styles.servicePrice}>${service.price}</Text>
      </View>
      <Text style={styles.serviceDescription}>{service.description}</Text>
      <Text style={styles.serviceDuration}>⏱️ {service.duration_minutes} minutes</Text>
    </TouchableOpacity>
  );

  return (
    <ScrollView style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.businessName}>{provider.business_name}</Text>
        {provider.rating_average && (
          <View style={styles.ratingContainer}>
            <Text style={styles.rating}>
              ⭐ {provider.rating_average.toFixed(1)}
            </Text>
            <Text style={styles.ratingCount}>({provider.rating_count} reviews)</Text>
          </View>
        )}
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>About</Text>
        <Text style={styles.description}>{provider.description}</Text>
      </View>

      {provider.provider_locations && provider.provider_locations.length > 0 && (
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Location</Text>
          <Text style={styles.address}>
            {provider.provider_locations[0].address_line1}
            {'\n'}
            {provider.provider_locations[0].city},{' '}
            {provider.provider_locations[0].state_province}{' '}
            {provider.provider_locations[0].postal_code}
          </Text>
        </View>
      )}

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Contact</Text>
        <Text style={styles.contact}>📞 {provider.phone}</Text>
        <Text style={styles.contact}>✉️ {provider.email}</Text>
      </View>

      {provider.services && provider.services.length > 0 && (
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Services</Text>
          {provider.services.map(renderService)}
        </View>
      )}
    </ScrollView>
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
  },
  errorText: {
    fontSize: 16,
    color: '#666',
  },
  header: {
    backgroundColor: '#fff',
    padding: 20,
    borderBottomWidth: 1,
    borderBottomColor: '#e5e7eb',
  },
  businessName: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#1a1a1a',
    marginBottom: 8,
  },
  ratingContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 8,
  },
  rating: {
    fontSize: 16,
    fontWeight: '600',
  },
  ratingCount: {
    fontSize: 14,
    color: '#666',
  },
  section: {
    backgroundColor: '#fff',
    padding: 20,
    marginTop: 12,
    borderTopWidth: 1,
    borderBottomWidth: 1,
    borderColor: '#e5e7eb',
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#1a1a1a',
    marginBottom: 12,
  },
  description: {
    fontSize: 16,
    color: '#666',
    lineHeight: 24,
  },
  address: {
    fontSize: 16,
    color: '#374151',
    lineHeight: 24,
  },
  contact: {
    fontSize: 16,
    color: '#374151',
    marginBottom: 8,
  },
  serviceCard: {
    backgroundColor: '#f9fafb',
    borderRadius: 12,
    padding: 16,
    marginBottom: 12,
    borderWidth: 1,
    borderColor: '#e5e7eb',
  },
  serviceHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  serviceName: {
    fontSize: 18,
    fontWeight: '600',
    color: '#1a1a1a',
    flex: 1,
  },
  servicePrice: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#2563eb',
  },
  serviceDescription: {
    fontSize: 14,
    color: '#666',
    marginBottom: 8,
    lineHeight: 20,
  },
  serviceDuration: {
    fontSize: 14,
    color: '#374151',
  },
});
