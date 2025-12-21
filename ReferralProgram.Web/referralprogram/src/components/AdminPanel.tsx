import { useState } from "react";
import { useForm, Controller } from "react-hook-form";
import {
  Form,
  Input,
  Button,
  Card,
  Typography,
  message,
  Descriptions,
  Tag,
  Space,
  Spin,
  Empty,
} from "antd";
import {
  SearchOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
} from "@ant-design/icons";

const { Title, Text } = Typography;

interface SearchFormData {
  referralCode: string;
}

interface ReferralDetails {
  referrerName: string;
  phoneNumber: string;
  referralCode: string;
  isRedeemed: boolean;
  createdAt: string;
  redeemedAt: string | null;
}

export function AdminPanel() {
  const [referralDetails, setReferralDetails] =
    useState<ReferralDetails | null>(null);
  const [isSearching, setIsSearching] = useState(false);
  const [isRedeeming, setIsRedeeming] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<SearchFormData>({
    defaultValues: {
      referralCode: "",
    },
  });

  // TODO: Replace with generated query from codegen
  // import { useGetApiAdminReferralByReferralCode } from '../api/queries';

  const onSearch = async (data: SearchFormData) => {
    setIsSearching(true);
    setHasSearched(true);
    try {
      // TODO: Replace with actual API call after codegen
      const response = await fetch(`/api/admin/referral/${data.referralCode}`);

      if (response.ok) {
        const details: ReferralDetails = await response.json();
        setReferralDetails(details);
      } else {
        setReferralDetails(null);
        message.error("Referral code not found");
      }
    } catch (error) {
      message.error("Failed to search for referral code");
      console.error(error);
    } finally {
      setIsSearching(false);
    }
  };

  // TODO: Replace with generated mutation from codegen
  // import { usePostApiAdminRedeemMutation } from '../api/queries';

  const handleRedeem = async () => {
    if (!referralDetails) return;

    setIsRedeeming(true);
    try {
      // TODO: Replace with actual API call after codegen
      const response = await fetch("/api/admin/redeem", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ referralCode: referralDetails.referralCode }),
      });

      const result = await response.json();

      if (result.success) {
        message.success(result.message);
        setReferralDetails({
          ...referralDetails,
          isRedeemed: true,
          redeemedAt: new Date().toISOString(),
        });
      } else {
        message.error(result.message);
      }
    } catch (error) {
      message.error("Failed to mark as redeemed");
      console.error(error);
    } finally {
      setIsRedeeming(false);
    }
  };

  const formatDate = (dateString: string | null) => {
    if (!dateString) return "N/A";
    return new Date(dateString).toLocaleString("en-AU", {
      dateStyle: "medium",
      timeStyle: "short",
    });
  };

  return (
    <div>
      <Card className="admin-card" style={{ marginBottom: 24 }}>
        <Title level={3}>🔍 Search Referral Code</Title>

        <Form
          layout="inline"
          onFinish={handleSubmit(onSearch)}
          style={{ marginBottom: 16 }}
        >
          <Form.Item
            validateStatus={errors.referralCode ? "error" : ""}
            help={errors.referralCode?.message}
            style={{ flex: 1, marginRight: 8 }}
          >
            <Controller
              name="referralCode"
              control={control}
              rules={{
                required: "Please enter a referral code",
              }}
              render={({ field }) => (
                <Input
                  {...field}
                  placeholder="Enter referral code (e.g., MARTIN-4821)"
                  size="large"
                  style={{ textTransform: "uppercase" }}
                  onChange={(e) => field.onChange(e.target.value.toUpperCase())}
                />
              )}
            />
          </Form.Item>
          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              icon={<SearchOutlined />}
              loading={isSearching}
              size="large"
            >
              Search
            </Button>
          </Form.Item>
        </Form>
      </Card>

      {isSearching && (
        <Card className="admin-card">
          <div style={{ textAlign: "center", padding: 40 }}>
            <Spin size="large" />
            <div style={{ marginTop: 16 }}>
              <Text>Searching...</Text>
            </div>
          </div>
        </Card>
      )}

      {!isSearching && hasSearched && !referralDetails && (
        <Card className="admin-card">
          <Empty description="Referral code not found" />
        </Card>
      )}

      {!isSearching && referralDetails && (
        <Card className="admin-card">
          <Title level={4}>Referral Details</Title>

          <Descriptions bordered column={1}>
            <Descriptions.Item label="Referrer Name">
              <Text strong>{referralDetails.referrerName}</Text>
            </Descriptions.Item>
            <Descriptions.Item label="Phone Number">
              {referralDetails.phoneNumber}
            </Descriptions.Item>
            <Descriptions.Item label="Referral Code">
              <Text code style={{ fontSize: 16 }}>
                {referralDetails.referralCode}
              </Text>
            </Descriptions.Item>
            <Descriptions.Item label="Status">
              {referralDetails.isRedeemed ? (
                <Tag icon={<CheckCircleOutlined />} color="success">
                  Redeemed
                </Tag>
              ) : (
                <Tag icon={<CloseCircleOutlined />} color="warning">
                  Not Redeemed
                </Tag>
              )}
            </Descriptions.Item>
            <Descriptions.Item label="Created">
              {formatDate(referralDetails.createdAt)}
            </Descriptions.Item>
            <Descriptions.Item label="Redeemed At">
              {formatDate(referralDetails.redeemedAt)}
            </Descriptions.Item>
          </Descriptions>

          <div style={{ marginTop: 24, textAlign: "center" }}>
            {!referralDetails.isRedeemed ? (
              <Space direction="vertical">
                <Text type="secondary">
                  Apply discount to the friend's order, then click below:
                </Text>
                <Button
                  type="primary"
                  size="large"
                  icon={<CheckCircleOutlined />}
                  loading={isRedeeming}
                  onClick={handleRedeem}
                >
                  Mark as Redeemed
                </Button>
              </Space>
            ) : (
              <Text type="success">
                ✅ This referral has already been redeemed on{" "}
                {formatDate(referralDetails.redeemedAt)}
              </Text>
            )}
          </div>
        </Card>
      )}
    </div>
  );
}
