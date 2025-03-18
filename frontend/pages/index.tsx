import Layout from "../components/Layout";
import { Typography } from "antd";
import Link from "next/link";
import paths from "../paths";
import Image from "next/image";
import overviewImage from "../public/overview.png";

function Page() {
  return (
    <Layout>
      <div style={{ maxWidth: 768 }}>
        <Typography.Paragraph>
          <Link href={paths.home}>buildingenvelopedata-dev.c3rro.com</Link> is the
          website of the product data server of{" "}
          <Typography.Link href="https://c3rro.com/eqwin-p/">
            c3rro - eqwin-p
          </Typography.Link>{" "}
          at{" "}
          <Typography.Link href="https://www.c3rro.com">
            c3rro
          </Typography.Link>
          . The product data server is part of the Product Data Network{" "}
          <Typography.Link href={`${process.env.NEXT_PUBLIC_METABASE_URL}`}>
            buildingenvelopedata.org
          </Typography.Link>
          . The product data server is an instance of the{" "}
          <Typography.Link href="https://github.com/c3rrolutions/hygrothermaldatabase">
            implementation of a database
          </Typography.Link>
          . a Fork of the {" "}
          <Typography.Link href="https://github.com/building-envelope-data/database">
            reference implementation of a database
          </Typography.Link>{" "}
          which is open-source with a permissive license, so that everyone can easily
          create their own product data server as part of the Product Data
          Network{" "}
          <Typography.Link href={`${process.env.NEXT_PUBLIC_METABASE_URL}`}>
            buildingenvelopedata.org
          </Typography.Link>
          .
        </Typography.Paragraph>
        <Typography.Paragraph>
          This website is the frontend of the product data server of{" "}
          <Typography.Link href="https://c3rro.com/eqwin-p/">
           c3rro - eqwin-p
          </Typography.Link>
          . You can use this website to search this product data server for{" "}
          <Link href={paths.hygrothermalData}>hygrothermal data</Link>. If you
          would like to search the entire Product Data Network{" "}
          <Typography.Link href={`${process.env.NEXT_PUBLIC_METABASE_URL}`}>
            buildingenvelopedata.org
          </Typography.Link>
          , you can search there for example for{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/data/hygrothermal`}
          >
            hygrothermal data{" "}
          </Typography.Link>
          . You will find there also an overview about all building envelope{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/components`}
          >
            {" "}
            components
          </Typography.Link>
          ,{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/institutions`}
          >
            institutions
          </Typography.Link>
          ,{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/data-formats`}
          >
            data formats
          </Typography.Link>
          ,{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/methods`}
          >
            methods
          </Typography.Link>{" "}
          and{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/databases`}
          >
            databases
          </Typography.Link>{" "}
          of the Product Data Network{" "}
          <Typography.Link href={`${process.env.NEXT_PUBLIC_METABASE_URL}`}>
            buildingenvelopedata.org
          </Typography.Link>
          .
        </Typography.Paragraph>
        <Typography.Paragraph>
          This website is completely based on the
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_DATABASE_URL}/graphql/`}
          >
            {" "}
            GraphQL endpoint
          </Typography.Link>
          . The{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_DATABASE_URL}/graphql/`}
          >
            GraphQL endpoint
          </Typography.Link>{" "}
          is the Application Programming Interface (API) to the backend of the
          product data server. If you like programming, you can use the endpoint
          to automate the interaction with this product data server. You find
          the{" "}
          <Typography.Link href="https://github.com/building-envelope-data/api">
            specification of the API on GitHub
          </Typography.Link>{" "}
          which is especially helpful if you develop software to plan buildings.
          The{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/graphql/`}
          >
            GraphQL endpoint of the metabase
          </Typography.Link>{" "}
          is then a convenient way to query for data, components, institutions,
          databases, data formats and methods.
        </Typography.Paragraph>
        <Typography.Paragraph>
          The data format{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/data-formats/9ca9e8f5-94bf-4fdd-81e3-31a58d7ca708`}
          >
            BED-JSON
          </Typography.Link>{" "}
          is a general format for optical, calorimetric and photovoltaic data
          sets. It is defined by the{" "}
          <Typography.Link href="https://github.com/building-envelope-data/api/tree/develop/schemas">
            JSON Schemas of the API specification
          </Typography.Link>
          . Other{" "}
          <Typography.Link
            href={`${process.env.NEXT_PUBLIC_METABASE_URL}/data-formats`}
          >
            data formats
          </Typography.Link>{" "}
          are available, too.
        </Typography.Paragraph>
        <Image
          src={overviewImage}
          alt="Schematic depiction of how users like architects, planners, or engineers can use the metabase to find products and data in and across databases."
          style={{
            maxWidth: "100%",
            height: "auto",
          }} />
      </div>
    </Layout>
  );
}

export default Page;
