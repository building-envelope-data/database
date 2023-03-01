import { Voyager } from "graphql-voyager";

const url = `${process.env.NEXT_PUBLIC_DATABASE_URL}/graphql/`;

// useEffect(() => {
// const fetchData = async () => {
// try {
// setLoading(true);
// const result = await fetch(url, {
// method: "post",
// headers: { "Content-Type": "application/json" },
// body: JSON.stringify({ query: query }),
// });
// if (result.ok) {
// setData(await result.json());
// } else {
// message.error(errorMessage);
// }
// } catch (error) {
// console.log(error);
// message.error(errorMessage);
// } finally {
// setLoading(false);
// }
// };
// fetchData();
// }, []);

function introspectionProvider(query: JSON): Promise<JSON> {
  return fetch(url, {
    method: "post",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ query: query }),
  }).then((response) => response.json());
}

function Page() {
  return (
    <Voyager
      introspection={introspectionProvider}
      displayOptions={{
        skipRelay: true,
        skipDeprecated: true,
        sortByAlphabet: true,
        showLeafFields: true,
        hideRoot: false,
      }}
      hideDocs={false}
      hideSettings={false}
    />
  );
}

export default Page;
