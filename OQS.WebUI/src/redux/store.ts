import {configureStore,combineReducers} from "@reduxjs/toolkit"
import activeQuizzesReducer from "./ActiveQuiz/ActiveQuizzesState";
import snackbarReducer from "./Snackbar/SnackbarState";
import storage from "redux-persist/lib/storage";
import { persistReducer } from "redux-persist";
import  userReducer  from "./User/UserState";
import connectionSlice from "./Connection/ConnectionState";

const persistConfig = {
    key: 'root',
    storage,
    version: 1,
}

const reducer=combineReducers({
    activeQuizzes:activeQuizzesReducer,
    snackbar:snackbarReducer,
    user:userReducer,
    connection:connectionSlice,
})

const persistedReducer = persistReducer(persistConfig, reducer);


  export const store = configureStore({
    reducer: persistedReducer,

});
  

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;