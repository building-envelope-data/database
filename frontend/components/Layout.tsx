import Head from "next/head";
import { ReactNode, useEffect, useState } from "react";
import Footer from "./Footer";
import NavBar from "./NavBar";
import { Modal, Layout as AntLayout, Typography } from "antd";
import paths from "../paths";
import { useCookies } from "react-cookie";

const navItems = [
  {
    path: paths.home,
    label: "Home",
  },
  {
    path: paths.data,
    label: "All Data",
  },
  {
    path: paths.calorimetricData,
    label: "Calorimetric Data",
  },
  {
    path: paths.geometricData,
    label: "Geometric Data",
  },
  {
    path: paths.hygrothermalData,
    label: "Hygrothermal Data",
  },
  {
    path: paths.opticalData,
    label: "Optical Data",
  },
  {
    path: paths.photovoltaicData,
    label: "Photovoltaic Data",
  },
  {
    path: paths.database,
    label: "Database",
  },
];

export type LayoutProps = {
  children?: ReactNode;
};

const cookieConsentName = "consent";
const cookieConsentValue = "yes";

export default function Layout({ children }: LayoutProps) {
  const appTitle = "TestLab Solar Façades";

  const [cookies, setCookie] = useCookies([cookieConsentName]);
  const shouldShowCookieConsent =
    cookies[cookieConsentName] != cookieConsentValue;
  const [loadedAntiforgeryToken, setLoadedAntiforgeryToken] = useState(false);

  useEffect(() => {
    if (shouldShowCookieConsent) {
      Modal.info({
        title: "Cookie Consent",
        content: (
          <Typography.Paragraph>
            This website employs cookies to make it work securely. As these
            cookies are essential you need to agree to their usage to use this
            website.
          </Typography.Paragraph>
        ),
        okText: "I agree",
        onOk: () => {
          setCookie(cookieConsentName, cookieConsentValue);
        },
      });
    }
  }, [shouldShowCookieConsent, setCookie]);

  useEffect(() => {
    fetch(paths.antiforgeryToken).then((_) => {
      setLoadedAntiforgeryToken(true);
    });
  }, []);

  if (!loadedAntiforgeryToken) {
    return null;
  }

  return (
    <AntLayout>
      <Head>
        <title>{appTitle}</title>
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <meta charSet="utf-8" />
      </Head>
      <AntLayout.Header>
        <NavBar items={navItems} />
      </AntLayout.Header>
      <AntLayout.Content style={{ padding: "50px" }}>
        {children}
      </AntLayout.Content>
      <AntLayout.Footer>
        <Footer />
      </AntLayout.Footer>
    </AntLayout>
  );
}
