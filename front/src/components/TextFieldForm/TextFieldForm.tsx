import {
  Box,
  FormHelperText,
  Input,
  InputLabel,
  TextField,
} from "@mui/material";
import { ITextFieldFormProps } from "./models";
import { ErrorMessage } from "../ErrorMessage";

export const TextFieldForm = ({
  name,
  inputLabel,
  handleInputChange,
  value,
  helperLabel,
  fullWidth,
  errorMessage,
}: ITextFieldFormProps) => (
  <Box>
    <TextField
      label={inputLabel}
      name={name}
      value={value}
      onChange={handleInputChange}
      id={`tff-${name}`}
      aria-describedby={`helper=${name}`}
      fullWidth
    />
    {helperLabel && (
      <FormHelperText id={`helper=${name}`}>{helperLabel}</FormHelperText>
    )}
    {errorMessage && <ErrorMessage message={errorMessage} />}
  </Box>
);
