import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { User } from "../../utils/types/user";

export interface UserState {
    isLogged: boolean;
    user: User | null;
}

const initialState: UserState = {
    isLogged: false,
    user: null,
};

const userSlice = createSlice({
    name: "user",
    initialState,
    reducers: {
        setUser: (state, action: PayloadAction<User>) => {
        return {
            isLogged: true,
            user: action.payload,
        };
        
        },
        clearUser: (state) => {
            return {
                isLogged: false,
                user: null,
            };
        }
        },
        
    },
    );

    export const { setUser, clearUser } = userSlice.actions;
    export default userSlice.reducer;

