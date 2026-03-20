import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import Header from "./components/Header";
import Footer from "./components/Footer";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: {
    default: "Sean Bogolin | Backend-First Software Engineer",
    template: "%s | Sean Bogolin",
  },
  description:
    "Portfolio platform showcasing backend-first engineering, API design, architecture, and pragmatic software delivery.",
  openGraph: {
    title: "Sean Bogolin | Backend-First Software Engineer",
    description:
      "Portfolio platform showcasing backend-first engineering, API design, architecture, and pragmatic software delivery.",
    siteName: "Sean Bogolin Portfolio",
    type: "website",
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${geistSans.variable} ${geistMono.variable}`}>
        <div className="appShell">
          <Header />
          <main className="appMain">{children}</main>
          <Footer />
        </div>
      </body>
    </html>
  );
}
