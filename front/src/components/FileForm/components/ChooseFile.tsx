import dynamic from "next/dynamic";
import React from "react";
import { IChooseFileProps } from "./models";

const MuiFileInput = dynamic(
  () => import("mui-file-input").then((mod) => mod.MuiFileInput),
  {
    ssr: false,
    loading: () => <p>Loading...</p>,
  }
);

export const ChooseFile = ({
  name,
  handleFileInput,
  value,
  label,
}: IChooseFileProps) => (
  <MuiFileInput
    name={name}
    value={value}
    // TODO: work on types
    onChange={handleFileInput as any}
    sx={{ display: "block" }}
  />
);
