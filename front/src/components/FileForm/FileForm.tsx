import { Button, Container, FormControl, FormGroup } from "@mui/material";
import { ChooseAction } from "./components/ChooseAction";
import { ChooseFile } from "./components/ChooseFile";
import { ChooseKey } from "./components/ChooseKey";
import { useState } from "react";
import { SelectForm } from "../SelectForm";

export const FileForm = () => {
  const keys = ["Szyfruj", "Deszyfruj", "Podpisz", "Zweryfikuj podpis"];
  const cryptoKeys = ["Szyfruj", "Deszyfruj", "Podpisz", "Zweryfikuj podpis"];

  type formDataType = {
    actionType: string;
    keyType: string;
    file?: File;
  };

  const [formData, setFormData] = useState<formDataType>({
    actionType: "",
    keyType: "",
    file: undefined,
  });

  const handleInputChange = (event: any) => {
    const { name, value } = event.target;
    setFormData({ ...formData, [name]: value });
  };
  const handleFileInput = (file: File) => {
    setFormData({ ...formData, file: file });
  };

  // TODO: create dynamic <T> event type
  const handleSubmit = (event: any) => {
    event.preventDefault();
    console.log(formData);
  };

  return (
    <form onSubmit={handleSubmit}>
      {/* <Container maxWidth="md" sx={{ mt: 10 }}> */}
      <FormControl>
        <FormGroup>
          <ChooseFile
            handleFileInput={handleFileInput}
            name="file"
            value={formData.file}
          />
          <ChooseAction
            handleOptionChange={handleInputChange}
            name="actionType"
            keys={keys}
            value={formData.actionType}
            label="Wybierz metodę"
          />
          <SelectForm
            inputLabel="Wybierz swj klucz z key_store"
            handleChange={handleInputChange}
            name="keyType"
            options={cryptoKeys}
            value={formData.keyType}
          />
        </FormGroup>
        <FormGroup>
          <Button variant="contained" color="primary" type="submit">
            Submit
          </Button>
        </FormGroup>
      </FormControl>
    </form>
  );
};
