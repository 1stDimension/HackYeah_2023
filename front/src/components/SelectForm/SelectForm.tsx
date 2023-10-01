import { Box, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import { ISelectFormProps } from "./models";
import { ErrorMessage } from "../ErrorMessage";

export const SelectForm = ({
  inputLabel,
  name,
  options,
  handleChange,
  value,
  helperLabel,
  fullWidth,
  errorMessage,
}: ISelectFormProps) => (
  <Box sx={{ position: "relative" }}>
    <InputLabel id={`select-label-${inputLabel}`}>{inputLabel}</InputLabel>
    <Select
      labelId={`select-label-${inputLabel}`}
      id={`select-${inputLabel}`}
      name={name}
      value={value}
      label={inputLabel}
      onChange={handleChange}
      fullWidth={fullWidth}
      // error
    >
      {options.map((option, index) => (
        <MenuItem key={name + index} value={option}>
          {option}
        </MenuItem>
      ))}
    </Select>
    <ErrorMessage message={errorMessage} />
    {/* {helperLabel && (
      <FormHelperText id={`helper=${name}`}>{helperLabel}</FormHelperText>
    )} */}
  </Box>
);
