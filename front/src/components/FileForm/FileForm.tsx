import {
  Box,
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
import axios, { AxiosResponse } from "axios";
import { redirect } from "next/navigation";

export const FileForm = ({ actionType }: { actionType: string }) => {
  const [cryptoKeys, setCryptoKeys] = useState<{ id: string; name: string }[]>(
    []
  );
  const [response, setResponse] = useState<AxiosResponse<any, any>>();

  useEffect(() => {
    const setKeys = async () => {
      if (!process.env.NEXT_PUBLIC_V1) return;

      const resp = await axios.get(process.env.NEXT_PUBLIC_V1 + "/keys");
      setCryptoKeys(resp.data);
    };
    setKeys();
  }, []);

  type formDataType = {
    keyType: string;
    file?: File;
  };
  type formErrorsType = {
    keyType: boolean;
    file: boolean;
  };

  const errorsMessages = {
    keyType: "Please choose key",
    file: "Please attach correct file",
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
  const handleSubmit = async (event: any) => {
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

    // if (Object.entries(errors).some((error) => error[0])) return;

    // API
    // formData + actionType
    if (!process.env.NEXT_PUBLIC_V1) return;

    const body = {
      ...formData,
    };

    const formdata = new FormData();
    formdata.append("key", formData.keyType);
    // @ts-ignore
    formdata.append("file", formData.file);
    console.log(formdata);
    // setResponse(
    axios
      .post(process.env.NEXT_PUBLIC_V1 + "/" + actionType, formdata, {
        responseType: "blob",
      })
      .then((response) => {
        // create file link in browser's memory
        const href = URL.createObjectURL(response.data);

        // create "a" HTML element with href to file & click
        const link = document.createElement("a");
        link.href = href;
        link.setAttribute(
          "download",
          response.headers["content-disposition"]
            .split(";")[1]
            .split("=")[1]
            .replace('"', "")
        ); //or any other extension
        document.body.appendChild(link);
        link.click();

        // clean up "a" element & remove ObjectURL
        document.body.removeChild(link);
        URL.revokeObjectURL(href);
      });
    // );
  };

  if (cryptoKeys.length === 0) return;

  // if (response)
  // redirect(
  //   "/success?succesMessage=Your%20eSignature%20has%20been%20verified%20successfully!"
  // );

  return (
    <Box>
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
              <Typography fontSize={12}>
                Upload a file for encryption
              </Typography>
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
                  options={cryptoKeys.map((key) => key.id)}
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
    </Box>
  );
};
