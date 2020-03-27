export const TENANT_ID = process.env.REACT_APP_TENANT_ID;
export const CLIENT_ID = process.env.REACT_APP_CLIENT_ID;

export const SVC_ROOT = process.env.REACT_APP_SVC_ROOT ? process.env.REACT_APP_SVC_ROOT : window.location.origin;

export const UI_ROOT = process.env.REACT_APP_UI_ROOT ? process.env.REACT_APP_UI_ROOT : window.location.origin;
export const AUTHORITY = `https://login.microsoftonline.com/${TENANT_ID}`;
export const API_ID = process.env.REACT_APP_API_ID;

export const CLIENT_DEV_ENV = false;
// export const CLIENT_DEV_ENV = true;
