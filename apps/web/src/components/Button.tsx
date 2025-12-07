'use client'

import { useState } from 'react'

export function Button() {
  const [count, setCount] = useState(0)

  return (
    <button
      onClick={() => setCount(count + 1)}
      style={{
        padding: '0.75rem 1.5rem',
        fontSize: '1rem',
        backgroundColor: '#0070f3',
        color: 'white',
        border: 'none',
        borderRadius: '0.5rem',
        cursor: 'pointer',
        transition: 'background-color 0.2s',
      }}
      onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#0051cc'}
      onMouseLeave={(e) => e.currentTarget.style.backgroundColor = '#0070f3'}
    >
      Clicked {count} times
    </button>
  )
}
