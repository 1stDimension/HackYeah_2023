import dynamic from "next/dynamic";
import React from "react";
import { IChooseFileProps } from "./models";
import { Box, Typography } from "@mui/material";
import { ErrorMessage } from "@/components/ErrorMessage";

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
  fullWidth,
  errorMessage,
}: IChooseFileProps) => (
  <Box sx={{ display: "block", position: "relative" }}>
    <MuiFileInput
      name={name}
      value={value}
      fullWidth={fullWidth}
      // TODO: work on types
      onChange={handleFileInput as any}
      error={!!errorMessage}
      InputProps={{
        style: {
          height: "150px",
          borderStyle: "dashed",
        },
      }}
    />
    <ErrorMessage message={errorMessage} />
  </Box>
);
