import * as React from "react";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import Button from "@mui/material/Button";
import AdbIcon from "@mui/icons-material/Adb";
import Link from "next/link";

const pages = [
  { name: "Operacje na dokumentach", href: "/" },
  { name: "Generacja kluczy", href: "/generate-keys" },
];

export const Navbar = () => (
  <AppBar position="static" sx={{ mb: 2 }}>
    <Container maxWidth="xl">
      <Toolbar disableGutters>
        <AdbIcon sx={{ display: { xs: "none", md: "flex" }, mr: 1 }} />
        <Typography
          variant="h6"
          noWrap
          component="a"
          href="#app-bar-with-responsive-menu"
          sx={{
            mr: 2,
            display: { xs: "none", md: "flex" },
            fontFamily: "monospace",
            fontWeight: 700,
            letterSpacing: ".3rem",
            color: "inherit",
            textDecoration: "none",
          }}
        >
          BKG
        </Typography>

        <AdbIcon sx={{ display: { xs: "flex", md: "none" }, mr: 1 }} />
        <Typography
          variant="h5"
          noWrap
          component="a"
          href="#app-bar-with-responsive-menu"
          sx={{
            mr: 2,
            display: { xs: "flex", md: "none" },
            flexGrow: 1,
            fontFamily: "monospace",
            fontWeight: 700,
            letterSpacing: ".3rem",
            color: "inherit",
            textDecoration: "none",
          }}
        >
          BKG
        </Typography>
        <Box sx={{ flexGrow: 1, display: { xs: "none", md: "flex" } }}>
          {pages.map((page, index) => (
            <Link href={page.href} key={page.name + index}>
              <Button sx={{ my: 2, color: "white", display: "block" }}>
                {page.name}
              </Button>
            </Link>
          ))}
        </Box>
      </Toolbar>
    </Container>
  </AppBar>
);
