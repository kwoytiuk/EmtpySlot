// CRITICAL: Import polyfills in specific order
// This MUST be the first import in your app
import 'react-native-get-random-values'; // For crypto/uuid support
import 'react-native-url-polyfill/auto'; // For URL/URLSearchParams
import { Buffer } from 'buffer';

// Polyfill global objects that Supabase needs
if (typeof global.Buffer === 'undefined') {
  global.Buffer = Buffer;
}

// TextEncoder/TextDecoder polyfill (needed for Supabase)
if (typeof global.TextEncoder === 'undefined') {
  const { TextEncoder, TextDecoder } = require('text-encoding');
  global.TextEncoder = TextEncoder;
  global.TextDecoder = TextDecoder;
}

// React Native 0.81+ should have FormData, but let's ensure it's on global
// @ts-ignore
if (typeof FormData !== 'undefined') {
  // @ts-ignore
  global.FormData = FormData;
}

// @ts-ignore
if (typeof Blob !== 'undefined') {
  // @ts-ignore
  global.Blob = Blob;
}

// @ts-ignore
if (typeof File !== 'undefined') {
  // @ts-ignore
  global.File = File;
}

// @ts-ignore
if (typeof FileReader !== 'undefined') {
  // @ts-ignore
  global.FileReader = FileReader;
}

// Verify polyfills loaded
const polyfillStatus = {
  FormData: typeof global.FormData !== 'undefined',
  Blob: typeof global.Blob !== 'undefined',
  File: typeof global.File !== 'undefined',
  FileReader: typeof global.FileReader !== 'undefined',
  Buffer: typeof global.Buffer !== 'undefined',
  TextEncoder: typeof global.TextEncoder !== 'undefined',
  URL: typeof global.URL !== 'undefined',
};

console.log('✅ Polyfills loaded:', polyfillStatus);

// Warn about missing polyfills
Object.entries(polyfillStatus).forEach(([key, loaded]) => {
  if (!loaded) {
    console.warn(`⚠️ Warning: ${key} polyfill not loaded!`);
  }
});

export {};
