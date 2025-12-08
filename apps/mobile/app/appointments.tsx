import { useState, useEffect } from 'react';
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  Alert,
} from 'react-native';
import { appointmentsApi } from 'shared';
import type { Appointment } from 'shared';

export default function AppointmentsScreen() {
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<'upcoming' | 'past'>('upcoming');

  useEffect(() => {
    loadAppointments();
  }, []);

  const loadAppointments = async () => {
    setLoading(true);
    try {
      const appts = await appointmentsApi.getCustomerAppointments();
      setAppointments(appts);
    } catch (error) {
      console.error('Error loading appointments:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = async (appointmentId: string) => {
    Alert.alert(
      'Cancel Appointment',
      'Are you sure you want to cancel this appointment?',
      [
        { text: 'No', style: 'cancel' },
        {
          text: 'Yes',
          style: 'destructive',
          onPress: async () => {
            try {
              const { error } = await appointmentsApi.cancelAppointment(appointmentId);
              if (error) {
                Alert.alert('Error', error.message);
              } else {
                Alert.alert('Success', 'Appointment cancelled');
                loadAppointments();
              }
            } catch (err) {
              Alert.alert('Error', 'Failed to cancel appointment');
            }
          },
        },
      ]
    );
  };

  const filterAppointments = () => {
    const now = new Date();
    if (filter === 'upcoming') {
      return appointments.filter(
        (a) => new Date(a.appointment_date) >= now && a.status !== 'cancelled'
      );
    } else {
      return appointments.filter(
        (a) => new Date(a.appointment_date) < now || a.status === 'cancelled'
      );
    }
  };

  const formatDateTime = (dateStr: string) => {
    const date = new Date(dateStr);
    return {
      date: date.toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
      }),
      time: date.toLocaleTimeString('en-US', {
        hour: 'numeric',
        minute: '2-digit',
      }),
    };
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'confirmed':
        return '#10b981';
      case 'pending':
        return '#f59e0b';
      case 'cancelled':
        return '#ef4444';
      case 'completed':
        return '#6b7280';
      default:
        return '#6b7280';
    }
  };

  const renderAppointment = ({ item }: { item: Appointment }) => {
    const { date, time } = formatDateTime(item.appointment_date);
    const canCancel =
      item.status === 'pending' || item.status === 'confirmed';

    return (
      <View style={styles.appointmentCard}>
        <View style={styles.appointmentHeader}>
          <Text style={styles.bookingReference}>{item.booking_reference}</Text>
          <View
            style={[
              styles.statusBadge,
              { backgroundColor: getStatusColor(item.status) },
            ]}
          >
            <Text style={styles.statusText}>{item.status}</Text>
          </View>
        </View>

        <Text style={styles.serviceName}>
          {item.services?.name || 'Service'}
        </Text>

        <View style={styles.dateTimeContainer}>
          <Text style={styles.dateTime}>
            📅 {date} at {time}
          </Text>
        </View>

        <Text style={styles.providerName}>
          📍 {item.providers?.business_name}
        </Text>

        {item.notes && (
          <Text style={styles.notes}>Note: {item.notes}</Text>
        )}

        {canCancel && (
          <TouchableOpacity
            style={styles.cancelButton}
            onPress={() => handleCancel(item.id)}
          >
            <Text style={styles.cancelButtonText}>Cancel Appointment</Text>
          </TouchableOpacity>
        )}
      </View>
    );
  };

  if (loading) {
    return (
      <View style={styles.centerContainer}>
        <ActivityIndicator size="large" color="#2563eb" />
      </View>
    );
  }

  const filteredAppointments = filterAppointments();

  return (
    <View style={styles.container}>
      <View style={styles.filterContainer}>
        <TouchableOpacity
          style={[
            styles.filterButton,
            filter === 'upcoming' && styles.filterButtonActive,
          ]}
          onPress={() => setFilter('upcoming')}
        >
          <Text
            style={[
              styles.filterText,
              filter === 'upcoming' && styles.filterTextActive,
            ]}
          >
            Upcoming
          </Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={[
            styles.filterButton,
            filter === 'past' && styles.filterButtonActive,
          ]}
          onPress={() => setFilter('past')}
        >
          <Text
            style={[
              styles.filterText,
              filter === 'past' && styles.filterTextActive,
            ]}
          >
            Past
          </Text>
        </TouchableOpacity>
      </View>

      <FlatList
        data={filteredAppointments}
        renderItem={renderAppointment}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.listContainer}
        ListEmptyComponent={
          <View style={styles.emptyContainer}>
            <Text style={styles.emptyText}>No appointments found</Text>
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
  },
  filterContainer: {
    flexDirection: 'row',
    padding: 16,
    gap: 12,
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#e5e7eb',
  },
  filterButton: {
    flex: 1,
    padding: 12,
    borderRadius: 8,
    backgroundColor: '#f3f4f6',
    alignItems: 'center',
    borderWidth: 2,
    borderColor: '#d1d5db',
  },
  filterButtonActive: {
    backgroundColor: '#eff6ff',
    borderColor: '#2563eb',
  },
  filterText: {
    fontSize: 16,
    fontWeight: '600',
    color: '#374151',
  },
  filterTextActive: {
    color: '#2563eb',
  },
  listContainer: {
    padding: 16,
    gap: 12,
  },
  appointmentCard: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    borderWidth: 1,
    borderColor: '#e5e7eb',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 2,
    elevation: 2,
  },
  appointmentHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 12,
  },
  bookingReference: {
    fontSize: 14,
    fontWeight: '600',
    color: '#666',
    fontFamily: 'monospace',
  },
  statusBadge: {
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 12,
  },
  statusText: {
    fontSize: 12,
    fontWeight: '600',
    color: '#fff',
    textTransform: 'capitalize',
  },
  serviceName: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#1a1a1a',
    marginBottom: 8,
  },
  dateTimeContainer: {
    marginBottom: 8,
  },
  dateTime: {
    fontSize: 16,
    color: '#374151',
  },
  providerName: {
    fontSize: 16,
    color: '#2563eb',
    marginBottom: 8,
  },
  notes: {
    fontSize: 14,
    color: '#666',
    fontStyle: 'italic',
    marginTop: 8,
  },
  cancelButton: {
    marginTop: 12,
    padding: 12,
    borderRadius: 8,
    backgroundColor: '#fee2e2',
    borderWidth: 1,
    borderColor: '#fecaca',
    alignItems: 'center',
  },
  cancelButtonText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#dc2626',
  },
  emptyContainer: {
    padding: 40,
    alignItems: 'center',
  },
  emptyText: {
    fontSize: 16,
    color: '#666',
  },
});
