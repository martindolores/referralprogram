import { useState } from "react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ConfigProvider, Layout, Menu, theme } from "antd";
import { GiftOutlined, SettingOutlined } from "@ant-design/icons";
import { ReferralForm } from "./components/ReferralForm";
import { AdminPanel } from "./components/AdminPanel";
import "./App.css";

const { Header, Content, Footer } = Layout;

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      retry: 1,
    },
  },
});

type Page = "refer" | "admin";

function App() {
  const [currentPage, setCurrentPage] = useState<Page>("refer");

  const menuItems = [
    {
      key: "refer",
      icon: <GiftOutlined />,
      label: "Refer a Friend",
    },
    {
      key: "admin",
      icon: <SettingOutlined />,
      label: "Admin",
    },
  ];

  return (
    <QueryClientProvider client={queryClient}>
      <ConfigProvider
        theme={{
          algorithm: theme.defaultAlgorithm,
          token: {
            colorPrimary: "#e91e63",
            borderRadius: 8,
          },
        }}
      >
        <Layout style={{ minHeight: "100vh" }}>
          <Header
            style={{ display: "flex", alignItems: "center", padding: "0 24px" }}
          >
            <div
              style={{
                color: "white",
                fontSize: 20,
                fontWeight: "bold",
                marginRight: 24,
              }}
            >
              🍰 Lorna's Baked Delights
            </div>
            <Menu
              theme="dark"
              mode="horizontal"
              selectedKeys={[currentPage]}
              items={menuItems}
              onClick={(e) => setCurrentPage(e.key as Page)}
              style={{ flex: 1, minWidth: 0 }}
            />
          </Header>

          <Content style={{ padding: "24px 48px" }}>
            <div style={{ maxWidth: 600, margin: "0 auto" }}>
              {currentPage === "refer" && <ReferralForm />}
              {currentPage === "admin" && <AdminPanel />}
            </div>
          </Content>

          <Footer style={{ textAlign: "center" }}>
            Lorna's Baked Delights © {new Date().getFullYear()} - Referral
            Program
          </Footer>
        </Layout>
      </ConfigProvider>
    </QueryClientProvider>
  );
}

export default App;
