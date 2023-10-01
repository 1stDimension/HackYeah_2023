import * as React from "react";
import Radio from "@mui/material/Radio";
import RadioGroup from "@mui/material/RadioGroup";
import FormControlLabel from "@mui/material/FormControlLabel";
import FormLabel from "@mui/material/FormLabel";
import { IChooseActionProps } from "./models";
import { Box } from "@mui/material";
import { ErrorMessage } from "@/components/ErrorMessage";

export const ChooseAction = ({
  name,
  handleOptionChange,
  value,
  keys,
  label,
  errorMessage,
}: IChooseActionProps) => {
  //   const keys = ["Szyfruj", "Deszyfruj", "Podpisz", "Zweryfikuj podpis"];

  return (
    <Box sx={{ width: "100%", position: "relative" }}>
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
      <ErrorMessage message={errorMessage} />
    </Box>
  );
};
