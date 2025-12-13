const { getDefaultConfig } = require('expo/metro-config');
const path = require('path');

const projectRoot = __dirname;
const sharedPackage = path.resolve(projectRoot, '../../packages/shared');

const config = getDefaultConfig(projectRoot);

// Only watch the shared package directory (not workspace root to avoid conflicts)
config.watchFolders = [sharedPackage];

// Tell Metro where to find the shared package
config.resolver.extraNodeModules = {
  shared: sharedPackage,
};

module.exports = config;
