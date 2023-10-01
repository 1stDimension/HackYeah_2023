import * as React from "react";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Container from "@mui/material/Container";
import Button from "@mui/material/Button";
import Link from "next/link";
import Image from "next/image";
import { useRouter } from "next/router";

const pages = [
  { name: "Encrypt files", href: "/?type=encrypt" },
  { name: "Decrypt files", href: "/?type=decrypt" },
  { name: "create eSignature", href: "/?type=sign" },
  { name: "Verify eSignature", href: "/?type=verify-signature" },
  { name: "Generacja kluczy", href: "/generate-keys" },
];

export const Navbar = () => {
  const router = useRouter();
  const type = router.query.type as string;

  const imgAlt =
    type === "encrypt"
      ? "Encrypt logo"
      : type === "decrypt"
      ? "Decrypt logo"
      : type === "sign"
      ? "Create signature logo"
      : type === "verify-signature"
      ? "Verify signature logo"
      : "Generate keys";
  const imgPath =
    type === "encrypt"
      ? "/EncryptBanner.png"
      : type === "decrypt"
      ? "/DecryptBanner.png"
      : type === "sign"
      ? "/SignBanner.png"
      : type === "verify-signature"
      ? "/VerifyBanner.png"
      : "/GenerateBanner.png";
  return (
    <AppBar position="static" sx={{ mb: 3, backgroundColor: "white" }}>
      <Container maxWidth="xl">
        <Toolbar disableGutters>
          <Link href={"/"}>
            <Image
              src={"/BKGlogo.png"}
              style={{ padding: "12px" }}
              width={172}
              height={77}
              alt="BKG logo"
            />
          </Link>
          <Box
            sx={{
              flexGrow: 1,
              display: {
                xs: "none",
                md: "flex",
                justifyContent: "space-around",
              },
            }}
          >
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
      {imgPath && (
        <Image
          alt={imgAlt}
          width={0}
          height={0}
          sizes="100vw"
          style={{ width: "100vw", height: "auto", margin: "auto" }}
          src={imgPath}
        />
      )}
    </AppBar>
  );
};
