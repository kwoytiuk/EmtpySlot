/**
 * Polyfill for React.use() - MUST RUN BEFORE ANYTHING ELSE
 */
const React = require('react');

if (!React.use) {
  console.log('🔧 Adding React.use() polyfill for React 18');

  React.use = function use(usable) {
    // Handle React Context (what expo-router uses)
    if (usable && typeof usable === 'object' && usable.$$typeof) {
      // Get the current context value
      // In React 18, context values are stored in _currentValue or _currentValue2
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

    // Handle objects with a read() method
    if (usable && typeof usable.read === 'function') {
      return usable.read();
    }

    // Default: return as-is
    return usable;
  };

  console.log('✅ React.use() polyfill installed');
} else {
  console.log('✅ React.use() already exists');
}
