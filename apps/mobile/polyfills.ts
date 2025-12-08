// Polyfills for React Native to support Supabase and web APIs
import 'react-native-url-polyfill/auto';

// React Native 0.81+ includes FormData, File, and Blob by default
// But we need to ensure they're available globally for TypeScript

// Polyfill Buffer for binary data operations
if (typeof global.Buffer === 'undefined') {
  try {
    const { Buffer } = require('buffer');
    global.Buffer = Buffer;
  } catch (e) {
    console.warn('Buffer polyfill not available');
  }
}

// Ensure FormData is available globally
if (typeof global.FormData === 'undefined') {
  // @ts-ignore - FormData exists in RN 0.81+ but might not be on global
  if (typeof FormData !== 'undefined') {
    // @ts-ignore
    global.FormData = FormData;
  }
}

// Ensure Blob is available globally
if (typeof global.Blob === 'undefined') {
  // @ts-ignore - Blob exists in RN 0.81+ but might not be on global
  if (typeof Blob !== 'undefined') {
    // @ts-ignore
    global.Blob = Blob;
  }
}

// Ensure File is available globally
if (typeof global.File === 'undefined') {
  // @ts-ignore - File exists in RN 0.81+ but might not be on global
  if (typeof File !== 'undefined') {
    // @ts-ignore
    global.File = File;
  }
}

export {};
