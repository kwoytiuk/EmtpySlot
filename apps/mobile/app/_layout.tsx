// Import polyfills first, before any other imports
import '../polyfills';
import { Stack } from 'expo-router';

export default function RootLayout() {
  return (
    <Stack>
      <Stack.Screen name="index" options={{ title: 'EmptySlot' }} />
    </Stack>
  );
}
