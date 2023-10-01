import { Box, Typography } from "@mui/material";
import { AxiosResponse } from "axios";

export const ResponseModal = ({
  response,
}: {
  response: AxiosResponse<any, any>;
}) => (
  <Box>
    <Typography>{response.status}</Typography>
    <Typography>{response.statusText}</Typography>
    <Typography>{JSON.stringify(response.data)}</Typography>
  </Box>
);
