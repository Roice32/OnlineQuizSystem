import { RouterProvider } from "react-router-dom";
import { Provider } from "react-redux";
import { store } from "./redux/store";
import { router } from "./Router/router";
import { PersistGate } from "redux-persist/integration/react";
import { persistStore } from "redux-persist";
import { CookiesProvider } from "react-cookie";
function App() {
  const persistor = persistStore(store);
  return (
    <>
      <CookiesProvider>
        <Provider store={store}>
          <PersistGate persistor={persistor}>
            <RouterProvider router={router} />
          </PersistGate>
        </Provider>
      </CookiesProvider>
    </>
  );
}

export default App;
