/**
 * Polyfill for React.use() (React 19 feature) for React 18
 * This is needed because expo-router 6.x uses React.use() but we're on React 18
 */

import React from 'react';

if (!React.use) {
  React.use = function use(usable) {
    if (usable && typeof usable.then === 'function') {
      // It's a promise - this is a simplified polyfill
      // In a real app, you'd want proper Suspense integration
      throw usable; // Let Suspense handle it
    }

    if (usable && typeof usable.read === 'function') {
      // It's a "usable" with a read method (like a Context)
      return usable.read();
    }

    // If it's a Context, just return it
    return usable;
  };
}
