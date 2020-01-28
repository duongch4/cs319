import { runWithAdal } from 'react-adal';
import { authContext } from './config/adalConfig';

const DO_NOT_LOGIN = false;

runWithAdal(authContext, () => {

  // eslint-disable-next-line
  require('./indexApp.js');

},DO_NOT_LOGIN);