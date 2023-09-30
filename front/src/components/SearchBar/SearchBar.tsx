import { Container, InputAdornment, TextField } from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";

export default function SearchBar() {
  return (
    // <Container maxWidth="md" sx={{ mt: 20 }}>
    <TextField
      id="search"
      type="search"
      label="Search"
      // value={searchTerm}
      // onChange={handleChange}
      sx={{ width: 600 }}
      InputProps={{
        endAdornment: (
          <InputAdornment position="end">
            <SearchIcon />
          </InputAdornment>
        ),
      }}
    />
    // </Container>
  );
}
