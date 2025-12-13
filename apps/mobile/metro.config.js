const { getDefaultConfig } = require('expo/metro-config');
const path = require('path');

const projectRoot = __dirname;
const workspaceRoot = path.resolve(projectRoot, '../..');
const sharedPackage = path.resolve(workspaceRoot, 'packages/shared');

const config = getDefaultConfig(projectRoot);

// Watch both the shared package and workspace root
config.watchFolders = [workspaceRoot];

// Tell Metro where to find packages (including workspace root for shared deps)
config.resolver.nodeModulesPaths = [
  path.resolve(projectRoot, 'node_modules'),
  path.resolve(workspaceRoot, 'node_modules'),
];

// Tell Metro where to find the shared package
config.resolver.extraNodeModules = {
  shared: sharedPackage,
};

module.exports = config;
