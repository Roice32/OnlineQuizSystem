import { HubConnection } from "@microsoft/signalr";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface ConnectionState {
    connection: HubConnection | null;
}

export const initialState: ConnectionState = {
    connection: null,
};

const connectionSlice = createSlice({
    name: "connection",
    initialState,
    reducers: {
        setConnection(state, action: PayloadAction<HubConnection>) {
            state.connection = action.payload;
        },
        disconnect(state) {
            /* console.log("Disconnecting"); */
            state.connection?.stop();
            state.connection = null;
        }
    },
});

export const { setConnection, disconnect } = connectionSlice.actions;
export default connectionSlice.reducer;