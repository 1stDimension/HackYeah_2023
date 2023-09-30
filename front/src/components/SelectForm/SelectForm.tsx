import { InputLabel, MenuItem, Select } from "@mui/material";
import { ISelectFormProps } from "./models";

export const SelectForm = ({
  name,
  options,
  handleChange,
  value,
  helperLabel,
}: ISelectFormProps) => (
  <>
    <Select
      // id="demo-simple-select"
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
  </>
);
