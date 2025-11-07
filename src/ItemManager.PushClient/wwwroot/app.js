import { initializeApp } from 'https://www.gstatic.com/firebasejs/10.12.2/firebase-app.js';
import { getMessaging, getToken, onMessage } from 'https://www.gstatic.com/firebasejs/10.12.2/firebase-messaging.js';
import { firebaseConfig as defaultFirebaseConfig, firebaseVapidKey as defaultVapidKey } from './firebase-config.js';

const elements = {
  apiBaseUrl: document.getElementById('apiBaseUrl'),
  deviceName: document.getElementById('deviceName'),
  initializeFirebase: document.getElementById('initializeFirebase'),
  registrationToken: document.getElementById('registrationToken'),
  registerForm: document.getElementById('registerForm'),
  registerUsername: document.getElementById('registerUsername'),
  registerPassword: document.getElementById('registerPassword'),
  registerOtp: document.getElementById('registerOtp'),
  registeredDevice: document.getElementById('registeredDevice'),
  loginForm: document.getElementById('loginForm'),
  loginUsername: document.getElementById('loginUsername'),
  loginPassword: document.getElementById('loginPassword'),
  challengeInfo: document.getElementById('challengeInfo'),
  confirmChallenge: document.getElementById('confirmChallenge'),
  sessionToken: document.getElementById('sessionToken'),
  eventLog: document.getElementById('eventLog'),
  clearLog: document.getElementById('clearLog'),
};

const storageKeys = {
  apiBaseUrl: 'pushClient.apiBaseUrl',
  deviceName: 'pushClient.deviceName',
  username: 'pushClient.username',
  deviceId: 'pushClient.deviceId',
};

const state = {
  firebaseConfig: { ...defaultFirebaseConfig },
  vapidKey: defaultVapidKey,
  messaging: null,
  serviceWorkerRegistration: null,
  registrationToken: '',
  apiBaseUrl: localStorage.getItem(storageKeys.apiBaseUrl) ?? 'http://localhost:5000',
  deviceName: localStorage.getItem(storageKeys.deviceName) ?? 'Navegador web',
  username: localStorage.getItem(storageKeys.username) ?? 'admin',
  deviceId: localStorage.getItem(storageKeys.deviceId) ?? '',
  activeChallenge: null,
  serviceWorkerListenerRegistered: false,
};

document.addEventListener('DOMContentLoaded', () => {
  elements.apiBaseUrl.value = state.apiBaseUrl;
  elements.deviceName.value = state.deviceName;
  elements.registerUsername.value = state.username;
  elements.loginUsername.value = state.username;

  if (state.deviceId) {
    elements.registeredDevice.textContent = `Dispositivo registrado (${state.deviceId}).`;
  }

  elements.initializeFirebase.addEventListener('click', initializeFirebaseFlow);
  elements.registerForm.addEventListener('submit', registerDevice);
  elements.loginForm.addEventListener('submit', initiatePushLogin);
  elements.confirmChallenge.addEventListener('click', confirmChallenge);
  elements.clearLog.addEventListener('click', clearLog);

  elements.apiBaseUrl.addEventListener('change', () => {
    state.apiBaseUrl = elements.apiBaseUrl.value.trim();
    localStorage.setItem(storageKeys.apiBaseUrl, state.apiBaseUrl);
    log(`URL base actualizada a ${state.apiBaseUrl}`);
  });

  elements.deviceName.addEventListener('change', () => {
    state.deviceName = elements.deviceName.value.trim();
    localStorage.setItem(storageKeys.deviceName, state.deviceName);
  });

  elements.registerUsername.addEventListener('change', syncUsername);
  elements.loginUsername.addEventListener('change', syncUsername);

  log('Listo para inicializar Firebase.');
});

function syncUsername() {
  const username = (elements.registerUsername.value || elements.loginUsername.value || '').trim();
  if (!username) {
    return;
  }

  state.username = username;
  elements.registerUsername.value = username;
  elements.loginUsername.value = username;
  localStorage.setItem(storageKeys.username, username);
}

async function initializeFirebaseFlow() {
  if (state.messaging) {
    log('Firebase ya fue inicializado.');
    return;
  }

  if (!state.firebaseConfig || !state.firebaseConfig.apiKey || state.firebaseConfig.apiKey.startsWith('REEMPLAZAR')) {
    log('Completa firebase-config.js con las credenciales reales antes de inicializar.', true);
    return;
  }

  if (!('serviceWorker' in navigator)) {
    log('El navegador no soporta Service Workers necesarios para FCM.', true);
    return;
  }

  try {
    state.serviceWorkerRegistration = await navigator.serviceWorker.register('firebase-messaging-sw.js', { type: 'module' });

    if (!state.serviceWorkerListenerRegistered) {
      navigator.serviceWorker.addEventListener('message', (event) => {
        if (!event.data || event.data.type !== 'push-message') {
          return;
        }

        log('Notificación recibida desde el Service Worker.');
        handlePushPayload(event.data.payload);
      });

      state.serviceWorkerListenerRegistered = true;
    }

    const app = initializeApp(state.firebaseConfig);
    state.messaging = getMessaging(app);

    const permission = await Notification.requestPermission();
    if (permission !== 'granted') {
      log('El permiso de notificaciones no fue concedido.', true);
      return;
    }

    const token = await getToken(state.messaging, {
      vapidKey: state.vapidKey,
      serviceWorkerRegistration: state.serviceWorkerRegistration,
    });

    if (!token) {
      log('Firebase no devolvió un token de registro.', true);
      return;
    }

    state.registrationToken = token;
    elements.registrationToken.value = token;
    log('Token FCM generado correctamente.');

    onMessage(state.messaging, (payload) => {
      log('Mensaje push recibido con la app en primer plano.');
      handlePushPayload(payload);
    });
  }
  catch (error) {
    console.error(error);
    log(`Error inicializando Firebase: ${error instanceof Error ? error.message : String(error)}`, true);
  }
}

async function registerDevice(event) {
  event.preventDefault();

  if (!state.registrationToken) {
    log('Primero generá el token FCM del dispositivo.', true);
    return;
  }

  const username = elements.registerUsername.value.trim();
  const password = elements.registerPassword.value.trim();
  const otpCode = elements.registerOtp.value.trim();
  const deviceName = elements.deviceName.value.trim() || 'Dispositivo web';

  if (!username || !password || !otpCode) {
    log('Completá usuario, contraseña y código TOTP para registrar el dispositivo.', true);
    return;
  }

  const result = await postJson(`${state.apiBaseUrl}/auth/push/register`, {
    username,
    password,
    otpCode,
    deviceName,
    registrationToken: state.registrationToken,
  });

  if (!result.success) {
    log(`Falló el registro: ${result.error}`, true);
    return;
  }

  const device = result.data?.device;
  if (!device) {
    log('Respuesta inesperada al registrar el dispositivo.', true);
    return;
  }

  state.deviceId = device.deviceId;
  localStorage.setItem(storageKeys.deviceId, state.deviceId);
  elements.registeredDevice.textContent = `Dispositivo ${device.deviceName} registrado con ID ${device.deviceId}.`;
  log(`Dispositivo registrado correctamente (${device.deviceId}).`);
}

async function initiatePushLogin(event) {
  event.preventDefault();

  const username = elements.loginUsername.value.trim();
  const password = elements.loginPassword.value.trim();

  if (!username || !password) {
    log('Indicá usuario y contraseña para iniciar el desafío.', true);
    return;
  }

  const result = await postJson(`${state.apiBaseUrl}/auth/push/login`, {
    username,
    password,
  });

  if (!result.success) {
    log(`No se pudo iniciar el desafío: ${result.error}`, true);
    return;
  }

  const challenge = result.data?.challenge;
  if (!challenge) {
    log('Respuesta inesperada al iniciar el desafío push.', true);
    return;
  }

  state.activeChallenge = {
    id: challenge.id,
    expiresAt: challenge.expiresAt,
    username: result.data?.user?.username ?? username,
    deviceId: state.deviceId,
  };

  updateChallengeView();
  log(`Desafío emitido (${challenge.id}). Esperando aprobación en el dispositivo.`);
}

async function confirmChallenge() {
  if (!state.activeChallenge?.id) {
    log('No hay desafíos activos para confirmar.', true);
    return;
  }

  const deviceId = state.activeChallenge.deviceId ?? state.deviceId;
  if (!deviceId) {
    log('No se conoce el dispositivo que debe aprobar el desafío.', true);
    return;
  }

  const result = await postJson(`${state.apiBaseUrl}/auth/push/confirm`, {
    challengeId: state.activeChallenge.id,
    deviceId,
  });

  if (!result.success) {
    log(`No se pudo confirmar el desafío: ${result.error}`, true);
    return;
  }

  const token = result.data?.token;
  if (!token) {
    log('La API no devolvió un token de sesión.', true);
    return;
  }

  elements.sessionToken.value = token;
  log('Sesión confirmada correctamente.');
  state.activeChallenge = null;
  updateChallengeView();
}

function handlePushPayload(payload) {
  if (!payload) {
    return;
  }

  const data = payload.data ?? {};
  if (data.challengeId) {
    state.activeChallenge = {
      ...(state.activeChallenge ?? {}),
      id: data.challengeId,
      username: data.username ?? state.activeChallenge?.username ?? 'desconocido',
      deviceId: data.deviceId ?? state.deviceId,
      receivedAt: new Date().toISOString(),
    };

    updateChallengeView();
    elements.confirmChallenge.disabled = false;
    log(`Push recibido para el desafío ${data.challengeId}.`);
  }
}

function updateChallengeView() {
  if (!state.activeChallenge?.id) {
    elements.challengeInfo.textContent = 'Sin desafíos activos.';
    elements.confirmChallenge.disabled = true;
    return;
  }

  const parts = [
    `ID: ${state.activeChallenge.id}`,
  ];

  if (state.activeChallenge.username) {
    parts.push(`Usuario: ${state.activeChallenge.username}`);
  }

  if (state.activeChallenge.deviceId) {
    parts.push(`Dispositivo: ${state.activeChallenge.deviceId}`);
  }

  if (state.activeChallenge.expiresAt) {
    parts.push(`Expira: ${state.activeChallenge.expiresAt}`);
  }

  if (state.activeChallenge.receivedAt) {
    parts.push(`Recibido: ${state.activeChallenge.receivedAt}`);
  }

  elements.challengeInfo.textContent = parts.join(' | ');
  elements.confirmChallenge.disabled = false;
}

function log(message, isError = false) {
  const entry = document.createElement('li');
  entry.className = isError ? 'error' : 'info';

  const time = document.createElement('time');
  time.dateTime = new Date().toISOString();
  time.textContent = new Date().toLocaleTimeString();

  const text = document.createElement('span');
  text.textContent = message;

  entry.appendChild(time);
  entry.appendChild(text);
  elements.eventLog.prepend(entry);
}

function clearLog() {
  elements.eventLog.innerHTML = '';
}

async function postJson(url, body) {
  try {
    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(body),
    });

    const text = await response.text();
    const data = text ? JSON.parse(text) : null;

    if (!response.ok) {
      return {
        success: false,
        status: response.status,
        data,
        error: data?.error ?? `HTTP ${response.status}`,
      };
    }

    return {
      success: true,
      status: response.status,
      data,
    };
  } catch (error) {
    return {
      success: false,
      status: 0,
      error: error instanceof Error ? error.message : String(error),
    };
  }
}
