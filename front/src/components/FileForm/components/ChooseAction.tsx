import * as React from "react";
import Radio from "@mui/material/Radio";
import RadioGroup from "@mui/material/RadioGroup";
import FormControlLabel from "@mui/material/FormControlLabel";
import FormControl from "@mui/material/FormControl";
import FormLabel from "@mui/material/FormLabel";
import { IChooseActionProps } from "./models";

export const ChooseAction = ({
  name,
  handleOptionChange,
  value,
  keys,
  label,
}: IChooseActionProps) => {
  //   const keys = ["Szyfruj", "Deszyfruj", "Podpisz", "Zweryfikuj podpis"];

  return (
    <FormControl sx={{ display: "block" }}>
      <FormLabel id="demo-row-radio-buttons-group-label">{label}</FormLabel>
      <RadioGroup
        name={name}
        onChange={handleOptionChange}
        row
        aria-labelledby="demo-row-radio-buttons-group-label"
        value={value}
      >
        {keys.map((key, index) => (
          <FormControlLabel
            value={key}
            control={<Radio />}
            label={key}
            key={key + index}
          />
        ))}
      </RadioGroup>
    </FormControl>
  );
};
