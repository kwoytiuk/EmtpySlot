// Polyfills for React Native to support Supabase and web APIs
import 'react-native-url-polyfill/auto';

// Ensure global types are available
if (typeof global !== 'undefined') {
  // @ts-ignore
  global.Buffer = global.Buffer || require('buffer').Buffer;
}

// FormData, File, and Blob are already polyfilled by React Native 0.81+
// but we ensure they're properly exposed on the global object
if (typeof FormData === 'undefined') {
  console.warn('FormData is not available in this React Native version');
}

export {};
