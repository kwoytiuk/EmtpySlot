const { getDefaultConfig } = require('expo/metro-config');
const path = require('path');

const projectRoot = __dirname;
const workspaceRoot = path.resolve(projectRoot, '../..');

const config = getDefaultConfig(projectRoot);

// 1. Watch all files within the monorepo
config.watchFolders = [workspaceRoot];

// 2. Let Metro know where to resolve packages and in what order
config.resolver.nodeModulesPaths = [
  path.resolve(projectRoot, 'node_modules'),
  path.resolve(workspaceRoot, 'node_modules'),
];

// 3. Force Metro to resolve shared package from source and all React/RN packages from mobile
config.resolver.extraNodeModules = {
  ...config.resolver.extraNodeModules,
  shared: path.resolve(workspaceRoot, 'packages/shared'),
  'react': path.resolve(projectRoot, 'node_modules/react'),
  'react-native': path.resolve(projectRoot, 'node_modules/react-native'),
  '@react-native': path.resolve(projectRoot, 'node_modules/@react-native'),
  'expo': path.resolve(projectRoot, 'node_modules/expo'),
  'expo-modules-core': path.resolve(projectRoot, 'node_modules/expo-modules-core'),
};

// 4. Add additional resolver for React Native
config.resolver.sourceExts = [...config.resolver.sourceExts, 'mjs'];

// 5. Add support for Hermes
config.transformer.unstable_allowRequireContext = true;

module.exports = config;
