import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import {
  LoaderFunctionArgs,
  useLoaderData,
  useLocation,
} from "react-router-dom";
import { userMock } from "../utils/mocks/userMock";
import { useState } from "react";
import UseLiveQuizConnection from "../hooks/UseLiveQuizConnection";
import { config } from "../config";
import LiveQuizAdminView from "../Components/LiveQuizzes/LiveQuizAdminView";
import LiveQuizUserView from "../Components/LiveQuizzes/LiveQuizUserView";

interface PageProps {
  params: { id: string };
}

export default function LiveQuizPage() {
  const { connection, isAdmin, connectedUsers, startQuiz, exitQuiz } =
    UseLiveQuizConnection();
  if (!connection && config.useBackend) {
    return <p>Loading...</p>;
  }
  return (
    <>
      {isAdmin ? (
        <LiveQuizAdminView
          connectedUsers={connectedUsers}
          connection={connection}
          startQuiz={startQuiz}
          exitQuiz={exitQuiz}
        />
      ) : (
        <LiveQuizUserView exitQuiz={exitQuiz} />
      )}
    </>
  );
}
