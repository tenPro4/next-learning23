import './globals.css'
import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import { ClerkProvider } from '@clerk/nextjs'
import { cn } from '@/lib/utils'
import { ThemeProvider } from "@/components/providers/theme-provider"
import { ToastProvider } from '@/components/providers/toaster-provider'

const inter = Inter({ subsets: ['latin'] })

export const metadata: Metadata = {
  title: 'Next Self Practice 2023',
  description: 'Generated by create next app',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <ClerkProvider>
    <html lang="en">
      <body className={
        cn(
          inter.className,
          "bg-white dark:bg-[#313338]"
        )
      }>
        <ThemeProvider
            attribute="class"
            defaultTheme="light"
            enableSystem={false}
            storageKey="sl-theme"
            disableTransitionOnChange
          >
            <ToastProvider />
            {children}
          </ThemeProvider>
      </body>
    </html>
    </ClerkProvider>
  )
}
