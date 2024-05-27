import { useEffect } from "react";
import { useCookies } from "react-cookie";
import { Outlet } from "react-router-dom";
import { config } from "../config";
import { userMock } from "../utils/mocks/userMock";

export default function Root() {
  const [cookies, setCookie, removeCookie] = useCookies();
  useEffect(() => {
    if (!config.useAuth) {
      setCookie("userId", userMock.id);
    }
  }, [setCookie]);
  return <Outlet />;
}
