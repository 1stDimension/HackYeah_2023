import { Typography } from "@mui/material";

export const ErrorMessage = ({ message }: { message?: string }) => {
  return (
    <Typography variant="caption" fontSize={"12px"} color={"red"}>
      {message}
    </Typography>
  );
};
