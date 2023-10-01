import { ResponseModal } from "@/components/ResponseModal";
import { SelectForm } from "@/components/SelectForm";
import { TextFieldForm } from "@/components/TextFieldForm";
import { postKeyParams } from "@/services/keyGenerator";
import { Box, Button, FormControl, FormGroup, Grid } from "@mui/material";
import { AxiosResponse } from "axios";
import { useState } from "react";
import Image from "next/image";

export const GenerateKeysForm = () => {
  // TODO - get options from this form
  const options = {
    RSA: [2048, 3072, 4096],
    ECDSA: [256, 384, 521],
    AES: [128, 192, 256],
  };

  type formDataType = {
    keyName: string;
    alghoritmType: keyof typeof options;
    keyLength: number;
  };
  type formErrorsType = {
    keyName: boolean;
    alghoritmType: boolean;
    keyLength: boolean;
  };

  const [response, setResponse] = useState<AxiosResponse<any, any>>();
  const [formData, setFormData] = useState<formDataType>({
    keyName: "",
    alghoritmType: Object.keys(options)[0] as keyof typeof options,
    keyLength: 2048,
  });

  const errorsMessages = {
    keyName: "Please input key name",
    keyLength: "Please choose key length",
    alghoritmType: "Please choose alghoritm type",
  };
  const [errors, setErrors] = useState<formErrorsType>({
    keyName: false,
    keyLength: false,
    alghoritmType: false,
  });
  const handleInputChange = (event: any) => {
    const { name, value } = event.target;
    setFormData({ ...formData, [name]: value });
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
    setErrors(tmpError);

    // console.log(errors);
    // if (Object.entries(errors).some((error) => error[0])) return;

    setResponse(await postKeyParams(formData));
  };

  return (
    <Box>
      {response ? (
        <ResponseModal response={response} />
      ) : (
        <form onSubmit={handleSubmit}>
          <FormControl sx={{ display: "block" }}>
            <Grid container columnSpacing={3} rowSpacing={5}>
              <Grid item xs={4}>
                <Image
                  src="/keyFormName.png"
                  width={130}
                  height={41}
                  alt="Upload file image"
                />
              </Grid>
              <Grid item xs={8}>
                <TextFieldForm
                  name="keyName"
                  inputLabel="Nazwa klucza"
                  handleInputChange={handleInputChange}
                  value={formData.keyName}
                  helperLabel="Wprowadź nazwę klucza generowanego do key_store"
                  errorMessage={
                    errors.keyName ? errorsMessages.keyName : undefined
                  }
                />
              </Grid>
              <Grid item xs={4}>
                <Image
                  src="/keyFormAlghoritm.png"
                  width={184}
                  height={41}
                  alt="Upload file image"
                />
              </Grid>
              <Grid item xs={8}>
                <SelectForm
                  inputLabel="typ algorytmu"
                  name="alghoritmType"
                  options={Object.keys(options)}
                  handleChange={handleInputChange}
                  value={formData.alghoritmType}
                  fullWidth
                  errorMessage={
                    errors.alghoritmType
                      ? errorsMessages.alghoritmType
                      : undefined
                  }
                />
              </Grid>
              <Grid item xs={4}>
                <Image
                  src="/keyFormLength.png"
                  width={132}
                  height={41}
                  alt="Upload file image"
                />
              </Grid>
              <Grid item xs={8}>
                {Object.keys(options).includes(formData.alghoritmType) && (
                  <SelectForm
                    inputLabel="długość klucza"
                    name="keyLength"
                    options={options[formData.alghoritmType]}
                    handleChange={handleInputChange}
                    value={formData.keyLength}
                    fullWidth
                    errorMessage={
                      errors.keyLength ? errorsMessages.keyLength : undefined
                    }
                  />
                )}
              </Grid>
            </Grid>
            <FormGroup>
              <Button
                variant="contained"
                color="primary"
                type="submit"
                sx={{ my: 2 }}
              >
                Generate keys
              </Button>
            </FormGroup>
          </FormControl>
        </form>
      )}
    </Box>
  );
};
