export const IS_DEV_MODE = process.env.NODE_ENV === 'development';

export const URL_BACKEND = IS_DEV_MODE
   ?
    'http://localhost:5018'
    :
    'https://pokerapi.digaodalpiaz.com';
