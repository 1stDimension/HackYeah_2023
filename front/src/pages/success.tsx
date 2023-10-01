// const checkIfArrayContains = <T,>(array: T[], value: T) => {
import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutline";
import { Grid, Typography } from "@mui/material";
import { useRouter } from "next/router";

// };

const GenerateKeys = () => {
  const router = useRouter();
  const successMessage = router.query.succesMessage as string;
  console.log(router.query);
  return (
    <Grid
      container
      spacing={0}
      direction="column"
      alignItems="center"
      justifyContent="center"
      // sx={{ minHeight: "100vh" }}
    >
      <Grid
        item
        xs={3}
        display={"flex"}
        flexDirection={"column"}
        alignItems={"center"}
      >
        <CheckCircleOutlineIcon sx={{ color: "green", fontSize: "100px" }} />
        <Typography align={"center"} fontSize={16} fontWeight={700}>
          Success
        </Typography>
        <Typography>{successMessage}</Typography>
      </Grid>
    </Grid>
  );
};

export default GenerateKeys;
