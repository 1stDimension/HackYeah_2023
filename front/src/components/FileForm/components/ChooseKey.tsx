import * as React from "react";
import FormControl from "@mui/material/FormControl";
import { IChooseKeyProps } from "./models";

export const ChooseKey = ({ keys }: IChooseKeyProps) => {
  return (
    <FormControl sx={{ display: "block" }}>
      {keys.map((key) => (
        <p key={key}>{key}</p>
      ))}
      {/* <FormLabel id="demo-row-radio-buttons-group-label">{label}</FormLabel>
      <RadioGroup
        row
        aria-labelledby="demo-row-radio-buttons-group-label"
        name="row-radio-buttons-group"
      ></RadioGroup> */}
    </FormControl>
  );
};
