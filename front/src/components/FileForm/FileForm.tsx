import { Container } from "@mui/material";
import { ChooseAction } from "./components/ChooseAction";
import { ChooseFile } from "./components/ChooseFile";
import { ChooseKey } from "./components/ChooseKey";

export const FileForm = () => {
  const keys = ["Szyfruj", "Deszyfruj", "Podpisz", "Zweryfikuj podpis"];
  const cryptoKeys = ["Szyfruj", "Deszyfruj", "Podpisz", "Zweryfikuj podpis"];

  return (
    <Container maxWidth="md" sx={{ mt: 10 }}>
      <ChooseFile />
      <ChooseAction keys={keys} label="Wybierz metodę" />
      <ChooseKey keys={cryptoKeys} />
    </Container>
  );
};
