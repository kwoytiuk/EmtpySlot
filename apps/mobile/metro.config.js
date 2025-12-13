const { getDefaultConfig } = require('expo/metro-config');
const path = require('path');

const projectRoot = __dirname;
const workspaceRoot = path.resolve(projectRoot, '../..');

const config = getDefaultConfig(projectRoot);

// Add workspace root to watch folders so Metro can follow symlinks
config.watchFolders = [workspaceRoot];

// Ensure Metro resolves all dependencies from the mobile app's node_modules
config.resolver = {
  ...config.resolver,
  nodeModulesPaths: [
    path.resolve(projectRoot, 'node_modules'),
  ],
  // Prevent Metro from resolving React/React Native from workspace root
  // This ensures only the mobile app's React/React Native is used
  blockList: [
    new RegExp(`${workspaceRoot.replace(/[/\\]/g, '[\\\\/]')}/node_modules/(react|react-native)/`),
  ],
};

module.exports = config;
