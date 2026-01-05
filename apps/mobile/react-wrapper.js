/**
 * React wrapper that adds React.use() polyfill
 * This intercepts all React imports and ensures the polyfill is present
 */

// Get the actual React module
const ActualReact = require('react/index.js');

// Add polyfill if not present
if (!ActualReact.use) {
  console.log('🔧 Patching React.use() in react-wrapper');

  ActualReact.use = function use(usable) {
    // Handle React Context
    if (usable && typeof usable === 'object' && usable.$$typeof) {
      if ('_currentValue' in usable) {
        return usable._currentValue;
      }
      if ('_currentValue2' in usable) {
        return usable._currentValue2;
      }
    }

    // Handle promises
    if (usable && typeof usable.then === 'function') {
      throw usable;
    }

    // Handle readables
    if (usable && typeof usable.read === 'function') {
      return usable.read();
    }

    return usable;
  };

  console.log('✅ React.use() polyfill applied in react-wrapper');
}

// Export everything from React
module.exports = ActualReact;
