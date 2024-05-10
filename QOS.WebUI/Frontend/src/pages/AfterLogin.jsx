import React from 'react';

const AfterLogin = () => {
    const style = {
        backgroundColor: 'pink', // Setează culoarea fundalului.
        height: '100vh', // Întreaga înălțime a paginii.
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
    };

    return (
        <div style={style}>
            <h1>after login....</h1>
        </div>
    );
};

export default AfterLogin;