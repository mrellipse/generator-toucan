import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';
import { default as jwtDecode } from 'jwt-decode';
import { default as base64 } from 'base-64';
import { IClaimsHelper } from './claims-helper';
import { GlobalConfig, PayloadMessageTypes, TokenHelper } from '../../common';
import { IAccessToken, ICredential, IJwtToken, ILoginProvider, IPayload, ISignupOptions, IUser } from '../../model';
import { ICommonState, Store, StoreService, StoreTypes } from '../../store';

// URL and endpoint constants
const AUTH_URL = GlobalConfig.uri.auth;
const AUTH_ISSUE_NONCE = AUTH_URL + 'external/issuenonce';
const LOGIN_URL = AUTH_URL + 'token';
const LOGOUT_URL = AUTH_URL + 'logout';
const SIGNUP_URL = AUTH_URL + 'signup';
const REDEEM_ACCESS_TOKEN = AUTH_URL + 'external/redeemtoken';
const VERIFICATION_CODE_URL = AUTH_URL + 'issueverificationcode';
const REDEEM_VERIFICATION_CODE = AUTH_URL + 'redeemverificationcode';

export { IClaimsHelper } from './claims-helper';

export class AuthenticationService extends StoreService implements IClaimsHelper {

    constructor(store: Store<{}>) {
        super(store);
    }

    /**
     * Retreives a nonce value, so that incoming redirects from oauth providers can be validated by the server when redeeming access tokens ...
     */
    getAuthorizationNonce() {

        let onSuccess = (payload: IPayload<string>) => {

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                return payload.data;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return this.exec(Axios.post(AUTH_ISSUE_NONCE))
            .then(onSuccess);
    }

    isInRole(user: IUser, role: string): boolean {

        if (!user.roles)
            return false;
        else
            return user.roles.find(o => o.toLowerCase() === role.toLowerCase()) !== undefined;

    }

    login(credentials: ICredential) {

        let onSuccess = (token: IAccessToken) => {

            if (token) {

                TokenHelper.setAccessToken(token.access_token);

                return TokenHelper.parseUserToken(token.access_token);
            }
        }

        return this.exec(Axios.post(LOGIN_URL, credentials))
            .then(value => this.processPayload(value))
            .then(onSuccess);
    }

    logout() {

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IAccessToken> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                let user: IUser = { authenticated: false, displayName: null, email: null, name: null, username: null, roles: [], verified: false };

                TokenHelper.removeAccessToken();

                return user;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.put(LOGOUT_URL, null)
            .then(onSuccess);
    }

    redeemAccessToken(provider: ILoginProvider) {

        let onSuccess = (token: IAccessToken) => {

            if (token) {

                TokenHelper.setAccessToken(token.access_token);
                TokenHelper.removeProvider();

                return TokenHelper.parseUserToken(token.access_token);
            }
        };

        let data = {
            accessToken: encodeURIComponent(provider.access_token),
            nonce: provider.nonce,
            providerId: provider.providerId
        }

        return this.exec(Axios.post(REDEEM_ACCESS_TOKEN, data))
            .then(value => this.processPayload(value))
            .then(onSuccess);
    }

    redeemVerificationCode(code: string) {

        let onSuccess = (token: IAccessToken) => {

            if (token) {

                TokenHelper.setAccessToken(token.access_token);

                return TokenHelper.parseUserToken(token.access_token);
            }
        };

        let config = {
            params: { code: code }
        };

        return this.exec(Axios.put(REDEEM_VERIFICATION_CODE, null, config))
            .then(value => this.processPayload(value))
            .then(onSuccess);
    }

    signup(signup: ISignupOptions) {

        let onSuccess = (token: IAccessToken) => {

            if (token) {

                TokenHelper.setAccessToken(token.access_token);

                return TokenHelper.parseUserToken(token.access_token);
            }
        }

        return this.exec(Axios.post(SIGNUP_URL, signup))
            .then(value => this.processPayload(value))
            .then(onSuccess);
    }

    verify() {
        return this.exec(Axios.get(VERIFICATION_CODE_URL))
            .then(value => this.processPayload<string>(<any>value))
    }
}