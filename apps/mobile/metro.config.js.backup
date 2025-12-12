const { getDefaultConfig } = require('expo/metro-config');
const path = require('path');

const projectRoot = __dirname;
const workspaceRoot = path.resolve(projectRoot, '../..');

const config = getDefaultConfig(projectRoot);

// 1. Watch all files within the monorepo
config.watchFolders = [workspaceRoot];

// 2. Let Metro know where to resolve packages
config.resolver.nodeModulesPaths = [
  path.resolve(projectRoot, 'node_modules'),
  path.resolve(workspaceRoot, 'node_modules'),
];

// 3. Disable hierarchical module lookup (important for monorepos)
config.resolver.disableHierarchicalLookup = true;

// 4. Only resolve the shared package explicitly
config.resolver.extraNodeModules = {
  shared: path.resolve(workspaceRoot, 'packages/shared'),
};

module.exports = config;
