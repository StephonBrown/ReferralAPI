# Carton Caps Referral REST API

This document overviews the referral process, technical considerations,and API Specifications the Carton Caps Referral REST API. 

The base API will be dependent on the environment, and an example for local testing will be running either the http or https configurations: 
- https://localhost:7251 (after trusting the local certificate)
- http://localhost:5285

## What Is the Carton Caps Referral API?
The Carton Caps Referral API allows for the creation and management of referral links and user referrals. The API is designed to be used by the Carton Caps application and is intended to be used by the Carton Caps team.

## What Is a Referral?
A referral is a way to track users who have been referred to the Carton Caps application by an existing user. Referrals are tracked using a unique referral code that is generated for each user. When a new user signs up for the application using an existing user's referral code, the existing user is credited with a referral.

## What Is a Referral Link?
A referral link is a unique URL that contains a referral code. When a new user clicks on the referral link, they are redirected to the Carton Caps application and the referrer's referral code is then automatically filled during their account setup. 
This allows the existing user to be credited with the referral without the new user having to manually enter the referral code.

## How Does a Referral Link Work?
1. A user generates a referral link using the Carton Caps application by selecting a channel to share the link.
2. The user shares the referral link with a potential new user through the selected channel (e.g. SMS, Email, etc.).
3. The potential new user clicks on the referral link which is a deferred deeplink that redirects them to the appropriate app store (Google Play Store or Apple App Store) to download the Carton Caps application or if they already have the app installed, it opens the app directly to the appropriate screen(deeplink).
4. Once the new user downloads and opens the app, the referral code is automatically filled in during the account setup process.
5. The new user completes the account setup process and the referrer is credited with the referral.

#### What is a deferred deeplink?
A _deferred deeplink_ allows for potential users who may not have the application installed to be redirected to the appropriate app store, download the app, and on the first open use the appropriate referral code and other parameters to shape their first experience.
This allows for a seamless experience for the new user and ensures that the referrer is credited with the referral.

Deferred deeplinks are managed by a third-party platform that is used to generate the link with associated parameters.
Each user will have a unique link associated with their referral code. 

## API Specifications

**Note:** The following endpoints require a valid Token to be included in the **Authorize** header of the request.

### Referral Link Endpoints
The following endpoints are used to manage referral links. 

**Note**: _The channel will need to be applied to the returned link when a selection of which channel is used to share.
An example of a channel would be SMS, Email, etc. and would ba applied to the link as a query parameter._

`https://cartoncaps.link?&channel=SMS`

#### Create Referral Link
- This endpoint creates or retrieves a sharable deferred deeplink.

**URL** : `/api/referralslinks`

**Method** : `POST`

**Authorization required** : Yes

**Permissions required** : None

The userId will be retrieved from the bearer token.

**Response Body**
```json
{
    "referral_link": "https://cartoncaps.link/Afbs3",
    "expiration_date": "2025-05-22T07:22Z"
}
```

#### Success Response
##### Created Response

**Code** : `201 CREATED`
```json
{
    "referral_link": "https://cartoncaps.link?referral_code={user_referral_code}",
}
```

##### Link Already Exists Response
**Code** : `200 OK`
```json
{
    "referral_link": "https://cartoncaps.link?referral_code={user_referral_code}",
}
```

#### Error Responses

**Code** : `400 Bad Request`
```json
{
    "code": 400,
    "error_message": "This user does not exist"
}
```

**Code** : `401 Unauthorized`
```json
{
    "code": 401,
    "error_message": "unauthorized to access this endpoint"
}
```

**Code** : `500 Internal Server Error`
```json
{
    "code": 500,
    "error_message": "Link generation failed"
}
```

### Get Referral Link
Get the referral link for the current user. This will be used to retrieve ther current users referral link that was created or cached.
- Note: The channel will need to be applied to the returned link when a selection of which channel is used to share.

**URL** : `/api/referralslinks`

### Referral Endpoints

### Create Referral
Add a new referral based on a specified referral code. This usually will come in the form of the referee just adding the referral code during account setup.
- Note this could be used for referral links
  **URL** : `/api/referrals`
  **Method** : `POST`
  **Auth required** : Yes
  **Permissions required** : Admin

**Body**
```json
{
    "user_id": "",
    "referral_code": ""
}
```
#### Success Response
**Code** : `200 OK`
#### Error Responses
**Code** : `400 Bad Request`
```json
{
    "code": 400,
    "error_message": "This referral code does not exist"
}
```

```json
{
    "code": 400,
    "error_message": "This user does not exist"
}
```

**Code** : `401 Unauthorized`
```json
{
    "code": 401,
    "error_message": "unauthorized to access this endpoint"
}
```

**Code** : `409 Conflict`
```json
{
    "code": 409,
    "error_message": "The user provided was already referred under a different referral code"
}
```
**Code** : `500 Internal Server Error`
```json
{
    "code": 500,
    "error_message": "Unable to add user to referral table"
}
```