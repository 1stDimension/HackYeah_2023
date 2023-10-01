import { Navbar } from "@/components/Navbar";
import "./globals.css";
import type { Metadata } from "next";
import { Inter } from "next/font/google";
import { Container } from "@mui/material";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Create Next App",
  description: "Generated by create next app",
};

export const DefaultLayout = ({ children }: { children: React.ReactNode }) => (
  <>
    <Navbar />
    <Container fixed>
      <div className={inter.className}>{children}</div>
    </Container>
  </>
);