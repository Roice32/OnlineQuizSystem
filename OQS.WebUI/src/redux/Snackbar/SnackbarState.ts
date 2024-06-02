import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface SnackbarData {
    message: string;
    severity: "error" | "info" | "success" | "warning";

}

export interface SnackbarState {
    open: boolean;
    data: SnackbarData | null;
}


const initialState:SnackbarState = {
    open:false,
    data:null
};

const snackbarSlice = createSlice({
  name: "snackbar",
  initialState,
  reducers: {
    openSnackbar: (state:SnackbarState,action:PayloadAction<SnackbarData>) => {
        state.open = true;
        state.data = action.payload;
    },
    closeSnackbar: (state) => {
        state.open = false;

    },
  },
});

export const { openSnackbar, closeSnackbar } = snackbarSlice.actions;

export default snackbarSlice.reducer;