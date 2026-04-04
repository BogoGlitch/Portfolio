import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import Header from "./components/Header";
import Footer from "./components/Footer";
import CommandPalette from "./components/CommandPalette";
import { getProjects, getSkills } from "@/lib/api";

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

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  // Fetch data server-side so CommandPalette has everything it needs
  // without the client making extra requests on Cmd+K
  const [projects, skills] = await Promise.allSettled([
    getProjects(),
    getSkills(),
  ]);

  const paletteProjects = projects.status === "fulfilled"
    ? projects.value.map((p) => ({ type: "project" as const, name: p.name, slug: p.slug, description: p.shortDescription }))
    : [];

  const paletteSkills = skills.status === "fulfilled"
    ? skills.value.map((s) => ({ type: "skill" as const, name: s.name, slug: s.slug, description: s.description ?? undefined }))
    : [];

  return (
    <html lang="en" data-theme="cosmos" suppressHydrationWarning>
      <head>
        {/* Prevent theme flash — reads localStorage before React hydrates */}
        <script
          dangerouslySetInnerHTML={{
            __html: `try{var t=localStorage.getItem('portfolio-theme');if(t)document.documentElement.setAttribute('data-theme',t);}catch(e){}`,
          }}
        />
      </head>
      <body className={`${geistSans.variable} ${geistMono.variable}`}>
        <div className="appShell">
          <Header />
          <main className="appMain">{children}</main>
          <Footer />
        </div>
        <CommandPalette projects={paletteProjects} skills={paletteSkills} />
      </body>
    </html>
  );
}
