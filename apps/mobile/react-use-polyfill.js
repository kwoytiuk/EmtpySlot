/**
 * Polyfill for React.use() (React 19 feature) for React 18
 * This is needed because expo-router 6.x uses React.use() but we're on React 18
 */

const React = require('react');

if (!React.use) {
  React.use = function use(usable) {
    // Handle promises (Suspense integration)
    if (usable && typeof usable.then === 'function') {
      throw usable;
    }

    // Handle Context - this is what expo-router uses
    if (usable && usable.$$typeof) {
      // It's a React Context object
      // Use React.useContext instead
      const ReactInternals = React.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED;
      if (ReactInternals && ReactInternals.ReactCurrentDispatcher) {
        const dispatcher = ReactInternals.ReactCurrentDispatcher.current;
        if (dispatcher && dispatcher.useContext) {
          return dispatcher.useContext(usable);
        }
      }
      // Fallback - just return the context (won't work but won't crash)
      return usable._currentValue || usable._currentValue2;
    }

    // For other "usables" with a read method
    if (usable && typeof usable.read === 'function') {
      return usable.read();
    }

    // Default: just return it
    return usable;
  };
}

module.exports = {};
