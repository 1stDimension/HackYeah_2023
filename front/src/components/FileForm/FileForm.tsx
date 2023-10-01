import {
  Button,
  FormControl,
  FormGroup,
  Grid,
  Typography,
} from "@mui/material";
import { ChooseFile } from "./components/ChooseFile";
import { useEffect, useState } from "react";
import { SelectForm } from "../SelectForm";
import Image from "next/image";

export const FileForm = ({ actionType }: { actionType: string }) => {
  // const keys = ["Szyfruj", "Deszyfruj", "Podpisz", "Zweryfikuj podpis"];
  // TODO - get those from server
  const cryptoKeys = [
    "aaa",
    "Szyfruj",
    "Deszyfruj",
    "Podpisz",
    "Zweryfikuj podpis",
  ];

  type formDataType = {
    keyType: string;
    file?: File;
  };
  type formErrorsType = {
    keyType: boolean;
    file: boolean;
  };

  const errorsMessages = {
    keyType: "Proszę wybrać klucz z dostępnych w key_store",
    file: "Proszę załączyć poprawny plik",
  };
  const [errors, setErrors] = useState<formErrorsType>({
    keyType: false,
    file: false,
  });
  const [formData, setFormData] = useState<formDataType>({
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
    const tmpError = errors;
    Object.entries(formData).forEach((entry) => {
      const [name] = entry;
      // @ts-ignore
      if (!formData[name]) tmpError[name] = true;
      // @ts-ignore
      else tmpError[name] = false;
    });
    setErrors((prevErrors) => ({ ...prevErrors, ...tmpError }));

    if (Object.entries(errors).some((error) => error[0])) return;

    // API
    // formData + actionType
  };

  return (
    <form onSubmit={handleSubmit}>
      <FormControl fullWidth>
        <Grid container columnSpacing={3} rowSpacing={5}>
          <Grid item xs={4}>
            <Image
              src="/FileFormUpload.png"
              width={132}
              height={41}
              alt="Upload file image"
            />
            <Typography fontSize={12}>Upload a file for encryption</Typography>
          </Grid>
          <Grid item xs={8}>
            <FormGroup>
              <ChooseFile
                handleFileInput={handleFileInput}
                name="file"
                value={formData.file}
                fullWidth
                errorMessage={errors.file ? errorsMessages.file : undefined}
              />
            </FormGroup>
          </Grid>
          <Grid item xs={4}>
            <Image
              src="/FileFormChoose.png"
              width={158}
              height={62}
              alt="Choose key image"
            />
          </Grid>
          <Grid item xs={8}>
            <FormGroup>
              <SelectForm
                inputLabel="Choose your key_store"
                handleChange={handleInputChange}
                name="keyType"
                options={cryptoKeys}
                value={formData.keyType}
                fullWidth
                errorMessage={
                  errors.keyType ? errorsMessages.keyType : undefined
                }
              />
            </FormGroup>
          </Grid>
        </Grid>
        <FormGroup>
          <Button
            variant="contained"
            color="primary"
            type="submit"
            sx={{ my: 2 }}
          >
            {actionType || "Operate"}
          </Button>
        </FormGroup>
      </FormControl>
    </form>
  );
};
