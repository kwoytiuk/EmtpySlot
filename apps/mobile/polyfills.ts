// CRITICAL: Import polyfills in specific order
// This MUST be the first import in your app
import './react-use-polyfill'; // Polyfill for React.use() (React 19 feature) - MUST BE FIRST!
import 'react-native-get-random-values'; // For crypto/uuid support
import 'react-native-url-polyfill/auto'; // For URL/URLSearchParams
import { Buffer } from 'buffer';

// Polyfill Buffer
global.Buffer = Buffer;

// React Native 0.81+ has FormData, Blob, File, TextEncoder built-in
// Just ensure they're on the global object

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

console.log('✅ Polyfills loaded');

export {};
