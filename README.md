# EmptySlot

A modern monorepo application with Next.js for web and React Native (Expo) for mobile, sharing common code and utilities.

## Project Structure

```
emtpy-slot/
├── apps/
│   ├── web/          # Next.js web application
│   └── mobile/       # React Native (Expo) mobile application
├── packages/
│   └── shared/       # Shared utilities, types, and constants
└── package.json      # Root package.json with workspace configuration
```

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- For mobile development:
  - iOS: Xcode and CocoaPods (macOS only)
  - Android: Android Studio and Android SDK
  - Or use Expo Go app for quick testing

### Installation

Install all dependencies:

```bash
npm install
```

### Development

#### Web Application

Run the Next.js development server:

```bash
npm run dev:web
```

Open [http://localhost:3000](http://localhost:3000) in your browser.

#### Mobile Application

Run the Expo development server:

```bash
npm run dev:mobile
```

Then:
- Press `i` for iOS simulator
- Press `a` for Android emulator
- Scan QR code with Expo Go app on your phone

### Building

#### Build Web Application

```bash
npm run build:web
```

#### Build Mobile Application

```bash
npm run build:mobile
```

## Workspaces

This project uses npm workspaces to manage the monorepo:

- **apps/web**: Next.js application with App Router and TypeScript
- **apps/mobile**: React Native application with Expo and Expo Router
- **packages/shared**: Shared code between web and mobile

## Shared Package

The `shared` package contains:

- **Utils**: Common utility functions (formatDate, capitalize, debounce)
- **Types**: TypeScript interfaces and types
- **Constants**: Shared constants (API endpoints, colors, etc.)

Import from shared package:

```typescript
import { formatDate, User, COLORS } from 'shared'
```

## Tech Stack

### Web
- Next.js 14 (App Router)
- React 18
- TypeScript
- CSS

### Mobile
- React Native
- Expo 50
- Expo Router
- TypeScript

### Shared
- TypeScript
- React types

## Scripts

- `npm run dev:web` - Start Next.js development server
- `npm run dev:mobile` - Start Expo development server
- `npm run build:web` - Build Next.js for production
- `npm run build:mobile` - Export Expo app
- `npm run lint` - Run linting across all workspaces
- `npm run type-check` - Run TypeScript type checking across all workspaces

## License

MIT
