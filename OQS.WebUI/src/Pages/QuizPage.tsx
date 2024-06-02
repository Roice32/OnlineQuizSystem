/* eslint-disable @typescript-eslint/no-unused-vars */

import { Link, useLoaderData, useNavigate } from "react-router-dom";
import { Quiz } from "../utils/types/quiz";
import axios from "../utils/axios-service";
import { GetAllQuizzesResponse } from "../utils/types/get-all-quizzes-response";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../redux/store";
import { useEffect, useState } from "react";
import { userMock, userMock2 } from "../utils/mocks/userMock";
import { User } from "../utils/types/user";
import { useCookies } from "react-cookie";
import { setUser } from "../redux/User/UserState";
import { CustomModal } from "../Components/Reusable/CustomModal";
import { Button, Input, InputLabel, Stack } from "@mui/material";
import { clearExpiredActiveQuizzes } from "../redux/ActiveQuiz/ActiveQuizzesState";

export const QuizzesLoader = async (): Promise<Quiz[]> => {
  const response = (await axios.get("/api/quizzes?offset=0&limit=10", {}))
    .data as GetAllQuizzesResponse;
  return response.quizzes as Quiz[];
};

export default function QuizPage() {
  const quizzes = useLoaderData() as Quiz[];
  const activeQuizzes = useSelector((state: RootState) => state.activeQuizzes);
  const navigate = useNavigate();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [quizCode, setQuizCode] = useState("");
  const dispatch = useDispatch();
  const [cookies, setCookie, removeCookie] = useCookies();
  const { user, isLogged } = useSelector((state: RootState) => state.user);

  const handleModalSubmit = async () => {
    if (quizCode) {
      navigate(`/live-quiz/${quizCode}`);
      setIsModalOpen(false);
    }
  };

  const handleOpenModal = () => {
    setIsModalOpen(true);
  };

  useEffect(() => {
    dispatch(clearExpiredActiveQuizzes());
  }, []);
  /*   const logIn = (user: User) => {
    setCookie("userId", user.id);
    setCookie("token", user.token);
    dispatch(setUser(user));
  }; */

  return (
    <div>
      <h1>Quiz</h1>
      {/*  <button onClick={() => logIn(userMock)}>User1</button>
      <button onClick={() => logIn(userMock2)}>User2</button> */}
      <ul>
        {quizzes.map((quiz) => (
          <li key={quiz.id}>
            <Link to={`/quizzes/${quiz.id}`} key={quiz.id}>
              {quiz.name}
            </Link>
          </li>
        ))}
      </ul>
      {isLogged && <button onClick={handleOpenModal}>Enter Quiz Code</button>}

      <CustomModal
        title={"Enter Quiz Code"}
        open={isModalOpen}
        handleClose={() => setIsModalOpen(false)}
      >
        <Stack spacing={2} direction="column">
          <Input
            type="text"
            value={quizCode}
            onChange={(e) => setQuizCode(e.target.value)}
          />
          <Stack spacing={2} direction="row" justifyContent={"space-evenly"}>
            <Button
              color="success"
              variant="outlined"
              onClick={handleModalSubmit}
            >
              Submit
            </Button>
            <Button
              color="error"
              variant="outlined"
              onClick={() => setIsModalOpen(false)}
            >
              Cancel
            </Button>
          </Stack>
        </Stack>
      </CustomModal>
    </div>
  );
}
