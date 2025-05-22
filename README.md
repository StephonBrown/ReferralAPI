# Carton Caps Referral REST API

This document overviews the endpoints and their uses for the Carton Caps Referral REST API. 

The base API will be dependent on the environment, and an example for local testing will be: https://carton-caps-referral-api.local/
## Endpoints that require Authentication

The following endpoints require a valid Token to be included in the header of the request. A token can be acquired from the Login view above.
## Referral Endpoints

Each endpoint is related to an an existing user. The user must be authenticated and each endpoint requires a valid bearer token in the request header.
### Create Referral Link
Create a referral link for the current user for the current user using their referral code and the channel through which they sent the referral.
- Note:  Create a generated referral id.
- Rate-limited for how many times a user can return it
- Think about generating a get link

**URL** : `/api/referrals/create-link/{id}`
**Method** : `POST`
**Auth required** : YES
#### Notes:
- This endpoint returns a deferred deeplink. The deferred deeplink allows for potential users who may not have the application installed to be redirected to the appropriate app store, download the app, and on the first open use the appropriate referral code and other parameters to shape their first experience.
- The link generation will retry based on the errors of the third-party call.
**Body**
```json
{
    "channel": "{ SMS | EMAIL | SHARE_SHEET }",
    "time_to_live": "30d"
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

### Get Referral Statuses By User Id
Get all of the referral statuses of users that registered with the current users referral code.

**URL** : `/api/referrals`
**Method** : `GET`
**Auth required** : YES
**Permissions required** : None
#### Success Response
**Code** : `200 OK`
```json
{
    "referees": "[{"first_name":"", "last_name": "", "status":""}]"
}
```

**Code** : `200 OK`
```json
{
    "referees": "[]"
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
    "error_message": "Unable to get referees for user"
}
```

Delete a Referral Link
Update a Referral Link