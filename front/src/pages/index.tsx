import type { ReactElement, ReactNode } from "react";
import type { NextPage } from "next";
import type { AppProps } from "next/app";
import { FileForm } from "@/components/FileForm";
import { useRouter } from "next/router";
import { Box } from "@mui/material";
import Image from "next/image";

export type NextPageWithLayout<P = {}, IP = P> = NextPage<P, IP> & {
  getLayout?: (page: ReactElement) => ReactNode;
};

type AppPropsWithLayout = AppProps & {
  Component: NextPageWithLayout;
};

export default function Index({ Component, pageProps }: AppPropsWithLayout) {
  const router = useRouter();
  const type = router.query.type as string;
  return (
    <Box>
      <FileForm actionType={type} />
    </Box>
  );
}
