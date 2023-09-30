import Image from "next/image";
import styles from "./page.module.css";
import SearchBar from "@/components/SearchBar/SearchBar";
import { Container } from "@mui/material";

export default function Home() {
  return (
    <Container maxWidth="sm">
      <SearchBar />
    </Container>
  );
}
