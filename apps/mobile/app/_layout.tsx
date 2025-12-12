// Import polyfills first, before any other imports
import '../polyfills';
import { Stack } from 'expo-router';

export default function RootLayout() {
  return (
    <Stack screenOptions={{ headerShown: true }}>
      <Stack.Screen name="minimal-test" options={{ title: '🧪 Minimal Test' }} />
      <Stack.Screen name="index" options={{ title: 'EmptySlot' }} />
      <Stack.Screen name="search-test" options={{ title: 'Test Screen' }} />
      <Stack.Screen name="auth/login" options={{ title: 'Login' }} />
      <Stack.Screen name="auth/signup" options={{ title: 'Sign Up' }} />
      <Stack.Screen name="search" options={{ title: 'Find Services' }} />
      <Stack.Screen name="provider/[id]" options={{ title: 'Provider Details' }} />
      <Stack.Screen name="booking" options={{ title: 'Book Appointment' }} />
      <Stack.Screen name="appointments" options={{ title: 'My Appointments' }} />
    </Stack>
  );
}
