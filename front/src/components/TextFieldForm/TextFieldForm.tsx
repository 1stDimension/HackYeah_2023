import { FormHelperText, Input, InputLabel } from "@mui/material";
import { ITextFieldFormProps } from "./models";

export const TextFieldForm = ({
  name,
  inputLabel,
  handleInputChange,
  value,
  helperLabel,
}: ITextFieldFormProps) => (
  <>
    <InputLabel htmlFor={`tff-${name}`}>{inputLabel}</InputLabel>
    <Input
      name={name}
      value={value}
      onChange={handleInputChange}
      id={`tff-${name}`}
      aria-describedby={`helper=${name}`}
    />
    {helperLabel && (
      <FormHelperText id={`helper=${name}`}>{helperLabel}</FormHelperText>
    )}
  </>
);
