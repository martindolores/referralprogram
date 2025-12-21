import { useForm, Controller } from "react-hook-form";
import { Form, Input, Button, Card, Typography, message, Result } from "antd";
import { UserOutlined, PhoneOutlined, GiftOutlined } from "@ant-design/icons";
import { useState } from "react";

const { Title, Text } = Typography;

interface ReferralFormData {
  name: string;
  phoneNumber: string;
}

interface ReferralResponse {
  success: boolean;
  message: string;
  referralCode?: string;
}

export function ReferralForm() {
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [referralCode, setReferralCode] = useState<string | null>(null);

  const {
    control,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<ReferralFormData>({
    defaultValues: {
      name: "",
      phoneNumber: "",
    },
  });

  // TODO: Replace with generated mutation from codegen
  // import { usePostApiReferralMutation } from '../api/queries';
  // const { mutateAsync } = usePostApiReferralMutation();

  const onSubmit = async (data: ReferralFormData) => {
    try {
      // TODO: Replace with actual API call after codegen
      // const response = await mutateAsync({ body: data });

      // Mock response for now
      const response: ReferralResponse = await fetch("/api/referral", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
      }).then((res) => res.json());

      if (response.success) {
        setReferralCode(response.referralCode || null);
        setIsSubmitted(true);
        message.success(response.message);
      } else {
        message.error(response.message);
      }
    } catch (error) {
      message.error("Something went wrong. Please try again.");
      console.error(error);
    }
  };

  const handleReset = () => {
    setIsSubmitted(false);
    setReferralCode(null);
    reset();
  };

  if (isSubmitted) {
    return (
      <Card className="referral-card">
        <Result
          status="success"
          icon={<GiftOutlined style={{ color: "#52c41a" }} />}
          title="Check your phone! 📱"
          subTitle={
            <>
              <Text>Your referral code has been sent via SMS.</Text>
              {referralCode && (
                <div style={{ marginTop: 16 }}>
                  <Text strong style={{ fontSize: 24 }}>
                    {referralCode}
                  </Text>
                </div>
              )}
              <div style={{ marginTop: 16 }}>
                <Text type="secondary">
                  Share it with a friend and you'll both get 10% off! 🍰
                </Text>
              </div>
            </>
          }
          extra={[
            <Button key="another" onClick={handleReset}>
              Create Another Referral
            </Button>,
          ]}
        />
      </Card>
    );
  }

  return (
    <Card className="referral-card">
      <div style={{ textAlign: "center", marginBottom: 24 }}>
        <GiftOutlined style={{ fontSize: 48, color: "#1890ff" }} />
        <Title level={2} style={{ marginTop: 16, marginBottom: 8 }}>
          Refer a Friend
        </Title>
        <Text type="secondary">
          Get 10% off your next order when your friend places an order!
        </Text>
      </div>

      <Form layout="vertical" onFinish={handleSubmit(onSubmit)}>
        <Form.Item
          label="Your Name"
          validateStatus={errors.name ? "error" : ""}
          help={errors.name?.message}
        >
          <Controller
            name="name"
            control={control}
            rules={{
              required: "Please enter your name",
              minLength: {
                value: 2,
                message: "Name must be at least 2 characters",
              },
            }}
            render={({ field }) => (
              <Input
                {...field}
                prefix={<UserOutlined />}
                placeholder="Enter your name"
                size="large"
              />
            )}
          />
        </Form.Item>

        <Form.Item
          label="Mobile Number"
          validateStatus={errors.phoneNumber ? "error" : ""}
          help={errors.phoneNumber?.message}
        >
          <Controller
            name="phoneNumber"
            control={control}
            rules={{
              required: "Please enter your mobile number",
              pattern: {
                value: /^04\d{8}$/,
                message:
                  "Please enter a valid Australian mobile number (04XXXXXXXX)",
              },
            }}
            render={({ field }) => (
              <Input
                {...field}
                prefix={<PhoneOutlined />}
                placeholder="04XX XXX XXX"
                size="large"
              />
            )}
          />
        </Form.Item>

        <Form.Item>
          <Button
            type="primary"
            htmlType="submit"
            loading={isSubmitting}
            block
            size="large"
          >
            Get My Referral Code
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
}
