import type { ReactElement, ReactNode } from "react";
import type { NextPage } from "next";
import type { AppProps } from "next/app";
import { DefaultLayout } from "@/Layouts/DefaultLayout";
import { Container } from "@mui/material";
import { FileForm } from "@/components/FileForm";

export type NextPageWithLayout<P = {}, IP = P> = NextPage<P, IP> & {
  getLayout?: (page: ReactElement) => ReactNode;
};

type AppPropsWithLayout = AppProps & {
  Component: NextPageWithLayout;
};

export default function MyApp({ Component, pageProps }: AppPropsWithLayout) {
  // Use the layout defined at the page level, if available
  //   const getLayout = Component.getLayout ?? ((page) => page);

  //   return getLayout(<Component {...pageProps} />);
  //   return <div>awd</div>;

  return (
    <DefaultLayout>
      <Component {...pageProps} />
    </DefaultLayout>
  );
}
