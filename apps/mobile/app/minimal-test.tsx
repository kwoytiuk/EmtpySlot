import { View, Text, StyleSheet } from 'react-native';

export default function MinimalTest() {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>✅ App Loaded Successfully!</Text>
      <Text style={styles.subtitle}>No shared package imports</Text>
      <Text style={styles.info}>
        If you can see this screen, the React Native runtime is working correctly.
      </Text>
      <Text style={styles.info}>
        The issue is likely with the 'shared' package imports.
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 20,
    backgroundColor: '#f0f9ff',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#059669',
    marginBottom: 16,
    textAlign: 'center',
  },
  subtitle: {
    fontSize: 18,
    color: '#6b7280',
    marginBottom: 24,
    textAlign: 'center',
  },
  info: {
    fontSize: 14,
    color: '#374151',
    marginBottom: 8,
    textAlign: 'center',
    lineHeight: 20,
  },
});
