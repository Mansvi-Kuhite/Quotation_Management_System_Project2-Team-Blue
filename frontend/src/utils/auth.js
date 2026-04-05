export function decodeJwt(token) {
  try {
    const payloadBase64 = token.split(".")[1];
    const payload = JSON.parse(atob(payloadBase64));
    return payload;
  } catch {
    return null;
  }
}

export function extractUserFromToken(token) {
  const payload = decodeJwt(token);
  if (!payload) return null;

  const role =
    payload.role ||
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
    "";
  const username =
    payload.unique_name ||
    payload.name ||
    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ||
    "";

  return { username, role, payload };
}

export function isTokenExpired(token) {
  const payload = decodeJwt(token);
  if (!payload?.exp) return true;

  const nowInSeconds = Math.floor(Date.now() / 1000);
  return payload.exp <= nowInSeconds;
}
