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

**Note:** The following endpoints require a valid Token to be included in the **Authorize** header of the request. To generate a token, Post to the AuthTest with the following Uri and body:
**URL** : `/api/authtest/get-bearer-token`
```json
{
  "secret_code": "TEST"
}
```

This will return a valid token that can be used to access the API endpoints that look like the following:
```json
{
  "Token": "abcd1234efgh5678ijkl9012mnop3456qrst7890uvwx1234yzab5678cdef9012ghij3456klmn7890opqr1234stuv5678wxyz9012abcd3456efgh7890ijklmnopqrstuvwx"
}
```
Add the token to the **Authorization** header of your the requests.

**Note**: _The functional controller tests are a good example of how to use the API and will return a valid token that can be used to access the API endpoints_

### Referral Link Endpoints
The following endpoints are used to manage referral links. 

**Note**: _The channel will need to be applied to the returned link when a selection of which channel is used to share.
An example of a channel would be SMS, Email, etc. and would ba applied to the link as a query parameter._

`https://cartoncaps.link?&channel=SMS`


A full spec of these endpoints can be found at:
- https://localhost:7251/swagger/index.html#/ReferralLinks
- http://localhost:5285/swagger/index.html#/ReferralLinks

when the API is running locally.


### Referral Endpoints
The following endpoints are used to manage referrals.

A full spec of these endpoints can be found at:
- https://localhost:7251/swagger/index.html#/Referrals
- http://localhost:5285/swagger/index.html#/Referrals

when the API is running locally.

### User Test Endpoints
The following endpoints are used to manage users.

A full spec of these endpoints can be found at:
- https://localhost:7251/swagger/index.html#/UserTest
- http://localhost:5285/swagger/index.html#/UserTest

when the API is running locally.

### Auth Test Endpoints
The following endpoints are used to manage authentication locally.
A full spec of these endpoints can be found at:
- https://localhost:7251/swagger/index.html#/AuthTest
- http://localhost:5285/swagger/index.html#/AuthTest

when the API is running locally.

## Referral Process
The referral process is designed to be simple and straightforward. There are 3 possible ways that a referral can be created:
### A New User Signs Up Using a Referral Code
When a new user signs up for the Carton Caps application using an existing user's referral code, the following steps occur:
1. The new user enters the referral code during the account setup process.

### A Potential User Clicks on a Referral Link and Tey Already Have the App Installed
When a potential user clicks on a referral link and they already have the Carton Caps application installed, the following steps occur:
1. The potential user clicks on the referral link.
2. The referral link redirects the user to the Carton Caps application using a deferred deeplink.
3. The Carton Caps application opens and the referral code is automatically filled in during the account setup process.
4. The new user completes the account setup process and the referral is created using the new user's ID and the referrer's referral code.

### A Potential User Clicks on a Referral Link and They Do Not Have the App Installed
When a potential user clicks on a referral link and they do not have the Carton Caps application installed, the following steps occur:
1. The potential user clicks on the referral link.
2. The referral link redirects the user to the appropriate app store (Google Play Store or Apple App Store) to download the Carton Caps application.
3. Once the new user downloads and opens the app, because there will be an SDK if a third party platform for deferred deeplinking, the referral code is automatically filled in during the account setup process.
4. The new user completes the account setup process and the referral is created using the new user's ID and the referrer's referral code.

## How to Simulate a Referral Using the API
To simulate a referral using the API, you can follow these steps:
1. Create an bearer token using the AuthTest endpoint. `/api/authtest/get-bearer-token` and the secret_code `TEST`
2. Use the UserTest endpoints to create a new user.
3. Use the POST endpoint to create a new referral using the new user's ID and the referrer's referral code `TESTCODE`
4. Retrieve the referrals using the GET  endpoint to verify that the referral was created successfully.

## How to Simulate a Referral Link Generation Using the API
To simulate a referral link generation using the API, you can follow these steps:
1. Create an bearer token using the AuthTest endpoint. `/api/authtest/get-bearer-token` and the secret_code `TEST`
2. Use the ReferralLink endpoints to create a new referral. **Note:** _The bearer token will have the user ID of the user that is creating the referral link._
3. Retrieve the referral link using the GET endpoint to verify that the referral link was created successfully.

## Technical Considerations
In order to mitigate abuse there are a few security pieces in place:
- The referral link generated is unique to the user and contains a referral code that is associated with the user.
  - **Note**: _The mock third-party services adds this as a query parameter to the link, but many platforms allow adding these parameters to the link in a different way._
- The referrals have unique indexes on the referrer and new user IDs to prevent duplicate referrals.
- There is prevention for duplicate referral links being created for the same user.
- The API is secured using bearer tokens to ensure that only authorized users can access the endpoints.
  - Admins and services will have access to the UpdateExpiration endpoint to update the expiration date of the referral links.

Future considerations could include:
- Adding rate limiting to the API endpoints to prevent abuse.
- Adding caching to the Get Links endpoint to improve performance
