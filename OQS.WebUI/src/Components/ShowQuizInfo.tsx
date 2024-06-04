import axios from "../utils/axios-service";
import React, {useState, useEffect} from "react";
import {config} from "../config";
import {Result} from "../utils/types/result";
import {activeQuizMock} from "../utils/mocks/activeQuizMock.ts";
import {openSnackbar} from "../redux/Snackbar/SnackbarState.ts";
import {useDispatch, useSelector} from "react-redux";
import {useNavigate} from "react-router-dom";
import {RootState} from "../redux/store.ts";

type CreateActiveQuizRequest = {
    quizId: string;
};

type CreateLiveQuizRequest = {
    quizId: string;
    userId: string;
};

type QuizDetails = {
    createdAt?: string;
    timeLimitMinutes?: number;
    imageUrl?: string;
    description?: string;
    id?: string;
    language?: string;
    name?: string;
};

type ErrorQuiz = {
    message?: string;
};

function ShowQuizInfo({quizId}) {
    const [quizData, setQuizData] = useState<QuizDetails>({});

    const userState = useSelector((state: RootState) => state.user);

    const [error, setError] = useState<ErrorQuiz>({});
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const user = useSelector((state: RootState) => state.user.user);

    const fetchQuiz = async () => {
        try {
            const response = await fetch(
                `http://localhost:5276/api/quizzes/${quizId}`
            );
            if (!response.ok) {
                throw new Error("Failed to fetch Quiz");
            }

            const data = await response.json();
            console.log("Received data:", data);
            setQuizData(data.value);
        } catch (error) {
            console.error("Error fetching quiz:", error);
            // setError(error);
        }
    };

    useEffect(() => {
        if (!quizId) return;
        fetchQuiz();
    }, [quizId]);

    const startQuiz = async () => {
        const body: CreateActiveQuizRequest = {
            quizId,
        };

        const responseWrapped = await axios.post(`/api/active-quizzes`, body);
        console.log("foooooo", responseWrapped);

        const response = responseWrapped.data as Result<string>;
        if (response.isFailure) {
            dispatch(
                openSnackbar({
                    message: response.error?.message as string,
                    severity: "error",
                })
            );
        } else {
            dispatch(
                openSnackbar({
                    message: `The quiz has started!`,
                    severity: "success",
                })
            );
            navigate(`/active-quizzes/${response.value}`);
        }
    };

    const startLiveQuiz = async () => {
        const body: CreateLiveQuizRequest = {
            quizId,
            userId: user?.id as string,
        };
        console.log(body);
        const responseWrapped = await axios.post(`/api/live-quizzes`, body);

        const response = responseWrapped.data as Result<string>;
        if (response.isFailure) {
            dispatch(
                openSnackbar({
                    message: response.error?.message as string,
                    severity: "error",
                })
            );
        } else {
            dispatch(
                openSnackbar({
                    message: `Live quiz created! Your code is ${response.value}`,
                    severity: "success",
                })
            );
            navigate(`/live-quizzes/${response.value}`);
        }
    };

    const startQuizMock = () => {
        navigate(`/active-quizzes/${activeQuizMock.value?.id}`);
    };

    const startLiveQuizMock = () => {
        navigate(`/live-quizzes/${activeQuizMock.value?.id}`);
    };
    const handleStart = () => {
        config.useBackend ? startQuiz() : startQuizMock();
    };

    const handleStartLive = () => {
        config.useBackend ? startLiveQuiz() : startLiveQuizMock();
    };

    return (
        <div className="container mx-auto p-6">
            {error && quizData ? (
                <p className="text-red-500 text-center">{error.message}</p>
            ) : null}
            {quizData ? (
                <div className="bg-[#1c4e4f] p-8 rounded-lg shadow-md text-white mt-10 flex-col flex">
                    <div className="font-bold text-5xl capitalize mb-12 text-center p-4 text-[#DEAE9F]">
                        {quizData.name}
                    </div>
                    <div className="lg:flex justify-center items-center">
                        <div className="lg:w-1/2 lg:pr-8">
                            <img
                                src={
                                    quizData.imageUrl ||
                                    "https://www.shutterstock.com/shutterstock/photos/2052894734/display_1500/stock-vector-quiz-and-question-marks-trivia-night-quiz-symbol-neon-sign-night-online-game-with-questions-2052894734.jpg"
                                }
                                alt="Quiz Image"
                                className="w-full h-full object-cover rounded-lg"
                            />
                        </div>
                        <div className="lg:w-1/2 lg:pl-8 text-center">
                            <div className="mt-5">
                                <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">
                                    ID:
                                </div>
                                <p className="text-lg">{quizData.id}</p>
                            </div>
                            <div className="mt-5">
                                <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">
                                    Language:
                                </div>
                                <p className="text-lg">{quizData.language || "English"}</p>
                            </div>
                            <div className="mt-5">
                                <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">
                                    Time Limit (Minutes):
                                </div>
                                <p className="text-lg">{quizData.timeLimitMinutes}</p>
                            </div>
                            <div className="mt-5">
                                <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">
                                    Created At:
                                </div>
                                <p className="text-lg">
                                    {new Date(
                                        quizData.createdAt || "2024-06-02T20:57:29.6568126"
                                    ).toLocaleString()}
                                </p>
                            </div>
                            <div className="mt-10">
                                <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">
                                    Description:
                                </div>
                                <p className="text-lg">{quizData.description}</p>
                            </div>


                            {userState.isLogged && (
                                <div>
                                    <button
                                        onClick={handleStart}
                                        className="bg-[#DEAE9F] hover:bg-[#a49e97] text-white font-bold py-2 px-4 rounded mt-10 mr-10"
                                    >
                                        Start Quiz
                                    </button>
                                    <button
                                        onClick={handleStartLive}
                                        className="bg-[#DEAE9F] hover:bg-[#a49e97] text-white font-bold py-2 px-4 rounded mt-10"
                                    >
                                        Start Live Quiz
                                    </button>
                                </div>)}

                        </div>
                    </div>
                </div>
            ) : (
                <div className="bg-red-200 p-8 rounded-lg shadow-md text-white mt-10 text-center">
                    <h2 className="text-red-800 text-center text-lg">Quiz not found</h2>
                </div>
            )}
        </div>
    );
}

export default ShowQuizInfo;
