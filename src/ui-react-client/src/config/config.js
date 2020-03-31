export const TENANT_ID = process.env.REACT_APP_TENANT_ID;
export const CLIENT_ID = process.env.REACT_APP_CLIENT_ID;

export const SVC_ROOT = process.env.REACT_APP_SVC_ROOT ? process.env.REACT_APP_SVC_ROOT : window.location.origin;

export const UI_ROOT = process.env.REACT_APP_UI_ROOT ? process.env.REACT_APP_UI_ROOT : window.location.origin;
export const AUTHORITY = `https://login.microsoftonline.com/${TENANT_ID}`;
export const API_ID = process.env.REACT_APP_API_ID;

export const CLIENT_DEV_ENV = false;
// export const CLIENT_DEV_ENV = true;

export const LOW_UTILIZATION = 50;
export const MEDIUM_UTILIZATION = 85;
export const HIGH_UTILIZATION = 100;
export const LOW_UTILIZATION_COLOUR = "#EB5757";
export const MEDIUM_UTILIZATION_COLOUR = "#F2994A";
export const HIGH_UTILIZATION_COLOUR = "#6FCF97";
export const OVER_UTILIZATION_COLOUR = "maroon";
