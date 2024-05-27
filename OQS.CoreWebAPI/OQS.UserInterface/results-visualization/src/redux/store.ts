import {configureStore,combineReducers} from "@reduxjs/toolkit"
import activeQuizzesReducer from "./ActiveQuiz/ActiveQuizzesState";
import storage from "redux-persist/lib/storage";
import { persistReducer } from "redux-persist";

const persistConfig = {
    key: 'root',
    storage,
    version: 1,
}

const reducer=combineReducers({
    activeQuizzes:activeQuizzesReducer
})

const persistedReducer = persistReducer(persistConfig, reducer);


  export const store = configureStore({
    reducer: persistedReducer,

});
  

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;