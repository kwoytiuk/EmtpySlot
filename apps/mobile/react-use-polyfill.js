/**
 * Polyfill for React.use() - MUST RUN BEFORE ANYTHING ELSE
 * Patches BOTH react and react-dom's internal React
 */

function addReactUsePolyfill(React) {
  if (React && !React.use) {
    console.log('🔧 Adding React.use() polyfill to', React.version || 'React');

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

    console.log('✅ React.use() polyfill installed for', React.version || 'React');
    return true;
  }
  return false;
}

// Patch the main React module
try {
  const React = require('react');
  addReactUsePolyfill(React);
} catch (e) {
  console.error('Failed to patch react:', e);
}

// Patch react-dom's internal React
try {
  const ReactDOM = require('react-dom');
  if (ReactDOM && ReactDOM.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED) {
    const internals = ReactDOM.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED;
    if (internals.React) {
      addReactUsePolyfill(internals.React);
    }
  }
} catch (e) {
  console.error('Failed to patch react-dom:', e);
}

// Also try to patch react-dom/server
try {
  const ReactDOMServer = require('react-dom/server');
  if (ReactDOMServer && ReactDOMServer.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED) {
    const internals = ReactDOMServer.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED;
    if (internals.React) {
      addReactUsePolyfill(internals.React);
    }
  }
} catch (e) {
  // react-dom/server might not be loaded yet, that's okay
  console.log('react-dom/server not loaded yet (this is normal)');
}

module.exports = {};
