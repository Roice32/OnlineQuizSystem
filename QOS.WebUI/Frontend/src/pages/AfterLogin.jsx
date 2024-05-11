import React, { useEffect, useState } from 'react';

function TokenPage() {
  // Definim un state pentru a stoca token-ul JWT
  const [authToken, setAuthToken] = useState('');

  // Folosim useEffect pentru a extrage token-ul din localStorage la încărcarea paginii
  useEffect(() => {
    // Extragem token-ul JWT din localStorage
    const token = localStorage.getItem('authToken');
    // Setăm token-ul în state
    setAuthToken(token);
  }, []); // [] pentru a rula efectul doar la încărcarea paginii

  return (
    <div>
      <h1>Token JWT</h1>
      <div>
        <p>Token-ul JWT salvat în localStorage este:</p>
        <pre>{authToken}</pre>
      </div>
    </div>
  );
}

export default TokenPage;
