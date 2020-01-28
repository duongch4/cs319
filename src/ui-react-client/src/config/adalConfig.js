import {
  AuthenticationContext,
  adalFetch,
  withAdalLogin,
} from 'react-adal';
import { CLIENT_ID, TENANT_ID } from './config';

export const adalConfig = {
  tenant: TENANT_ID,
  clientId: CLIENT_ID,
  endpoints: {
    api: TENANT_ID,
  },
  cacheLocation: 'localStorage',
};

export const authContext = new AuthenticationContext(adalConfig);

export const adalApiFetch = (fetch, url, options) =>
  adalFetch(
    authContext,
    adalConfig.endpoints.api,
    fetch,
    url,
    options,
  );

export const withAdalLoginApi = withAdalLogin(
  authContext,
  adalConfig.endpoints.api,
);

export const getToken = () =>
  authContext.getCachedToken(adalConfig.clientId);

export const headers = { Authorization: `Bearer ${getToken()}` };
