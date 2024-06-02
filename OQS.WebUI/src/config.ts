type Config = {
    useBackend: boolean;
    useAuth: boolean;
    backendURL: string;
}

export const config: Config = {
    useBackend: true,
    useAuth: false,
    backendURL: 'http://localhost:5276'
}