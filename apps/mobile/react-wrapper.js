/**
 * React wrapper that ensures React.use() polyfill is always present
 */

// Load the real React
const React = require('react/cjs/react.development.js');

// Add polyfill if not present
if (!React.use) {
  React.use = function use(usable) {
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
}

module.exports = React;
