# Referral Program API Endpoints

## Referral Endpoints (Public)

### Create Referral
**POST** `/api/referral`

Creates a new referral and sends SMS with referral code.

**Request Body:**
```json
{
  "name": "Martin",
  "phoneNumber": "0412345678"
}
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Referral code sent successfully!",
  "referralCode": "MARTIN-4821"
}
```

**Response (Error - Duplicate Phone):**
```json
{
  "success": false,
  "message": "This phone number has already been used for a referral."
}
```

### Get Referral Details
**GET** `/api/referral/{referralCode}`

Retrieves details for a specific referral code.

**Response:**
```json
{
  "referrerName": "Martin",
  "phoneNumber": "0412345678",
  "referralCode": "MARTIN-4821",
  "isRedeemed": false,
  "createdAt": "2025-12-12T10:30:00Z",
  "redeemedAt": null
}
```

## Admin Endpoints

### Get Referral Details (Admin)
**GET** `/api/admin/referral/{referralCode}`

Same as public endpoint - retrieves referral details for admin panel.

### Mark Referral as Redeemed
**POST** `/api/admin/redeem`

Marks a referral code as redeemed when the referred friend places an order.

**Request Body:**
```json
{
  "referralCode": "MARTIN-4821"
}
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Referral code marked as redeemed successfully."
}
```

**Response (Error):**
```json
{
  "success": false,
  "message": "Referral code not found or already redeemed."
}
```

## Features Implemented

? **Controllers & Endpoints**: All necessary endpoints for referral creation and admin management  
? **DTOs**: Request/Response models for clean API contracts  
? **Service Interfaces**: `IReferralService` and `ISmsService` ready for implementation  
? **Mock Services**: In-memory implementations for testing (replace with SQLite + Twilio later)  
? **CORS**: Configured for React frontend (localhost + Netlify)  
? **Rate Limiting**: 10 requests per minute per IP to prevent abuse  
? **Anti-abuse**: One referral per phone number enforced  
? **Error Handling**: Proper HTTP status codes and error messages  

## Next Steps

1. **Database**: Replace `MockReferralService` with SQLite implementation
2. **SMS**: Replace `MockSmsService` with Twilio integration
3. **Authentication**: Add basic auth or JWT for admin endpoints
4. **Validation**: Add phone number format validation
5. **Testing**: Add unit and integration tests

## Testing Locally

The mock services will log SMS messages to the console and store referrals in memory. This allows you to test the full flow without setting up SQLite or Twilio.
