import { Button } from '@/components/Button'

export default function Home() {
  return (
    <main style={{
      minHeight: '100vh',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      padding: '2rem'
    }}>
      <h1 style={{ fontSize: '3rem', fontWeight: 'bold', marginBottom: '2rem' }}>
        Welcome to EmptySlot
      </h1>
      <p style={{ fontSize: '1.25rem', marginBottom: '2rem', textAlign: 'center' }}>
        A modern monorepo with Next.js for web and React Native for mobile
      </p>
      <Button />
    </main>
  )
}
