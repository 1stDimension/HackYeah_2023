import { SelectForm } from "@/components/SelectForm";
import { TextFieldForm } from "@/components/TextFieldForm";
import { Button, FormControl, FormGroup } from "@mui/material";
import { useState } from "react";

// const checkIfArrayContains = <T,>(array: T[], value: T) => {

// };

const GenerateKeys = () => {
  const options = { RSA: [2048, 4096], ECDSA: [512], AES: [128] };
  const [formData, setFormData] = useState({
    keyName: "",
    alghoritmType: "",
    keyLength: 0,
  });

  const handleInputChange = (event: any) => {
    const { name, value } = event.target;
    setFormData({ ...formData, [name]: value });
  };

  // TODO: create dynamic <T> event type
  const handleSubmit = (event: any) => {
    event.preventDefault();
    console.log(formData);
  };

  return (
    <form onSubmit={handleSubmit}>
      <FormControl>
        <TextFieldForm
          name="keyName"
          inputLabel="Nazwa klucza"
          handleInputChange={handleInputChange}
          value={formData.keyName}
          helperLabel="Wprowadź nazwę klucza generowanego do key_store"
        />
        <SelectForm
          name="alghoritmType"
          options={Object.keys(options)}
          handleChange={handleInputChange}
          value={formData.alghoritmType}
        />
        {Object.keys(options).includes(formData.alghoritmType) && (
          <SelectForm
            name="keyLength"
            // @ts-expect-error - TODO: to place handler that check types
            options={options[formData.alghoritmType]}
            handleChange={handleInputChange}
            value={formData.keyLength}
          />
        )}

        <FormGroup>
          <Button variant="contained" color="primary" type="submit">
            Submit
          </Button>
        </FormGroup>
      </FormControl>
    </form>
  );
};

export default GenerateKeys;
