import * as React from "react";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Container from "@mui/material/Container";
import Button from "@mui/material/Button";
import Link from "next/link";
import Image from "next/image";

const pages = [
  { name: "Encrypt files", href: "/?type=encrypt" },
  { name: "Decrypt files", href: "/?type=decrypt" },
  { name: "create eSignature", href: "/?type=sign" },
  { name: "Verify eSignature", href: "/?type=verify-signature" },
  { name: "Generacja kluczy", href: "/generate-keys" },
];

export const Navbar = () => (
  <AppBar position="static" sx={{ mb: 2, backgroundColor: "white" }}>
    <Container maxWidth="xl">
      <Toolbar disableGutters>
        <Image src={"/BKGlogo.png"} width={172} height={77} alt="BKG logo" />
        <Box sx={{ flexGrow: 1, display: { xs: "none", md: "flex" } }}>
          {pages.map((page, index) => (
            <Link href={page.href} key={page.name + index}>
              <Button
                sx={{
                  fontWeight: 700,
                  my: 2,
                  color: "black",
                  display: "block",
                }}
              >
                {page.name}
              </Button>
            </Link>
          ))}
        </Box>
      </Toolbar>
    </Container>
  </AppBar>
);
