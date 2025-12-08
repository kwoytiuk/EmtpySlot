// Polyfills for React Native to support Supabase and web APIs
import 'react-native-url-polyfill/auto';
import { Buffer } from 'buffer';

// Make Buffer available globally
if (typeof global.Buffer === 'undefined') {
  global.Buffer = Buffer;
}

// React Native 0.81+ has FormData, File, and Blob built-in
// Explicitly assign them to global to ensure they're available everywhere
if (typeof FormData !== 'undefined' && typeof global.FormData === 'undefined') {
  global.FormData = FormData;
}

if (typeof Blob !== 'undefined' && typeof global.Blob === 'undefined') {
  global.Blob = Blob;
}

if (typeof File !== 'undefined' && typeof global.File === 'undefined') {
  global.File = File;
}

// Log for debugging
console.log('Polyfills loaded:', {
  FormData: typeof global.FormData !== 'undefined',
  Blob: typeof global.Blob !== 'undefined',
  File: typeof global.File !== 'undefined',
  Buffer: typeof global.Buffer !== 'undefined',
});

export {};
