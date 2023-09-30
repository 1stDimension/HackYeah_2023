import dynamic from "next/dynamic";
import React from "react";

const MuiFileInput = dynamic(
  () => import("mui-file-input").then((mod) => mod.MuiFileInput),
  {
    ssr: false,
    loading: () => <p>Loading...</p>,
  }
);

export const ChooseFile = () => {
  const [file, setFile] = React.useState(null);

  const handleChange = (newFile: any) => {
    setFile(newFile);
  };

  return (
    <MuiFileInput
      value={file}
      onChange={handleChange}
      sx={{ display: "block" }}
    />
  );
};
