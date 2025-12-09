import { View, Text, StyleSheet, TouchableOpacity, ScrollView } from 'react-native';
import { StatusBar } from 'expo-status-bar';
import { useRouter } from 'expo-router';
import { LinearGradient } from 'expo-linear-gradient';

export default function Home() {
  const router = useRouter();

  return (
    <ScrollView style={styles.scrollView} contentContainerStyle={styles.scrollContent}>
      {/* Hero Section with Gradient */}
      <LinearGradient
        colors={['#2563eb', '#1e40af', '#1e3a8a']}
        style={styles.hero}
      >
        <View style={styles.heroContent}>
          <Text style={styles.title}>EmptySlot</Text>
          <Text style={styles.subtitle}>
            Book Last-Minute{'\n'}Appointments Near You
          </Text>
          <Text style={styles.description}>
            Discover available services and book instantly
          </Text>

          <TouchableOpacity
            style={styles.primaryButton}
            onPress={() => router.push('/search')}
          >
            <Text style={styles.primaryButtonText}>Find Services Now</Text>
          </TouchableOpacity>
        </View>
      </LinearGradient>

      {/* Features Section */}
      <View style={styles.featuresSection}>
        <Text style={styles.sectionTitle}>How It Works</Text>

        <View style={styles.featureCard}>
          <View style={styles.featureIcon}>
            <Text style={styles.featureIconText}>🔍</Text>
          </View>
          <View style={styles.featureText}>
            <Text style={styles.featureTitle}>1. Search</Text>
            <Text style={styles.featureDescription}>
              Find services near you with last-minute availability
            </Text>
          </View>
        </View>

        <View style={styles.featureCard}>
          <View style={styles.featureIcon}>
            <Text style={styles.featureIconText}>📅</Text>
          </View>
          <View style={styles.featureText}>
            <Text style={styles.featureTitle}>2. Book</Text>
            <Text style={styles.featureDescription}>
              Choose your time and confirm your appointment
            </Text>
          </View>
        </View>

        <View style={styles.featureCard}>
          <View style={styles.featureIcon}>
            <Text style={styles.featureIconText}>✨</Text>
          </View>
          <View style={styles.featureText}>
            <Text style={styles.featureTitle}>3. Enjoy</Text>
            <Text style={styles.featureDescription}>
              Show up and enjoy your service - it's that easy!
            </Text>
          </View>
        </View>
      </View>

      {/* Auth Section */}
      <View style={styles.authSection}>
        <Text style={styles.authTitle}>Already have an account?</Text>
        <TouchableOpacity
          style={styles.loginButton}
          onPress={() => router.push('/auth/login')}
        >
          <Text style={styles.loginButtonText}>Sign In</Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.signupButton}
          onPress={() => router.push('/auth/signup')}
        >
          <Text style={styles.signupButtonText}>Create Account</Text>
        </TouchableOpacity>
      </View>

      <StatusBar style="light" />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  scrollView: {
    flex: 1,
    backgroundColor: '#f9fafb',
  },
  scrollContent: {
    flexGrow: 1,
  },
  hero: {
    paddingTop: 60,
    paddingBottom: 50,
    paddingHorizontal: 20,
  },
  heroContent: {
    alignItems: 'center',
  },
  title: {
    fontSize: 36,
    fontWeight: 'bold',
    color: '#fff',
    marginBottom: 16,
  },
  subtitle: {
    fontSize: 32,
    fontWeight: 'bold',
    color: '#fff',
    textAlign: 'center',
    lineHeight: 40,
    marginBottom: 12,
  },
  description: {
    fontSize: 18,
    color: '#bfdbfe',
    textAlign: 'center',
    marginBottom: 32,
  },
  primaryButton: {
    backgroundColor: '#dc2626',
    paddingVertical: 16,
    paddingHorizontal: 32,
    borderRadius: 12,
    width: '100%',
    maxWidth: 320,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
  },
  primaryButtonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
    textAlign: 'center',
  },
  featuresSection: {
    padding: 20,
    paddingTop: 32,
  },
  sectionTitle: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#1a1a1a',
    marginBottom: 24,
    textAlign: 'center',
  },
  featureCard: {
    flexDirection: 'row',
    backgroundColor: '#fff',
    padding: 20,
    borderRadius: 12,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  featureIcon: {
    width: 56,
    height: 56,
    borderRadius: 28,
    backgroundColor: '#eff6ff',
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: 16,
  },
  featureIconText: {
    fontSize: 28,
  },
  featureText: {
    flex: 1,
    justifyContent: 'center',
  },
  featureTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#1a1a1a',
    marginBottom: 4,
  },
  featureDescription: {
    fontSize: 14,
    color: '#666',
    lineHeight: 20,
  },
  authSection: {
    padding: 20,
    paddingBottom: 40,
  },
  authTitle: {
    fontSize: 16,
    color: '#666',
    textAlign: 'center',
    marginBottom: 16,
  },
  loginButton: {
    backgroundColor: '#2563eb',
    paddingVertical: 14,
    borderRadius: 10,
    marginBottom: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  loginButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
    textAlign: 'center',
  },
  signupButton: {
    backgroundColor: '#fff',
    paddingVertical: 14,
    borderRadius: 10,
    borderWidth: 2,
    borderColor: '#2563eb',
  },
  signupButtonText: {
    color: '#2563eb',
    fontSize: 16,
    fontWeight: '600',
    textAlign: 'center',
  },
});
