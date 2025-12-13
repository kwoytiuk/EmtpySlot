const { getDefaultConfig } = require('expo/metro-config');
const path = require('path');

const projectRoot = __dirname;
const sharedPackage = path.resolve(projectRoot, '../../packages/shared');

const config = getDefaultConfig(projectRoot);

// Watch the shared package directory
config.watchFolders = [sharedPackage];

// Tell Metro where to find the shared package and its dependencies
config.resolver.extraNodeModules = {
  shared: sharedPackage,
  '@supabase/supabase-js': path.resolve(projectRoot, 'node_modules/@supabase/supabase-js'),
  'zod': path.resolve(projectRoot, 'node_modules/zod'),
};

module.exports = config;
