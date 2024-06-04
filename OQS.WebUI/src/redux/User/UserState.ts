import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { User } from "../../utils/types/user";

export interface UserState {
    isLogged: boolean;
    user: User | null;
    authDeadline:string|null;
}

const initialState: UserState = {
    isLogged: false,
    user: null,
    authDeadline:null,
};

const userSlice = createSlice({
    name: "user",
    initialState,
    reducers: {
        setUser: (state, action: PayloadAction<User>) => {
        return {
            isLogged: true,
            user: action.payload,
            authDeadline: (new Date(new Date().getTime() + 1000 * 60 * 60 * 24*6)).toISOString(),
        };
        
        },
        clearUser: (state) => {
            return {
                isLogged: false,
                user: null,
                authDeadline: null,
            };
        },
        updateUser: (state, action: PayloadAction<Partial<User>>) => {
            if (state.user) {
                state.user = { ...state.user, ...action.payload };
            }
        }
        },
        
    },
    );

    export const { setUser, clearUser, updateUser } = userSlice.actions;
    export default userSlice.reducer;

