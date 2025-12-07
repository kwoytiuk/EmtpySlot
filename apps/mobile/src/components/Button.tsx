import { useState } from 'react'
import { TouchableOpacity, Text, StyleSheet } from 'react-native'

export function Button() {
  const [count, setCount] = useState(0)

  return (
    <TouchableOpacity
      style={styles.button}
      onPress={() => setCount(count + 1)}
      activeOpacity={0.8}
    >
      <Text style={styles.text}>Clicked {count} times</Text>
    </TouchableOpacity>
  )
}

const styles = StyleSheet.create({
  button: {
    backgroundColor: '#0070f3',
    paddingVertical: 12,
    paddingHorizontal: 24,
    borderRadius: 8,
  },
  text: {
    color: 'white',
    fontSize: 16,
    fontWeight: '600',
  },
})
