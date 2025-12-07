import { View, Text, StyleSheet } from 'react-native'
import { StatusBar } from 'expo-status-bar'
import { Button } from '../src/components/Button'

export default function Home() {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Welcome to EmptySlot</Text>
      <Text style={styles.subtitle}>
        A modern monorepo with Next.js for web and React Native for mobile
      </Text>
      <Button />
      <StatusBar style="auto" />
    </View>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20,
  },
  title: {
    fontSize: 32,
    fontWeight: 'bold',
    marginBottom: 20,
    textAlign: 'center',
  },
  subtitle: {
    fontSize: 18,
    marginBottom: 30,
    textAlign: 'center',
    color: '#666',
  },
})
