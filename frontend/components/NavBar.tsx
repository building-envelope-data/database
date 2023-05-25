import * as React from "react";
import Link from "next/link";
import { useRouter } from "next/router";
import { Button, Menu } from "antd";
import { UserOutlined } from "@ant-design/icons";
import paths from "../paths";
import { useCurrentUserQuery } from "../queries/currentUser.graphql";
import { getXsrfToken } from "../lib/apollo";

type NavItemProps = {
  path: string;
  label: string;
};

export type NavBarProps = {
  items: NavItemProps[];
};

const moderatorItems = [
  {
    path: paths.userInfo,
    label: "User Info",
  },
  {
    path: paths.createData,
    label: "Create Data",
  },
  {
    path: paths.uploadFile,
    label: "Upload File",
  },
];

export default function NavBar({ items }: NavBarProps) {
  const router = useRouter();
  const currentUser = useCurrentUserQuery()?.data?.currentUser;

  return (
    <Menu mode="horizontal" selectedKeys={[router.pathname]} theme="dark">
      {items.map(({ path, label }) => (
        <Menu.Item key={path}>
          <Link href={path}>{label}</Link>
        </Menu.Item>
      ))}
      {currentUser ? (
        <Menu.SubMenu
          key="moderator"
          title={currentUser ? currentUser.name : null}
          icon={<UserOutlined />}
        >
          {moderatorItems.map(({ path, label }) => (
            <Menu.Item key={path}>
              <Link href={path}>{label}</Link>
            </Menu.Item>
          ))}
          <Menu.Item key={paths.logout}>
            <form action={paths.logout} method="post">
              <input
                name="__RequestVerificationToken"
                type="hidden"
                value={
                  typeof window !== "undefined"
                    ? getXsrfToken() ?? undefined
                    : ""
                }
              />
              <Button type="primary" htmlType="submit">
                Logout
              </Button>
            </form>
          </Menu.Item>
        </Menu.SubMenu>
      ) : (
        <Menu.Item key={paths.login}>
          <Link href={paths.login}>Login</Link>
        </Menu.Item>
      )}
    </Menu>
  );
}
