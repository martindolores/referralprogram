Lorna's Baked Delights - Referral Program Architecture

# Tech Stack and Cloud Services

| **Layer**            | **Technology / Service** | **Notes**                                                                |
| -------------------- | ------------------------ | ------------------------------------------------------------------------ |
| **Frontend**         | React                    | Lightweight, single-page referral form + admin page                      |
| **Frontend Hosting** | Netlify (Free Tier)      | Easy drag-and-drop or GitHub deploy, HTTPS included                      |
| **Backend / API**    | ASP.NET Core Web API     | Minimal APIs or Controllers                                              |
| **Backend Hosting**  | Render Free Tier         | Free hosting for ASP.NET Core, HTTPS included, simple deploy from GitHub |
| **Database**         | SQLite                   | Single file DB, lightweight, zero config, works on Render                |

Optional:

- QR code generation (can generate dynamically in React)
- Environment variables on Render for secrets (SMS API keys, admin password)

Whole cost would be the domain ~\$30, rest of the cloud hosting will be using free tier as we don't need anything fancy

### Anti-abuse protections

1 referral per phone number

1 redemption per code

Rate limiting form submissions

Optional expiration (e.g., 60 days)

SMS verification stops casual spamming

# Workflow (End-to-End)

## One-Time Setup

Deploy React app to Netlify (/refer + /admin)

Deploy ASP.NET Core API to Render

Set up SQLite database (referrals.db)

Connect SMS provider (Twilio) ???

## Referrer fills out form

User clicks /refer link

Enters:

- Name
- Mobile number

Clicks "Get referral code"

## SMS verification

Backend generates unique referral code: e.g. MARTIN-4821

Sends SMS to provided mobile:

Your referral code is MARTIN -4821 🍰

Tell your friend to mention it when they DM us.

You'll both get 10% off!

Only creates referral in DB if SMS sent successfully

## Referrer confirmation

Screen shows:

✅ Check your phone!

Your referral code has been sent.

Share it with a friend and you'll both get 10% off.

## Referrer shares code

User shares referral code

- No app installs, no login (good especially for the age group mum is catering for)

## Referral places order

DMs Instagram:

- Hi! I was referred by Martin - Referral code: MARTIN-4821

Places order as normal by dm'ing mum

## Mum checks code

Open /admin page (protected via password or HTTP basic auth)

Search referral code (MARTIN-4821)

Sees:

Referrer: Sarah

Phone: 04xx xxx xxx

Status: ❌ Not Redeemed

Created: 12 Dec 2025

## Marks redeemed

Mum applies discount to friend's order

Click \[Mark Redeemed\]

Backend updates:

- Status: Redeemed
- RedeemedAt: DateTime.Now

## Reward referrer

Next time Martin orders:

Mum checks her code / phone to see if code has been redeemed

Applies 10% discount
