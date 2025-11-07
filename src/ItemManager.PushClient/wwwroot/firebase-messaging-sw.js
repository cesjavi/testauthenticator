import { initializeApp } from 'https://www.gstatic.com/firebasejs/10.12.2/firebase-app.js';
import { getMessaging, onBackgroundMessage } from 'https://www.gstatic.com/firebasejs/10.12.2/firebase-messaging-sw.js';
import { firebaseConfig } from './firebase-config.js';

const app = initializeApp(firebaseConfig);
const messaging = getMessaging(app);

onBackgroundMessage(messaging, async (payload) => {
  const title = payload.notification?.title ?? 'Solicitud de inicio de sesión';
  const body = payload.notification?.body ?? 'Aprobá el ingreso desde ItemManager.';

  await broadcast(payload);

  self.registration.showNotification(title, {
    body,
    data: payload.data ?? {},
  });
});

self.addEventListener('notificationclick', (event) => {
  event.notification.close();
  event.waitUntil(
    self.clients.matchAll({ type: 'window', includeUncontrolled: true }).then((clientList) => {
      if (clientList.length > 0) {
        return clientList[0].focus();
      }

      return self.clients.openWindow('/');
    })
  );
});

async function broadcast(payload) {
  const clients = await self.clients.matchAll({ type: 'window', includeUncontrolled: true });
  for (const client of clients) {
    client.postMessage({ type: 'push-message', payload });
  }
}
