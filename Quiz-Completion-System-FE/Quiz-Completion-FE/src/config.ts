type Config = {
    useBackend: boolean;
    useAuth: boolean;
    backendURL: string;
}

export const config: Config = {
    useBackend: false,
    useAuth: false,
    backendURL: 'http://localhost:5276'
}