import type { Metadata } from 'next'
import './globals.css'

export const metadata: Metadata = {
  title: 'EmptySlot',
  description: 'A Next.js and React Native monorepo application',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  )
}
