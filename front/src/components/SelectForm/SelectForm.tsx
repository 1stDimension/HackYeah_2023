import { FormGroup, InputLabel, MenuItem, Select } from "@mui/material";
import { ISelectFormProps } from "./models";

export const SelectForm = ({
  inputLabel,
  name,
  options,
  handleChange,
  value,
  helperLabel,
}: ISelectFormProps) => (
  <FormGroup sx={{ my: 1 }}>
    <InputLabel id={`select-label-${inputLabel}`}>{inputLabel}</InputLabel>
    <Select
      labelId={`select-label-${inputLabel}`}
      id={`select-${inputLabel}`}
      name={name}
      value={value}
      label="Age"
      onChange={handleChange}
    >
      {options.map((option, index) => (
        <MenuItem key={name + index} value={option}>
          {option}
        </MenuItem>
      ))}
    </Select>
    {/* {helperLabel && (
      <FormHelperText id={`helper=${name}`}>{helperLabel}</FormHelperText>
    )} */}
  </FormGroup>
);
