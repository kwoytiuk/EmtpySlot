import { View, Text, StyleSheet } from 'react-native';

export default function SearchTest() {
  return (
    <View style={styles.container}>
      <Text style={styles.text}>Search Test Screen - If you see this, React Native works!</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#fff',
  },
  text: {
    fontSize: 18,
    color: '#000',
  },
});
